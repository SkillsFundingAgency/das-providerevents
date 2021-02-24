using MediatR;

namespace SFA.DAS.Provider.Events.Application.Period.GetPeriodQuery
{
    public class GetPeriodQueryRequest : IAsyncRequest<GetPeriodQueryResponse>
    {
        public string PeriodId  { get; set; }

        public short AcademicYear => short.Parse(PeriodId.Substring(0, 4));
        
        public byte Period => byte.Parse(PeriodId.Substring(6));
    }
}
