using reexjungle.xcal.domain.contracts;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.foundation.contracts;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace reexjungle.xcal.domain.models
{
    /// <summary>
    /// Represents a core Calendar type
    /// </summary>
    [DataContract]
    public class VCALENDAR : ICALENDAR, IEquatable<VCALENDAR>, IContainsKey<Guid>
    {
        public static readonly VCALENDAR Empty = new VCALENDAR();

        /// <summary>
        /// Gets or sets the product identifier. Neccesary as primary key in datastore
        /// </summary>
        [DataMember]
        [Index(Unique = true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the product that created the iCalendar object.
        /// This property is REQUIRED. This identifier should be guaranteed to be a globally unique identifier (GUID)
        /// </summary>
        [DataMember]
        public string ProdId { get; set; }

        /// <summary>
        /// Gets or sets the identifier corresponding to the highest version number or the minimum and maximum range of the iCalendar
        /// specification that is required in order to interpret the iCalendar object.
        /// This property is REQUIRED.
        /// </summary>
        [DataMember]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the calendar scale used for the calendar information specified in the iCalendar object.
        /// This property is OPTIONAL. Its default value is "GREGORIAN"
        /// </summary>
        [DataMember]
        public CALSCALE Calscale { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DataMember]
        public METHOD Method { get; set; }

        /// <summary>
        /// Gets or sets the events of the iCalendar core object
        /// </summary>
        [DataMember]
        [Ignore]
        public List<VEVENT> Events { get; set; }

        /// <summary>
        /// Gets or sets the to-dos of the iCalendar core object
        /// </summary>
        [DataMember]
        [Ignore]
        public List<VTODO> ToDos { get; set; }

        /// <summary>
        /// Gets or sets the free or busy time information groups of the iCalendar core object
        /// </summary>
        [DataMember]
        [Ignore]
        public List<VFREEBUSY> FreeBusies { get; set; }

        /// <summary>
        /// Gets or sets the journals of the iCalendar core object
        /// </summary>
        [DataMember]
        [Ignore]
        public List<VJOURNAL> Journals { get; set; }

        /// <summary>
        /// Gets or sets the timezones of the iCalendar core object
        /// </summary>
        [DataMember]
        [Ignore]
        public List<VTIMEZONE> TimeZones { get; set; }

        /// <summary>
        /// Gets or sets the timezones of the iCalendar core object
        /// </summary>
        [DataMember]
        [Ignore]
        public List<IANA_COMPONENT> IanaComponents { get; set; }

        /// <summary>
        /// Gets or sets the X-components of the iCalendar core object
        /// </summary>
        [DataMember]
        [Ignore]
        public List<X_COMPONENT> XComponents { get; set; }

        /// <summary>
        /// Default Constructor of the iCalendar core object
        /// </summary>
        public VCALENDAR()
        {
            Version = "2.0";
            Calscale = CALSCALE.GREGORIAN;
            Events = new List<VEVENT>();
            TimeZones = new List<VTIMEZONE>();
            ToDos = new List<VTODO>();
            Journals = new List<VJOURNAL>();
            FreeBusies = new List<VFREEBUSY>();
            IanaComponents = new List<IANA_COMPONENT>();
            XComponents = new List<X_COMPONENT>();
        }

        public bool Equals(VCALENDAR other)
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
            return Equals((VCALENDAR)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(VCALENDAR left, VCALENDAR right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(VCALENDAR left, VCALENDAR right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:VCALENDAR").AppendLine();
            sb.AppendFormat("VERSION:{0}", Version).AppendLine();
            sb.AppendFormat("PRODID:{0}", ProdId).AppendLine();
            if (Calscale != default(CALSCALE)) sb.AppendFormat("CALSCALE:{0}", Calscale).AppendLine();
            if (Method != default(METHOD)) sb.AppendFormat("METHOD:{0}", Method).AppendLine();
            if (!Events.NullOrEmpty()) Events.ForEach(x => sb.Append(x.ToString()).AppendLine());
            if (!ToDos.NullOrEmpty()) ToDos.ForEach(x => sb.Append(x.ToString()).AppendLine());
            if (!FreeBusies.NullOrEmpty()) FreeBusies.ForEach(x => sb.Append(x.ToString()).AppendLine());
            if (!Journals.NullOrEmpty()) Journals.ForEach(x => sb.Append(x.ToString()).AppendLine());
            if (!TimeZones.NullOrEmpty()) TimeZones.ForEach(x => sb.Append(x.ToString()).AppendLine());
            if (!IanaComponents.NullOrEmpty()) IanaComponents.ForEach(x => sb.Append(x.ToString()).AppendLine());
            if (!XComponents.NullOrEmpty()) XComponents.ForEach(x => sb.Append(x.ToString()).AppendLine());
            sb.Append("END:VCALENDAR");
            return sb.ToString().FoldLines(75);
        }
    }
}