using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Submission.Application.GetCurrentVersions;
using SFA.DAS.Provider.Events.Submission.Domain;
using SFA.DAS.Provider.Events.Submission.Domain.Data;

namespace SFA.DAS.Provider.Events.Submission.UnitTests.Application.GetCurrentVersions.GetCurrentVersionsQueryHandlerTests
{
    public class WhenHandlingGetCurrentVersionsQuery
    {
        private IlrDetails _ilr1;
        private Mock<IIlrSubmissionRepository> _ilrSubmissionRepository;
        private GetCurrentVersionsQueryHandler _handler;

        [SetUp]
        public void Arrange()
        {
            _ilr1 = new IlrDetails();

            _ilrSubmissionRepository = new Mock<IIlrSubmissionRepository>();
            _ilrSubmissionRepository.Setup(r => r.GetCurrentVersions())
                .Returns(new[]
                {
                    _ilr1
                });

            _handler = new GetCurrentVersionsQueryHandler(_ilrSubmissionRepository.Object);
        }

        [Test]
        public void ThenItShouldReturnIlrDetailsFromRepository()
        {
            // Act
            var actual = _handler.Handle(new GetCurrentVersionsQuery());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);
            Assert.IsNotNull(actual.Items);
            Assert.AreEqual(1, actual.Items.Length);
            Assert.AreSame(_ilr1, actual.Items[0]);
        }

        [Test]
        public void ThenItShouldReturnEmptyArrayIfNoResultFromRepository()
        {
            // Arrange
            _ilrSubmissionRepository.Setup(r => r.GetCurrentVersions())
                .Returns<IlrDetails[]>(null);

            // Act
            var actual = _handler.Handle(new GetCurrentVersionsQuery());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);
            Assert.IsNotNull(actual.Items);
            Assert.AreEqual(0, actual.Items.Length);
        }

        [Test]
        public void ThenItShouldReturnInvalidResponseIfRepositoryErrors()
        {
            // Arrange
            _ilrSubmissionRepository.Setup(r => r.GetCurrentVersions())
                .Throws(new Exception("Test"));

            // Act
            var actual = _handler.Handle(new GetCurrentVersionsQuery());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.IsNotNull(actual.Exception);
            Assert.AreEqual("Test", actual.Exception.Message);
        }
    }
}
