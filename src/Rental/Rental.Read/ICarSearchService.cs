namespace Rental.Read
{
    using System;
    using System.Collections.Generic;

    using Rental.Read.Model;

    public interface ICarSearchService : IDisposable
    {
        IEnumerable<CarView> GetCarViewList();

        IEnumerable<CarView> GetCarViewListByModel(string model);

        CarView GetCarViewByReg(string reg);

        IEnumerable<RentedDates> GetCarRentedDatesByModel(string model);

        void Dispose();
    }
}
