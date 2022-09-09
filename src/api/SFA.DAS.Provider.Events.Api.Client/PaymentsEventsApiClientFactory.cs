using System.Net.Http;
using SFA.DAS.Provider.Events.Api.Client.Configuration;

namespace SFA.DAS.Provider.Events.Api.Client
{
    public class PaymentsEventsApiClientFactory : IPaymentsEventsApiClientFactory
    {
        private readonly IPaymentsEventsApiClientConfiguration _configuration;

        public PaymentsEventsApiClientFactory(IPaymentsEventsApiClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IPaymentsEventsApiClient CreateClient(HttpMessageHandler handler = null)
        {
            var secureHttpClient = new SecureHttpClient(_configuration, handler);
            var apiClient = new PaymentsEventsApiClient(_configuration, secureHttpClient);
            return apiClient;
        }
    }
}
