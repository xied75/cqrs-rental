namespace Rental.Read.Model
{
    using System;
 
    public class RentedDates
    {
        public string Reg { get; set; }

        public decimal RentPerDay { get; set; }

        public DateTime RentingStart { get; set; }

        public DateTime RentingEnd { get; set; }
    }
}
