using Microsoft.Azure;
using Owin;
using SFA.DAS.Authentication.Extensions;
using System.Configuration;

namespace SFA.DAS.Provider.Events.Api
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            if (CloudConfigurationManager.AppSettings["EnvironmentName"] == "LOCAL")
                return;

            app.UseMixedModeAuthentication(new MixedModeAuthenticationOptions
            {
                ValidIssuers = CloudConfigurationManager.AppSettings["ApiIssuers"].Split(' '),
                ValidAudiences = CloudConfigurationManager.AppSettings["ApiAudiences"].Split(' '),
                ApiTokenSecret = CloudConfigurationManager.AppSettings["ApiTokenSecret"],
                MetadataEndpoint = CloudConfigurationManager.AppSettings["MetadataEndpoint"]
            });
        }
    }
}