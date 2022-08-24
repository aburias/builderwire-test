using BuilderWire.Models.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuilderWire.Utilities
{
    public static class StringHelper
    {
        static int tableWidth = 110;

        public static List<string> ExtractParagraphs(string cleanArticle)
        {
            return cleanArticle.Replace(". ", "|").Split('|').Select(p => $"{p}").ToList();
        }

        public static string SanitizeString(string str, List<KeyValuePair<string, string>> rep)
        {
            str = str.ToLower();
            foreach (var toReplace in rep)
                str = str.Replace(toReplace.Key, toReplace.Value);

            return str;
        }

        public static void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth));
        }

        public static void PrintRow(params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }

        public static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
                //return text.PadRight(width - (width - text.Length) / 2);
            }
        }
    }
}
