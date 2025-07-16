using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture.NUnit3;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Api.UnitTests.Mocks;
using SFA.DAS.Provider.Events.Application.Data;
using SFA.DAS.Provider.Events.Application.Payments.GetPaymentsQuery;
using SFA.DAS.Provider.Events.Application.Period.GetPeriodQuery;
using SFA.DAS.Provider.Events.Application.Validation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace SFA.DAS.Provider.Events.Api.UnitTests.Controllers.PaymentsController
{
    public class WhenGettingAListOfPayments : BaseMockController
    {
        private static string _periodId = "PERIOD-1";
        private const string EmployerAccountId = "ACCOUNT-1";
        private const int Ukprn = 432508734;
        private const int Page = 2;
        private const int PageSize = 10000;
        private const int TotalNumberOfPages = 100;

        private Payment _payment1;
        private CollectionPeriod _period;
        private Mock<IMediator> _mediator;
        private Api.Controllers.PaymentsController _controller;

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
                CollectionPeriod = new NamedCalendarPeriod
                {
                    Id = "1718-R02",
                    Month = 9,
                    Year = 2017
                },
                DeliveryPeriod = new CalendarPeriod
                {
                    Month = 8,
                    Year = 2017
                },
                EvidenceSubmittedOn = DateTime.Today,
                EmployerAccountVersion = "20170601",
                ApprenticeshipVersion = "V1",
                FundingSource = FundingSource.Levy,
                FundingAccountId = 666,
                TransactionType = TransactionType.Learning,
                Amount = 1234.56m,
                StandardCode = 25,
                ContractType = ContractType.ContractWithEmployer
            };
            _period = new CollectionPeriod
            {
                Id = _periodId,
                CalendarMonth = 9,
                CalendarYear = 2017
            };

            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.Is<GetPeriodQueryRequest>(r => r.PeriodId == _periodId)))
                .ReturnsAsync(new GetPeriodQueryResponse
                {
                    IsValid = true,
                    Result = _period
                });

            _mediator.Setup(m => m.SendAsync(It.Is<GetPaymentsQueryRequest>(r => r.Period == _period
                                                                                       && r.EmployerAccountId == EmployerAccountId
                                                                                       && r.PageNumber == Page
                                                                                       && r.PageSize == PageSize
                                                                                       && r.Ukprn == Ukprn)))
                .ReturnsAsync(new GetPaymentsQueryResponse
                {
                    IsValid = true,
                    Result = new PageOfResults<Payment>
                    {
                        PageNumber = Page,
                        TotalNumberOfPages = TotalNumberOfPages,
                        Items = new[] { _payment1 }
                    }
                });

            //var telemetryClient = InitializeMockTelemetryChannel();
            //telemetryClient.Setup(l => l.TrackException(It.IsAny<Exception>(), It.IsAny<Dictionary<string,string>>(), It.IsAny<Dictionary<string,double>>()))
            //    .Callback<Exception, string, object[]>((ex, msg, args) =>
            //    {
            //        Console.WriteLine($"Error Logged\n{msg}\n{ex}");
            //    });

            _controller = new Api.Controllers.PaymentsController(_mediator.Object, telemetryClient);
        }

        [Test]
        public async Task ThenItShouldReturnAOkResult()
        {
            //Arrange
            _periodId = "1920-R12";
            _mediator.Setup(m => m.SendAsync(It.Is<GetPaymentsQueryRequest>(r => 
                    r.Period.AcademicYear == 1920
                    && r.Period.Period == 12
                    && r.EmployerAccountId == EmployerAccountId
                    && r.PageNumber == Page
                    && r.PageSize == PageSize
                    && r.Ukprn == Ukprn)))
                .ReturnsAsync(new GetPaymentsQueryResponse
                {
                    IsValid = true,
                    Result = new PageOfResults<Payment>
                    {
                        PageNumber = Page,
                        TotalNumberOfPages = TotalNumberOfPages,
                        Items = new[] { _payment1 }
                    }
                });
            
            // Act
            var actual = await _controller
                .GetListOfPayments(_periodId, EmployerAccountId, Page, Ukprn)
                .ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<OkNegotiatedContentResult<PageOfResults<Payment>>>(actual);
        }

        [Test]
        [InlineAutoData(25L, null, null, null)]
        [InlineAutoData(null, 550, 20, 6)]
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
                CollectionPeriod = new NamedCalendarPeriod
                {
                    Id = "1718-R02",
                    Month = 9,
                    Year = 2017
                },
                DeliveryPeriod = new CalendarPeriod
                {
                    Month = 8,
                    Year = 2017
                },
                EvidenceSubmittedOn = DateTime.Today,
                EmployerAccountVersion = "20170601",
                ApprenticeshipVersion = "V1",
                FundingSource = FundingSource.Levy,
                FundingAccountId = 777,
                TransactionType = TransactionType.Learning,
                Amount = 1234.56m,
                StandardCode = standardCode,
                FrameworkCode = frameworkCode,
                ProgrammeType = programmeType,
                PathwayCode = pathwayCode,
                ContractType = ContractType.ContractWithEmployer
            };

            _mediator.Setup(m => m.SendAsync(It.Is<GetPaymentsQueryRequest>(r => 
                                                                                          r.Period == _period
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
            var actual = await _controller
                .GetListOfPayments(_periodId, EmployerAccountId, Page, Ukprn)
                .ConfigureAwait(false);

            // Assert
            var page = ((OkNegotiatedContentResult<PageOfResults<Payment>>)actual).Content;
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
            Assert.AreEqual(payment.FundingAccountId, actualPayment.FundingAccountId);
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
            _periodId = "1920-R12";
            _mediator.Setup(m => m.SendAsync(It.Is<GetPeriodQueryRequest>(r => r.PeriodId == _periodId)))
                .Returns(Task.FromResult(new GetPeriodQueryResponse
                {
                    IsValid = true,
                    Result = null
                }));
            _mediator.Setup(m => m.SendAsync(It.Is<GetPaymentsQueryRequest>(r => 
                    r.Period.AcademicYear == 1920
                    && r.Period.Period == 12
                    && r.EmployerAccountId == EmployerAccountId
                    && r.PageNumber == Page
                    && r.PageSize == PageSize)))
                .ReturnsAsync(new GetPaymentsQueryResponse
                {
                    IsValid = true,
                    Result = new PageOfResults<Payment>
                    {
                        PageNumber = Page,
                        TotalNumberOfPages = 0,
                        Items = new Payment[0],
                    }
                });

            // Act
            var actual = await _controller
                .GetListOfPayments(_periodId, EmployerAccountId, Page)
                .ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<OkNegotiatedContentResult<PageOfResults<Payment>>>(actual);

            var page = ((OkNegotiatedContentResult<PageOfResults<Payment>>)actual).Content;
            Assert.IsNotNull(page);
            Assert.AreEqual(Page, page.PageNumber);
            Assert.AreEqual(0, page.TotalNumberOfPages);
            Assert.AreEqual(0, page.Items.Length);
        }

        [Test]
        public async Task AndAValidationExceptionReturnedThenItShouldReturnBadRequest()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.Is<GetPeriodQueryRequest>(r => r.PeriodId == _periodId)))
                .Returns(Task.FromResult(new GetPeriodQueryResponse
                {
                    IsValid = false,
                    Exception = new ValidationException(new[] { "Unit tests" })
                }));

            // Act
            var actual = await _controller
                .GetListOfPayments(_periodId, EmployerAccountId, Page)
                .ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(actual);
            Assert.AreEqual("Unit tests", ((BadRequestErrorMessageResult)actual).Message);
        }

        [Test]
        public async Task AndAExceptionIsReturnedThenItShouldReturnInternalServerError()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.Is<GetPeriodQueryRequest>(r => r.PeriodId == _periodId)))
                .Returns(Task.FromResult(new GetPeriodQueryResponse
                {
                    IsValid = false,
                    Exception = new Exception("Unit tests")
                }));

            // Act
            var actual = await _controller
                .GetListOfPayments(_periodId, EmployerAccountId, Page)
                .ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<InternalServerErrorResult>(actual);
        }

        [Test]
        public async Task TheDefaultPageSizeShouldBeTenThousand()
        {
            _periodId = "1920-R12";
            await _controller
                .GetListOfPayments(_periodId, EmployerAccountId)
                .ConfigureAwait(false);

            _mediator.Verify(m => m.SendAsync(It.Is<GetPaymentsQueryRequest>(r => r.PageSize == 10000)), Times.Once);
        }
    }
}
