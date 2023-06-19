
using System;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.Protected;

namespace Portfolio.Services.Tests
{
	public class StockTickerServiceTests
	{
		private Mock<IMemoryCache> _memoryCacheMock;
		private Mock<IHttpClientFactory> _httpClientFactory;

		public StockTickerServiceTests()
		{

		}

		private static Mock<IHttpClientFactory> BuildHttpClientForFactoryMock<T>(T response)
		{
			var httpclientFactoryMock = new Mock<IHttpClientFactory>();
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            HttpResponseMessage result = new HttpResponseMessage();
            // add response to result
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(result)
                .Verifiable();

            var client = new HttpClient(handlerMock.Object);

            httpclientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

			return httpclientFactoryMock;
        }
	}
}

