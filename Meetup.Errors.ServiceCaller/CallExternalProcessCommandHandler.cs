using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Meetup.Errors.Contracts;
using Serilog;

namespace Meetup.Errors.ServiceCaller
{
    internal class CallExternalProcessCommandHandler : IConsumer<CallExternalProcess>
    {
        public async Task Consume(ConsumeContext<CallExternalProcess> context)
        {
            Log.Information("Calling external service {index}", context.Message.Index);

            var client = new HttpClient();
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            var result = await client.GetAsync("http://localhost:5000/api/faulty", cancellationTokenSource.Token);
            result.EnsureSuccessStatusCode();
            var response = await result.Content.ReadAsStringAsync();

            Log.Information("Received result : {response}", response);
        }
    }
}