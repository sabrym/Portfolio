using System;
namespace Portfolio.Utilities
{
	public interface IFileWrapper
	{
		bool FileExists(string fileLocation);
		bool DirectoryIsEmpty(string path);
	}
}

