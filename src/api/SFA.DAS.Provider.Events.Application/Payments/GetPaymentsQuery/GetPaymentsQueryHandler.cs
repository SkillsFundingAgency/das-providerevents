using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Domain;
using SFA.DAS.Provider.Events.Domain.Data;
using SFA.DAS.Provider.Events.Domain.Data.Entities;
using SFA.DAS.Provider.Events.Domain.Mapping;

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
                PageOfEntities<PaymentEntity> payments;
                if (message.Period != null && !string.IsNullOrEmpty(message.EmployerAccountId))
                {
                    payments = await _paymentRepository.GetPaymentsForAccountInPeriod(message.EmployerAccountId,
                        message.Period.CalendarYear, message.Period.CalendarMonth, message.PageNumber, message.PageSize);
                }
                else if (message.Period != null)
                {
                    payments = await _paymentRepository.GetPaymentsForPeriod(message.Period.CalendarYear,
                                    message.Period.CalendarMonth,
                                    message.PageNumber, message.PageSize);
                }
                else if (!string.IsNullOrEmpty(message.EmployerAccountId))
                {
                    payments = await _paymentRepository.GetPaymentsForAccount(message.EmployerAccountId,
                        message.PageNumber, message.PageSize);
                }
                else
                {
                    payments = await _paymentRepository.GetPayments(message.PageNumber, message.PageSize);
                }

                return new GetPaymentsQueryResponse
                {
                    IsValid = true,
                    Result = _mapper.Map<PageOfResults<Payment>>(payments)
                };
            }
            catch (Exception ex)
            {
                return new GetPaymentsQueryResponse
                {
                    IsValid = false,
                    Exception = ex
                };
            }
        }
    }
}