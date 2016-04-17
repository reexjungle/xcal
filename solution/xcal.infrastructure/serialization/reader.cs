using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using reexjungle.xcal.infrastructure.contracts;
using reexjungle.xcal.infrastructure.extensions;
using Sprache;

namespace reexjungle.xcal.infrastructure.serialization
{
    /// <summary>
    /// Represents a reader that provides fast, noncached, forward-only access to iCalendar data.
    /// </summary>
    public class iCalReader : TextReader
    {
        static readonly Parser<char> HTabDelimiter = Parse.Char('\u0009');
        static readonly Parser<char> EmptyCharDelimiter = Parse.Char('\0');
        static readonly Parser<char> DQuoteDelimiter = Parse.Char('"');
        static readonly Parser<char> CommaDelimiter = Parse.Char(',');
        static readonly Parser<char> ColonDelimiter = Parse.Char(':');
        static readonly Parser<char> SemicolonDelimiter = Parse.Char(';');

        static readonly Parser<char> DQuotedStringDelimiter= Parse.AnyChar
            .Except(DQuoteDelimiter).Or(DQuoteDelimiter.CalEscaped());

        static readonly Parser<char> SafeStringDelimiter = Parse.AnyChar
            .Except(CommaDelimiter).Or(CommaDelimiter.CalEscaped())
            .Except(ColonDelimiter).Or(ColonDelimiter.CalEscaped())
            .Except(SemicolonDelimiter).Or(SemicolonDelimiter.CalEscaped());


        public CalendarFragmentType FragmentType { get; private set; }

        public iCalReader()
        {
            FragmentType = CalendarFragmentType.UNKNOWN;

        }

        public iCalReader(Stream stream)
        {
            FragmentType = CalendarFragmentType.UNKNOWN;

        }

        public iCalReader(TextReader reader)
        {
            
        }

        public iCalReader(string fragment, CalendarFragmentType fragmentType)
        {
            FragmentType = fragmentType;
        }

        public iCalReader(Stream fragment, CalendarFragmentType fragmentType)
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


        public static iCalReader Create(Stream stream)
        {
            return Create(new StreamReader(stream));
        }

        public static iCalReader Create(string @string)
        {
            return Create(new StringReader(@string));
        }

        public static iCalReader Create(TextReader reader)
        {
            return new iCalReader(reader);
        }

        public static iCalReader Create(iCalReader reader)
        {
            return new iCalReader(reader);
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


        public override int Peek()
        {
            return base.Peek();
        }


        public override int Read()
        {
            return base.Read();
        }
    }
}
