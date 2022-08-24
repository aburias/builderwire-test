using System;
using System.Collections.Generic;
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
    }
}
