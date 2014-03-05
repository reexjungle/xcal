using System;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using reexmonkey.crosscut.essentials.contracts;
using reexmonkey.crosscut.essentials.concretes;
using reexmonkey.xcal.domain.contracts;

namespace reexmonkey.xcal.domain.models
{
    /// <summary>
    /// Represents a core Calendar type
    /// </summary>
    [DataContract]
    [KnownType(typeof(VEVENT))]
    public class VCALENDAR : ICALENDAR, IEquatable<VCALENDAR>, IContainsKey<string>
    {
        private string prodid;

        /// <summary>
        /// Gets or sets the product identifier. Neccesary as primary key in Ormlite
        /// </summary>
        public string Id 
        {
            get { return prodid; }
            set { this.prodid = value; }
        }

        /// <summary>
        /// Gets or sets the identifier for the product that created the iCalendar object.
        /// This property is REQUIRED. This identifier should be guaranteed to be a globally unique identifier (GUID)
        /// </summary>
        [DataMember]
        [Index(Unique = true)] 
        public string ProdId
        { 
            get { return prodid; }
            set { prodid = value; }        
        }

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

        [DataMember]
        [Ignore]
        public List<ICOMPONENT> Components { get; set; }

        /// <summary>
        /// Default Constructor of the iCalendar core object
        /// </summary>
        public VCALENDAR()
        {
            this.Version = "2.0";
            this.Calscale = CALSCALE.GREGORIAN;
            this.Components = new List<ICOMPONENT>();
        }

        public bool Equals(VCALENDAR other)
        {
            if (other == null) return false;
            return
                this.ProdId == other.ProdId &&
                this.Calscale == other.Calscale &&
                this.Version == other.Version &&
                this.Components.OfType<VEVENT>().
                AreDuplicatesOf(other.Components.OfType<VEVENT>());
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return this.Equals(obj as VCALENDAR);
        }

        public override int GetHashCode()
        {
            return
                this.ProdId.GetHashCode() ^
                ((this.Version != null)? this.Version.GetHashCode() : 0)^
                this.Components.GetHashCode();
        }

        public static bool operator ==(VCALENDAR a, VCALENDAR b)
        {
            if ((object)a == null || (object)b == null) return Object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(VCALENDAR a, VCALENDAR b)
        {
            if (a == null || b == null) return !Object.Equals(a, b);
            return !a.Equals(b);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:VCALENDAR").AppendLine();
            sb.AppendFormat("VERSION:{0}", this.Version).AppendLine();
            if(this.Calscale != CALSCALE.UNKNOWN) sb.AppendFormat("CALSCALE:{0}", this.Calscale).AppendLine();
            sb.AppendFormat("PRODID:{0}", this.ProdId).AppendLine();
            foreach (var x in Components) if(x != null) sb.Append(x.ToString()).AppendLine();
            sb.Append("END:VCALENDAR");
            return sb.ToString();
        }


    }

}
