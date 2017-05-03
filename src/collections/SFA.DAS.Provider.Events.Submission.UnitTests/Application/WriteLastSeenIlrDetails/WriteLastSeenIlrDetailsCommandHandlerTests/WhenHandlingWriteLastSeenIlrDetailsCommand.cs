using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Submission.Application.WriteLastSeenIlrDetails;
using SFA.DAS.Provider.Events.Submission.Domain;
using SFA.DAS.Provider.Events.Submission.Domain.Data;

namespace SFA.DAS.Provider.Events.Submission.UnitTests.Application.WriteLastSeenIlrDetails.WriteLastSeenIlrDetailsCommandHandlerTests
{
    public class WhenHandlingWriteLastSeenIlrDetailsCommand
    {
        private Mock<IIlrSubmissionRepository> _ilrSubmissionRepository;
        private WriteLastSeenIlrDetailsCommandHandler _handler;
        private IlrDetails _ilr;

        [SetUp]
        public void Arrange()
        {
            _ilrSubmissionRepository = new Mock<IIlrSubmissionRepository>();

            _handler = new WriteLastSeenIlrDetailsCommandHandler(_ilrSubmissionRepository.Object);

            _ilr = new IlrDetails();
        }

        [Test]
        public void ThenItShouldStoreDetailsInRepository()
        {
            // Act
            _handler.Handle(new WriteLastSeenIlrDetailsCommand { LastSeenIlrs = new[] { _ilr } });

            // Assert
            _ilrSubmissionRepository.Verify(r => r.StoreLastSeenVersions(It.Is<IlrDetails[]>(x => x[0] == _ilr)), Times.Once);
        }
    }
}
