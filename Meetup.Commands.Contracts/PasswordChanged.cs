using System;

namespace Meetup.Commands.Contracts
{
    public interface PasswordChanged : SecurityItemChanged
    {
        Guid UserId { get; }
    }

    public interface GroupChanged : SecurityItemChanged
    {
        Guid UserId { get; }
    }

    public interface SecurityItemChanged
    {
        DateTime Timestamp { get; }
    }
}