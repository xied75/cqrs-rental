namespace Rental.Read.Command
{
    using System;
    using Infrastructure.Messaging;

    public class ChangeBooking : ICommand
    {
        public ChangeBooking()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public DateTime RentingStart { get; set; }

        public DateTime RentingEnd { get; set; }
    }
}
