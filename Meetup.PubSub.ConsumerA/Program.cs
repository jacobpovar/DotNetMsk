using System;
using System.Threading.Tasks;

namespace Meetup.PubSub.ConsumerA
{
    using MassTransit;

    using Meetup.PubSub.Messages;

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

                    configurator.ReceiveEndpoint(rabbitMqHost, "MskDotNet.HelloWorld.InputQueue",
                        endpointConfigurator =>
                        {
                            endpointConfigurator.Consumer<SayHiCommandHandler>(); 
                        });
                });

            bus.Start();
            
            Console.WriteLine("Consumer started");

            Console.ReadLine();
        }
    }

    internal class SayHiCommandHandler : IConsumer<SayHi>
    {
        public Task Consume(ConsumeContext<SayHi> context)
        {
            return Console.Out.WriteLineAsync($"From consumer: hi, {context.Message.Name}");
        }
    }
}
