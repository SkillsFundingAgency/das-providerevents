using MediatR;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;

namespace SFA.DAS.Provider.Events.DataLock.Application.WriteDataLockEvent
{
    public class WriteDataLockEventCommandHandler : IRequestHandler<WriteDataLockEventCommandRequest, Unit>
    {
        private readonly IDataLockEventRepository _dataLockEventRepository;
        private readonly IDataLockEventPeriodRepository _dataLockEventPeriodRepository;
        private readonly IDataLockEventCommitmentVersionRepository _dataLockEventCommitmentVersionRepository;
        private readonly IDataLockEventErrorRepository _dataLockEventErrorRepository;

        public WriteDataLockEventCommandHandler(IDataLockEventRepository dataLockEventRepository,
            IDataLockEventPeriodRepository dataLockEventPeriodRepository,
            IDataLockEventCommitmentVersionRepository dataLockEventCommitmentVersionRepository,
            IDataLockEventErrorRepository dataLockEventErrorRepository)
        {
            _dataLockEventRepository = dataLockEventRepository;
            _dataLockEventPeriodRepository = dataLockEventPeriodRepository;
            _dataLockEventCommitmentVersionRepository = dataLockEventCommitmentVersionRepository;
            _dataLockEventErrorRepository = dataLockEventErrorRepository;
        }

        public Unit Handle(WriteDataLockEventCommandRequest message)
        {


            return Unit.Value;
        }
    }
}