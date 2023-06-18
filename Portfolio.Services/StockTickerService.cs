using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Portfolio.Data;
using Portfolio.Services.Interfaces;
using Serilog;

namespace Portfolio.Services
{
	public class StockTickerService: IStockTickerService
	{
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        public StockTickerService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IMemoryCache memoryCache) => (_httpClientFactory, _configuration, _memoryCache) = (httpClientFactory, configuration, memoryCache);

        public async Task<DailyStock> GetStockInformationByDate(string symbol, DateTime? reportDate)
        {
            // check if there is an existing reporting date, else take the current date
            reportDate = reportDate.GetValueOrDefault(DateTime.Today);
            var stockInformation = await GetStockInformation(symbol);
            var stockInfoForReportingDate = stockInformation.DailyStocks.FirstOrDefault(x => x.Date == reportDate);
            var stockInfoForPrevious = stockInformation.DailyStocks.FirstOrDefault(x => x.Date == reportDate.Value.AddDays(-1));

            if (stockInfoForReportingDate == null || stockInfoForPrevious == null)
                throw new Exception($"Insufficient pricing information found for symbol: {symbol}");

            stockInfoForReportingDate.Close = stockInfoForPrevious.Price;
            return stockInfoForReportingDate;
        }

        public async Task<StockInfo> GetStockInformation(string symbol)
        {
            try
            {
                Log.Information("Retrieving Stock Information for symbol: {symbol}", symbol);

                if (!_memoryCache.TryGetValue(symbol, out StockInfo stockingfo))
                {
                    using HttpClient client = _httpClientFactory.CreateClient();
                    var response = await client.GetAsync($"{_configuration["Stock:Url"]}&symbol={symbol}&apikey={_configuration["Stock:ApiKey"]}&outputsize=full");
                    response.EnsureSuccessStatusCode();

                    var items = await response.Content.ReadAsStringAsync();
                    stockingfo = JsonConvert.DeserializeObject<StockInfo>(items);

                    if (stockingfo == null)
                        throw new Exception($"Error retrieving stock information for {symbol}");

                    _memoryCache.Set(symbol, stockingfo);
                }

                Log.Information("Retrieved Stock Information for symbol: {symbol}, {metaData}", symbol, stockingfo.StockMetaData);

                return stockingfo;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving Stock Information for symbol: {symbol}", symbol);
                throw;
            }
        }

        public DailyStock? StockInfoMocker(string symbol)
        {
            switch (symbol)
            {
                case "MSFT":
                    return new DailyStock
                    {
                        Price = 92.64m,
                        Close = 90.77m,
                        Date = new DateTime(2018, 04, 10)
                    };
                case "GOOG":
                    return new DailyStock
                    {
                        Price = 1024.61m,
                        Close = 1015.45m,
                        Date = new DateTime(2018, 04, 10)
                    };
                default:
                    return null;
            }
        }
    }
}

