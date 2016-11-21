using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DBEN.ETM.Startup))]
namespace DBEN.ETM
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
