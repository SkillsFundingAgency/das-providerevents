using System;
using System.Collections.Generic;
using CS.Common.External.Interfaces;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.DCFS.Context;
using SFA.DAS.Payments.DCFS.Infrastructure.DependencyResolution;

namespace SFA.DAS.Provider.Events.Submission.UnitTests.SubmissionEventsTaskTests
{
    public class WhenExecuted
    {
        private Mock<SubmissionEventsProcessor> _processor;
        private Mock<IDependencyResolver> _dependencyResolver;
        private SubmissionEventsTask _task;
        private Mock<IExternalContext> _context;

        [SetUp]
        public void Arrange()
        {
            _processor = new Mock<SubmissionEventsProcessor>();

            _dependencyResolver = new Mock<IDependencyResolver>();
            _dependencyResolver.Setup(r => r.GetInstance<SubmissionEventsProcessor>())
                .Returns(_processor.Object);

            _task = new SubmissionEventsTask(_dependencyResolver.Object);

            _context = new Mock<IExternalContext>();
            _context.Setup(c => c.Properties)
                .Returns(new Dictionary<string, string>
                {
                    {ContextPropertyKeys.TransientDatabaseConnectionString, "some-connection-string"},
                    {ContextPropertyKeys.LogLevel, "Debug"},
                    {SubmissionEventsContextPropertyKeys.YearOfCollection,"1617" }
                });
        }

        [Test]
        public void ThenItShouldInitaliseTheDependencyResolver()
        {
            // Act
            _task.Execute(_context.Object);

            // Assert
            _dependencyResolver.Verify(r => r.Init(It.Is<Type>(t => t == typeof(SubmissionEventsProcessor)), It.IsAny<ContextWrapper>()), Times.Once);
        }

        [Test]
        public void ThenItShouldUseAResolvedProcessorToStartProcessing()
        {
            // Act
            _task.Execute(_context.Object);

            // Assert
            _dependencyResolver.Verify(r => r.GetInstance<SubmissionEventsProcessor>(), Times.Once);
            _processor.Verify(p => p.Process(), Times.Once);
        }
    }
}
