using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Mapping;
using SFA.DAS.Provider.Events.Application.Repositories;

namespace SFA.DAS.Provider.Events.Application.Payments.GetPaymentsStatistics
{
    public class GetPaymentsStatisticsHandler : IAsyncRequestHandler<GetPaymentsStatisticsRequest, GetPaymentsStatisticsResponse>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;

        public GetPaymentsStatisticsHandler(IPaymentRepository paymentRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        public async Task<GetPaymentsStatisticsResponse> Handle(GetPaymentsStatisticsRequest message)
        {
            try
            {
                var payments = await _paymentRepository.GetStatistics()
                    .ConfigureAwait(false);
                

                return new GetPaymentsStatisticsResponse
                {
                    IsValid = true,
                    Result = payments,
                };
            }
            catch (Exception ex)
            {
                return new GetPaymentsStatisticsResponse
                {
                    IsValid = false,
                    Exception = ex,
                };
            }
        }
    }
}