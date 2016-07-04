using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.extensions;
using reexjungle.xcal.infrastructure.contracts;
using reexjungle.xcal.infrastructure.extensions;
using reexjungle.xcal.infrastructure.serialization;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.foundation.contracts;
using ServiceStack.DataAnnotations;

namespace reexjungle.xcal.domain.models
{
    #region Descriptive Properties

    /// <summary>
    /// Specifies a contract for attaching an inline binary encooded content information.
    /// </summary>
    [DataContract]
    public abstract class ATTACH : IATTACH, IContainsKey<Guid>, ICalendarSerializable
    {
        /// <summary>
        /// ID of an AttachmentBinary for a particular Calendar component
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Binary encoded content of the attachment
        /// </summary>
        [DataMember]
        public string Content { get; set; }

        /// <summary>
        /// Media Type of the resource in an Attachmant
        /// </summary>
        [DataMember]
        public FMTTYPE FormatType { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        protected ATTACH()
        {
            FormatType = null;
            Content = null;
        }

        protected ATTACH(IATTACH attachment)
        {
            if (attachment != null)
            {
                FormatType = new FMTTYPE(attachment.FormatType);
                Content = attachment.Content;
            }
        }

        protected ATTACH(string base64, FMTTYPE format = null)
        {
            Content = base64;
            FormatType = format;
        }

        protected ATTACH(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            var pattern =
                @"^ATTACH(;(?<fmttype>FMTTYPE=\w+/\w+\S*))?;(?<encoding>ENCODING=BASE64;VALUE=BINARY):(?<binary>(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?)$";

            var options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                          RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(value, pattern, options)) throw new FormatException("value");
            foreach (Match match in regex.Matches(value))
            {
                if (match.Groups["fmttype"].Success) FormatType = new FMTTYPE(match.Groups["fmttype"].Value);
                if (match.Groups["binary"].Success) Content = match.Groups["binary"].Value;
            }
        }

        public abstract void WriteCalendar(CalendarWriter writer);

        public abstract void ReadCalendar(CalendarReader reader);

        public bool CanSerialize() => !string.IsNullOrEmpty(Content) && !string.IsNullOrWhiteSpace(Content);
    }

    public sealed class ATTACH_BINARY : ATTACH, IEquatable<ATTACH_BINARY>
    {
        public ATTACH_BINARY()
        {
        }

        public ATTACH_BINARY(IATTACH attachment) : base(attachment)
        {
        }

        public ATTACH_BINARY(string base64, FMTTYPE format = null) : base(base64, format)
        {
        }

        public override void WriteCalendar(CalendarWriter writer)
        {
            writer.Write("ATTACH");
            if (FormatType != null) writer.AppendParameter(FormatType);
            writer.AppendParameter("ENCODING", ENCODING.BASE64.ToString());
            writer.AppendPropertyValue(Content);
        }

        public override void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ATTACH_BINARY other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is ATTACH_BINARY && Equals((ATTACH_BINARY)obj);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
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

    public sealed class ATTACH_URI : ATTACH, IEquatable<ATTACH_URI>
    {
        public ATTACH_URI()
        {
        }

        public ATTACH_URI(IATTACH attachment) : base(attachment)
        {
        }

        public ATTACH_URI(Uri uri, FMTTYPE format = null) : base(uri.ToString(), format)
        {
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ATTACH_URI other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ATTACH_URI)obj);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
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

        public override void WriteCalendar(CalendarWriter writer)
        {
            writer.Write("ATTACH");
            if (FormatType != null) writer.AppendParameter(FormatType);
            writer.AppendPropertyValue(Content);
        }

        public override void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Defines the categories or subtypes for a calendar component
    /// </summary>
    [DataContract]
    public class CATEGORIES : ICATEGORIES, IEquatable<CATEGORIES>, IContainsKey<Guid>, ICalendarSerializable
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
        }

        public CATEGORIES(ICATEGORIES categories)
        {
            if (categories != null)
            {
                Language = categories.Language;
                Values = categories.Values.NullOrEmpty()
                    ? new List<string>()
                    : new List<string>(categories.Values);
            }
        }

