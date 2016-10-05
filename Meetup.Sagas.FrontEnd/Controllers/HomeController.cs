using System;
using System.Threading.Tasks;
using MassTransit;
using Meetup.Sagas.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Meetup.Sagas.FrontEnd.Controllers
{
    public class HomeController : Controller
    {
        private string userName = "DotNetLover";

        private readonly IBus bus;

        public HomeController(IBus bus)
        {
            this.bus = bus;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public async Task<IActionResult> AddItem()
        {
            await bus.Publish(new ItemAddedEvent(this.userName));

            return this.View("Added");
        }

        public async Task<IActionResult> Checkout()
        {
            await bus.Publish(new CheckedOutEvent(this.userName));

            return this.View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }

    internal class CheckedOutEvent : CheckedOut
    {
        public CheckedOutEvent(string username)
        {
            Username = username;

            this.Timestamp = DateTime.UtcNow;
        }

        public string Username { get; }

        public DateTime Timestamp { get; }
    }

    internal class ItemAddedEvent : ItemAdded
    {
        public ItemAddedEvent(string username)
        {
            Username = username;
            this.Timestamp = DateTime.UtcNow;
        }

        public string Username { get; }

        public DateTime Timestamp { get; }
    }
}
