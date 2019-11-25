using System;

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
        }
    }
}
