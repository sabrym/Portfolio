using System;
namespace Portfolio.Data
{
	public record StockReportItem(string Ticker, DateTime AsOfDate, decimal Cost, int Quantity, decimal Price, decimal PreviousClose)
	{
		public decimal MarketValue { get => Price * (decimal)Quantity; }
		public decimal DailyPAndL { get => (Price - PreviousClose) * (decimal)Quantity; }
        public decimal InceptionPandL { get => MarketValue - Cost; }
    }
}

