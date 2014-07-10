namespace Rental.Read.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rental.Read.Model;

    [TestClass]
    public class CarSearchServiceTests
    {
        private IUnityContainer container;

        private ICarSearchService service;

        public CarSearchServiceTests()
        {
            this.container = new UnityContainer();
            Rental.Read.Bootstrapper.Register(container);

            this.service = container.Resolve<ICarSearchService>();
        }

        [TestMethod]
        public void TestGetCarViewList()
        {
            var list = this.service.GetCarViewList();

            Assert.IsInstanceOfType(list, typeof(IEnumerable<CarView>));

            list.ToList();
        }

        [TestMethod]
        public void TestGetCarViewListByModel()
        {
            var list = this.service.GetCarViewListByModel("V-7");

            Assert.IsInstanceOfType(list, typeof(IEnumerable<CarView>));

            list.ToList();
        }

        [TestMethod]
        public void TestGetCarViewByReg()
        {
            var car = this.service.GetCarViewByReg("OU14 XLE");

            Assert.IsInstanceOfType(car, typeof(CarView));
        }

        [TestMethod]
        public void TestGetCarRentedDatesByModel()
        {
            var list = this.service.GetCarRentedDatesByModel("V-7");

            Assert.IsInstanceOfType(list, typeof(IEnumerable<RentedDates>));

            list.ToList();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.service.Dispose();
        }
    }
}
