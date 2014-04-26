using System;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using reexmonkey.foundation.essentials.contracts;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.xcal.domain.contracts;

namespace reexmonkey.xcal.domain.models
{
    /// <summary>
    /// Represents a core Calendar type
    /// </summary>
    [DataContract]
    public class VCALENDAR : ICALENDAR, IEquatable<VCALENDAR>, IContainsKey<string>
    {
        /// <summary>
        /// Gets or sets the product identifier. Neccesary as primary key in Ormlite
        /// </summary>
        [DataMember]
        [Index(Unique = true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the product that created the iCalendar object.
        /// This property is REQUIRED. This identifier should be guaranteed to be a globally unique identifier (GUID)
        /// </summary>
        [DataMember]
        [Index(Unique = false)]
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
        [Ignore]
        public List<VTODO> ToDos { get; set; }

        /// <summary>
        /// Gets or sets the free or busy time information groups of the iCalendar core object
        /// </summary>
        [Ignore]
        public List<VFREEBUSY> FreeBusies { get; set; }

        /// <summary>
        /// Gets or sets the journals of the iCalendar core object
        /// </summary>
        public List<VJOURNAL> Journals { get; set; }

        /// <summary>
        /// Gets or sets the timezones of the iCalendar core object
        /// </summary>
        [Ignore]
        public List<VTIMEZONE> TimeZones { get; set; }

        /// <summary>
        /// Gets or sets the timezones of the iCalendar core object
        /// </summary>
        [Ignore]
        public List<IANA_COMPONENT> IanaComponents { get; set; }

        /// <summary>
        /// Gets or sets the X-components of the iCalendar core object
        /// </summary>
        [Ignore]
        public List<X_COMPONENT> XComponents { get; set; }

        /// <summary>
        /// Default Constructor of the iCalendar core object
        /// </summary>
        public VCALENDAR()
        {
            this.Version = "2.0";
            this.Calscale = CALSCALE.GREGORIAN;
        }

        public bool Equals(VCALENDAR other)
        {
            if (other == null) return false;
            return this.Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return this.Equals(obj as VCALENDAR);
        }

        public override int GetHashCode()
        {
            return
                this.ProdId.GetHashCode() ^ this.Version.GetHashCode() ^ this.Method.GetHashCode() ^
                this.Events.GetHashCode();
        }

        public static bool operator ==(VCALENDAR a, VCALENDAR b)
        {
            if ((object)a == null || (object)b == null) return Object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(VCALENDAR a, VCALENDAR b)
        {
            if ((object)a == null || (object)b == null) return !object.Equals(a, b);
            return !a.Equals(b);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:VCALENDAR").AppendLine();
            sb.AppendFormat("VERSION:{0}", this.Version).AppendLine();
            sb.AppendFormat("PRODID:{0}", this.ProdId).AppendLine();
            if(this.Calscale != CALSCALE.UNKNOWN) sb.AppendFormat("CALSCALE:{0}", this.Calscale).AppendLine();
            if(this.Method != METHOD.UNKNOWN) sb.AppendFormat("METHOD:{0}", this.Method).AppendLine();
            foreach (var x in Events) if(x != null) sb.Append(x.ToString()).AppendLine();
            foreach (var x in ToDos) if (x != null) sb.Append(x.ToString()).AppendLine();
            foreach (var x in FreeBusies) if (x != null) sb.Append(x.ToString()).AppendLine();
            foreach (var x in Journals) if (x != null) sb.Append(x.ToString()).AppendLine();
            foreach (var x in TimeZones) if (x != null) sb.Append(x.ToString()).AppendLine();
            foreach (var x in IanaComponents) if (x != null) sb.Append(x.ToString()).AppendLine();
            foreach (var x in XComponents) if (x != null) sb.Append(x.ToString()).AppendLine();
            sb.Append("END:VCALENDAR");
            return sb.ToString();
        }


    }

}
