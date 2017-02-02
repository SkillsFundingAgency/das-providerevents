using System;
using System.Threading.Tasks;
using SFA.DAS.Provider.Events.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.Domain.Data
{
    public interface ISubmissionEventsRepository
    {
        Task<PageOfEntities<SubmissionEventEntity>> GetSubmissionEventsSinceId(int eventId, int page, int pageSize);
        Task<PageOfEntities<SubmissionEventEntity>> GetSubmissionEventsSinceTime(DateTime time, int page, int pageSize);
    }
}