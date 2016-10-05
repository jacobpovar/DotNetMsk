namespace Meetup.Errors.Producer
{
    using System;
    using System.Threading.Tasks;

    using MassTransit;

    using Meetup.Errors.Contracts;

    public class Program
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

            Console.WriteLine("Producer bus started");

            int index = 0;
            while (true)
            {
                Console.ReadKey();

                var sendEndpoint = await bus.GetSendEndpoint(
                    new Uri("rabbitmq://localhost/MskDotNet.ErrorsSample.InputQueue"));
                await sendEndpoint.Send(new CallExternalProcessCommand(index++));

                Console.WriteLine("Sent command");
            }
            

            Console.ReadLine();
        }
    }

    internal class CallExternalProcessCommand : CallExternalProcess
    {
        public CallExternalProcessCommand(int index)
        {
            this.Index = index;
        }

        public int Index { get; }
    }
}
