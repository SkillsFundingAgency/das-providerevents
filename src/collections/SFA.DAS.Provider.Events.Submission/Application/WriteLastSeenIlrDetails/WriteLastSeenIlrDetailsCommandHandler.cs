using MediatR;
using SFA.DAS.Provider.Events.Submission.Domain.Data;

namespace SFA.DAS.Provider.Events.Submission.Application.WriteLastSeenIlrDetails
{
    public class WriteLastSeenIlrDetailsCommandHandler : IRequestHandler<WriteLastSeenIlrDetailsCommand, Unit>
    {
        private readonly IIlrSubmissionRepository _ilrSubmissionRepository;

        public WriteLastSeenIlrDetailsCommandHandler(IIlrSubmissionRepository ilrSubmissionRepository)
        {
            _ilrSubmissionRepository = ilrSubmissionRepository;
        }

        public Unit Handle(WriteLastSeenIlrDetailsCommand message)
        {
            _ilrSubmissionRepository.StoreLastSeenVersions(message.LastSeenIlrs);

            return Unit.Value;
        }
    }
}
