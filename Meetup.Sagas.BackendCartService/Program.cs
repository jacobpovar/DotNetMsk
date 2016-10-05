using System;
using Automatonymous;
using MassTransit;
using MassTransit.EntityFrameworkIntegration;
using MassTransit.EntityFrameworkIntegration.Saga;
using MassTransit.Saga;

namespace Meetup.Sagas.BackendCartService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(configurator =>
            {
                var host = configurator.Host(new Uri("rabbitmq://localhost"), hostConfigurator =>
                {
                    hostConfigurator.Username("guest");
                    hostConfigurator.Password("guest");
                });

                var saga = new ShoppingCartSaga();

                SagaDbContextFactory sagaDbContextFactory =
                    () => new SagaDbContext<ShoppingCart, ShoppingCartMap>("Data Source=.;Initial Catalog=SagasExample;Integrated Security=True");

                var repository = new Lazy<ISagaRepository<ShoppingCart>>(
                    () => new EntityFrameworkSagaRepository<ShoppingCart>(sagaDbContextFactory));

                configurator.ReceiveEndpoint(host, "SagaExample.ShoppingCarts", e =>
                {
                    e.PrefetchCount = 8;
                    e.StateMachineSaga(saga, repository.Value);
                });
                
                configurator.UseInMemoryScheduler();

                configurator.ReceiveEndpoint(host, "SagaExample.Puppies", e =>
                {
                    e.Consumer<ShowSadPuppyConsumer>();
                });
            });

            bus.Start();

            Console.WriteLine("Started cart service.");

            Console.ReadLine();
        }
    }
}
