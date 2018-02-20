using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.DataLock.GetLatestDataLocksQuery;
using SFA.DAS.Provider.Events.Application.Mapping;
using SFA.DAS.Provider.Events.Application.Repositories;
using SFA.DAS.Provider.Events.Application.Validation.Rules;

namespace SFA.DAS.Provider.Events.Application.UnitTests.DataLock.GetLatestDataLocksQuery
{
    public class WhenHandling
    {
        private Mock<IDataLockEventRepository> _dataLockEventRepository;
        private GetLatestDataLocksQueryHandler _handler;
        private GetLatestDataLocksQueryRequest _request;
        private Mock<IMapper> _mapper;

        [SetUp]
        public void Arrange()
        {
            _mapper = new Mock<IMapper>();
            _mapper
                .Setup(m => m.Map<Api.Types.DataLock[]>(It.IsAny<DataLockEntity[]>()))
                .Returns((DataLockEntity[] list) =>
                {
                    return list.Select(source =>
                    {
                        var entity = new Api.Types.DataLock
                        {
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
                            PriceEpisodeIdentifier = source.PriceEpisodeIdentifier,
                            Ukprn = source.Ukprn,
                            Uln = source.Uln,
                            ErrorCodes = string.IsNullOrEmpty(source.ErrorCodes) ? null : JsonConvert.DeserializeObject<IList<string>>(source.ErrorCodes),
                            AimSequenceNumber = source.AimSequenceNumber,
                            LearnerReferenceNumber = source.LearnerReferenceNumber
                        };
                        return entity;
                    }).ToArray();
                });           

            _dataLockEventRepository = new Mock<IDataLockEventRepository>();

            _handler = new GetLatestDataLocksQueryHandler(new GetLatestDataLocksQueryRequestValidator(new PageNumberMustBeAtLeastOneRule()), _dataLockEventRepository.Object, _mapper.Object);

            _request = new GetLatestDataLocksQueryRequest { PageNumber = 1 };
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

            _dataLockEventRepository.Setup(r => r.GetLastDataLocks(1,1,3)).Throws(new ApplicationException("test ex")).Verifiable();

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            _dataLockEventRepository.VerifyAll();

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

            var dataLockPage = new PageOfResults<DataLockEntity>
            {
                PageNumber = 1,
                TotalNumberOfPages = 1,
                Items = new DataLockEntity[0]
            };

            _dataLockEventRepository.Setup(r => r.GetLastDataLocks(1,1,3)).ReturnsAsync(dataLockPage).Verifiable();

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            _dataLockEventRepository.VerifyAll();

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
                ErrorCodes = "[\"E1\", \"E2\"]"
            };

            var dataLocks = new[] { dataLock };

            var dataLockPage = new PageOfResults<DataLockEntity>
            {
                PageNumber = 1,
                TotalNumberOfPages = 1,
                Items = dataLocks
            };

            _dataLockEventRepository.Setup(r => r.GetLastDataLocks(1, 1, 3)).ReturnsAsync(dataLockPage).Verifiable();

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            _dataLockEventRepository.VerifyAll();

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
        public async Task AndThereAreNoEpisodesThenItShouldReturnDataLocksWithErrorsAndEpisodes()
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
                ErrorCodes = "[\"E1\", \"E2\"]"
            };

            var dataLocks = new[]
            {
                dataLock
            };

            var dataLockPage = new PageOfResults<DataLockEntity>
            {
                PageNumber = 1,
                TotalNumberOfPages = 1,
                Items = dataLocks
            };

            _dataLockEventRepository.Setup(r => r.GetLastDataLocks(1,1,3)).ReturnsAsync(dataLockPage).Verifiable();

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            _dataLockEventRepository.VerifyAll();

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
                CommitmentId = 777
            };

            var dataLock2 = new DataLockEntity
            {
                Ukprn = 2,
                AimSequenceNumber = 3,
                LearnerReferenceNumber = "L3",
                PriceEpisodeIdentifier = "P3",
                CommitmentId = 747
            };

            var dataLocks = new[] {dataLock1, dataLock2};

            var dataLockPage = new PageOfResults<DataLockEntity>
            {
                PageNumber = 1,
                TotalNumberOfPages = 1,
                Items = dataLocks
            };


            _dataLockEventRepository.Setup(r => r.GetLastDataLocks(1,1,3)).ReturnsAsync(dataLockPage).Verifiable();

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            _dataLockEventRepository.VerifyAll();

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Result);
            Assert.AreEqual(2, actual.Result.Items.Length);
            Assert.AreEqual(1, actual.Result.Items[0].Ukprn);
            Assert.IsNull(actual.Result.Items[0].ErrorCodes);
            Assert.IsNull(actual.Result.Items[0].CommitmentVersions);
        }
    }
}