using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using Xunit;

namespace DataProcessor.Tests
{
    public class CsvFileProcessorShould
    {
        [Fact]
        public void AddLargestNumber()
        {
            const string inputDir = @"d:\root\in";
            const string inputFileName = @"myfile.csv";
            var inputFilePath = Path.Combine(inputDir, inputFileName);

            const string outputDir = @"d:\root\out";
            const string outputFileName = @"myfileout.csv";
            var outputFilePath = Path.Combine(outputDir, outputFileName);

            var csvLines = new StringBuilder();
            csvLines.AppendLine("OrderNumber,CustomerNumber,Description,Quantity");
            csvLines.AppendLine("42, 100001, Shirt, II");
            csvLines.AppendLine("43, 200002, Shorts, I");
            csvLines.AppendLine("@ This is a comment");
            csvLines.AppendLine("");
            csvLines.AppendLine("44, 300003, Cap, V");

            var mockFileInput = new MockFileData(csvLines.ToString());
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(inputFilePath, mockFileInput);
            mockFileSystem.AddDirectory(outputDir);

            var sut = new CsvFileProcessor(inputFilePath, outputFilePath, mockFileSystem);

            sut.Process();

            Assert.True(mockFileSystem.FileExists(outputFilePath));

            MockFileData processedFile = mockFileSystem.GetFile(outputFilePath);

            string[] lines = processedFile.TextContents.SplitLines();

            Assert.Equal("OrderNumber,Customer,Amount", lines[0]);
            Assert.Equal("42,100001,2", lines[1]);
            Assert.Equal("43,200002,1", lines[2]);
            Assert.Equal("44,300003,5", lines[3]);
        }
    }
}
