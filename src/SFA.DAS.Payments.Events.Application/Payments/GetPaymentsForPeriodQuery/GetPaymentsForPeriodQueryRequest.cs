using MediatR;

namespace SFA.DAS.Payments.Events.Application.Payments.GetPaymentsForPeriodQuery
{
    public class GetPaymentsForPeriodQueryRequest : IAsyncRequest<GetPaymentsForPeriodQueryResponse>
    {
        public Domain.Period Period { get; set; }
        public string EmployerAccountId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
