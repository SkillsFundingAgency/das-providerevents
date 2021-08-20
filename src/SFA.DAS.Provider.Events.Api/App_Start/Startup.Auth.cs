using Microsoft.Azure;
using Owin;
using SFA.DAS.Authentication.Extensions;

namespace SFA.DAS.Provider.Events.Api
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            if (CloudConfigurationManager.GetSetting("EnvironmentName") == "LOCAL")
                return;

            app.UseMixedModeAuthentication(new MixedModeAuthenticationOptions
            {
                ValidIssuers = CloudConfigurationManager.GetSetting("ApiIssuers").Split(' '),
                ValidAudiences = CloudConfigurationManager.GetSetting("ApiAudiences").Split(' '),
                ApiTokenSecret = CloudConfigurationManager.GetSetting("ApiTokenSecret"),
                MetadataEndpoint = CloudConfigurationManager.GetSetting("MetadataEndpoint")
            });
        }
    }
}