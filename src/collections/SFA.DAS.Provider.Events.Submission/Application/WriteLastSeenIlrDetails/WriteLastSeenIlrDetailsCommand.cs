using MediatR;
using SFA.DAS.Provider.Events.Submission.Domain;

namespace SFA.DAS.Provider.Events.Submission.Application.WriteLastSeenIlrDetails
{
    public class WriteLastSeenIlrDetailsCommand : IRequest
    {
        public IlrDetails[] LastSeenIlrs { get; set; }
    }
}
