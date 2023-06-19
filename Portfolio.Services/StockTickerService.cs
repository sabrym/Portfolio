using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Portfolio.Data;
using Portfolio.Data.Configs;
using Portfolio.Services.Interfaces;
using Portfolio.Utilities;
using Serilog;

namespace Portfolio.Services
{
	public class StockTickerService: IStockTickerService
	{
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly StockConfig _configuration;
        private readonly IMemoryCache _memoryCache;

        public StockTickerService(IHttpClientFactory httpClientFactory, StockConfig configuration, IMemoryCache memoryCache) => (_httpClientFactory, _configuration, _memoryCache) = (httpClientFactory, configuration, memoryCache);

        public async Task<DailyStock> GetStockInformationByDate(string symbol, DateTime reportDate)
        {
            var stockInformation = await GetStockInformation(symbol);
            var (actualDate, previousDate) = DateTimeUtilities.GetReportingDate(reportDate);
            var stockInfoForReportingDate = stockInformation.DailyStocks.FirstOrDefault(x => x.Date == actualDate);
            var stockInfoForPrevious = stockInformation.DailyStocks.FirstOrDefault(x => x.Date == previousDate);

            if (stockInfoForReportingDate == null || stockInfoForPrevious == null)
                throw new Utilities.ApplicationException($"Insufficient pricing information found for symbol: {symbol}");

            stockInfoForReportingDate.Close = stockInfoForPrevious.Price;
            return stockInfoForReportingDate;
        }

        public async Task<StockInfo> GetStockInformation(string symbol)
        {
            try
            {
                Log.Information("Retrieving Stock Information for symbol: {symbol}", symbol);

                if (string.IsNullOrWhiteSpace(symbol))
                    throw new Utilities.ApplicationException("The symbol is invalid");

                if (!_memoryCache.TryGetValue(symbol, out StockInfo stockingfo))
                {
                    using HttpClient client = _httpClientFactory.CreateClient();
                    var response = await client.GetAsync($"{_configuration.Url}&symbol={symbol}&apikey={_configuration.ApiKey}&outputsize=full");
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
    }
}

