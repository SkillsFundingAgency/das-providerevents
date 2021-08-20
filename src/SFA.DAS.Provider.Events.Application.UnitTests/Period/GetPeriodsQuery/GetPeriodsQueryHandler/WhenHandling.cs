using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Plumbing.Mapping;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.Mapping;
using SFA.DAS.Provider.Events.Application.Period.GetPeriodsQuery;
using SFA.DAS.Provider.Events.Application.Repositories;
using SFA.DAS.Provider.Events.Infrastructure.Mapping;

namespace SFA.DAS.Provider.Events.Application.UnitTests.Period.GetPeriodsQuery.GetPeriodsQueryHandler
{
    public class WhenHandling
    {
        private GetPeriodsQueryRequest _request;
        private PeriodEntity _periodEntity1;
        private PeriodEntity _periodEntity2;
        private Mock<IPeriodRepository> _periodRepository;
        private IMapper _mapper;
        private Application.Period.GetPeriodsQuery.GetPeriodsQueryHandler _handler;

        [SetUp]
        public void Arrange()
        {
            _request = new GetPeriodsQueryRequest();

            _periodEntity1 = new PeriodEntity
            {
                Period = 12,
                AcademicYear = 1671,
                ReferenceDataValidationDate = new DateTime(2017, 8, 31),
                CompletionDate = new DateTime(2017, 9, 5, 19, 27, 34)
            };
            _periodEntity2 = new PeriodEntity
            {
                //Id = "1718-R01",
                Period = 1,
                AcademicYear = 1718,
                ReferenceDataValidationDate = new DateTime(2017, 10, 2),
                CompletionDate = new DateTime(2017, 10, 4, 23, 34, 10)
            };

            _periodRepository = new Mock<IPeriodRepository>();
            _periodRepository.Setup(r => r.GetPeriods())
                .Returns(Task.FromResult(new[] { _periodEntity1, _periodEntity2 }));

            _mapper = new AutoMapperMapper(AutoMapperConfigurationFactory.CreateMappingConfig());

            _handler = new Application.Period.GetPeriodsQuery.GetPeriodsQueryHandler(_periodRepository.Object, _mapper);
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
            Assert.AreEqual(period.Id, $"{entity.AcademicYear}-R{entity.Period:D2}");
            Assert.AreEqual(period.CalendarMonth, PeriodExtensions.GetMonthFromPaymentEntity(entity.Period));
            Assert.AreEqual(period.CalendarYear, PeriodExtensions.GetYearFromPaymentEntity(entity.AcademicYear, entity.Period));
            Assert.AreEqual(period.AccountDataValidAt, entity.ReferenceDataValidationDate);
            Assert.AreEqual(period.CommitmentDataValidAt, entity.ReferenceDataValidationDate);
            Assert.AreEqual(period.CompletionDateTime, entity.CompletionDate);
        }
    }
}
