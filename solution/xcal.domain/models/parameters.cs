using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.infrastructure.contracts;
using reexjungle.xcal.infrastructure.extensions;
using reexjungle.xcal.infrastructure.serialization;
using reexjungle.xmisc.foundation.concretes;

namespace reexjungle.xcal.domain.models
{
    /// <summary>
    /// Time Zone ID class
    /// </summary>
    [DataContract]
    public class TZID : ITZID, IEquatable<TZID>, IComparable<TZID>, ICalendarSerializable
    {
        private string prefix;
        private bool globallyUnique;

        /// <summary>
        /// Gets or sets the identifier for the time zone definition for a time component
        /// </summary>
        [DataMember]
        public string Prefix
        {
            get { return prefix; }
            set
            {
                prefix = value;
                globallyUnique = string.IsNullOrWhiteSpace(prefix);
            }
        }

        /// <summary>
        /// Gets or sets the identifier for the time zone definition for a time component
        /// </summary>
        [DataMember]
        public string Suffix { get; set; }

        [DataMember]
        public bool GloballyUnique
        {
            get { return globallyUnique; }
            set
            {
                globallyUnique = value;
                if (globallyUnique) prefix = null;
            }
        }

        public TZID()
        {
            Prefix = string.Empty;
            Suffix = "GMT";
        }

        public TZID(ITZID other)
        {
            if (other != null)
            {
                Prefix = other.Prefix;
                Suffix = other.Suffix;
            }
        }

        public TZID(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException("value");

            var pattern = @"^TZID=(?<prefix>\w+)?/(?<suffix>\w+)$";

            var options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
              RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(value, pattern, options)) throw new FormatException("value");

            GloballyUnique = true;
            foreach (Match match in regex.Matches(value))
            {
                if (match.Groups["prefix"].Success)
                {
                    GloballyUnique = false;
                    Prefix = match.Groups["prefix"].Value;
                }
                Suffix = match.Groups["suffix"].Value;
            }
        }

        public TZID(string prefix, string suffix)
        {
            if (string.IsNullOrEmpty(suffix))
                throw new ArgumentException("suffix");

            Prefix = prefix;
            Suffix = suffix;
        }

