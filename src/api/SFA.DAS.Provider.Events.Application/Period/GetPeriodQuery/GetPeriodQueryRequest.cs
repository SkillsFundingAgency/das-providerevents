using MediatR;

namespace SFA.DAS.Provider.Events.Application.Period.GetPeriodQuery
{
    public class GetPeriodQueryRequest : IAsyncRequest<GetPeriodQueryResponse>
    {
        public string PeriodId { get; set; }
    }
}
