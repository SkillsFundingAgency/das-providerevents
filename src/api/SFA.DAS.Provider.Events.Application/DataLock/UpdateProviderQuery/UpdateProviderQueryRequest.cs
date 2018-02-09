using MediatR;
using SFA.DAS.Provider.Events.Application.Data.Entities;

namespace SFA.DAS.Provider.Events.Application.DataLock.UpdateProviderQuery
{
    public class UpdateProviderQueryRequest : IAsyncRequest<UpdateProviderQueryResponse>
    {
        public ProviderEntity Provider { get; set; }
    }
}