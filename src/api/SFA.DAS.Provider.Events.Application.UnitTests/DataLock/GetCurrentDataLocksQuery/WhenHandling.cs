using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.DataLock.GetCurrentDataLocksQuery;
using SFA.DAS.Provider.Events.Application.Mapping;
using SFA.DAS.Provider.Events.Application.Repositories;
using SFA.DAS.Provider.Events.Application.Validation;
using SFA.DAS.Provider.Events.Application.Validation.Rules;

namespace SFA.DAS.Provider.Events.Application.UnitTests.DataLock.GetCurrentDataLocksQuery
{
    public class WhenHandling
    {
        private Mock<IDataLockRepository> _dataLockRepository;
        private GetCurrentDataLocksQueryHandler _handler;
        private GetCurrentDataLocksQueryRequest _request;
        private Mock<IMapper> _mapper;

        [SetUp]
        public void Arrange()
        {
            _dataLockRepository = new Mock<IDataLockRepository>();
            _mapper = new Mock<IMapper>();
            _mapper
                .Setup(m => m.Map<Api.Types.DataLock[]>(It.IsAny<IList<DataLockEntity>>()))
                .Returns((IList<DataLockEntity> list) =>
                {
                    return list.Select(source => new Api.Types.DataLock
                    {
                        AimSequenceNumber = source.AimSequenceNumber,
                        CommitmentId = source.CommitmentId,
                        EmployerAccountId = source.EmployerAccountId,
                        IlrEndpointAssessorPrice = source.IlrEndpointAssessorPrice,
                        IlrFrameworkCode = source.IlrFrameworkCode,
                        IlrPathwayCode = source.IlrPathwayCode,
                        IlrPriceEffectiveFromDate = source.IlrPriceEffectiveFromDate,
                        IlrPriceEffectiveToDate = source.IlrPriceEffectiveToDate,
                        IlrProgrammeType = source.IlrProgrammeType,
                        IlrStandardCode = source.IlrStandardCode,
                        IlrStartDate = source.IlrStartDate,
                        IlrTrainingPrice = source.IlrTrainingPrice,
                        LearnerReferenceNumber = source.LearnerReferenceNumber,
                        PriceEpisodeIdentifier = source.PriceEpisodeIdentifier,
                        Ukprn = source.Ukprn,
                        Uln = source.Uln,
                        ErrorCodes = string.IsNullOrEmpty(source.ErrorCodes) ? null : JsonConvert.DeserializeObject<List<string>>(source.ErrorCodes)
                    }).ToArray();
                });

            _handler = new GetCurrentDataLocksQueryHandler(new GetCurrentDataLocksQueryRequestValidator(new PageNumberMustBeAtLeastOneRule()), _dataLockRepository.Object, _mapper.Object);

            _request = new GetCurrentDataLocksQueryRequest { PageNumber = 1 };
        }

        [Test]
        public async Task ThenItShouldReturnErrorWithValidatorFails()
        {
            // Arrange
            _request.PageNumber = 0;
            _request.PageSize = 3;
            _request.Ukprn = 1;

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
        }

        [Test]
        public async Task ThenItShouldReturnErrorWithInternalException()
        {
            // Arrange
            _request.PageNumber = 1;
            _request.PageSize = 3;
            _request.Ukprn = 1;

            _dataLockRepository.Setup(r => r.GetDataLocks(1,1,3)).Throws(new ApplicationException("test ex")).Verifiable();

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            _dataLockRepository.VerifyAll();

            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.AreEqual("test ex", actual.Exception.Message);
        }

        [Test]
        public async Task ThenItShouldReturnValidResponseWithValidatorDoesNotFail()
        {
            // Arrange
            _request.PageNumber = 1;
            _request.PageSize = 3;
            _request.Ukprn = 1;

            var dataLockPage = new DataLockEntity[0];

            _dataLockRepository.Setup(r => r.GetDataLocks(1,1,3)).ReturnsAsync(dataLockPage).Verifiable();

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);
        }

