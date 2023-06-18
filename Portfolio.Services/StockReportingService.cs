using System;
using Portfolio.Data;
using Portfolio.Services.Interfaces;
using Serilog;

namespace Portfolio.Services
{
	public class StockReportingService: IReportingService
	{
        private readonly IStockTickerService _stockTickerService;
        private readonly ITradeReaderService _tradeReaderService;

        public StockReportingService(IStockTickerService stockTickerService, ITradeReaderService tradeReaderService) => (_stockTickerService, _tradeReaderService) = (stockTickerService, tradeReaderService);

        public async Task<byte[]> GenerateReportWithItemRetrieval(DateTime reportingDate)
        {
            try
            {
                Log.Information("Generating P & L report as of {date}", reportingDate);
                var processingResult = await _tradeReaderService.RetrieveItemsForPandL(reportingDate);

                if (processingResult == null)
                    throw new Exception("Unable to process input");

                var list = new List<StockReportItem>();
                foreach (var item in processingResult.GroupBy(x => x.Ticker))
                {
                    var trades = item.ToList();
                    var stock = await _stockTickerService.GetStockInformationByDate(item.Key, reportingDate);
                    var stockItem = new StockReportItem(item.Key, reportingDate, trades.Sum(x => x.Cost), trades.Sum(x => x.Quantity), stock.Price, stock.Close);
                    list.Add(stockItem);
                }

                // generate the report using closed xml
                var generatedReport = ExcelReportWriter.GenerateReport(list, "default", "das");
                Log.Information("Generated P & L report as of {date}", reportingDate);
                return generatedReport;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error generating P & L report as of {date}", reportingDate);
                throw;
            }
        }
    }
}

