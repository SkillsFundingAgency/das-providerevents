using MediatR;
using SFA.DAS.Provider.Events.Submission.Domain;

namespace SFA.DAS.Provider.Events.Submission.Application.WriteSubmissionEvent
{
    public class WriteSubmissionEventCommand : IRequest
    {
        public SubmissionEvent Event { get; set; }
    }
}
