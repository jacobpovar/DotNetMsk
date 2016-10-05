using System;
using MassTransit;

namespace Meetup.PubSub.Sender
{
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
                   
               });

            bus.Start();

            bus.Publish(new SayHiEvent());

            Console.WriteLine("Published message!");
        }
    }

    public class SayHiEvent : SayHi
    {
    }
}
