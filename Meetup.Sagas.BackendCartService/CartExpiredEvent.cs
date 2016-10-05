using System;
using Meetup.Sagas.Contracts;

namespace Meetup.Sagas.BackendCartService
{
    internal class CartExpiredEvent : CartExpired
    {
        private readonly ShoppingCart instance;

        public CartExpiredEvent(ShoppingCart instance)
        {
            this.instance = instance;
        }

        public Guid CartId => this.instance.CorrelationId;
    }
}