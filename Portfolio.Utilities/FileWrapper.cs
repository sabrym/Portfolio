using System;
namespace Portfolio.Utilities
{
    public class FileWrapper : IFileWrapper
    {
        public bool FileExists(string fileLocation)
        {
            return File.Exists(fileLocation);
        }
    }
}

