using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceConsole.Helpers
{
    public static class StringExtensions
    {
        public static int ToInt(this string text)
        {
            int result;
            if (int.TryParse(text, out result))
            {
                result = int.Parse(text);
            }
            else
            {
                result = 0;
            }
            return result;
        }
        public static bool ToBool(this string text)
        {
            bool result;
            if (bool.TryParse(text, out result))
            {
                result = bool.Parse(text);
            }
            else
            {
                result = false;
            }
            return result;
        }
        public static string CenterText(this string text, int length)
        {
            if (text.Length >= length)
            {
                // The text is already at least as long as the target length, so return it as-is
                return text;
            }
            else
            {
                // Calculate the number of spaces to add on each side of the text
                int spaces = length - text.Length;
                int leftSpaces = spaces / 2;
                int rightSpaces = spaces - leftSpaces;

                // Build the centered text string with brackets
                //string centeredText = new string(' ', leftSpaces) + text + new string(' ', rightSpaces);
                string centeredText = text;

                return centeredText;
            }
        }
    }
}
