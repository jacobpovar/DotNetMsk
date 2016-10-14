using System;

namespace Meetup.PubSub.Messages.MoreMessages
{
    public interface InvoiceCreated
    {
        string InvoiceId { get; }
    }

    public interface InvoiceCreatedV2
    {
        Guid InvoiceId { get; }

        DateTime Timestamp { get; }
    }


    internal class InvoiceCreatedEvent : InvoiceCreated, InvoiceCreatedV2
    {
        public InvoiceCreatedEvent(Guid invoiceId)
        {
            this.Timestamp = DateTime.Now;
            this.InvoiceId = invoiceId;
        }

        public DateTime Timestamp { get; }

        public Guid InvoiceId { get; }

        string InvoiceCreated.InvoiceId => this.InvoiceId.ToString();
    }
}