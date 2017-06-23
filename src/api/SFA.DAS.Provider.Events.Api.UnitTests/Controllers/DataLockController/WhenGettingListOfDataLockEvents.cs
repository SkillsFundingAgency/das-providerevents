using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.DataLock.GetDataLockEventsQuery;
using SFA.DAS.Provider.Events.Application.Validation;
using SFA.DAS.Provider.Events.Domain.Mapping;

namespace SFA.DAS.Provider.Events.Api.UnitTests.Controllers.DataLockController
{
    public class WhenGettingListOfDataLockEvents
    {
        private Mock<IMediator> _mediator;
        private Mock<IMapper> _mapper;
        private Mock<ILogger> _logger;
        private Api.Controllers.DataLockController _controller;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetDataLockEventsQueryRequest>()))
                .ReturnsAsync(new GetDataLockEventsQueryResponse
                {
                    IsValid = true,
                    Result = new Domain.PageOfResults<Domain.DataLockEvent>
                    {
                        PageNumber = 1,
                        TotalNumberOfPages = 10,
                        Items = new[]
                        {
                            new Domain.DataLockEvent
                            {
                                Id = 123,
                                Errors = new []
                                {
                                    new Domain.DataLockEventError
                                    {
                                        ErrorCode = "Err1",
                                        SystemDescription = "Error 1"
                                    }
                                },
                                Periods = new []
                                {
                                    new Domain.DataLockEventPeriod
                                    {
                                        ApprenticeshipVersion = "1-015",
                                        Period = new Domain.NamedCalendarPeriod
                                        {
                                            Id = "1617-R09",
                                            Month = 4,
                                            Year = 2017
                                        },
                                        IsPayable = true,
                                        TransactionType = Domain.TransactionType.Learning
                                    }
                                },
                                Apprenticeships = new []
                                {
                                    new Domain.DataLockEventApprenticeship
                                    {
                                        Version = 19,
                                        StartDate = new DateTime(2017, 5, 1),
                                        StandardCode = 27,
                                        NegotiatedPrice = 17500m,
                                        EffectiveDate = new DateTime(2017, 5, 1)
                                    }
                                }
                            }
                        }
                    }
                });

            _mapper = new Mock<IMapper>();
            _mapper.Setup(m => m.Map<PageOfResults<DataLockEvent>>(It.IsAny<Domain.PageOfResults<Domain.DataLockEvent>>()))
                .Returns((Domain.PageOfResults<Domain.DataLockEvent> source) =>
                {
                    return new PageOfResults<DataLockEvent>
                    {
                        PageNumber = source.PageNumber,
                        TotalNumberOfPages = source.TotalNumberOfPages,
                        Items = source.Items.Select(e => new DataLockEvent
                        {
                            Id = e.Id,
                            Errors = new []
                            {
                                new DataLockEventError
                                {
                                    ErrorCode = e.Errors[0].ErrorCode,
                                    SystemDescription = e.Errors[0].SystemDescription
                                }
                            },
                            Periods = new []
                            {
                                new DataLockEventPeriod
                                {
                                    ApprenticeshipVersion = e.Periods[0].ApprenticeshipVersion,
                                    Period = new NamedCalendarPeriod
                                    {
                                        Id = e.Periods[0].Period.Id,
                                        Month = e.Periods[0].Period.Month,
                                        Year = e.Periods[0].Period.Year
                                    },
                                    IsPayable = e.Periods[0].IsPayable
                                }
                            },
                            Apprenticeships = new []
                            {
                                new DataLockEventApprenticeship
                                {
                                    Version = e.Apprenticeships[0].Version,
                                    StartDate = e.Apprenticeships[0].StartDate,
                                    StandardCode = e.Apprenticeships[0].StandardCode,
                                    ProgrammeType = e.Apprenticeships[0].ProgrammeType,
                                    FrameworkCode = e.Apprenticeships[0].FrameworkCode,
                                    PathwayCode = e.Apprenticeships[0].PathwayCode,
                                    NegotiatedPrice = e.Apprenticeships[0].NegotiatedPrice,
                                    EffectiveDate = e.Apprenticeships[0].EffectiveDate
                                }
                            }
                        }).ToArray()
                    };
                });

            _logger = new Mock<ILogger>();

            _controller = new Api.Controllers.DataLockController(_mediator.Object, _mapper.Object, _logger.Object);
        }

        [Test]
        public async Task ThenItShouldReturnAnOkResult()
        {
            // Act
            var actual = await _controller.GetDataLockEventsV2();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<OkNegotiatedContentResult<PageOfResults<DataLockEvent>>>(actual);
        }

        [Test]
        public async Task ThenItShouldReturnCorrectPageInformation()
        {
            // Act
            var actual = ((OkNegotiatedContentResult<PageOfResults<DataLockEvent>>)await _controller.GetDataLockEventsV2()).Content;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.PageNumber);
            Assert.AreEqual(10, actual.TotalNumberOfPages);
        }

        [Test]
        public async Task ThenItShouldReturnCorrectListOfEvents()
        {
            // Act
            var actual = ((OkNegotiatedContentResult<PageOfResults<DataLockEvent>>)await _controller.GetDataLockEventsV2()).Content;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Items);
            Assert.AreEqual(1, actual.Items.Length);
            Assert.AreEqual(123, actual.Items[0].Id);
        }

        [Test, TestCaseSource(nameof(RequestedFiltersTestCases))]
        public async Task ThenItShouldQueryWithTheRequestedFilters(int sinceEventId, DateTime? sinceTime, string employerAccountId, long ukprn)
        {
            // Act
            await _controller.GetDataLockEventsV2(sinceEventId, sinceTime, employerAccountId, ukprn);

            // Assert
            _mediator.Verify(m => m.SendAsync(It.Is<GetDataLockEventsQueryRequest>(r => r.SinceEventId == sinceEventId && r.SinceTime == sinceTime && r.EmployerAccountId == employerAccountId && r.Ukprn == ukprn)), Times.Once);
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        public async Task ThenItShouldQueryWithCorrectPageInfo(int pageNumber)
        {
            // Act
            await _controller.GetDataLockEventsV2(pageNumber: pageNumber);

            // Assert
            _mediator.Verify(m => m.SendAsync(It.Is<GetDataLockEventsQueryRequest>(r => r.PageNumber == pageNumber && r.PageSize == 250)), Times.Once);
        }

        [Test]
        public async Task ThenItShouldReturnBadRequestWhenValidationExceptionOccurs()
        {
            // Arrange
            var validationErrorMessage = "Your request is not valid for some reason";
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetDataLockEventsQueryRequest>()))
                .ReturnsAsync(new GetDataLockEventsQueryResponse
                {
                    IsValid = false,
                    Exception = new ValidationException(new[] { validationErrorMessage })
                });

            // Act
            var actual = await _controller.GetDataLockEventsV2();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(actual);
            Assert.AreEqual(validationErrorMessage, ((BadRequestErrorMessageResult)actual).Message);
        }

        [Test]
        public async Task ThenItShouldReturnInternalServerErrorWhenGeneralExceptionOccurs()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetDataLockEventsQueryRequest>()))
                .ThrowsAsync(new Exception("Something really bad happened"));

            // Act
            var actual = await _controller.GetDataLockEventsV2();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<InternalServerErrorResult>(actual);
        }

        private static object[] RequestedFiltersTestCases => new object[]
        {
            new object[] {0, new DateTime(2017, 2, 8, 15, 15, 09), "Acc1", 1},
            new object[] {7, null, "Acc1", 1},
            new object[] {0, null, "Acc1", 1},

            new object[] {0, new DateTime(2017, 2, 8, 15, 15, 09), null, 1},
            new object[] {7, null, null, 1},
            new object[] {0, null, null, 1},

            new object[] {0, new DateTime(2017, 2, 8, 15, 15, 09), "Acc1", 0},
            new object[] {7, null, "Acc1", 0},
            new object[] {0, null, "Acc1", 0},

            new object[] {3, null, null, 0},
            new object[] {0, new DateTime(2017, 2, 3, 12, 45, 36), null, 0},
            new object[] {3, new DateTime(2017, 2, 3, 12, 45, 36), null, 0}
        };
    }
}