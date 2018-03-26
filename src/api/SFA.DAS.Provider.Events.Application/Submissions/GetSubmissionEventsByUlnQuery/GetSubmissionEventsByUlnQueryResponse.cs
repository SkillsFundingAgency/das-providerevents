using System.Collections.Generic;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;

namespace SFA.DAS.Provider.Events.Application.Submissions.GetSubmissionEventsByUlnQuery
{
    public class GetSubmissionEventsByUlnQueryResponse : QueryResponse<IEnumerable<SubmissionEventEntity>>
    {
    }
}