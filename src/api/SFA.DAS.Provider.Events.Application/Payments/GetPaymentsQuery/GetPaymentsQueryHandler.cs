using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Mapping;
using SFA.DAS.Provider.Events.Application.Repositories;

namespace SFA.DAS.Provider.Events.Application.Payments.GetPaymentsQuery
{
    public class GetPaymentsQueryHandler : IAsyncRequestHandler<GetPaymentsQueryRequest, GetPaymentsQueryResponse>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;

        public GetPaymentsQueryHandler(IPaymentRepository paymentRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        public async Task<GetPaymentsQueryResponse> Handle(GetPaymentsQueryRequest message)
        {
            try
            {
                var payments = await _paymentRepository.GetPayments(
                        message.PageNumber,
                        message.PageSize,
                        message.EmployerAccountId,
                        message.Period?.AcademicYear,
                        message.Period?.Period,
                        message.Ukprn)
                    .ConfigureAwait(false);

                var result = _mapper.Map<PageOfResults<Payment>>(payments);

                return new GetPaymentsQueryResponse
                {
                    IsValid = true,
                    Result = result,
                };
            }
            catch (Exception ex)
            {
                return new GetPaymentsQueryResponse
                {
                    IsValid = false,
                    Exception = ex,
                };
            }
        }
    }
}