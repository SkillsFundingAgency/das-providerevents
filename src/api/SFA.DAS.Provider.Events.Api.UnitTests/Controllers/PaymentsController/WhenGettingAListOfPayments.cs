using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Application.Payments.GetPaymentsQuery;
using SFA.DAS.Provider.Events.Application.Period.GetPeriodQuery;
using SFA.DAS.Provider.Events.Application.Validation;
using SFA.DAS.Provider.Events.Domain;
using SFA.DAS.Provider.Events.Domain.Mapping;
using CalendarPeriod = SFA.DAS.Provider.Events.Api.Types.CalendarPeriod;
using NamedCalendarPeriod = SFA.DAS.Provider.Events.Api.Types.NamedCalendarPeriod;

namespace SFA.DAS.Provider.Events.Api.UnitTests.Controllers.PaymentsController
{
    public class WhenGettingAListOfPayments
    {
        private const string PeriodId = "PERIOD-1";
        private const string EmployerAccountId = "ACCOUNT-1";
        private const int Ukprn = 432508734;
        private const int Page = 2;
        private const int PageSize = 1000;
        private const int TotalNumberOfPages = 100;

        private Payment _payment1;
        private Period _period;
        private Mock<IMediator> _mediator;
        private Mock<IMapper> _mapper;
        private Api.Controllers.PaymentsController _controller;
        private Mock<ILogger> _logger;

        [SetUp]
        public void Arrange()
        {
            _payment1 = new Payment
            {
                Id = Guid.NewGuid().ToString(),
                Ukprn = 123456,
                Uln = 987654,
                EmployerAccountId = EmployerAccountId,
                ApprenticeshipId = 147852,
                CollectionPeriod = new Domain.NamedCalendarPeriod
                {
                    Id = "1718-R02",
                    Month = 9,
                    Year = 2017
                },
                DeliveryPeriod = new Domain.CalendarPeriod
                {
                    Month = 8,
                    Year = 2017
                },
                EvidenceSubmittedOn = DateTime.Today,
                EmployerAccountVersion = "20170601",
                ApprenticeshipVersion = "V1",
                FundingSource = FundingSource.Levy,
                TransactionType = TransactionType.Learning,
                Amount = 1234.56m,
                StandardCode = 25,
                ContractType = ContractType.ContractWithEmployer
            };
            _period = new Period
            {
                Id = PeriodId,
                CalendarMonth = 9,
                CalendarYear = 2017
            };

            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.Is<GetPeriodQueryRequest>(r => r.PeriodId == PeriodId)))
                .Returns(Task.FromResult(new GetPeriodQueryResponse
                {
                    IsValid = true,
                    Result = _period
                }));
            _mediator.Setup(m => m.SendAsync(It.Is<GetPaymentsQueryRequest>(r => r.Period == _period
                                                                                       && r.EmployerAccountId == EmployerAccountId
                                                                                       && r.PageNumber == Page
                                                                                       && r.PageSize == PageSize
                                                                                       && r.Ukprn == Ukprn)))
                .Returns(Task.FromResult(new GetPaymentsQueryResponse
                {
                    IsValid = true,
                    Result = new PageOfResults<Payment>
                    {
                        PageNumber = Page,
                        TotalNumberOfPages = TotalNumberOfPages,
                        Items = new[] { _payment1 }
                    }
                }));

