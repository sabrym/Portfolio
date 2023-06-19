using System;
using Portfolio.Data;

namespace Portfolio.Services.Tests
{
	internal static class DataGenerator
	{
        internal static List<Trade> GenerateTrades => new List<Trade> { new Trade("MSNBC", DateTime.Today, TradeAction.Buy, 100, 80.00m) };

        internal static DailyStock DailyStockInfoMocker => new DailyStock
        {
            Price = 92.64m,
            Close = 90.77m,
            Date = new DateTime(2018, 04, 10)
        };

        internal static StockInfo StockInfoMocker => new StockInfo { DailyStocks = new List<DailyStock> { DailyStockInfoMocker }, StockMetaData = new StockMetaData { Information = "foo" } };
    }
}