        /// <summary>
        /// Constructor based on the TEXT and LANGUAGE properties of the class
        /// </summary>
        /// <param name="text">Description of the calendar component category or subtype</param>
        /// <param name="language">Language, used in a given category of calendar components</param>
        public CATEGORIES(IEnumerable<string> values, LANGUAGE language = null)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            Language = language;
            Values = values.NullOrEmpty()
                ? new List<string>()
                : new List<string>(values);
        }

        public CATEGORIES(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            var pattern = @"^CATEGORIES(;(?<language>LANGUAGE=\w{2}-\w{2}))?:(?<text>(\w+\s*,?)+)$";
            var options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                          RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(value, pattern, options)) throw new FormatException("value");
            foreach (Match match in regex.Matches(value))
            {
                if (match.Groups["lang"].Success) Language = new LANGUAGE(match.Groups["lang"].Value);
                if (match.Groups["text"].Success)
                {
                    var lines = match.Groups["text"].Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    Values = new List<string>(lines);
                }
            }
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

        public void WriteCalendar(CalendarWriter writer)
        {
            writer.Write("CATEGORIES");
            if (Language != null) writer.AppendParameter(Language);
            writer.AppendByComma(Values);
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public bool CanSerialize() => Values.Any();
    }

    /// <summary>
    /// Specify non-processing information intended to provide a comment to the calendar user
    /// </summary>
    [DataContract]
    public abstract class TEXTUAL : ITEXTUAL, IEquatable<TEXTUAL>, IComparable<TEXTUAL>, IContainsKey<Guid>, ICalendarSerializable
    {
        /// <summary>
        /// ID of the Comment to the Calendar Component
        /// </summary>
        [DataMember]
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
        public ALTREP AlternativeText { get; set; }

        /// <summary>
        /// Languge of the Comment
        /// </summary>
        [DataMember]
        public LANGUAGE Language { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        protected TEXTUAL()
        {
            Text = string.Empty;
            AlternativeText = null;
            Language = null;
        }

        protected TEXTUAL(ITEXTUAL value)
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
        protected TEXTUAL(string text, ALTREP altrep, LANGUAGE language = null)
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

        public virtual void WriteCalendar(CalendarWriter writer)
        {
            if (AlternativeText != null) writer.AppendParameter(AlternativeText);
            if (Language != null) writer.AppendParameter(Language);
            writer.AppendPropertyValue(writer.ConvertToSAFE_STRING(Text));
        }

        public abstract void ReadCalendar(CalendarReader reader);

        public bool CanSerialize() => string.IsNullOrEmpty(Text) || string.IsNullOrWhiteSpace(Text);
    }

    [DataContract]
    public sealed class DESCRIPTION : TEXTUAL
    {
        public DESCRIPTION()
        {
        }

        public DESCRIPTION(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));

            var pattern =
                @"^DESCRIPTION(;(?<altrep>ALTREP=\p{P}\s*(\w+:\S+)\s*\p{P}))?(;(?<lang>LANGUAGE=\p{L}{2}\-\p{L}{2}))?:(?<text>(\P{L}*\p{L}\p{M}*\P{L}*)+)$";
            var options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                          RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(value, pattern, options)) throw new FormatException("value");
            foreach (Match match in regex.Matches(value))
            {
                if (match.Groups["lang"].Success)
                {
                    Language = new LANGUAGE(match.Groups["lang"].Value);
                }

                if (match.Groups["altrep"].Success)
                {
                    AlternativeText = new ALTREP(match.Groups["altrep"].Value);
                }
                if (match.Groups["text"].Success)
                {
                    Text = match.Groups["text"].Value;
                }
            }
        }

        public DESCRIPTION(string text, ALTREP altrep, LANGUAGE language = null)
            : base(text, altrep, language)
        {
        }

        public DESCRIPTION(DESCRIPTION other)
        {
            if (other != null)
            {
                if (other.Text != null)
                    Text = string.Copy(other.Text);

                if (other.AlternativeText != null)
                    AlternativeText = new ALTREP(other.AlternativeText);

                if (other.Language != null)
                    Language = new LANGUAGE(other.Language);
            }
        }

        public override void WriteCalendar(CalendarWriter writer)
        {
            writer.Write("DESCRIPTION");
            base.WriteCalendar(writer);
        }

        public override void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    public sealed class COMMENT : TEXTUAL
    {
        public COMMENT()
        {
        }

        public COMMENT(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));

            var pattern =
                @"^COMMENT(;(?<altrep>ALTREP=\p{P}\s*(\w+:\S+)\s*\p{P}))?(;(?<lang>LANGUAGE=\p{L}{2}\-\p{L}{2}))?:(?<text>(\P{L}*\p{L}\p{M}*\P{L}*)+)$";
            var options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                          RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);
            if (Regex.IsMatch(value, pattern, options))
            {
                foreach (Match match in regex.Matches(value))
                {
                    if (match.Groups["lang"].Success)
                    {
                        Language = new LANGUAGE(match.Groups["lang"].Value);
                    }

                    if (match.Groups["altrep"].Success)
                    {
                        AlternativeText = new ALTREP(match.Groups["altrep"].Value);
                    }
                    if (match.Groups["text"].Success)
                    {
                        Text = match.Groups["text"].Value;
                    }
                }
            }
        }

        public COMMENT(string text, ALTREP altrep, LANGUAGE language = null)
            : base(text, altrep, language)
        {
        }

        public COMMENT(COMMENT other)
        {
            if (other != null)
            {
                if (other.Text != null)
                    Text = string.Copy(other.Text);

                if (other.AlternativeText != null)
                    AlternativeText = new ALTREP(other.AlternativeText);

                if (other.Language != null)
                    Language = new LANGUAGE(other.Language);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder("COMMENT");
            if (AlternativeText != null) sb.AppendFormat(";{0}", AlternativeText);
            if (Language != null) sb.AppendFormat(";{0}", Language);
            sb.AppendFormat(":{0}", Text.EscapeStrings());
            return sb.ToString();
        }

        public override void WriteCalendar(CalendarWriter writer)
        {
            writer.Write("COMMENT");
            base.WriteCalendar(writer);
        }

        public override void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    public sealed class CONTACT : TEXTUAL
    {
        public CONTACT()
        {
        }

        public CONTACT(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));

            var pattern =
                @"^CONTACT(;(?<altrep>ALTREP=\p{P}\s*(\w+:\S+)\s*\p{P}))?(;(?<lang>LANGUAGE=\p{L}{2}\-\p{L}{2}))?:(?<text>(\P{L}*\p{L}\p{M}*\P{L}*)+)$";
            var options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                          RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);
            if (Regex.IsMatch(value, pattern, options))
            {
                foreach (Match match in regex.Matches(value))
                {
                    if (match.Groups["lang"].Success)
                    {
                        Language = new LANGUAGE(match.Groups["lang"].Value);
                    }

                    if (match.Groups["altrep"].Success)
                    {
                        AlternativeText = new ALTREP(match.Groups["altrep"].Value);
                    }
                    if (match.Groups["text"].Success)
                    {
                        Text = match.Groups["text"].Value;
                    }
                }
            }
        }

        public CONTACT(string text, ALTREP altrep, LANGUAGE language = null)
            : base(text, altrep, language)
        {
        }

        public CONTACT(CONTACT other)
        {
            if (other != null)
            {
                if (other.Text != null)
                    Text = string.Copy(other.Text);

                if (other.AlternativeText != null)
                    AlternativeText = new ALTREP(other.AlternativeText);

                if (other.Language != null)
                    Language = new LANGUAGE(other.Language);
            }
        }

        public override void WriteCalendar(CalendarWriter writer)
        {
            writer.Write("CONTACT");
            base.WriteCalendar(writer);
        }

        public override void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    public sealed class SUMMARY : TEXTUAL
    {
        public SUMMARY()
        {
        }

        public SUMMARY(SUMMARY other)
        {
            if (other != null)
            {
                if (other.Text != null)
                    Text = string.Copy(other.Text);

                if (other.AlternativeText != null)
                    AlternativeText = new ALTREP(other.AlternativeText);

                if (other.Language != null)
                    Language = new LANGUAGE(other.Language);
            }
        }

        public SUMMARY(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));

            var pattern =
                @"^SUMMARY(;(?<altrep>ALTREP=\p{P}\s*(\w+:\S+)\s*\p{P}))?(;(?<lang>LANGUAGE=\p{L}{2}\-\p{L}{2}))?:(?<text>(\P{L}*\p{L}\p{M}*\P{L}*)+)$";
            var options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                          RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);
            if (Regex.IsMatch(value, pattern, options))
            {
                foreach (Match match in regex.Matches(value))
                {
                    if (match.Groups["lang"].Success)
                    {
                        Language = new LANGUAGE(match.Groups["lang"].Value);
                    }

                    if (match.Groups["altrep"].Success)
                    {
                        AlternativeText = new ALTREP(match.Groups["altrep"].Value);
                    }
                    if (match.Groups["text"].Success)
                    {
                        Text = match.Groups["text"].Value;
                    }
                }
            }
        }

        public SUMMARY(string text, ALTREP altrep, LANGUAGE language = null)
            : base(text, altrep, language)
        {
        }

        public override void WriteCalendar(CalendarWriter writer)
        {
            writer.Write("SUMMARY");
            base.WriteCalendar(writer);
        }

        public override void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    public class LOCATION : TEXTUAL
    {
        public LOCATION()
        {
        }

        public LOCATION(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));

            var pattern =
                @"^LOCATION(;(?<altrep>ALTREP=\p{P}\s*(\w+:\S+)\s*\p{P}))?(;(?<lang>LANGUAGE=\p{L}{2}\-\p{L}{2}))?:(?<text>(\P{L}*\p{L}\p{M}*\P{L}*)+)$";
            var options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                          RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);
            if (Regex.IsMatch(value, pattern, options))
            {
                foreach (Match match in regex.Matches(value))
                {
                    if (match.Groups["lang"].Success)
                    {
                        Language = new LANGUAGE(match.Groups["lang"].Value);
                    }

                    if (match.Groups["altrep"].Success)
                    {
                        AlternativeText = new ALTREP(match.Groups["altrep"].Value);
                    }
                    if (match.Groups["text"].Success)
                    {
                        Text = match.Groups["text"].Value;
                    }
                }
            }
        }

        public LOCATION(string text, ALTREP altrep, LANGUAGE language = null)
            : base(text, altrep, language)
        {
        }

        public LOCATION(LOCATION other)
        {
            if (other != null)
            {
                if (other.Text != null)
                    Text = string.Copy(other.Text);

                if (other.AlternativeText != null)
                    AlternativeText = new ALTREP(other.AlternativeText);

                if (other.Language != null)
                    Language = new LANGUAGE(other.Language);
            }
        }

        public override void WriteCalendar(CalendarWriter writer)
        {
            writer.Write("LOCATION");
            base.WriteCalendar(writer);
        }

        public override void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Specifies the information related to the global position for the activity spiecified by a
    /// calendar component
    /// </summary>
    [DataContract]
    public struct GEO : IGEO, IEquatable<GEO>, IContainsKey<Guid>, ICalendarSerializable
    {
        private float longitude;
        private float latitude;
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
            set { longitude = value; }
        }

        /// <summary>
        /// Lattitude of the Geographical Position
        /// </summary>
        [DataMember]
        public float Latitude
        {
            get { return latitude; }
            set { latitude = value; }
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

            var pattern =
                @"^(?<lon>[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)(?<lat>[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)$";
            if (Regex.IsMatch(value, pattern,
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
            {
                foreach (Match match in Regex.Matches(value, pattern,
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
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

        public void WriteCalendar(CalendarWriter writer)
        {
            writer.WriteProperty("GEO", $"{Latitude};{Longitude}");
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public bool CanSerialize() => true;
    }

    /// <summary>
    /// Defines the Equipment or resources anticipated for an activity specified by a calendar component
    /// </summary>
    [DataContract]
    public class RESOURCES : IRESOURCES, IEquatable<RESOURCES>, IContainsKey<Guid>, ICalendarSerializable
    {
        /// <summary>
        /// ID of a particular Resource
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Alternative Text for a specified Resource, can contain the Description of this resource
        /// </summary>
        [DataMember]
        public ALTREP AlternativeText { get; set; }

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

        public RESOURCES()
        {
            Values = new List<string>();
        }

        public RESOURCES(IRESOURCES resources)
        {
            if (resources == null) throw new ArgumentNullException(nameof(resources));
            AlternativeText = resources.AlternativeText;
            Language = resources.Language;
            Values = new List<string>(resources.Values);
        }

        /// <summary>
        /// Constructor specifying the Text, Alternative Text and Language for Resource Property
        /// </summary>
        /// <param name="values"></param>
        /// <param name="alt">
        /// Alternative Text, can represent particular the description of the Resource
        /// </param>
        /// <param name="language">Language nescessary for the Resource</param>
        public RESOURCES(IEnumerable<string> values, ALTREP alt = null, LANGUAGE language = null)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (alt == null) throw new ArgumentNullException(nameof(alt));
            if (language == null) throw new ArgumentNullException(nameof(language));

            AlternativeText = alt;
            Language = language;

            Values = values.NullOrEmpty()
                ? new List<string>()
                : new List<string>(values);
        }

        public RESOURCES(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            var pattern =
                @"^RESOURCES(;(?<altrep>ALTREP=\p{P}\s*(\w+:\S+)\s*\p{P}))?(;(?<lang>LANGUAGE=\p{L}{2}\-\p{L}{2}))?:(?<text>(\P{L}*\p{L}\p{M}*\P{L}*)+)$";
            var options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                          RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(value, pattern, options)) throw new FormatException("value");

            foreach (Match match in regex.Matches(value))
            {
                if (match.Groups["lang"].Success)
                {
                    Language = new LANGUAGE(match.Groups["lang"].Value);
                }

                if (match.Groups["altrep"].Success)
                {
                    AlternativeText = new ALTREP(match.Groups["altrep"].Value);
                }
                if (match.Groups["text"].Success)
                {
                    var lines = match.Groups["text"].Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    Values = new List<string>(lines);
                }
            }
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

        public void WriteCalendar(CalendarWriter writer)
        {
            writer.Write("RESOURCES");
            if (AlternativeText != null) writer.AppendParameter(AlternativeText);
            if (Language != null) writer.AppendParameter(Language);
            writer.AppendPropertyValues(Values.Select(writer.ConvertToSAFE_STRING));
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public bool CanSerialize() => Values.Any();
    }

    [DataContract]
    public struct PRIORITY : IPRIORITY, IEquatable<PRIORITY>, IComparable<PRIORITY>, ICalendarSerializable
    {
        private readonly int value;
        private readonly PRIORITYLEVEL level;
        private readonly PRIORITYSCHEMA schema;
        private readonly PriorityType format;

        private int LevelToValue(PRIORITYLEVEL level)
        {
            var val = -1;
            switch (level)
            {
                case PRIORITYLEVEL.NONE:
                    val = 0;
                    break;

                case PRIORITYLEVEL.LOW:
                    val = 9;
                    break;

                case PRIORITYLEVEL.MEDIUM:
                    val = 5;
                    break;

                case PRIORITYLEVEL.HIGH:
                    val = 1;
                    break;
            }
            return val;
        }

        private PRIORITYLEVEL ValueToLevel(int value)
        {
            if (value >= 1 && this.value <= 4) return PRIORITYLEVEL.HIGH;
            if (value == 5) return PRIORITYLEVEL.MEDIUM;
            if (value >= 6 && value <= 9) return PRIORITYLEVEL.LOW;
            return PRIORITYLEVEL.NONE;
        }

        private int SchemaToValue(PRIORITYSCHEMA schema)
        {
            var val = 0;
            switch (schema)
            {
                case PRIORITYSCHEMA.NONE:
                    val = 0;
                    break;

                case PRIORITYSCHEMA.C3:
                    val = 9;
                    break;

                case PRIORITYSCHEMA.C2:
                    val = 8;
                    break;

                case PRIORITYSCHEMA.C1:
                    val = 7;
                    break;

                case PRIORITYSCHEMA.B3:
                    val = 6;
                    break;

                case PRIORITYSCHEMA.B2:
                    val = 5;
                    break;

                case PRIORITYSCHEMA.B1:
                    val = 4;
                    break;

                case PRIORITYSCHEMA.A3:
                    val = 3;
                    break;

                case PRIORITYSCHEMA.A2:
                    val = 2;
                    break;

                case PRIORITYSCHEMA.A1:
                    val = 1;
                    break;
            }
            return val;
        }

        private PRIORITYSCHEMA ValueToSchema(int value)
        {
            var schema = PRIORITYSCHEMA.NONE;
            switch (value)
            {
                case 1:
                    schema = PRIORITYSCHEMA.A1;
                    break;

                case 2:
                    schema = PRIORITYSCHEMA.A2;
                    break;

                case 3:
                    schema = PRIORITYSCHEMA.A3;
                    break;

                case 4:
                    schema = PRIORITYSCHEMA.B1;
                    break;

                case 5:
                    schema = PRIORITYSCHEMA.B2;
                    break;

                case 6:
                    schema = PRIORITYSCHEMA.B3;
                    break;

                case 7:
                    schema = PRIORITYSCHEMA.C1;
                    break;

                case 8:
                    schema = PRIORITYSCHEMA.C2;
                    break;

                case 9:
                    schema = PRIORITYSCHEMA.C3;
                    break;
            }

            return schema;
        }

        private PRIORITYLEVEL SchemaToLevel(PRIORITYSCHEMA schema)
        {
            var val = PRIORITYLEVEL.NONE;
            switch (schema)
            {
                case PRIORITYSCHEMA.NONE:
                    val = PRIORITYLEVEL.NONE;
                    break;

                case PRIORITYSCHEMA.C3:
                    val = PRIORITYLEVEL.LOW;
                    break;

                case PRIORITYSCHEMA.C2:
                    val = PRIORITYLEVEL.LOW;
                    break;

                case PRIORITYSCHEMA.C1:
                    val = PRIORITYLEVEL.LOW;
                    break;

                case PRIORITYSCHEMA.B3:
                    val = PRIORITYLEVEL.LOW;
                    break;

                case PRIORITYSCHEMA.B2:
                    val = PRIORITYLEVEL.MEDIUM;
                    break;

                case PRIORITYSCHEMA.B1:
                    val = PRIORITYLEVEL.HIGH;
                    break;

                case PRIORITYSCHEMA.A3:
                    val = PRIORITYLEVEL.HIGH;
                    break;

                case PRIORITYSCHEMA.A2:
                    val = PRIORITYLEVEL.HIGH;
                    break;

                case PRIORITYSCHEMA.A1:
                    val = PRIORITYLEVEL.HIGH;
                    break;
            }
            return val;
        }

        private PRIORITYSCHEMA LevelToSchema(PRIORITYLEVEL schema)
        {
            var value = PRIORITYSCHEMA.NONE;
            switch (schema)
            {
                case PRIORITYLEVEL.LOW:
                    value = PRIORITYSCHEMA.C3;
                    break;

                case PRIORITYLEVEL.MEDIUM:
                    value = PRIORITYSCHEMA.B2;
                    break;

                case PRIORITYLEVEL.HIGH:
                    value = PRIORITYSCHEMA.A1;
                    break;

                case PRIORITYLEVEL.NONE:
                    value = PRIORITYSCHEMA.NONE;
                    break;
            }

            return value;
        }

        public PriorityType Format => format;

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
            schema = PRIORITYSCHEMA.NONE;
            level = PRIORITYLEVEL.NONE;
            schema = ValueToSchema(value);
            level = ValueToLevel(value);
        }

        public PRIORITY(PRIORITYLEVEL level)
        {
            format = PriorityType.Level;
            value = 0;
            schema = PRIORITYSCHEMA.NONE;
            this.level = level;
            value = LevelToValue(level);
            schema = LevelToSchema(level);
        }

        public PRIORITY(PRIORITYSCHEMA schema)
        {
            format = PriorityType.Schema;
            value = 0;
            this.schema = schema;
            level = PRIORITYLEVEL.NONE;
            value = SchemaToValue(schema);
            level = SchemaToLevel(schema);
        }

        public PRIORITY(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            format = PriorityType.Integral;
            this.value = 0;
            schema = PRIORITYSCHEMA.NONE;
            level = PRIORITYLEVEL.NONE;

            var pattern = @"^(?<priority>PRIORITY:)?(?<value>\d)$";
            var options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                          RegexOptions.CultureInvariant;

            var regex = new Regex(pattern, options);

            if (!regex.IsMatch(value)) return;

            foreach (Match match in Regex.Matches(value, pattern, options))
            {
                if (match.Groups["value"].Success)
                {
                    this.value = int.Parse(match.Groups["value"].Value);
                    schema = ValueToSchema(this.value);
                    level = ValueToLevel(this.value);
                }
            }
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

        public void WriteCalendar(CalendarWriter writer)
        {
            if (Format == PriorityType.Integral) writer.WriteProperty("PRIORITY", value.ToString());
            if (Format == PriorityType.Level) writer.WriteProperty("PRIORITY", LevelToValue(level).ToString());
            if (Format == PriorityType.Schema) writer.WriteProperty("PRIORITY", SchemaToValue(schema).ToString());
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public bool CanSerialize() => value >= 0 && value <= 9;
    }

    #endregion Descriptive Properties

    #region Date and Time Component Properties

    /// <summary>
    /// Defines one or more free or busy time intervals
    /// </summary>
    [DataContract]
    public class FREEBUSY : IFREEBUSY_PROPERTY, IContainsKey<Guid>, IEquatable<FREEBUSY>, ICalendarSerializable
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

        public FREEBUSY()
        {
            Periods = new List<PERIOD>();
            Type = FBTYPE.FREE;
        }

        /// <summary>
        /// Constructor based on the time interval
        /// </summary>
        /// <param name="periods"></param>
        /// <param name="type"></param>
        public FREEBUSY(IEnumerable<PERIOD> periods, FBTYPE type = FBTYPE.FREE)
        {
            Type = type;
            Periods = periods.NullOrEmpty() ? new List<PERIOD>() : new List<PERIOD>(periods);
        }

        public FREEBUSY(IFREEBUSY_PROPERTY other)
        {
            if (other != null)
            {
                Type = other.Type;
                Periods = other.Periods.NullOrEmpty()
                    ? new List<PERIOD>()
                    : new List<PERIOD>(other.Periods);
            }
        }

        public FREEBUSY(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));

            var trimmed = Regex.Replace(value, @"\s", string.Empty);
            var pattern =
                @"^FREEBUSY(;(?<fbtype>FBTYPE=(FREE|BUSY|BUSY-UNAVAILABLE|BUSY-TENTATIVE)))?:(?<periods>(((((\p{L})+)*(\/)*((\p{L}+\p{P}*\s*)+):)*(\d{2,4})(\d{1,2})(\d{1,2})(T(\d{1,2})(\d{1,2})(\d{1,2})Z)?/(((\p{L})+)*(\/)*((\p{L}+\p{P}*\s*)+):)*(\d{2,4})(\d{1,2})(\d{1,2})(T(\d{1,2})(\d{1,2})(\d{1,2})Z)?)|((((\p{L})+)*(\/)*((\p{L}+\p{P}*\s*)+):)*(\d{2,4})(\d{1,2})(\d{1,2})(T(\d{1,2})(\d{1,2})(\d{1,2})Z)?/(\-)?P((\d*)W)?((\d*)D)?(T((\d*)H)?((\d*)M)?((\d*)S)?)?),?)+)$";
            var options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                          RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(trimmed, pattern, options)) throw new FormatException("value");
            foreach (Match match in regex.Matches(trimmed))
            {
                if (match.Groups["fbtype"].Success)
                {
                    FBTYPE type;
                    if (!Enum.TryParse(match.Groups["fbtype"].Value, true, out type))
                        throw new FormatException("value");
                    Type = type;
                }
                if (match.Groups["periods"].Success)
                {
                    var lines = match.Groups["periods"].Value.Split(new[] { ',', ' ' },
                        StringSplitOptions.RemoveEmptyEntries);

                    Periods.AddRange(lines.Select(x => new PERIOD(x)));
                }
            }
        }

        public bool Equals(FREEBUSY other)
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
            return Equals((FREEBUSY)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(FREEBUSY left, FREEBUSY right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FREEBUSY left, FREEBUSY right)
        {
            return !Equals(left, right);
        }

        public void WriteCalendar(CalendarWriter writer)
        {
            if (Type != default(FBTYPE)) writer.WriteSemicolon().WriteParameter("FBTYPE", Type.ToString());
            writer.AppendPropertyValues(Periods);
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public bool CanSerialize() => Periods.Any();
    }

    #endregion Date and Time Component Properties

    #region Time-zone Properties

    /// <summary>
    /// Specifies the customary designation for a time zone description
    /// </summary>
    [DataContract]
    public class TZNAME : ITZNAME, IEquatable<TZNAME>, IComparable<TZNAME>, IContainsKey<Guid>, ICalendarSerializable
    {
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Language, inherent for this time zone
        /// </summary>
        [DataMember]
        public LANGUAGE Language { get; set; }

        /// <summary>
        /// Text, containing the Name of the Time-Zone
        /// </summary>
        [DataMember]
        public string Text { get; set; }

        public TZNAME()
        {
        }

        /// <summary>
        /// Constructor, based on the Name of the Time Zone
        /// </summary>
        /// <param name="text"></param>
        public TZNAME(string text, LANGUAGE language = null)
        {
            Text = text;
            Language = language;
        }

        public TZNAME(ITZNAME tzname)
        {
            if (tzname != null)
            {
                Language = tzname.Language;
                Text = tzname.Text;
            }
        }

        public TZNAME(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var pattern = @"^TZNAME(;(?<lang>LANGUAGE=\p{L}{2}\-\p{L}{2}))?:(?<text>(\P{L}*\p{L}\p{M}*\P{L}*)+)$";
            var options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                          RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(value, pattern, options)) throw new FormatException("value");
            foreach (Match match in regex.Matches(value))
            {
                if (match.Groups["lang"].Success)
                {
                    Language = new LANGUAGE(match.Groups["lang"].Value);
                }

                if (match.Groups["text"].Success)
                {
                    Text = match.Groups["text"].Value;
                }
            }
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
            return Text.CompareTo(other.Text);
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

        public void WriteCalendar(CalendarWriter writer)
        {
            writer.Write("TZNAME");
            if (Language != null) writer.AppendParameter(Language);
            writer.AppendPropertyValue(writer.ConvertToSAFE_STRING(Text));
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public bool CanSerialize() => !string.IsNullOrEmpty(Text) && !string.IsNullOrWhiteSpace(Text);
    }

    #endregion Time-zone Properties

    #region Relationship Component Properties

    /// <summary>
    /// Defines an "Attendee" within a calendar component
    /// </summary>
    [DataContract]
    public class ATTENDEE : IATTENDEE, IEquatable<ATTENDEE>, IContainsKey<Guid>, ICalendarSerializable
    {
        private CAL_ADDRESS address;

        /// <summary>
        /// ID of the current Attendee
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Address of the Current Attendee
        /// </summary>
        [DataMember]
        public CAL_ADDRESS Address
        {
            get { return address; }
            set
            {
                if (value != null)
                {
                    address = !value.ToString().StartsWith("mailto:", StringComparison.OrdinalIgnoreCase)
                        ? new CAL_ADDRESS(string.Format("mailto:{0}", value))
                        : value;
                }
            }
        }

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

        [DataMember]
        public BOOLEAN Rsvp { get; set; }

        /// <summary>
        /// Specifies the Delegate for current Attendee
        /// </summary>
        [DataMember]
        public DELEGATED_TO Delegatee { get; set; }

        /// <summary>
        /// Specifies the Delegator to the current Attendee
        /// </summary>
        [DataMember]
        public DELEGATED_FROM Delegator { get; set; }

        /// <summary>
        /// Specifies the company or community that sends the Attendee
        /// </summary>
        [DataMember]
        public SENT_BY SentBy { get; set; }

        /// <summary>
        /// Common name of the Attendee
        /// </summary>
        [DataMember]
        public string CN { get; set; }

        /// <summary>
        /// Specifies a reference to a directory entry associated with the current Attendee
        /// </summary>
        [DataMember]
        public DIR Directory { get; set; }

        /// <summary>
        /// Native Language of the current Attendee
        /// </summary>
        [DataMember]
        public LANGUAGE Language { get; set; }

        /// <summary>
        /// </summary>
        public ATTENDEE()
        {
            Address = null;
            CalendarUserType = CUTYPE.NONE;
            Member = null;
            Role = ROLE.NONE;
            Participation = PARTSTAT.NONE;
            Rsvp = BOOLEAN.FALSE;
            Delegatee = null;
            Delegator = null;
            SentBy = null;
            CN = null;
            Directory = null;
        }

        public ATTENDEE(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var pattern = @"^ATTENDEE(?<params>;(\w+\S*))*:(?<value>\w+:\S*)$";
            var options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                          RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(value, pattern, options)) throw new FormatException("value");
            foreach (Match match in regex.Matches(value))
            {
                if (match.Groups["params"].Success)
                {
                    var parts = match.Groups["params"].Value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in parts)
                    {
                        if (part.StartsWith("CUTYPE", StringComparison.OrdinalIgnoreCase))
                        {
                            CalendarUserType = part.ParseEnumParameter<CUTYPE>();
                        }

                        if (part.StartsWith("MEMBER", StringComparison.OrdinalIgnoreCase))
                        {
                            Member = new MEMBER(part);
                        }

                        if (part.StartsWith("ROLE", StringComparison.OrdinalIgnoreCase))
                        {
                            Role = part.ParseEnumParameter<ROLE>();
                        }

                        if (part.StartsWith("ROLE", StringComparison.OrdinalIgnoreCase))
                        {
                            Participation = part.ParseEnumParameter<PARTSTAT>();
                        }

                        if (part.StartsWith("RSVP", StringComparison.OrdinalIgnoreCase))
                        {
                            Rsvp = part.ParseEnumParameter<BOOLEAN>();
                        }

                        if (part.StartsWith("DELEGATED-TO", StringComparison.OrdinalIgnoreCase))
                        {
                            Delegatee = new DELEGATED_TO(part);
                        }

                        if (part.StartsWith("DELEGATED-FROM", StringComparison.OrdinalIgnoreCase))
                        {
                            Delegator = new DELEGATED_FROM(part);
                        }

                        if (part.StartsWith("SENT-BY", StringComparison.OrdinalIgnoreCase))
                        {
                            SentBy = new SENT_BY(part);
                        }

                        if (part.StartsWith("CN", StringComparison.OrdinalIgnoreCase))
                        {
                            var names = part.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                            if (names.Length < 2) throw new FormatException("Invalid Common Name Format");
                            CN = names[1];
                        }
                        if (part.StartsWith("DIR", StringComparison.OrdinalIgnoreCase))
                        {
                            Directory = new DIR(part);
                        }

                        if (part.StartsWith("LANGUAGE", StringComparison.OrdinalIgnoreCase))
                        {
                            Language = new LANGUAGE(part);
                        }
                    }
                }

                if (match.Groups["value"].Success)
                {
                    Address = new CAL_ADDRESS(match.Groups["value"].Value);
                }
            }
        }

        public ATTENDEE(IATTENDEE other)
        {
            if (other != null)
            {
                Address = new CAL_ADDRESS(other.Address);
                CalendarUserType = other.CalendarUserType;
                Role = other.Role;
                Participation = other.Participation;
                Rsvp = other.Rsvp;
                Delegatee = new DELEGATED_TO(other.Delegatee);
                Delegator = new DELEGATED_FROM(other.Delegatee);
                SentBy = new SENT_BY(other.SentBy);
                if (other.CN != null) CN = string.Copy(other.CN);
                Directory = new DIR(other.Directory);
            }
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

        public void WriteCalendar(CalendarWriter writer)
        {
            writer.Write("ATTENDEE");
            if (CalendarUserType != default(CUTYPE)) writer.AppendParameter("CUTYPE", CalendarUserType.ToString());
            if (Member != null) writer.AppendParameter(Member);
            if (Role != default(ROLE)) writer.AppendParameter("ROLE", Role.ToString());
            if (Participation != default(PARTSTAT)) writer.AppendParameter("PARTSTAT", Participation.ToString());
            if (Rsvp != BOOLEAN.TRUE) writer.AppendParameter("RSVP", Rsvp.ToString());
            if (Delegatee != null) writer.AppendParameter(Delegatee);
            if (Delegator != null) writer.AppendParameter(Delegator);
            if (SentBy != null) writer.AppendParameter(SentBy);
            if (!string.IsNullOrEmpty(CN) && !string.IsNullOrWhiteSpace(CN)) writer.AppendParameter("CN", CN);
            if (Directory != null ) writer.AppendParameter(Directory);
            if (Language != null) writer.AppendParameter(Language);

            writer.AppendPropertyValue(Address);
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public bool CanSerialize() => Address != null;
    }

    /// <summary>
    /// Defines the Organizer for Calendar Component
    /// </summary>
    [DataContract]
    public class ORGANIZER : IORGANIZER, IEquatable<ORGANIZER>, IContainsKey<Guid>, ICalendarSerializable
    {
        private CAL_ADDRESS address;

        /// <summary>
        /// ID of the Organizer
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Address of an Organizer
        /// </summary>
        [DataMember]
        public CAL_ADDRESS Address
        {
            get { return address; }
            set
            {
                if (value != null)
                {
                    address = !value.ToString().StartsWith("mailto:", StringComparison.OrdinalIgnoreCase)
                        ? new CAL_ADDRESS(string.Format("mailto:{0}", value))
                        : value;
                }
            }
        }

        /// <summary>
        /// Common or Display Name associated with "Organizer"
        /// </summary>
        [DataMember]
        public string CN { get; set; }

        /// <summary>
        /// Directory information associated with "Organizer"
        /// </summary>
        [DataMember]
        public DIR Directory { get; set; }

        /// <summary>
        /// Specifies another calendar user that is acting on behalf of the Organizer
        /// </summary>
        [DataMember]
        public SENT_BY SentBy { get; set; }

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

        public ORGANIZER(IORGANIZER organizer)
        {
            if (organizer != null)
            {
                Address = new CAL_ADDRESS(organizer.Address);
                CN = organizer.CN;
                Directory = new DIR(organizer.Directory);
                SentBy = new SENT_BY(organizer.SentBy);
                Language = new LANGUAGE(organizer.Language);
            }
        }

        public ORGANIZER(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var pattern = @"^ORGANIZER(?<params>;(\w+\S*))*:(?<address>\w+:\S*)$";
            var options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                          RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(value, pattern, options)) throw new FormatException("value");
            foreach (Match match in regex.Matches(value))
            {
                if (match.Groups["params"].Success)
                {
                    var parts = match.Groups["params"].Value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in parts)
                    {
                        if (part.StartsWith("SENT-BY", StringComparison.OrdinalIgnoreCase))
                        {
                            SentBy = new SENT_BY(part);
                        }

                        if (part.StartsWith("CN", StringComparison.OrdinalIgnoreCase))
                        {
                            var names = part.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                            if (names.Length < 2) throw new FormatException("Invalid Common Name Format");
                            CN = names[1];
                        }
                        if (part.StartsWith("DIR", StringComparison.OrdinalIgnoreCase))
                        {
                            Directory = new DIR(part);
                        }

                        if (part.StartsWith("LANGUAGE", StringComparison.OrdinalIgnoreCase))
                        {
                            Language = new LANGUAGE(part);
                        }
                    }
                }

                if (match.Groups["address"].Success)
                {
                    var address = match.Groups["address"].Value;
                    Address = new CAL_ADDRESS(address);
                }
            }
        }

        /// <summary>
        /// Overloaded ToString Method
        /// </summary>
        /// <returns>String Representation of the Organizer property"</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("ORGANIZER");
            if (CN != null) sb.AppendFormat(";CN={0}", CN);
            if (Directory != null) sb.AppendFormat(";{0}", Directory);
            if (SentBy != null) sb.AppendFormat(";{0}", SentBy);
            if (Language != null) sb.AppendFormat(";{0}", Language);
            sb.AppendFormat(":{0}", Address);
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

        public void WriteCalendar(CalendarWriter writer)
        {
            writer.Write("ORGANIZER");
            if (SentBy != null) writer.AppendParameter(SentBy);
            if (!string.IsNullOrEmpty(CN) && !string.IsNullOrWhiteSpace(CN)) writer.AppendParameter("CN", CN);
            if (Directory != null) writer.AppendParameter(Directory);
            if (Language != null) writer.AppendParameter(Language);
            writer.AppendPropertyValue(Address);
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public bool CanSerialize() => Address != null;

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
    public class RECURRENCE_ID : IRECURRENCE_ID, IEquatable<RECURRENCE_ID>, IContainsKey<Guid>, ICalendarSerializable
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
        /// ID of the current Time Zone
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
            Range = RANGE.NONE;
        }

        /// <summary>
        /// </summary>
        /// <param name="value">
        /// Gets or sets the Time when the original recurrence instance would occur
        /// </param>
        /// <param name="range"></param>
        public RECURRENCE_ID(DATE_TIME value, RANGE range = RANGE.THISANDFUTURE)
        {
            Value = value;
            Range = range;
        }

        public RECURRENCE_ID(IRECURRENCE_ID other)
        {
            if (other != null)
            {
                Value = other.Value;
                if (other.TimeZoneId != null) TimeZoneId = new TZID(other.TimeZoneId);
                Range = other.Range;
            }
        }

        public RECURRENCE_ID(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var pattern =
                @"^RECURRENCE-ID(?<params>;(\w+\S*))*:(?<values>((((TZID=((\P{L}*\p{L}\p{M}*\P{L}*)+)?/((\P{L}*\p{L}\p{M}*\P{L}*)+)):)?(\d{2,4})(\d{1,2})(\d{1,2})(T(\d{1,2})(\d{1,2})(\d{1,2})(Z)?)?),?)+)$";
            var options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                          RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(value, pattern, options)) throw new FormatException("value");
            foreach (Match match in regex.Matches(value))
            {
                if (match.Groups["params"].Success)
                {
                    var parts = match.Groups["params"].Value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in parts)
                    {
                        if (part.StartsWith("VALUE", StringComparison.OrdinalIgnoreCase))
                        {
                            var dateType = part.ParseEnumParameter<VALUE>();
                            if (dateType != VALUE.DATE && dateType != VALUE.DATE_TIME)
                                throw new FormatException("Invalid VALUE format");
                        }

                        if (part.StartsWith("TZID", StringComparison.OrdinalIgnoreCase))
                        {
                            TimeZoneId = new TZID(part);
                        }

                        if (part.StartsWith("RANGE", StringComparison.OrdinalIgnoreCase))
                        {
                            Range = part.ParseEnumParameter<RANGE>();
                        }
                    }
                }

                if (match.Groups["value"].Success)
                {
                    Value = new DATE_TIME(match.Groups["value"].Value);
                }
            }
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

        public void WriteCalendar(CalendarWriter writer)
        {
            writer.Write("RECURRENCE-ID");
            if (TimeZoneId != null)
            {
                writer.AppendParameter(TimeZoneId);
                writer.AppendParameter("RANGE", Range.ToString());
            }
            writer.AppendPropertyValue(Value);
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public bool CanSerialize() => Value != default(DATE_TIME);

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
    public class RELATEDTO : IRELATEDTO, IEquatable<RELATEDTO>, IContainsKey<Guid>, ICalendarSerializable
    {
        /// <summary>
        /// ID of the Relationship or Reference between one calendar component and another
        /// </summary>
        [DataMember]
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
            RelationshipType = RELTYPE.NONE;
        }

        /// <summary>
        /// Constructor specifying the Text and Type properties of the Related_To property
        /// </summary>
        /// <param name="reference">
        /// Description of the relationship or reference to another Calendar component
        /// </param>
        /// <param name="type">Type of the relationship: dafault - PARENT, can be: CHILD and SIBLING</param>
        public RELATEDTO(string reference, RELTYPE type = RELTYPE.NONE)
        {
            if (reference == null) throw new ArgumentNullException(nameof(reference));
            Reference = reference;
            RelationshipType = type;
        }

        public RELATEDTO(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var pattern =
                @"^RELATED-TO(;(?<reltype>RELTYPE=(PARENT|CHILD|SIBLING))?:(?<reference>(\P{L}*\p{L}\p{M}*\P{L}*)+))$";
            var options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                          RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(value, pattern, options)) throw new FormatException("value");
            foreach (Match match in regex.Matches(value))
            {
                if (match.Groups["reltype"].Success)
                {
                    RelationshipType =
                        match.Groups["reltype"].Value.ParseEnumParameter<RELTYPE>();
                }

                if (match.Groups["reference"].Success)
                {
                    Reference = match.Groups["reference"].Value;
                }
            }
        }

        /// <summary>
        /// Overloaded ToString Method
        /// </summary>
        /// <returns>String Representation of the Related_To property in form of "RELATED-TO;Relationship:Text"</returns>

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

        public void WriteCalendar(CalendarWriter writer)
        {
            writer.Write("RELATED-TO");
            if (RelationshipType != default(RELTYPE)) writer.AppendParameter("RELTYPE", RelationshipType.ToString());
            writer.AppendPropertyValue(Reference);
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public bool CanSerialize() => !string.IsNullOrEmpty(Reference) && !string.IsNullOrWhiteSpace(Reference);

        public static bool operator ==(RELATEDTO left, RELATEDTO right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RELATEDTO left, RELATEDTO right)
        {
            return !Equals(left, right);
        }
    }

    [DataContract]
    public class URL : IContainsKey<Guid>, IEquatable<URL>, ICalendarSerializable
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public Uri Uri { get; set; }

        public URL()
        {
        }

        public URL(Uri uri)
        {
            if (uri != null)
            {
                Uri = new Uri(uri.ToString());
            }
        }

        public URL(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            const string pattern = @"^RECURRENCE-ID(?<params>;(\w+\S*))*:(?<value>((\w+:\S+))$";
            const RegexOptions options =
                RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(value, pattern, options)) throw new FormatException("value");
            foreach (Match match in regex.Matches(value))
            {
                if (match.Groups["value"].Success)
                {
                    Uri = new Uri(match.Groups["value"].Value);
                }
            }
        }

        public bool Equals(URL other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((URL)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ Id.GetHashCode();
            }
        }

        public static bool operator ==(URL left, URL right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(URL left, URL right)
        {
            return !Equals(left, right);
        }

        public void WriteCalendar(CalendarWriter writer)
        {
            writer.WriteProperty("URL", Uri.ToString());
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public bool CanSerialize() => Uri != null;
    }

    #endregion Relationship Component Properties

    #region Recurrence Component properties

    /// <summary>
    /// Defines the list of DATE-TIME exceptions for recurring events, to-dos, entries, or time zone definitions
    /// </summary>
    [DataContract]
    public class EXDATE : IEXDATE, IEquatable<EXDATE>, IContainsKey<Guid>, ICalendarSerializable
    {
        /// <summary>
        /// ID of the Date-Time Exception
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

        public EXDATE()
        {
            DateTimes = new List<DATE_TIME>();
            TimeZoneId = null;
        }

        public EXDATE(IEXDATE other)
        {
            if (other != null)
            {
                if (other.DateTimes.Any())
                {
                    DateTimes = new List<DATE_TIME>(other.DateTimes.Select(x => x));
                }

                if (other.TimeZoneId != null)
                    TimeZoneId = new TZID(other.TimeZoneId);
            }
        }

        /// <summary>
        /// Constructor based on the set of Dates of Recurrence Exceptions and ID of the Time Zone
        /// </summary>
        /// <param name="dateTimes">Set of Dates of Recurrence Exceptions</param>
        /// <param name="tzid"></param>
        public EXDATE(IEnumerable<DATE_TIME> dateTimes, TZID tzid)
        {
            DateTimes = dateTimes.NullOrEmpty()
                ? new List<DATE_TIME>()
                : new List<DATE_TIME>(dateTimes);

            TimeZoneId = tzid;
        }

        public EXDATE(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var trimmed = Regex.Replace(value, @"\s", string.Empty);
            const string pattern =
                @"^EXDATE(?<params>;(\w+\S*))*:(?<values>((((TZID=((\P{L}*\p{L}\p{M}*\P{L}*)+)?/((\P{L}*\p{L}\p{M}*\P{L}*)+)):)?(\d{2,4})(\d{1,2})(\d{1,2})(T(\d{1,2})(\d{1,2})(\d{1,2})(Z)?)?),?)+)$";
            const RegexOptions options =
                RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(trimmed, pattern, options)) throw new FormatException("value");

            foreach (Match match in regex.Matches(trimmed))
            {
                if (match.Groups["params"].Success)
                {
                    var parts = match.Groups["params"].Value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in parts)
                    {
                        if (part.StartsWith("VALUE", StringComparison.OrdinalIgnoreCase))
                        {
                            var dateType = part.ParseEnumParameter<VALUE>();
                            if (dateType != VALUE.DATE && dateType != VALUE.DATE_TIME)
                                throw new FormatException("Invalid VALUE format");
                        }

                        if (part.StartsWith("TZID", StringComparison.OrdinalIgnoreCase))
                        {
                            TimeZoneId = new TZID(part);
                        }
                    }
                }

                if (match.Groups["values"].Success)
                {
                    var values = match.Groups["values"].Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    DateTimes.AddRange(values.Select(x => new DATE_TIME(x)));
                }
            }
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

        public void WriteCalendar(CalendarWriter writer)
        {
            writer.Write("EXDATE");
            if (TimeZoneId != null) writer.AppendParameter(TimeZoneId);
            writer.AppendPropertyValues(DateTimes);
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public bool CanSerialize() => DateTimes.Any();

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
    /// Defines the list of DATE-TIME values for recurring events, to-dos, journal entries or
    /// time-zone definitions
    /// </summary>
    [DataContract]
    public class RDATE : IRDATE, IEquatable<RDATE>, IContainsKey<Guid>, ICalendarSerializable
    {
        /// <summary>
        /// ID of the list of DATE-TIME values for recurring events, to-dos, journal entries or
        /// time-zone definitions
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
            ValueType = VALUE.NONE;
        }

        /// <summary>
        /// Constructor based on the list of DATE-TIME values and the ID of the current Time Zone
        /// </summary>
        /// <param name="values">
        /// List of DATE-TIME values for recurring events, to-dos, journal entries or time-zone definitions
        /// </param>
        /// <param name="tzid">ID of the current Time Zone</param>
        public RDATE(IEnumerable<DATE_TIME> values, TZID tzid = null, VALUE value_type = VALUE.NONE)
        {
            DateTimes = values.NullOrEmpty() ? new List<DATE_TIME>() : new List<DATE_TIME>(values);
            TimeZoneId = tzid;
            Periods = new List<PERIOD>();
            ValueType = value_type;
        }

        /// <summary>
        /// Constructor based on the list of periods and the ID of currtnt time zone
        /// </summary>
        /// <param name="periods">
        /// List of Periods between recurring events, to-dos, journal entries or time-zone definitions
        /// </param>
        /// <param name="tzid">ID of the current Time Zone</param>
        public RDATE(IEnumerable<PERIOD> periods, TZID tzid = null, VALUE value_type = VALUE.NONE)
        {
            DateTimes = new List<DATE_TIME>();
            Periods = periods.NullOrEmpty() ? new List<PERIOD>() : new List<PERIOD>(periods);
            TimeZoneId = tzid;
            ValueType = value_type;
        }

        public RDATE(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var trimmed = Regex.Replace(value, @"\s", string.Empty);
            const string pattern =
                @"^RDATE(;(?<params>(\w+\S*)))?:(?<values>(((((TZID=((\P{L}*\p{L}\p{M}*\P{L}*)+)?/((\P{L}*\p{L}\p{M}*\P{L}*)+)):)?(\d{2,4})(\d{1,2})(\d{1,2})(T(\d{1,2})(\d{1,2})(\d{1,2})(Z)?)?)|((\d{2,4}\d{1,2}\d{1,2})(T\d{1,2}\d{1,2}\d{1,2}Z?)?/(\d{2,4}\d{1,2}\d{1,2})(T\d{1,2}\d{1,2}\d{1,2}Z?)?)|((\d{2,4}\d{1,2}\d{1,2})(T\d{1,2}\d{1,2}\d{1,2}Z?)?/(\-)?P(\d*W)?(\d*D)?(T(\d*H)?((\d*)M)?((\d*)S)?)?)),?)+)$";

            const RegexOptions options =
                RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(trimmed, pattern, options)) throw new FormatException("value");

            string[] values = null;
            foreach (Match match in regex.Matches(trimmed))
            {
                if (match.Groups["params"].Success)
                {
                    var parts = match.Groups["params"].Value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in parts)
                    {
                        if (part.StartsWith("VALUE", StringComparison.OrdinalIgnoreCase))
                        {
                            var valueType = part.ParseEnumParameter<VALUE>();
                            if (valueType != VALUE.DATE && valueType != VALUE.DATE_TIME && valueType != VALUE.PERIOD)
                                throw new FormatException("Invalid VALUE format");
                        }

                        if (part.StartsWith("TZID", StringComparison.OrdinalIgnoreCase))
                        {
                            TimeZoneId = new TZID(part);
                        }
                    }
                }

                if (match.Groups["values"].Success)
                {
                    values = match.Groups["values"].Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }

            if ((ValueType == VALUE.DATE || ValueType == VALUE.DATE_TIME) && !values.NullOrEmpty())
            {
                DateTimes = values.Select(x => new DATE_TIME(x)).ToList();
            }

            if (ValueType == VALUE.PERIOD && !values.NullOrEmpty())
            {
                Periods = values.Select(x => new PERIOD(x)).ToList();
            }
        }

        public RDATE(IRDATE other)
        {
            if (other != null)
            {
                ValueType = other.ValueType;
                if (other.DateTimes != null) DateTimes = new List<DATE_TIME>(other.DateTimes);
                if (other.Periods != null) Periods = new List<PERIOD>(other.Periods);
                TimeZoneId = other.TimeZoneId;
            }
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

        public void WriteCalendar(CalendarWriter writer)
        {
            writer.Write("RDATE");
            if (ValueType != default(VALUE)) writer.AppendParameter("VALUE", ValueType.ToString());

            if (DateTimes.Any()) writer.AppendPropertyValues(DateTimes);
            else if (Periods.Any()) writer.AppendPropertyValues(Periods);
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public bool CanSerialize() => DateTimes.Any() || Periods.Any();

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
    public class TRIGGER : ITRIGGER, IEquatable<TRIGGER>, IContainsKey<Guid>, ICalendarSerializable
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
        public VALUE ValueType { get; set; }

        [DataMember]
        public RELATED Related { get; set; }

        public TRIGGER()
        {
            type = TriggerType.Related;
        }

        public TRIGGER(DURATION duration, RELATED related = RELATED.NONE, VALUE valueType = VALUE.NONE)
        {
            Duration = duration;
            Related = related;
            ValueType = valueType;
            type = TriggerType.Related;
        }

        public TRIGGER(DATE_TIME datetime, VALUE valueType = VALUE.NONE)
        {
            ValueType = valueType;
            DateTime = datetime;
            type = TriggerType.Absolute;
        }

        public TRIGGER(ITRIGGER other)
        {
            if (other != null)
            {
                DateTime = other.DateTime;
                ValueType = other.ValueType;
                Related = other.Related;
                Duration = other.Duration;
            }
        }

        public TRIGGER(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var trimmed = Regex.Replace(value, @"\s", string.Empty);
            const string pattern =
                @"^TRIGGER(;((?<valueType>VALUE=DURATION|VALUE=DATE-TIME)?(?<related>RELATED=(START|END))?))?:
((?<trigrel>((\-)?P((\d*)W)?((\d*)D)?(T((\d*)H)?((\d*)M)?((\d*)S)?)?))|
((?<trigabs>((?<tzid>TZID=((\P{L}*\p{L}\p{M}*\P{L}*)+)?/((\P{L}*\p{L}\p{M}*\P{L}*)+)):)?(\d{2,4})(\d{1,2})(\d{1,2})(T(\d{1,2})(\d{1,2})(\d{1,2})(Z)?)?)))$";

            const RegexOptions options =
                RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(trimmed, pattern, options)) throw new FormatException("value");

            foreach (Match match in regex.Matches(trimmed))
            {
                if (match.Groups["valueType"].Success)
                {
                    ValueType = match.Groups["valueType"].Value.ParseEnumParameter<VALUE>();
                    if (ValueType != VALUE.DURATION && ValueType != VALUE.DATE_TIME)
                        throw new FormatException("Not Supported format");
                }

                if (match.Groups["related"].Success)
                {
                    Related = match.Groups["related"].Value.ParseEnumParameter<RELATED>();
                    if (Related == RELATED.NONE) throw new FormatException("Not Supported format");
                }

                if (match.Groups["trigrel"].Success)
                {
                    Duration = new DURATION(match.Groups["trigrel"].Value);
                }

                if (match.Groups["trigabs"].Success)
                {
                    DateTime = new DATE_TIME(match.Groups["trigabs"].Value);
                }
            }
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

        public void WriteCalendar(CalendarWriter writer)
        {
            writer.Write("TRIGGER");
            if (type == TriggerType.Related)
            {
                if (ValueType == VALUE.DURATION) writer.AppendParameter("VALUE", VALUE.DURATION.ToString());
                else if (Related != default(RELATED)) writer.AppendParameter("RELATED", Related.ToString());
                writer.AppendPropertyValue(Duration);
            }
            if (type == TriggerType.Absolute)
            {
                if (ValueType == VALUE.DATE_TIME) writer.WriteParameter("VALUE", VALUE.DATE_TIME.ToString());
                writer.AppendPropertyValue(DateTime);
            }
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public bool CanSerialize() =>
            (ValueType == VALUE.DURATION && Duration != default(DURATION))
            || (ValueType == VALUE.DATE_TIME && DateTime != default(DATE_TIME));

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

    #region Miscellaneous Properties

    [DataContract]
    public struct STATCODE : ISTATCODE, IEquatable<STATCODE>, ICalendarSerializable
    {
        private uint l1;
        private uint l2;
        private uint? l3;

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
        public uint? L3
        {
            get { return l3; }
            set { l3 = value; }
        }

        public STATCODE(uint l1, uint l2 = 0, uint? l3 = null)
            : this()
        {
            this.l1 = l1;
            this.l2 = l2;
            this.l3 = l3;
        }

        public STATCODE(ISTATCODE other)
        {
            if (other != null)
            {
                l1 = other.L1;
                l2 = other.L2;
                l3 = other.L3;
            }

            l1 = 0;
            l2 = 0;
            l3 = null;
        }

        public STATCODE(string value)
        {
            l1 = l2 = default(uint);
            l3 = null;

            if (value == null) throw new ArgumentNullException(nameof(value));

            var trimmed = Regex.Replace(value, @"\s", string.Empty);
            const string pattern = @"^(?<l1>\d+).(?<l2>\d+)(.(?<l3>\d+))?$";

            const RegexOptions options =
                RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(trimmed, pattern, options)) throw new FormatException("value");

            foreach (Match match in regex.Matches(trimmed))
            {
                if (match.Groups["l1"].Success)
                {
                    l1 = uint.Parse(match.Groups["l1"].Value);
                }

                if (match.Groups["l2"].Success)
                {
                    l2 = uint.Parse(match.Groups["l2"].Value);
                }

                if (match.Groups["l3"].Success)
                {
                    l3 = uint.Parse(match.Groups["l3"].Value);
                }
            }
        }

        public bool Equals(STATCODE other)
        {
            return l1 == other.l1 && l2 == other.l2 && l3 == other.l3;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is STATCODE && Equals((STATCODE)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)l1;
                hashCode = (hashCode * 397) ^ (int)l2;
                hashCode = (hashCode * 397) ^ l3.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(STATCODE left, STATCODE right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(STATCODE left, STATCODE right)
        {
            return !left.Equals(right);
        }

        public void WriteCalendar(CalendarWriter writer)
        {
            writer.Write(L3.HasValue ? $"{L1}.{L2}.{L3}" : $"{L1}.{L2}");
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public bool CanSerialize() => true;
    }

    /// <summary>
    /// Defines the status code returned for a scheduling request
    /// </summary>
    [DataContract]
    public class REQUEST_STATUS : IREQUEST_STATUS, IContainsKey<Guid>, IEquatable<REQUEST_STATUS>, ICalendarSerializable
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
        public STATCODE Code { get; set; }

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
            Code = default(STATCODE);
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

        public REQUEST_STATUS(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var trimmed = Regex.Replace(value, @"\s", string.Empty);
            const string pattern = @"^REQUEST-STATUS(;(?<lang>LANGUAGE=(\w+)(\-(\w+))?))?:(?<statcode>(\d+).(\d+)(.(\d+))?)(;(?<desc>(\P{L}*\p{L}\p{M}*\P{L}*)+))(;(?<exdata>(\P{L}*\p{L}\p{M}*\P{L}*)+))?$";

            const RegexOptions options =
                RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(trimmed, pattern, options)) throw new FormatException("value");

            foreach (Match match in regex.Matches(trimmed))
            {
                if (match.Groups["lang"].Success)
                {
                    Language = new LANGUAGE(match.Groups["lang"].Value);
                }

                if (match.Groups["statcode"].Success)
                {
                    Code = new STATCODE(match.Groups["statcode"].Value);
                }

                if (match.Groups["description"].Success)
                {
                    ExceptionData = match.Groups["description"].Value;
                }
                if (match.Groups["exdata"].Success)
                {
                    ExceptionData = match.Groups["exdata"].Value;
                }
            }
        }

        public REQUEST_STATUS(IREQUEST_STATUS other)
        {
            if (other != null)
            {
                Code = other.Code;
                if (other.Description != null)
                {
                    Description = string.Copy(other.Description);
                }

                Language = new LANGUAGE(other.Language);
                if (other.ExceptionData != null)
                {
                    ExceptionData = string.Copy(other.ExceptionData);
                }
            }
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
            if (obj.GetType() != GetType()) return false;
            return Equals((REQUEST_STATUS)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public void WriteCalendar(CalendarWriter writer)
        {
            writer.Write("REQUEST-STATUS");
            if (Language != null) writer.AppendParameter(Language);
            writer.AppendPropertyValue(Code).AppendBySemicolon(writer.ConvertToSAFE_STRING(Description));
            if (!string.IsNullOrEmpty(ExceptionData) && !string.IsNullOrWhiteSpace(ExceptionData)) writer.AppendBySemicolon(ExceptionData);
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public bool CanSerialize() => Code != default(STATCODE) && !string.IsNullOrEmpty(Description) && !string.IsNullOrWhiteSpace(Description);

        public static bool operator ==(REQUEST_STATUS left, REQUEST_STATUS right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(REQUEST_STATUS left, REQUEST_STATUS right)
        {
            return !Equals(left, right);
        }
    }

    #endregion Miscellaneous Properties
}
