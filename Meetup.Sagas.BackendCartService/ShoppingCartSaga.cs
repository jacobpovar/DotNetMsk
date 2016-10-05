using System;
using Automatonymous;
using Meetup.Sagas.Contracts;

namespace Meetup.Sagas.BackendCartService
{
    public class ShoppingCartSaga : MassTransitStateMachine<ShoppingCart>
    {
        public ShoppingCartSaga()
        {
            this.InstanceState(x => x.CurrentState);
            
            this.Event(() => this.ItemAdded, x => x.CorrelateBy(cart => cart.UserName, context => context.Message.Username)
                .SelectId(context => Guid.NewGuid()));

            this.Event(() => this.CheckedOut, x => x.CorrelateBy((cart, context) => cart.UserName == context.Message.Username));
            
            this.Schedule(() => this.CartExpired, x => x.ExpirationId, x =>
            {
                x.Delay = TimeSpan.FromSeconds(10);
                x.Received = e => e.CorrelateById(context => context.Message.CartId);
            });


            this.Initially(
                this.When(this.ItemAdded)
                    .Then(context =>
                    {
                        context.Instance.Created = context.Data.Timestamp;
                        context.Instance.Updated = context.Data.Timestamp;
                        context.Instance.UserName = context.Data.Username;
                    })
                    .ThenAsync(
                        context =>
                            Console.Out.WriteLineAsync(
                                $"Created new saga: {context.Data.Username}, {context.Data.Timestamp}"))
                    .Schedule(this.CartExpired, context => new CartExpiredEvent(context.Instance))
                    .TransitionTo(this.Active));
            
            this.During(this.Active, 
                this.When(this.ItemAdded)
                    .Then(context =>
                    {
                        if (context.Data.Timestamp > context.Instance.Updated)
                        {
                            context.Instance.Updated = context.Data.Timestamp;
                        }
                    })
                    .ThenAsync(context => Console.Out.WriteLineAsync($"Updating saga with added item {context.Data.Timestamp}"))
                    //.Unschedule(this.CartExpired)
                    .Schedule(this.CartExpired, context => new CartExpiredEvent(context.Instance)),

                this.When(this.CheckedOut)
                    .Then(context =>
                    {
                        if (context.Data.Timestamp > context.Instance.Updated)
                        {
                            context.Instance.Updated = context.Data.Timestamp;
                        }
                    })
                    .ThenAsync(context => Console.Out.WriteLineAsync($"Completing with checkout cart, {context.Data.Timestamp}"))
                    .Unschedule(this.CartExpired)
                    .Finalize(), 

                this.When(this.CartExpired.Received)
                    .ThenAsync(context => Console.Out.WriteLineAsync("Received expired event!"))
                    .Publish(context => new ShowSadPuppyCommand(context.Instance.UserName))
                    //.TransitionTo(this.Expired)
                    .Finalize()
            );

            this.SetCompletedWhenFinalized();
        }
        
        public State Active { get; private set; }

        public State Expired { get; private set; }

        // + 2 состояние - Initial и Completed

        public Event<ItemAdded> ItemAdded { get; private set; }

        public Event<CheckedOut> CheckedOut { get; private set; }

        public Schedule<ShoppingCart, CartExpired> CartExpired { get; private set; }
    }
}