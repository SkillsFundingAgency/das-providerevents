using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Events.DataLock.Application.GetProviders;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.UnitTests.Application.GetProviders.GetProvidersQueryHandler
{
    public class WhenHandling
    {
        private static readonly ProviderEntity[] ProviderEntities =
        {
            new ProviderEntity {Ukprn = 10007459},
            new ProviderEntity {Ukprn = 10007460}
        };

        private static readonly object[] RepositoryResponses =
        {
            new object[] {ProviderEntities},
            new object[] {null}
        };

        private Mock<IProviderRepository> _repository;
        private GetProvidersQueryRequest _request;
        private DataLock.Application.GetProviders.GetProvidersQueryHandler _handler;

        [SetUp]
        public void Arrange()
        {
            _request = new GetProvidersQueryRequest();

            _repository = new Mock<IProviderRepository>();

            _handler = new DataLock.Application.GetProviders.GetProvidersQueryHandler(_repository.Object);
        }

        [Test]
        [TestCaseSource(nameof(RepositoryResponses))]
        public void ThenValidGetProvidersQueryResponseReturnedForValidRepositoryResponse(ProviderEntity[] providers)
        {
            // Arrange
            _repository = new Mock<IProviderRepository>();
            _repository.Setup(r => r.GetAllProviders())
                .Returns(providers);

            // Act
            var response = _handler.Handle(_request);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsValid);
        }

        [Test]
        public void ThenProvidersShouldBeInTheGetProvidersQueryResponse()
        {
            // Arrange
            _repository.Setup(r => r.GetAllProviders())
                .Returns(ProviderEntities);

            // Act
            var response = _handler.Handle(_request);

            // Assert
            Assert.IsNotNull(response?.Items);
            Assert.AreEqual(ProviderEntities[0].Ukprn, response.Items[0].Ukprn);
            Assert.AreEqual(ProviderEntities[1].Ukprn, response.Items[1].Ukprn);
        }

        [Test]
        public void ThenInvalidGetProvidersQueryResponseReturnedForInvalidRepositoryResponse()
        {
            // Arrange
            _repository.Setup(r => r.GetAllProviders())
                .Throws<Exception>();

            // Act
            var response = _handler.Handle(_request);

            // Assert
            Assert.IsFalse(response.IsValid);
            Assert.IsNull(response.Items);
            Assert.IsNotNull(response.Exception);
        }
    }
}