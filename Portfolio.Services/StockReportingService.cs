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
                var processingResult = _tradeReaderService.RetrieveItemsForPandL(reportingDate);

                if (!processingResult.Any())
                    throw new ApplicationException($"No trade files found for reporting period: {reportingDate}");

                var list = new List<StockReportItem>();
                foreach (var item in processingResult.GroupBy(x => x.Ticker))
                {
                    var trades = item.ToList();
                    var stock = await _stockTickerService.GetStockInformationByDate(item.Key, reportingDate);
                    var totalCost = trades.Where(x => x.Tradetype == TradeAction.Buy).Sum(x => x.Cost);
                    var totalQuantity = trades.Where(x => x.Tradetype == TradeAction.Buy).Sum(x => x.Quantity);

                    list.Add(new StockReportItem(item.Key, reportingDate, totalCost, totalQuantity, stock.Price, stock.Close));
                }

                // generate the report using closed xml
                var generatedReport = ExcelReportWriter.GenerateReport(list, "P&L");

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

