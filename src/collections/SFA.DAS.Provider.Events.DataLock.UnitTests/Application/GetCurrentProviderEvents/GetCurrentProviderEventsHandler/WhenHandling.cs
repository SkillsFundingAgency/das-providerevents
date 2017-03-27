using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.DCFS.Domain;
using SFA.DAS.Provider.Events.DataLock.Application.GetCurrentProviderEvents;
using SFA.DAS.Provider.Events.DataLock.Domain;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.UnitTests.Application.GetCurrentProviderEvents.GetCurrentProviderEventsHandler
{
    public class WhenHandling
    {
        private static readonly object[] EmptyPriceEpisodeMatches =
        {
            new object[] { null },
            new object[] { new PriceEpisodeMatchEntity[] {} }
        };

        private static readonly object[] EmptyPriceEpisodePeriodMatches =
        {
            new object[] { null },
            new object[] { new PriceEpisodePeriodMatchEntity[] {} }
        };

        private static readonly object[] EmptyValidationErrors =
        {
            new object[] { null },
            new object[] { new ValidationErrorEntity[] {} }
        };

        private static readonly object[] EmptyCommitmentVersions =
        {
            new object[] { null },
            new object[] { new CommitmentEntity[] {} }
        };

        private Mock<IPriceEpisodeMatchRepository> _priceEpisodeMatchRepository;
        private Mock<IPriceEpisodePeriodMatchRepository> _priceEpisodePeriodMatchRepository;
        private Mock<IValidationErrorRepository> _validationErrorRepository;
        private Mock<IIlrPriceEpisodeRepository> _ilrPriceEpisodeRepository;
        private Mock<ICommitmentRepository> _commitmentRepository;

        private DataLock.Application.GetCurrentProviderEvents.GetCurrentProviderEventsHandler _handler;

        private PriceEpisodeMatchEntity _priceEpisodeMatch;
        private PriceEpisodePeriodMatchEntity _priceEpisodePeriodMatch;
        private ValidationErrorEntity _validationError;
        private IlrPriceEpisodeEntity _ilrPriceEpisode;
        private CommitmentEntity _commitment;

        private string _academicYear = "1617";
        private EventSource _eventsSource = EventSource.Submission;

        private readonly string _expectedErrorDescription = "No matching record found in the employer digital account for the negotiated cost of training";

        [SetUp]
        public void Arrange()
        {
            _priceEpisodeMatch = new PriceEpisodeMatchEntity
            {
                Ukprn = 10000534,
                PriceEpisodeIdentifier = "25-27-01/05/2017",
                LearnRefnumber = "Lrn-001",
                AimSeqNumber = 1,
                CommitmentId = 99,
                IsSuccess = false
            };

            _priceEpisodePeriodMatch = new PriceEpisodePeriodMatchEntity
            {
                Ukprn = 10000534,
                PriceEpisodeIdentifier = "25-27-01/05/2017",
                LearnRefnumber = "Lrn-001",
                AimSeqNumber = 1,
                CommitmentId = 99,
                VersionId = 1,
                Period = 9,
                Payable = false,
                TransactionType = 1
            };

            _validationError = new ValidationErrorEntity
            {
                Ukprn = 10000534,
                PriceEpisodeIdentifier = "25-27-01/05/2017",
                LearnRefnumber = "Lrn-001",
                AimSeqNumber = 1,
                RuleId = "DLOCK_07"
            };

            _ilrPriceEpisode = new IlrPriceEpisodeEntity
            {
                IlrFileName = "ILR-1617-10000534.xml",
                SubmittedTime = new DateTime(2017, 2, 14, 9, 15,23),
                Ukprn = 10000534,
                Uln = 1000000019,
                PriceEpisodeIdentifier = "25-27-01/05/2017",
                LearnRefnumber = "Lrn-001",
                AimSeqNumber = 1,
                IlrStartDate = new DateTime(2017, 5, 1),
                IlrStandardCode = 27,
                IlrTrainingPrice = 12000,
                IlrEndpointAssessorPrice = 3000,
                IlrPriceEffectiveDate = DateTime.Today
            };

            _commitment = new CommitmentEntity
            {
                CommitmentId = 99,
                CommitmentVersion = 1,
                EmployerAccountId = 10,
                StartDate = new DateTime(2017, 5, 1),
                StandardCode = 27,
                NegotiatedPrice = 17500,
                EffectiveDate = new DateTime(2017, 5, 1)
            };

            _priceEpisodeMatchRepository = new Mock<IPriceEpisodeMatchRepository>();
            _priceEpisodePeriodMatchRepository = new Mock<IPriceEpisodePeriodMatchRepository>();
            _validationErrorRepository = new Mock<IValidationErrorRepository>();
            _ilrPriceEpisodeRepository = new Mock<IIlrPriceEpisodeRepository>();
            _commitmentRepository = new Mock<ICommitmentRepository>();

            _priceEpisodeMatchRepository.Setup(r => r.GetProviderPriceEpisodeMatches(It.IsAny<long>()))
                .Returns(new[]
                {
                    _priceEpisodeMatch
                });

            _priceEpisodePeriodMatchRepository.Setup(r => r.GetPriceEpisodePeriodMatches(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new[]
                {
                    _priceEpisodePeriodMatch
                });

            _validationErrorRepository.Setup(r => r.GetPriceEpisodeValidationErrors(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new[]
                {
                    _validationError
                });

            _ilrPriceEpisodeRepository.Setup(r => r.GetPriceEpisodeIlrData(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(_ilrPriceEpisode);

            _commitmentRepository.Setup(r => r.GetCommitmentVersions(It.IsAny<long>()))
                .Returns(new[]
                {
                    _commitment
                });

            _handler = new DataLock.Application.GetCurrentProviderEvents.GetCurrentProviderEventsHandler(_priceEpisodeMatchRepository.Object,
                _priceEpisodePeriodMatchRepository.Object,
                _validationErrorRepository.Object,
                _ilrPriceEpisodeRepository.Object,
                _commitmentRepository.Object,
                _academicYear,
                _eventsSource);
        }

        [Test]
        public void ThenItShouldReturnCurrentDataLockEventsFromRepository()
        {
            // Act
            var response = _handler.Handle(new GetCurrentProviderEventsRequest());

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsValid);
            Assert.IsNotNull(response.Items);
            Assert.AreEqual(1, response.Items.Length);
            Assert.IsTrue(EventMatches(response.Items[0]));
        }

        [Test]
        [TestCaseSource(nameof(EmptyPriceEpisodeMatches))]
        public void ThenItShouldReturnEmptyArrayIfNoResultFromRepository(PriceEpisodeMatchEntity[] entities)
        {
            // Arrange
            _priceEpisodeMatchRepository.Setup(r => r.GetProviderPriceEpisodeMatches(It.IsAny<long>()))
                .Returns(entities);

            // Act
            var actual = _handler.Handle(new GetCurrentProviderEventsRequest());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);
            Assert.IsNotNull(actual.Items);
            Assert.AreEqual(0, actual.Items.Length);
        }

        [Test]
        [TestCaseSource(nameof(EmptyPriceEpisodePeriodMatches))]
        public void ThenItShouldReturnEventWithNoPeriodsIfEmptyArrayReturnedFromPeriodsRepository(PriceEpisodePeriodMatchEntity[] entities)
        {
            // Arrange
            _priceEpisodePeriodMatchRepository.Setup(r => r.GetPriceEpisodePeriodMatches(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(entities);

            // Act
            var actual = _handler.Handle(new GetCurrentProviderEventsRequest());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);
            Assert.IsNotNull(actual.Items);
            Assert.AreEqual(1, actual.Items.Length);
            Assert.AreEqual(0, actual.Items[0].Periods.Length);
        }

        [Test]
        [TestCaseSource(nameof(EmptyValidationErrors))]
        public void ThenItShouldReturnEventWithNoErrorsIfEmptyArrayReturnedFromErrorsRepository(ValidationErrorEntity[] entities)
        {
            // Arrange
            _validationErrorRepository.Setup(r => r.GetPriceEpisodeValidationErrors(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(entities);

            // Act
            var actual = _handler.Handle(new GetCurrentProviderEventsRequest());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);
            Assert.IsNotNull(actual.Items);
            Assert.AreEqual(1, actual.Items.Length);
            Assert.AreEqual(0, actual.Items[0].Errors.Length);
        }

        [Test]
        public void ThenItShouldReturnInvalidResponseIfPriceEpisodeMatchRepositoryErrors()
        {
            // Arrange
            _priceEpisodeMatchRepository.Setup(r => r.GetProviderPriceEpisodeMatches(It.IsAny<long>()))
                .Throws(new Exception("Test"));

            // Act
            var actual = _handler.Handle(new GetCurrentProviderEventsRequest());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.IsNotNull(actual.Exception);
            Assert.AreEqual("Test", actual.Exception.Message);
        }

        [Test]
        public void ThenItShouldReturnInvalidResponseIfPriceEpisodePeriodMatchRepositoryErrors()
        {
            // Arrange
            _priceEpisodePeriodMatchRepository.Setup(r => r.GetPriceEpisodePeriodMatches(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("Test"));

            // Act
            var actual = _handler.Handle(new GetCurrentProviderEventsRequest());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.IsNotNull(actual.Exception);
            Assert.AreEqual("Test", actual.Exception.Message);
        }

        [Test]
        public void ThenItShouldReturnInvalidResponseIfValidationErrorRepositoryErrors()
        {
            // Arrange
            _validationErrorRepository.Setup(r => r.GetPriceEpisodeValidationErrors(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("Test"));

            // Act
            var actual = _handler.Handle(new GetCurrentProviderEventsRequest());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.IsNotNull(actual.Exception);
            Assert.AreEqual("Test", actual.Exception.Message);
        }

        [Test]
        public void ThenItShouldReturnInvalidResponseIfIlrPriceEpisodeRepositoryErrors()
        {
            // Arrange
            _ilrPriceEpisodeRepository.Setup(r => r.GetPriceEpisodeIlrData(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("Test"));

            // Act
            var actual = _handler.Handle(new GetCurrentProviderEventsRequest());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.IsNotNull(actual.Exception);
            Assert.AreEqual("Test", actual.Exception.Message);
        }

        [Test]
        public void ThenItShouldReturnInvalidResponseIfCommitmentRepositoryErrors()
        {
            // Arrange
            _commitmentRepository.Setup(r => r.GetCommitmentVersions(It.IsAny<long>()))
                .Throws(new Exception("Test"));

            // Act
            var actual = _handler.Handle(new GetCurrentProviderEventsRequest());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.IsNotNull(actual.Exception);
            Assert.AreEqual("Test", actual.Exception.Message);
        }

        [Test]
        public void ThenItShouldReturnInvalidResponseIfNoIlrDataFound()
        {
            // Arrange
            _ilrPriceEpisodeRepository.Setup(r => r.GetPriceEpisodeIlrData(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns((IlrPriceEpisodeEntity)null);

            // Act
            var actual = _handler.Handle(new GetCurrentProviderEventsRequest());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.IsNotNull(actual.Exception);
        }

        [Test]
        [TestCaseSource(nameof(EmptyCommitmentVersions))]
        public void ThenItShouldReturnInvalidResponseIfNoCommitmentVersionsFound(CommitmentEntity[] entities)
        {
            // Arrange
            _commitmentRepository.Setup(r => r.GetCommitmentVersions(It.IsAny<long>()))
                .Returns(entities);

            // Act
            var actual = _handler.Handle(new GetCurrentProviderEventsRequest());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.IsNotNull(actual.Exception);
        }

        [Test]
        [TestCase(1, "1617-R01", 8, 2016)]
        [TestCase(2, "1617-R02", 9, 2016)]
        [TestCase(3, "1617-R03", 10, 2016)]
        [TestCase(4, "1617-R04", 11, 2016)]
        [TestCase(5, "1617-R05", 12, 2016)]
        [TestCase(6, "1617-R06", 1, 2017)]
        [TestCase(7, "1617-R07", 2, 2017)]
        [TestCase(8, "1617-R08", 3, 2017)]
        [TestCase(9, "1617-R09", 4, 2017)]
        [TestCase(10, "1617-R10", 5, 2017)]
        [TestCase(11, "1617-R11", 6, 2017)]
        [TestCase(12, "1617-R12", 7, 2017)]
        public void ThenItShouldReturnTheCorrectPeriodInformation(int period, string name, int month, int year)
        {
            // Arrange
            _priceEpisodePeriodMatch = new PriceEpisodePeriodMatchEntity
            {
                Ukprn = 10000534,
                PriceEpisodeIdentifier = "25-27-01/05/2017",
                LearnRefnumber = "Lrn-001",
                AimSeqNumber = 1,
                CommitmentId = 99,
                VersionId = 1,
                Period = period,
                Payable = false,
                TransactionType = 1
            };

            _priceEpisodePeriodMatchRepository.Setup(r => r.GetPriceEpisodePeriodMatches(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
               .Returns(new[]
               {
                    _priceEpisodePeriodMatch
               });

            // Act
            var actual = _handler.Handle(new GetCurrentProviderEventsRequest());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);

            var actualPeriod = actual.Items[0].Periods[0].CollectionPeriod;

            Assert.AreEqual(name, actualPeriod.Name);
            Assert.AreEqual(month, actualPeriod.Month);
            Assert.AreEqual(year, actualPeriod.Year);
        }

        private bool EventMatches(DataLockEvent @event)
        {
            return @event.IlrFileName == _ilrPriceEpisode.IlrFileName
                   && @event.SubmittedDateTime == _ilrPriceEpisode.SubmittedTime
                   && @event.AcademicYear == _academicYear
                   && @event.Ukprn == _priceEpisodeMatch.Ukprn
                   && @event.Uln == _ilrPriceEpisode.Uln
                   && @event.LearnRefnumber == _priceEpisodeMatch.LearnRefnumber
                   && @event.AimSeqNumber == _priceEpisodeMatch.AimSeqNumber
                   && @event.PriceEpisodeIdentifier == _priceEpisodeMatch.PriceEpisodeIdentifier
                   && @event.CommitmentId == _priceEpisodeMatch.CommitmentId
                   && @event.EmployerAccountId == _commitment.EmployerAccountId
                   && @event.EventSource == _eventsSource
                   && @event.HasErrors == !_priceEpisodeMatch.IsSuccess
                   && @event.IlrStartDate == _ilrPriceEpisode.IlrStartDate
                   && @event.IlrStandardCode == _ilrPriceEpisode.IlrStandardCode
                   && @event.IlrProgrammeType == _ilrPriceEpisode.IlrProgrammeType
                   && @event.IlrFrameworkCode == _ilrPriceEpisode.IlrFrameworkCode
                   && @event.IlrPathwayCode == _ilrPriceEpisode.IlrPathwayCode
                   && @event.IlrTrainingPrice == _ilrPriceEpisode.IlrTrainingPrice
                   && @event.IlrEndpointAssessorPrice == _ilrPriceEpisode.IlrEndpointAssessorPrice
                   && @event.IlrPriceEffectiveDate == _ilrPriceEpisode.IlrPriceEffectiveDate
                   && ErrorMatches(@event.Errors[0])
                   && PeriodMatches(@event.Periods[0])
                   && CommitmentVersionMatches(@event.CommitmentVersions[0]);
        }

        private bool ErrorMatches(DataLockEventError error)
        {
            return error.ErrorCode == _validationError.RuleId
                   && error.SystemDescription == _expectedErrorDescription;
        }

        private bool PeriodMatches(DataLockEventPeriod period)
        {
            return period.CollectionPeriod.Name == "1617-R09"
                   && period.CollectionPeriod.Month == 4
                   && period.CollectionPeriod.Year == 2017
                   && period.CommitmentVersion == _priceEpisodePeriodMatch.VersionId
                   && period.IsPayable == _priceEpisodePeriodMatch.Payable
                   && period.TransactionType == (TransactionType) _priceEpisodePeriodMatch.TransactionType;
        }

        private bool CommitmentVersionMatches(DataLockEventCommitmentVersion version)
        {
            return version.CommitmentVersion == _commitment.CommitmentVersion
                   && version.CommitmentStartDate == _commitment.StartDate
                   && version.CommitmentStandardCode == _commitment.StandardCode
                   && version.CommitmentProgrammeType == _commitment.ProgrammeType
                   && version.CommitmentFrameworkCode == _commitment.FrameworkCode
                   && version.CommitmentPathwayCode == _commitment.PathwayCode
                   && version.CommitmentNegotiatedPrice == _commitment.NegotiatedPrice
                   && version.CommitmentEffectiveDate == _commitment.EffectiveDate;
        }
    }
}