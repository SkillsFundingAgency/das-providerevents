using SFA.DAS.Http.Configuration;

namespace SFA.DAS.Provider.Events.Api.Client.Configuration
{
    public interface IPaymentsEventsApiConfiguration : IAzureActiveDirectoryClientConfiguration, IJwtClientConfiguration
    {
        string BaseUrl { get; set; }
    }
}