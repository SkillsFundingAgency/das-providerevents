using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.Client
{
    public class PaymentsEventsApiClient : IPaymentsEventsApiClient
    {
        private readonly IPaymentsEventsApiConfiguration _configuration;
        private readonly SecureHttpClient _httpClient;

        [ExcludeFromCodeCoverage]
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

        public async Task<PageOfResults<Payment>> GetPayments(string periodId = null, string employerAccountId = null, int page = 1, long? ukprn = null)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}api/payments?page={page}&periodId={periodId}&employerAccountId={employerAccountId}&ukprn={ukprn}");
            return JsonConvert.DeserializeObject<PageOfResults<Payment>>(response);
        }

        public async Task<PageOfResults<AccountTransfer>> GetTransfers(string periodId = null, long? senderAccountId = null, long? receiverAccountId = null, int page = 1)
        {
            var parameters = new List<string> {$"page={page}"};

            if (!string.IsNullOrEmpty(periodId))
                parameters.Add($"periodId={periodId}");
            if (senderAccountId.HasValue)
                parameters.Add($"senderAccountId={senderAccountId}");
            if (receiverAccountId.HasValue)
                parameters.Add($"receiverAccountId={receiverAccountId}");

            var response = await _httpClient.GetAsync($"{BaseUrl}api/transfers?" + string.Join("&", parameters));
            return JsonConvert.DeserializeObject<PageOfResults<AccountTransfer>>(response);
        }

        public async Task<PaymentStatistics> GetPaymentStatistics()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}api/v2/payments/statistics");
            return JsonConvert.DeserializeObject<PaymentStatistics>(response);
        }

        public async Task<PageOfResults<SubmissionEvent>> GetSubmissionEvents(long sinceEventId = 0, DateTime? sinceTime = null, long ukprn = 0, int page = 1)
        {
            var url = $"{BaseUrl}api/submissions?page={page}";
            if (sinceEventId > 0)
            {
                url += $"&sinceEventId={sinceEventId}";
            }
            if (sinceTime.HasValue)
            {
                url += $"&sinceTime={sinceTime.Value:yyyy-MM-ddTHH:mm:ss}";
            }
            if (ukprn > 0)
            {
                url += $"&ukprn={ukprn}";
            }

            var response = await _httpClient.GetAsync(url);
            return JsonConvert.DeserializeObject<PageOfResults<SubmissionEvent>>(response);
        }

        public async Task<List<SubmissionEvent>> GetLatestLearnerEventForStandards(long uln, long sinceEventId = 0)
        {
            var url = $"{BaseUrl}api/learners?uln={uln}";
            if (sinceEventId > 0)
            {
                url += $"&sinceEventId={sinceEventId}";
            }

            var response = await _httpClient.GetAsync(url);
            return JsonConvert.DeserializeObject<List<SubmissionEvent>>(response);
        }

        public async Task<PageOfResults<DataLockEvent>> GetDataLockEvents(long sinceEventId = 0, DateTime? sinceTime = null, string employerAccountId = null, long  ukprn = 0, int page = 1)
        {
            var url = $"{BaseUrl}api/datalock?page={page}";
            if (sinceEventId > 0)
            {
                url += $"&sinceEventId={sinceEventId}";
            }
            if (sinceTime.HasValue)
            {
                url += $"&sinceTime={sinceTime.Value:yyyy-MM-ddTHH:mm:ss}";
            }
            if (!string.IsNullOrEmpty(employerAccountId))
            {
                url += $"&employerAccountId={employerAccountId}";
            }
            if (ukprn > 0)
            {
                url += $"&ukprn={ukprn}";
            }

            var response = await _httpClient.GetAsync(url);
            return JsonConvert.DeserializeObject<PageOfResults<DataLockEvent>>(response);
        }

    }
}
