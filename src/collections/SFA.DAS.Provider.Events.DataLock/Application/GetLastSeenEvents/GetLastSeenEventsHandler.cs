using System;
using System.Linq;
using MediatR;
using SFA.DAS.Payments.DCFS.Domain;
using SFA.DAS.Provider.Events.DataLock.Domain;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;

namespace SFA.DAS.Provider.Events.DataLock.Application.GetLastSeenEvents
{
    public class GetLastSeenEventsHandler : IRequestHandler<GetLastSeenEventsRequest, GetLastSeenEventsResponse>
    {
        private readonly IDataLockEventRepository _dataLockEventRepository;
        private readonly IDataLockEventPeriodRepository _dataLockEventPeriodRepository;
        private readonly IDataLockEventCommitmentVersionRepository _dataLockEventCommitmentVersionRepository;
        private readonly IDataLockEventErrorRepository _dataLockEventErrorRepository;

        public GetLastSeenEventsHandler(IDataLockEventRepository dataLockEventRepository,
            IDataLockEventPeriodRepository dataLockEventPeriodRepository,
            IDataLockEventCommitmentVersionRepository dataLockEventCommitmentVersionRepository,
            IDataLockEventErrorRepository dataLockEventErrorRepository)
        {
            _dataLockEventRepository = dataLockEventRepository;
            _dataLockEventPeriodRepository = dataLockEventPeriodRepository;
            _dataLockEventCommitmentVersionRepository = dataLockEventCommitmentVersionRepository;
            _dataLockEventErrorRepository = dataLockEventErrorRepository;
        }

        public GetLastSeenEventsResponse Handle(GetLastSeenEventsRequest message)
        {
            try
            {
                var lastSeenEventEntities = _dataLockEventRepository.GetLastSeenEvents();

                var lastSeenEvents = lastSeenEventEntities == null
                    ? null
                    : lastSeenEventEntities
                        .Select(e => new DataLockEvent
                        {
                            Id = e.Id,
                            ProcessDateTime = e.ProcessDateTime,
                            IlrFileName = e.IlrFileName,
                            SubmittedDateTime = e.SubmittedDateTime,
                            AcademicYear = e.AcademicYear,
                            Ukprn = e.Ukprn,
                            Uln = e.Uln,
                            LearnRefnumber = e.LearnRefnumber,
                            AimSeqNumber = e.AimSeqNumber,
                            PriceEpisodeIdentifier = e.PriceEpisodeIdentifier,
                            CommitmentId = e.CommitmentId,
                            EmployerAccountId = e.EmployerAccountId,
                            EventSource = (EventSource) e.EventSource,
                            HasErrors = e.HasErrors,
                            IlrStartDate = e.IlrStartDate,
                            IlrStandardCode = e.IlrStandardCode,
                            IlrProgrammeType = e.IlrProgrammeType,
                            IlrFrameworkCode = e.IlrFrameworkCode,
                            IlrPathwayCode = e.IlrPathwayCode,
                            IlrTrainingPrice = e.IlrTrainingPrice,
                            IlrEndpointAssessorPrice = e.IlrEndpointAssessorPrice,
                            Errors = GetEventErrors(e.Id),
                            Periods = GetEventPeriods(e.Id),
                            CommitmentVersions = GetEventCommitmentVersions(e.Id)
                        })
                        .ToArray();

                return new GetLastSeenEventsResponse
                {
                    IsValid = true,
                    Items = lastSeenEvents
                };
            }
            catch (Exception ex)
            {
                return new GetLastSeenEventsResponse
                {
                    IsValid =  false,
                    Exception = ex
                };
            }
        }

        private DataLockEventError[] GetEventErrors(long eventId)
        {
            var errorEnities = _dataLockEventErrorRepository.GetDatalockEventErrors(eventId);

            return errorEnities == null
                ? null
                : errorEnities
                    .Select(e => new DataLockEventError
                    {
                        DataLockEventId = e.DataLockEventId,
                        ErrorCode = e.ErrorCode,
                        SystemDescription = e.SystemDescription
                    })
                    .ToArray();
        }

        private DataLockEventPeriod[] GetEventPeriods(long eventId)
        {
            var periodEntities = _dataLockEventPeriodRepository.GetDataLockEventPeriods(eventId);

            return periodEntities == null
                ? null
                : periodEntities
                    .Select(e => new DataLockEventPeriod
                    {
                        DataLockEventId = e.DataLockEventId,
                        CollectionPeriod = new CollectionPeriod
                        {
                            Name = e.CollectionPeriodName,
                            Month = e.CollectionPeriodMonth,
                            Year = e.CollectionPeriodYear
                        },
                        CommitmentVersion = e.CommitmentVersion,
                        IsPayable = e.IsPayable,
                        TransactionType = (TransactionType) e.TransactionType
                    })
                    .ToArray();
        }

        private DataLockEventCommitmentVersion[] GetEventCommitmentVersions(long eventId)
        {
            var commitmentVersionEntities = _dataLockEventCommitmentVersionRepository.GetDataLockEventCommitmentVersions(eventId);

            return commitmentVersionEntities == null
                ? null
                : commitmentVersionEntities
                    .Select(e => new DataLockEventCommitmentVersion
                    {
                        DataLockEventId = e.DataLockEventId,
                        CommitmentVersion = e.CommitmentVersion,
                        CommitmentStartDate = e.CommitmentStartDate,
                        CommitmentStandardCode = e.CommitmentStandardCode,
                        CommitmentProgrammeType = e.CommitmentProgrammeType,
                        CommitmentFrameworkCode = e.CommitmentFrameworkCode,
                        CommitmentPathwayCode = e.CommitmentPathwayCode,
                        CommitmentNegotiatedPrice = e.CommitmentNegotiatedPrice,
                        CommitmentEffectiveDate = e.CommitmentEffectiveDate
                    })
                    .ToArray();
        }
    }
}