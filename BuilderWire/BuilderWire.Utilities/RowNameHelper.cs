using System;
using System.Collections.Generic;
using System.Text;

namespace BuilderWire.Utilities
{
    public static class RowNameHelper
    {
        public static string GetRowName(int rowNum, int letterCount)
        {
            string rowName = Convert.ToChar(64 + rowNum).ToString();
            var result = rowName;
            for (int i = 0; i < letterCount-1; i++)
            {
                result += rowName;
            }
            return result;
        }
    }
}
