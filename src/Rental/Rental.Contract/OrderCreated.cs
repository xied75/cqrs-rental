namespace Rental.Contract
{
    using System;
    using Infrastructure.Messaging;

    public class OrderCreated : IEvent
    {
        public Guid SourceId { get; set; }

        public string Reg { get; set; }

        public DateTime RentingStart { get; set; }

        public DateTime RentingEnd { get; set; }

        public decimal RentPerDay { get; set; }

        public string CustomerName { get; set; }
    }
}
