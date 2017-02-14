using MediatR;
using SFA.DAS.Provider.Events.DataLock.Domain;

namespace SFA.DAS.Provider.Events.DataLock.Application.WriteDataLockEvent
{
    public class WriteDataLockEventCommandRequest : IRequest
    {
        public DataLockEvent Event { get; set; }
    }
}