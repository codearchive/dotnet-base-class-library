﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            using (StreamWriter output = File.CreateText(OutputFilePath))
            using (CsvWriter csvWriter = new CsvWriter(output))
            {
                csvReader.Configuration.TrimOptions = CsvHelper.Configuration.TrimOptions.Trim;
                csvReader.Configuration.Comment = '@';
                csvReader.Configuration.AllowComments = true;
                csvReader.Configuration.RegisterClassMap<ProcessedOrderMap>();

                IEnumerable<ProcessedOrder> records = csvReader.GetRecords<ProcessedOrder>();

                //csvWriter.WriteRecords(records);

                csvWriter.WriteHeader<ProcessedOrder>();
                csvWriter.NextRecord();

                var recordsArray = records.ToArray();
                for (int i = 0; i < recordsArray.Length; i++)
                {
                    csvWriter.WriteField(recordsArray[i].OrderNumber);
                    csvWriter.WriteField(recordsArray[i].Customer);
                    csvWriter.WriteField(recordsArray[i].Amount);

                    bool isLastRecord = i == recordsArray.Length - 1;

                    if (!isLastRecord) csvWriter.NextRecord();
                }
            }
        }
    }
}