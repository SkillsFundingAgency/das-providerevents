using System.Net.Http;

namespace SFA.DAS.Provider.Events.Api.Client
{
    public interface IPaymentsEventsApiClientFactory
    {
        IPaymentsEventsApiClient CreateClient(HttpMessageHandler handler = null);
    }
}
