using MediatR;
using SFA.DAS.Payments.Events.Domain;

namespace SFA.DAS.Payments.Events.Application.Payments.GetPaymentsForPeriodQuery
{
    public class GetPaymentsForPeriodQueryRequest : IAsyncRequest<GetPaymentsForPeriodQueryResponse>
    {
        public Period Period { get; set; }
        public string EmployerAccountId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
