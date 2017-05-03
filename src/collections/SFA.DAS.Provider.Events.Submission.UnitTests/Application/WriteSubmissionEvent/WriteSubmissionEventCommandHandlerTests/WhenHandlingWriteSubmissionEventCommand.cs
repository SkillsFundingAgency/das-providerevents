using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Submission.Application.WriteSubmissionEvent;
using SFA.DAS.Provider.Events.Submission.Domain;
using SFA.DAS.Provider.Events.Submission.Domain.Data;

namespace SFA.DAS.Provider.Events.Submission.UnitTests.Application.WriteSubmissionEvent.WriteSubmissionEventCommandHandlerTests
{
    class WhenHandlingWriteSubmissionEventCommand
    {
        private Mock<ISubmissionEventRepository> _submissionEventRepository;
        private WriteSubmissionEventCommandHandler _handler;
        private SubmissionEvent _event;

        [SetUp]
        public void Arrange()
        {
            _submissionEventRepository = new Mock<ISubmissionEventRepository>();

            _handler = new WriteSubmissionEventCommandHandler(_submissionEventRepository.Object);

            _event = new SubmissionEvent();
        }

        [Test]
        public void ThenItShouldStoreEventInRepository()
        {
            // Act
            _handler.Handle(new WriteSubmissionEventCommand { Events = new[] { _event } });

            // Assert
            _submissionEventRepository.Verify(r => r.StoreSubmissionEvents(It.Is<SubmissionEvent[]>(x => x[0] == _event)), Times.Once);
        }
    }
}
