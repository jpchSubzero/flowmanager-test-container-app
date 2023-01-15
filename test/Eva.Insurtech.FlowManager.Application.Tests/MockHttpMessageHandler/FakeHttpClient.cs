using Moq;
using Moq.Protected;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Eva.Insurtech.Flows.MockHttpMessageHandler
{
    public static class FakeHttpClient
    {
        public static Mock<HttpMessageHandler> GetMockHttpMessageHandler(string fakeResponse)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(fakeResponse),
               })
               .Verifiable();

            return handlerMock;
        }
        public static Mock<HttpMessageHandler> GetMockWithHttpMessageHandlerSequence(List<string> fakeResponses)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var handlerPart = handlerMock
               .Protected()
               .SetupSequence<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               );
            foreach (var fakeResponse in fakeResponses)
            {
                handlerPart = AdddReturnPart(handlerPart, fakeResponse);
            }
            handlerMock.Verify();
            return handlerMock;
        }
        private static Moq.Language.ISetupSequentialResult<Task<HttpResponseMessage>> AdddReturnPart(
            Moq.Language.ISetupSequentialResult<Task<HttpResponseMessage>> handlerPart,
            string fakeResponse)
        {
            return handlerPart
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(fakeResponse)
               });
        }
    }
}
