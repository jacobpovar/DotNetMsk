namespace TrackingService
{
    using MassTransit.NHibernateIntegration;
    using Tracking;


    public class RoutingSlipStateSagaMap :
        SagaClassMapping<RoutingSlipState>
    {
        public RoutingSlipStateSagaMap()
        {
            this.Property(x => x.State);

            this.Property(x => x.CreateTime);
            this.Property(x => x.StartTime);
            this.Property(x => x.EndTime);
            this.Property(x => x.Duration);
        }
    }
}