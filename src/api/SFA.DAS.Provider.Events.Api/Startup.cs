using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SFA.DAS.Provider.Events.Api.Startup))]
namespace SFA.DAS.Provider.Events.Api
{
    public class Startup
    {
        public partial class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                ConfigureAuth(app);
            }
        }
    }
}