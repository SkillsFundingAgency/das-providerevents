﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Submissions.GetSubmissionEventsQuery;
using SFA.DAS.Provider.Events.Application.Validation;
using SFA.DAS.Provider.Events.Domain.Mapping;

namespace SFA.DAS.Provider.Events.Api.UnitTests.Controllers.SubmissionsController
{
    public class WhenGettingListOfSubmissionEvents
    {
        private Mock<IMediator> _mediator;
        private Mock<IMapper> _mapper;
        private Mock<ILogger> _logger;
        private Api.Controllers.SubmissionsController _controller;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetSubmissionEventsQueryRequest>()))
                .ReturnsAsync(new GetSubmissionEventsQueryResponse
                {
                    IsValid = true,
                    Result = new Domain.PageOfResults<Domain.SubmissionEvent>
                    {
                        PageNumber = 1,
                        TotalNumberOfPages = 10,
                        Items = new[]
                        {
                            new Domain.SubmissionEvent
                            {
                                Id = 123
                            }
                        }
                    }
                });

            _mapper = new Mock<IMapper>();
            _mapper.Setup(m => m.Map<PageOfResults<SubmissionEvent>>(It.IsAny<Domain.PageOfResults<Domain.SubmissionEvent>>()))
                .Returns((Domain.PageOfResults<Domain.SubmissionEvent> source) =>
                {
                    return new PageOfResults<SubmissionEvent>
                    {
                        PageNumber = source.PageNumber,
                        TotalNumberOfPages = source.TotalNumberOfPages,
                        Items = source.Items.Select(e => new SubmissionEvent
                        {
                            Id = e.Id
                        }).ToArray()
                    };
                });

            _logger = new Mock<ILogger>();

            _controller = new Api.Controllers.SubmissionsController(_mediator.Object, _mapper.Object, _logger.Object);
        }

        [Test]
        public async Task ThenItShouldReturnAnOkResult()
        {
            // Act
            var actual = await _controller.GetSubmissionEvents();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<OkNegotiatedContentResult<PageOfResults<SubmissionEvent>>>(actual);
        }

        [Test]
        public async Task ThenItShouldReturnCorrectPageInformation()
        {
            // Act
            var actual = ((OkNegotiatedContentResult<PageOfResults<SubmissionEvent>>)await _controller.GetSubmissionEvents()).Content;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.PageNumber);
            Assert.AreEqual(10, actual.TotalNumberOfPages);
        }

        [Test]
        public async Task ThenItShouldReturnCorrectListOfEvents()
        {
            // Act
            var actual = ((OkNegotiatedContentResult<PageOfResults<SubmissionEvent>>)await _controller.GetSubmissionEvents()).Content;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Items);
            Assert.AreEqual(1, actual.Items.Length);
            Assert.AreEqual(123, actual.Items[0].Id);
        }

        [Test, TestCaseSource(nameof(RequestedFiltersTestCases))]
        public async Task ThenItShouldQueryWithTheRequestedFilters(int sinceEventId, DateTime? sinceTime)
        {
            // Act
            await _controller.GetSubmissionEvents(sinceEventId, sinceTime);

            // Assert
            _mediator.Verify(m => m.SendAsync(It.Is<GetSubmissionEventsQueryRequest>(r => r.SinceEventId == sinceEventId && r.SinceTime == sinceTime)), Times.Once);
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        public async Task ThenItShouldQueryWithCorrectPageInfo(int pageNumber)
        {
            // Act
            await _controller.GetSubmissionEvents(pageNumber: pageNumber);

            // Assert
            _mediator.Verify(m => m.SendAsync(It.Is<GetSubmissionEventsQueryRequest>(r => r.PageNumber == pageNumber && r.PageSize == 1000)), Times.Once);
        }

        [Test]
        public async Task ThenItShouldReturnBadRequestWhenValidationExceptionOccurs()
        {
            // Arrange
            var validationErrorMessage = "Your request is not valid for some reason";
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetSubmissionEventsQueryRequest>()))
                .ReturnsAsync(new GetSubmissionEventsQueryResponse
                {
                    IsValid = false,
                    Exception = new ValidationException(new[] { validationErrorMessage })
                });

            // Act
            var actual = await _controller.GetSubmissionEvents();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(actual);
            Assert.AreEqual(validationErrorMessage, ((BadRequestErrorMessageResult)actual).Message);
        }

        [Test]
        public async Task ThenItShouldReturnInternalServerErrorWhenGeneralExceptionOccurs()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetSubmissionEventsQueryRequest>()))
                .ThrowsAsync(new Exception("Something really bad happened"));

            // Act
            var actual = await _controller.GetSubmissionEvents();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<InternalServerErrorResult>(actual);
        }



        private static object[] RequestedFiltersTestCases => new object[]
        {
            new object[] {3, null},
            new object[] {0, new DateTime(2017, 2, 3, 12, 45, 36)},
            new object[] {3, new DateTime(2017, 2, 3, 12, 45, 36)}
        };
    }
}