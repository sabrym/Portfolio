using System;
namespace Portfolio.Utilities
{
    public class FileWrapper : IFileWrapper
    {
        public bool DirectoryIsEmpty(string path)
        {
            return Directory.Exists(path) && Directory.GetFiles(path).Any();
        }

        public bool FileExists(string fileLocation)
        {
            return File.Exists(fileLocation);
        }
    }
}

