namespace Rental.Read
{
    using System.Configuration;
    using System.Data.SqlClient;

    using Microsoft.Practices.Unity;

    public static class Bootstrapper
    {
        public static string ConnStr = ConfigurationManager.ConnectionStrings["Rental.Read.Conn"].ConnectionString;

        public static void Register(IUnityContainer container)
        {
            container.RegisterType<SqlConnection>(new InjectionFactory(c => new SqlConnection(ConnStr)));

            container.RegisterType<ICarSearchService, CarSearchService>();
        }
    }
}
