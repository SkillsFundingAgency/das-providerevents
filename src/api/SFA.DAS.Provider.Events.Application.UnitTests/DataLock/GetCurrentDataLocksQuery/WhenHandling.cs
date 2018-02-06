﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.DataLock.GetCurrentDataLocksQuery;
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

        [SetUp]
        public void Arrange()
        {
            _dataLockRepository = new Mock<IDataLockRepository>();

            _handler = new GetCurrentDataLocksQueryHandler(new GetCurrentDataLocksQueryRequestValidator(new PageNumberMustBeAtLeastOneRule()), _dataLockRepository.Object);

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

            var dataLockPage = new PageOfResults<Api.Types.DataLock>
            {
                PageNumber = 1,
                TotalNumberOfPages = 1,
                Items = new Api.Types.DataLock[0]
            };

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

            var dataLock = new Api.Types.DataLock
            {
                Ukprn = 1,
                AimSequenceNumber = 2,
                LearnerReferenceNumber = "L1",
                PriceEpisodeIdentifier = "P1",
                ErrorCodes = new List<string>() {"E1", "E2"},
                Commitments = new List<long>() {9, 8, 7}
            };

            var dataLocks = new[] { dataLock };

            var dataLockPage = new PageOfResults<Api.Types.DataLock>
            {
                PageNumber = 1,
                TotalNumberOfPages = 1,
                Items = dataLocks
            };

            _dataLockRepository.Setup(r => r.GetDataLocks(1,1,3)).ReturnsAsync(dataLockPage).Verifiable();

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
            Assert.AreEqual(3, actual.Result.Items[0].Commitments.Count);
            Assert.AreEqual(9L, actual.Result.Items[0].Commitments[0]);
            Assert.AreEqual(8L, actual.Result.Items[0].Commitments[1]);
            Assert.AreEqual(7L, actual.Result.Items[0].Commitments[2]);
        }

        [Test]
        public async Task AndThereAreNoEpisodesThenItShouldReturnDataLocksWithErrorsAndNoEpisodes()
        {
            // Arrange
            _request.PageNumber = 1;
            _request.PageSize = 3;
            _request.Ukprn = 1;

            var dataLock = new Api.Types.DataLock
            {
                Ukprn = 1,
                AimSequenceNumber = 2,
                LearnerReferenceNumber = "L1",
                PriceEpisodeIdentifier = "P1",
                ErrorCodes = new List<string>() {"E1", "E2"},
                Commitments = new long[0]
            };

            var dataLocks = new[]
            {
                dataLock
            };

            var dataLockPage = new PageOfResults<Api.Types.DataLock>
            {
                PageNumber = 1,
                TotalNumberOfPages = 1,
                Items = dataLocks
            };

            _dataLockRepository.Setup(r => r.GetDataLocks(1,1,3)).ReturnsAsync(dataLockPage).Verifiable();

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
            Assert.AreEqual(0, actual.Result.Items[0].Commitments.Count);
        }

        [Test]
        public async Task AndThereAreNoEpisodesOrErrorsThenItShouldReturnDataLocksWithErrorsAndEpisodes()
        {
            // Arrange
            _request.PageNumber = 1;
            _request.PageSize = 3;
            _request.Ukprn = 1;

            var dataLock1 = new Api.Types.DataLock
            {
                Ukprn = 1,
                AimSequenceNumber = 2,
                LearnerReferenceNumber = "L1",
                PriceEpisodeIdentifier = "P1",
                ErrorCodes = null,
                Commitments = null
            };

            var dataLock2 = new Api.Types.DataLock
            {
                Ukprn = 2,
                AimSequenceNumber = 3,
                LearnerReferenceNumber = "L3",
                PriceEpisodeIdentifier = "P3",
                ErrorCodes = null,
                Commitments = null
            };

            var dataLocks = new[] {dataLock1, dataLock2};

            var dataLockPage = new PageOfResults<Api.Types.DataLock>
            {
                PageNumber = 1,
                TotalNumberOfPages = 1,
                Items = dataLocks
            };

            _dataLockRepository.Setup(r => r.GetDataLocks(1,1,3)).ReturnsAsync(dataLockPage).Verifiable();

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            _dataLockRepository.VerifyAll();

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Result);
            Assert.AreEqual(2, actual.Result.Items.Length);
            Assert.AreEqual(1, actual.Result.Items[0].Ukprn);
            Assert.IsNull(actual.Result.Items[0].ErrorCodes);
            Assert.IsNull(actual.Result.Items[0].Commitments);
        }
    }
}