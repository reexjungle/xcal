using System;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using reexjungle.foundation.essentials.contracts;
using reexjungle.foundation.essentials.concretes;
using reexjungle.xcal.domain.contracts;

namespace reexjungle.xcal.domain.models
{
        
    [DataContract]
    public class AUDIO_ALARM : IAUDIO_ALARM, IEquatable<AUDIO_ALARM>, IContainsKey<string>
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public ACTION Action { get; set; }

        [DataMember]
        public TRIGGER Trigger { get; set; }

        [DataMember]
        public DURATION Duration { get; set; }

        [DataMember]
        public int Repeat { get; set; }

        [DataMember]
        [Ignore]
        public ATTACH_BINARY AttachmentBinary { get; set; }

        [DataMember]
        [Ignore]
        public ATTACH_URI AttachmentUri { get; set; }

        public AUDIO_ALARM(): base() 
        {
            this.Action = ACTION.AUDIO;
        }

        public AUDIO_ALARM(ACTION action, TRIGGER trigger, ATTACH_BINARY attachbin = null, ATTACH_URI attachuri = null)
        {
            this.Action = action;
            this.Trigger = trigger;
            this.AttachmentBinary = attachbin;
            this.AttachmentUri = attachuri;
        }

        public AUDIO_ALARM(ACTION action, TRIGGER trigger, DURATION duration, int repeat, ATTACH_BINARY attachbin = null, ATTACH_URI attachuri = null)
        {
            this.Action = action;
            this.Trigger = trigger;
            this.Duration = duration;
            this.Repeat = repeat;
            this.AttachmentBinary = attachbin;
            this.AttachmentUri = attachuri;
        }

