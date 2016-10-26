using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Events.Application.Payments.GetPaymentsQuery;
using SFA.DAS.Payments.Events.Domain;
using SFA.DAS.Payments.Events.Domain.Data;
using SFA.DAS.Payments.Events.Domain.Data.Entities;
using SFA.DAS.Payments.Events.Domain.Mapping;

namespace SFA.DAS.Payments.Events.Application.UnitTests.Payments.GetPaymentsQuery.GetPaymentsQueryHandler
{
    public class WhenHandling
    {
        private Mock<IPaymentRepository> _paymentRepository;
        private Mock<IMapper> _mapper;
        private Application.Payments.GetPaymentsQuery.GetPaymentsQueryHandler _handler;
        private GetPaymentsQueryRequest _request;
        private PageOfEntities<PaymentEntity> _pageOfEntities;

        [SetUp]
        public void Arrange()
        {
            _request = new GetPaymentsQueryRequest
            {
                PageNumber = 1,
                PageSize = 1000
            };

            _pageOfEntities = new PageOfEntities<PaymentEntity>
            {
                PageNumber = 1,
                NumberOfPages = 10,
                Items = new[]
                {
                    new PaymentEntity
                    {
                        Id = "PAYMENT-1",
                        Ukprn = 100,
                        Uln = 101,
                        EmployerAccountId = "ACC001",
                        ApprenticeshipId = 102,
                        DeliveryPeriodMonth = 9,
                        DeliveryPeriodYear = 2017,
                        CollectionPeriodMonth = 10,
                        CollectionPeriodYear = 2017,
                        EvidenceSubmittedOn = new DateTime(2017,10,2),
                        EmployerAccountVersion = "AV1",
                        ApprenticeshipVersion = "CV1",
                        FundingSource = 1,
                        TransactionType = 1,
                        Amount = 123.45m
                    },
                    new PaymentEntity
                    {
                        Id = "PAYMENT-2",
                        Ukprn = 200,
                        Uln = 201,
                        EmployerAccountId = "ACC002",
                        ApprenticeshipId = 202,
                        DeliveryPeriodMonth = 10,
                        DeliveryPeriodYear = 2017,
                        CollectionPeriodMonth = 10,
                        CollectionPeriodYear = 2017,
                        EvidenceSubmittedOn = new DateTime(2017,6,15),
                        EmployerAccountVersion = "AV2",
                        ApprenticeshipVersion = "CV2",
                        FundingSource = 2,
                        TransactionType = 2,
                        Amount = 987.65m
                    }
                }
            };

            _paymentRepository = new Mock<IPaymentRepository>();
            _paymentRepository.Setup(r => r.GetPayments(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(_pageOfEntities));
            _paymentRepository.Setup(r => r.GetPaymentsForAccount(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(_pageOfEntities));
            _paymentRepository.Setup(r => r.GetPaymentsForPeriod(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(_pageOfEntities));
            _paymentRepository.Setup(r => r.GetPaymentsForAccountInPeriod(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(_pageOfEntities));

            _mapper = new Mock<IMapper>();
            _mapper.Setup(m => m.Map<PageOfResults<Payment>>(It.IsAny<PageOfEntities<PaymentEntity>>()))
                .Returns((PageOfEntities<PaymentEntity> source) =>
                {
                    return new PageOfResults<Payment>
                    {
                        PageNumber = source.PageNumber,
                        TotalNumberOfPages = source.NumberOfPages,
                        Items = source.Items.Select(e => new Payment
                        {
                            Id = e.Id,
                            Ukprn = e.Ukprn,
                            Uln = e.Uln,
                            EmployerAccountId = e.EmployerAccountId,
                            ApprenticeshipId = e.ApprenticeshipId,
                            DeliveryPeriod = new CalendarPeriod
                            {
                                Month = e.DeliveryPeriodMonth,
                                Year = e.DeliveryPeriodYear
                            },
                            CollectionPeriod = new NamedCalendarPeriod
                            {
                                Id = e.CollectionPeriodId,
                                Month = e.CollectionPeriodMonth,
                                Year = e.CollectionPeriodYear
                            },
                            EvidenceSubmittedOn = e.EvidenceSubmittedOn,
                            EmployerAccountVersion = e.EmployerAccountVersion,
                            ApprenticeshipVersion = e.ApprenticeshipVersion,
                            FundingSource = (FundingSource)e.FundingSource,
                            TransactionType = (TransactionType)e.TransactionType,
                            Amount = e.Amount
                        }).ToArray()
                    };
                });

            _handler = new Application.Payments.GetPaymentsQuery.GetPaymentsQueryHandler(_paymentRepository.Object, _mapper.Object);
        }

        [Test]
        public async Task ThenItShouldReturnAValidResponseWithAPageOfResults()
        {
            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);
            Assert.IsNotNull(actual.Result);
            Assert.AreEqual(_request.PageNumber, actual.Result.PageNumber);
            Assert.AreEqual(10, actual.Result.TotalNumberOfPages);
            Assert.IsNotNull(actual.Result.Items);
            Assert.AreEqual(2, actual.Result.Items.Length);
        }

        [Test]
        public async Task ThenItShouldGetAnUnfilteredPageOfPaymentsIfAccountIdAndPeriodNotOnRequest()
        {
            // Act
            await _handler.Handle(_request);

            // Assert
            _paymentRepository.Verify(r => r.GetPayments(_request.PageNumber, _request.PageSize), Times.Once);
        }

        [Test]
        public async Task ThenItShouldGetAnPageOfPaymentsForPeriodIfPeriodOnRequest()
        {
            // Arrange
            _request.Period = new Domain.Period
            {
                Id = "0917",
                CalendarMonth = 9,
                CalendarYear = 2017
            };

            // Act
            await _handler.Handle(_request);

            // Assert
            _paymentRepository.Verify(r => r.GetPaymentsForPeriod(_request.Period.CalendarYear, _request.Period.CalendarMonth,
                _request.PageNumber, _request.PageSize), Times.Once);
        }

        [Test]
        public async Task ThenItShouldGetAPageOfPaymentsForAccountIfAccountIdOnRequest()
        {
            // Arrange
            _request.EmployerAccountId = "TEST";

            // Act
            await _handler.Handle(_request);

            // Assert
            _paymentRepository.Verify(r => r.GetPaymentsForAccount(_request.EmployerAccountId, _request.PageNumber, _request.PageSize), Times.Once);
        }

        [Test]
        public async Task ThenItShouldGetAnPageOfPaymentsForAccountInPeriodIfPeriodAndAccountOnRequest()
        {
            // Arrange
            _request.Period = new Domain.Period
            {
                Id = "0917",
                CalendarMonth = 9,
                CalendarYear = 2017
            };
            _request.EmployerAccountId = "TEST";

            // Act
            await _handler.Handle(_request);

            // Assert
            _paymentRepository.Verify(r => r.GetPaymentsForAccountInPeriod(_request.EmployerAccountId, _request.Period.CalendarYear, _request.Period.CalendarMonth,
                _request.PageNumber, _request.PageSize), Times.Once);
        }

        [Test]
        public async Task ThenItShouldMapEntitiesToDomainObjects()
        {
            // Act
            await _handler.Handle(_request);

            // Assert
            _mapper.Verify(m => m.Map<PageOfResults<Payment>>(It.IsAny<PageOfEntities<PaymentEntity>>()), Times.Once);
        }

        [Test]
        public async Task ThenItShouldReturnAnInvalidResponseWithExceptionIfExceptionThrownFromRepository()
        {
            // Arrange
            var ex = new Exception();
            _paymentRepository.Setup(r => r.GetPayments(It.IsAny<int>(), It.IsAny<int>()))
                .Throws(ex);

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.AreSame(ex, actual.Exception);
        }
    }
}
