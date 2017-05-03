using MediatR;
using SFA.DAS.Provider.Events.Submission.Domain.Data;

namespace SFA.DAS.Provider.Events.Submission.Application.WriteSubmissionEvent
{
    public class WriteSubmissionEventCommandHandler : IRequestHandler<WriteSubmissionEventCommand, Unit>
    {
        private readonly ISubmissionEventRepository _submissionEventRepository;

        public WriteSubmissionEventCommandHandler(ISubmissionEventRepository submissionEventRepository)
        {
            _submissionEventRepository = submissionEventRepository;
        }

        public Unit Handle(WriteSubmissionEventCommand message)
        {
            _submissionEventRepository.StoreSubmissionEvents(message.Events);
            return Unit.Value;
        }
    }
}
