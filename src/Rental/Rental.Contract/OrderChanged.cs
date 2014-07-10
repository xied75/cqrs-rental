namespace Rental.Contract
{
    using System;
    using Infrastructure.Messaging;

    public class OrderChanged : IEvent
    {
        public Guid SourceId { get; set; }

        public DateTime RentingStart { get; set; }

        public DateTime RentingEnd { get; set; }
    }
}
