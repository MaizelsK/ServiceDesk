using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ServiceDeskApplication.Startup))]
namespace ServiceDeskApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