            _mapper = new Mock<IMapper>();
            _mapper.Setup(m => m.Map<Types.PageOfResults<Types.Payment>>(It.IsAny<PageOfResults<Payment>>()))
                .Returns((PageOfResults<Payment> source) =>
                {
                    return new Types.PageOfResults<Types.Payment>
                    {
                        PageNumber = source.PageNumber,
                        TotalNumberOfPages = source.TotalNumberOfPages,
                        Items = source.Items.Select(p => new Types.Payment
                        {
                            Id = p.Id,
                            Ukprn = p.Ukprn,
                            Uln = p.Uln,
                            EmployerAccountId = p.EmployerAccountId,
                            ApprenticeshipId = p.ApprenticeshipId,
                            CollectionPeriod = new NamedCalendarPeriod
                            {
                                Month = p.CollectionPeriod.Month,
                                Year = p.CollectionPeriod.Year
                            },
                            DeliveryPeriod = new CalendarPeriod
                            {
                                Month = p.DeliveryPeriod.Month,
                                Year = p.DeliveryPeriod.Year
                            },
                            EvidenceSubmittedOn = p.EvidenceSubmittedOn,
                            EmployerAccountVersion = p.EmployerAccountVersion,
                            ApprenticeshipVersion = p.ApprenticeshipVersion,
                            FundingSource = (Types.FundingSource)(int)p.FundingSource,
                            TransactionType = (Types.TransactionType)(int)p.TransactionType,
                            Amount = p.Amount,
                            StandardCode = p.StandardCode,
                            FrameworkCode = p.FrameworkCode,
                            ProgrammeType = p.ProgrammeType,
                            PathwayCode = p.PathwayCode,
                            ContractType = (Types.ContractType)(int)p.ContractType
                        }).ToArray()
                    };
                });

            _logger = new Mock<ILogger>();
            _logger.Setup(l => l.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<Exception, string, object[]>((ex, msg, args) =>
                {
                    Console.WriteLine($"Error Logged\n{msg}\n{ex}");
                });

            _controller = new Api.Controllers.PaymentsController(_mediator.Object, _mapper.Object, _logger.Object);
        }

        [Test]
        public async Task ThenItShouldReturnAOkResult()
        {
            // Act
            var actual = await _controller.GetListOfPayments(PeriodId, EmployerAccountId, Page, Ukprn);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<OkNegotiatedContentResult<Types.PageOfResults<Types.Payment>>>(actual);
        }

        [Test]
        [TestCase(25L, null, null, null)]
        [TestCase(null, 550, 20, 6)]
        public async Task ThenItShouldReturnCorrectTrainingCourseInformation(long? standardCode, int? frameworkCode, int? programmeType, int? pathwayCode)
        {
            // Assert
            var payment = new Payment
            {
                Id = Guid.NewGuid().ToString(),
                Ukprn = 123456,
                Uln = 987654,
                EmployerAccountId = EmployerAccountId,
                ApprenticeshipId = 147852,
                CollectionPeriod = new Domain.NamedCalendarPeriod
                {
                    Id = "1718-R02",
                    Month = 9,
                    Year = 2017
                },
                DeliveryPeriod = new Domain.CalendarPeriod
                {
                    Month = 8,
                    Year = 2017
                },
                EvidenceSubmittedOn = DateTime.Today,
                EmployerAccountVersion = "20170601",
                ApprenticeshipVersion = "V1",
                FundingSource = FundingSource.Levy,
                TransactionType = TransactionType.Learning,
                Amount = 1234.56m,
                StandardCode = standardCode,
                FrameworkCode = frameworkCode,
                ProgrammeType = programmeType,
                PathwayCode = pathwayCode,
                ContractType = ContractType.ContractWithEmployer
            };

            _mediator.Setup(m => m.SendAsync(It.Is<GetPaymentsQueryRequest>(r => r.Period == _period
                                                                                       && r.EmployerAccountId == EmployerAccountId
                                                                                       && r.PageNumber == Page
                                                                                       && r.PageSize == PageSize
                                                                                       && r.Ukprn == Ukprn)))
                .Returns(Task.FromResult(new GetPaymentsQueryResponse
                {
                    IsValid = true,
                    Result = new PageOfResults<Payment>
                    {
                        PageNumber = Page,
                        TotalNumberOfPages = TotalNumberOfPages,
                        Items = new[] { payment }
                    }
                }));

            // Act
            var actual = await _controller.GetListOfPayments(PeriodId, EmployerAccountId, Page, Ukprn);

            // Assert
            var page = ((OkNegotiatedContentResult<Types.PageOfResults<Types.Payment>>)actual).Content;
            Assert.IsNotNull(page);
            Assert.AreEqual(Page, page.PageNumber);
            Assert.AreEqual(TotalNumberOfPages, page.TotalNumberOfPages);
            Assert.AreEqual(1, page.Items.Length);

            var actualPayment = page.Items[0];
            Assert.AreEqual(payment.Id, actualPayment.Id);
            Assert.AreEqual(payment.Ukprn, actualPayment.Ukprn);
            Assert.AreEqual(payment.Uln, actualPayment.Uln);
            Assert.AreEqual(payment.EmployerAccountId, actualPayment.EmployerAccountId);
            Assert.AreEqual(payment.ApprenticeshipId, actualPayment.ApprenticeshipId);
            Assert.AreEqual(payment.CollectionPeriod.Month, actualPayment.CollectionPeriod.Month);
            Assert.AreEqual(payment.CollectionPeriod.Year, actualPayment.CollectionPeriod.Year);
            Assert.AreEqual(payment.CollectionPeriod.Month, actualPayment.CollectionPeriod.Month);
            Assert.AreEqual(payment.DeliveryPeriod.Year, actualPayment.DeliveryPeriod.Year);
            Assert.AreEqual(payment.EvidenceSubmittedOn, actualPayment.EvidenceSubmittedOn);
            Assert.AreEqual(payment.EmployerAccountVersion, actualPayment.EmployerAccountVersion);
            Assert.AreEqual(payment.ApprenticeshipVersion, actualPayment.ApprenticeshipVersion);
            Assert.AreEqual((int)payment.FundingSource, (int)actualPayment.FundingSource);
            Assert.AreEqual((int)payment.TransactionType, (int)actualPayment.TransactionType);
            Assert.AreEqual(payment.Amount, actualPayment.Amount);
            Assert.AreEqual(payment.StandardCode, actualPayment.StandardCode);
            Assert.AreEqual(payment.FrameworkCode, actualPayment.FrameworkCode);
            Assert.AreEqual(payment.ProgrammeType, actualPayment.ProgrammeType);
            Assert.AreEqual(payment.PathwayCode, actualPayment.PathwayCode);
            Assert.AreEqual((int)payment.ContractType, (int)actualPayment.ContractType);
        }

