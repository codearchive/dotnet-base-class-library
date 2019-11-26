using System;
using System.IO;

namespace DataProcessor
{
    internal class FileProcessor
    {
        private static readonly string BackupDirectoryName = "backup";
        private static readonly string InProgressDirectoryName = "processing";
        private static readonly string CompletedDirectoryName = "completed";

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

            string rootDirectoryPath = new DirectoryInfo(InputFilePath).Parent.Parent.FullName;
            Console.WriteLine($"Root data path is {rootDirectoryPath}");

            string inputFileDirectoryPath = Path.GetDirectoryName(InputFilePath);
            string backupDirectoryPath = Path.Combine(rootDirectoryPath, BackupDirectoryName);

            //if (!Directory.Exists(backupDirectoryPath))
            //{
            //    Console.WriteLine($"Creating {backupDirectoryPath}");
                Directory.CreateDirectory(backupDirectoryPath);
            //}
        }
    }
}
