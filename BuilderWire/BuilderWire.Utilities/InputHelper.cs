using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BuilderWire.Utilities
{
    public static class InputHelper
    {
        public static async Task<string> ReadInputContent(string filePath)
        {
            if(!File.Exists(filePath))
                throw new FileNotFoundException("File doesn't exist!");

            var content = await File.ReadAllTextAsync(filePath);

            return content;
        }

        public static async Task<List<string>> ReadInputLines(string filePath)
        {
            var content = await ReadInputContent(filePath);
            return content.Split('\n').Where(r => !string.IsNullOrEmpty(r)).Select(r => r).ToList();
        }
    }
}
