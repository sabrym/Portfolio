using System;
namespace Portfolio.Data
{
	public enum TradeAction
	{
		Buy,
		Sell
	}
	public record Trade(string Ticker, DateTime TradeDate, TradeAction Tradetype, int Quantity, decimal Price);
}
