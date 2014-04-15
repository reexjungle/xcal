using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.xcal.domain.contracts;

namespace reexmonkey.xcal.domain.models
{

    /// <summary>
    /// Provides the alternative text representation of a property value.
    /// </summary>
    [DataContract]
    public class ALTREP : IURI, IEquatable<ALTREP>, IComparable<ALTREP>
    {
        private string path;

        [DataMember]
        public string Path
        {
            get { return this.path; }
            set
            {
                if (Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute)) this.path = value;
                else throw new FormatException("The format of the path is not URI-compatible");
            }

        }

        public bool IsDefault()
        {
            return (this.Path.Equals(string.Empty));
        }

        public ALTREP(string value)
        {
            if (Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute)) this.path = value;
            else throw new FormatException("The path is not a valid uri!");
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("ALTREP=\"{0}\"", this.path);
            return sb.ToString();
        }

        public bool Equals(ALTREP other)
        {
            if (other == null) return false;
            return (this.path.Equals(other.Path, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var other = (ALTREP)obj;
            return (other == null) ? false : Equals(other);
        }

        public override int GetHashCode()
        {
            return this.path.GetHashCode();
        }

        public int CompareTo(ALTREP other)
        {
            return this.path.CompareTo(other.Path);
        }

        public static bool operator <(ALTREP x, ALTREP y)
        {
            if (x == null || y == null) return false;
            return x.CompareTo(y) < 0;
        }

        public static bool operator >(ALTREP x, ALTREP y)
        {
            if (x == null || y == null) return false;
            return x.CompareTo(y) > 0;
        }

        public static bool operator ==(ALTREP x, ALTREP y)
        {
            if ((object)x == null || (object)y == null) return object.Equals(x, y);
            return x.Equals(y);
        }

        public static bool operator !=(ALTREP x, ALTREP y)
        {
            if (x == null || y == null) return !object.Equals(x, y);
            return !x.Equals(y);
        }

    }

    [DataContract]
    public class CN : ICN, IEquatable<CN>
    {
        /// <summary>
        /// Gets or sets the Uniform Resource Identifier (URI) that points to an alternative representation for a textual property value
        /// </summary>
        [DataMember]
        public string Value { get; set; }

        public bool IsDefault()
        {
            return string.IsNullOrEmpty(this.Value);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uri">The textual representation of the URI value for the alternative text representation</param>
        public CN(string value)
        {
            this.Value = value;
        }

        public bool Equals(CN other)
        {
            if (other == null) return false;
            return (this.Value == other.Value);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("CN={0}", this.Value);
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return this.Equals(obj as CN);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public static bool operator ==(CN cn, CN other)
        {
            if (other == null) return false;
            return cn.Equals(other);
        }

        public static bool operator !=(CN cn, CN other)
        {
            if (other == null) return false;
            return !cn.Equals(other);
        }

    }

    [DataContract]
    [KnownType(typeof(URI))]
    public class DELEGATED_FROM : IDELEGATE, IEquatable<DELEGATED_FROM>
    {
        /// <summary>
        /// Gets or sets the calendar addresses of the participation delegators
        /// </summary>
        [DataMember]
        public List<IURI> Addresses { get; set; }

        public bool IsDefault()
        {
             return this.Addresses.Count() == 0;
        }
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public DELEGATED_FROM()
        {
            this.Addresses = new List<IURI>();
        }

        /// <summary>
        /// Overloaded Constructor
        /// </summary>
        /// <param name="addresses">The enumerable collection of calendar addresses, which represent the delegators</param>
        public DELEGATED_FROM (List<IURI> addresses)
        {
            this.Addresses = addresses;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("DELEGATED-FROM=");
            foreach (var addr in this.Addresses)
            {
                if (addr != this.Addresses.Last()) sb.AppendFormat("\"mailto:{0}\", ", addr);
                else sb.AppendFormat("\"mailto:{0}\"", addr); 
            }
            return sb.ToString();
        }

        public bool Equals(DELEGATED_FROM other)
        {
            if (other == null) return false;
            return this.Addresses.AreDuplicatesOf(other.Addresses);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return this.Equals(obj as DELEGATED_FROM);
        }

        public override int GetHashCode()
        {
            return this.Addresses.GetHashCode();
        }

        public static bool operator ==(DELEGATED_FROM a, DELEGATED_FROM b)
        {
            if (a == null || b == null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(DELEGATED_FROM a, DELEGATED_FROM b)
        {
            if (a == null || b == null) return false;
            return !a.Equals(b);
        }
    }

    [DataContract]
    [KnownType(typeof(URI))]
    public class DELEGATED_TO : IDELEGATE, IEquatable<DELEGATED_TO>
    {
        /// <summary>
        /// Gets or sets the calendar addresses of the participation delegators
        /// </summary>
        [DataMember]
        public List<IURI> Addresses { get; set; }

        public bool IsDefault()
        {
             return this.Addresses.Count() == 0;
        }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public DELEGATED_TO()
        {
            this.Addresses = new List<IURI>() { };
        }

        /// <summary>
        /// Overloaded Constructor
        /// </summary>
        /// <param name="addresses">The enumerable collection of calendar addresses, which represent the delegators</param>
        public DELEGATED_TO(List<IURI> addresses)
        {
            this.Addresses = addresses;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("DELEGATED-TO=");
            foreach (var addr in this.Addresses)
            {
                if (addr != this.Addresses.Last()) sb.AppendFormat("\"mailto:{0}\", ", addr);
                else sb.AppendFormat("\"mailto:{0}\"", addr);
            }
            return sb.ToString();
        }

        public bool Equals(DELEGATED_TO other)
        {
            if (other == null) return false;
            return this.Addresses.AreDuplicatesOf(other.Addresses);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return this.Equals(obj as DELEGATED_TO);
        }

        public override int GetHashCode()
        {
            return this.Addresses.GetHashCode();
        }

        public static bool operator ==(DELEGATED_TO a, DELEGATED_TO b)
        {
            if (a == null || b == null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(DELEGATED_TO a, DELEGATED_TO b)
        {
            if (a == null || b == null) return false;
            return !a.Equals(b);
        }

    }

    /// <summary>
    /// Directory Entry Reference. 
    /// Provides a reference to a directory entry associated with the calendar user specified by the user.
    /// </summary>
    [DataContract]
    [KnownType(typeof(URI))]
    public class DIR : IDIR, IEquatable<DIR>
    {
        /// <summary>
        /// Gets or sets URI reference to the directory entry.
        /// </summary>
        [DataMember]
        public IURI Uri{ get; set; }

        public bool IsDefault()
        {
             return this.Uri.IsDefault();
        }

        /// <summary>
        /// Overloaded constructor
        /// </summary>
        /// <param name="value">The textual representation of the URI value for the alternative text representation</param>
        public DIR(URI value)
        {
            this.Uri = value;
        }

        public override string ToString()
        {
            return string.Format("DIR={0}", this.Uri);
        }

        public bool Equals(DIR other)
        {
            if (other == null) return false;
            return (this.Uri == other.Uri);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return this.Equals(obj as DIR);
        }

        public override int GetHashCode()
        {
            return this.Uri.GetHashCode();
        }

        public static bool operator ==(DIR dir, DIR other)
        {
            if (other == null) return false;
            return dir.Equals(other);
        }

        public static bool operator !=(DIR dir, DIR other)
        {
            if (other == null) return false;
            return !dir.Equals(other);
        }

    }

    [DataContract]
    public class FMTTYPE: IFMTTYPE, IEquatable<FMTTYPE>
    {
        [DataMember]
        public string TypeName { get; set; }

        [DataMember]
        public string SubTypeName { get; set; }

        public bool IsDefault()
        {
            return (string.IsNullOrEmpty(this.TypeName) && string.IsNullOrEmpty(this.SubTypeName));
        }

        public FMTTYPE(string name, string subname)
        {
            this.TypeName = name;
            this.SubTypeName = subname;
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
            if (obj == null) return false;
            return this.Equals(obj as FMTTYPE);
        }

        public override int GetHashCode()
        {
            return this.TypeName.GetHashCode() ^this.SubTypeName.GetHashCode();
        }

        public static bool operator ==(FMTTYPE fmttype, FMTTYPE other)
        {
            if (other == null) return false;
            return fmttype.Equals(other);
        }

        public static bool operator !=(FMTTYPE fmttype, FMTTYPE other)
        {
            if (other == null) return false;
            return !fmttype.Equals(other);
        }

    }

    [DataContract]
    public class LANGUAGE : ILANGUAGE, IEquatable<LANGUAGE>
    {
        [DataMember]
        public string Tag { get; set; }

        [DataMember]
        public string SubTag { get; set; }

        public bool IsDefault()
        {
            return (string.IsNullOrEmpty(this.Tag) && string.IsNullOrEmpty(this.SubTag));
        }

        public LANGUAGE(string tag, string subtag=null)
        {
            if (string.IsNullOrEmpty(tag)) throw new ArgumentException("tag");
            this.Tag = tag;
            this.SubTag = subtag;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(this.Tag))  
            {
               return (!string.IsNullOrEmpty(this.SubTag))? 
                   string.Format("LANGUAGE={0}-{1}", this.Tag, this.SubTag):
                   string.Format("LANGUAGE={0}", this.Tag);
            }

            return string.Empty;
        }

        public bool Equals(LANGUAGE other)
        {
            if (other == null) return false;
            return (!string.IsNullOrEmpty(this.SubTag)? this.Tag.Equals(other.Tag, StringComparison.OrdinalIgnoreCase):
                this.Tag.Equals(other.Tag, StringComparison.OrdinalIgnoreCase) && 
                this.SubTag.Equals(other.SubTag, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return this.Equals(obj as LANGUAGE);
        }

        public override int GetHashCode()
        {
            return this.Tag.GetHashCode() ^ this.SubTag.GetHashCode();
        }

        public static bool operator ==(LANGUAGE language, LANGUAGE other)
        {
            if (other == null) return false;
            return language.Equals(other);
        }

        public static bool operator !=(LANGUAGE language, LANGUAGE other)
        {
            if (other == null) return false;
            return !language.Equals(other);
        }

    }

    /// <summary>
    /// Group or List Membership.
    /// Provides the group or list membership of the calendar user specified by a property
    /// </summary>
    [DataContract]
    [KnownType(typeof(URI))]
    public class MEMBER : IMEMBER, IEquatable<MEMBER>
    {
        /// <summary>
        /// Gets or sets the calendar addresses of the group members
        /// </summary>
        [DataMember]
        public IEnumerable<IURI> Addresses { get; set; }

        public bool IsDefault()
        {
             return this.Addresses.Count() == 0;
        }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public MEMBER()
        {
            this.Addresses = new List<IURI>();
        }

        /// <summary>
        /// Overloaded Constructor.
        /// </summary>
        /// <param name="addresses">The enumerable collection of addresses representing users in the membership list</param>
        public MEMBER(IEnumerable<IURI> addresses)
        {
            this.Addresses = addresses;
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
            if (obj == null) return false;
            return this.Equals(obj as MEMBER);
        }

        public override int GetHashCode()
        {
            return this.Addresses.GetHashCode();
        }

        public static bool operator ==(MEMBER a, MEMBER b)
        {
            if (a == null || b == null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(MEMBER a, MEMBER b)
        {
            if (a == null || b == null) return false;
            return !a.Equals(b);
        }
    }

    /// <summary>
    /// Sent By.
    /// Provides the calendar user that is acting on behalf of the calendar user specified by a property.
    /// </summary>
    [DataContract]
    [KnownType(typeof(URI))]
    public class SENT_BY : ISENT_BY
    {
        /// <summary>
        /// Gets or sets the address of the calendar user, who is acting on behalf of another user.
        /// </summary>
        [DataMember]
        public IURI Address { get; set; }

        public bool IsDefault()
        {
             return this.Address.IsDefault();
        }

        /// <summary>
        /// Overloaded constructor
        /// </summary>
        /// <param name="address">The calendar address of the calendar user</param>
        public SENT_BY(URI address)
        {
            this.Address = address;
        }

        public override string ToString()
        {
             return string.Format("SENT-BY =\"mailto:{0}\"", this.Address);

        }

    }

    /// <summary>
    /// Time Zone Identifier.
    /// Provides the identifier for the time zone definition for a time component in a property value.
    /// </summary>
    /// <remarks>
    /// This paramter MUST be specified on the DATE_TIME, DATE_TIME, DUE, EXDATE and RDATE propertiws when either a DATE-TIME 
    /// or TIME value type is specified and when the value is neither a UTC or &quot; floating time &quot; time.
    /// </remarks>
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

        public bool IsDefault()
        {
           return this.Suffix.Equals(string.Empty);
        }

        public TZID()
        {
            this.Prefix = string.Empty;
            this.Suffix = "GMT";
            this.GloballyUnique = true;
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
            if(this.GloballyUnique) return string.Format("TZID=/{0}", this.Suffix);
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
            if (obj == null) return false;
            return this.Equals((TZID) obj);
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
            if (a == null || b == null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(TZID a, TZID b)
        {
            if (a == null || b == null) return false;
            return !a.Equals(b);
        }

        public static bool operator <(TZID a, TZID b)
        {
            if (a == null || b == null) return false;
            return a.CompareTo(b) < 0;
        }

        public static bool operator > (TZID a, TZID b)
        {
            if (a == null || b == null) return false;
            return a.CompareTo(b) > 0;
        }

    }

}