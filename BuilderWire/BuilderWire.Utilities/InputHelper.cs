using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BuilderWire.Models;

namespace BuilderWire.Utilities
{
    public static class InputHelper
    {
        public static async Task<string> ReadInputContent(string filePath,
            List<KeyValuePair<string, string>> replacements = null)
        {
            if(!File.Exists(filePath))
                throw new FileNotFoundException("File doesn't exist!");

            var content = await File.ReadAllTextAsync(filePath);

            if (string.IsNullOrEmpty(content))
                throw new NullReferenceException("Content cannot be empty!");

            content = content.ToLower().Trim().Trim('.');

            if (replacements != null && replacements.Any())
            {
                foreach (var rep in replacements)
                {
                    content = content.Replace(rep.Key.ToLower().Trim(), rep.Value.ToLower().Trim());
                }
            }

            return content;
        }

        public static async Task<List<string>> ReadInputLines(string filePath,
            List<KeyValuePair<string, string>> replacements = null)
        {
            var lines = await File.ReadAllLinesAsync(filePath);

            var cleanLines = new List<string>();

            foreach (var line in lines.Select(l => l.ToLower().Trim()))
            {
                var oneWord = line;
                if (replacements != null && replacements.Any())
                {
                    foreach (var rep in replacements)
                    {
                        if (rep.Key.ToLower().Trim() == line)
                            oneWord = rep.Value.ToLower().Trim();
                    }
                }
                cleanLines.Add(oneWord);
            }

            return cleanLines;
        }

        public static List<Paragraph> ExtractParagraphList(List<string> paragraphs)
        {
            var paragraphCount = 1;
            var pList = new List<Paragraph>();
            foreach (var paragraph in paragraphs)
            {
                pList.Add(new Paragraph()
                {
                    Text = paragraph,
                    ParagraphNumber = paragraphCount
                });
                paragraphCount++;
            }

            return pList;
        }
    }
}
