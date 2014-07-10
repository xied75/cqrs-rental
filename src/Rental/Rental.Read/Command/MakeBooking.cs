namespace Rental.Read.Command
{
    using System;
    using Infrastructure.Messaging;

    public class MakeBooking : ICommand
    {
        public Guid Id { get; set; }

        public MakeBooking()
        {
            this.Id = Guid.NewGuid();
        }

        public string Reg { get; set; }

        public DateTime RentingStart { get; set; }

        public DateTime RentingEnd { get; set; }

        public decimal RentPerDay { get; set; }
    }
}
