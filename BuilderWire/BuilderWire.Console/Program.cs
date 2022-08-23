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
                if(replacements.Any(r => r.Key.Trim().ToLower() == w.Text.Trim().ToLower()))
                    w.Text = replacements.FirstOrDefault().Value;

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
                if(rowCount >= 27)
                {
                    rowCount = 1;
                    letterCount++;
                }

                output.Add(new Output()
                {
                    LetterIndex = RowNameHelper.GetRowName(rowCount, letterCount).ToLower(),
                    Word = word.Text.Trim().ToLower(),
                    WordCount = word.WordCount,
                    ParagraphLocations = paragraphLlist.Where(p => 
                    p.Text.ToLower().Contains($" {word.Text.Trim().ToLower()} ") 
                    || p.Text.ToLower().Contains($" {word.Text.Trim().ToLower()},")
                    || p.Text.ToLower().StartsWith($"{word.Text.Trim().ToLower()} ") 
                    || p.Text.ToLower().EndsWith($" {word.Text.Trim().ToLower()}")
                    ).Select(p => p.PCount.ToString()).ToList()
                });
                rowCount++;
            }

            // Word replacement could be better...
            var finalOutputList = StringHelper.ExtractFinalOutput(output);
            var cleanOutput = new List<string>();

            foreach (var co in finalOutputList)
            {
                if(replacements.Any(r => co.Contains($" {r.Value} ")))
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
            if(cleanOutput.Count != outputText.Count)
                throw new Exception("Invalid output!");

            for (int i = 0; i < outputText.Count; i++)
            {
                if(cleanOutput[i] != outputText[i])
                    throw new Exception("Invalid output!");
            }
        }
    }
}
