namespace Rental.Read.Command
{
    using System;
    using Infrastructure.Messaging;

    public class ExtendBooking : ICommand
    {
        public ExtendBooking()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public DateTime RentingEnd { get; set; }

        public decimal RentPerDay { get; set; }
    }
}
