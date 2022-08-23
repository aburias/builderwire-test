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
            var currentInputDirectory = "Input";
            var currentOutputDirectory = "Output";
            var articleFileName = "Article.txt";
            var wordsFileName = "Words.txt";
            var outputFileName = "Output.txt";
            var articlePath = $"{currentInputDirectory}\\{articleFileName}";
            var wordsPath = $"{currentInputDirectory}\\{wordsFileName}";
            var outputPath = $"{currentOutputDirectory}\\{outputFileName}";

            if (!ValidationHelper.IsValidFile(articlePath) || !ValidationHelper.IsValidFile(wordsPath) || !ValidationHelper.IsValidFile(outputPath))
                throw new FileNotFoundException("Invalid Input Files!");

            var article = InputHelper.ReadInputContent(articlePath).GetAwaiter().GetResult();
            var articleObj = new Article()
            {
                Content = article
            };
            var articleWords = articleObj.Content.Split(' ');

            // Clean paragraph and extract
            var replacements = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("mr.", "mr"),
                new KeyValuePair<string, string>("mrs.", "mrs"),
                new KeyValuePair<string, string>("e.g.", "eg"),
            };

            var words = InputHelper.ReadInputLines(wordsPath).GetAwaiter().GetResult();
            var wordsListUnclean = words.Select(w => new Word() { Text = w, WordCount = articleWords.Count(wr => wr.ToLower().Trim() == w.ToLower().Trim()) }).ToList();

            var wordsList = new List<Word>();
            foreach (var w in wordsListUnclean)
            {
                if (replacements.Any(r => r.Key.Trim().ToLower() == w.Text.Trim().ToLower()))
                    w.Text = replacements.FirstOrDefault(r => r.Key.Trim().ToLower() == w.Text.Trim().ToLower()).Value;

                wordsList.Add(w);
            }

            //wordsList.ForEach(w => replacements.Any(r => r.Key.Trim().ToLower() == w.Text.Trim().ToLower() ? w.Text = replacements.FirstOrDefault().Value : w.Text = w.Text));
            var cleanArticle = StringHelper.SanitizeString(article, replacements);

            var paragraphs = StringHelper.ExtractParagraphs(cleanArticle);
            var paragraphCount = 1;
            var paragraphLlist = new List<Paragraph>();
            foreach (var paragraph in paragraphs)
            {
                paragraphLlist.Add(new Paragraph()
                {
                    Text = paragraph,
                    PCount = paragraphCount
                });
                paragraphCount++;
            }

            var output = new List<Output>();
            var rowCount = 1;
            var letterCount = 1;
            foreach (var word in wordsList)
            {
                if (rowCount >= 27)
                {
                    rowCount = 1;
                    letterCount++;
                }

                var instances = paragraphLlist.Where(p =>
                    p.Text.ToLower().Contains($" {word.Text.Trim().ToLower()} ")
                    || p.Text.ToLower().Contains($" {word.Text.Trim().ToLower()},")
                    || p.Text.ToLower().StartsWith($"{word.Text.Trim().ToLower()} ")
                    || p.Text.ToLower().EndsWith($" {word.Text.Trim().ToLower()}")
                    );

                var newOutput = new Output()
                {
                    LetterIndex = RowNameHelper.GetRowName(rowCount, letterCount).ToLower(),
                    Word = word.Text.Trim().ToLower(),
                    ParagraphLocations = instances.Select(p => p.PCount.ToString()).ToList()
                };

                foreach (var par in paragraphLlist)
                {
                    var paragraphWords = par.Text.ToLower().Split(' ').ToList();
                    var totalInstances = paragraphWords.Count(p => p.ToLower().Trim() == word.Text.Trim().ToLower());
                    if(totalInstances > 1)
                    {
                        for (int i = 1; i < totalInstances; i++)
                        {
                            newOutput.ParagraphLocations.Add(par.PCount.ToString());
                        }
                    }
                }

                newOutput.WordCount = newOutput.ParagraphLocations.Count;

                var sortedLocations = newOutput.ParagraphLocations.OrderBy(o => Convert.ToInt32(o)).ToList();
                newOutput.ParagraphLocations = sortedLocations;

                output.Add(newOutput);

                rowCount++;
            }

            // Word replacement could be better...
            var finalOutputList = StringHelper.ExtractFinalOutput(output);
            var cleanOutput = new List<string>();

            foreach (var co in finalOutputList)
            {
                if (replacements.Any(r => co.Contains($" {r.Value} ")))
                {
                    var rep = replacements.FirstOrDefault(r => co.Contains($" {r.Value} "));
                    var newCo = co.Replace(rep.Value, rep.Key);
                    cleanOutput.Add(newCo);
                }
                else
                    cleanOutput.Add(co);

            }

            var outputText = InputHelper.ReadInputLines(outputPath).GetAwaiter().GetResult().Select(r => r.Trim()).ToList();

            // Validate Output
            if (cleanOutput.Count != outputText.Count)
                throw new Exception("Invalid output!");

            var valid = true;

            System.Console.WriteLine("My Output \t->\t Text Output \t->\t Is Equals?");

            for (int i = 0; i < outputText.Count; i++)
            {
                var needChecked = cleanOutput[i] == outputText[i] ? "" : "Needs Validation!";
                if(string.IsNullOrEmpty(needChecked))
                    System.Console.ForegroundColor = ConsoleColor.White;
                else
                    System.Console.ForegroundColor = ConsoleColor.Red;

                System.Console.WriteLine($"{cleanOutput[i]} \t->\t {outputText[i]} \t->\t {cleanOutput[i] == outputText[i]}");

                if (cleanOutput[i] != outputText[i] && valid)
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
                if(File.Exists(newOutputPath))
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
