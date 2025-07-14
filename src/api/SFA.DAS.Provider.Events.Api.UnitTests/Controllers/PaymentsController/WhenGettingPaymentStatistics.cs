using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using MediatR;
using Moq;
using Microsoft.ApplicationInsights;
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
        private Mock<TelemetryClient> _telemetryClient;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();

            _telemetryClient = new Mock<TelemetryClient>(); _telemetryClient.Setup(l => l.TrackException(It.IsAny<Exception>(), It.IsAny<Dictionary<string,string>>(), It.IsAny<Dictionary<string, double>>()))
                .Callback<Exception, string, object[]>((ex, msg, args) =>
                {
                    Console.WriteLine($"Error Logged\n{msg}\n{ex}");
                });

            _controller = new Api.Controllers.PaymentsController(_mediator.Object, _telemetryClient.Object);
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