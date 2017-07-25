using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Events.DataLock.Application.GetCurrentCollectionPeriod;
using SFA.DAS.Provider.Events.DataLock.Domain;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.UnitTests.Application.GetCurrentCollectionPeriod.GetCurrentCollectionPeriodEventsHandler
{
    public class WhenHandling
    {
        private const string ExpectedPeriodName = "Period Name";
        private const int ExpectedPeriodId = 1;
        private const int ExpectedPeriodYear = 2016;
        private GetCurrentCollectionPeriodHandler _handler;
        private Mock<ICollectionPeriodRepository> _repository;

        [SetUp]
        public void Arrange()
        {
            _repository = new Mock<ICollectionPeriodRepository>();
            _repository.Setup(x => x.GetCurrentCollectionPeriod()).Returns(new CollectionPeriodEntity { Month = ExpectedPeriodId, Name = ExpectedPeriodName, PeriodId = 2, Year = ExpectedPeriodYear });

            _handler = new GetCurrentCollectionPeriodHandler(_repository.Object);
        }

        [Test]
        public void ThenTheRepositoryIsCalled()
        {
            //Act
            _handler.Handle(new GetCurrentCollectionPeriodRequest());

            //Assert
            _repository.Verify(x => x.GetCurrentCollectionPeriod(), Times.Once);
        }

        [Test]
        public void ThenTheResponseIsPopulatedWithTheCurrentPeriod()
        {
            //Act
            var actual = _handler.Handle(new GetCurrentCollectionPeriodRequest());

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsAssignableFrom<GetCurrentCollectionPeriodResposne>(actual);
            Assert.IsAssignableFrom<CollectionPeriod>(actual.CollectionPeriod);
            Assert.AreEqual(ExpectedPeriodYear, actual.CollectionPeriod.Year);
            Assert.AreEqual(ExpectedPeriodId, actual.CollectionPeriod.Month);
            Assert.AreEqual(ExpectedPeriodName, actual.CollectionPeriod.Name);
        }

        [Test]
        public void ThenNullIsReturnedWhenTheRepositoryReturnsNull()
        {
            //Arrange
            _repository.Setup(x => x.GetCurrentCollectionPeriod()).Returns((CollectionPeriodEntity)null);

            //Act
            var actual = _handler.Handle(new GetCurrentCollectionPeriodRequest());

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsNull(actual.CollectionPeriod);
        }
    }
}
