
using System;
using System.Collections.Specialized;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.Protected;
using Portfolio.Data.Configs;

namespace Portfolio.Services.Tests
{
	public class StockTickerServiceTests
	{
		private Mock<IMemoryCache> _memoryCacheMock;
		private Mock<IHttpClientFactory> _httpClientFactory;

		public StockTickerServiceTests()
		{
            _httpClientFactory = new Mock<IHttpClientFactory>();
            _memoryCacheMock = new Mock<IMemoryCache>();
		}

        [Fact]
        public async Task GetStockInformation_WithInvalidSymbol_ThrowsException()
        {
            // arrange
            var symbol = string.Empty;
            var stockTickerService = new StockTickerService(_httpClientFactory.Object, new StockConfig { ApiKey = "q23", Url = "http://www.google.com" }, _memoryCacheMock.Object);

            // act           
            // assert
            await Assert.ThrowsAsync<Utilities.ApplicationException>(() => stockTickerService.GetStockInformation(symbol));
        }

        [Fact]
        public async Task GetStockInformation_WithValidSymbolWithCacheEmpty_ReturnsSuccessfulResult()
        {
            // arrange
            var payload = "{\n    \"Meta Data\": {\n        \"1. Information\": \"Daily Time Series with Splits and Dividend Events\",\n        \"2. Symbol\": \"msft\",\n        \"3. Last Refreshed\": \"2023-06-16\",\n        \"4. Output Size\": \"Full size\",\n        \"5. Time Zone\": \"US/Eastern\"\n    },\n    \"Time Series (Daily)\": {\n        \"2023-06-16\": {\n            \"1. open\": \"351.32\",\n            \"2. high\": \"351.47\",\n            \"3. low\": \"341.95\",\n            \"4. close\": \"342.33\",\n            \"5. adjusted close\": \"342.33\",\n            \"6. volume\": \"46551985\",\n            \"7. dividend amount\": \"0.0000\",\n            \"8. split coefficient\": \"1.0\"\n        }\n    }\n}";
            var symbol = "MSFT";

            var entryMock = new Mock<ICacheEntry>();
            _memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(entryMock.Object);
            _httpClientFactory = BuildHttpClientFactoryMock(payload);

            var stockTickerService = new StockTickerService(_httpClientFactory.Object, new StockConfig { ApiKey = "q23", Url = "http://www.mockapi.com/" }, _memoryCacheMock.Object);

            // act
            var result = await stockTickerService.GetStockInformation(symbol);

            // assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetStockInformation_WithValidSymbolWithCache_ReturnsSuccessfulResult()
        {
            // arrange
            var payload = "{\n    \"Meta Data\": {\n        \"1. Information\": \"Daily Time Series with Splits and Dividend Events\",\n        \"2. Symbol\": \"msft\",\n        \"3. Last Refreshed\": \"2023-06-16\",\n        \"4. Output Size\": \"Full size\",\n        \"5. Time Zone\": \"US/Eastern\"\n    },\n    \"Time Series (Daily)\": {\n        \"2023-06-16\": {\n            \"1. open\": \"351.32\",\n            \"2. high\": \"351.47\",\n            \"3. low\": \"341.95\",\n            \"4. close\": \"342.33\",\n            \"5. adjusted close\": \"342.33\",\n            \"6. volume\": \"46551985\",\n            \"7. dividend amount\": \"0.0000\",\n            \"8. split coefficient\": \"1.0\"\n        }\n    }\n}";
            var symbol = "MSFT";


            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            memoryCache.Set(symbol, DataGenerator.StockInfoMocker);

            _httpClientFactory = BuildHttpClientFactoryMock(payload);

            var stockTickerService = new StockTickerService(_httpClientFactory.Object, new StockConfig { ApiKey = "q23", Url = "http://www.mockapi.com/" }, memoryCache);

            // act
            var result = await stockTickerService.GetStockInformation(symbol);

            // assert
            Assert.NotNull(result);
            Assert.Equal(DataGenerator.StockInfoMocker.StockMetaData, result.StockMetaData);
        }

        [Fact]
        public async Task GetStockInformationByDate_WithValidSymbolAndReportDate_ReturnsValidResult()
        {
            // arrange
            var payload = "{\n    \"Meta Data\": {\n        \"1. Information\": \"Daily Time Series with Splits and Dividend Events\",\n        \"2. Symbol\": \"msft\",\n        \"3. Last Refreshed\": \"2023-06-16\",\n        \"4. Output Size\": \"Full size\",\n        \"5. Time Zone\": \"US/Eastern\"\n    },\n    \"Time Series (Daily)\": {\n        \"2023-06-16\": {\n            \"1. open\": \"351.32\",\n            \"2. high\": \"351.47\",\n            \"3. low\": \"341.95\",\n            \"4. close\": \"342.33\",\n            \"5. adjusted close\": \"342.33\",\n            \"6. volume\": \"46551985\",\n            \"7. dividend amount\": \"0.0000\",\n            \"8. split coefficient\": \"1.0\"\n        },\n        \"2023-06-15\": {\n            \"1. open\": \"351.32\",\n            \"2. high\": \"351.47\",\n            \"3. low\": \"341.95\",\n            \"4. close\": \"342.33\",\n            \"5. adjusted close\": \"342.33\",\n            \"6. volume\": \"46551985\",\n            \"7. dividend amount\": \"0.0000\",\n            \"8. split coefficient\": \"1.0\"\n        }\n    }\n}";
            var symbol = "MSFT";
            var reportDate = new DateTime(2023, 06, 16);
            _httpClientFactory = BuildHttpClientFactoryMock(payload);

            var stockTickerService = new StockTickerService(_httpClientFactory.Object, new StockConfig { ApiKey = "q23", Url = "http://www.mockapi.com/" }, new MemoryCache(new MemoryCacheOptions()));

            // act
            var result = await stockTickerService.GetStockInformationByDate(symbol, reportDate);

            // assert
            Assert.Equal(reportDate, result.Date);
        }

        [Fact]
        public async Task GetStockInformationByDate_WithValidSymbolAndReportDateWithMissingTradeData_ThrowsException()
        {
            // arrange
            var payload = "{\n    \"Meta Data\": {\n        \"1. Information\": \"Daily Time Series with Splits and Dividend Events\",\n        \"2. Symbol\": \"msft\",\n        \"3. Last Refreshed\": \"2023-06-16\",\n        \"4. Output Size\": \"Full size\",\n        \"5. Time Zone\": \"US/Eastern\"\n    },\n    \"Time Series (Daily)\": {\n        \"2023-06-16\": {\n            \"1. open\": \"351.32\",\n            \"2. high\": \"351.47\",\n            \"3. low\": \"341.95\",\n            \"4. close\": \"342.33\",\n            \"5. adjusted close\": \"342.33\",\n            \"6. volume\": \"46551985\",\n            \"7. dividend amount\": \"0.0000\",\n            \"8. split coefficient\": \"1.0\"\n        }\n    }\n}";
            var symbol = "MSFT";
            var reportDate = new DateTime(2023, 06, 16);
            _httpClientFactory = BuildHttpClientFactoryMock(payload);

            var stockTickerService = new StockTickerService(_httpClientFactory.Object, new StockConfig { ApiKey = "q23", Url = "http://www.mockapi.com/" }, new MemoryCache(new MemoryCacheOptions()));

            // act
            // assert
            await Assert.ThrowsAsync<Utilities.ApplicationException>(() => stockTickerService.GetStockInformationByDate(symbol, DateTime.Today));
        }

        private static Mock<IHttpClientFactory> BuildHttpClientFactoryMock(string response)
		{
			var httpclientFactoryMock = new Mock<IHttpClientFactory>();
            var handlerMock = new Mock<HttpMessageHandler>();

            HttpResponseMessage result = new HttpResponseMessage();
            result.Content = new StringContent(response);
            // add response to result
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(result);

            var client = new HttpClient(handlerMock.Object);

            httpclientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

			return httpclientFactoryMock;
        }
	}
}

