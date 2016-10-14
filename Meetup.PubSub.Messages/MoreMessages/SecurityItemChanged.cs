using System;

namespace Meetup.PubSub.Messages.MoreMessages
{
    public interface SecurityItemChanged
    {
        DateTime Timestamp { get; }
    }

    public interface GroupChanged : SecurityItemChanged
    {
        Guid UserId { get; }
    }

    public interface PasswordChanged : SecurityItemChanged
    {
        Guid UserId { get; }
    }
}