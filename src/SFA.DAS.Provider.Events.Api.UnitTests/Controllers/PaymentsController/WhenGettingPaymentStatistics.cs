﻿using System;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Payments.GetPaymentsStatistics;

namespace SFA.DAS.Provider.Events.Api.UnitTests.Controllers.PaymentsController
{
    public class WhenGettingPaymentStatistics
    {
        private PaymentStatistics _statistics;
        private Mock<IMediator> _mediator;
        private Api.Controllers.PaymentsController _controller;
        private Mock<ILogger> _logger;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
        
            _logger = new Mock<ILogger>();
            _logger.Setup(l => l.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<Exception, string, object[]>((ex, msg, args) =>
                {
                    Console.WriteLine($"Error Logged\n{msg}\n{ex}");
                });

            _controller = new Api.Controllers.PaymentsController(_mediator.Object, _logger.Object);
        }

        [Test]
        public async Task ThenItShouldReturnAOkResult()
        {
            //Arrange
            _statistics = new PaymentStatistics()
            {
                TotalNumberOfPayments = 13000,
                TotalNumberOfPaymentsWithRequiredPayment = 11000
            };
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetPaymentsStatisticsRequest>()))
                .ReturnsAsync(new GetPaymentsStatisticsResponse()
                {
                    IsValid = true,
                    Result = _statistics
                });

            // Act
            var actual = await _controller
                .GetPaymentStatistics()
                .ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<OkNegotiatedContentResult<PaymentStatistics>>(actual);

            var paymentStatistics = ((OkNegotiatedContentResult<PaymentStatistics>) actual).Content;

            paymentStatistics.TotalNumberOfPayments.Should().Be(13000);
            paymentStatistics.TotalNumberOfPaymentsWithRequiredPayment.Should().Be(11000);
        }

        [Test]
        public void AndAnExceptionReturnedThenItShouldthrowError()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetPaymentsStatisticsRequest>()))
                .Throws(new Exception("Unit tests"));

            // Act/Assert
            Assert.ThrowsAsync<Exception>(() => _controller.GetPaymentStatistics());
        }
    }
}