        [Test]
        public async Task ThenItShouldReturnAnOkResultWithNoItemsIfPeriodSpecifiedNotFound()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.Is<GetPeriodQueryRequest>(r => r.PeriodId == PeriodId)))
                .Returns(Task.FromResult(new GetPeriodQueryResponse
                {
                    IsValid = true,
                    Result = null
                }));

            // Act
            var actual = await _controller.GetListOfPayments(PeriodId, EmployerAccountId, Page);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<OkNegotiatedContentResult<Types.PageOfResults<Types.Payment>>>(actual);

            var page = ((OkNegotiatedContentResult<Types.PageOfResults<Types.Payment>>)actual).Content;
            Assert.IsNotNull(page);
            Assert.AreEqual(Page, page.PageNumber);
            Assert.AreEqual(0, page.TotalNumberOfPages);
            Assert.AreEqual(0, page.Items.Length);
        }

        [Test]
        public async Task AndAValidationExceptionReturnedThenItShouldReturnBadRequest()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.Is<GetPeriodQueryRequest>(r => r.PeriodId == PeriodId)))
                .Returns(Task.FromResult(new GetPeriodQueryResponse
                {
                    IsValid = false,
                    Exception = new ValidationException(new[] { "Unit tests" })
                }));

            // Act
            var actual = await _controller.GetListOfPayments(PeriodId, EmployerAccountId, Page);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(actual);
            Assert.AreEqual("Unit tests", ((BadRequestErrorMessageResult)actual).Message);
        }

        [Test]
        public async Task AndAExceptionIsReturnedThenItShouldReturnInternalServerError()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.Is<GetPeriodQueryRequest>(r => r.PeriodId == PeriodId)))
                .Returns(Task.FromResult(new GetPeriodQueryResponse
                {
                    IsValid = false,
                    Exception = new Exception("Unit tests")
                }));

            // Act
            var actual = await _controller.GetListOfPayments(PeriodId, EmployerAccountId, Page);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<InternalServerErrorResult>(actual);
        }
    }
}
