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
            Console.WriteLine($"inputFileDirectoryPath: {inputFileDirectoryPath}");
            string backupDirectoryPath = Path.Combine(rootDirectoryPath, BackupDirectoryName);

            //if (!Directory.Exists(backupDirectoryPath))
            //{
            //    Console.WriteLine($"Creating {backupDirectoryPath}");
                Directory.CreateDirectory(backupDirectoryPath);
            //}

            string inputFileName = Path.GetFileName(InputFilePath);
            string backupFilePath = Path.Combine(backupDirectoryPath, inputFileName);
            Console.WriteLine($"Copying {InputFilePath} to {backupFilePath}");
            File.Copy(InputFilePath, backupFilePath, true);

            Directory.CreateDirectory(Path.Combine(rootDirectoryPath, InProgressDirectoryName));
            string inProgressFilePath = Path.Combine(rootDirectoryPath, InProgressDirectoryName, inputFileName);

            if (File.Exists(inProgressFilePath))
            {
                Console.WriteLine($"ERROR: file with the name {inProgressFilePath} is already being processed");
                return;
            }

            Console.WriteLine($"Moving {InputFilePath} to {inProgressFilePath}");
            File.Move(InputFilePath, inProgressFilePath);
        }
    }
}
