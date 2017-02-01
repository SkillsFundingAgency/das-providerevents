using System;
using System.Collections.Generic;
using CS.Common.External.Interfaces;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.DCFS.Context;
using SFA.DAS.Payments.DCFS.Infrastructure.DependencyResolution;

namespace SFA.DAS.Provider.Events.DataLock.UnitTests.DataLockEventsTaskTests
{
    public class WhenExecuting
    {
        private Mock<DataLockEventsProcessor> _processor;
        private Mock<IDependencyResolver> _dependencyResolver;
        private DataLockEventsTask _task;
        private Mock<IExternalContext> _context;

        [SetUp]
        public void Arrange()
        {
            _processor = new Mock<DataLockEventsProcessor>();

            _dependencyResolver = new Mock<IDependencyResolver>();
            _dependencyResolver.Setup(r => r.GetInstance<DataLockEventsProcessor>())
                .Returns(_processor.Object);

            _task = new DataLockEventsTask(_dependencyResolver.Object);

            _context = new Mock<IExternalContext>();
            _context.Setup(c => c.Properties)
                .Returns(new Dictionary<string, string>
                {
                    {ContextPropertyKeys.TransientDatabaseConnectionString, "some-connection-string"},
                    {ContextPropertyKeys.LogLevel, "Debug"}
                });
        }

        [Test]
        public void ThenItShouldInitaliseTheDependencyResolver()
        {
            // Act
            _task.Execute(_context.Object);

            // Assert
            _dependencyResolver.Verify(r => r.Init(It.Is<Type>(t => t == typeof(DataLockEventsProcessor)), It.IsAny<ContextWrapper>()), Times.Once);
        }

        [Test]
        public void ThenItShouldUseAResolvedProcessorToStartProcessing()
        {
            // Act
            _task.Execute(_context.Object);

            // Assert
            _dependencyResolver.Verify(r => r.GetInstance<DataLockEventsProcessor>(), Times.Once);
            _processor.Verify(p => p.Process(), Times.Once);
        }
    }
}
