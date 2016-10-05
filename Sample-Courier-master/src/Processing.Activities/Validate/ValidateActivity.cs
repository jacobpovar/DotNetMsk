namespace Processing.Activities.Validate
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using MassTransit.Courier;
    using MassTransit.Logging;

    public class ValidateActivity : ExecuteActivity<ValidateArguments>
    {
        private static readonly ILog Log = Logger.Get<ValidateActivity>();

        public async Task<ExecutionResult> Execute(ExecuteContext<ValidateArguments> context)
        {
            var address = context.Arguments.GuestsCount;

            if (address < 1 || address > 10)
            {
                await context.Publish<RequestRejected>(new
                {
                    context.Arguments.RequestId,
                    context.TrackingNumber,
                    Timestamp = DateTime.UtcNow,
                    ReasonText = "Invalid guests count",
                });

                Serilog.Log.Error("Request {id} rejected", context.Arguments.RequestId);

                return context.Terminate();
            }

            Log.InfoFormat("Validated Request {0} with count {1}", context.Arguments.RequestId,
                    context.Arguments.GuestsCount);

            return context.Completed();
        }
    }
}