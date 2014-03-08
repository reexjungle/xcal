using System;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using reexmonkey.crosscut.essentials.contracts;
using reexmonkey.crosscut.essentials.concretes;
using reexmonkey.xcal.domain.contracts;

namespace reexmonkey.xcal.domain.models
{
        
    [DataContract]
    [KnownType(typeof(TRIGGER))]
    [KnownType(typeof(DURATION))]
    [KnownType(typeof(REPEAT))]
    [KnownType(typeof(AUDIO_ALARM))]
    [KnownType(typeof(DISPLAY_ALARM))]
    [KnownType(typeof(EMAIL_ALARM))]
    public abstract class VALARM: IALARM, IEquatable<VALARM>, IContainsKey<string>
    {     
        protected ITRIGGER trigger;
        protected IDURATION duration;
        protected int repeat;

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public ACTION Action { get; set; }

        [DataMember]
        public ITRIGGER Trigger 
        {
            get { return this.trigger; }
            set 
            {
                //if (value == null) throw new ArgumentNullException("Trigger MUST not be null");
                this.trigger = value; 
            }
        }

        [DataMember]
        public IDURATION Duration
        {
            get { return duration; }
            set
            {
                ////if (value == null && this.repeat != -1) throw new ArgumentException("The Repeat and Duration properties are optional but MUST occur together"); 
                this.duration = value;
            }
        }

        [DataMember]
        public int Repeat
        {
            get { return this.repeat; }
            set
            {
                //if (value != -1 && this.duration != null) throw new ArgumentException("The Repeat and Duration properties are optional but MUST occur together"); 
                this.repeat = value;
            }
        }

        public VALARM()
        {
            this.Action = ACTION.UNKNOWN;
            this.trigger = null;
            this.repeat = -1;
        }

        public VALARM(ACTION action, ITRIGGER trigger)
        {
            this.Action = action;
            this.Trigger = trigger;
        }
     
        public VALARM(ACTION action, ITRIGGER trigger, IDURATION duration, int repeat)
        {
            this.Action = action;
            this.Trigger = trigger;
            this.Duration = duration;
            this.Repeat = repeat;
        }

        public bool Equals(VALARM other)
        {
            return this.Action == other.Action && 
                this.Trigger == other.Trigger && 
                this.Duration == other.Duration && 
                this.Repeat == other.Repeat;
        }
    }

    [DataContract]
    [KnownType(typeof(ATTACH_BINARY))]
    [KnownType(typeof(ATTACH_URI))]
    public class AUDIO_ALARM: VALARM, IAUDIO_ALARM, IEquatable<AUDIO_ALARM>
    {
        [DataMember]
        public IATTACH Attachment { get; set; }

        public AUDIO_ALARM(): base() { }

        public AUDIO_ALARM(ACTION action, ITRIGGER trigger, IATTACH attachment = null): base(action, trigger)
        {
            this.Attachment = attachment;
        }

        public AUDIO_ALARM(ACTION action, ITRIGGER trigger, IDURATION duration, int repeat, IATTACH attachment = null)
            : base(action, trigger, duration, repeat)
        {
            this.Attachment = attachment;
        }

        public bool Equals(AUDIO_ALARM other)
        {
            if (other == null) return false;
            return (this.Action == other.Action && this.Trigger == other.Trigger 
                && this.Duration == other.Duration && this.Repeat == other.Repeat);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:VALARM").AppendLine();
            sb.AppendFormat("ACTION:{0}", this.Action).AppendLine();
            if(this.Trigger != null) sb.AppendFormat("{0}", this.Trigger).AppendLine();
            if (this.Duration != null && this.Repeat != -1)
            {
                sb.AppendFormat("{0}", this.Duration).AppendLine();
                sb.AppendFormat("REPEAT:{0}", this.Repeat).AppendLine();
            }
            if(this.Attachment != null)  sb.AppendFormat("{0}", this.Attachment).AppendLine();
            sb.Append("END:VALARM").AppendLine();
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return this.Equals(obj as AUDIO_ALARM);
        }

        public override int GetHashCode()
        {
            return this.Action.GetHashCode() ^
                ((this.Trigger != null)? this.Trigger.GetHashCode(): 0)^ 
                ((this.Duration != null)? this.Duration.GetHashCode(): 0)^
                this.Repeat.GetHashCode() ^
                ((this.Attachment != null)? this.Attachment.GetHashCode(): 0);
        }

