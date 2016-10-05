namespace TrackingService
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.Courier.Contracts;


    public class RoutingSlipActivityConsumer :
        IConsumer<RoutingSlipActivityCompleted>
    {
        private readonly string activityName;

        private readonly RoutingSlipMetrics metrics;

        public RoutingSlipActivityConsumer(RoutingSlipMetrics metrics, string activityName)
        {
            this.metrics = metrics;
            this.activityName = activityName;
        }

        public async Task Consume(ConsumeContext<RoutingSlipActivityCompleted> context)
        {
            if (context.Message.ActivityName.Equals(this.activityName, StringComparison.OrdinalIgnoreCase))
                this.metrics.AddComplete(context.Message.Duration);
        }
    }
}