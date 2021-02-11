using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.Mapping;
using SFA.DAS.Provider.Events.Application.Period.GetPeriodsQuery;
using SFA.DAS.Provider.Events.Application.Repositories;

namespace SFA.DAS.Provider.Events.Application.UnitTests.Period.GetPeriodsQuery.GetPeriodsQueryHandler
{
    public class WhenHandling
    {
        private GetPeriodsQueryRequest _request;
        private PeriodEntity _periodEntity1;
        private PeriodEntity _periodEntity2;
        private Mock<IPeriodRepository> _periodRepository;
        private Mock<IMapper> _mapper;
        private Application.Period.GetPeriodsQuery.GetPeriodsQueryHandler _handler;

        [SetUp]
        public void Arrange()
        {
            _request = new GetPeriodsQueryRequest();

            _periodEntity1 = new PeriodEntity
            {
                Id = "1617-R12",
                CalendarMonth = 8,
                CalendarYear = 2017,
                AccountDataValidAt = new DateTime(2017, 8, 31),
                CommitmentDataValidAt = new DateTime(2017, 9, 1),
                CompletionDateTime = new DateTime(2017, 9, 5, 19, 27, 34)
            };
            _periodEntity2 = new PeriodEntity
            {
                Id = "1718-R01",
                CalendarMonth = 9,
                CalendarYear = 2017,
                AccountDataValidAt = new DateTime(2017, 10, 2),
                CommitmentDataValidAt = new DateTime(2017, 9, 30),
                CompletionDateTime = new DateTime(2017, 10, 4, 23, 34, 10)
            };

            _periodRepository = new Mock<IPeriodRepository>();
            _periodRepository.Setup(r => r.GetPeriods())
                .Returns(Task.FromResult(new[] { _periodEntity1, _periodEntity2 }));

            _mapper = new Mock<IMapper>();
            _mapper.Setup(m => m.Map<Data.CollectionPeriod[]>(It.IsAny<PeriodEntity[]>()))
                .Returns((PeriodEntity[] source) =>
                {
                    return source.Select(e => new Data.CollectionPeriod
                    {
                        Id = e.Id,
                        CalendarMonth = e.CalendarMonth,
                        CalendarYear = e.CalendarYear,
                        AccountDataValidAt = e.AccountDataValidAt,
                        CommitmentDataValidAt = e.CommitmentDataValidAt,
                        CompletionDateTime = e.CompletionDateTime
                    }).ToArray();
                });

            _handler = new Application.Period.GetPeriodsQuery.GetPeriodsQueryHandler(_periodRepository.Object, _mapper.Object);
        }

        [Test]
        public async Task ThenItShouldReturnAValidResponseWithPeriods()
        {
            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);
            Assert.IsNotNull(actual.Result);
            Assert.AreEqual(2, actual.Result.Length);
            AssertPeriodForEntity(actual.Result[0], _periodEntity1);
            AssertPeriodForEntity(actual.Result[1], _periodEntity2);
        }

        [Test]
        public async Task ThenItShouldReturnAnInvalidResponseIfTheRepositoryThrowsAnException()
        {
            // Arrange
            var ex = new Exception("Error");
            _periodRepository.Setup(r => r.GetPeriods())
                .Throws(ex);

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.AreSame(ex, actual.Exception);
        }

        private void AssertPeriodForEntity(Data.CollectionPeriod period, PeriodEntity entity)
        {
            Assert.AreEqual(entity.Id, period.Id);
            Assert.AreEqual(entity.CalendarMonth, period.CalendarMonth);
            Assert.AreEqual(entity.CalendarYear, period.CalendarYear);
            Assert.AreEqual(entity.AccountDataValidAt, period.AccountDataValidAt);
            Assert.AreEqual(entity.CommitmentDataValidAt, period.CommitmentDataValidAt);
            Assert.AreEqual(entity.CompletionDateTime, period.CompletionDateTime);
        }
    }
}
