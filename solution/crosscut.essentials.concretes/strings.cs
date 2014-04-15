using System.Text;
using System.Text.RegularExpressions;

namespace reexmonkey.foundation.essentials.concretes
{
    public static class CommonStringExtensions
    {
        public static string RegexReplace(this string value, string pattern, string replacement)
        {
            return Regex.Replace(value, pattern, replacement);
        }

        public static bool Match(this string value, string pattern)
        {
            return Regex.IsMatch(value, pattern);
        }

        public static bool Match(this string value, string pattern, RegexOptions options)
        {
            return Regex.IsMatch(value, pattern, options);
        }

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

    }

}
