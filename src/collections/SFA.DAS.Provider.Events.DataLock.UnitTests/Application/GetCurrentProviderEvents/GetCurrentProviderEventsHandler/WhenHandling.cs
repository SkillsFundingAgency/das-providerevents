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
        private static readonly object[] EventConsolidationCases =
        {
            new object[] { new[] { MakeDataLockEntity(), MakeDataLockEntity(commitmentVersionId: "1-002") }, 1 },
            new object[] { new[] { MakeDataLockEntity(), MakeDataLockEntity(learnRefNumber: "Lrn-002") }, 2 },
            new object[] { new[] { MakeDataLockEntity(), MakeDataLockEntity(priceEpisodeIdentifier: "25-22-0/05/2017") }, 2 },
        };

        private static readonly object[] PeriodCases =
        {
            new object[] {new[] {MakeDataLockEntity(), MakeDataLockEntity(period: 2)}, 2},
            new object[] {new[] {MakeDataLockEntity(), MakeDataLockEntity(period: 2), MakeDataLockEntity(period: 3) }, 3},
            new object[] {new[] {MakeDataLockEntity(), MakeDataLockEntity(period: 2), MakeDataLockEntity(period: 2) }, 2},
            new object[] {new[] {MakeDataLockEntity(), MakeDataLockEntity(period: 2), MakeDataLockEntity(period: 2, transactionType: 2) }, 3},
        };
        private static readonly object[] ErrorCases =
        {
            new object[] {new[] {MakeDataLockEntity(), MakeDataLockEntity(ruleId: "DLOCK_08")}, 2},
            new object[] {new[] {MakeDataLockEntity(), MakeDataLockEntity(ruleId: "DLOCK_08"), MakeDataLockEntity(ruleId: "DLOCK_08") }, 2},
        };
        private static readonly object[] CommitmentVersionCases =
        {
            new object[] {new[] {MakeDataLockEntity(), MakeDataLockEntity(commitmentVersionId: "1-002")}, 2},
            new object[] {new[] {MakeDataLockEntity(), MakeDataLockEntity(commitmentVersionId: "1-002"), MakeDataLockEntity(commitmentVersionId:"1-002") }, 2},
        };

        private Mock<IDataLockEventDataRepository> _dataLockEventDataRepository;

        private DataLock.Application.GetCurrentProviderEvents.GetCurrentProviderEventsHandler _handler;

        private string _academicYear = "1617";
        private EventSource _eventsSource = EventSource.Submission;

        [SetUp]
        public void Arrange()
        {
            //_priceEpisodeMatch = new PriceEpisodeMatchEntity
            //{
            //    Ukprn = 10000534,
            //    PriceEpisodeIdentifier = "25-27-01/05/2017",
            //    LearnRefnumber = "Lrn-001",
            //    AimSeqNumber = 1,
            //    CommitmentId = 99,
            //    IsSuccess = false
            //};

            //_priceEpisodePeriodMatch = new PriceEpisodePeriodMatchEntity
            //{
            //    Ukprn = 10000534,
            //    PriceEpisodeIdentifier = "25-27-01/05/2017",
            //    LearnRefnumber = "Lrn-001",
            //    AimSeqNumber = 1,
            //    CommitmentId = 99,
            //    VersionId = 1,
            //    Period = 9,
            //    Payable = false,
            //    TransactionType = 1
            //};

            //_validationError = new ValidationErrorEntity
            //{
            //    Ukprn = 10000534,
            //    PriceEpisodeIdentifier = "25-27-01/05/2017",
            //    LearnRefnumber = "Lrn-001",
            //    AimSeqNumber = 1,
            //    RuleId = "DLOCK_07"
            //};

            //_ilrPriceEpisode = new IlrPriceEpisodeEntity
            //{
            //    IlrFileName = "ILR-1617-10000534.xml",
            //    SubmittedTime = new DateTime(2017, 2, 14, 9, 15,23),
            //    Ukprn = 10000534,
            //    Uln = 1000000019,
            //    PriceEpisodeIdentifier = "25-27-01/05/2017",
            //    LearnRefnumber = "Lrn-001",
            //    AimSeqNumber = 1,
            //    IlrStartDate = new DateTime(2017, 5, 1),
            //    IlrStandardCode = 27,
            //    IlrTrainingPrice = 12000,
            //    IlrEndpointAssessorPrice = 3000,
            //    IlrPriceEffectiveDate = DateTime.Today
            //};

            //_commitment = new CommitmentEntity
            //{
            //    CommitmentId = 99,
            //    CommitmentVersion = 1,
            //    EmployerAccountId = 10,
            //    StartDate = new DateTime(2017, 5, 1),
            //    StandardCode = 27,
            //    NegotiatedPrice = 17500,
            //    EffectiveDate = new DateTime(2017, 5, 1)
            //};

            _dataLockEventDataRepository = new Mock<IDataLockEventDataRepository>();

            _handler = new DataLock.Application.GetCurrentProviderEvents.GetCurrentProviderEventsHandler(_dataLockEventDataRepository.Object,
                _academicYear,
                _eventsSource);
        }

        [Test]
        [TestCaseSource(nameof(EventConsolidationCases))]
        public void ThenItShouldReturnAnEventPerChangeInLearnerRefOrPriceEpisode(DataLockEventDataEntity[] entities, int expectedNumberOfEvents)
        {
            // Arrange
            _dataLockEventDataRepository.Setup(r => r.GetCurrentEvents(It.IsAny<long>()))
                .Returns(entities);

            // Act
            var response = _handler.Handle(new GetCurrentProviderEventsRequest { Ukprn = 10000534 });

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsValid);
            Assert.IsNotNull(response.Items);
            Assert.AreEqual(expectedNumberOfEvents, response.Items.Length);
        }

        [Test]
        public void ThenItShouldMapEntityToDomainModelCorrectly()
        {
            // Arrange
            var entities = new[]
            {
                MakeDataLockEntity(),
                MakeDataLockEntity(ruleId: "DLOCK_08"),
                MakeDataLockEntity(period: 2, commitmentVersionId: "1-002", payable: false, commitmentStandardCode: 22, commitmentNegotiatedPrice: 16000, commitmentEffectiveDate: new DateTime(2017, 5, 1))
            };
            _dataLockEventDataRepository.Setup(r => r.GetCurrentEvents(10000534))
                .Returns(entities);

            // Act
            var response = _handler.Handle(new GetCurrentProviderEventsRequest { Ukprn = 10000534 });

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsValid);
            Assert.IsNotNull(response.Items);
            Assert.AreEqual(1, response.Items.Length);

            // Main event
            var actualEvent = response.Items[0];
            Assert.AreEqual(entities[0].IlrFilename, actualEvent.IlrFileName);
            Assert.AreEqual(entities[0].SubmittedTime, actualEvent.SubmittedDateTime);
            Assert.AreEqual(_academicYear, actualEvent.AcademicYear);
            Assert.AreEqual(entities[0].Ukprn, actualEvent.Ukprn);
            Assert.AreEqual(entities[0].Uln, actualEvent.Uln);
            Assert.AreEqual(entities[0].LearnRefNumber, actualEvent.LearnRefnumber);
            Assert.AreEqual(entities[0].AimSeqNumber, actualEvent.AimSeqNumber);
            Assert.AreEqual(entities[0].PriceEpisodeIdentifier, actualEvent.PriceEpisodeIdentifier);
            Assert.AreEqual(entities[0].CommitmentId, actualEvent.CommitmentId);
            Assert.AreEqual(entities[0].EmployerAccountId, actualEvent.EmployerAccountId);
            Assert.AreEqual(_eventsSource, actualEvent.EventSource);
            Assert.AreEqual(!entities[0].IsSuccess, actualEvent.HasErrors);
            Assert.AreEqual(entities[0].IlrStartDate, actualEvent.IlrStartDate);
            Assert.AreEqual(entities[0].IlrStandardCode, actualEvent.IlrStandardCode);
            Assert.AreEqual(entities[0].IlrProgrammeType, actualEvent.IlrProgrammeType);
            Assert.AreEqual(entities[0].IlrFrameworkCode, actualEvent.IlrFrameworkCode);
            Assert.AreEqual(entities[0].IlrPathwayCode, actualEvent.IlrPathwayCode);
            Assert.AreEqual(entities[0].IlrTrainingPrice, actualEvent.IlrTrainingPrice);
            Assert.AreEqual(entities[0].IlrEndpointAssessorPrice, actualEvent.IlrEndpointAssessorPrice);
            Assert.AreEqual(entities[0].IlrPriceEffectiveFromDate, actualEvent.IlrPriceEffectiveFromDate);

            // Errors
            Assert.IsNotNull(actualEvent.Errors);
            Assert.AreEqual(2, actualEvent.Errors.Length);

            Assert.AreEqual("DLOCK_07", actualEvent.Errors[0].ErrorCode);
            Assert.AreEqual("No matching record found in the employer digital account for the negotiated cost of training", actualEvent.Errors[0].SystemDescription);

            Assert.AreEqual("DLOCK_08", actualEvent.Errors[1].ErrorCode);
            Assert.AreEqual("Multiple matching records found in the employer digital account", actualEvent.Errors[1].SystemDescription);

            // Periods
            Assert.IsNotNull(actualEvent.Periods);
            Assert.AreEqual(2, actualEvent.Periods.Length);

            Assert.AreEqual(8, actualEvent.Periods[0].CollectionPeriod.Month);
            Assert.AreEqual(2016, actualEvent.Periods[0].CollectionPeriod.Year);
            Assert.AreEqual("1617-R01", actualEvent.Periods[0].CollectionPeriod.Name);
            Assert.AreEqual("1-001", actualEvent.Periods[0].CommitmentVersion);
            Assert.AreEqual(true, actualEvent.Periods[0].IsPayable);
            Assert.AreEqual(TransactionType.Learning, actualEvent.Periods[0].TransactionType);

            Assert.AreEqual(9, actualEvent.Periods[1].CollectionPeriod.Month);
            Assert.AreEqual(2016, actualEvent.Periods[1].CollectionPeriod.Year);
            Assert.AreEqual("1617-R02", actualEvent.Periods[1].CollectionPeriod.Name);
            Assert.AreEqual("1-002", actualEvent.Periods[1].CommitmentVersion);
            Assert.AreEqual(false, actualEvent.Periods[1].IsPayable);
            Assert.AreEqual(TransactionType.Learning, actualEvent.Periods[1].TransactionType);

            // Commitment versions
            Assert.IsNotNull(actualEvent.CommitmentVersions);
            Assert.AreEqual(2, actualEvent.CommitmentVersions.Length);

            Assert.AreEqual(entities[0].CommitmentVersionId, actualEvent.CommitmentVersions[0].CommitmentVersion);
            Assert.AreEqual(entities[0].CommitmentStandardCode, actualEvent.CommitmentVersions[0].CommitmentStandardCode);
            Assert.AreEqual(entities[0].CommitmentProgrammeType, actualEvent.CommitmentVersions[0].CommitmentProgrammeType);
            Assert.AreEqual(entities[0].CommitmentFrameworkCode, actualEvent.CommitmentVersions[0].CommitmentFrameworkCode);
            Assert.AreEqual(entities[0].CommitmentPathwayCode, actualEvent.CommitmentVersions[0].CommitmentPathwayCode);
            Assert.AreEqual(entities[0].CommitmentStartDate, actualEvent.CommitmentVersions[0].CommitmentStartDate);
            Assert.AreEqual(entities[0].CommitmentNegotiatedPrice, actualEvent.CommitmentVersions[0].CommitmentNegotiatedPrice);
            Assert.AreEqual(entities[0].CommitmentEffectiveDate, actualEvent.CommitmentVersions[0].CommitmentEffectiveDate);

            Assert.AreEqual(entities[2].CommitmentVersionId, actualEvent.CommitmentVersions[1].CommitmentVersion);
            Assert.AreEqual(entities[2].CommitmentStandardCode, actualEvent.CommitmentVersions[1].CommitmentStandardCode);
            Assert.AreEqual(entities[2].CommitmentProgrammeType, actualEvent.CommitmentVersions[1].CommitmentProgrammeType);
            Assert.AreEqual(entities[2].CommitmentFrameworkCode, actualEvent.CommitmentVersions[1].CommitmentFrameworkCode);
            Assert.AreEqual(entities[2].CommitmentPathwayCode, actualEvent.CommitmentVersions[1].CommitmentPathwayCode);
            Assert.AreEqual(entities[2].CommitmentStartDate, actualEvent.CommitmentVersions[1].CommitmentStartDate);
            Assert.AreEqual(entities[2].CommitmentNegotiatedPrice, actualEvent.CommitmentVersions[1].CommitmentNegotiatedPrice);
            Assert.AreEqual(entities[2].CommitmentEffectiveDate, actualEvent.CommitmentVersions[1].CommitmentEffectiveDate);
        }

        [Test]
        public void ThenItShouldReturnValidResponseWithEmptyItemArrayIfNoEventsInRepository()
        {
            // Act
            var response = _handler.Handle(new GetCurrentProviderEventsRequest { Ukprn = 10000534 });

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsValid);
            Assert.IsNotNull(response.Items);
            Assert.AreEqual(0, response.Items.Length);
        }

        [Test]
        public void ThenItShouldReturnInvalidResponseWithExceptionIfRepositoryThrowsException()
        {
            // Arrange
            _dataLockEventDataRepository.Setup(r => r.GetCurrentEvents(It.IsAny<long>()))
                .Throws(new FormatException("XYZ"));

            // Act
            var response = _handler.Handle(new GetCurrentProviderEventsRequest { Ukprn = 10000534 });

            // Assert
            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsValid);
            Assert.IsNotNull(response.Exception);
            Assert.IsInstanceOf<FormatException>(response.Exception);
            Assert.AreEqual("XYZ", response.Exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(PeriodCases))]
        public void ThenItShouldReturnADistinctListOfPeriodsForAnEvent(DataLockEventDataEntity[] entities, int expectedNumberOfPeriods)
        {
            // Arrange
            _dataLockEventDataRepository.Setup(r => r.GetCurrentEvents(It.IsAny<long>()))
                .Returns(entities);

            // Act
            var response = _handler.Handle(new GetCurrentProviderEventsRequest { Ukprn = 10000534 });

            // Assert
            var actual = response.Items[0];
            Assert.IsNotNull(actual.Periods);
            Assert.AreEqual(expectedNumberOfPeriods, actual.Periods.Length);
        }

        [Test]
        [TestCaseSource(nameof(ErrorCases))]
        public void ThenItShouldReturnADistinctListOfErrorCodesForAnEvent(DataLockEventDataEntity[] entities, int expectedNumberOfErrors)
        {
            // Arrange
            _dataLockEventDataRepository.Setup(r => r.GetCurrentEvents(It.IsAny<long>()))
                .Returns(entities);

            // Act
            var response = _handler.Handle(new GetCurrentProviderEventsRequest { Ukprn = 10000534 });

            // Assert
            var actual = response.Items[0];
            Assert.IsNotNull(actual.Errors);
            Assert.AreEqual(expectedNumberOfErrors, actual.Errors.Length);
        }

        [Test]
        [TestCaseSource(nameof(CommitmentVersionCases))]
        public void ThenItShouldReturnADistinctListOfCommitmentVersionsForAnEvent(DataLockEventDataEntity[] entities, int expectedNumberOfVersions)
        {
            // Arrange
            _dataLockEventDataRepository.Setup(r => r.GetCurrentEvents(It.IsAny<long>()))
                .Returns(entities);

            // Act
            var response = _handler.Handle(new GetCurrentProviderEventsRequest { Ukprn = 10000534 });

            // Assert
            var actual = response.Items[0];
            Assert.IsNotNull(actual.CommitmentVersions);
            Assert.AreEqual(expectedNumberOfVersions, actual.CommitmentVersions.Length);
        }


        private static DataLockEventDataEntity MakeDataLockEntity(long ukprn = 10000534, string priceEpisodeIdentifier = "25-21-0/05/2017", string learnRefNumber = "Lrn-001",
            int aimSeqNumber = 1, long commitmentId = 1, bool isSuccess = true, string ilrFileName = "something", DateTime submittedTime = default(DateTime),
            long uln = 1532456, DateTime ilrStartDate = default(DateTime), long? ilrStandardCode = 21, int? ilrProgrammeType = null, int? ilrFrameworkCode = null,
            int? ilrPathwayCode = null, decimal ilrTrainingPrice = 12000, decimal ilrEndpointAssessorPrice = 3000, DateTime ilrPriceEffectiveDate = default(DateTime),
            string commitmentVersionId = "1-001", int period = 1, bool payable = true, int transactionType = 1, long employerAccountId = 1, DateTime commitmentStartDate = default(DateTime),
            long? commitmentStandardCode = 21, int? commitmentProgrammeType = null, int? commitmentFrameworkCode = null, int? commitmentPathwayCode = null,
            decimal commitmentNegotiatedPrice = 16000, DateTime commitmentEffectiveDate = default(DateTime), string ruleId = "DLOCK_07")
        {
            return new DataLockEventDataEntity
            {
                Ukprn = ukprn,
                PriceEpisodeIdentifier = priceEpisodeIdentifier,
                LearnRefNumber = learnRefNumber,
                AimSeqNumber = aimSeqNumber,
                CommitmentId = commitmentId,
                IsSuccess = isSuccess,
                IlrFilename = ilrFileName,
                SubmittedTime = submittedTime,
                Uln = uln,
                IlrStartDate = ilrStartDate,
                IlrStandardCode = ilrStandardCode,
                IlrProgrammeType = ilrProgrammeType,
                IlrFrameworkCode = ilrFrameworkCode,
                IlrPathwayCode = ilrPathwayCode,
                IlrTrainingPrice = ilrTrainingPrice,
                IlrEndpointAssessorPrice = ilrEndpointAssessorPrice,
                IlrPriceEffectiveFromDate = ilrPriceEffectiveDate,
                CommitmentVersionId = commitmentVersionId,
                Period = period,
                Payable = payable,
                TransactionType = transactionType,
                EmployerAccountId = employerAccountId,
                CommitmentStartDate = commitmentStartDate,
                CommitmentStandardCode = commitmentStandardCode,
                CommitmentProgrammeType = commitmentProgrammeType,
                CommitmentFrameworkCode = commitmentFrameworkCode,
                CommitmentPathwayCode = commitmentPathwayCode,
                CommitmentNegotiatedPrice = commitmentNegotiatedPrice,
                CommitmentEffectiveDate = commitmentEffectiveDate,
                RuleId = ruleId
            };
        }
        
    }
}