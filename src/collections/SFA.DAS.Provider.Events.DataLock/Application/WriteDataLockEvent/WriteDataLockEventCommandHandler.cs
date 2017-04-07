using MediatR;
using SFA.DAS.Provider.Events.DataLock.Domain;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;
using System;

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
            var eventEntity = new DataLockEventEntity
            {
                ProcessDateTime = message.Event.ProcessDateTime,
                IlrFileName = message.Event.IlrFileName,
                SubmittedDateTime = message.Event.SubmittedDateTime,
                AcademicYear = message.Event.AcademicYear,
                Ukprn = message.Event.Ukprn,
                Uln = message.Event.Uln,
                LearnRefnumber = message.Event.LearnRefnumber,
                AimSeqNumber = message.Event.AimSeqNumber,
                PriceEpisodeIdentifier = message.Event.PriceEpisodeIdentifier,
                CommitmentId = message.Event.CommitmentId,
                EmployerAccountId = message.Event.EmployerAccountId,
                EventSource = (int) message.Event.EventSource,
                HasErrors = message.Event.HasErrors,
                IlrStartDate = message.Event.IlrStartDate,
                IlrStandardCode = message.Event.IlrStandardCode,
                IlrProgrammeType = message.Event.IlrProgrammeType,
                IlrFrameworkCode = message.Event.IlrFrameworkCode,
                IlrPathwayCode = message.Event.IlrPathwayCode,
                IlrTrainingPrice = message.Event.IlrTrainingPrice,
                IlrEndpointAssessorPrice = message.Event.IlrEndpointAssessorPrice
            };

            var eventId = _dataLockEventRepository.WriteDataLockEvent(eventEntity);

            WriteEventErrors(eventId, message.Event.Errors);
            WriteEventPeriods(eventId, message.Event.Periods);
            WriteEventCommitmentVersions(eventId, message.Event.CommitmentVersions);

            return Unit.Value;
        }

        private void WriteEventErrors(Guid eventId, DataLockEventError[] errors)
        {
            if (errors == null)
            {
                return;
            }

            foreach (var error in errors)
            {
                var errorEntity = new DataLockEventErrorEntity
                {
                    DataLockEventId = eventId,
                    ErrorCode = error.ErrorCode,
                    SystemDescription = error.SystemDescription
                };

                _dataLockEventErrorRepository.WriteDataLockEventError(errorEntity);
            }
        }

        private void WriteEventPeriods(Guid eventId, DataLockEventPeriod[] periods)
        {
            if (periods == null)
            {
                return;
            }

            foreach (var period in periods)
            {
                var periodEntity = new DataLockEventPeriodEntity
                {
                    DataLockEventId = eventId,
                    CollectionPeriodName = period.CollectionPeriod.Name,
                    CollectionPeriodMonth = period.CollectionPeriod.Month,
                    CollectionPeriodYear = period.CollectionPeriod.Year,
                    CommitmentVersion = period.CommitmentVersion,
                    IsPayable = period.IsPayable,
                    TransactionType = (int)period.TransactionType
                };

                _dataLockEventPeriodRepository.WriteDataLockEventPeriod(periodEntity);
            }
        }

        private void WriteEventCommitmentVersions(Guid eventId, DataLockEventCommitmentVersion[] versions)
        {
            if (versions == null)
            {
                return;
            }

            foreach (var version in versions)
            {
                var versionEntity = new DataLockEventCommitmentVersionEntity
                {
                    DataLockEventId = eventId,
                    CommitmentVersion = version.CommitmentVersion,
                    CommitmentStartDate = version.CommitmentStartDate,
                    CommitmentStandardCode = version.CommitmentStandardCode,
                    CommitmentProgrammeType = version.CommitmentProgrammeType,
                    CommitmentFrameworkCode = version.CommitmentFrameworkCode,
                    CommitmentPathwayCode = version.CommitmentPathwayCode,
                    CommitmentNegotiatedPrice = version.CommitmentNegotiatedPrice,
                    CommitmentEffectiveDate = version.CommitmentEffectiveDate
                };

                _dataLockEventCommitmentVersionRepository.WriteDataLockEventCommitmentVersion(versionEntity);
            }
        }
    }
}