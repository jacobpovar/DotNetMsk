using System;

namespace Meetup.Sagas.Contracts
{
    public interface CartExpired
    {
        Guid CartId { get; }
    }
}
