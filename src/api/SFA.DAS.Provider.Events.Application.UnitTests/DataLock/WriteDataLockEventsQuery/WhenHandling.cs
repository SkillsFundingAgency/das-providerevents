
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.DataLock.WriteDataLockEventsQuery;
using SFA.DAS.Provider.Events.Application.Mapping;
using SFA.DAS.Provider.Events.Application.Repositories;

namespace SFA.DAS.Provider.Events.Application.UnitTests.DataLock.WriteDataLockEventsQuery
{
    public class WhenHandling
    {
        private Mock<IDataLockEventRepository> _dataLockEventRepository;
        private WriteDataLockEventsQueryHandler _handler;
        private WriteDataLockEventsQueryRequest _request;
        private Mock<IMapper> _mapper;

        [SetUp]
        public void Arrange()
        {

            _mapper = new Mock<IMapper>();
            _mapper
                .Setup(m => m.Map<IList<DataLockEventEntity>>(It.IsAny<IList<DataLockEvent>>()))
                .Returns((IList<DataLockEvent> source) =>
                {
                    return new List<DataLockEventEntity>(source.Select(e =>
                    {
                        var entity = new DataLockEventEntity();
                        if (e.Errors != null)
                            entity.ErrorCodes = JsonConvert.SerializeObject(e.Errors.Select(er => er.ErrorCode).ToArray());
                        if (e.Apprenticeships != null)
                            entity.CommitmentVersions = JsonConvert.SerializeObject(e.Apprenticeships);
                        return entity;
                    }));
                });           
            
            _dataLockEventRepository = new Mock<IDataLockEventRepository>();

            _handler = new WriteDataLockEventsQueryHandler(_dataLockEventRepository.Object, _mapper.Object);

            _request = new WriteDataLockEventsQueryRequest();
        }

        [Test]
        public async Task ThenItShouldReturnErrorWithInternalException()
        {
            // Arrange
            var dataLockEvent = new DataLockEvent();
            var events = new List<DataLockEvent> { dataLockEvent };
            _request.DataLockEvents = events;
            _dataLockEventRepository.Setup(r => r.WriteDataLockEvents(It.IsAny<IList<DataLockEventEntity>>())).Throws(new ApplicationException("test ex")).Verifiable();

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            _dataLockEventRepository.VerifyAll();

            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid);
            Assert.AreEqual("test ex", actual.Exception.Message);
        }

        [Test]
        public async Task ThenItShouldReturnValidResponse()
        {
            // Arrange
            IList<DataLockEventEntity> actualEntities = null;

            var dataLockEvent = new DataLockEvent { Ukprn = 1, Uln = 2 };
            var events = new List<DataLockEvent> { dataLockEvent };
            _request.DataLockEvents = events;
            _dataLockEventRepository.Setup(r => r.WriteDataLockEvents(It.IsAny<IList<DataLockEventEntity>>()))
                .Returns(Task.FromResult(default(object)))
                .Callback<IList<DataLockEventEntity>>(e => { actualEntities = e; });

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            _dataLockEventRepository.VerifyAll();

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);
            Assert.IsNotNull(actualEntities);
            Assert.AreEqual(1, actualEntities.Count);
            Assert.IsNull(actualEntities[0].ErrorCodes);
            Assert.IsNull(actualEntities[0].CommitmentVersions);
        }
        [Test]
        public async Task AndThereAreErrorsThenItShouldReturnValidResponse()
        {
            // Arrange
            IList<DataLockEventEntity> actualEntities = null;

            var dataLockEventErrors = new []
            {
                new DataLockEventError { ErrorCode = "E1", SystemDescription = "Blah"},
                new DataLockEventError { ErrorCode = "E2", SystemDescription = "Blah"},
                new DataLockEventError { ErrorCode = "E3", SystemDescription = "Blah"},
                new DataLockEventError { ErrorCode = "E4", SystemDescription = "Blah"}
            };

            var dataLockEventApprenticeships = new []
            {
                new DataLockEventApprenticeship { EffectiveDate = DateTime.Today,  Version = "1" },
                new DataLockEventApprenticeship { EffectiveDate = DateTime.Today,  Version = "2" }
            };

            var dataLockEvent = new DataLockEvent {Ukprn = 1, Uln = 2, Errors = dataLockEventErrors, Apprenticeships = dataLockEventApprenticeships};

            var events = new List<DataLockEvent> { dataLockEvent };
            _request.DataLockEvents = events;
            _dataLockEventRepository.Setup(r => r.WriteDataLockEvents(It.IsAny<IList<DataLockEventEntity>>()))
                .Returns(Task.FromResult(default(object)))
                .Callback<IList<DataLockEventEntity>>(e => { actualEntities = e; });

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            _dataLockEventRepository.VerifyAll();

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);
            Assert.IsNotNull(actualEntities);
            Assert.AreEqual(1, actualEntities.Count);
            Assert.AreEqual(4, actualEntities[0].ErrorCodes.Length);
            Assert.AreEqual(2, actualEntities[0].CommitmentVersions.Length);
        }

        [Test]
        public async Task ThenItShouldNotFailWhenPassedEmptyList()
        {
            // Arrange
            _request.DataLockEvents = new DataLockEvent[0];

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);
        }

        [Test]
        public async Task ThenItShouldNotFailWhenPassedNull()
        {
            // Arrange
            _request.DataLockEvents = null;

            // Act
            var actual = await _handler.Handle(_request);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);
        }       
    }
}