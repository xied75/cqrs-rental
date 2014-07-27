namespace Rental.Read.Handlers
{
    using System.Linq;
    using Infrastructure.EventSourcing;
    using Infrastructure.Messaging.Handling;
    using Rental.Read.Command;

    public class MakeBookingHandler : ICommandHandler<MakeBooking>
    {
        public void Handle(MakeBooking command)
        {
            throw new System.NotImplementedException();
        }
    }
}