using System;

namespace Meetup.Sagas.Contracts
{
    public interface ItemAdded
    {
        string Username { get; }

        DateTime Timestamp { get; }
    }

    public interface CheckedOut
    {
        string Username { get; }

        DateTime Timestamp { get; }
    }
    
    public interface ShowSadPuppy
    {
        string Username { get; }
    }
}
