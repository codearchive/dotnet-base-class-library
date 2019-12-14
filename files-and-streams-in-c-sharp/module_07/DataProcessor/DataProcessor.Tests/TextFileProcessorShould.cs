using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace DataProcessor.Tests
{
    public class TextFileProcessorShould
    {
        [Fact]
        public void MakeSecondLineUpperCase()
        {
            var mockFileInput = new MockFileData("Line 1\nLine 2\nLine 3");

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(@"d:\root\in\myfile.txt", mockFileInput);
            mockFileSystem.AddDirectory(@"d:\root\out");

            var sut = new TextFileProcessor(@"d:\root\in\myfile.txt",
                @"d:\root\out\myfile.txt", mockFileSystem);

            sut.Process();

            Assert.True(mockFileSystem.FileExists(@"d:\root\out\myfile.txt"));

            MockFileData processedFile = mockFileSystem.GetFile(@"d:\root\out\myfile.txt");

            string[] lines = processedFile.TextContents.SplitLines();

            Assert.Equal("Line 1", lines[0]);
            Assert.Equal("LINE 2", lines[1]);
            Assert.Equal("Line 3", lines[2]);
        }
    }
}
