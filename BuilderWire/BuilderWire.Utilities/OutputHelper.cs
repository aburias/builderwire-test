using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using BuilderWire.Models;
using BuilderWire.Models.Output;

namespace BuilderWire.Utilities
{
    public static class OutputHelper
    {
        public static List<string> ExtractFinalOutput(List<Output> output)
        {
            return output.Select(r => $"{r.LetterIndex}. {r.Word} {{{r.WordCount}:{r.ParagraphLocations.Aggregate((first, next) => $"{first},{next}")}}}").ToList();
        }

        public static List<Output> GenerateOutput(List<string> cleanWords, List<Paragraph> pList, List<KeyValuePair<string, string>> replacements)
        {
            // Generate and calculate output.
            var output = new List<Output>();
            var rowCount = 1;
            var letterCount = 1;
            foreach (var word in cleanWords)
            {
                if (rowCount >= 27)
                {
                    rowCount = 1;
                    letterCount++;
                }

                var instances = pList.Where(p =>
                    p.Text.Contains($" {word} ")
                    || p.Text.Contains($" {word},")
                    || p.Text.StartsWith($"{word} ")
                    || p.Text.EndsWith($" {word}")
                    );

                var replacedWord = replacements.FirstOrDefault(r => r.Value == word);
                var newOutput = new Output()
                {
                    LetterIndex = RowNameHelper.GetRowName(rowCount, letterCount).ToLower(),
                    Word = replacedWord.Key == null && replacedWord.Value == null ? word : replacedWord.Key,
                    ParagraphLocations = instances.Select(p => p.ParagraphNumber.ToString()).ToList()
                };

                foreach (var par in pList)
                {
                    var paragraphWords = par.Text.ToLower().Split(' ').ToList();
                    var totalInstances = paragraphWords.Count(p => p.ToLower().Trim() == word);
                    if (totalInstances > 1)
                    {
                        for (int i = 1; i < totalInstances; i++)
                        {
                            newOutput.ParagraphLocations.Add(par.ParagraphNumber.ToString());
                        }
                    }
                }

                newOutput.WordCount = newOutput.ParagraphLocations.Count;

                var sortedLocations = newOutput.ParagraphLocations.OrderBy(o => Convert.ToInt32(o)).ToList();
                newOutput.ParagraphLocations = sortedLocations;

                output.Add(newOutput);

                rowCount++;
            }

            return output;
        }

        public static void ValidateAndFinalizeResults(List<string> finalOutputList, List<string> outputText,
            string currentOutputDirectory)
        {
            // Validate Output
            if (finalOutputList.Count != outputText.Count)
                throw new Exception("Invalid output!");

            var valid = true;

            //System.Console.WriteLine("My Output \t->\t Text Output \t->\t Is Equals?");
            StringHelper.PrintLine();
            StringHelper.PrintRow("Original Output", "Generated Output", "Is Equals?");
            StringHelper.PrintLine();

            for (var i = 0; i < outputText.Count; i++)
            {
                var needChecked = finalOutputList[i] == outputText[i] ? "" : "Needs Validation!";

                System.Console.ForegroundColor = string.IsNullOrEmpty(needChecked) ? ConsoleColor.White : ConsoleColor.Red;

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
                // Please check where the output files are...
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

                System.Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("OPENING OUTPUT FILES...");

                Process.Start("notepad.exe", newOutputPath);
                Process.Start("notepad.exe", $"{currentOutputDirectory}\\Output.txt");
            }
        }
    }
}
