using System;
using System.Collections.Generic;
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
        /// <param name="newline">The newline characters to delimit the line of the string.</param>
        /// <returns>The string, whose lines are folded</returns>
        public static string FoldLines(this string value, int max, string newline = "\r\n")
        {
            var lines = value.Split(new string[]{newline}, System.StringSplitOptions.RemoveEmptyEntries);
            using (var ms = new System.IO.MemoryStream(value.Length))
            {
                var crlf = Encoding.UTF8.GetBytes(newline); //CRLF
                var crlfs = Encoding.UTF8.GetBytes(string.Format("{0} ", newline)); //CRLF and SPACE
                foreach (var line in lines)
                {
                    var bytes = Encoding.UTF8.GetBytes(line);
                    var len = bytes.Length;
                    if (len <= max)
                    {
                        ms.Write(bytes, 0, len);
                        ms.Write(crlf, 0, crlf.Length); 
                    }
                    else
                    {
                        var blen = len / max; //calculate block length
                        var rlen = len % max; //calculate remaining length
                        var b = 0;
                        while (b < blen)
                        {
                            ms.Write(bytes, (b++) * max, max);
                            ms.Write(crlfs, 0, crlfs.Length); 
                        }
                        if (rlen > 0)
                        {
                            ms.Write(bytes, blen * max, rlen);
                            ms.Write(crlf, 0, crlf.Length);
                        }
                    }
                }

                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// Unfolds the lines of a string, which have been delimited to a specified length.
        /// </summary>
        /// <param name="value">The string, whose lines are unfolded.</param>
        /// <param name="newline">The newline characters, which were used to delimit the string to lines.</param>
        /// <returns>The string, whose lines are unfolded.</returns>
        public static string UnfoldLines(this string value, string newline = "\r\n")
        {
            return value.Replace(string.Format("{0} ", newline), string.Empty);
        }

        /// <summary>
        /// Replaces the occurences of a first item of each tuple of strings in the current string instance with the second item of the tuple.
        /// </summary>
        /// <param name="value">The current string instance.</param>
        /// <param name="pairs">A enumerable collection of string tuples.</param>
        /// <returns>The string, in which specified substrings are replaced</returns>
        public static string Replace(this string value, IEnumerable<Tuple<string, string>> pairs)
        {
            try
            {
                foreach (var pair in pairs) value = value.Replace(pair.Item1, pair.Item2);
                return value;
            }
            catch (ArgumentNullException) {  throw; }
            catch (ArgumentException) {  throw; }
        }

        /// <summary>
        /// Escapes the strings.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string EscapeStrings(this string value)
        {
            return value.Replace(new List<Tuple<string, string>>
            {
                new Tuple<string, string>(@"\", "\\\\"),
                new Tuple<string, string>(";",  @"\;"),
                new Tuple<string, string>(",",  @"\,"),
                new Tuple<string, string>("\r\n",  @"\n"),
            });
        }
    }
}