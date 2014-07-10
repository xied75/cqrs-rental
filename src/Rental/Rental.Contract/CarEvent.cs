namespace Rental.Contract
{
    using System;
    using Infrastructure.Messaging;

    public class CarEvent : IEvent
    {
        public Guid SourceId { get; set; }

        public string Reg { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }
    }
}
