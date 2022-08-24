using BuilderWire.Models;
using BuilderWire.Models.Input;
using BuilderWire.Models.Output;
using BuilderWire.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BuilderWire.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Declare Global variables...
            var currentInputDirectory = "Input";
            var currentOutputDirectory = "Output";
            var articleFileName = "Article.txt";
            var wordsFileName = "Words.txt";
            var outputFileName = "Output.txt";
            var articlePath = $"{currentInputDirectory}\\{articleFileName}";
            var wordsPath = $"{currentInputDirectory}\\{wordsFileName}";
            var outputPath = $"{currentOutputDirectory}\\{outputFileName}";

            // Clean paragraph and extract
            var replacements = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("mr.", "mr"),
                new KeyValuePair<string, string>("mrs.", "mrs"),
                new KeyValuePair<string, string>("e.g.", "eg"),
            };

            // Validate if all files are existing.
            if (!ValidationHelper.IsValidFile(articlePath) || !ValidationHelper.IsValidFile(wordsPath) || !ValidationHelper.IsValidFile(outputPath))
                throw new FileNotFoundException("Invalid Input Files!");

            var cleanArticleContent = InputHelper.ReadInputContent(articlePath, replacements).GetAwaiter().GetResult();
            var cleanWords = InputHelper.ReadInputLines(wordsPath, replacements).GetAwaiter().GetResult();

            var paragraphs = StringHelper.ExtractParagraphs(cleanArticleContent);

            var pList = InputHelper.ExtractParagraphList(paragraphs);

            var output = OutputHelper.GenerateOutput(cleanWords, pList, replacements);

            //// Word replacement could be better...
            var finalOutputList = OutputHelper.ExtractFinalOutput(output);

            var outputText = InputHelper.ReadInputLines(outputPath).GetAwaiter().GetResult().Select(r => r.Trim()).ToList();

            // Validate Output
            if (finalOutputList.Count != outputText.Count)
                throw new Exception("Invalid output!");

            var valid = true;

            //System.Console.WriteLine("My Output \t->\t Text Output \t->\t Is Equals?");
            StringHelper.PrintLine();
            StringHelper.PrintRow("Original Output", "Generated Output", "Is Equals?");
            StringHelper.PrintLine();

            for (int i = 0; i < outputText.Count; i++)
            {
                var needChecked = finalOutputList[i] == outputText[i] ? "" : "Needs Validation!";
                if (string.IsNullOrEmpty(needChecked))
                    System.Console.ForegroundColor = ConsoleColor.White;
                else
                    System.Console.ForegroundColor = ConsoleColor.Red;

                StringHelper.PrintRow(outputText[i], finalOutputList[i], (finalOutputList[i] == outputText[i]).ToString());

                if (finalOutputList[i] != outputText[i] && valid)
                    valid = false;
            }

            if (!valid)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                throw new Exception("Invalid output!");
            }
            else
            {
                var newOutputPath = $"{currentOutputDirectory}\\Output-{DateTime.Now:MMddyyyyhhmmss}.txt";
                if (File.Exists(newOutputPath))
                    File.Delete(newOutputPath);
                else
                    File.WriteAllLines(newOutputPath, outputText);

                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine("\r\r");
                System.Console.WriteLine("--------------------------");
                System.Console.WriteLine("SUCCESSFULLY VALIDATED OUTPUT!");
                System.Console.WriteLine("--------------------------");
            }
        }
    }
}
