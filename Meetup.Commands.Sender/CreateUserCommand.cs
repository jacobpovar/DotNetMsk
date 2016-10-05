namespace Meetup.Commands.Sender
{
    using Meetup.Commands.Contracts;

    public class CreateUserCommand : CreateUser
    {
        public string Name { get; set; }
    }
}