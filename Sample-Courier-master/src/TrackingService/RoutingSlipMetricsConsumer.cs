namespace TrackingService
{
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.Courier.Contracts;


    public class RoutingSlipMetricsConsumer :
        IConsumer<RoutingSlipCompleted>
    {
        private readonly RoutingSlipMetrics metrics;

        public RoutingSlipMetricsConsumer(RoutingSlipMetrics metrics)
        {
            this.metrics = metrics;
        }

        public async Task Consume(ConsumeContext<RoutingSlipCompleted> context)
        {
            this.metrics.AddComplete(context.Message.Duration);
        }
    }
}