using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Rental.Web.Public.Startup))]
namespace Rental.Web.Public
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
