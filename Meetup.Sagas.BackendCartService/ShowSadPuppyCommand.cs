using Meetup.Sagas.Contracts;

namespace Meetup.Sagas.BackendCartService
{
    public class ShowSadPuppyCommand : ShowSadPuppy
    {
        public ShowSadPuppyCommand(string username)
        {
            this.Username = username;
        }

        public string Username { get; }
    }
}