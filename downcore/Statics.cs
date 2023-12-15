using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace downcore
{
    public static class Statics
    {

        /// <summary>
        /// App text icon image.
        /// </summary>
        public static string appName = $@"
   / /__ / /_____/ /__ _    _____ 
  /  '_//  '_/ _  / _ \ |/|/ / _ \
 /_/\_\/_/\_\\_,_/\___/__,__/_//_/                                 
 ";

        public static string version = "version: 0.0.1-alpha";
        public static string repositry = "https://github.com/kuyoru-kamikisho/kkdown";

        /// <summary>
        /// Print text with color use writeline.
        /// </summary>
        public static void Log(string content, ConsoleColor? color = null)
        {
            if (color != null)
            {
                Console.ForegroundColor = (ConsoleColor)color;
            }
            Console.WriteLine(content);
            if (color != null)
            {
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Returns a string at a position before or after a specified string in the string array.
        /// </summary>
        /// <param name="array">A string array.</param>
        /// <param name="searchString">The string queried in the array, if there are multiple identical values in the array, takes the first one.</param>
        /// <param name="next">Positive values come after, negative values come before.</param>
        /// <returns>The string corresponding to the position.</returns>
        public static string? FindNextString(string[] array, string searchString, int next = 1)
        {
            int index = Array.IndexOf(array, searchString);
            if (index >= 0 && index < array.Length - next && index + next >= 0)
            {
                return array[index + next];
            }
            return null;
        }
        public static int ParseDataRate(string dataRate)
        {
            int rate = 0;
            int multiplier = 1;

            if (dataRate.EndsWith("kb/s"))
            {
                multiplier = 1;
            }
            else if (dataRate.EndsWith("mb/s"))
            {
                multiplier = 1024;
            }

            string numericPart = new string(dataRate.Where(char.IsDigit).ToArray());

            int.TryParse(numericPart, out rate);
            rate *= multiplier;

            return rate;
        }
    }
}
