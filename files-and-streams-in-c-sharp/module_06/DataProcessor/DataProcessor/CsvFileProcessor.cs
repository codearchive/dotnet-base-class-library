using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;

namespace DataProcessor
{
    public class CsvFileProcessor
    {
        public string InputFilePath { get; }
        public string OutputFilePath { get; }

        public CsvFileProcessor(string inputFilePath, string outputFilePath)
        {
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
        }

        public void Process()
        {
            using (StreamReader input = File.OpenText(InputFilePath))
            using (CsvReader csvReader = new CsvReader(input))
            {
                IEnumerable<ProcessedOrder> records = csvReader.GetRecords<ProcessedOrder>();

                csvReader.Configuration.TrimOptions = CsvHelper.Configuration.TrimOptions.Trim;
                csvReader.Configuration.Comment = '@';
                csvReader.Configuration.AllowComments = true;
                //csvReader.Configuration.IgnoreBlankLines = false; // Unit 6.4
                //csvReader.Configuration.Delimiter = ";"; // Unit 6.5
                //csvReader.Configuration.HasHeaderRecord = false; // Unit 6.6
                //csvReader.Configuration.HeaderValidated = null; // Unit 6.8
                //csvReader.Configuration.MissingFieldFound = null; // Unit 6.8
                csvReader.Configuration.RegisterClassMap<ProcessedOrderMap>();

                foreach (ProcessedOrder record in records)
                {
                    Console.WriteLine(record.OrderNumber);
                    Console.WriteLine(record.Customer);
                    Console.WriteLine(record.Amount);
                }
            }
        }
    }
}