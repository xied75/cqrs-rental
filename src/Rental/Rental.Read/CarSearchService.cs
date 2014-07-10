namespace Rental.Read
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;

    using Dapper;
    using Rental.Read.Model;

    public class CarSearchService : ICarSearchService
    {
        private bool disposed = false;

        private SqlConnection conn;

        public CarSearchService(SqlConnection conn)
        {
            this.conn = conn;
        }

        public IEnumerable<CarView> GetCarViewList()
        {
            this.conn.Open();
            var results = this.conn.Query<CarView>(@"SELECT Reg, Make, Model, RentPerDay FROM dbo.CarView ORDER BY Reg;",
                buffered: true);
            this.conn.Close();
            return results;
        }

        public IEnumerable<CarView> GetCarViewListByModel(string model)
        {
            this.conn.Open();
            var results = this.conn.Query<CarView>(@"SELECT Reg, Make, Model, RentPerDay FROM dbo.CarView
  WHERE Model = @model ORDER BY Reg;", new { model = model }, buffered: true);
            this.conn.Close();
            return results;
        }

        public CarView GetCarViewByReg(string reg)
        {
            this.conn.Open();
            var results = this.conn.Query<CarView>(@"SELECT Reg, Make, Model, RentPerDay FROM dbo.CarView
  WHERE Reg = @reg;", new { reg = reg }, buffered: true).SingleOrDefault();
            this.conn.Close();
            return results;
        }

        public IEnumerable<RentedDates> GetCarRentedDatesByModel(string model)
        {
            this.conn.Open();
            var results = this.conn.Query<RentedDates>(@"SELECT c.Reg, c.RentPerDay, o.RentingStart, o.RentingEnd FROM dbo.OrderView o
  RIGHT JOIN dbo.CarView c ON o.CarReg = c.Reg
  WHERE c.Model = @model ORDER BY RentingStart, RentingEnd;", new { model = model }, buffered: true);
            this.conn.Close();
            return results;
        }

        ~CarSearchService()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (this.conn != null)
                    this.conn.Dispose();
            }

            disposed = true;
        }
    }
}
