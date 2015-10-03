using reexjungle.xcal.domain.contracts;
using reexjungle.xmisc.foundation.concretes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace reexjungle.xcal.domain.models
{
    /// <summary>
    /// Time Zone ID class
    /// </summary>
    [DataContract]
    public class TZID : ITZID, IEquatable<TZID>, IComparable<TZID>
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
            if (other == null) throw new ArgumentNullException("other");

            Prefix = other.Prefix;
            Suffix = other.Suffix;
        }

        public TZID(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException("value");

            var tokens = value.Split('/');
            switch (tokens.Length)
            {
                case 2:
                    Prefix = tokens[0];

                    if (string.IsNullOrEmpty(tokens[1]))
                        throw new ArgumentException("Suffix of Time Zone ID must neither be null nor empty!");

                    Suffix = tokens[1];
                    break;

                case 1:
                    GloballyUnique = false;

                    if (string.IsNullOrEmpty(tokens[0]))
                        throw new ArgumentException("Suffix of Time Zone ID must neither be null nor empty!");

                    Suffix = tokens[0];
                    break;
            }
        }

        public TZID(string prefix, string suffix)
        {
            if (string.IsNullOrEmpty(suffix))
                throw new ArgumentException("suffix");

            Prefix = prefix;
            Suffix = suffix;
        }

        public override string ToString()
        {
            if (GloballyUnique) return string.Format("TZID=/{0}", Suffix);
            return string.Format("TZID={0}/{1}", Prefix, Suffix);
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
    }

    [DataContract]
    public class FMTTYPE : IFMTTYPE, IEquatable<FMTTYPE>
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
            
            var pattern = @"^(?<type>\w+)/(?<subtype>\w+)$";
            
            if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture |RegexOptions.IgnoreCase))
                throw new FormatException("Invalid Format type");
            
            foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
            {
                if (match.Groups["type"].Success) TypeName = match.Groups["type"].Value;
                else if (match.Groups["subtype"].Success) SubTypeName = match.Groups["subtype"].Value;
            }
        }

        public FMTTYPE(IFMTTYPE fmttype)
        {
            if (fmttype == null) throw new ArgumentNullException("fmttype");
            
            TypeName = fmttype.TypeName;
            SubTypeName = fmttype.SubTypeName;
        }

        public override string ToString()
        {
            return string.Format("FMTTYPE={0}/{1}", TypeName, SubTypeName);
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
    }

    [DataContract]
    public class LANGUAGE : ILANGUAGE, IEquatable<LANGUAGE>
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
            var pattern = @"^(?<tag>\w+)(\-(?<subtag>\w+))?$";
            if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                throw new FormatException("Invalid Format type");
            foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
            {
                if (match.Groups["tag"].Success) Tag = match.Groups["tag"].Value;
                else if (match.Groups["subtag"].Success) SubTag = match.Groups["subtag"].Value;
            }
        }

        public LANGUAGE(ILANGUAGE language)
        {
            if (language == null) throw new ArgumentNullException("language");
            Tag = language.Tag;
            SubTag = language.SubTag;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Tag))
            {
                return (!string.IsNullOrEmpty(SubTag)) ?
                    string.Format("LANGUAGE={0}-{1}", Tag, SubTag) :
                    string.Format("LANGUAGE={0}", Tag);
            }

            return string.Empty;
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
    }

    [DataContract]
    public class DELEGATE : IDELEGATE, IEquatable<DELEGATE>
    {
        /// <summary>
        /// Gets or sets the calendar addresses of the participation delegators
        /// </summary>
        [DataMember]
        public List<URI> Addresses { get; set; }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public DELEGATE()
        {
            Addresses = new List<URI>();
        }

        /// <summary>
        /// Overloaded Constructor
        /// </summary>
        /// <param name="addresses">The enumerable collection of calendar addresses, which represent the delegators</param>
        public DELEGATE(List<URI> addresses)
        {
            Addresses = addresses;
        }

        public DELEGATE(string value)
        {
            var uricheck = @"(?<value>(\w+)((([A-Za-z]{3,9}:(?:\/\/)?)(?:[\-;:&=\+\$,\w]+@)?[A-Za-z0-9\.\-]+|(?:www\.|[\-;:&=\+\$,\w]+@)[A-Za-z0-9\.\-]+)((?:\/[\+~%\/\.\w\-_]*)?\??(?:[\-\+=&;%@\.\w_]*)#?(?:[\.\!\/\\\w]*))?))";
            var pattern = string.Format("(\")(mailto:){0}(\")", uricheck);

            if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                throw new FormatException("Invalid Delegate format");
            var val = string.Empty;

            foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
            {
                if (match.Groups["value"].Success) val = match.Groups["value"].Value;
            }
            var parts = val.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            Addresses = new List<URI>(parts.Select(p => new URI(p)));
        }

        public DELEGATE(IDELEGATE @delegate)
        {
            if (@delegate == null) throw new ArgumentNullException("delegate");
            Addresses = @delegate.Addresses;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var addr in Addresses)
            {
                if (addr != Addresses.Last()) sb.AppendFormat("\"mailto:{0}\", ", addr);
                else sb.AppendFormat("\"mailto:{0}\"", addr);
            }
            return sb.ToString();
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
    }

    /// <summary>
    /// Group or List Membership.
    /// Provides the group or list membership of the calendar user specified by a property
    /// </summary>
    [DataContract]
    public class MEMBER : IMEMBER, IEquatable<MEMBER>
    {
        /// <summary>
        /// Gets or sets the calendar addresses of the group members
        /// </summary>
        [DataMember]
        public List<URI> Addresses { get; set; }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public MEMBER()
        {
            Addresses = new List<URI>();
        }

        /// <summary>
        /// Overloaded Constructor.
        /// </summary>
        /// <param name="addresses">The enumerable collection of addresses representing users in the membership list</param>
        public MEMBER(List<URI> addresses)
        {
            Addresses = addresses;
        }

        public MEMBER(string value)
        {
            var uricheck = @"(?<value>(\w+)((([A-Za-z]{3,9}:(?:\/\/)?)(?:[\-;:&=\+\$,\w]+@)?[A-Za-z0-9\.\-]+|(?:www\.|[\-;:&=\+\$,\w]+@)[A-Za-z0-9\.\-]+)((?:\/[\+~%\/\.\w\-_]*)?\??(?:[\-\+=&;%@\.\w_]*)#?(?:[\.\!\/\\\w]*))?))";
            var pattern = string.Format("(\")(mailto:){0}(\")", uricheck);

            if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                throw new FormatException("Invalid Member format");
            var val = string.Empty;

            foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
            {
                if (match.Groups["value"].Success) val = match.Groups["value"].Value;
            }
            var parts = val.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            Addresses = new List<URI>(parts.Select(p => new URI(p)));
        }

        public MEMBER(IMEMBER member)
        {
            if (member == null) throw new ArgumentNullException("member");
            member.Addresses = member.Addresses;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("MEMBER=");
            foreach (var addr in Addresses)
            {
                if (addr != Addresses.Last()) sb.AppendFormat("mailto:{0}, ", addr);
                else sb.AppendFormat("mailto:{0}", addr);
            }

            return sb.ToString();
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
    }
}