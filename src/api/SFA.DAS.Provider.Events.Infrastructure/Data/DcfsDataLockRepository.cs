using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Provider.Events.Domain.Data;
using SFA.DAS.Provider.Events.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.Infrastructure.Data
{
    public class DcfsDataLockRepository : DcfsRepository, IDataLockRepository
    {
        private const string Source = "DataLock.DataLockEvents ev";
        private const string Columns = "Id, "
                                     + "DataLockEventId, "
                                     + "ProcessDateTime, "
                                     + "Status, "
                                     + "IlrFileName,  "
                                     + "UKPRN, "
                                     + "ULN, "
                                     + "LearnRefNumber, "
                                     + "AimSeqNumber, "
                                     + "PriceEpisodeIdentifier, "
                                     + "CommitmentId AS ApprenticeshipId, "
                                     + "EmployerAccountId, "
                                     + "EventSource, "
                                     + "HasErrors, "
                                     + "IlrStartDate, "
                                     + "IlrStandardCode, "
                                     + "IlrProgrammeType, "
                                     + "IlrFrameworkCode, "
                                     + "IlrPathwayCode, "
                                     + "IlrTrainingPrice, "
                                     + "IlrEndpointAssessorPrice,"
                                     + "IlrPriceEffectiveFromDate,"
                                     + "IlrPriceEffectiveToDate";
        private const string CountColumn = "COUNT(ev.Id)";
        private const string Pagination = "ORDER BY ev.Id OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        private const string ErrorsSource = "DataLock.DataLockEventErrors ";
        private const string ErrorsColumns = "DataLockEventId,"
                                           + "ErrorCode, "
                                           + "SystemDescription";

        private const string PeriodsSource = "DataLock.DataLockEventPeriods ";
        private const string PeriodsColumns = "DataLockEventId,"
                                              + "CollectionPeriodName AS CollectionPeriodId, "
                                              + "CollectionPeriodMonth, "
                                              + "CollectionPeriodYear, "
                                              + "CommitmentVersion AS ApprenticeshipVersion, "
                                              + "IsPayable, "
                                              + "TransactionType";

        private const string ApprenticeshipsSource = "DataLock.DataLockEventCommitmentVersions ";
        private const string ApprenticeshipsColumns = "DataLockEventId,"
                                                      + "CommitmentVersion AS Version, "
                                                      + "CommitmentStartDate AS StartDate, "
                                                      + "CommitmentStandardCode AS StandardCode, "
                                                      + "CommitmentProgrammeType AS ProgrammeType, "
                                                      + "CommitmentFrameworkCode AS FrameworkCode, "
                                                      + "CommitmentPathwayCode AS PathwayCode, "
                                                      + "CommitmentNegotiatedPrice AS NegotiatedPrice, "
                                                      + "CommitmentEffectiveDate AS EffectiveDate";

        public DcfsDataLockRepository()
            : base("EventsConnectionString")
        {
        }

        public async Task<PageOfEntities<DataLockEventEntity>> GetDataLockEventsSinceId(long eventId, int page, int pageSize)
        {
            var whereClause = eventId > 0 ? $"WHERE ev.Id > {eventId}" : string.Empty;
            return await GetPageOfDataLockEvents(whereClause, page, pageSize);
        }

        public async Task<PageOfEntities<DataLockEventEntity>> GetDataLockEventsSinceTime(DateTime time, int page, int pageSize)
        {
            var whereClause = $"WHERE ev.ProcessDateTime > '{time:yyyy-MM-dd HH:mm:ss}'";
            return await GetPageOfDataLockEvents(whereClause, page, pageSize);
        }

        public async Task<PageOfEntities<DataLockEventEntity>> GetDataLockEventsForAccountSinceId(string employerAccountId, long eventId, int page, int pageSize)
        {
            var whereClause = eventId > 0
                ? $"WHERE ev.Id > {eventId} AND ev.EmployerAccountId = '{employerAccountId.Replace("'", "''")}'"
                : $"WHERE ev.EmployerAccountId = '{employerAccountId.Replace("'", "''")}'";
            return await GetPageOfDataLockEvents(whereClause, page, pageSize);
        }

        public async Task<PageOfEntities<DataLockEventEntity>> GetDataLockEventsForAccountSinceTime(string employerAccountId, DateTime time, int page, int pageSize)
        {
            var whereClause = $"WHERE ev.EmployerAccountId = '{employerAccountId.Replace("'", "''")}'"
                              + $" AND ev.ProcessDateTime > '{time:yyyy-MM-dd HH:mm:ss}'";
            return await GetPageOfDataLockEvents(whereClause, page, pageSize);
        }

        public async Task<PageOfEntities<DataLockEventEntity>> GetDataLockEventsForProviderSinceId(long ukprn, long eventId, int page, int pageSize)
        {
            var whereClause = eventId > 0
                ? $"WHERE ev.Id > {eventId} AND ev.Ukprn = '{ukprn}'"
                : $"WHERE ev.Ukprn = {ukprn}";
            return await GetPageOfDataLockEvents(whereClause, page, pageSize);
        }

        public async Task<PageOfEntities<DataLockEventEntity>> GetDataLockEventsForProviderSinceTime(long ukprn, DateTime time, int page, int pageSize)
        {
            var whereClause = $"WHERE ev.Ukprn = {ukprn}"
                              + $" AND ev.ProcessDateTime > '{time:yyyy-MM-dd HH:mm:ss}'";
            return await GetPageOfDataLockEvents(whereClause, page, pageSize);
        }

        public async Task<PageOfEntities<DataLockEventEntity>> GetDataLockEventsForAccountAndProviderSinceId(string employerAccountId, long ukprn, long eventId, int page, int pageSize)
        {
            var whereClause = eventId > 0
                ? $"WHERE ev.Id > {eventId} AND ev.EmployerAccountId = '{employerAccountId.Replace("'", "''")}' AND ev.Ukprn = {ukprn}"
                : $"WHERE ev.EmployerAccountId = '{employerAccountId.Replace("'", "''")}' AND ev.Ukprn = {ukprn}";
            return await GetPageOfDataLockEvents(whereClause, page, pageSize);
        }

        public async Task<PageOfEntities<DataLockEventEntity>> GetDataLockEventsForAccountAndProviderSinceTime(string employerAccountId, long ukprn, DateTime time, int page, int pageSize)
        {
            var whereClause = $"WHERE ev.EmployerAccountId = '{employerAccountId.Replace("'", "''")}' AND ev.Ukprn = {ukprn}"
                              + $" AND ev.ProcessDateTime > '{time:yyyy-MM-dd HH:mm:ss}'";
            return await GetPageOfDataLockEvents(whereClause, page, pageSize);
        }

        public async Task<DataLockEventErrorEntity[]> GetDataLockErrorsForEvents(string[] eventIds)
        {
            if (!eventIds.Any())
            {
                return new DataLockEventErrorEntity[0];
            }

            var where = eventIds.Select(x => $"'{x}'").Aggregate((x, y) => $"{x}, {y}");
            var command = $"SELECT {ErrorsColumns} FROM {ErrorsSource} WHERE DataLockEventId IN ({where})";
            return await Query<DataLockEventErrorEntity>(command);
        }

        public async Task<DataLockEventPeriodEntity[]> GetDataLockPeriodsForEvent(string[] eventIds)
        {
            if (!eventIds.Any())
            {
                return new DataLockEventPeriodEntity[0];
            }

            var where = eventIds.Select(x => $"'{x}'").Aggregate((x, y) => $"{x}, {y}");
            var command = $"SELECT {PeriodsColumns} FROM {PeriodsSource} WHERE DataLockEventId IN ({where})";
            return await Query<DataLockEventPeriodEntity>(command);
        }

        public async Task<DataLockEventApprenticeshipEntity[]> GetDataLockApprenticeshipsForEvent(string[] eventIds)
        {
            if (!eventIds.Any())
            {
                return new DataLockEventApprenticeshipEntity[0];
            }

            var where = eventIds.Select(x => $"'{x}'").Aggregate((x, y) => $"{x}, {y}");
            var command = $"SELECT {ApprenticeshipsColumns} FROM {ApprenticeshipsSource} WHERE DataLockEventId IN ({where})";
            return await Query<DataLockEventApprenticeshipEntity>(command);
        }

        private async Task<PageOfEntities<DataLockEventEntity>> GetPageOfDataLockEvents(string whereClause, int page, int pageSize)
        {
            var numberOfPages = await GetNumberOfPages(whereClause, pageSize);

            var events = await GetDataLockEvents(whereClause, page, pageSize);

            return new PageOfEntities<DataLockEventEntity>
            {
                PageNumber = page,
                TotalNumberOfPages = numberOfPages,
                Items = events
            };
        }

        private async Task<DataLockEventEntity[]> GetDataLockEvents(string whereClause, int page, int pageSize)
        {
            var command = $"SELECT {Columns} FROM {Source} {whereClause} {Pagination}";

            var offset = (page - 1) * pageSize;
            return await Query<DataLockEventEntity>(command, new { offset, pageSize });
        }

        private async Task<int> GetNumberOfPages(string whereClause, int pageSize)
        {
            var command = $"SELECT {CountColumn} FROM {Source} {whereClause}";
            var count = await QuerySingle<int>(command);

            return (int)Math.Ceiling(count / (float)pageSize);
        }
    }
}