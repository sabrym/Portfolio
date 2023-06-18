using System;
namespace Portfolio.Services.Interfaces
{
	public interface IReportingService
	{
		Task<byte[]> GenerateReportWithItemRetrieval(DateTime reportingDate);
	}
}

