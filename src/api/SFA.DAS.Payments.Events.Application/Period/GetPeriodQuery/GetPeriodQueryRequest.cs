using MediatR;

namespace SFA.DAS.Payments.Events.Application.Period.GetPeriodQuery
{
    public class GetPeriodQueryRequest : IAsyncRequest<GetPeriodQueryResponse>
    {
        public string PeriodId { get; set; }
    }
}
