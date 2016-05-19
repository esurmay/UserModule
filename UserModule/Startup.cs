using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UserModule.Startup))]
namespace UserModule
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
