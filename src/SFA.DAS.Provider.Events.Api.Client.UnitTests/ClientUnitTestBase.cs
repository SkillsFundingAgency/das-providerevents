using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace SFA.DAS.Provider.Events.Api.Client.UnitTests
{
    public class ClientUnitTestBase
    {
        protected Mock<HttpMessageHandler> _httpMessageHandlerMock;

        public Mock<HttpMessageHandler> SetupHttpMessageHandler(string responseJson)
        {
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            httpMessageHandlerMock
               .Protected()
               // Setup the PROTECTED method to mock
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               // prepare the expected response of the mocked http call
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(responseJson)
               })
               .Verifiable();

            return httpMessageHandlerMock;
        }

        public void VerifyExpectedUrlCalled(string expectedUrl, int timesCalled = 1)
        {
            var expectedUri = new Uri(expectedUrl);

            _httpMessageHandlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(timesCalled),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get
                  && req.RequestUri == expectedUri // to this uri
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