        public bool Equals(TZID other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Suffix, other.Suffix) && GloballyUnique == other.GloballyUnique && string.Equals(Prefix, other.Prefix);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TZID)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Suffix != null ? Suffix.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ GloballyUnique.GetHashCode();
                hashCode = (hashCode * 397) ^ (Prefix != null ? Prefix.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(TZID left, TZID right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TZID left, TZID right)
        {
            return !Equals(left, right);
        }

        public int CompareTo(TZID other)
        {
            return string.Compare(Suffix, other.Suffix, StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator <(TZID a, TZID b)
        {
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(TZID a, TZID b)
        {
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
            return a.CompareTo(b) > 0;
        }

        public void WriteCalendar(CalendarWriter writer)
        {
            if (string.IsNullOrEmpty(Suffix) || string.IsNullOrWhiteSpace(Suffix)) return;

            writer.WriteParameter("TZID", GloballyUnique ? $"/{Suffix}" : $"{Prefix}/{Suffix}");
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    public class FMTTYPE : IFMTTYPE, IEquatable<FMTTYPE>, ICalendarSerializable
    {
        [DataMember]
        public string TypeName { get; set; }

        [DataMember]
        public string SubTypeName { get; set; }

        public FMTTYPE()
        {
            TypeName = null;
            SubTypeName = null;
        }

        public FMTTYPE(string name, string subname)
        {
            TypeName = name;
            SubTypeName = subname;
        }

        public FMTTYPE(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("value");

            var pattern = @"^FMTTYPE=(?<type>\w+)/(?<subtype>\w+\S*)$";
            var options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
              RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(value, pattern, options)) throw new FormatException("value");

            foreach (Match match in regex.Matches(value))
            {
                if (match.Groups["type"].Success) TypeName = match.Groups["type"].Value;
                if (match.Groups["subtype"].Success) SubTypeName = match.Groups["subtype"].Value;
            }
        }

        public FMTTYPE(IFMTTYPE fmttype)
        {
            if (fmttype != null)
            {
                if (!string.IsNullOrEmpty(fmttype.TypeName))
                    TypeName = string.Copy(fmttype.TypeName);

                if (!string.IsNullOrEmpty(fmttype.SubTypeName))
                    SubTypeName = string.Copy(fmttype.SubTypeName);
            }
        }

        public bool Equals(FMTTYPE other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(TypeName, other.TypeName) && string.Equals(SubTypeName, other.SubTypeName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((FMTTYPE)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((TypeName != null ? TypeName.GetHashCode() : 0) * 397) ^ (SubTypeName != null ? SubTypeName.GetHashCode() : 0);
            }
        }

        public static bool operator ==(FMTTYPE left, FMTTYPE right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FMTTYPE left, FMTTYPE right)
        {
            return !Equals(left, right);
        }

        public void WriteCalendar(CalendarWriter writer)
        {
            if (string.IsNullOrEmpty(TypeName) || string.IsNullOrWhiteSpace(TypeName)) return;
            writer.WriteParameter("FMTTYPE", $"{TypeName}/{SubTypeName}");
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    public class LANGUAGE : ILANGUAGE, IEquatable<LANGUAGE>, ICalendarSerializable
    {
        [DataMember]
        public string Tag { get; set; }

        [DataMember]
        public string SubTag { get; set; }

        public LANGUAGE()
        {
            Tag = null;
            SubTag = null;
        }

        public LANGUAGE(string tag, string subtag)
        {
            if (string.IsNullOrEmpty(tag)) throw new ArgumentException("tag");
            Tag = tag;
            SubTag = subtag;
        }

        public LANGUAGE(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("value");
            var pattern = @"^LANGUAGE=(?<tag>\w{2})(-(?<subtag>\w{2}))?$";
            if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                throw new FormatException("Invalid Language Format type");
            foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
            {
                if (match.Groups["tag"].Success)
                    Tag = match.Groups["tag"].Value;
                if (match.Groups["subtag"].Success)
                    SubTag = match.Groups["subtag"].Value;
            }
        }

        public LANGUAGE(ILANGUAGE language)
        {
            if (language != null)
            {
                Tag = language.Tag;
                SubTag = language.SubTag;
            }
        }

        public bool Equals(LANGUAGE other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Tag, other.Tag) && string.Equals(SubTag, other.SubTag);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((LANGUAGE)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Tag != null ? Tag.GetHashCode() : 0) * 397) ^ (SubTag != null ? SubTag.GetHashCode() : 0);
            }
        }

        public static bool operator ==(LANGUAGE left, LANGUAGE right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LANGUAGE left, LANGUAGE right)
        {
            return !Equals(left, right);
        }

        public void WriteCalendar(CalendarWriter writer)
        {
            if (string.IsNullOrEmpty(Tag) || string.IsNullOrWhiteSpace(Tag)) return;

            writer.WriteParameter("LANGUAGE", !string.IsNullOrEmpty(SubTag) && !string.IsNullOrWhiteSpace(SubTag)
                ? $"{Tag}-{SubTag}"
                : $"{Tag}");
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    public abstract class DELEGATE : IDELEGATE, IEquatable<DELEGATE>, ICalendarSerializable
    {
        /// <summary>
        /// Gets or sets the calendar addresses of the participation delegators
        /// </summary>
        [DataMember]
        public List<CAL_ADDRESS> Addresses { get; set; }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        protected DELEGATE()
        {
            Addresses = new List<CAL_ADDRESS>();
        }

        /// <summary>
        /// Overloaded Constructor
        /// </summary>
        /// <param name="addresses">
        /// The enumerable collection of calendar addresses, which represent the delegators
        /// </param>
        protected DELEGATE(IEnumerable<CAL_ADDRESS> addresses)
        {
            Addresses = addresses.NullOrEmpty()
                ? new List<CAL_ADDRESS>()
                : new List<CAL_ADDRESS>(addresses);
        }

        protected DELEGATE(IDELEGATE @delegate)
        {
            if (@delegate != null)
            {
                Addresses = @delegate.Addresses.NullOrEmpty()
            ? new List<CAL_ADDRESS>()
            : new List<CAL_ADDRESS>(@delegate.Addresses);
            }
        }

        public bool Equals(DELEGATE other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Addresses.IsEquivalentOf(other.Addresses);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DELEGATE)obj);
        }

        public override int GetHashCode()
        {
            return (Addresses != null ? Addresses.GetHashCode() : 0);
        }

        public static bool operator ==(DELEGATE left, DELEGATE right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DELEGATE left, DELEGATE right)
        {
            return !Equals(left, right);
        }

        public abstract void WriteCalendar(CalendarWriter writer);

        public abstract void ReadCalendar(CalendarReader reader);
    }

    [DataContract]
    public class DELEGATED_FROM : DELEGATE
    {
        public DELEGATED_FROM()
        {
        }

        public DELEGATED_FROM(string value)
        {
            Addresses = new List<CAL_ADDRESS>();
            var pattern = @"DELEGATED-FROM=mailto:(?<uri>\s*(\w+:\S+)\s*)";
            var options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                          RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(value, pattern, options)) throw new FormatException("Invalid Delegate format");

            foreach (Match match in regex.Matches(value))
            {
                if (match.Groups["uri"].Success)
                {
                    Addresses.Add(new CAL_ADDRESS(match.Groups["value"].Value));
                }
            }
        }

        public DELEGATED_FROM(List<CAL_ADDRESS> addresses) : base(addresses)
        {
        }

        public DELEGATED_FROM(IDELEGATE @delegate) : base(@delegate)
        {
        }

        public override void WriteCalendar(CalendarWriter writer)
        {
            if (Addresses.NullOrEmpty()) return;
            writer.WriteParameterWithDQuotedValues("DELEGATED-FROM", Addresses.ToArray());
        }

        public override void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    public class DELEGATED_TO : DELEGATE
    {
        public DELEGATED_TO()
        {
        }

        public DELEGATED_TO(List<CAL_ADDRESS> addresses) : base(addresses)
        {
        }

        public DELEGATED_TO(string value)
        {
            Addresses = new List<CAL_ADDRESS>();
            var pattern = @"DELEGATED-TO=mailto:(?<uri>\s*(\w+:\S+)\s*)";
            var options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                          RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(value, pattern, options)) throw new FormatException("Invalid Delegate format");

            foreach (Match match in regex.Matches(value))
            {
                if (match.Groups["uri"].Success)
                {
                    Addresses.Add(new CAL_ADDRESS(match.Groups["value"].Value));
                }
            }
        }

        public DELEGATED_TO(IDELEGATE @delegate) : base(@delegate)
        {
        }

        public override void WriteCalendar(CalendarWriter writer)
        {
            if (Addresses.NullOrEmpty()) return;
            writer.WriteParameterWithDQuotedValues("DELEGATED-TO", Addresses.ToArray());

        }

        public override void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Group or List Membership. Provides the group or list membership of the calendar user
    /// specified by a property
    /// </summary>
    [DataContract]
    public class MEMBER : IMEMBER, IEquatable<MEMBER>, ICalendarSerializable
    {
        /// <summary>
        /// Gets or sets the calendar addresses of the group members
        /// </summary>
        [DataMember]
        public List<CAL_ADDRESS> Addresses { get; set; }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public MEMBER()
        {
            Addresses = new List<CAL_ADDRESS>();
        }

        /// <summary>
        /// Overloaded Constructor.
        /// </summary>
        /// <param name="addresses">
        /// The enumerable collection of addresses representing users in the membership list
        /// </param>
        public MEMBER(IEnumerable<CAL_ADDRESS> addresses)
        {
            Addresses = addresses.NullOrEmpty()
                ? new List<CAL_ADDRESS>()
                : new List<CAL_ADDRESS>(addresses);
        }

        public MEMBER(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var trimmed = value.Replace(" ", string.Empty);
            var pattern = @"^MEMBER=(?<emails>(?:""mailto:(\w+\S+)""(,\s*)?)+)$";
            var options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(trimmed, pattern, options)) throw new FormatException("value");

            foreach (Match match in regex.Matches(trimmed))
            {
                if (match.Groups["emails"].Success)
                {
                    var emails = match.Groups["emails"].Value.Split(new[] { '\"', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    Addresses = emails.Select(e => new CAL_ADDRESS(e.Trim())).ToList();
                }
            }
        }

        public MEMBER(IMEMBER member)
        {
            if (member?.Addresses != null)
            {
                member.Addresses = member.Addresses;
            }
        }

        public bool Equals(MEMBER other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Addresses.IsEquivalentOf(other.Addresses);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MEMBER)obj);
        }

        public override int GetHashCode()
        {
            return (Addresses != null ? Addresses.GetHashCode() : 0);
        }

        public static bool operator ==(MEMBER left, MEMBER right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MEMBER left, MEMBER right)
        {
            return !Equals(left, right);
        }

        public void WriteCalendar(CalendarWriter writer)
        {
            if (Addresses.NullOrEmpty()) return;
            writer.WriteParameterWithDQuotedValues("MEMBER", Addresses.ToArray());

        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    public class ALTREP : IEquatable<ALTREP>, ICalendarSerializable
    {
        [DataMember]
        public Uri Uri { get; set; }

        public ALTREP()
        {
        }

        public ALTREP(ALTREP other)
        {
            if (other?.Uri != null && other.Uri.IsWellFormedOriginalString())
            {
                Uri = new Uri(other.Uri.ToString());
            }
        }

        public ALTREP(string uriString, UriKind kind)
        {
            if (uriString == null) throw new ArgumentNullException("uriString");
            Uri = new Uri(uriString, kind);
        }

        public ALTREP(Uri uri)
        {
            if (uri == null) throw new ArgumentNullException("uri");
            Uri = new Uri(uri.ToString());
        }

        public ALTREP(string value)
        {
            var pattern = @"^ALTREP=""(?<value>(\s*(\w+:\S+)\s*))""$";
            var options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                          RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(value, pattern, options)) throw new FormatException("value");
            foreach (Match match in regex.Matches(value))
            {
                if (match.Groups["value"].Success)
                {
                    Uri = new Uri(match.Groups["value"].Value, UriKind.RelativeOrAbsolute);
                }
            }
        }

        public bool Equals(ALTREP other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Uri, other.Uri);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ALTREP)obj);
        }

        public override int GetHashCode()
        {
            return (Uri != null ? Uri.GetHashCode() : 0);
        }

        public static bool operator ==(ALTREP left, ALTREP right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ALTREP left, ALTREP right)
        {
            return !Equals(left, right);
        }

        public void WriteCalendar(CalendarWriter writer)
        {
            if (Uri == null) return;
            writer.WriteParameter("ALTREP", writer.ConvertToQuotedString(Uri.ToString()));
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    public class DIR : IEquatable<DIR>, ICalendarSerializable
    {
        [DataMember]
        public Uri Uri { get; set; }

        public DIR(DIR other)
        {
            if (other?.Uri != null)
            {
                Uri = new Uri(other.Uri.ToString());
            }
        }

        public DIR()
        {
        }

        public DIR(Uri uri)
        {
            if (uri == null) throw new ArgumentNullException("uri");
            Uri = new Uri(uri.ToString());
        }

        public DIR(string uriString, UriKind kind)
        {
            if (uriString == null) throw new ArgumentNullException("uriString");
            Uri = new Uri(uriString, kind);
        }

        public DIR(string value)
        {
            var pattern = @"^DIR=""(?<value>(\s*(\w+:\S+)\s*))""$";
            var options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                          RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(value, pattern, options)) throw new FormatException("value");

            foreach (Match match in regex.Matches(value))
            {
                if (match.Groups["value"].Success)
                {
                    Uri = new Uri(match.Groups["value"].Value, UriKind.RelativeOrAbsolute);
                }
            }
        }

        public bool Equals(DIR other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Uri, other.Uri);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DIR)obj);
        }

        public override int GetHashCode()
        {
            return (Uri != null ? Uri.GetHashCode() : 0);
        }

        public static bool operator ==(DIR left, DIR right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DIR left, DIR right)
        {
            return !Equals(left, right);
        }

        public void WriteCalendar(CalendarWriter writer)
        {
            if (Uri == null) return;
            writer.WriteParameter("DIR", writer.ConvertToQuotedString(Uri.ToString()));
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    public class SENT_BY : ISENT_BY, IEquatable<SENT_BY>, ICalendarSerializable
    {
        [DataMember]
        public CAL_ADDRESS Address { get; set; }

        public SENT_BY()
        {
        }

        public SENT_BY(SENT_BY other)
        {
            if (other?.Address != null)
            {
                Address = new CAL_ADDRESS(other.Address);
            }
        }

        public SENT_BY(CAL_ADDRESS address)
        {
            Address = address;
        }

        public SENT_BY(string value)
        {
            var pattern = @"^SENT-BY=""(?<value>(\s*(\w+\S+)\s*))""$";
            var options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                          RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(value, pattern, options)) throw new FormatException("value");

            foreach (Match match in regex.Matches(value))
            {
                if (match.Groups["value"].Success)
                {
                    Address = new CAL_ADDRESS(match.Groups["value"].Value);
                }
            }
        }

        public bool Equals(SENT_BY other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Address, other.Address);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SENT_BY)obj);
        }

        public override int GetHashCode()
        {
            return (Address != null ? Address.GetHashCode() : 0);
        }

        public static bool operator ==(SENT_BY left, SENT_BY right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SENT_BY left, SENT_BY right)
        {
            return !Equals(left, right);
        }

        public void WriteCalendar(CalendarWriter writer)
        {
            if (Address == null) return;
            writer.WriteParameterWithDQuotedvalue("SENT-BY", Address);

        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
