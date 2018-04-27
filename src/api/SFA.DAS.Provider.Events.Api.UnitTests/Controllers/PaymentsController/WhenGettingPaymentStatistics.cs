using System;
using System.Threading.Tasks;
using System.Web.Http.Results;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using Ploeh.AutoFixture.NUnit3;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data;
using SFA.DAS.Provider.Events.Application.Payments.GetPaymentsQuery;
using SFA.DAS.Provider.Events.Application.Payments.GetPaymentsStatistics;
using SFA.DAS.Provider.Events.Application.Period.GetPeriodQuery;
using SFA.DAS.Provider.Events.Application.Validation;

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
                TotalPayments = 13000
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
        }

       

       
        [Test]
        public async Task AndAnExceptionReturnedThenItShouldReturnBadRequest()
        {
            // Arrange
           
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetPaymentsStatisticsRequest>()))
                .ReturnsAsync(new GetPaymentsStatisticsResponse()
                {
                    IsValid = false,
                    Exception = new Exception("Unit tests")
                });

            // Act
            var actual = await _controller
                .GetPaymentStatistics()
                .ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<InternalServerErrorResult>(actual);
        }

       
    }
}
