using FluentAssertions;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Submissions.GetLatestLearnerEventByStandardQuery;
using SFA.DAS.Provider.Events.Application.Validation;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace SFA.DAS.Provider.Events.Api.UnitTests.Controllers.SubmissionsController
{
    public class WhenGettingLatestLearnerEventsForStandards
    {
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private Api.Controllers.LearnersController _controller;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetLatestLearnerEventForStandardsQueryRequest>()))
                .ReturnsAsync(new GetLatestLearnerEventForStandardsQueryResponse
                {
                    IsValid = true,
                    Result = new PageOfResults<SubmissionEvent>
                    {
                        PageNumber = 1,
                        TotalNumberOfPages = 1,
                        Items = new SubmissionEvent[]
                                {
                                new SubmissionEvent
                                {
                                    Id = 123
                                },
                                new SubmissionEvent
                                {
                                    Id = 987
                                }
                        }
                    }
                });

            _logger = new Mock<ILogger>();

            _controller = new Api.Controllers.LearnersController(_mediator.Object, _logger.Object);
        }

        [Test]
        public async Task ThenItShouldReturnAnOkResult()
        {
            // Act
            var actual = await _controller.GetLatestLearnerEventForStandardsByUln(1111111111);

            // Assert
            actual.Should().NotBeNull();
            actual.Should().BeOfType<OkNegotiatedContentResult<PageOfResults<SubmissionEvent>>>();
        }

        [Test]
        public async Task ThenItShouldReturnCorrectListOfEvents()
        {
            // Act
            var actual = ((OkNegotiatedContentResult<PageOfResults<SubmissionEvent>>)await _controller.GetLatestLearnerEventForStandardsByUln(1111111111)).Content;

            // Assert
            actual.Should().NotBeNull();
            actual.Items.Count().Should().Be(2);
            actual.Items[0].Id.Should().Be(123);
            actual.Items[1].Id.Should().Be(987);
            
        }

        [Test, TestCaseSource(nameof(RequestedFiltersTestCases))]
        public async Task ThenItShouldQueryWithTheRequestedFilters(long uln, long sinceEventId)
        {
            // Act
            await _controller.GetLatestLearnerEventForStandardsByUln(uln, sinceEventId);

            // Assert
            _mediator.Verify(
                m => m.SendAsync(
                    It.Is<GetLatestLearnerEventForStandardsQueryRequest>(r =>
                        r.SinceEventId == sinceEventId && r.Uln == uln)), Times.Once);
        }
        
        [Test]
        public async Task ThenItShouldReturnBadRequestWhenValidationExceptionOccurs()
        {
            // Arrange
            var validationErrorMessage = "Your request is not valid for some reason";
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetLatestLearnerEventForStandardsQueryRequest>()))
                .ReturnsAsync(new GetLatestLearnerEventForStandardsQueryResponse()
                {
                    Exception = new ValidationException(new[] {validationErrorMessage})
                });

            // Act
            var actual = await _controller.GetLatestLearnerEventForStandardsByUln(1111111111);

            // Assert
            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult) actual).Message.Should().Be(validationErrorMessage);
        }

        [Test]
        public async Task ThenItShouldReturnInternalServerErrorWhenGeneralExceptionOccurs()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetLatestLearnerEventForStandardsQueryRequest>()))
                .ThrowsAsync(new Exception("Something really bad happened"));

            // Act
            var actual = await _controller.GetLatestLearnerEventForStandardsByUln(3);

            // Assert
            actual.Should().NotBeNull();
            actual.Should().BeOfType<InternalServerErrorResult>();
        }



        private static object[] RequestedFiltersTestCases => new object[]
        {
            new object[] {1111111111, 10023534},
            new object[] {2222222222, 10000534},
        };
    }
}
