using System;
namespace Portfolio.Utilities
{
	public static class DateTimeUtilities
	{
		public static (DateTime reportingDate, DateTime previousClosingDate) GetReportingDate(DateTime candidateReportingDate)
		{
			var actualReportingDate = DateTime.MinValue;
			switch (candidateReportingDate.DayOfWeek)
			{
				case DayOfWeek.Saturday:
					actualReportingDate = candidateReportingDate.AddDays(-1);
					break;
				case DayOfWeek.Sunday:
                    actualReportingDate = candidateReportingDate.AddDays(-2);
                    break;
				case DayOfWeek.Monday:
                    actualReportingDate = candidateReportingDate.AddDays(-3);
                    break;
				default:
					actualReportingDate = candidateReportingDate;
					break;
			}

			return (actualReportingDate, actualReportingDate.AddDays(-1));
		}
	}
}

