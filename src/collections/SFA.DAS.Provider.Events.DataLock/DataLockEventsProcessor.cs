using MediatR;

namespace SFA.DAS.Provider.Events.DataLock
{
    public class DataLockEventsProcessor
    {
        private readonly IMediator _mediator;

        public DataLockEventsProcessor(IMediator mediator)
        {
            _mediator = mediator;
        }
        protected DataLockEventsProcessor()
        {
            // For unit testing
        }

        public virtual void Process()
        {
        }
    }
}
