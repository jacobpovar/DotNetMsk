namespace Processing.Activities.Retrieve
{
    using System;


    public interface MakeReservationArguments
    {
        /// <summary>
        /// The requestId for eventing
        /// </summary>
        Guid RequestId { get; }

        int GuestsCount { get; }
    }
}