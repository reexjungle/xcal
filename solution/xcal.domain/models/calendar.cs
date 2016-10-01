using reexjungle.xcal.domain.contracts;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.foundation.contracts;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using reexjungle.xcal.infrastructure.contracts;
using reexjungle.xcal.infrastructure.serialization;
using reexjungle.xcal.infrastructure.extensions;

namespace reexjungle.xcal.domain.models
{
    /// <summary>
    /// Represents a core Calendar type
    /// </summary>
    [DataContract]
    public class VCALENDAR : ICALENDAR, IEquatable<VCALENDAR>, IContainsKey<Guid>, ICalendarSerializable
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
            if (obj.GetType() != GetType()) return false;
            return Equals((VCALENDAR)obj);
        }

        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(VCALENDAR left, VCALENDAR right) => Equals(left, right);

        public static bool operator !=(VCALENDAR left, VCALENDAR right) => !Equals(left, right);

        public void WriteCalendar(CalendarWriter writer)
        {
            writer.WriteStartComponent("VCALENDAR");
            writer.AppendProperty("VERSION", Version);
            writer.AppendProperty("PRODID", ProdId);
            if (Calscale != default(CALSCALE))writer.AppendProperty("CALSCALE", Calscale.ToString());
            if (Method != default(METHOD))writer.AppendProperty("METHOD", Method.ToString());
            if (Events.Any()) writer.AppendProperties(Events);
            if (ToDos.Any()) writer.AppendProperties(ToDos);
            if (FreeBusies.Any()) writer.AppendProperties(FreeBusies);
            if (Journals.Any()) writer.AppendProperties(Journals);
            if (TimeZones.Any()) writer.AppendProperties(TimeZones);
            writer.WriteLine();
            writer.WriteEndComponent("VCALENDAR");
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public bool CanSerialize() => Events.Any() || ToDos.Any() || FreeBusies.Any() || Journals.Any() || TimeZones.Any();
    }
}