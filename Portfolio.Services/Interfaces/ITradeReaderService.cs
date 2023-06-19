using System;
using Portfolio.Data;

namespace Portfolio.Services.Interfaces
{
	public interface ITradeReaderService
	{
        List<Trade> RetrieveItems(DateTime reportingDate);
        List<Trade> RetrieveItemsForPandL(DateTime yearEnding);
    }
}

