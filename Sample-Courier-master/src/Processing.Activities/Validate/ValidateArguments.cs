namespace Processing.Activities.Validate
{
    using System;
    
    public interface ValidateArguments
    {
        int GuestsCount { get; }

        /// <summary>
        /// The requestId for this activity argument
        /// </summary>
        Guid RequestId { get; }
    }
}