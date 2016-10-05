using System;
using System.Threading.Tasks;
using MassTransit;
using Meetup.PubSub.Messages;

namespace Meetup.PubSub.ConsumerB
{
    class Program
    {
        static void Main(string[] args)
        {
            IBusControl bus = Bus.Factory.CreateUsingRabbitMq(
                configurator =>
                {
                    var rabbitMqHost = configurator.Host(
                        new Uri("rabbitmq://localhost"),
                        hostConfigurator =>
                        {
                            hostConfigurator.Username("server");
                            hostConfigurator.Password("server");
                        });

                    configurator.ReceiveEndpoint(rabbitMqHost, "MskDotNet.PubSub.InputQueue_OtherStuff",
                        endpointConfigurator =>
                        {
                            endpointConfigurator.Consumer<CreateUserCommandHandler>();
                        });
                });

            bus.Start();

            Console.WriteLine("Consumer started");

            Console.ReadLine();
        }
    }

    internal class CreateUserCommandHandler : IConsumer<SayHi>
    {
        public Task Consume(ConsumeContext<SayHi> context)
        {
            return Console.Out.WriteLineAsync("Consumer B: doing other usefull stuff!");
        }
    }
}
