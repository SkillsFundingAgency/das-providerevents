using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.DCFS.Domain;
using SFA.DAS.Provider.Events.DataLock.Application.GetLastSeenProviderEvents;
using SFA.DAS.Provider.Events.DataLock.Domain;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.UnitTests.Application.GetLastSeenProviderEvents.GetLastSeenProviderEventsHandler
{
    public class WhenHandling
    {
        private static readonly object[] EmptyDataLockEvents =
           {
            new object[] { null },
            new object[] { new DataLockEventEntity[] {} }
        };

        private static readonly object[] EmptyDataLockEventPeriods =
        {
            new object[] { null },
            new object[] { new DataLockEventPeriodEntity[] {} }
        };

        private static readonly object[] EmptyDataLockEventCommitmentVersions =
        {
            new object[] { null },
            new object[] { new DataLockEventCommitmentVersionEntity[] {} }
        };

        private static readonly object[] EmptyDataLockEventErrors =
        {
            new object[] { null },
            new object[] { new DataLockEventErrorEntity[] {} }
        };

        private Mock<IDataLockEventRepository> _dataLockEventRepository;
        private Mock<IDataLockEventPeriodRepository> _dataLockEventPeriodRepository;
        private Mock<IDataLockEventCommitmentVersionRepository> _dataLockEventCommitmentVersionRepository;
        private Mock<IDataLockEventErrorRepository> _dataLockEventErrorRepository;

        private DataLock.Application.GetLastSeenProviderEvents.GetLastSeenProviderEventsHandler _handler;

        private DataLockEventEntity _event;
        private DataLockEventPeriodEntity _eventPeriod;
        private DataLockEventCommitmentVersionEntity _eventCommitmentVersion;
        private DataLockEventErrorEntity _eventError;
        private Guid EventId;


        [SetUp]
        public void Arrange()
        {
            EventId = Guid.NewGuid();
            _event = new DataLockEventEntity
            {
                DataLockEventId = EventId,
                ProcessDateTime = DateTime.Now,
                IlrFileName = "ILR-1617-10000534.xml",
                SubmittedDateTime = new DateTime(2017, 2, 14, 9, 15, 23),
                AcademicYear = "1617",
                Ukprn = 10000534,
                Uln = 1000000019,
                LearnRefnumber = "Lrn-001",
                AimSeqNumber = 1,
                PriceEpisodeIdentifier = "20-550-6-01/05/2017",
                CommitmentId = 99,
                EmployerAccountId = 10,
                EventSource = 1,
                HasErrors = true,
                IlrStartDate = new DateTime(2017, 5, 1),
                IlrProgrammeType = 20,
                IlrFrameworkCode = 550,
                IlrPathwayCode = 6,
                IlrTrainingPrice = 12000,
                IlrEndpointAssessorPrice = 3000,
                IlrPriceEffectiveDate = DateTime.Today
            };

            _eventPeriod = new DataLockEventPeriodEntity
            {
                DataLockEventId = EventId,
                CollectionPeriodName = "1617-R09",
                CollectionPeriodMonth = 4,
                CollectionPeriodYear = 2017,
                CommitmentVersion = 15,
                IsPayable = false,
                TransactionType = 1
            };

            _eventCommitmentVersion = new DataLockEventCommitmentVersionEntity
            {
                DataLockEventId = EventId,
                CommitmentVersion = 1,
                CommitmentStartDate = new DateTime(2017, 5, 1),
                CommitmentProgrammeType = 20,
                CommitmentFrameworkCode = 550,
                CommitmentPathwayCode = 6,
                CommitmentNegotiatedPrice = 17500,
                CommitmentEffectiveDate = new DateTime(2017, 5, 1)
            };

            _eventError = new DataLockEventErrorEntity
            {
                DataLockEventId = EventId,
                ErrorCode = "DLOCK_07",
                SystemDescription = "DLOCK_07"
            };

            _dataLockEventRepository = new Mock<IDataLockEventRepository>();
            _dataLockEventPeriodRepository = new Mock<IDataLockEventPeriodRepository>();
            _dataLockEventCommitmentVersionRepository = new Mock<IDataLockEventCommitmentVersionRepository>();
            _dataLockEventErrorRepository = new Mock<IDataLockEventErrorRepository>();

            _dataLockEventRepository.Setup(r => r.GetProviderLastSeenEvents(It.IsAny<long>()))
                .Returns(new[]
                {
                    _event
                });

            _dataLockEventPeriodRepository.Setup(r => r.GetDataLockEventPeriods(EventId))
                .Returns(new[]
                {
                    _eventPeriod
                });

            _dataLockEventCommitmentVersionRepository.Setup(r => r.GetDataLockEventCommitmentVersions(EventId))
                .Returns(new[]
                {
                    _eventCommitmentVersion
                });

            _dataLockEventErrorRepository.Setup(r => r.GetDatalockEventErrors(EventId))
                .Returns(new[]
                {
                    _eventError
                });

            _handler = new DataLock.Application.GetLastSeenProviderEvents.GetLastSeenProviderEventsHandler(_dataLockEventRepository.Object,
                _dataLockEventPeriodRepository.Object,
                _dataLockEventCommitmentVersionRepository.Object,
                _dataLockEventErrorRepository.Object);
        }

        [Test]
        public void ThenItShouldReturnLastSeenDataLockEventsFromRepository()
        {
            // Act
            var response = _handler.Handle(new GetLastSeenProviderEventsRequest());

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsValid);
            Assert.IsNotNull(response.Items);
            Assert.AreEqual(1, response.Items.Length);
            Assert.IsTrue(EventMatches(response.Items[0]));
        }

        [Test]
        [TestCaseSource(nameof(EmptyDataLockEvents))]
        public void ThenItShouldReturnEmptyArrayIfNoResultFromRepository(DataLockEventEntity[] entities)
        {
            // Arrange
            _dataLockEventRepository.Setup(r => r.GetProviderLastSeenEvents(It.IsAny<long>()))
                .Returns(entities);

            // Act
            var actual = _handler.Handle(new GetLastSeenProviderEventsRequest());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);
            Assert.IsNotNull(actual.Items);
            Assert.AreEqual(0, actual.Items.Length);
        }

        [Test]
        [TestCaseSource(nameof(EmptyDataLockEventPeriods))]
        public void ThenItShouldReturnEventWithNoPeriodsIfEmptyArrayReturnedFromPeriodsRepository(DataLockEventPeriodEntity[] entities)
        {
            // Arrange
            _dataLockEventPeriodRepository.Setup(r => r.GetDataLockEventPeriods(EventId))
                .Returns(entities);

            // Act
            var actual = _handler.Handle(new GetLastSeenProviderEventsRequest());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);
            Assert.IsNotNull(actual.Items);
            Assert.AreEqual(1, actual.Items.Length);
            Assert.AreEqual(0, actual.Items[0].Periods.Length);
        }

        [Test]
        [TestCaseSource(nameof(EmptyDataLockEventCommitmentVersions))]
        public void ThenItShouldReturnEventWithNoCommitmentVersionsIfEmptyArrayReturnedFromCommitmentsRepository(DataLockEventCommitmentVersionEntity[] entities)
        {
            // Arrange
            _dataLockEventCommitmentVersionRepository.Setup(r => r.GetDataLockEventCommitmentVersions(EventId))
                .Returns(entities);

            // Act
            var actual = _handler.Handle(new GetLastSeenProviderEventsRequest());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);
            Assert.IsNotNull(actual.Items);
            Assert.AreEqual(1, actual.Items.Length);
            Assert.AreEqual(0, actual.Items[0].CommitmentVersions.Length);
        }

        [Test]
        [TestCaseSource(nameof(EmptyDataLockEventErrors))]
        public void ThenItShouldReturnEventWithNoErrorsIfEmptyArrayReturnedFromErrorsRepository(DataLockEventErrorEntity[] entities)
        {
            // Arrange
            _dataLockEventErrorRepository.Setup(r => r.GetDatalockEventErrors(EventId))
                .Returns(entities);

            // Act
            var actual = _handler.Handle(new GetLastSeenProviderEventsRequest());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);
            Assert.IsNotNull(actual.Items);
            Assert.AreEqual(1, actual.Items.Length);
            Assert.AreEqual(0, actual.Items[0].Errors.Length);
        }

        [Test]
        public void ThenItShouldReturnInvalidResponseIfEventRepositoryErrors()
        {
            // Arrange
            _dataLockEventRepository.Setup(r => r.GetProviderLastSeenEvents(It.IsAny<long>()))
                .Throws(new Exception("Test"));

            // Act
            var actual = _handler.Handle(new GetLastSeenProviderEventsRequest());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.IsNotNull(actual.Exception);
            Assert.AreEqual("Test", actual.Exception.Message);
        }

        [Test]
        public void ThenItShouldReturnInvalidResponseIfEventPeriodsRepositoryErrors()
        {
            // Arrange
            _dataLockEventPeriodRepository.Setup(r => r.GetDataLockEventPeriods(EventId))
                .Throws(new Exception("Test"));

            // Act
            var actual = _handler.Handle(new GetLastSeenProviderEventsRequest());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.IsNotNull(actual.Exception);
            Assert.AreEqual("Test", actual.Exception.Message);
        }

        [Test]
        public void ThenItShouldReturnInvalidResponseIfEventCommitmentVersionsRepositoryErrors()
        {
            // Arrange
            _dataLockEventCommitmentVersionRepository.Setup(r => r.GetDataLockEventCommitmentVersions(EventId))
                .Throws(new Exception("Test"));

            // Act
            var actual = _handler.Handle(new GetLastSeenProviderEventsRequest());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.IsNotNull(actual.Exception);
            Assert.AreEqual("Test", actual.Exception.Message);
        }

        [Test]
        public void ThenItShouldReturnInvalidResponseIfEventErrorsRepositoryErrors()
        {
            // Arrange
            _dataLockEventErrorRepository.Setup(r => r.GetDatalockEventErrors(EventId))
                .Throws(new Exception("Test"));

            // Act
            var actual = _handler.Handle(new GetLastSeenProviderEventsRequest());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.IsNotNull(actual.Exception);
            Assert.AreEqual("Test", actual.Exception.Message);
        }

        private bool EventMatches(DataLockEvent @event)
        {
            return 
                    @event.ProcessDateTime == _event.ProcessDateTime
                   && @event.IlrFileName == _event.IlrFileName
                   && @event.SubmittedDateTime == _event.SubmittedDateTime
                   && @event.AcademicYear == _event.AcademicYear
                   && @event.Ukprn == _event.Ukprn
                   && @event.Uln == _event.Uln
                   && @event.LearnRefnumber == _event.LearnRefnumber
                   && @event.AimSeqNumber == _event.AimSeqNumber
                   && @event.PriceEpisodeIdentifier == _event.PriceEpisodeIdentifier
                   && @event.CommitmentId == _event.CommitmentId
                   && @event.EmployerAccountId == _event.EmployerAccountId
                   && @event.EventSource == (EventSource) _event.EventSource
                   && @event.HasErrors == _event.HasErrors
                   && @event.IlrStartDate == _event.IlrStartDate
                   && @event.IlrStandardCode == _event.IlrStandardCode
                   && @event.IlrProgrammeType == _event.IlrProgrammeType
                   && @event.IlrFrameworkCode == _event.IlrFrameworkCode
                   && @event.IlrPathwayCode == _event.IlrPathwayCode
                   && @event.IlrTrainingPrice == _event.IlrTrainingPrice
                   && @event.IlrEndpointAssessorPrice == _event.IlrEndpointAssessorPrice
                   && @event.IlrPriceEffectiveDate == _event.IlrPriceEffectiveDate
                   && ErrorMatches(@event.Errors[0])
                   && PeriodMatches(@event.Periods[0])
                   && CommitmentVersionMatches(@event.CommitmentVersions[0]);
        }

        private bool ErrorMatches(DataLockEventError error)
        {
            return error.DataLockEventId == _eventError.DataLockEventId
                   && error.ErrorCode == _eventError.ErrorCode
                   && error.SystemDescription == _eventError.SystemDescription;
        }

        private bool PeriodMatches(DataLockEventPeriod period)
        {
            return period.DataLockEventId == _eventPeriod.DataLockEventId
                   && period.CollectionPeriod.Name == _eventPeriod.CollectionPeriodName
                   && period.CollectionPeriod.Month == _eventPeriod.CollectionPeriodMonth
                   && period.CollectionPeriod.Year == _eventPeriod.CollectionPeriodYear
                   && period.CommitmentVersion == _eventPeriod.CommitmentVersion
                   && period.IsPayable == _eventPeriod.IsPayable
                   && period.TransactionType == (TransactionType) _eventPeriod.TransactionType;
        }

        private bool CommitmentVersionMatches(DataLockEventCommitmentVersion version)
        {
            return version.DataLockEventId == _eventCommitmentVersion.DataLockEventId
                   && version.CommitmentVersion == _eventCommitmentVersion.CommitmentVersion
                   && version.CommitmentStartDate == _eventCommitmentVersion.CommitmentStartDate
                   && version.CommitmentStandardCode == _eventCommitmentVersion.CommitmentStandardCode
                   && version.CommitmentProgrammeType == _eventCommitmentVersion.CommitmentProgrammeType
                   && version.CommitmentFrameworkCode == _eventCommitmentVersion.CommitmentFrameworkCode
                   && version.CommitmentPathwayCode == _eventCommitmentVersion.CommitmentPathwayCode
                   && version.CommitmentNegotiatedPrice == _eventCommitmentVersion.CommitmentNegotiatedPrice
                   && version.CommitmentEffectiveDate == _eventCommitmentVersion.CommitmentEffectiveDate;
        }
    }
}