using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebScrapingUI.Startup))]
namespace WebScrapingUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
