namespace Processing.Activities.Retrieve
{
    using System;

    public interface ReservationLog
    {
        Guid ReservationId { get; }

        Guid RequestId { get; }
    }
}