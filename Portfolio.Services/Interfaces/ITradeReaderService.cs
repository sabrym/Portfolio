using System;
using Portfolio.Data;

namespace Portfolio.Services.Interfaces
{
	public interface ITradeReaderService
	{
		Task<ProcessingResult> RetrieveItems(DateTime reportingDate);
        Task<List<Trade>> RetrieveItemsForPandL(DateTime yearEnding);
    }
}

