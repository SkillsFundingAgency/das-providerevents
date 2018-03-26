using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;

namespace SFA.DAS.Provider.Events.Application.Repositories
{
    public interface ISubmissionEventsRepository
    {
        Task<PageOfResults<SubmissionEventEntity>> GetSubmissionEventsSinceId(long eventId, int page, int pageSize);
        Task<PageOfResults<SubmissionEventEntity>> GetSubmissionEventsSinceTime(DateTime time, int page, int pageSize);
        Task<PageOfResults<SubmissionEventEntity>> GetSubmissionEventsForProviderSinceId(long ukprn, long eventId, int page, int pageSize);
        Task<PageOfResults<SubmissionEventEntity>> GetSubmissionEventsForProviderSinceTime(long ukprn, DateTime time, int page, int pageSize);
        Task<IEnumerable<SubmissionEventEntity>> GetSubmissionEventsForUln(long eventId, long uln);
    }
}