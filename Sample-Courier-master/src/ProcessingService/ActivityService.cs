using Serilog;

namespace ProcessingService
{
    using System;
    using System.Configuration;
    using System.Threading;
    using MassTransit;
    using MassTransit.Courier;
    using MassTransit.Courier.Factories;
    using MassTransit.Log4NetIntegration;
    using MassTransit.RabbitMqTransport;

    using Processing.Activities.Confirmation;
    using Processing.Activities.Retrieve;
    using Processing.Activities.Validate;
    using Topshelf;
    using Topshelf.Logging;

    public class ActivityService : ServiceControl
    {
        private readonly LogWriter log = HostLogger.Get<ActivityService>();

        private IBusControl busControl;

        public bool Start(HostControl hostControl)
        {
            int workerThreads;
            int completionPortThreads;
            ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
            Console.WriteLine("Min: {0}", workerThreads);

            ThreadPool.SetMinThreads(200, completionPortThreads);

            this.log.Info("Creating bus...");

            this.busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                var host = x.Host(new Uri(ConfigurationManager.AppSettings["RabbitMQHost"]), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                x.UseSerilog(Log.Logger);

                ///////
                /// 
                x.ReceiveEndpoint(host, ConfigurationManager.AppSettings["ValidateActivityQueue"], e =>
                {
                    e.PrefetchCount = 100;
                    e.ExecuteActivityHost<ValidateActivity, ValidateArguments>(
                        DefaultConstructorExecuteActivityFactory<ValidateActivity, ValidateArguments>.ExecuteFactory);
                });
                
                var compQueue = ConfigurationManager.AppSettings["CompensateRetrieveActivityQueue"];

                var compAddress = host.Settings.GetQueueAddress(compQueue);

                x.ReceiveEndpoint(host, ConfigurationManager.AppSettings["RetrieveActivityQueue"], e =>
                {
                    e.PrefetchCount = 100;
                    //                    e.Retry(Retry.Selected<HttpRequestException>().Interval(5, TimeSpan.FromSeconds(1)));
                    e.ExecuteActivityHost<MakeReservationActivity, MakeReservationArguments>(compAddress);
                });

                x.ReceiveEndpoint(host, ConfigurationManager.AppSettings["CompensateRetrieveActivityQueue"],
                    e => e.CompensateActivityHost<MakeReservationActivity, ReservationLog>());

                x.ReceiveEndpoint(host, "confirm",
                    configurator =>
                        {
                            configurator.ExecuteActivityHost<ConfirmActivity, ConfirmActivityArguments>();
                        });
            });

            this.log.Info("Starting bus...");

            this.busControl.Start();

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            this.log.Info("Stopping bus...");

            this.busControl?.Stop();

            return true;
        }
    }
}