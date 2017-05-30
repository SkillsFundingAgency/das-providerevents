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
                payments = await _paymentRepository.GetPayments(message.PageNumber, 
                                                                message.PageSize,
                                                                message.EmployerAccountId,
                                                               message.Period == null ? null :(int?) message.Period.CalendarYear, 
                                                               message.Period == null ? null : (int?) message.Period.CalendarMonth,
                                                               message.Ukprn );

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