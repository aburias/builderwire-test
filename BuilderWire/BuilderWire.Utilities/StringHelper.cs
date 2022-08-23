using BuilderWire.Models.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuilderWire.Utilities
{
    public static class StringHelper
    {
        public static List<string> ExtractFinalOutput(List<Output> output)
        {
            return output.Select(r => $"{r.LetterIndex}. {r.Word} {{{r.WordCount}:{r.ParagraphLocations.Aggregate((first, next) => $"{first},{next}")}}}").ToList();
        }

        public static List<string> ExtractParagraphs(string cleanArticle)
        {
            return cleanArticle.Replace(". ", "|").Split('|').Select(p => $"{p.TrimEnd('.')}").ToList();
        }

        public static string SanitizeString(string str, List<KeyValuePair<string, string>> rep)
        {
            str = str.ToLower();
            foreach (var toReplace in rep)
                str = str.Replace(toReplace.Key, toReplace.Value);

            return str;
        }
    }
}
