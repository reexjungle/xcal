using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.xcal.domain.contracts;


namespace reexmonkey.xcal.domain.models
{
    [DataContract]
    public class TZID : ITZID, IEquatable<TZID>, IComparable<TZID>
    {
        [DataMember]
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the time zone definition for a time component
        /// </summary>
        [DataMember]
        public string Suffix { get; set; }

        [DataMember]
        public bool GloballyUnique { get; set; }

        public TZID()
        {
            this.Prefix = string.Empty;
            this.Suffix = "GMT";
            this.GloballyUnique = true;
        }

        public TZID(string value)
        {

        }

        public TZID(string prefix, string suffix, bool unique = false)
        {
            if (string.IsNullOrEmpty(suffix)) throw new ArgumentException("Suffix MUST neither be emty nor null");
            this.Prefix = prefix;
            this.Suffix = suffix;
            this.GloballyUnique = unique;
        }

        public override string ToString()
        {
            if (this.GloballyUnique) return string.Format("TZID=/{0}", this.Suffix);
            return string.Format("TZID={0}/{1}", this.Prefix, this.Suffix);
        }

        public bool Equals(TZID other)
        {
            if (other == null) return false;
            return
                this.Suffix == other.Suffix &&
                this.GloballyUnique == other.GloballyUnique;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return this.Equals((TZID)obj);
        }

        public override int GetHashCode()
        {
            return this.Suffix.GetHashCode() ^
                this.GloballyUnique.GetHashCode();
        }

        public int CompareTo(TZID other)
        {
            return this.Suffix.CompareTo(other.Suffix);
        }

