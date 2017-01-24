using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Period.GetPeriodsQuery;
using SFA.DAS.Provider.Events.Application.Validation;
using SFA.DAS.Provider.Events.Domain;
using SFA.DAS.Provider.Events.Domain.Mapping;
using CalendarPeriod = SFA.DAS.Provider.Events.Api.Types.CalendarPeriod;

namespace SFA.DAS.Provider.Events.Api.UnitTests.Controllers.PeriodEndsController
{
    public class WhenGettingListOfPeriodEnds
    {
        private Period _period1;
        private Period _period2;
        private Mock<IMediator> _mediator;
        private Mock<IMapper> _mapper;
        private Mock<ILogger> _logger;
        private Api.Controllers.PeriodEndsController _controller;
        private Mock<UrlHelper> _urlHelper;

        [SetUp]
        public void Arrange()
        {
            _period1 = new Period
            {
                Id= Guid.NewGuid().ToString(),
                CalendarMonth = 9,
                CalendarYear =  2017,
                AccountDataValidAt = new DateTime(2017, 9, 1),
                CommitmentDataValidAt = new DateTime(2017, 9, 2),
                CompletionDateTime = new DateTime(2017, 9, 3)
            };
            _period2 = new Period
            {
                Id = Guid.NewGuid().ToString(),
                CalendarMonth = 10,
                CalendarYear = 2017,
                AccountDataValidAt = new DateTime(2017, 10, 2),
                CommitmentDataValidAt = new DateTime(2017, 10, 1),
                CompletionDateTime = new DateTime(2017, 10, 3)
            };

            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetPeriodsQueryRequest>()))
                .Returns(Task.FromResult(new GetPeriodsQueryResponse
                {
                    IsValid = true,
                    Result = new[] { _period1, _period2 }
                }));

            _mapper = new Mock<IMapper>();
            _mapper.Setup(m => m.Map<PeriodEnd[]>(It.IsAny<Period[]>()))
                .Returns((Period[] source) =>
                {
                    return source.Select(p => new PeriodEnd
                    {
                        Id = p.Id,
                        CalendarPeriod = new CalendarPeriod
                        {
                            Month = p.CalendarMonth,
                            Year = p.CalendarYear
                        },
                        ReferenceData = new ReferenceDataDetails
                        {
                            AccountDataValidAt = p.AccountDataValidAt,
                            CommitmentDataValidAt = p.CommitmentDataValidAt
                        },
                        CompletionDateTime = p.CompletionDateTime
                    }).ToArray();
                });

            _logger = new Mock<ILogger>();
            _logger.Setup(l => l.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<Exception, string, object[]>((ex, msg, args) =>
                {
                    Console.WriteLine($"Error Logged\n{msg}\n{ex}");
                });

            _urlHelper = new Mock<UrlHelper>();
            _urlHelper.Setup(h => h.Link(It.IsAny<string>(), It.IsAny<object>()))
                .Returns("payments-url");

            _controller = new Api.Controllers.PeriodEndsController(_mediator.Object, _mapper.Object, _logger.Object);
            _controller.Url = _urlHelper.Object;
        }

        [Test]
        public async Task ThenItShouldReturnOkResultWithPeriodEndArray()
        {
            // Act
            var actual = await _controller.ListPeriodEnds();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<OkNegotiatedContentResult<PeriodEnd[]>>(actual);

            var periods = ((OkNegotiatedContentResult<PeriodEnd[]>) actual).Content;
            Assert.IsNotNull(periods);
            Assert.AreEqual(2, periods.Length);
            AssertPeriodForDomainObject(_period1, periods[0]);
            AssertPeriodForDomainObject(_period2, periods[1]);
        }

        [Test]
        public async Task AndAValidationExceptionReturnedThenItShouldReturnBadRequest()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetPeriodsQueryRequest>()))
                .Returns(Task.FromResult(new GetPeriodsQueryResponse
                {
                    IsValid = false,
                    Exception = new ValidationException(new[] { "Unit tests" })
                }));

            // Act
            var actual = await _controller.ListPeriodEnds();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(actual);
            Assert.AreEqual("Unit tests", ((BadRequestErrorMessageResult)actual).Message);
        }

        [Test]
        public async Task AndAExceptionIsReturnedThenItShouldReturnInternalServerError()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetPeriodsQueryRequest>()))
                .Returns(Task.FromResult(new GetPeriodsQueryResponse
                {
                    IsValid = false,
                    Exception = new Exception("Unit tests")
                }));

            // Act
            var actual = await _controller.ListPeriodEnds();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<InternalServerErrorResult>(actual);
        }

        private void AssertPeriodForDomainObject(Period period, PeriodEnd periodEnd)
        {
            Assert.AreEqual(period.Id, period.Id);
            Assert.AreEqual(period.CalendarMonth, periodEnd.CalendarPeriod.Month);
            Assert.AreEqual(period.CalendarYear, periodEnd.CalendarPeriod.Year);
            Assert.AreEqual(period.AccountDataValidAt, periodEnd.ReferenceData.AccountDataValidAt);
            Assert.AreEqual(period.CommitmentDataValidAt, periodEnd.ReferenceData.CommitmentDataValidAt);
            Assert.AreEqual(period.CompletionDateTime, periodEnd.CompletionDateTime);
        }
    }
}
