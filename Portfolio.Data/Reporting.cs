using System;
namespace Portfolio.Data
{
	public record StockReportItem(string Ticker, DateTime AsOfDate, decimal Cost, int Quantity, decimal Price, decimal PreviousClose)
	{
		public decimal MarketValue { get => Price * Quantity; }
		public decimal DailyPAndL { get => (Price - PreviousClose) * Quantity; }
        public decimal InceptionPandL { get => MarketValue - Cost; }
    }
}

