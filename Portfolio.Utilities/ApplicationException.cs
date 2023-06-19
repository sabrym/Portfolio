using System;
namespace Portfolio.Utilities
{
	public class ApplicationException: Exception
	{
		public ApplicationException(string exceptionText): base(exceptionText)
		{
		}
	}
}

