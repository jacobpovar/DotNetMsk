namespace Client
{
    using System;
    using System.Configuration;

    using MassTransit;
    using MassTransit.Courier;
    using MassTransit.Courier.Contracts;
    using MassTransit.RabbitMqTransport;
    using Processing.Contracts;

    using Serilog;

    internal class Program
    {
        private static IRabbitMqHost _host;

        private static void Main()
        {
            var logger = new LoggerConfiguration().WriteTo.ColoredConsole().CreateLogger();

            Log.Logger = logger;
            
            var busControl = CreateBus();

            busControl.Start();

            var validateQueueName = ConfigurationManager.AppSettings["ValidateActivityQueue"];

            var validateAddress = _host.Settings.GetQueueAddress(validateQueueName);

            var retrieveQueueName = ConfigurationManager.AppSettings["RetrieveActivityQueue"];

            var retrieveAddress = _host.Settings.GetQueueAddress(retrieveQueueName);

            var confirmQueueName = "confirm";

            var confirmAddress = _host.Settings.GetQueueAddress(confirmQueueName);


            try
            {
                while (true)
                {
                    Console.Write("Enter seats to reserver (quit exits): ");
                    var requestAddress = Console.ReadLine();
                    if (requestAddress == "quit")
                        break;

                    int count;
                    if (!int.TryParse(requestAddress, out count))
                    {
                        Console.WriteLine("Invalid arg");
                        continue;
                    }

                    var builder = new RoutingSlipBuilder(NewId.NextGuid());

                    builder.AddActivity("Validate", validateAddress);
                    builder.AddActivity("Retrieve", retrieveAddress);
                    builder.AddActivity("Confirm", confirmAddress);

                    builder.SetVariables(new
                    {
                        RequestId = NewId.NextGuid(),
                        GuestsCount = count
                    });

                    var routingSlip = builder.Build();

                    busControl.Publish<RoutingSlipCreated>(new
                    {
                        TrackingNumber = routingSlip.TrackingNumber,
                        Timestamp = routingSlip.CreateTimestamp,
                    }).Wait();

                    busControl.Execute(routingSlip).Wait();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception!!! OMG!!! {0}", ex);
                Console.ReadLine();
            }
            finally
            {
                busControl.Stop();
            }
        }

        private static IBusControl CreateBus()
        {
            return Bus.Factory.CreateUsingRabbitMq(x =>
            {
                _host = x.Host(new Uri(ConfigurationManager.AppSettings["RabbitMQHost"]), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                x.UseSerilog();
            });
        }

    }
}