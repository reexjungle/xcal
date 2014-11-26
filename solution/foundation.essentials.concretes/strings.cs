using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace reexmonkey.foundation.essentials.concretes
{
    /// <summary>
    /// Extensions to the <see cref="DATE_TIME"/> type
    /// </summary>
    public static class CommonStringExtensions
    {
        /// <summary>
        /// Extracts hexadecimal digits from a string
        /// </summary>
        /// <param name="source">The hexadecimal string from which the digits are extracted</param>
        /// <returns>An array of extracted hexadecimal digits</returns>
        public static char[] ExtractHexDigits(this string source)
        {
            var regex = new Regex("[abcdefABECDEF\\d]+", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
            var extracted = from hex in source
                            where regex.IsMatch(hex.ToString())
                            select hex;

            return extracted.ToArray();
        }

        /// <summary>
        /// Replaces substrings in a string using a regular expresion pattern.
        /// </summary>
        /// <param name="value">The string, whose substrings are identified through pattern-recognition.</param>
        /// <param name="pattern">The regular expression pattern used in recognizing the substring in the string.</param>
        /// <param name="replacement">The string to replaces each found substring in the string.</param>
        /// <returns></returns>
        public static string RegexReplace(this string value, string pattern, string replacement)
        {
            return Regex.Replace(value, pattern, replacement);
        }

        /// <summary>
        /// Checks if a string matches the specified pattern.
        /// </summary>
        /// <param name="value">The string to be checked.</param>
        /// <param name="pattern">The regular expression pattern used in recognizing the substring in the string.</param>
        /// <param name="options">Regular expression options to be used in the check.</param>
        /// <returns></returns>
        public static bool Match(this string value, string pattern, RegexOptions options = RegexOptions.None)
        {
            return Regex.IsMatch(value, pattern, options);
        }

        /// <summary>
        /// Finds substrings in a string based on a pattern and appends a prefix to each found substring.
        /// </summary>
        /// <param name="value">The string to be searched</param>
        /// <param name="pattern">The regular expression pattern used in recognizing the substrings in the string.</param>
        /// <param name="prefix">The string to be added at the beginning of each found substring</param>
        /// <returns>The original string with </returns>
        public static string FindAndPrepend(this string value, string pattern, string prefix)
        {
            var sb = new StringBuilder(value);
            var matches = Regex.Matches(value, pattern);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (match.Value == string.Empty) continue;
                    sb.Replace(match.Value, string.Format("{0}{1}", prefix, match.Value));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Finds and substrings in a string based on a pattern and appends a suffix to each found substring.
        /// </summary>
        /// <param name="value">The string to be searched</param>
        /// <param name="pattern">The regular expression pattern used in recognizing the substrings in the string.</param>
        /// <param name="suffix">The string to be added at the beginning of each found substring</param>
        /// <returns></returns>
        public static string FindAndAppend(this string value, string pattern, string suffix)
        {
            var sb = new StringBuilder(value);
            var matches = Regex.Matches(value, pattern);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (match.Value == string.Empty) continue;
                    sb.Replace(match.Value, string.Format("{0}{1}", match.Value, suffix));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Delimits the lines of a string, when they are longer than the allowed maximum length.
        /// </summary>
        /// <param name="value">The string, whose lines are folded.</param>
        /// <param name="max">The maximum limit allowed for each line of the string</param>
        /// <param name="newline">The newline to delimit the line of the string.</param>
        /// <returns>The string whose lines are folded</returns>
        public static string FoldLines(this string value, int max, string newline = "\r\n")
        {
            var lines = value.Split(newline.ToSingleton(), System.StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                var len = line.Length;
                if (len <= max) sb.Append(line).Append(newline);
                else
                {
                    var chars = line.ToArray();
                    var blen = len / max;
                    var rlen = len % max;
                    var b = 0;
                    while (b < blen) sb.Append(chars, (b++) * max, max).AppendFormat("{0} ", newline);
                    if (rlen > 0) sb.Append(chars, blen * max, rlen).AppendFormat("{0}", newline);
                }
            }

            return sb.ToString();
        }

    }
}