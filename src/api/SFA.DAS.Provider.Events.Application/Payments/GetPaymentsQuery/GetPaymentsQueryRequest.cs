using MediatR;

namespace SFA.DAS.Provider.Events.Application.Payments.GetPaymentsQuery
{
    public class GetPaymentsQueryRequest : IAsyncRequest<GetPaymentsQueryResponse>
    {
        public Domain.Period Period { get; set; }
        public string EmployerAccountId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
