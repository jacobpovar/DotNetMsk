using System.Threading.Tasks;

namespace Processing.Activities.Confirmation
{
    using MassTransit.Courier;

    using Serilog;

    public class ConfirmActivity : ExecuteActivity<ConfirmActivityArguments>
    {
        public Task<ExecutionResult> Execute(ExecuteContext<ConfirmActivityArguments> context)
        {
            if (context.Arguments.GuestsCount == 7)
            {
                Log.Error("Error in reservation!");
                return Task.FromResult(context.Faulted());
            }

            Log.Information("Finishing reservation");

            return Task.FromResult(context.Completed());
        }
    }

    public interface ConfirmActivityArguments
    {
        int GuestsCount { get; }
    }
}
