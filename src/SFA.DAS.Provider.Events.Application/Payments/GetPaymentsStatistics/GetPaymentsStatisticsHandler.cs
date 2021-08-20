using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Application.Repositories;

namespace SFA.DAS.Provider.Events.Application.Payments.GetPaymentsStatistics
{
    public class GetPaymentsStatisticsHandler : IAsyncRequestHandler<GetPaymentsStatisticsRequest, GetPaymentsStatisticsResponse>
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetPaymentsStatisticsHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<GetPaymentsStatisticsResponse> Handle(GetPaymentsStatisticsRequest message)
        {
                var payments = await _paymentRepository.GetStatistics()
                    .ConfigureAwait(false);
                
                return new GetPaymentsStatisticsResponse
                {
                    IsValid = true,
                    Result = payments,
                };
        }
    }
}