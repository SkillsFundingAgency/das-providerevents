using SFA.DAS.Http.Configuration;

namespace SFA.DAS.Provider.Events.Api.Client.Configuration
{
    public interface IPaymentsEventsApiConfiguration : IAzureActiveDirectoryClientConfiguration, IJwtClientConfiguration
    {
        ///// <summary>
        ///// The base url (schema, server, port and application path as appropriate)
        ///// </summary>
        ///// <example>https://some-server/</example>
        //string ApiBaseUrl { get; set; }
    }
}