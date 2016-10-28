using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Payments.Events.Api.Types;

namespace SFA.DAS.Payments.Events.Api.Client
{
    public class PaymentsEventsApiClient : IPaymentsEventsApiClient
    {
        private readonly PaymentsEventsApiConfiguration _configuration;
        private readonly SecureHttpClient _httpClient;

        public PaymentsEventsApiClient(PaymentsEventsApiConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new SecureHttpClient(configuration.ClientToken);
        }
        internal PaymentsEventsApiClient(PaymentsEventsApiConfiguration configuration, SecureHttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        private string BaseUrl
        {
            get
            {
                return _configuration.ApiBaseUrl.EndsWith("/")
                    ? _configuration.ApiBaseUrl
                    : _configuration.ApiBaseUrl + "/";
            }
        }

        public async Task<PeriodEnd[]> GetPeriodEnds()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}api/periodends");
            return JsonConvert.DeserializeObject<PeriodEnd[]>(response);
        }

        public async Task<PageOfResults<Payment>> GetPayments(string periodId = null, string employerAccountId = null, int page = 1)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}api/payments?page={page}&periodId={periodId}&employerAccountId={employerAccountId}");
            return JsonConvert.DeserializeObject<PageOfResults<Payment>>(response);
        }
    }
}