        public static bool operator ==(AUDIO_ALARM a, AUDIO_ALARM b)
        {
            if ((object)a == null || (object)b == null) return object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(AUDIO_ALARM a, AUDIO_ALARM b)
        {
            if (a == null || b == null) return !object.Equals(a, b);
            return !a.Equals(b);
        }

        public bool IsDefault()
        {
            return this.Action == ACTION.UNKNOWN &&
                ((this.Trigger != null) ? this.Trigger.IsDefault() : true) &&
                ((this.Duration != null) ? this.Duration.IsDefault() : true) &&
                (this.Repeat != -1) &&
                ((this.Attachment != null) ? this.Attachment.IsDefault() : true);
        }
    }

    [DataContract]
    [KnownType(typeof(DESCRIPTION))]
    public class DISPLAY_ALARM: VALARM, IDALARM, IEquatable<DISPLAY_ALARM>
    {
        private ITEXT description;

        [DataMember]
        public ITEXT Description 
        {
            get { return this.description; }
            set 
            {
                //if (value == null) throw new ArgumentNullException("Description MUST not be null");
                this.description = value;
            }
        }

        public DISPLAY_ALARM(): base(){}

        public DISPLAY_ALARM(ACTION action, ITRIGGER trigger, ITEXT description): base(action, trigger)
        {
            this.Description = description;
        }

        public DISPLAY_ALARM(ACTION action, ITRIGGER trigger, IDURATION duration, int repeat, ITEXT description = null): base(action, trigger, duration, repeat)
        {
            this.Description = description;
        }

        public bool Equals(DISPLAY_ALARM other)
        {
            if (other == null) return false;
            return (this.Action == other.Action && 
                this.Trigger == other.Trigger && 
                this.Duration == other.Duration && 
                this.Repeat == other.Repeat) &&
                this.Description == other.Description;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:VALARM").AppendLine();
            sb.AppendFormat("ACTION:{0}", this.Action).AppendLine();
            if (this.Description != null) sb.AppendFormat("{0}", this.Description).AppendLine();
            if (this.Duration != null && this.Repeat != -1)
            {
                sb.AppendFormat("{0}", this.Duration).AppendLine();
                sb.AppendFormat("REPEAT:{0}", this.Repeat).AppendLine();
            }
            sb.Append("END:VALARM").AppendLine();
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return this.Equals(obj as DISPLAY_ALARM);
        }

        public override int GetHashCode()
        {
            return this.Action.GetHashCode() ^
                ((this.Trigger != null) ? this.Trigger.GetHashCode() : 0) ^
                ((this.Duration != null) ? this.Duration.GetHashCode() : 0) ^
                ((this.Repeat != -1) ? this.Repeat.GetHashCode() : 0) ^
                ((this.Description != null) ? this.Description.GetHashCode() : 0);
        }

        public static bool operator ==(DISPLAY_ALARM a, DISPLAY_ALARM b)
        {
            if ((object)a == null || (object)b == null) return object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(DISPLAY_ALARM a, DISPLAY_ALARM b)
        {
            if (a == null || b == null) return !object.Equals(a, b);
            return !a.Equals(b);
        }

        public bool IsDefault()
        {
            return this.Action == ACTION.UNKNOWN &&
                ((this.Trigger != null) ? this.Trigger.IsDefault() : true) &&
                ((this.Duration != null) ? this.Duration.IsDefault() : true) &&
                (this.Repeat != -1) &&
                ((this.Description != null) ? this.Description.IsDefault() : true);

        }
    }

    [DataContract]
    [KnownType(typeof(ATTACH_BINARY))]
    [KnownType(typeof(ATTACH_URI))]
    [KnownType(typeof(DESCRIPTION))]
    [KnownType(typeof(SUMMARY))]
    public class EMAIL_ALARM : VALARM, IEMAIL_ALARM, IEquatable<EMAIL_ALARM>
    {
        private ITEXT description;
        private ITEXT summary;

        [DataMember]
        public ITEXT Description 
        {
            get { return this.description; }
            set
            {
                //if (value == null) throw new ArgumentNullException("Description MUST be null");
                this.description = value;
            }
        }

