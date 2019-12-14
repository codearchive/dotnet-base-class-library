using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace DataProcessor.Tests
{
    public class BinaryFileProcessorShould
    {
        [Fact]
        public void AddLargestNumber()
        {
            var mockFileInput = new MockFileData(new byte[] { 0xFF, 0x34, 0x56, 0xD2 });
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(@"d:\root\in\myfile.data", mockFileInput);
            mockFileSystem.AddDirectory(@"d:\root\out");

            var sut = new BinaryFileProcessor(@"d:\root\in\myfile.data",
                @"d:\root\out\myfile.data", mockFileSystem);

            sut.Process();

            Assert.True(mockFileSystem.FileExists(@"d:\root\out\myfile.data"));

            MockFileData processedFile = mockFileSystem.GetFile(@"d:\root\out\myfile.data");

            byte[] data = processedFile.Contents;

            Assert.Equal(5, data.Length);
            Assert.Equal(0xFF, data[4]);
        }
    }
}
