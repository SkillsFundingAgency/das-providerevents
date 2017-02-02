using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.Client
{
    public class PaymentsEventsApiClient : IPaymentsEventsApiClient
    {
        private readonly IPaymentsEventsApiConfiguration _configuration;
        private readonly SecureHttpClient _httpClient;

        public PaymentsEventsApiClient(IPaymentsEventsApiConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new SecureHttpClient(configuration.ClientToken);
        }
        internal PaymentsEventsApiClient(IPaymentsEventsApiConfiguration configuration, SecureHttpClient httpClient)
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



        public async Task<PageOfResults<SubmissionEvent>> GetSubmissionEvents(int sinceEventId = 0, DateTime? sinceTime = null, int page = 1)
        {
            var url = $"{BaseUrl}api/submissions?page={page}";
            if (sinceEventId > 0)
            {
                url += $"sinceEventId={sinceEventId}";
            }
            if (sinceTime.HasValue)
            {
                url += $"sinceTime={sinceTime.Value:yyyy-MM-ddTHH:mm:ss}";
            }

            var response = await _httpClient.GetAsync(url);
            return JsonConvert.DeserializeObject<PageOfResults<SubmissionEvent>>(response);
        }
    }
}
