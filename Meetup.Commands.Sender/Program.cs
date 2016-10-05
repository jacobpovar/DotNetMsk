using System;
using System.Threading.Tasks;

namespace Meetup.Commands.Sender
{
    using MassTransit;

    using Meetup.Commands.Contracts;

    class Program
    {
        static void Main(string[] args)
        {
            Run().Wait();
        }

        private static async Task Run()
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(
                configurator =>
                {
                    var uri = "rabbitmq://localhost";

                    configurator.Host(
                        new Uri(uri),
                        hostConfigurator =>
                        {
                            hostConfigurator.Username("server");
                            hostConfigurator.Password("server");
                        });
                });

            bus.Start();

            Console.WriteLine("Bus started");

            Console.ReadKey();
            
            var sendEndpoint = await bus.GetSendEndpoint(new Uri("rabbitmq://localhost/MskDotNet.Consumer.InputQueue"));
            await sendEndpoint.Send(new CreateUserCommand { Name = "user" });

            Console.WriteLine("Sent command");

            Console.ReadLine();
        }
    }
}
