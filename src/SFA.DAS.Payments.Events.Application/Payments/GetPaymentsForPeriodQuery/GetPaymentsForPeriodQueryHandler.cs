using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Payments.Events.Domain.Data;
using SFA.DAS.Payments.Events.Domain.Data.Entities;

namespace SFA.DAS.Payments.Events.Application.Payments.GetPaymentsForPeriodQuery
{
    public class GetPaymentsForPeriodQueryHandler : IAsyncRequestHandler<GetPaymentsForPeriodQueryRequest, GetPaymentsForPeriodQueryResponse>
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetPaymentsForPeriodQueryHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<GetPaymentsForPeriodQueryResponse> Handle(GetPaymentsForPeriodQueryRequest message)
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
                    payments =
                        await
                            _paymentRepository.GetPaymentsForPeriod(message.Period.CalendarYear,
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

                return new GetPaymentsForPeriodQueryResponse
                {
                    IsValid = true,
                    Result = new Domain.PageOfResults<Domain.Payment>
                    {
                        PageNumber = payments.PageNumber,
                        TotalNumberOfPages = payments.NumberOfPages,
                        Items = payments.Items.Select(e => new Domain.Payment
                        {
                            Id = e.Id,
                            Ukprn = e.Ukprn,
                            Uln = e.Uln,
                            EmployerAccountId = e.EmployerAccountId,
                            ApprenticeshipId = e.ApprenticeshipId,
                            DeliveryPeriod = new Domain.CalendarPeriod
                            {
                                Month = e.DeliveryPeriodMonth,
                                Year = e.DeliveryPeriodYear
                            },
                            CollectionPeriod = new Domain.CalendarPeriod
                            {
                                Month = e.CollectionPeriodMonth,
                                Year = e.CollectionPeriodYear
                            },
                            EvidenceSubmittedOn = e.EvidenceSubmittedOn,
                            EmployerAccountVersion = e.EmployerAccountVersion,
                            ApprenticeshipVersion = e.ApprenticeshipVersion,
                            FundingSource = (Domain.FundingSource)e.FundingSource,
                            TransactionType = (Domain.TransactionType)e.TransactionType,
                            Amount = e.Amount
                        }).ToArray()
                    }
                };
            }
            catch (Exception ex)
            {
                return new GetPaymentsForPeriodQueryResponse
                {
                    IsValid = false,
                    Exception = ex
                };
            }
        }
    }
}