        public bool Equals(AUDIO_ALARM other)
        {
            if (other == null) return false;
            return this.Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:VALARM").AppendLine();
            sb.AppendFormat("ACTION:{0}", this.Action.ToString()).AppendLine();
            if(this.Trigger != null) sb.Append(this.Trigger.ToString()).AppendLine();
            if (this.Duration != default(DURATION))
            {
                sb.AppendFormat("DURATION:{0}", this.Duration.ToString()).AppendLine();
                sb.AppendFormat("REPEAT:{0}", this.Repeat.ToString()).AppendLine();
            }
            if(this.AttachmentBinary != null)  sb.Append(this.AttachmentBinary.ToString()).AppendLine();
            else if (this.AttachmentUri != null)sb.Append(this.AttachmentUri.ToString()).AppendLine();
            sb.Append("END:VALARM");
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return this.Equals(obj as AUDIO_ALARM);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static bool operator ==(AUDIO_ALARM a, AUDIO_ALARM b)
        {
            if ((object)a == null || (object)b == null) return object.Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(AUDIO_ALARM a, AUDIO_ALARM b)
        {
            if ((object)a == null || (object)b == null) return !object.Equals(a, b);
            return !a.Equals(b);
        }

    }

    [DataContract]
    public class DISPLAY_ALARM : IDISPLAY_ALARM, IEquatable<DISPLAY_ALARM>, IContainsKey<string>
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public ACTION Action { get; set; }

        [DataMember]
        public TRIGGER Trigger { get; set; }

        [DataMember]
        public DURATION Duration { get; set; }

        [DataMember]
        public int Repeat { get; set; }

        [DataMember]
        public DESCRIPTION Description { get; set; }

        public DISPLAY_ALARM(): base()
        {
            this.Action = ACTION.DISPLAY;
        }

        public DISPLAY_ALARM(ACTION action, TRIGGER trigger, DESCRIPTION description)
        {
            this.Action = action;
            this.Trigger = trigger;
            this.Description = description;
        }

        public DISPLAY_ALARM(ACTION action, TRIGGER trigger, DURATION duration, int repeat, DESCRIPTION description = null)
        {
            this.Action = action;
            this.Trigger = trigger;
            this.Duration = duration;
            this.Repeat = repeat;
            this.Description = description;
        }

        public bool Equals(DISPLAY_ALARM other)
        {
            if (other == null) return false;
            return this.Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:VALARM").AppendLine();
            sb.AppendFormat("ACTION:{0}", this.Action.ToString()).AppendLine();
            if (this.Description != null) sb.AppendFormat("{0}", this.Description.ToString()).AppendLine();
            if (this.Trigger != null) sb.Append(this.Trigger.ToString()).AppendLine();
            if (this.Duration != default(DURATION))
            {
                sb.AppendFormat("DURATION:{0}", this.Duration.ToString()).AppendLine();
                sb.AppendFormat("REPEAT:{0}", this.Repeat.ToString()).AppendLine();
            }
            sb.Append("END:VALARM");
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
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
    }

    [DataContract]
    public class EMAIL_ALARM : IEMAIL_ALARM, IEquatable<EMAIL_ALARM>, IContainsKey<string>
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public ACTION Action { get; set; }

        [DataMember]
        public TRIGGER Trigger { get; set; }

        [DataMember]
        public DURATION Duration { get; set; }

        [DataMember]
        public int Repeat { get; set; }

        [DataMember]
        public DESCRIPTION Description { get; set; }

        [DataMember]
        public SUMMARY Summary { get; set; }

        [DataMember]
        [Ignore]
        public List<ATTENDEE> Attendees { get; set; }

        [DataMember]
        [Ignore]
        public List<ATTACH_BINARY> AttachmentBinaries { get; set; }

        [DataMember]
        [Ignore]
        public List<ATTACH_URI> AttachmentUris { get; set; }


        public EMAIL_ALARM() : base() 
        {
            this.Action = ACTION.EMAIL;
            this.Attendees = new List<ATTENDEE>();
            this.AttachmentBinaries = new  List<ATTACH_BINARY>();
            this.AttachmentUris = new List<ATTACH_URI>();
        }

        public EMAIL_ALARM(ACTION action, TRIGGER trigger, DESCRIPTION description, SUMMARY summary, 
            List<ATTENDEE> attendees, List<ATTACH_BINARY> attachbins = null, List<ATTACH_URI> attachuris = null)
        {
            this.Action = action;
            this.Trigger = trigger;
            this.Description = description;
            this.Summary = summary;
            this.Attendees = attendees;
            this.AttachmentBinaries = attachbins;
            this.AttachmentUris = attachuris;

        }

        public EMAIL_ALARM(ACTION action, TRIGGER trigger, DURATION duration, int repeat, DESCRIPTION description, SUMMARY summary,
            List<ATTENDEE> attendees, List<ATTACH_BINARY> attachbins = null, List<ATTACH_URI> attachuris = null)
        {
            this.Action = action;
            this.Trigger = trigger;
            this.Duration = duration;
            this.Repeat = repeat;
            this.Description = description;
            this.Summary = summary;
            this.Attendees = attendees;
            this.AttachmentBinaries = attachbins;
            this.AttachmentUris = attachuris;
        }

        public bool Equals(EMAIL_ALARM other)
        {
            if (other == null) return false;
            return this.Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:VALARM").AppendLine();
            sb.AppendFormat("ACTION:{0}", this.Action.ToString()).AppendLine();
            if (this.Description != null) sb.AppendFormat("{0}", this.Description.ToString()).AppendLine();
            if (this.Summary != null) sb.AppendFormat("{0}", this.Summary.ToString()).AppendLine();
            if (this.Trigger != null) sb.Append(this.Trigger.ToString()).AppendLine();
            if (this.Duration != null && this.Repeat != -1)
            {
                sb.AppendFormat("DURATION:{0}", this.Duration.ToString()).AppendLine();
                sb.AppendFormat("REPEAT:{0}", this.Repeat.ToString()).AppendLine();
            }
            foreach (var attendee in this.Attendees) sb.Append(attendee.ToString()).AppendLine();
            if(!this.AttachmentBinaries.NullOrEmpty())
                foreach (var attachment in this.AttachmentBinaries) sb.Append(attachment.ToString()).AppendLine();
            else if(!this.AttachmentUris.NullOrEmpty())
                foreach (var attachment in this.AttachmentUris) sb.Append(attachment.ToString()).AppendLine();
            sb.Append("END:VALARM");
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return this.Equals(obj as EMAIL_ALARM);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
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

    }

    
   
}
