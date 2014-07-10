namespace Rental.Read.Command
{
    using System;
    using Infrastructure.Messaging;

    public class CancelBooking : ICommand
    {
        public CancelBooking()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid OrderId { get; set; }
    }
}
