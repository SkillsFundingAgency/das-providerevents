using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Events.Application.Payments.GetPaymentsForPeriodQuery;
using SFA.DAS.Payments.Events.Application.Period.GetPeriodQuery;
using SFA.DAS.Payments.Events.Application.Validation;
using SFA.DAS.Payments.Events.Domain.Mapping;

namespace SFA.DAS.Payments.Events.Api.UnitTests.Controllers.PaymentsController
{
    public class WhenGettingAListOfPayments
    {
        private const string PeriodId = "PERIOD-1";
        private const string EmployerAccountId = "ACCOUNT-1";
        private const int Page = 2;
        private const int PageSize = 500;
        private const int TotalNumberOfPages = 100;

        private Domain.Payment _payment1;
        private Domain.Period _period;
        private Mock<IMediator> _mediator;
        private Mock<IMapper> _mapper;
        private Api.Controllers.PaymentsController _controller;

        [SetUp]
        public void Arrange()
        {
            _payment1 = new Domain.Payment
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
                FundingSource = Domain.FundingSource.Levy,
                TransactionType = Domain.TransactionType.Learning,
                Amount = 1234.56m
            };
            _period = new Domain.Period
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
            _mediator.Setup(m => m.SendAsync(It.Is<GetPaymentsForPeriodQueryRequest>(r => r.Period == _period
                                                                                       && r.EmployerAccountId == EmployerAccountId
                                                                                       && r.PageNumber == Page
                                                                                       && r.PageSize == PageSize)))
                .Returns(Task.FromResult(new GetPaymentsForPeriodQueryResponse
                {
                    IsValid = true,
                    Result = new Domain.PageOfResults<Domain.Payment>
                    {
                        PageNumber = Page,
                        TotalNumberOfPages = TotalNumberOfPages,
                        Items = new[] { _payment1 }
                    }
                }));

            _mapper = new Mock<IMapper>();
            _mapper.Setup(m => m.Map<Types.PageOfResults<Types.Payment>>(It.IsAny<Domain.PageOfResults<Domain.Payment>>()))
                .Returns((Domain.PageOfResults<Domain.Payment> source) =>
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
                            CollectionPeriod = new Types.NamedCalendarPeriod
                            {
                                Month = p.CollectionPeriod.Month,
                                Year = p.CollectionPeriod.Year
                            },
                            DeliveryPeriod = new Types.CalendarPeriod
                            {
                                Month = p.DeliveryPeriod.Month,
                                Year = p.DeliveryPeriod.Year
                            },
                            EvidenceSubmittedOn = p.EvidenceSubmittedOn,
                            EmployerAccountVersion = p.EmployerAccountVersion,
                            ApprenticeshipVersion = p.ApprenticeshipVersion,
                            FundingSource = (Types.FundingSource)(int)p.FundingSource,
                            TransactionType = (Types.TransactionType)(int)p.TransactionType,
                            Amount = p.Amount
                        }).ToArray()
                    };
                });

            _controller = new Api.Controllers.PaymentsController(_mediator.Object, _mapper.Object);
        }

        [Test]
        public async Task ThenItShouldReturnAOkResult()
        {
            // Act
            var actual = await _controller.GetListOfPayments(PeriodId, EmployerAccountId, Page, PageSize);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<OkNegotiatedContentResult<Types.PageOfResults<Types.Payment>>>(actual);
        }

        [Test]
        public async Task ThenItShouldReturnCorrectPageOfResults()
        {
            // Act
            var actual = await _controller.GetListOfPayments(PeriodId, EmployerAccountId, Page, PageSize);

            // Assert
            var page = ((OkNegotiatedContentResult<Types.PageOfResults<Types.Payment>>)actual).Content;
            Assert.IsNotNull(page);
            Assert.AreEqual(Page, page.PageNumber);
            Assert.AreEqual(TotalNumberOfPages, page.TotalNumberOfPages);
            Assert.AreEqual(1, page.Items.Length);

            var actualPayment = page.Items[0];
            Assert.AreEqual(_payment1.Id, actualPayment.Id);
            Assert.AreEqual(_payment1.Ukprn, actualPayment.Ukprn);
            Assert.AreEqual(_payment1.Uln, actualPayment.Uln);
            Assert.AreEqual(_payment1.EmployerAccountId, actualPayment.EmployerAccountId);
            Assert.AreEqual(_payment1.ApprenticeshipId, actualPayment.ApprenticeshipId);
            Assert.AreEqual(_payment1.CollectionPeriod.Month, actualPayment.CollectionPeriod.Month);
            Assert.AreEqual(_payment1.CollectionPeriod.Year, actualPayment.CollectionPeriod.Year);
            Assert.AreEqual(_payment1.CollectionPeriod.Month, actualPayment.CollectionPeriod.Month);
            Assert.AreEqual(_payment1.DeliveryPeriod.Year, actualPayment.DeliveryPeriod.Year);
            Assert.AreEqual(_payment1.EvidenceSubmittedOn, actualPayment.EvidenceSubmittedOn);
            Assert.AreEqual(_payment1.EmployerAccountVersion, actualPayment.EmployerAccountVersion);
            Assert.AreEqual(_payment1.ApprenticeshipVersion, actualPayment.ApprenticeshipVersion);
            Assert.AreEqual((int)_payment1.FundingSource, (int)actualPayment.FundingSource);
            Assert.AreEqual((int)_payment1.TransactionType, (int)actualPayment.TransactionType);
            Assert.AreEqual(_payment1.Amount, actualPayment.Amount);
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
            var actual = await _controller.GetListOfPayments(PeriodId, EmployerAccountId, Page, PageSize);

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
            var actual = await _controller.GetListOfPayments(PeriodId, EmployerAccountId, Page, PageSize);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<InternalServerErrorResult>(actual);
        }
    }
}