        [DataMember]
        public ITEXT Summary 
        {
            get { return this.summary; }
            set
            {
                //if (value == null) throw new ArgumentNullException("Summary MUST be null");
                this.summary = value;
            }
        }

        [DataMember]
        [Ignore]
        public List<IATTENDEE> Attendees { get; set; }

        [DataMember]
        [Ignore]
        public List<IATTACH> Attachments { get; set; }


        public EMAIL_ALARM() : base() 
        {
            this.Attendees = new List<IATTENDEE>();
            this.Attachments = new List<IATTACH>();
        }

        public EMAIL_ALARM(ACTION action, ITRIGGER trigger, ITEXT description, ITEXT summary, 
            List<IATTENDEE> attendees, List<IATTACH> attachments = null)
            : base(action, trigger)
        {
            this.Description = description;
            this.Summary = summary;
            this.Attendees = attendees;
            this.Attachments = attachments;

        }

        public EMAIL_ALARM(ACTION action, ITRIGGER trigger, IDURATION duration, int repeat, ITEXT description, ITEXT summary, 
            List<IATTENDEE> attendees, List<IATTACH> attachments = null)
            : base(action, trigger, duration, repeat)
        {
            this.Description = description;
            this.Summary = summary;
            this.Attendees = attendees;
            this.Attachments = attachments;
        }

        public bool Equals(EMAIL_ALARM other)
        {
            if (other == null) return false;
            return this.Action == other.Action && 
                this.Trigger == other.Trigger && 
                this.Duration == other.Duration && 
                this.Repeat == other.Repeat &&
                this.Description == other.Description &&
                this.Summary == other.Summary &&
                ((!this.Attendees.NullOrEmpty())? this.Attendees.AreDuplicatesOf(other.Attendees): true) &&
                ((!this.Attachments.NullOrEmpty())? this.Attachments.AreDuplicatesOf(other.Attachments): true);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:VALARM").AppendLine();
            if (this.Description != null) sb.AppendFormat("{0}", this.Description).AppendLine();
            if (this.Summary != null) sb.AppendFormat("{0}", this.Summary).AppendLine();
            if (this.Duration != null && this.Repeat != -1)
            {
                sb.AppendFormat("{0}", this.Duration).AppendLine();
                sb.AppendFormat("{0}", this.Repeat).AppendLine();
            }
            foreach (var attendee in this.Attendees) sb.Append(attendee);
            foreach (var attachment in this.Attachments) sb.Append(attachment);
            sb.Append("END:VALARM").AppendLine();
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return this.Equals(obj as EMAIL_ALARM);
        }

        public override int GetHashCode()
        {
            return this.Action.GetHashCode() ^
                ((this.Trigger != null)? this.Trigger.GetHashCode(): 0) ^
                ((this.Duration != null)? this.Duration.GetHashCode(): 0) ^
                (this.Repeat.GetHashCode()) ^
                ((this.Description != null) ? this.Description.GetHashCode() : 0) ^
                ((this.Summary != null) ? this.Description.GetHashCode() : 0) ^
                ((!this.Attendees.NullOrEmpty()) ? this.Attendees.GetHashCode() : 0) ^
                ((!this.Attachments.NullOrEmpty()) ? this.Attachments.GetHashCode() : 0);
        }

        public static bool operator ==(EMAIL_ALARM a, EMAIL_ALARM b)
        {
            if ((object)a == null || (object)b == null) return object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(EMAIL_ALARM a, EMAIL_ALARM b)
        {
            if (a == null || b == null) return !object.Equals(a, b);
            return !a.Equals(b);
        }

        public bool IsDefault()
        {
            return this.Action == ACTION.UNKNOWN &&
                ((this.Trigger != null) ? this.Trigger.IsDefault() : true) &&
                ((this.Duration != null) ? this.Duration.IsDefault() : true) &&
                (this.Repeat != -1) &&
                ((this.Description != null)? this.Description.IsDefault(): true) &&
                ((this.Summary != null)? this.summary.IsDefault(): true) &&
                this.Attendees.NullOrEmpty() &&
                this.Attachments.NullOrEmpty();
        }

    }

    
   
}
