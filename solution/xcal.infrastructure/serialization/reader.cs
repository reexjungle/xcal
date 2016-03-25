using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using reexjungle.xcal.infrastructure.contracts;

namespace reexjungle.xcal.infrastructure.serialization
{
    /// <summary>
    /// Represents a reader that provides fast, noncached, forward-only access to iCalendar data.
    /// </summary>
    public class CalendarReader : TextReader
    {
        private FragmentData[] fragments;

        private FragmentData current;

        private int index = 0;

        public CalendarFragmentType FragmentType { get; private set; }

        public CalendarReader()
        {
            FragmentType = CalendarFragmentType.UNKNOWN;

        }

        public CalendarReader(Stream stream)
        {
            FragmentType = CalendarFragmentType.UNKNOWN;

        }

        public CalendarReader(TextReader reader)
        {
            
        }

        public CalendarReader(string fragment, CalendarFragmentType fragmentType)
        {
            FragmentType = fragmentType;
        }

        public CalendarReader(Stream fragment, CalendarFragmentType fragmentType)
        {
            FragmentType = fragmentType;
            if (fragment != null)
            {
                using (var sr = new StreamReader(fragment))
                {
                    //this.fragment = sr.ReadToEnd();
                }
            }
        }


        public static CalendarReader Create(Stream stream)
        {
            return Create(new StreamReader(stream));
        }

        public static CalendarReader Create(string @string)
        {
            return Create(new StringReader(@string));
        }

        public static CalendarReader Create(TextReader reader)
        {
            return new CalendarReader(reader);
        }

        public static CalendarReader Create(CalendarReader reader)
        {
            return new CalendarReader(reader);
        }

        protected virtual CalendarFragmentType MoveToValue(CalendarFragmentType type)
        {
            //TODO: 
            return CalendarFragmentType.VALUE;

        }

        protected virtual CalendarFragmentType MoveToParameter(CalendarFragmentType type)
        {
            //TODO: 
            return CalendarFragmentType.PARAMETER;

        }

        protected virtual CalendarFragmentType MoveToProperty(CalendarFragmentType type)
        {
            //TODO: 
            return CalendarFragmentType.PROPERTY;
        }

        protected virtual CalendarFragmentType MoveToComponent(CalendarFragmentType type)
        {
            //TODO: 
            return CalendarFragmentType.COMPONENT;
        }

        public virtual CalendarFragmentType MoveToContent()
        {
            if (FragmentType == CalendarFragmentType.VALUE)
            {
                FragmentType = MoveToParameter(FragmentType);
            }
            if (FragmentType == CalendarFragmentType.PARAMETER)
            {
                FragmentType = MoveToProperty(FragmentType);
            }

            if (FragmentType == CalendarFragmentType.PROPERTY)
            {
                FragmentType = MoveToComponent(FragmentType);
            }
            return FragmentType;
        }


        public static bool IsValidIANAToken(string token)
        {
            var pattern = @"^(\w-?)+$";
            var options = RegexOptions.IgnoreCase
                          | RegexOptions.ExplicitCapture
                          | RegexOptions.IgnorePatternWhitespace
                          | RegexOptions.CultureInvariant
                          | RegexOptions.Compiled;

            return new Regex(pattern, options).IsMatch(token);
        }

        public static bool IsValidXName(string xname)
        {
            var pattern = @"^X-(\w{3}-)*(\w-?)+$";
            var options = RegexOptions.IgnoreCase
                          | RegexOptions.ExplicitCapture
                          | RegexOptions.IgnorePatternWhitespace
                          | RegexOptions.CultureInvariant
                          | RegexOptions.Compiled;

            return new Regex(pattern, options).IsMatch(xname);
        }

        public static bool IsValidName(string name)
        {
            return IsValidIANAToken(name) || IsValidXName(name);
        }

        public static bool IsValidVendorId(string id)
        {
            return !string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id) && id.Length == 3;
        }

        public static bool IsValidQuotedString(string value)
        {
            var pattern = @"^""((\w-*(\\"")*)+)""$";
            var options = RegexOptions.IgnoreCase
                          | RegexOptions.ExplicitCapture
                          | RegexOptions.IgnorePatternWhitespace
                          | RegexOptions.CultureInvariant
                          | RegexOptions.Compiled;

            return new Regex(pattern, options).IsMatch(value);
        }

        public static bool IsValidQSAFE_STRING(string value)
        {
            var pattern = @"^(\w-*(\\"")*)+$";
            var options = RegexOptions.IgnoreCase
                          | RegexOptions.ExplicitCapture
                          | RegexOptions.IgnorePatternWhitespace
                          | RegexOptions.CultureInvariant
                          | RegexOptions.Compiled;

            return new Regex(pattern, options).IsMatch(value);
        }

        public static bool IsValidSAFE_STRING(string value)
        {
            var pattern = @"^(\w*-*(\\,)*(\\;)*(\\:)*)+$";
            var options = RegexOptions.IgnoreCase
                          | RegexOptions.ExplicitCapture
                          | RegexOptions.IgnorePatternWhitespace
                          | RegexOptions.CultureInvariant
                          | RegexOptions.Compiled;

            return new Regex(pattern, options).IsMatch(value);            
        }

        public static bool IsValidVALUE_STRING(string value)
        {
            return value != null;         
        }

        public static bool IsValidParameterValue(string value)
        {
            return IsValidSAFE_STRING(value) || IsValidQuotedString(value);
        }

        public static bool AreValidParameterValues(params string[] values)
        {
            return IsValidParameterValue(string.Join(",", values));
        }

        public static bool IsValidParameter(string name, params string[] values)
        {
            return IsValidName(name) && AreValidParameterValues(values);
        }

        public static bool IsValidParameter(string value)
        {
            var pattern = @"^((?<iana>(\w-?)+)|(?<xname>X-(\w{3}-)*(\w-?)+))=((?<safe>(\w(\\"")*-*)+)|(?<quoted>""((\w(\\"")*-*)+)""))$";
            var options = RegexOptions.IgnoreCase
                          | RegexOptions.ExplicitCapture
                          | RegexOptions.IgnorePatternWhitespace
                          | RegexOptions.CultureInvariant
                          | RegexOptions.Compiled;

            return new Regex(pattern, options).IsMatch(value);
        }

        public static bool IsValidProperty(string name, string value, params string[] parameters)
        {
            return IsValidName(name) && IsValidParameter(string.Join(";", parameters)) && IsValidVALUE_STRING(value);
        }


        private class FragmentData
        {
            private string value;
            private char[] buffer;
            private int startpos;
            private int length;

            private static volatile FragmentData s_None;

            internal static FragmentData None => s_None ?? new FragmentData();

            internal string StringValue => value ?? new string(buffer, startpos, length);

            internal CalendarFragmentType FragmentType { get; }


        }

    }
}
