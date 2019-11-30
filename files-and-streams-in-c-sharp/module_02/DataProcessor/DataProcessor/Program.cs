using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Xml;

namespace DataProcessor
{
    class Program
    {
        private static ConcurrentDictionary<string, string> FilesToProcess = new ConcurrentDictionary<string, string>();

        static void Main(string[] args)
        {
            Console.WriteLine("Parsing command line options");

            var directoryToWatch = args[0];

            if (!Directory.Exists(directoryToWatch))
            {
                Console.WriteLine($"ERROR: {directoryToWatch} does not exist");
            }
            else
            {
                Console.WriteLine($"Watching directory {directoryToWatch} for changes");
                using (var inputFileWatcher = new FileSystemWatcher(directoryToWatch))
                using (var timer = new Timer(ProcessFiles, null, 0, 1000))
                {
                    inputFileWatcher.IncludeSubdirectories = false;
                    inputFileWatcher.InternalBufferSize = 32768;
                    inputFileWatcher.Filter = "*.*"; // by default
                    inputFileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;

                    inputFileWatcher.Created += FileCreated;
                    inputFileWatcher.Changed += FileChanged;
                    inputFileWatcher.Deleted += FileDeleted;
                    inputFileWatcher.Renamed += FileRenamed;
                    inputFileWatcher.Error += WatcherError;

                    inputFileWatcher.EnableRaisingEvents = true;

                    Console.WriteLine("Press enter to quit");
                    Console.ReadLine();
                }
            }
        }

        private static void WatcherError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine($"ERROR: file system watching may no longer be active: {e.GetException()}");
        }
    

        private static void FileRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"* File Renamed: {e.OldName} to {e.Name} - type: {e.ChangeType}");
        }
    

        private static void FileDeleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"* File Deleted: {e.Name} - type: {e.ChangeType}");
        }

        private static void FileChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"* File Changed: {e.Name} - type: {e.ChangeType}");

            //var fileProcessor = new FileProcessor(e.FullPath);
            //fileProcessor.Process();

            FilesToProcess.TryAdd(e.FullPath, e.FullPath);
        }

        private static void FileCreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"* File created: {e.Name} - type: {e.ChangeType}");

            //var fileProcessor = new FileProcessor(e.FullPath);
            //fileProcessor.Process();

            FilesToProcess.TryAdd(e.FullPath, e.FullPath);
        }

        private static void ProcessDirectory(string directoryPath, string fileType)
        {
            //var allFiles = Directory.GetFiles(directoryPath);

            switch (fileType)
            {
                case "TEXT":
                    string[] textFiles = Directory.GetFiles(directoryPath, "*.txt");
                    foreach (var textFilePath in textFiles)
                    {
                        var fileProcessor = new FileProcessor(textFilePath);
                        fileProcessor.Process();
                    }
                    break;
                default:
                    Console.WriteLine($"ERROR: {fileType} is not supported");
                    return;
            }
        }

        private static void ProcessSingleFile(string filePath)
        {
            var fileProcessor = new FileProcessor(filePath);
            fileProcessor.Process();
        }

        private static void ProcessFiles(object stateInfo)
        {
            foreach (var fileName in FilesToProcess.Keys)
            {
                if (FilesToProcess.TryRemove(fileName, out _))
                {
                    var fileProcessor = new FileProcessor(fileName);
                    fileProcessor.Process();
                }
            }
        }
    }
}
