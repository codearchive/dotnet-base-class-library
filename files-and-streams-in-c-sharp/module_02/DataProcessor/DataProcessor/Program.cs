using System;
using System.Runtime.Caching;
using System.IO;
using System.Threading;

namespace DataProcessor
{
    class Program
    {
        private static MemoryCache FilesToProcess = MemoryCache.Default;
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

                ProcessExistingFiles(directoryToWatch);

                using (var inputFileWatcher = new FileSystemWatcher(directoryToWatch))
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

        private static void ProcessExistingFiles(string inputDirectory)
        {
            Console.WriteLine($"Checking {inputDirectory} for existing files");

            foreach (var filePath in Directory.EnumerateFiles(inputDirectory))
            {
                Console.WriteLine($" - Found {filePath}");
                AddToCache(filePath);
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

            AddToCache(e.FullPath);
        }

        private static void FileCreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"* File created: {e.Name} - type: {e.ChangeType}");

            //var fileProcessor = new FileProcessor(e.FullPath);
            //fileProcessor.Process();

            AddToCache(e.FullPath);
        }

        private static void AddToCache(string fullPath)
        {
            var item = new CacheItem(fullPath, fullPath);

            var policy = new CacheItemPolicy
            {
                RemovedCallback = ProcessFile,
                SlidingExpiration = TimeSpan.FromSeconds(2)
            };

            FilesToProcess.Add(item, policy);
        }

        private static void ProcessFile(CacheEntryRemovedArguments arguments)
        {
            Console.WriteLine($"*  Cache item removed: {arguments.CacheItem.Key} because {arguments.RemovedReason}");

            if (arguments.RemovedReason == CacheEntryRemovedReason.Expired)
            {
                var fileProcessor = new FileProcessor(arguments.CacheItem.Key);
                fileProcessor.Process();
            }
            else
            {
                Console.WriteLine($"WARNING: {arguments.CacheItem.Key} was removed unexpectedly");
            }
        }
    }
}
