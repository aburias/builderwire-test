using System;
using System.Collections.Generic;
using System.Text;

namespace BuilderWire.Models.Output
{
    public class Output
    {
        public string LetterIndex { get; set; }
        public string Word { get; set; }
        public int WordCount { get; set; }
        public List<string> ParagraphLocations { get; set; }
    }
}
