using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meetup.Commands.Consumer
{
    using MassTransit;

    using Meetup.Commands.Contracts;

    class Program
    {
        static void Main(string[] args)
        {
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
                    
                    configurator.ReceiveEndpoint(rabbitMqHost, "MskDotNet.Consumer.InputQueue",
                        endpointConfigurator =>
                        {
                            endpointConfigurator.Consumer<CreateUserCommandHandler>();

                            //endpointConfigurator.Handler<CreateUser>(context =>
                            //{
                            //    return Console.Out.WriteLineAsync("Handling without consumer class");
                            //});
                        });
                });

            bus.Start();

            Console.WriteLine("Consumer started");

            Console.ReadLine();
        }
    }

    internal class CreateUserCommandHandler : IConsumer<CreateUser>
    {
        public Task Consume(ConsumeContext<CreateUser> context)
        {
            return Console.Out.WriteLineAsync("creating new user");

        }
    }
}
