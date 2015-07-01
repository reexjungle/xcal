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
    [DataContract]
    public class AUDIO_ALARM : IAUDIO_ALARM, IEquatable<AUDIO_ALARM>, IContainsKey<Guid>
    {
        [DataMember]
        public Guid Id { get; set; }

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

        public AUDIO_ALARM()
            : base()
        {
            Action = ACTION.AUDIO;
        }

        public AUDIO_ALARM(ACTION action, TRIGGER trigger, ATTACH_BINARY attachbin = null, ATTACH_URI attachuri = null)
        {
            Action = action;
            Trigger = trigger;
            AttachmentBinary = attachbin;
            AttachmentUri = attachuri;
        }

        public AUDIO_ALARM(ACTION action, TRIGGER trigger, DURATION duration, int repeat, ATTACH_BINARY attachbin = null, ATTACH_URI attachuri = null)
        {
            Action = action;
            Trigger = trigger;
            Duration = duration;
            Repeat = repeat;
            AttachmentBinary = attachbin;
            AttachmentUri = attachuri;
        }

        public bool Equals(AUDIO_ALARM other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:VALARM").AppendLine();
            sb.AppendFormat("ACTION:{0}", Action).AppendLine();
            if (Trigger != null) sb.Append(Trigger).AppendLine();
            if (Duration != default(DURATION))
            {
                sb.AppendFormat("DURATION:{0}", Duration).AppendLine();
                sb.AppendFormat("REPEAT:{0}", Repeat).AppendLine();
            }
            if (AttachmentBinary != null) sb.Append(AttachmentBinary).AppendLine();
            else if (AttachmentUri != null) sb.Append(AttachmentUri).AppendLine();
            sb.Append("END:VALARM");
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AUDIO_ALARM)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(AUDIO_ALARM a, AUDIO_ALARM b)
        {
            if ((object)a == null || (object)b == null) return Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(AUDIO_ALARM a, AUDIO_ALARM b)
        {
            if ((object)a == null || (object)b == null) return !Equals(a, b);
            return !a.Equals(b);
        }
    }

    [DataContract]
    public class DISPLAY_ALARM : IDISPLAY_ALARM, IEquatable<DISPLAY_ALARM>, IContainsKey<Guid>
    {
        [DataMember]
        public Guid Id { get; set; }

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

        public DISPLAY_ALARM()
            : base()
        {
            Action = ACTION.DISPLAY;
        }

        public DISPLAY_ALARM(ACTION action, TRIGGER trigger, DESCRIPTION description)
        {
            Action = action;
            Trigger = trigger;
            Description = description;
        }

        public DISPLAY_ALARM(ACTION action, TRIGGER trigger, DURATION duration, int repeat, DESCRIPTION description = null)
        {
            Action = action;
            Trigger = trigger;
            Duration = duration;
            Repeat = repeat;
            Description = description;
        }

        public bool Equals(DISPLAY_ALARM other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:VALARM").AppendLine();
            sb.AppendFormat("ACTION:{0}", Action).AppendLine();
            if (Description != null) sb.AppendFormat("{0}", Description).AppendLine();
            if (Trigger != null) sb.Append(Trigger).AppendLine();
            if (Duration != default(DURATION))
            {
                sb.AppendFormat("DURATION:{0}", Duration).AppendLine();
                sb.AppendFormat("REPEAT:{0}", Repeat).AppendLine();
            }
            sb.Append("END:VALARM");
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DISPLAY_ALARM)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(DISPLAY_ALARM a, DISPLAY_ALARM b)
        {
            if ((object)a == null || (object)b == null) return Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(DISPLAY_ALARM a, DISPLAY_ALARM b)
        {
            if (a == null || b == null) return !Equals(a, b);
            return !a.Equals(b);
        }
    }

    [DataContract]
    public class EMAIL_ALARM : IEMAIL_ALARM, IEquatable<EMAIL_ALARM>, IContainsKey<Guid>
    {
        [DataMember]
        public Guid Id { get; set; }

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

        public EMAIL_ALARM()
            : base()
        {
            Action = ACTION.EMAIL;
            Attendees = new List<ATTENDEE>();
            AttachmentBinaries = new List<ATTACH_BINARY>();
            AttachmentUris = new List<ATTACH_URI>();
        }

        public EMAIL_ALARM(ACTION action, TRIGGER trigger, DESCRIPTION description, SUMMARY summary,
            List<ATTENDEE> attendees, List<ATTACH_BINARY> attachbins = null, List<ATTACH_URI> attachuris = null)
        {
            Action = action;
            Trigger = trigger;
            Description = description;
            Summary = summary;
            Attendees = attendees;
            AttachmentBinaries = attachbins;
            AttachmentUris = attachuris;
        }

        public EMAIL_ALARM(ACTION action, TRIGGER trigger, DURATION duration, int repeat, DESCRIPTION description, SUMMARY summary,
            List<ATTENDEE> attendees, List<ATTACH_BINARY> attachbins = null, List<ATTACH_URI> attachuris = null)
        {
            Action = action;
            Trigger = trigger;
            Duration = duration;
            Repeat = repeat;
            Description = description;
            Summary = summary;
            Attendees = attendees;
            AttachmentBinaries = attachbins;
            AttachmentUris = attachuris;
        }

        public bool Equals(EMAIL_ALARM other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BEGIN:VALARM").AppendLine();
            sb.AppendFormat("ACTION:{0}", Action).AppendLine();
            if (Description != null) sb.AppendFormat("{0}", Description).AppendLine();
            if (Summary != null) sb.AppendFormat("{0}", Summary).AppendLine();
            if (Trigger != null) sb.Append(Trigger).AppendLine();
            if (Duration != default(DURATION) && Repeat != -1)
            {
                sb.AppendFormat("DURATION:{0}", Duration).AppendLine();
                sb.AppendFormat("REPEAT:{0}", Repeat).AppendLine();
            }
            foreach (var attendee in Attendees) sb.Append(attendee).AppendLine();
            if (!AttachmentBinaries.NullOrEmpty())
                foreach (var attachment in AttachmentBinaries) sb.Append(attachment).AppendLine();
            else if (!AttachmentUris.NullOrEmpty())
                foreach (var attachment in AttachmentUris) sb.Append(attachment).AppendLine();
            sb.Append("END:VALARM");
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EMAIL_ALARM)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(EMAIL_ALARM a, EMAIL_ALARM b)
        {
            if ((object)a == null || (object)b == null) return Equals(a, b);
            return a.Equals(b);
        }

        public static bool operator !=(EMAIL_ALARM a, EMAIL_ALARM b)
        {
            if (a == null || b == null) return !Equals(a, b);
            return !a.Equals(b);
        }
    }
}