        public static bool operator ==(TZID a, TZID b)
        {
            if ((object)a == null || (object)b == null) return object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(TZID a, TZID b)
        {
            if ((object)a == null || (object)b == null) return !object.Equals(a, b);
            return !a.Equals(b);
        }

        public static bool operator <(TZID a, TZID b)
        {
            if ((object)a == null || (object)b == null) return false;
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(TZID a, TZID b)
        {
            if ((object)a == null || (object)b == null) return false;
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
            this.TypeName = null;
            this.SubTypeName = null;
        }

        public FMTTYPE(string name, string subname)
        {
            this.TypeName = name;
            this.SubTypeName = subname;
        }

        public FMTTYPE(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("value");
                var pattern = @"^FMTTYPE=(?<type>\w+)(\.?<subtype>\w+)?$";
                if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                    throw new FormatException("Invalid Format type");
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                {
                    if (match.Groups["type"].Success) this.TypeName = match.Groups["type"].Value;
                    else if (match.Groups["subtype"].Success) this.SubTypeName = match.Groups["subtype"].Value;
                }

            }
            catch (FormatException) { throw; }
            catch (ArgumentNullException) { throw; }
            catch (ArgumentOutOfRangeException) { throw; }
            catch (ArgumentException) { throw; }
            catch (Exception) { throw; }
        }

        public FMTTYPE(IFMTTYPE fmttype)
        {
            if (fmttype == null) throw new ArgumentNullException("fmttype");
            this.TypeName = fmttype.TypeName;
            this.SubTypeName = fmttype.SubTypeName;
        }

        public override string ToString()
        {
            return string.Format("FMTTYPE={0}/{1}", this.TypeName, this.SubTypeName);
        }

        public bool Equals(FMTTYPE other)
        {
            if (other == null) return false;
            return (this.TypeName.Equals(other.TypeName, StringComparison.OrdinalIgnoreCase) &&
                this.SubTypeName.Equals(other.SubTypeName, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return this.Equals(obj as FMTTYPE);
        }

        public override int GetHashCode()
        {
            return this.TypeName.GetHashCode() ^ this.SubTypeName.GetHashCode();
        }

        public static bool operator ==(FMTTYPE a, FMTTYPE b)
        {
            if ((object)a == null || (object)b == null) return object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(FMTTYPE a, FMTTYPE b)
        {
            if ((object)a == null || (object)b == null) return !object.Equals(a, b);
            return !a.Equals(b);
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
            this.Tag = null;
            this.SubTag = null;
        }

        public LANGUAGE(string tag, string subtag)
        {
            if (string.IsNullOrEmpty(tag)) throw new ArgumentException("tag");
            this.Tag = tag;
            this.SubTag = subtag;
        }

        public LANGUAGE(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("value");
                var pattern = @"^(LANGUAGE=)?(?<tag>\w+)(\-(?<subtag>\w+))?$";
                if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                    throw new FormatException("Invalid Format type");
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                {
                    if (match.Groups["tag"].Success) this.Tag = match.Groups["tag"].Value;
                    else if (match.Groups["subtag"].Success) this.SubTag = match.Groups["subtag"].Value;
                }

            }
            catch (FormatException) { throw; }
            catch (ArgumentNullException) { throw; }
            catch (ArgumentOutOfRangeException) { throw; }
            catch (ArgumentException) { throw; }
            catch (Exception) { throw; }
        }

        public LANGUAGE(ILANGUAGE language)
        {
            if (language == null) throw new ArgumentNullException("language");
            this.Tag = language.Tag;
            this.SubTag = language.SubTag;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(this.Tag))
            {
                return (!string.IsNullOrEmpty(this.SubTag)) ?
                    string.Format("LANGUAGE={0}-{1}", this.Tag, this.SubTag) :
                    string.Format("LANGUAGE={0}", this.Tag);
            }

            return string.Empty;
        }

        public bool Equals(LANGUAGE other)
        {
            if (other == null) return false;
            return (!string.IsNullOrEmpty(this.SubTag) ? this.Tag.Equals(other.Tag, StringComparison.OrdinalIgnoreCase) :
                this.Tag.Equals(other.Tag, StringComparison.OrdinalIgnoreCase) &&
                this.SubTag.Equals(other.SubTag, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return this.Equals(obj as LANGUAGE);
        }

        public override int GetHashCode()
        {
            return this.Tag.GetHashCode() ^ this.SubTag.GetHashCode();
        }

        public static bool operator ==(LANGUAGE a, LANGUAGE b)
        {
            if ((object)a == null || (object)b == null) return object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(LANGUAGE a, LANGUAGE b)
        {
            if ((object)a == null || (object)b == null) return !object.Equals(a, b);
            return !a.Equals(b);
        }

    }

    [DataContract]
    public class DELEGATE: IDELEGATE, IEquatable<DELEGATE>
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
            this.Addresses = new List<URI>();
        }

        /// <summary>
        /// Overloaded Constructor
        /// </summary>
        /// <param name="addresses">The enumerable collection of calendar addresses, which represent the delegators</param>
        public DELEGATE(List<URI> addresses)
        {
            this.Addresses = addresses;
        }

        public DELEGATE(string value)
        {
            var uricheck = @"(DELEGATED-FROM=|DELEGATED-TO=)?(?<value>(\w+)((([A-Za-z]{3,9}:(?:\/\/)?)(?:[\-;:&=\+\$,\w]+@)?[A-Za-z0-9\.\-]+|(?:www\.|[\-;:&=\+\$,\w]+@)[A-Za-z0-9\.\-]+)((?:\/[\+~%\/\.\w\-_]*)?\??(?:[\-\+=&;%@\.\w_]*)#?(?:[\.\!\/\\\w]*))?))";
            var pattern = string.Format(@"^(DELEGATED\-{0}(,[\s]*{0})*$", uricheck);
            try
            {
                if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                    throw new FormatException("Invalid Delegate format");
                var val = string.Empty;

                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                {
                    if (match.Groups["value"].Success) val = match.Groups["value"].Value;
                }
                var parts = val.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                this.Addresses = new List<URI>(parts.Select(p => new URI(p)));
            }
            catch (ArgumentNullException) { throw; }
            catch (ArgumentOutOfRangeException) { throw; }
            catch (ArgumentException) { throw; }
        }

        public DELEGATE(IDELEGATE del)
        {
            if (del == null) throw new ArgumentNullException("delegatee or delegator");
            this.Addresses = del.Addresses;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var addr in this.Addresses)
            {
                if (addr != this.Addresses.Last()) sb.AppendFormat("\"mailto:{0}\", ", addr);
                else sb.AppendFormat("\"mailto:{0}\"", addr); 
            }
            return sb.ToString();
        }

        public bool Equals(DELEGATE other)
        {
            if (other == null) return false;
            return this.Addresses.AreDuplicatesOf(other.Addresses);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return this.Equals(obj as DELEGATE);
        }

        public override int GetHashCode()
        {
            return this.Addresses.GetHashCode();
        }

        public static bool operator ==(DELEGATE x, DELEGATE y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(DELEGATE x, DELEGATE y)
        {
            if ((object)x == null || (object)y == null) return !object.Equals(x, y);
            return !x.Equals(y);
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
            this.Addresses = new List<URI>();
        }

        /// <summary>
        /// Overloaded Constructor.
        /// </summary>
        /// <param name="addresses">The enumerable collection of addresses representing users in the membership list</param>
        public MEMBER(List<URI> addresses)
        {
            this.Addresses = addresses;
        }

        public MEMBER(string value)
        {
            var uricheck = @"(?<value>(\w+)((([A-Za-z]{3,9}:(?:\/\/)?)(?:[\-;:&=\+\$,\w]+@)?[A-Za-z0-9\.\-]+|(?:www\.|[\-;:&=\+\$,\w]+@)[A-Za-z0-9\.\-]+)((?:\/[\+~%\/\.\w\-_]*)?\??(?:[\-\+=&;%@\.\w_]*)#?(?:[\.\!\/\\\w]*))?))";
            var pattern = string.Format(@"^(DELEGATED\-{0}(,\s*{0})*$", uricheck);

            try
            {
                if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                    throw new FormatException("Invalid Member format");
                var val = string.Empty;

                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                {
                    if (match.Groups["value"].Success) val = match.Groups["value"].Value;
                }
                var parts = val.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                this.Addresses = new List<URI>(parts.Select(p => new URI(p)));

            }
            catch (FormatException) { throw; }
            catch (ArgumentNullException) { throw; }
            catch (ArgumentOutOfRangeException) { throw; }
            catch (ArgumentException) { throw; }
            catch (Exception) { throw; }
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
            foreach (var addr in this.Addresses)
            {
                if (addr != this.Addresses.Last()) sb.AppendFormat("mailto:{0}, ", addr);
                else sb.AppendFormat("mailto:{0}", addr);
            }

            return sb.ToString();
        }

        public bool Equals(MEMBER other)
        {
            if (other == null) return false;
            return this.Addresses.AreDuplicatesOf(other.Addresses);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return this.Equals(obj as MEMBER);
        }

        public override int GetHashCode()
        {
            return this.Addresses.GetHashCode();
        }

        public static bool operator ==(MEMBER a, MEMBER b)
        {
            if ((object)a == null || (object)b == null) return object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(MEMBER a, MEMBER b)
        {
            if ((object)a == null || (object)b == null) return !object.Equals(a, b);
            return !a.Equals(b);
        }
    }

}