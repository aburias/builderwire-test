using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BuilderWire.Utilities
{
    public static class ValidationHelper
    {
        public static bool IsValidFile(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
