using MediatR;

namespace SFA.DAS.Provider.Events.Application.Payments.GetPaymentsStatistics
{
    public class GetPaymentsStatisticsRequest : IAsyncRequest<GetPaymentsStatisticsResponse>
    {
        public int? CourseType { get; set; }
    }
}
