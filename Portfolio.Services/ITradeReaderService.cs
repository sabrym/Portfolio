using System;
using Portfolio.Data;

namespace Portfolio.Services
{
	public interface ITradeReaderService
	{
		Task<ProcessingResult> RetrieveItems();

		Task SaveItems();
	}
}

