using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.DCFS.Domain;
using SFA.DAS.Provider.Events.DataLock.Application.WriteDataLockEvent;
using SFA.DAS.Provider.Events.DataLock.Domain;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.UnitTests.Application.WriteDataLockEvent.WriteDataLockEventCommandHandler
{
    public class WhenHandling
    {
        private Mock<IDataLockEventRepository> _dataLockEventRepository;
        private Mock<IDataLockEventPeriodRepository> _dataLockEventPeriodRepository;
        private Mock<IDataLockEventCommitmentVersionRepository> _dataLockEventCommitmentVersionRepository;
        private Mock<IDataLockEventErrorRepository> _dataLockEventErrorRepository;

        private DataLock.Application.WriteDataLockEvent.WriteDataLockEventCommandHandler _handler;

        private DataLockEvent _event;
        private DataLockEventPeriod _eventPeriod;
        private DataLockEventCommitmentVersion _eventCommitmentVersion;
        private DataLockEventError _eventError;

        [SetUp]
        public void Arrange()
        {
            _eventPeriod = new DataLockEventPeriod
            {
                CollectionPeriod = new CollectionPeriod
                {
                    Name = "1617-R09",
                    Month = 4,
                    Year = 2017
                },
                CommitmentVersion = 1,
                IsPayable = false,
                TransactionType = TransactionType.Learning
            };

            _eventCommitmentVersion = new DataLockEventCommitmentVersion
            {
                CommitmentVersion = 1,
                CommitmentStartDate = new DateTime(2017, 4, 1),
                CommitmentStandardCode = 27,
                CommitmentNegotiatedPrice = 17500,
                CommitmentEffectiveDate = new DateTime(2017, 4, 1)
            };

            _eventError = new DataLockEventError
            {
                ErrorCode = "DLOCK_07",
                SystemDescription = "DLOCK_07"
            };

            _event = new DataLockEvent
            {
                IlrFileName = "ILR-1617-10000534-75.xml",
                SubmittedDateTime = new DateTime(2017, 2, 14, 9, 15, 23),
                AcademicYear = "1617",
                Ukprn = 10000534,
                Uln = 1000000027,
                LearnRefnumber = "Lrn-002",
                AimSeqNumber = 1,
                PriceEpisodeIdentifier = "25-27-01/05/2017",
                CommitmentId = 75,
                EmployerAccountId = 10,
                EventSource = EventSource.Submission,
                HasErrors = true,
                IlrStartDate = new DateTime(2017, 4, 1),
                IlrStandardCode = 27,
                IlrTrainingPrice = 12000,
                IlrEndpointAssessorPrice = 3000,
                Periods = new[]
                {
                    _eventPeriod
                },
                Errors = new[]
                {
                    _eventError
                },
                CommitmentVersions = new[]
                {
                    _eventCommitmentVersion
                }
            };

            _dataLockEventRepository = new Mock<IDataLockEventRepository>();
            _dataLockEventPeriodRepository = new Mock<IDataLockEventPeriodRepository>();
            _dataLockEventCommitmentVersionRepository = new Mock<IDataLockEventCommitmentVersionRepository>();
            _dataLockEventErrorRepository = new Mock<IDataLockEventErrorRepository>();

            _dataLockEventRepository.Setup(r => r.WriteDataLockEvent(It.IsAny<DataLockEventEntity>()))
                .Returns(15);

            _handler = new DataLock.Application.WriteDataLockEvent.WriteDataLockEventCommandHandler(_dataLockEventRepository.Object,
                _dataLockEventPeriodRepository.Object,
                _dataLockEventCommitmentVersionRepository.Object,
                _dataLockEventErrorRepository.Object);
        }

        [Test]
        public void ThenItShouldStoreEventInRepository()
        {
            // Act
            _handler.Handle(new WriteDataLockEventCommandRequest { Event = _event });

            // Assert
            _dataLockEventRepository.Verify(r => r.WriteDataLockEvent(It.Is<DataLockEventEntity>(e => EventsMatch(e, _event))), Times.Once);
            _dataLockEventErrorRepository.Verify(r => r.WriteDataLockEventError(It.Is<DataLockEventErrorEntity>(e => ErrorsMatch(e, _eventError))));
            _dataLockEventPeriodRepository.Verify(r => r.WriteDataLockEventPeriod(It.Is<DataLockEventPeriodEntity>(e => PeriodsMatch(e, _eventPeriod))));
            _dataLockEventCommitmentVersionRepository.Verify(r => r.WriteDataLockEventCommitmentVersion(It.Is<DataLockEventCommitmentVersionEntity>(e => CommitmentVersionsMatch(e, _eventCommitmentVersion))));
        }

        [Test]
        public void ThenItShouldStoreEventWithNoErrorsInRepository()
        {
            // Arrange
            _event.Errors = null;

            // Act
            _handler.Handle(new WriteDataLockEventCommandRequest { Event = _event });

            // Assert
            _dataLockEventRepository.Verify(r => r.WriteDataLockEvent(It.Is<DataLockEventEntity>(e => EventsMatch(e, _event))), Times.Once);
            _dataLockEventErrorRepository.Verify(r => r.WriteDataLockEventError(It.IsAny<DataLockEventErrorEntity>()), Times.Never);
            _dataLockEventPeriodRepository.Verify(r => r.WriteDataLockEventPeriod(It.Is<DataLockEventPeriodEntity>(e => PeriodsMatch(e, _eventPeriod))));
            _dataLockEventCommitmentVersionRepository.Verify(r => r.WriteDataLockEventCommitmentVersion(It.Is<DataLockEventCommitmentVersionEntity>(e => CommitmentVersionsMatch(e, _eventCommitmentVersion))));
        }

        [Test]
        public void ThenItShouldStoreEventWithNoPeriodsInRepository()
        {
            // Arrange
            _event.Periods = null;

            // Act
            _handler.Handle(new WriteDataLockEventCommandRequest { Event = _event });

            // Assert
            _dataLockEventRepository.Verify(r => r.WriteDataLockEvent(It.Is<DataLockEventEntity>(e => EventsMatch(e, _event))), Times.Once);
            _dataLockEventErrorRepository.Verify(r => r.WriteDataLockEventError(It.Is<DataLockEventErrorEntity>(e => ErrorsMatch(e, _eventError))));
            _dataLockEventPeriodRepository.Verify(r => r.WriteDataLockEventPeriod(It.IsAny<DataLockEventPeriodEntity>()), Times.Never);
            _dataLockEventCommitmentVersionRepository.Verify(r => r.WriteDataLockEventCommitmentVersion(It.Is<DataLockEventCommitmentVersionEntity>(e => CommitmentVersionsMatch(e, _eventCommitmentVersion))));
        }

        [Test]
        public void ThenItShouldStoreEventWithNoCommitmentVersionsInRepository()
        {
            // Arrange
            _event.CommitmentVersions = null;

            // Act
            _handler.Handle(new WriteDataLockEventCommandRequest { Event = _event });

            // Assert
            _dataLockEventRepository.Verify(r => r.WriteDataLockEvent(It.Is<DataLockEventEntity>(e => EventsMatch(e, _event))), Times.Once);
            _dataLockEventErrorRepository.Verify(r => r.WriteDataLockEventError(It.Is<DataLockEventErrorEntity>(e => ErrorsMatch(e, _eventError))));
            _dataLockEventPeriodRepository.Verify(r => r.WriteDataLockEventPeriod(It.Is<DataLockEventPeriodEntity>(e => PeriodsMatch(e, _eventPeriod))));
            _dataLockEventCommitmentVersionRepository.Verify(r => r.WriteDataLockEventCommitmentVersion(It.IsAny<DataLockEventCommitmentVersionEntity>()), Times.Never);
        }

        private bool EventsMatch(DataLockEventEntity entity, DataLockEvent @event)
        {
            return entity.IlrFileName == @event.IlrFileName
                && entity.SubmittedDateTime == @event.SubmittedDateTime
                && entity.AcademicYear == @event.AcademicYear
                && entity.Ukprn == @event.Ukprn
                && entity.Uln == @event.Uln
                && entity.LearnRefnumber == @event.LearnRefnumber
                && entity.AimSeqNumber == @event.AimSeqNumber
                && entity.PriceEpisodeIdentifier == @event.PriceEpisodeIdentifier
                && entity.CommitmentId == @event.CommitmentId
                && entity.EmployerAccountId == @event.EmployerAccountId
                && entity.EventSource == (int) @event.EventSource
                && entity.HasErrors == @event.HasErrors
                && entity.IlrStartDate == @event.IlrStartDate
                && entity.IlrStandardCode == @event.IlrStandardCode
                && entity.IlrProgrammeType == @event.IlrProgrammeType
                && entity.IlrFrameworkCode == @event.IlrFrameworkCode
                && entity.IlrPathwayCode == @event.IlrPathwayCode
                && entity.IlrTrainingPrice == @event.IlrTrainingPrice
                && entity.IlrEndpointAssessorPrice == @event.IlrEndpointAssessorPrice;
        }

        private bool ErrorsMatch(DataLockEventErrorEntity entity, DataLockEventError error)
        {
            return entity.DataLockEventId == 15
                   && entity.ErrorCode == error.ErrorCode
                   && entity.SystemDescription == error.SystemDescription;
        }

        private bool PeriodsMatch(DataLockEventPeriodEntity entity, DataLockEventPeriod period)
        {
            return entity.DataLockEventId == 15
                   && entity.CollectionPeriodName == period.CollectionPeriod.Name
                   && entity.CollectionPeriodMonth == period.CollectionPeriod.Month
                   && entity.CollectionPeriodYear == period.CollectionPeriod.Year
                   && entity.CommitmentVersion == period.CommitmentVersion
                   && entity.IsPayable == period.IsPayable
                   && entity.TransactionType == (int) period.TransactionType;
        }

        private bool CommitmentVersionsMatch(DataLockEventCommitmentVersionEntity entity, DataLockEventCommitmentVersion version)
        {
            return entity.DataLockEventId == 15
                   && entity.CommitmentVersion == version.CommitmentVersion
                   && entity.CommitmentStartDate == version.CommitmentStartDate
                   && entity.CommitmentStandardCode == version.CommitmentStandardCode
                   && entity.CommitmentProgrammeType == version.CommitmentProgrammeType
                   && entity.CommitmentFrameworkCode == version.CommitmentFrameworkCode
                   && entity.CommitmentPathwayCode == version.CommitmentPathwayCode
                   && entity.CommitmentNegotiatedPrice == version.CommitmentNegotiatedPrice
                   && entity.CommitmentEffectiveDate == version.CommitmentEffectiveDate;
        }
    }
}