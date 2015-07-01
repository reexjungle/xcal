using reexjungle.xcal.domain.contracts;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.foundation.contracts;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace reexjungle.xcal.domain.models
{
    #region Descriptive Properties

    /// <summary>
    /// Specifies a contract for attaching an inline binary encooded content information.
    /// </summary>
    [DataContract]
    public class ATTACH_BINARY : IATTACH<BINARY>, IEquatable<ATTACH_BINARY>, IContainsKey<Guid>
    {
        /// <summary>
        /// ID of an Attachment for a particular Calendar component
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Binary encoded content of the attachment
        /// </summary>
        [DataMember]
        public BINARY Content { get; set; }

        /// <summary>
        /// Media Type of the resource in an Attachmant
        /// </summary>
        [DataMember]
        public FMTTYPE FormatType { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ATTACH_BINARY()
        {
            FormatType = null;
            Content = null;
        }

        public ATTACH_BINARY(IATTACH<BINARY> attachment)
        {
            FormatType = attachment.FormatType;
            Content = attachment.Content;
        }

        public ATTACH_BINARY(BINARY content, FMTTYPE format = null)
        {
            Content = content;
            FormatType = format;
        }

        /// <summary>
        /// Overloaded ToString method
        /// </summary>
        /// <returns>String of the format "ATTACH;FormatType;ENCODING=Encoding; VALUE=BINARY:Content"</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Content != null)
            {
                sb.AppendFormat("ATTACH");
                if (FormatType != null) sb.AppendFormat(";{0}", FormatType);
                sb.AppendFormat(";ENCODING={0}", Content.Encoding);
                sb.AppendFormat(";VALUE=BINARY:{0}", Content);
            }

            return sb.ToString();
        }

        public bool Equals(ATTACH_BINARY other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ATTACH_BINARY)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(ATTACH_BINARY left, ATTACH_BINARY right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ATTACH_BINARY left, ATTACH_BINARY right)
        {
            return !Equals(left, right);
        }
    }

    /// <summary>
    /// Capability to associate a document or another resource given by it's URI with a calendar component
    /// </summary>
    [DataContract]
    public class ATTACH_URI : IATTACH<URI>, IEquatable<ATTACH_URI>, IComparable<ATTACH_URI>, IContainsKey<Guid>
    {
        /// <summary>
        /// ID of an Attachment for a particular Calendar component
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Media Type of the resource in an Attachmant
        /// </summary>
        [DataMember]
        public FMTTYPE FormatType { get; set; }

        /// <summary>
        /// Universal Resource Identifier of the attached Content
        /// </summary>
        [DataMember]
        public URI Content { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ATTACH_URI()
        {
        }

        public ATTACH_URI(IATTACH<URI> attachment)
        {
            Content = attachment.Content;
            FormatType = attachment.FormatType;
        }

        /// <summary>
        /// Constructor on the base of CONTENT of the Attachment and it's TYPE
        /// </summary>
        /// <param name="content">URI of the attached content</param>
        /// <param name="format">The format type of the attachmen</param>
        public ATTACH_URI(URI content, FMTTYPE format = null)
        {
            FormatType = format;
            Content = content;
        }

        /// <summary>
        /// Overloaded ToString method
        /// </summary>
        /// <returns>String of the format "ATTACH;FormatType;ENCODING=Encoding; VALUE=BINARY:Content"</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("ATTACH");
            if (!FormatType.Equals(default(FMTTYPE)))
                sb.AppendFormat(";{0}", FormatType);
            sb.AppendFormat(":{0}", Content);
            return sb.ToString();
        }

        public bool Equals(ATTACH_URI other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ATTACH_URI)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(ATTACH_URI left, ATTACH_URI right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ATTACH_URI left, ATTACH_URI right)
        {
            return !Equals(left, right);
        }

        public int CompareTo(ATTACH_URI other)
        {
            return Content.CompareTo(other.Content);
        }

        public static bool operator <(ATTACH_URI a, ATTACH_URI b)
        {
            if (a == null || b == null) return false;
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(ATTACH_URI a, ATTACH_URI b)
        {
            if (a == null || b == null) return false;
            return a.CompareTo(b) > 0;
        }
    }

    /// <summary>
    /// Defines the categories or subtypes for a calendar component
    /// </summary>
    [DataContract]
    public class CATEGORIES : ICATEGORIES, IEquatable<CATEGORIES>, IContainsKey<Guid>
    {
        /// <summary>
        /// ID of the category
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Description of the category or the subtype
        /// </summary>
        [DataMember]
        public List<string> Values { get; set; }

        /// <summary>
        /// Language used for this category of calendar components
        /// </summary>
        [DataMember]
        public LANGUAGE Language { get; set; }

        public CATEGORIES()
        {
            Values = new List<string>();
            Language = null;
        }

        public CATEGORIES(ICATEGORIES categories)
        {
            Language = categories.Language;
            Values = categories.Values;
        }

        /// <summary>
        /// Constructor based on the TEXT and LANGUAGE properties of the class
        /// </summary>
        /// <param name="text">Description of the calendar component category or subtype</param>
        /// <param name="language">Language, used in a given category of calendar components</param>
        public CATEGORIES(List<string> values, LANGUAGE language = null)
        {
            Values = values;
            Language = language;
        }

        /// <summary>
        /// Overloaded ToString method
        /// </summary>
        /// <returns>String representation of a category property in the form of "CATEGORIES;Language;Text"</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("CATEGORIES");
            if (Language.Equals(string.Empty)) sb.AppendFormat(";{0}", Language);
            sb.Append(":");
            var last = Values.Last();
            foreach (var val in Values)
            {
                if (val != last) sb.AppendFormat("{0}, ", val);
                else sb.Append(val);
            }
            return sb.ToString();
        }

        public bool Equals(CATEGORIES other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CATEGORIES)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(CATEGORIES left, CATEGORIES right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CATEGORIES left, CATEGORIES right)
        {
            return !Equals(left, right);
        }
    }

    /// <summary>
    /// Specify non-processing information intended to provide a comment to the calendar user
    /// </summary>
    [DataContract]
    [KnownType(typeof(COMMENT))]
    [KnownType(typeof(CONTACT))]
    [KnownType(typeof(SUMMARY))]
    [KnownType(typeof(LOCATION))]
    [KnownType(typeof(DESCRIPTION))]
    public abstract class TEXTUAL : ITEXTUAL, IEquatable<TEXTUAL>, IComparable<TEXTUAL>, IContainsKey<Guid>
    {
        /// <summary>
        /// ID of the Comment to the Calendar Component
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The TEXT content of the comment
        /// </summary>
        [DataMember]
        [StringLength(int.MaxValue)]
        public string Text { get; set; }

        /// <summary>
        /// Alternative Text of the Comment, can content more particular Description of a Calendar Component
        /// </summary>
        [DataMember]
        public URI AlternativeText { get; set; }

        /// <summary>
        /// Languge of the Comment
        /// </summary>
        [DataMember]
        public LANGUAGE Language { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public TEXTUAL()
        {
            Text = string.Empty;
            AlternativeText = null;
        }

        public TEXTUAL(ITEXTUAL value)
        {
            Language = value.Language;
            AlternativeText = value.AlternativeText;
            Text = value.Text;
        }

        /// <summary>
        /// Constructor specifying the Text, Alternative Text and Language of the Comment
        /// </summary>
        /// <param name="text">Text content of the comment</param>
        /// <param name="altrep"></param>
        /// <param name="language">Language of the comment</param>
        protected TEXTUAL(string text, URI altrep, LANGUAGE language = null)
        {
            Text = text;
            AlternativeText = altrep;
            Language = language;
        }

        public bool Equals(TEXTUAL other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TEXTUAL)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(TEXTUAL left, TEXTUAL right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TEXTUAL left, TEXTUAL right)
        {
            return !Equals(left, right);
        }

        public int CompareTo(TEXTUAL other)
        {
            return Text.CompareTo(other.Text);
        }

        public static bool operator <(TEXTUAL a, TEXTUAL b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(TEXTUAL a, TEXTUAL b)
        {
            return a.CompareTo(b) > 0;
        }
    }

    [DataContract]
    public class DESCRIPTION : TEXTUAL
    {
        public DESCRIPTION()
        {
        }

        public DESCRIPTION(string text)
        {
            //todo: Parsing code here
        }

        public DESCRIPTION(string text, URI altrep, LANGUAGE language = null)
            : base(text, altrep, language)
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder("DESCRIPTION");
            if (AlternativeText != null) sb.AppendFormat(";ALTREP=\"{0}\"", AlternativeText);
            if (Language != null) sb.AppendFormat(";{0}", Language);
            sb.AppendFormat(":{0}", Text.EscapeStrings());
            return sb.ToString();
        }
    }

    [DataContract]
    public class COMMENT : TEXTUAL
    {
        public COMMENT()
        {
        }

        public COMMENT(string text)
        {
            //todo: Parsing code here
        }

        public COMMENT(string text, URI altrep, LANGUAGE language = null)
            : base(text, altrep, language)
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder("COMMENT");
            if (AlternativeText != null) sb.AppendFormat(";ALTREP=\"{0}\"", AlternativeText);
            if (Language != null) sb.AppendFormat(";{0}", Language);
            sb.AppendFormat(":{0}", Text.EscapeStrings());
            return sb.ToString();
        }
    }

    [DataContract]
    public class CONTACT : TEXTUAL
    {
        public CONTACT()
        {
        }

        public CONTACT(string text)
        {
            //todo: Parsing code here
        }

        public CONTACT(string text, URI altrep, LANGUAGE language = null)
            : base(text, altrep, language)
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder("CONTACT");
            if (AlternativeText != null) sb.AppendFormat(";ALTREP=\"{0}\"", AlternativeText);
            if (Language != null) sb.AppendFormat(";{0}", Language);
            sb.AppendFormat(":{0}", Text.EscapeStrings());
            return sb.ToString();
        }
    }

    [DataContract]
    public class SUMMARY : TEXTUAL
    {
        public SUMMARY()
        {
        }

        public SUMMARY(string text)
        {
            //todo: Parsing code here
        }

        public SUMMARY(string text, URI altrep, LANGUAGE language = null)
            : base(text, altrep, language)
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder("SUMMARY");
            if (AlternativeText != null) sb.AppendFormat(";ALTREP=\"{0}\"", AlternativeText);
            if (Language != null) sb.AppendFormat(";{0}", Language);
            sb.AppendFormat(":{0}", Text.EscapeStrings());
            return sb.ToString();
        }
    }

    [DataContract]
    public class LOCATION : TEXTUAL
    {
        public LOCATION()
        {
        }

        public LOCATION(string text)
        {
            //todo: Parsing code here
        }

        public LOCATION(string text, URI altrep, LANGUAGE language = null)
            : base(text, altrep, language)
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder("LOCATION");
            if (AlternativeText != null) sb.AppendFormat(";ALTREP=\"{0}\"", AlternativeText);
            if (Language != null) sb.AppendFormat(";{0}", Language);
            sb.AppendFormat(":{0}", Text.EscapeStrings());
            return sb.ToString();
        }
    }

    /// <summary>
    /// Specifies the information related to the global position for the activity spiecified by a calendar component
    /// </summary>
    [DataContract]
    public struct GEO : IGEO, IEquatable<GEO>, IContainsKey<Guid>
    {
        private readonly float longitude;
        private readonly float latitude;
        private Guid id;

        [DataMember]
        public Guid Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Longitude of the Geographical Position
        /// </summary>
        [DataMember]
        public float Longitude
        {
            get { return longitude; }
        }

        /// <summary>
        /// Lattitude of the Geographical Position
        /// </summary>
        [DataMember]
        public float Latitude
        {
            get { return latitude; }
        }

        public GEO(IGEO geo)
        {
            id = Guid.Empty;
            latitude = geo.Latitude;
            longitude = geo.Longitude;
        }

        /// <summary>
        /// Constructor based on the Longitude and Lattitude of the specified location
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        public GEO(float lon, float lat)
        {
            id = Guid.Empty;
            longitude = lon;
            latitude = lat;
        }

        public GEO(string value)
        {
            id = Guid.Empty;
            longitude = default(float);
            latitude = default(float);

            var pattern = @"^(?<lon>[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)(?<lat>[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)$";
            if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
            {
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
                {
                    if (match.Groups["lon"].Success) longitude = float.Parse(match.Groups["lon"].Value);
                    if (match.Groups["lat"].Success) latitude = float.Parse(match.Groups["lat"].Value);
                }
            }
        }

        public bool Equals(GEO other)
        {
            return longitude.Equals(other.longitude) &&
                latitude.Equals(other.latitude);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is GEO && Equals((GEO)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (longitude.GetHashCode() * 397) ^
                    latitude.GetHashCode();
            }
        }

        public static bool operator ==(GEO left, GEO right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GEO left, GEO right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Overloaded ToString Method
        /// </summary>
        /// <returns>String representation of the GEO property in form of "GEO:Lattilude;Longitude" </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("GEO:{0};{1}", Latitude, Longitude);
            return sb.ToString();
        }
    }

    /// <summary>
    /// Defines the Equipment or resources anticipated for an activity specified by a calendar component
    /// </summary>
    [DataContract]
    public class RESOURCES : IRESOURCES, IEquatable<RESOURCES>, IContainsKey<Guid>
    {
        /// <summary>
        /// ID of a particular Resource
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Alternative Text for a specified Resource, can content the Description of this resource
        /// </summary>
        [DataMember]
        public URI AlternativeText { get; set; }

        /// <summary>
        /// Language needed for a particular Resource
        /// </summary>
        [DataMember]
        public LANGUAGE Language { get; set; }

        /// <summary>
        /// Name and other parameters, descripting a particular Resource
        /// </summary>
        [DataMember]
        public List<string> Values { get; set; }

        /// <summary>
        /// Indicates, if the Resource Property is set to default
        /// </summary>
        public bool IsDefault()
        {
            return Values.Count() == 0 && AlternativeText.IsDefault();
        }

        public RESOURCES()
        {
            Values = null;
            AlternativeText = null;
            Language = null;
        }

        public RESOURCES(IRESOURCES resources)
        {
            AlternativeText = resources.AlternativeText;
            Language = resources.Language;
            Values = resources.Values;
        }

        /// <summary>
        /// Constructor specifying the Text, Alternative Text and Language for Resource Property
        /// </summary>
        /// <param name="text">Text with the name and other parameters, specifying the current Resource</param>
        /// <param name="alt">Alternative Text, can represent particular the description of the Resource</param>
        /// <param name="language">Language nescessary for the Resource</param>
        public RESOURCES(List<string> values, URI alt, LANGUAGE language)
        {
            Values = values;
            AlternativeText = alt;
            Language = language;
        }

        /// <summary>
        /// Overloaded ToString Method
        /// </summary>
        /// <returns>String representation of the Resource property in form of "RESOURCES;AlternativeText;Language:Text"</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("RESOURCES");
            if (AlternativeText != null) sb.AppendFormat(";ALTREP=\"{0}\"", AlternativeText);
            if (Language != null) sb.AppendFormat(";{0}", Language);
            sb.Append(":");
            var last = Values.Last();
            foreach (var val in Values)
            {
                if (val != last) sb.AppendFormat("{0}, ", val);
                else sb.Append(val);
            }
            return sb.ToString();
        }

        public bool Equals(RESOURCES other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((RESOURCES)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(RESOURCES left, RESOURCES right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RESOURCES left, RESOURCES right)
        {
            return !Equals(left, right);
        }
    }

    [DataContract]
    public struct PRIORITY : IPRIORITY, IEquatable<PRIORITY>, IComparable<PRIORITY>
    {
        private readonly int value;
        private readonly PRIORITYLEVEL level;
        private readonly PRIORITYSCHEMA schema;
        private readonly PriorityType format;

        private int LevelToValue(PRIORITYLEVEL level)
        {
            var val = 0;
            switch (level)
            {
                case PRIORITYLEVEL.UNKNOWN: val = 0; break;
                case PRIORITYLEVEL.LOW: val = 9; break;
                case PRIORITYLEVEL.MEDIUM: val = 5; break;
                case PRIORITYLEVEL.HIGH: val = 1; break;
            }
            return val;
        }

        private PRIORITYLEVEL ValueToLevel(int value)
        {
            if (value >= 1 && this.value <= 4) return PRIORITYLEVEL.HIGH;
            else if (value == 5) return PRIORITYLEVEL.MEDIUM;
            else if (value >= 6 && value <= 9) return PRIORITYLEVEL.LOW;
            return PRIORITYLEVEL.UNKNOWN;
        }

        private int SchemaToValue(PRIORITYSCHEMA schema)
        {
            var val = 0;
            switch (schema)
            {
                case PRIORITYSCHEMA.UNKNOWN: val = 0; break;
                case PRIORITYSCHEMA.C3: val = 9; break;
                case PRIORITYSCHEMA.C2: val = 8; break;
                case PRIORITYSCHEMA.C1: val = 7; break;
                case PRIORITYSCHEMA.B3: val = 6; break;
                case PRIORITYSCHEMA.B2: val = 5; break;
                case PRIORITYSCHEMA.B1: val = 4; break;
                case PRIORITYSCHEMA.A3: val = 3; break;
                case PRIORITYSCHEMA.A2: val = 2; break;
                case PRIORITYSCHEMA.A1: val = 1; break;
            }
            return val;
        }

        private PRIORITYSCHEMA ValueToSchema(int value)
        {
            var schema = PRIORITYSCHEMA.UNKNOWN;
            switch (value)
            {
                case 1: schema = PRIORITYSCHEMA.A1; break;
                case 2: schema = PRIORITYSCHEMA.A2; break;
                case 3: schema = PRIORITYSCHEMA.A3; break;
                case 4: schema = PRIORITYSCHEMA.B1; break;
                case 5: schema = PRIORITYSCHEMA.B2; break;
                case 6: schema = PRIORITYSCHEMA.B3; break;
                case 7: schema = PRIORITYSCHEMA.C1; break;
                case 8: schema = PRIORITYSCHEMA.C2; break;
                case 9: schema = PRIORITYSCHEMA.C3; break;
            }

            return schema;
        }

        private PRIORITYLEVEL SchemaToLevel(PRIORITYSCHEMA schema)
        {
            var val = PRIORITYLEVEL.UNKNOWN;
            switch (schema)
            {
                case PRIORITYSCHEMA.UNKNOWN: val = PRIORITYLEVEL.UNKNOWN; break;
                case PRIORITYSCHEMA.C3: val = PRIORITYLEVEL.LOW; break;
                case PRIORITYSCHEMA.C2: val = PRIORITYLEVEL.LOW; break;
                case PRIORITYSCHEMA.C1: val = PRIORITYLEVEL.LOW; break;
                case PRIORITYSCHEMA.B3: val = PRIORITYLEVEL.LOW; break;
                case PRIORITYSCHEMA.B2: val = PRIORITYLEVEL.MEDIUM; break;
                case PRIORITYSCHEMA.B1: val = PRIORITYLEVEL.HIGH; break;
                case PRIORITYSCHEMA.A3: val = PRIORITYLEVEL.HIGH; break;
                case PRIORITYSCHEMA.A2: val = PRIORITYLEVEL.HIGH; break;
                case PRIORITYSCHEMA.A1: val = PRIORITYLEVEL.HIGH; break;
            }
            return val;
        }

        private PRIORITYSCHEMA LevelToSchema(PRIORITYLEVEL schema)
        {
            var value = PRIORITYSCHEMA.UNKNOWN;
            switch (schema)
            {
                case PRIORITYLEVEL.LOW: value = PRIORITYSCHEMA.C3; break;
                case PRIORITYLEVEL.MEDIUM: value = PRIORITYSCHEMA.B2; break;
                case PRIORITYLEVEL.HIGH: value = PRIORITYSCHEMA.A1; break;
                case PRIORITYLEVEL.UNKNOWN: value = PRIORITYSCHEMA.UNKNOWN; break;
            }

            return value;
        }

        public PriorityType Format
        {
            get { return format; }
        }

        public int Value
        {
            get { return value; }
        }

        public PRIORITYLEVEL Level
        {
            get { return level; }
        }

        public PRIORITYSCHEMA Schema
        {
            get { return schema; }
        }

        public PRIORITY(IPRIORITY priority)
        {
            format = priority.Format;
            value = priority.Value;
            schema = priority.Schema;
            level = priority.Level;
        }

        public PRIORITY(int value)
        {
            format = PriorityType.Integral;
            this.value = value;
            schema = PRIORITYSCHEMA.UNKNOWN;
            level = PRIORITYLEVEL.UNKNOWN;
            schema = ValueToSchema(value);
            level = ValueToLevel(value);
        }

        public PRIORITY(PRIORITYLEVEL level)
        {
            format = PriorityType.Level;
            value = 0;
            schema = PRIORITYSCHEMA.UNKNOWN;
            this.level = level;
            value = LevelToValue(level);
            schema = LevelToSchema(level);
        }

        public PRIORITY(PRIORITYSCHEMA schema)
        {
            format = PriorityType.Schema;
            value = 0;
            this.schema = schema;
            level = PRIORITYLEVEL.UNKNOWN;
            value = SchemaToValue(schema);
            level = SchemaToLevel(schema);
        }

        public PRIORITY(string value)
        {
            format = PriorityType.Integral;
            this.value = 0;
            schema = PRIORITYSCHEMA.UNKNOWN;
            level = PRIORITYLEVEL.UNKNOWN;

            var pattern = @"^(?<priority>PRIORITY:)?(?<value>\d)$$";
            if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
            {
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
                {
                    if (match.Groups["value"].Success) this.value = int.Parse(match.Groups["value"].Value);
                }
            }

            schema = ValueToSchema(this.value);
            level = ValueToLevel(this.value);
        }

        public override string ToString()
        {
            if (Format == PriorityType.Integral) return string.Format("PRIORITY:{0}", value);
            else if (Format == PriorityType.Level) return string.Format("PRIORITY:{0}", LevelToValue(level));
            else return string.Format("PRIORITY:{0}", SchemaToValue(schema));
        }

        public bool Equals(PRIORITY other)
        {
            return value == other.value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is PRIORITY && Equals((PRIORITY)obj);
        }

        public override int GetHashCode()
        {
            return value;
        }

        public static bool operator ==(PRIORITY left, PRIORITY right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PRIORITY left, PRIORITY right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(PRIORITY other)
        {
            return Value.CompareTo(other.Value);
        }

        public static bool operator <(PRIORITY a, PRIORITY b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(PRIORITY a, PRIORITY b)
        {
            return a.CompareTo(b) > 0;
        }
    }

    #endregion Descriptive Properties

    #region Date and Time Component Properties

    /// <summary>
    /// Defines one or more free or busy time intervals
    /// </summary>
    [DataContract]
    public class FREEBUSY_INFO : IFREEBUSY_INFO, IContainsKey<Guid>, IEquatable<FREEBUSY_INFO>
    {
        /// <summary>
        /// ID of a free od busy time interval
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Defines, whether the time interval is FREE or BUSY
        /// </summary>
        [DataMember]
        public FBTYPE Type { get; set; }

        /// <summary>
        /// Period of time, taken by a calendar component
        /// </summary>
        [DataMember]
        [Ignore]
        public List<PERIOD> Periods { get; set; }

        public FREEBUSY_INFO()
        {
        }

        /// <summary>
        /// Constructor based on the time interval
        /// </summary>
        /// <param name="period"></param>
        public FREEBUSY_INFO(List<PERIOD> periods, FBTYPE type = FBTYPE.FREE)
        {
            Periods = periods;
            Type = type;
        }

        public bool Equals(FREEBUSY_INFO other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((FREEBUSY_INFO)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(FREEBUSY_INFO left, FREEBUSY_INFO right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FREEBUSY_INFO left, FREEBUSY_INFO right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Overloaded ToString Method
        /// </summary>
        /// <returns>String representation of the FREEBUSY property in form of "FREEBUSY;Type:Period"</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("FREEBUSY");
            if (Type != FBTYPE.UNKNOWN) sb.AppendFormat(";{0}", Type);
            sb.Append(":");

            if (!Periods.NullOrEmpty())
            {
                var last = Periods.Last();
                foreach (var period in Periods)
                {
                    if (period != last) sb.AppendFormat("{0}, ", period);
                    else sb.AppendFormat("{0}", period);
                }
            }
            return sb.ToString();
        }
    }

    #endregion Date and Time Component Properties

    #region Time-zone Properties

    /// <summary>
    /// Specifies the customary designation for a time zone description
    /// </summary>
    [DataContract]
    public class TZNAME : ITZNAME, IEquatable<TZNAME>, IComparable<TZNAME>, IContainsKey<Guid>
    {
        private LANGUAGE language;
        private string text;

        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Language, inherent for this time zone
        /// </summary>
        [DataMember]
        public LANGUAGE Language
        {
            get { return language; }
            set { language = value; }
        }

        /// <summary>
        /// Text, containing the Name of the Time-Zone
        /// </summary>
        [DataMember]
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
            }
        }

        public TZNAME()
        {
        }

        /// <summary>
        /// Constructor, based on the Name of the Time Zone
        /// </summary>
        /// <param name="text"></param>
        public TZNAME(string text, LANGUAGE language = null)
        {
            this.text = text;
            this.language = language;
        }

        public TZNAME(ITZNAME tzname)
        {
            Language = tzname.Language;
            Text = tzname.Text;
        }

        public bool Equals(TZNAME other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TZNAME)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(TZNAME left, TZNAME right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TZNAME left, TZNAME right)
        {
            return !Equals(left, right);
        }

        public int CompareTo(TZNAME other)
        {
            return text.CompareTo(other.Text);
        }

        public static bool operator <(TZNAME a, TZNAME b)
        {
            if ((object)a == null || (object)b == null) return false;
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(TZNAME a, TZNAME b)
        {
            if ((object)a == null || (object)b == null) return false;
            return a.CompareTo(b) > 0;
        }

        /// <summary>
        /// Overloaded ToString method
        /// </summary>
        /// <returns>String Representation of the Time Zone Name property in form of "TZNAME;Language:Text"</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("TZNAME");
            if (language != null) sb.AppendFormat(";{0}", language);
            sb.AppendFormat(":{0}", text);
            return sb.ToString();
        }
    }

    #endregion Time-zone Properties

    #region Relationship Component Properties

    /// <summary>
    /// Defines an "Attendee" within a calendar component
    /// </summary>
    [DataContract]
    public class ATTENDEE : IATTENDEE, IEquatable<ATTENDEE>, IContainsKey<Guid>
    {
        /// <summary>
        /// ID of the current Attendee
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Address of the Current Attendee
        /// </summary>
        [DataMember]
        public URI Address { get; set; }

        /// <summary>
        /// Calendar User Type of the Attendee
        /// </summary>
        [DataMember]
        public CUTYPE CalendarUserType { get; set; }

        /// <summary>
        /// Membership of the Attendee
        /// </summary>
        [DataMember]
        public MEMBER Member { get; set; }

        /// <summary>
        /// Participation Role of the Attendee
        /// </summary>
        [DataMember]
        public ROLE Role { get; set; }

        /// <summary>
        /// Participation Status of the Attendee
        /// </summary>
        [DataMember]
        public PARTSTAT Participation { get; set; }

        /// <summary>
        ///  Rapid Serial Visual Presentation expectation: will be or not
        /// </summary>
        [DataMember]
        public BOOLEAN Rsvp { get; set; }

        /// <summary>
        /// Specifies the Delegate for current Attendee
        /// </summary>
        [DataMember]
        public DELEGATE Delegatee { get; set; }

        /// <summary>
        /// Specifies the Delegator to the current Attendee
        /// </summary>
        [DataMember]
        public DELEGATE Delegator { get; set; }

        /// <summary>
        /// Specifyes the company or community that sends the Attendee
        /// </summary>
        [DataMember]
        public URI SentBy { get; set; }

        /// <summary>
        /// Common name of the Attendee
        /// </summary>
        [DataMember]
        public string CN { get; set; }

        /// <summary>
        /// Specifies a reference to a directory entry associated with the current Attendee
        /// </summary>
        [DataMember]
        public URI Directory { get; set; }

        /// <summary>
        /// Native Language of the current Attendee
        /// </summary>
        [DataMember]
        public LANGUAGE Language { get; set; }

        /// <summary>
        ///
        /// </summary>
        public ATTENDEE()
        {
            Address = null;
            CalendarUserType = CUTYPE.UNKNOWN;
            Member = null;
            Role = ROLE.UNKNOWN;
            Participation = PARTSTAT.UNKNOWN;
            Rsvp = BOOLEAN.UNKNOWN;
            Delegatee = null;
            Delegator = null;
            SentBy = null;
            CN = null;
            Directory = null;
        }

        /// <summary>
        /// Constructor, defining all parameters defining the current Attendee for domestical Puposes (no language specification)
        /// </summary>
        /// <param name="address">Address of the Current Attendee</param>
        /// <param name="cutype">Calendat User Type of the Attendee</param>
        /// <param name="member">Membership type of the Attendee</param>
        /// <param name="role">Participation Role of the Attendee</param>
        /// <param name="partstat">Participation Status of the attendee</param>
        /// <param name="rsvp"> Rapid serial visual presentation expectation</param>
        /// <param name="delegatee">Delegatee for the current Attendee</param>
        /// <param name="delegator">Delegation of the current Attendee</param>
        /// <param name="sentby">Organisation sendin the Current Attendee</param>
        /// <param name="name">Name of the current Attendee</param>
        /// <param name="dir">Reference to a directory entry associated with the current Attendee</param>
        public ATTENDEE(URI address, CUTYPE cutype = CUTYPE.UNKNOWN, ROLE role = ROLE.UNKNOWN,
            PARTSTAT partstat = PARTSTAT.UNKNOWN, BOOLEAN rsvp = BOOLEAN.UNKNOWN, MEMBER member = null, DELEGATE delegatee = null, DELEGATE delegator = null,
            URI sentby = null, string cn = null, URI dir = null, LANGUAGE language = null)
        {
            Address = address;
            CalendarUserType = cutype;
            Member = member;
            Role = role;
            Participation = partstat;
            Rsvp = rsvp;
            Delegatee = delegatee;
            Delegator = delegator;
            SentBy = sentby;
            CN = cn;
            Language = language;
            Directory = dir;
        }

        /// <summary>
        /// Overloaded To String Method
        /// </summary>
        /// <returns>String representation of the Attendee property in form of "ATTENDEE;UserType;RSVP:TRUE(or FALSE);Delegatee;Delegator;SentBy;CN;Directory;Language:Address"</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("ATTENDEE");
            if (CalendarUserType != CUTYPE.UNKNOWN) sb.AppendFormat(";CUTYPE={0}", CalendarUserType);
            if (Member != null) sb.AppendFormat(";{0}", Member);
            if (Role != default(ROLE)) sb.AppendFormat(";ROLE={0}", Role);
            if (Participation != default(PARTSTAT)) sb.AppendFormat(";PARTSTAT={0}", Participation);
            if (Rsvp != BOOLEAN.UNKNOWN) sb.AppendFormat(";RSVP={0}", Rsvp);
            if (Delegatee != null) sb.AppendFormat(";DELEGATED-TO={0}", Delegatee);
            if (Delegator != null) sb.AppendFormat(";DELEGATED-FROM={0}", Delegator);
            if (SentBy != null) sb.AppendFormat("SENT-BY=\"mailto:{0}\"", SentBy);
            if (!string.IsNullOrEmpty(CN)) sb.AppendFormat(";CN={0}", CN);
            if (Directory != null) sb.AppendFormat(";DIR={0}", Directory);
            if (Language != null) sb.AppendFormat(";{0}", Language);
            sb.AppendFormat(":mailto:{0}", Address);
            return sb.ToString();
        }

        public bool Equals(ATTENDEE other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ATTENDEE)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(ATTENDEE left, ATTENDEE right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ATTENDEE left, ATTENDEE right)
        {
            return !Equals(left, right);
        }
    }

    /// <summary>
    /// Defines the Organizer for Calendar Component
    /// </summary>
    [DataContract]
    public class ORGANIZER : IORGANIZER, IEquatable<ORGANIZER>, IContainsKey<Guid>
    {
        /// <summary>
        /// ID of the Organizer
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Address of an Organizer
        /// </summary>
        [DataMember]
        public URI Address { get; set; }

        /// <summary>
        /// Common or Display Name associated with "Organizer"
        /// </summary>
        [DataMember]
        public string CN { get; set; }

        /// <summary>
        /// Directory information associated with "Organizer"
        /// </summary>
        [DataMember]
        public URI Directory { get; set; }

        /// <summary>
        /// Specifies another calendar user that is acting on behalf of the Organizer
        /// </summary>
        [DataMember]
        public URI SentBy { get; set; }

        /// <summary>
        /// Native Language of the Organizer
        /// </summary>
        [DataMember]
        public LANGUAGE Language { get; set; }

        public ORGANIZER()
        {
            Address = null;
            SentBy = null;
            CN = string.Empty;
            Directory = null;
        }

        public ORGANIZER(IORGANIZER organizer, Guid id)
        {
            Id = id;
            Address = organizer.Address;
            CN = organizer.CN;
            Directory = organizer.Directory;
            SentBy = organizer.SentBy;
            Language = organizer.Language;
        }

        public ORGANIZER(URI address, URI sentby, string cname = null, URI dir = null, LANGUAGE language = null)
        {
            Address = address;
            SentBy = sentby;
            CN = cname;
            Directory = dir;
            Language = language;
        }

        /// <summary>
        /// Overloaded ToString Method
        /// </summary>
        /// <returns>String Representation of the Organizer oroperty in form of "ORGANIZER;CN;Directory;SentBy;Language:Address"</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("ORGANIZER");
            if (CN != null) sb.AppendFormat(";CN={0}", CN);
            if (Directory != null) sb.AppendFormat(";{0}", Directory);
            if (SentBy != null) sb.AppendFormat(";SENT-BY=\"mailto:{0}\"", SentBy);
            if (Language != null) sb.AppendFormat(";{0}", Language);
            sb.AppendFormat(":mailto:{0}", Address);
            return sb.ToString();
        }

        public bool Equals(ORGANIZER other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ORGANIZER)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(ORGANIZER left, ORGANIZER right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ORGANIZER left, ORGANIZER right)
        {
            return !Equals(left, right);
        }
    }

    /// <summary>
    /// Identifies a specific instance of a recurring "VEVENT", "VTODO", "VJOURNAL" calendar component
    /// </summary>
    [DataContract]
    public class RECURRENCE_ID : IRECURRENCE_ID, IEquatable<RECURRENCE_ID>, IContainsKey<Guid>
    {
        /// <summary>
        /// Original date time: settable only once
        /// </summary>
        private DATE_TIME value;

        /// <summary>
        /// ID of the Recurrence
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the Time when the original recurrence instance would occur
        /// </summary>
        [DataMember]
        public DATE_TIME Value
        {
            get { return value; }
            set
            {
                this.value = value;
                TimeZoneId = value.TimeZoneId;
            }
        }

        /// <summary>
        /// ID of the  current Time Zone
        /// </summary>
        [DataMember]
        public TZID TimeZoneId { get; set; }

        /// <summary>
        /// Effective Range of te recurrentce instances from the instance specified by the "RECURRENCE_ID"
        /// </summary>
        [DataMember]
        public RANGE Range { get; set; }

        public RECURRENCE_ID()
        {
            Value = default(DATE_TIME);
            TimeZoneId = null;
            Range = RANGE.UNKNOWN;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"> Gets or sets the Time when the original recurrence instance would occur</param>
        /// <param name="tzid">ID of the  current Time Zone</param>
        public RECURRENCE_ID(DATE_TIME value, RANGE range = RANGE.THISANDFUTURE)
        {
            Value = value;
            Range = range;
        }

        /// <summary>
        /// Overloaded ToStringMethod
        /// </summary>
        /// <returns>String representation of the Recurrence property in form of "RECURRENCE-ID;TimeZoneId;Range:DateTime"</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("RECURRENCE-ID");
            if (TimeZoneId != null) sb.AppendFormat(";{0}", TimeZoneId);
            if (Range != RANGE.UNKNOWN) sb.AppendFormat(";RANGE={0}", Range);
            sb.AppendFormat(":{0}", Value);
            return sb.ToString();
        }

        public bool Equals(RECURRENCE_ID other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((RECURRENCE_ID)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(RECURRENCE_ID left, RECURRENCE_ID right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RECURRENCE_ID left, RECURRENCE_ID right)
        {
            return !Equals(left, right);
        }
    }

    /// <summary>
    /// Represents the relationship or reference between one calendar component and another
    /// </summary>
    [DataContract]
    public class RELATEDTO : IRELATEDTO, IEquatable<RELATEDTO>, IContainsKey<Guid>
    {
        /// <summary>
        /// ID of the Relationship or Reference between one calendar component and another
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Description of the relationship or reference to another Calendar component
        /// </summary>
        [DataMember]
        public string Reference { get; set; }

        /// <summary>
        /// Type of the relationship: dafault - PARENT, can be: CHILD and SIBLING
        /// </summary>
        [DataMember]
        public RELTYPE RelationshipType { get; set; }

        public RELATEDTO()
        {
            Reference = null;
            RelationshipType = RELTYPE.UNKNOWN;
        }

        /// <summary>
        /// Constructor specifying the Text and Type properties of the Related_To property
        /// </summary>
        /// <param name="text">Description of the relationship or reference to another Calendar component</param>
        /// <param name="type"> Type of the relationship: dafault - PARENT, can be: CHILD and SIBLING</param>
        public RELATEDTO(string text, RELTYPE type = RELTYPE.UNKNOWN)
        {
            Reference = text;
            RelationshipType = type;
        }

        /// <summary>
        /// Overloaded ToString Method
        /// </summary>
        /// <returns>String Representation of the Related_To property in form of "RELATED-TO;Relationship:Text"</returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Reference)) return string.Empty;
            var sb = new StringBuilder();
            sb.Append("RELATED-TO");
            if (RelationshipType != RELTYPE.UNKNOWN) sb.AppendFormat(";{0}", RelationshipType);
            sb.AppendFormat(":{0}", Reference);
            return sb.ToString();
        }

        public bool Equals(RELATEDTO other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((RELATEDTO)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(RELATEDTO left, RELATEDTO right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RELATEDTO left, RELATEDTO right)
        {
            return !Equals(left, right);
        }
    }

    #endregion Relationship Component Properties

    #region Recurrence Component properties

    /// <summary>
    /// Defines the list of DATE-TIME exceptions for recurring events, to-dos, entries, or time zone definitions
    /// </summary>
    [DataContract]
    public class EXDATE : IEXDATE, IEquatable<EXDATE>, IContainsKey<Guid>
    {
        /// <summary>
        ///  ID of the Date-Time Exception
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Set of Dates of exceptions in a Recurrence
        /// </summary>
        [DataMember]
        public List<DATE_TIME> DateTimes { get; set; }

        [DataMember]
        public TZID TimeZoneId { get; set; }

        /// <summary>
        /// Format of the Exception Date
        /// </summary>
        [DataMember]
        public VALUE ValueType { get; set; }

        public EXDATE()
        {
            DateTimes = new List<DATE_TIME>();
            TimeZoneId = null;
            ValueType = VALUE.UNKNOWN;
        }

        /// <summary>
        /// Constructor based on the set of Dates of Recurrence Exceptions and ID of the Time Zone
        /// </summary>
        /// <param name="values">Set of Dates of Recurrence Exceptions</param>
        /// <param name="tzid"></param>
        public EXDATE(List<DATE_TIME> values, TZID tzid, VALUE value_type = VALUE.UNKNOWN)
        {
            DateTimes = values;
            TimeZoneId = tzid;
            ValueType = value_type;
        }

        /// <summary>
        /// Overloaded ToString Method
        /// </summary>
        /// <returns>String representation of the EXDATE property in form of "EXDATE;TimeZone:comma splited dates of exctptions"</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("EXDATE");
            if (ValueType != VALUE.UNKNOWN) sb.AppendFormat(";VALUE={1}", ValueType);
            else if (TimeZoneId != null) sb.AppendFormat(";{0}", TimeZoneId);
            sb.Append(":"); var last = DateTimes.Last();
            foreach (var datetime in DateTimes)
            {
                if (datetime != last) sb.AppendFormat("{0}, ", datetime);
                else sb.AppendFormat("{0}", datetime);
            }
            return sb.ToString();
        }

        public bool Equals(EXDATE other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((EXDATE)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(EXDATE left, EXDATE right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EXDATE left, EXDATE right)
        {
            return !Equals(left, right);
        }
    }

    /// <summary>
    /// Defines the list of DATE-TIME values for recurring events, to-dos, journal entries or time-zone definitions
    /// </summary>
    [DataContract]
    public class RDATE : IRDATE, IEquatable<RDATE>, IContainsKey<Guid>
    {
        /// <summary>
        /// ID of the list of DATE-TIME values for recurring events, to-dos, journal entries or time-zone definitions
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// List of DATE-TIME values for recurring events, to-dos, journal entries or time-zone definitions
        /// </summary>
        [DataMember]
        public List<DATE_TIME> DateTimes { get; set; }

        /// <summary>
        /// List of Periods between recurring events, to-dos, journal entries or time-zone definitions
        /// </summary>
        [DataMember]
        public List<PERIOD> Periods { get; set; }

        /// <summary>
        /// ID of the current time zone
        /// </summary>
        [DataMember]
        public TZID TimeZoneId { get; set; }

        /// <summary>
        /// Mode of the recurrence: events, to-dos, journal entries or time-zone definitions
        /// </summary>
        [DataMember]
        public VALUE ValueType { get; set; }

        public RDATE()
        {
            DateTimes = new List<DATE_TIME>();
            TimeZoneId = null;
            Periods = new List<PERIOD>();
            ValueType = VALUE.UNKNOWN;
        }

        /// <summary>
        /// Constructor based on the list of DATE-TIME values and the ID of the current Time Zone
        /// </summary>
        /// <param name="values">List of DATE-TIME values for recurring events, to-dos, journal entries or time-zone definitions</param>
        /// <param name="tzid">ID of the current Time Zone</param>
        public RDATE(List<DATE_TIME> values, TZID tzid = null, VALUE value_type = VALUE.UNKNOWN)
        {
            DateTimes = values;
            TimeZoneId = tzid;
            Periods = new List<PERIOD>();
            ValueType = value_type;
        }

        /// <summary>
        /// Constructor based on the list of periods and the ID of currtnt time zone
        /// </summary>
        /// <param name="periods">List of Periods between recurring events, to-dos, journal entries or time-zone definitions</param>
        /// <param name="tzid">ID of the current Time Zone</param>
        public RDATE(List<PERIOD> periods, TZID tzid = null, VALUE value_type = VALUE.UNKNOWN)
        {
            DateTimes = new List<DATE_TIME>();
            Periods = periods;
            TimeZoneId = tzid;
            ValueType = value_type;
        }

        /// <summary>
        /// Overloaded ToString Method
        /// </summary>
        /// <returns>String representation of the RDATE property in form of "RDATE;VALUE=Mode;TimeZoneId:coma separated list of DATE-TIME values(or Periods)"</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("RDATE");
            if (ValueType != VALUE.UNKNOWN) sb.AppendFormat(";VALUE={0}", ValueType);
            else if (TimeZoneId != null) sb.AppendFormat(";{0}", TimeZoneId);
            sb.AppendFormat(":");
            if (!DateTimes.NullOrEmpty())
            {
                var last = DateTimes.Last();
                foreach (var datetime in DateTimes)
                {
                    if (datetime != last) sb.AppendFormat("{0}, ", datetime);
                    else sb.AppendFormat("{0}", datetime);
                }
            }
            else if (!Periods.NullOrEmpty())
            {
                var last = Periods.Last();
                foreach (var period in Periods)
                {
                    if (period != last) sb.AppendFormat("{0}, ", period);
                    else sb.AppendFormat("{0}", period);
                }
            }
            return sb.ToString();
        }

        public bool Equals(RDATE other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((RDATE)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(RDATE left, RDATE right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RDATE left, RDATE right)
        {
            return !Equals(left, right);
        }
    }

    #endregion Recurrence Component properties

    #region Alarm Component Properties

    /// <summary>
    /// Specifies, when the alarm will trigger
    /// </summary>
    [DataContract]
    public class TRIGGER : ITRIGGER, IEquatable<TRIGGER>, IContainsKey<Guid>
    {
        private readonly TriggerType type;

        /// <summary>
        /// ID of the trigger
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Duration between two alarm actions
        /// </summary>
        [DataMember]
        public DURATION Duration { get; set; }

        /// <summary>
        /// Date-Time, when the Alarm action must be invoked
        /// </summary>
        [DataMember]
        public DATE_TIME DateTime { get; set; }

        /// <summary>
        /// Action value mode, for example "AUDIO", "DISPLAY", "EMAIL"
        /// </summary>
        [DataMember]
        public VALUE Value { get; set; }

        [DataMember]
        public RELATED Related { get; set; }

        public TRIGGER()
        {
            type = TriggerType.Related;
        }

        public TRIGGER(DURATION duration, RELATED related = RELATED.UNKNOWN, VALUE value = VALUE.UNKNOWN)
        {
            Duration = duration;
            Related = related;
            Value = value;
            type = TriggerType.Related;
        }

        public TRIGGER(DATE_TIME datetime, VALUE value = VALUE.UNKNOWN)
        {
            Value = value;
            DateTime = datetime;
            type = TriggerType.Absolute;
        }

        /// <summary>
        /// Overloaded ToString Method
        /// </summary>
        /// <returns>String Representation of the Trigger property in Form of "TRIGGER;VALUE=Mode:Dutarion(or DateTime)"</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("TRIGGER");
            if (type == TriggerType.Related)
            {
                if (Value == VALUE.DURATION) sb.AppendFormat(";VALUE={0}", Value);
                else if (Related != default(RELATED)) sb.AppendFormat(";RELATED={0}", Related);
                sb.AppendFormat(":{0}", Duration);
            }
            else
            {
                if (Value == VALUE.DATE_TIME) sb.AppendFormat(";VALUE={0}", Value);
                sb.AppendFormat(":{0}", DateTime);
            }
            return sb.ToString();
        }

        public bool Equals(TRIGGER other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TRIGGER)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(TRIGGER left, TRIGGER right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TRIGGER left, TRIGGER right)
        {
            return !Equals(left, right);
        }
    }

    #endregion Alarm Component Properties

    #region Miscellaneous Component Properties

    /// <summary>
    /// Internet Assigned Numbers Authority properties
    /// </summary>
    /// <typeparam name="TValue">Specific Value Type</typeparam>
    [DataContract]
    [KnownType(typeof(IANA_PROPERTY))]
    [KnownType(typeof(X_PROPERTY))]
    [KnownType(typeof(TZID))]
    [KnownType(typeof(FMTTYPE))]
    [KnownType(typeof(LANGUAGE))]
    [KnownType(typeof(DELEGATE))]
    [KnownType(typeof(MEMBER))]
    public abstract class MISC_PROPERTY<TValue> : IMISC_PROPERTY<TValue>, IContainsKey<Guid>, IEquatable<MISC_PROPERTY<TValue>>
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// The value assigned to the IANA-Property
        /// </summary>
        [DataMember]
        public TValue Value { get; set; }

        /// <summary>
        /// List of any nescessary parameters
        /// </summary>
        [DataMember]
        [Ignore]
        public List<IPARAMETER> Parameters { get; set; }

        [DataMember]
        public VALUE ValueType { get; set; }

        protected MISC_PROPERTY()
        {
        }

        /// <summary>
        /// Constructor based on the value of the IANA-Property
        /// </summary>
        /// <param name="value"> The value assigned to the IANA-Property</param>
        protected MISC_PROPERTY(string name, TValue value, List<IPARAMETER> parameters)
        {
            Name = name;
            Value = value;
            Parameters = parameters;
        }

        /// <summary>
        /// Overloaded ToString method
        /// </summary>
        /// <returns>String Representation of the IANA-Property in form of "Key;semicolon separated Parameters:value"</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Name);
            foreach (var parameter in Parameters) sb.AppendFormat(";{0}", parameter);
            if (ValueType == VALUE.UNKNOWN) sb.AppendFormat(":{0}", Value);
            else sb.AppendFormat("VALUE={0}:{1}", ValueType, Value);
            return sb.ToString();
        }

        public bool Equals(MISC_PROPERTY<TValue> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MISC_PROPERTY<TValue>)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(MISC_PROPERTY<TValue> left, MISC_PROPERTY<TValue> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MISC_PROPERTY<TValue> left, MISC_PROPERTY<TValue> right)
        {
            return !Equals(left, right);
        }
    }

    [DataContract]
    public class IANA_PROPERTY : MISC_PROPERTY<object>
    {
        public IANA_PROPERTY()
        {
        }

        public IANA_PROPERTY(string name, object value, List<IPARAMETER> parameters)
            : base(name, value, parameters)
        {
        }
    }

    [DataContract]
    public class X_PROPERTY : MISC_PROPERTY<object>
    {
        public X_PROPERTY()
        {
        }

        public X_PROPERTY(string name, object value, List<IPARAMETER> parameters)
            : base(name, value, parameters)
        {
        }
    }

    [DataContract]
    public struct STATCODE : ISTATCODE, IEquatable<STATCODE>, IComparable<STATCODE>, IContainsKey<Guid>
    {
        private Guid id;
        private uint l1;
        private uint l2;
        private uint l3;

        [DataMember]
        public Guid Id
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember]
        public uint L1
        {
            get { return l1; }
            set { l1 = value; }
        }

        [DataMember]
        public uint L2
        {
            get { return l2; }
            set { l2 = value; }
        }

        [DataMember]
        public uint L3
        {
            get { return l3; }
            set { l3 = value; }
        }

        public STATCODE(uint l1, uint l2 = 0, uint l3 = 0)
            : this()
        {
            id = Guid.Empty;

            this.l1 = l1;
            this.l2 = l2;
            this.l3 = l3;
        }

        public STATCODE(string value)
        {
            id = Guid.Empty;
            this.l1 = this.l2 = this.l3 = default(uint);

            //todo: Parser here
        }

        public bool Equals(STATCODE other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is STATCODE && Equals((STATCODE)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(STATCODE left, STATCODE right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(STATCODE left, STATCODE right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(STATCODE other)
        {
            return L1.CompareTo(other.L1) +
                L2.CompareTo(other.L2) +
                L3.CompareTo(other.L3);
        }

        public override string ToString()
        {
            return (L3 != 0) ?
                string.Format("{0:D1}.{1:D1}.{2:D1}", L1, L2, L3) :
                string.Format("{0:D1}.{1:D1}", L1, L2);
        }

        public static bool operator <(STATCODE a, STATCODE b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(STATCODE a, STATCODE b)
        {
            return a.CompareTo(b) > 0;
        }
    }

    /// <summary>
    /// Defines the status code returned for a scheduling request
    /// </summary>
    [DataContract]
    public class REQUEST_STATUS : IREQUEST_STATUS, IContainsKey<Guid>, IEquatable<REQUEST_STATUS>
    {
        /// <summary>
        /// ID of the status code
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Status code returned for a scheduling request
        /// </summary>
        [DataMember]
        public ISTATCODE Code { get; set; }

        /// <summary>
        /// Description of the status (what does the code mean)
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Exception data, received with current request
        /// </summary>
        [DataMember]
        public string ExceptionData { get; set; }

        /// <summary>
        /// Language for current request
        /// </summary>
        [DataMember]
        public LANGUAGE Language { get; set; }

        public REQUEST_STATUS()
        {
            Code = null;
            Description = null;
            ExceptionData = null;
            Language = null;
        }

        /// <summary>
        /// Constructor, specifying the status code, description, exception data and the language
        /// </summary>
        /// <param name="code">Status code returned for a scheduling request</param>
        /// <param name="description">Description of the status (what does the code mean)</param>
        /// <param name="exception">Exception data, received with current request</param>
        /// <param name="language">Language used for the request</param>
        public REQUEST_STATUS(STATCODE code, string description, string exception = null, LANGUAGE language = null)
        {
            Code = code;
            Description = description;
            ExceptionData = exception;
            Language = language;
        }

        /// <summary>
        /// Overloaded ToString method
        /// </summary>
        /// <returns>String representation of the REQUEST_STATUS property in form of "REQUEST-STQTUS;Lanquage:Code;Description;ExceptionData"</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("REQUEST-STATUS");
            if (Language != null) sb.AppendFormat(";{0}", Language);
            sb.AppendFormat(":{0}", Code);
            sb.AppendFormat(";{0}", Description.EscapeStrings());
            if (!string.IsNullOrEmpty(ExceptionData)) sb.AppendFormat(";{0}", ExceptionData);
            return sb.ToString();
        }

        public bool Equals(REQUEST_STATUS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((REQUEST_STATUS)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(REQUEST_STATUS left, REQUEST_STATUS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REQUEST_STATUS left, REQUEST_STATUS right)
        {
            return !Equals(left, right);
        }
    }

    #endregion Miscellaneous Component Properties
}