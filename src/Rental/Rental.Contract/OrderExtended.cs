namespace Rental.Contract
{
    using System;
    using Infrastructure.Messaging;

    public class OrderExtended : IEvent
    {
        public Guid SourceId { get; set; }

        public DateTime RentingEnd { get; set; }

        public decimal RentPerDay { get; set; }
    }
}
