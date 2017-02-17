using System.Collections.Generic;
using CS.Common.External.Interfaces;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.DCFS.Context;
using SFA.DAS.Payments.DCFS.Infrastructure.DependencyResolution;
using SFA.DAS.Provider.Events.DataLock.Infrastructure.Context;

namespace SFA.DAS.Provider.Events.DataLock.UnitTests.DataLockEventsTask
{
    public class WhenExecutedWithInvalidContext
    {
        private Mock<DataLock.DataLockEventsProcessor> _processor;
        private Mock<IDependencyResolver> _dependencyResolver;
        private DataLock.DataLockEventsTask _task;
        private Mock<IExternalContext> _context;

        [SetUp]
        public void Arrange()
        {
            _processor = new Mock<DataLock.DataLockEventsProcessor>();

            _dependencyResolver = new Mock<IDependencyResolver>();
            _dependencyResolver.Setup(r => r.GetInstance<DataLock.DataLockEventsProcessor>())
                .Returns(_processor.Object);

            _task = new DataLock.DataLockEventsTask(_dependencyResolver.Object);

            _context = new Mock<IExternalContext>();
        }

        [Test]
        public void ThenItShouldThrowExceptionWhenNoYearOfCollectionPresent()
        {
            // Arrange
            _context.Setup(c => c.Properties)
                .Returns(new Dictionary<string, string>
                {
                    {ContextPropertyKeys.TransientDatabaseConnectionString, "some-connection-string"},
                    {ContextPropertyKeys.LogLevel, "Debug"},
                    {DataLockContextPropertyKeys.DataLockEventsSource, "Submission"}
                });

            // Assert
            Assert.Throws<InvalidContextException>(() => _task.Execute(_context.Object));
        }

        [Test]
        public void ThenItShouldThrowExceptionWhenInvalidYearOfCollectionPresent()
        {
            // Arrange
            _context.Setup(c => c.Properties)
                .Returns(new Dictionary<string, string>
                {
                    {ContextPropertyKeys.TransientDatabaseConnectionString, "some-connection-string"},
                    {ContextPropertyKeys.LogLevel, "Debug"},
                    {DataLockContextPropertyKeys.YearOfCollection, "16-17"},
                    {DataLockContextPropertyKeys.DataLockEventsSource, "Submission"}
                });

            // Assert
            Assert.Throws<InvalidContextException>(() => _task.Execute(_context.Object));
        }

        [Test]
        public void ThenItShouldThrowExceptionWhenNoEventsSourcePresent()
        {
            // Arrange
            _context.Setup(c => c.Properties)
                .Returns(new Dictionary<string, string>
                {
                    {ContextPropertyKeys.TransientDatabaseConnectionString, "some-connection-string"},
                    {ContextPropertyKeys.LogLevel, "Debug"},
                    {DataLockContextPropertyKeys.YearOfCollection, "1617"}
                });

            // Assert
            Assert.Throws<InvalidContextException>(() => _task.Execute(_context.Object));
        }

        [Test]
        public void ThenItShouldThrowExceptionWhenInvalidEventsSourcePresent()
        {
            // Arrange
            _context.Setup(c => c.Properties)
                .Returns(new Dictionary<string, string>
                {
                    {ContextPropertyKeys.TransientDatabaseConnectionString, "some-connection-string"},
                    {ContextPropertyKeys.LogLevel, "Debug"},
                    {DataLockContextPropertyKeys.YearOfCollection, "1617"},
                    {DataLockContextPropertyKeys.DataLockEventsSource, "Invalid"}
                });

            // Assert
            Assert.Throws<InvalidContextException>(() => _task.Execute(_context.Object));
        }
    }
}