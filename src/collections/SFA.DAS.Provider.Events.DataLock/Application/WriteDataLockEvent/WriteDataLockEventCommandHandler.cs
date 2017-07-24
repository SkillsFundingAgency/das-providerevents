using MediatR;
using SFA.DAS.Provider.Events.DataLock.Domain;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var events = new List<DataLockEventEntity>();
            var errors = new List<DataLockEventErrorEntity>();
            var periods = new List<DataLockEventPeriodEntity>();
            var commitmentVersions = new List<DataLockEventCommitmentVersionEntity>();

            ExtractEntityFromEvents(message.Events, events, errors, periods, commitmentVersions);

            _dataLockEventRepository.BulkWriteDataLockEvents(events.ToArray());
            if (periods.Any())
            {
                _dataLockEventPeriodRepository.BulkWriteDataLockEventPeriods(periods.ToArray());
            }
            if (commitmentVersions.Any())
            {
                _dataLockEventCommitmentVersionRepository.BulkWriteDataLockEventCommitmentVersion(commitmentVersions.ToArray());
            }
            if (errors.Any())
            {
                _dataLockEventErrorRepository.BulkWriteDataLockEventError(errors.ToArray());
            }

            return Unit.Value;
        }


        private void ExtractEntityFromEvents(DataLockEvent[] sourceEvents, List<DataLockEventEntity> events, List<DataLockEventErrorEntity> errors, List<DataLockEventPeriodEntity> periods, List<DataLockEventCommitmentVersionEntity> commitmentVersions)
        {
            foreach (var @event in sourceEvents)
            {
                if (!CheckWhetherPriceEffectiveFromDateIsInTheCurrentPeriod(@event))
                {
                    continue;
                }

                var id = @event.DataLockEventId == default(Guid) ? Guid.NewGuid() : @event.DataLockEventId;
                events.Add(new DataLockEventEntity
                {
                    DataLockEventId = id,
                    ProcessDateTime = @event.ProcessDateTime,
                    Status = (int)@event.Status,
                    IlrFileName = @event.IlrFileName,
                    SubmittedDateTime = @event.SubmittedDateTime,
                    AcademicYear = @event.AcademicYear,
                    Ukprn = @event.Ukprn,
                    Uln = @event.Uln,
                    LearnRefnumber = @event.LearnRefnumber,
                    AimSeqNumber = @event.AimSeqNumber,
                    PriceEpisodeIdentifier = @event.PriceEpisodeIdentifier,
                    CommitmentId = @event.CommitmentId,
                    EmployerAccountId = @event.EmployerAccountId,
                    EventSource = (int)@event.EventSource,
                    HasErrors = @event.HasErrors,
                    IlrStartDate = @event.IlrStartDate,
                    IlrStandardCode = @event.IlrStandardCode,
                    IlrProgrammeType = @event.IlrProgrammeType,
                    IlrFrameworkCode = @event.IlrFrameworkCode,
                    IlrPathwayCode = @event.IlrPathwayCode,
                    IlrTrainingPrice = @event.IlrTrainingPrice,
                    IlrEndpointAssessorPrice = @event.IlrEndpointAssessorPrice,
                    IlrPriceEffectiveFromDate = @event.IlrPriceEffectiveFromDate,
                    IlrPriceEffectiveToDate = @event.IlrPriceEffectiveToDate
                });
                if (@event.Errors != null && @event.Errors.Any())
                {
                    errors.AddRange(@event.Errors.Select(x => new DataLockEventErrorEntity
                    {
                        DataLockEventId = id,
                        ErrorCode = x.ErrorCode,
                        SystemDescription = x.SystemDescription
                    }));
                }
                if (@event.Periods != null && @event.Periods.Any())
                {
                    periods.AddRange(@event.Periods.Select(x => new DataLockEventPeriodEntity
                    {
                        DataLockEventId = id,
                        CollectionPeriodName = x.CollectionPeriod.Name,
                        CollectionPeriodMonth = x.CollectionPeriod.Month,
                        CollectionPeriodYear = x.CollectionPeriod.Year,
                        CommitmentVersion = x.CommitmentVersion,
                        IsPayable = x.IsPayable,
                        TransactionType = (int)x.TransactionType
                    }));
                }
                if (@event.CommitmentVersions != null && @event.CommitmentVersions.Any())
                {
                    commitmentVersions.AddRange(@event.CommitmentVersions.Select(x => new DataLockEventCommitmentVersionEntity
                    {
                        DataLockEventId = id,
                        CommitmentVersion = x.CommitmentVersion,
                        CommitmentStartDate = x.CommitmentStartDate,
                        CommitmentStandardCode = x.CommitmentStandardCode,
                        CommitmentProgrammeType = x.CommitmentProgrammeType,
                        CommitmentFrameworkCode = x.CommitmentFrameworkCode,
                        CommitmentPathwayCode = x.CommitmentPathwayCode,
                        CommitmentNegotiatedPrice = x.CommitmentNegotiatedPrice,
                        CommitmentEffectiveDate = x.CommitmentEffectiveDate
                    }));
                }
            }
        }

        private static bool CheckWhetherPriceEffectiveFromDateIsInTheCurrentPeriod(DataLockEvent @event)
        {
            var collectionPeriod =
                @event.Periods?.OrderBy(c => c.CollectionPeriod.Month)
                    .ThenBy(c => c.CollectionPeriod.Year)
                    .FirstOrDefault();

            if (collectionPeriod == null)
            {
                return true;
            }

            if (!@event.IlrPriceEffectiveFromDate.HasValue)
            {
                return true;
            }

            var daysInMonth = DateTime.DaysInMonth(collectionPeriod.CollectionPeriod.Year, collectionPeriod.CollectionPeriod.Month);
            var dateTo = new DateTime(collectionPeriod.CollectionPeriod.Year, collectionPeriod.CollectionPeriod.Month, daysInMonth);
            var dateFrom = new DateTime(collectionPeriod.CollectionPeriod.Year, collectionPeriod.CollectionPeriod.Month, 1);

            if (@event.IlrPriceEffectiveFromDate.Value >= dateTo)
            {
                return true;
            }

            return  @event.IlrPriceEffectiveFromDate.Value >= dateFrom
                && @event.IlrPriceEffectiveFromDate.Value <= dateTo;
        }
    }
}