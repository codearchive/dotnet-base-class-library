using System;
using System.IO;

namespace DataProcessor
{
    internal class FileProcessor
    {
        private string InputFilePath { get; }

        public FileProcessor(string filePath)
        {
            InputFilePath = filePath;
        }

        public void Process()
        {
            Console.WriteLine($"Begin process of {InputFilePath}");

            if (!File.Exists(InputFilePath))
            {
                Console.WriteLine($"ERROR: file {InputFilePath} does not exist");
                return;
            }

            string rootDirectoryPath = new DirectoryInfo(InputFilePath).Parent.FullName;
            Console.WriteLine(rootDirectoryPath);
        }
    }
}
