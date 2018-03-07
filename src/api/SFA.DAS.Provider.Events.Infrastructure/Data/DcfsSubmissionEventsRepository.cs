using System;
using System.Threading.Tasks;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.Repositories;

namespace SFA.DAS.Provider.Events.Infrastructure.Data
{
    public class DcfsSubmissionEventsRepository : DcfsRepository, ISubmissionEventsRepository
    {
        private const string Source = "Submissions.SubmissionEvents se";
        private const string Columns = "Id, "
                                       + "IlrFileName, "
                                       + "FileDateTime, "
                                       + "SubmittedDateTime, "
                                       + "ComponentVersionNumber, "
                                       + "UKPRN, "
                                       + "ULN, "
                                       // no need to fetch these, as SubmissionEventEntity doesn't contain them
                                       // + "LearnRefNumber, "
                                       // + "AimSeqNumber, "
                                       // + "PriceEpisodeIdentifier, "
                                       + "StandardCode, "
                                       + "ProgrammeType, "
                                       + "FrameworkCode, "
                                       + "PathwayCode, "
                                       + "ActualStartDate, "
                                       + "PlannedEndDate, "
                                       + "ActualEndDate, "
                                       + "OnProgrammeTotalPrice AS TrainingPrice, "
                                       + "CompletionTotalPrice AS EndpointAssessorPrice, "
                                       + "NINumber, "
                                       + "CommitmentId AS ApprenticeshipId, "
                                       + "AcademicYear, "
                                       + "EmployerReferenceNumber, "
                                       + "EPAOrgId";
        private const string CountColumn = "COUNT(se.Id)";
        private const string Pagination = "ORDER BY se.Id OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        public DcfsSubmissionEventsRepository()
            : base("EventsConnectionString")
        {
        }

        public async Task<PageOfResults<SubmissionEventEntity>> GetSubmissionEventsSinceId(long eventId, int page, int pageSize)
        {
            var whereClause = eventId > 0 ? $"WHERE se.Id > {eventId}" : string.Empty;
            return await GetPageOfSubmissionEvents(whereClause, page, pageSize)
                .ConfigureAwait(false);
        }

        public async Task<PageOfResults<SubmissionEventEntity>> GetSubmissionEventsSinceTime(DateTime time, int page, int pageSize)
        {
            return await GetPageOfSubmissionEvents($"WHERE SubmittedDateTime > '{time:yyyy-MM-dd HH:mm:ss}'", page, pageSize)
                .ConfigureAwait(false);
        }

        public async Task<PageOfResults<SubmissionEventEntity>> GetSubmissionEventsForProviderSinceId(long ukprn, long eventId, int page, int pageSize)
        {
            var whereClause = eventId > 0
                ? $"WHERE se.Id > {eventId} AND se.UKPRN = '{ukprn}'"
                : $"WHERE se.UKPRN = {ukprn}";
            return await GetPageOfSubmissionEvents(whereClause, page, pageSize)
                .ConfigureAwait(false);
        }

        public async Task<PageOfResults<SubmissionEventEntity>> GetSubmissionEventsForProviderSinceTime(long ukprn, DateTime time, int page, int pageSize)
        {
            var whereClause = $"WHERE se.UKPRN = {ukprn}"
                              + $" AND se.SubmittedDateTime > '{time:yyyy-MM-dd HH:mm:ss}'";
            return await GetPageOfSubmissionEvents(whereClause, page, pageSize)
                .ConfigureAwait(false);
        }


        private async Task<PageOfResults<SubmissionEventEntity>> GetPageOfSubmissionEvents(string whereClause, int page, int pageSize)
        {
            var numberOfPages = await GetNumberOfPages(whereClause, pageSize).ConfigureAwait(false);

            var payments = await GetSubmissionEvents(whereClause, page, pageSize)
                .ConfigureAwait(false);

            return new PageOfResults<SubmissionEventEntity>
            {
                PageNumber = page,
                TotalNumberOfPages = numberOfPages,
                Items = payments
            };
        }

        private async Task<SubmissionEventEntity[]> GetSubmissionEvents(string whereClause, int page, int pageSize)
        {
            var command = $"SELECT {Columns} FROM {Source} {whereClause} {Pagination}";

            var offset = (page - 1) * pageSize;
            return await Query<SubmissionEventEntity>(command, new {offset, pageSize})
                .ConfigureAwait(false);
        }

        private async Task<int> GetNumberOfPages(string whereClause, int pageSize)
        {
            var command = $"SELECT {CountColumn} FROM {Source} {whereClause}";
            var count = await QuerySingle<int>(command).ConfigureAwait(false);

            return NumberOfPages(count, pageSize);
        }
    }
}
