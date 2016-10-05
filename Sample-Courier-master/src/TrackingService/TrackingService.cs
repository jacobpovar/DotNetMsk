namespace TrackingService
{
    using System;
    using System.Configuration;
    using Automatonymous;
    using MassTransit;
    using MassTransit.NHibernateIntegration.Saga;
    using MassTransit.RabbitMqTransport;
    using MassTransit.Saga;
    using NHibernate;
    using Topshelf;
    using Topshelf.Logging;
    using Tracking;

    internal class TrackingService : ServiceControl
    {
        private readonly LogWriter log = HostLogger.Get<TrackingService>();

        private RoutingSlipMetrics activityMetrics;

        private IBusControl busControl;

        private RoutingSlipStateMachine machine;

        private RoutingSlipMetrics metrics;

        private SqLiteSessionFactoryProvider provider;

        private ISagaRepository<RoutingSlipState> repository;

        private ISessionFactory sessionFactory;

        public bool Start(HostControl hostControl)
        {
            this.log.Info("Creating bus...");

            this.metrics = new RoutingSlipMetrics("Routing Slip");
            this.activityMetrics = new RoutingSlipMetrics("Validate Activity");

            this.machine = new RoutingSlipStateMachine();
            this.provider = new SqLiteSessionFactoryProvider(false, typeof(RoutingSlipStateSagaMap));
            this.sessionFactory = this.provider.GetSessionFactory();

            this.repository = new NHibernateSagaRepository<RoutingSlipState>(this.sessionFactory);

            this.busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                var host = x.Host(new Uri(ConfigurationManager.AppSettings["RabbitMQHost"]), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                x.UseSerilog();

                x.EnabledPerformanceCounters();

                x.ReceiveEndpoint(host, "routing_slip_metrics", e =>
                {
                    e.PrefetchCount = 100;
                    e.UseRetry(Retry.None);
                    e.Consumer(() => new RoutingSlipMetricsConsumer(this.metrics));
                });

                x.ReceiveEndpoint(host, "routing_slip_activity_metrics", e =>
                {
                    e.PrefetchCount = 100;
                    e.UseRetry(Retry.None);
                    e.Consumer(() => new RoutingSlipActivityConsumer(this.activityMetrics, "Validate"));
                });

                x.ReceiveEndpoint(host, "routing_slip_state", e =>
                {
                    e.PrefetchCount = 8;
                    e.UseConcurrencyLimit(1);
                    e.StateMachineSaga(this.machine, this.repository);
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