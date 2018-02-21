using System;
using MediatR;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.DataLock.GetProvidersQuery;

namespace SFA.DAS.Provider.Events.DataLockEventWorker.UnitTests.Tests
{

    public class DataLockProcessorTestBase
    {
        protected IDataLockProcessor _dataLockProcessor;
        protected readonly Mock<ILog> _loggerMock = new Mock<ILog>();
        protected readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>(MockBehavior.Strict);
        protected GetProvidersQueryResponse _getProvidersQueryResponse;

        [SetUp]
        protected virtual void SetUp()
        {
            _dataLockProcessor = new DataLockProcessor(_loggerMock.Object, _mediatorMock.Object);
            _getProvidersQueryResponse = new GetProvidersQueryResponse
            {
                IsValid = true,
                Result = new[] {new ProviderEntity {Ukprn = 1, IlrSubmissionDateTime = DateTime.Today}}
            };
        }

        protected T DeepClone<T>(T obj)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
        }
    }
}
