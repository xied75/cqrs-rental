namespace Rental
{
    using System;

    public class RentOrder
    {
        public Guid Id { get; set; }

        public string CarReg { get; set; }

        public DateTime RentingStart { get; set; }

        public DateTime RentingEnd { get; set; }

        public string CustomerName { get; set; }
    }
}