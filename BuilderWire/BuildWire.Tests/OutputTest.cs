using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using BuilderWire.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildWire.Tests
{
    [TestClass]
    public class OutputTest
    {
        public string currentInputDirectory { get; set; }
        public string currentOutputDirectory { get; set; }
        public string articleFileName { get; set; }
        public string wordsFileName { get; set; }
        public string outputFileName { get; set; }
        public string articlePath { get; set; }
        public string wordsPath { get; set; }
        public string outputPath { get; set; }
        public List<KeyValuePair<string, string>> replacements { get; set; }

        [TestInitialize]
        public void SetupTests()
        {
            currentInputDirectory = "Input";
            currentOutputDirectory = "Output";
            articleFileName = "Article.txt";
            wordsFileName = "Words.txt";
            outputFileName = "Output.txt";
            articlePath = $"{currentInputDirectory}\\{articleFileName}";
            wordsPath = $"{currentInputDirectory}\\{wordsFileName}";
            outputPath = $"{currentOutputDirectory}\\{outputFileName}";

            // Clean paragraph and extract
            replacements = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("mr.", "mr"),
                new KeyValuePair<string, string>("mrs.", "mrs"),
                new KeyValuePair<string, string>("e.g.", "eg"),
            };
        }

        [TestMethod]
        public void CanGetArticleContent()
        {
            var cleanArticleContent = InputHelper.ReadInputContent(articlePath, replacements).GetAwaiter().GetResult();

            Assert.IsNotNull(cleanArticleContent);
            Assert.IsTrue(cleanArticleContent.Length > 0);
        }

        [TestMethod]
        public void CanGetWords()
        {
            var cleanWords = InputHelper.ReadInputLines(wordsPath, replacements).GetAwaiter().GetResult();

            Assert.IsNotNull(cleanWords);
            Assert.IsTrue(cleanWords.Any());
        }

        [TestMethod]
        public void CanGetParagraphListInfo()
        {
            var cleanArticleContent = InputHelper.ReadInputContent(articlePath, replacements).GetAwaiter().GetResult();
            var paragraphs = StringHelper.ExtractParagraphs(cleanArticleContent);

            var pList = InputHelper.ExtractParagraphList(paragraphs);

            Assert.IsNotNull(paragraphs);
            Assert.IsTrue(paragraphs.Any());

            Assert.IsNotNull(pList);
            Assert.IsTrue(pList.Any());
        }

        [TestMethod]
        public void CanGenerateCorrectOutput()
        {
            // Assign
            var cleanWords = InputHelper.ReadInputLines(wordsPath, replacements).GetAwaiter().GetResult();

            var cleanArticleContent = InputHelper.ReadInputContent(articlePath, replacements).GetAwaiter().GetResult();
            var paragraphs = StringHelper.ExtractParagraphs(cleanArticleContent);

            var pList = InputHelper.ExtractParagraphList(paragraphs);

            var outputText = InputHelper.ReadInputLines(outputPath).GetAwaiter().GetResult().Select(r => r.Trim()).ToList();

            // Act
            var output = OutputHelper.GenerateOutput(cleanWords, pList, replacements);

            var finalOutputList = OutputHelper.ExtractFinalOutput(output);

            // Assert
            Assert.IsNotNull(outputText);
            Assert.IsTrue(outputText.Any());

            Assert.IsNotNull(finalOutputList);
            Assert.IsTrue(finalOutputList.Any());

            Assert.IsTrue(outputText.Count() == finalOutputList.Count());
            Assert.IsTrue(finalOutputList.SequenceEqual(outputText));
        }
    }
}
