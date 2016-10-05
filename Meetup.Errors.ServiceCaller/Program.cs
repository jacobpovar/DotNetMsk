using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Serilog;

namespace Meetup.Errors.ServiceCaller
{
    using MassTransit;
    using MassTransit.Policies;

    class Program
    {
        static void Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.ColoredConsole()
                .CreateLogger();

            Log.Logger = logger;

            var bus = Bus.Factory.CreateUsingRabbitMq(
                configurator =>
                {
                    var rabbitMqHost = configurator.Host(
                        new Uri("rabbitmq://localhost"),
                        hostConfigurator =>
                        {
                            hostConfigurator.Username("server");
                            hostConfigurator.Password("server");
                        });

                    configurator.ReceiveEndpoint(rabbitMqHost, "MskDotNet.ErrorsSample.InputQueue",
                        endpointConfigurator =>
                        {
                            endpointConfigurator.Consumer<CallExternalProcessCommandHandler>();

                            //endpointConfigurator.UseRetry(Retry.Incremental(3,
                            //    TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)));

                            endpointConfigurator.UseCircuitBreaker(
                                breakerConfigurator =>
                                    {
                                        breakerConfigurator.TrackingPeriod = TimeSpan.FromSeconds(20);
                                        breakerConfigurator.TripThreshold = 15;
                                        breakerConfigurator.ActiveThreshold = 10;
                                        breakerConfigurator.ResetInterval(TimeSpan.FromMinutes(1));
                                    });
                        });

                    configurator.UseSerilog(logger);
                });

            bus.Start();

            Console.WriteLine("Consumer started");

            Console.ReadLine();
        }
    }
}
