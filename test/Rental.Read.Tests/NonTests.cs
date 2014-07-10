namespace Rental.Read.Tests
{
    using System;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Linq;

    using DapperExtensions;
    using FizzWare.NBuilder;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NonTests
    {
        string connStr = ConfigurationManager.ConnectionStrings["Rental.Read.Conn"].ConnectionString;
        
        [Ignore]
        [TestMethod]
        public void NonTestPopulatingCarTable()
        {
            Random r = new Random();
            Func<string> reg = () =>
            {
                return String.Concat("OU14 ", (char)(65+r.Next(26)), (char)(65+r.Next(26)), (char)(65+r.Next(26)));
            };

            string[] makes = new string[] { "Audi", "Fiat", "Peugeot", "Volvo" };
            string[] models = new string[] { "1", "3", "5", "7" };

            var makeModel = from ma in makes
                            from mo in models
                            select ma + "-" + mo;

            foreach (var s in makeModel)
            {
                var cars = Builder<Model.CarView>.CreateListOfSize(10).All().With(c =>
                {
                    var tokens = s.Split('-');
                    c.Make = tokens[0];
                    c.Model = tokens[0][0] + "-" + tokens[1];
                    c.Reg = reg();
                    c.RentPerDay = r.Next(30, 500);
                    return c;
                }
                    ).Build();

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    conn.Insert<Model.CarView>(cars);
                    conn.Close();
                }
            }
        }
    }
}
