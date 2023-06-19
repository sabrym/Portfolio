using System;
using Portfolio.Data;

namespace Portfolio.Services.Interfaces
{
	public interface IStockTickerService
	{
		Task<StockInfo> GetStockInformation(string symbol);
		Task<DailyStock> GetStockInformationByDate(string symbol, DateTime reportDate);
    }
}

