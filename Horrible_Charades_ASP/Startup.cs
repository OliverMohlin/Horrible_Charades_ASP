using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Horrible_Charades_ASP.Startup))]
namespace Horrible_Charades_ASP
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

        }
    }
}
