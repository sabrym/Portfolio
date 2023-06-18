using System;
namespace Portfolio.Data
{
	public record ProcessingResult(string Identifier, int TotalItems, int ProcessedCount, int FailedCount, List<Trade> Trades);
}