        [Test]
        public async Task ThenItShouldReturnDataLocksWithErrorsAndEpisodes()
        {
            // Arrange
            _request.PageNumber = 1;
            _request.PageSize = 3;
            _request.Ukprn = 1;

            var dataLock = new DataLockEntity
            {
                Ukprn = 1,
                AimSequenceNumber = 2,
                LearnerReferenceNumber = "L1",
                PriceEpisodeIdentifier = "P1",
                CommitmentId = 1,
                ErrorCodes = "['E1','E2']"
            };

            var dataLocks = new[] { dataLock };

            _dataLockRepository.Setup(r => r.GetDataLocks(1,1,3)).ReturnsAsync(dataLocks).Verifiable();

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            _dataLockRepository.VerifyAll();

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Result);
            Assert.AreEqual(1, actual.Result.Items.Length);
            Assert.AreEqual(1, actual.Result.Items[0].Ukprn);
            Assert.AreEqual(2, actual.Result.Items[0].ErrorCodes.Count);
            Assert.AreEqual("E1", actual.Result.Items[0].ErrorCodes[0]);
            Assert.AreEqual("E2", actual.Result.Items[0].ErrorCodes[1]);
            Assert.IsNull(actual.Result.Items[0].CommitmentVersions);
        }

        [Test]
        public async Task AndThereAreNoEpisodesThenItShouldReturnDataLocksWithErrorsAndNoEpisodes()
        {
            // Arrange
            _request.PageNumber = 1;
            _request.PageSize = 3;
            _request.Ukprn = 1;

            var dataLock = new DataLockEntity
            {
                Ukprn = 1,
                AimSequenceNumber = 2,
                LearnerReferenceNumber = "L1",
                PriceEpisodeIdentifier = "P1",
                CommitmentId = 1,
                ErrorCodes = "['E1','E2']"
            };

            var dataLocks = new[]
            {
                dataLock
            };

            _dataLockRepository.Setup(r => r.GetDataLocks(1,1,3)).ReturnsAsync(dataLocks).Verifiable();

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            _dataLockRepository.VerifyAll();

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Result);
            Assert.AreEqual(1, actual.Result.Items.Length);
            Assert.AreEqual(1, actual.Result.Items[0].Ukprn);
            Assert.AreEqual(2, actual.Result.Items[0].ErrorCodes.Count);
            Assert.AreEqual("E1", actual.Result.Items[0].ErrorCodes[0]);
            Assert.AreEqual("E2", actual.Result.Items[0].ErrorCodes[1]);
        }

        [Test]
        public async Task AndThereAreNoEpisodesOrErrorsThenItShouldReturnDataLocksWithErrorsAndEpisodes()
        {
            // Arrange
            _request.PageNumber = 1;
            _request.PageSize = 3;
            _request.Ukprn = 1;

            var dataLock1 = new DataLockEntity
            {
                Ukprn = 1,
                AimSequenceNumber = 2,
                LearnerReferenceNumber = "L1",
                PriceEpisodeIdentifier = "P1",
                CommitmentId = 1,
                ErrorCodes = null
            };

            var dataLock2 = new DataLockEntity
            {
                Ukprn = 2,
                AimSequenceNumber = 3,
                LearnerReferenceNumber = "L3",
                PriceEpisodeIdentifier = "P3",
                CommitmentId = 2,
                ErrorCodes = null
            };

            var dataLocks = new[] {dataLock1, dataLock2};

            _dataLockRepository.Setup(r => r.GetDataLocks(1,1,3)).ReturnsAsync(dataLocks).Verifiable();

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            _dataLockRepository.VerifyAll();

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Result);
            Assert.AreEqual(2, actual.Result.Items.Length);
            Assert.AreEqual(1, actual.Result.Items[0].Ukprn);
            Assert.IsNull(actual.Result.Items[0].ErrorCodes);
        }
    }
}