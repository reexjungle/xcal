using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.infrastructure.contracts;
using reexjungle.xcal.infrastructure.serialization;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.foundation.contracts;
using ServiceStack.DataAnnotations;
using reexjungle.xcal.infrastructure.extensions;

namespace reexjungle.xcal.domain.models
{
    [DataContract]
    public abstract class VALARM : IALARM, IContainsKey<Guid>, ICalendarSerializable
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public ACTION Action { get;  set; }

        [DataMember]
        public TRIGGER Trigger { get;  set; }

        [DataMember]
        public DURATION Duration { get;  set; }

        [DataMember]
        public int Repeat { get;  set; }

        protected VALARM(ACTION action)
        {
            Action = action;
        }

        protected VALARM(ACTION action, TRIGGER trigger, DURATION duration = default(DURATION), int repeat = 0)
        {
            if (trigger == null)
                throw new ArgumentNullException(nameof(trigger));

            if (duration != default(DURATION) && repeat == default(int))
                throw new ArgumentException($"{nameof(duration)} and {nameof(repeat)} MUST be valid and occur together");

            if (repeat != default(int) && duration == default(DURATION))
                throw new ArgumentException($"{nameof(duration)} and {nameof(repeat)} MUST be valid and occur together");

            Action = action;
            Trigger = trigger;
            Duration = duration;
            Repeat = repeat;
        }

        public abstract void WriteCalendar(CalendarWriter writer);

        public abstract void ReadCalendar(CalendarReader reader);
    }

    [DataContract]
    [KnownType(typeof(ATTACH_BINARY))]
    [KnownType(typeof(ATTACH_URI))]
    public class AUDIO_ALARM : VALARM, IAUDIO_ALARM, IEquatable<AUDIO_ALARM>
    {
        [DataMember]
        [Ignore]
        public ATTACH Attachment { get; set; }

        public AUDIO_ALARM() : base(ACTION.AUDIO)
        {
        }

        public AUDIO_ALARM(TRIGGER trigger, DURATION duration = default(DURATION), int repeat = 0, ATTACH attachment = null)
            : base(ACTION.AUDIO, trigger, duration, repeat)
        {
            Trigger = trigger;
            Duration = duration;
            Repeat = repeat;
            Attachment = attachment;
        }

        public bool Equals(AUDIO_ALARM other)
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
            return Equals((AUDIO_ALARM)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override void WriteCalendar(CalendarWriter writer)
        {
            if (Trigger == null) return;

            writer.WriteStartComponent("VALARM");
            writer.AppendProperty("ACTION", Action.ToString());

            writer.AppendProperty(Trigger);

            if (Duration != default(DURATION))
            {
                writer.AppendProperty(Duration);
                writer.AppendProperty("REPEAT", Repeat.ToString());
            }

            if (Attachment != null) writer.AppendProperty(Attachment);

            writer.WriteEndComponent("VALARM");
        }

        public override void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(AUDIO_ALARM left, AUDIO_ALARM right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AUDIO_ALARM left, AUDIO_ALARM right)
        {
            return !Equals(left, right);
        }
    }

    [DataContract]
    public class DISPLAY_ALARM : VALARM, IDISPLAY_ALARM, IEquatable<DISPLAY_ALARM>
    {
        [DataMember]
        public DESCRIPTION Description { get; set; }

        public DISPLAY_ALARM() : base(ACTION.DISPLAY)
        {
        }

        public DISPLAY_ALARM(TRIGGER trigger, DESCRIPTION description, DURATION duration = default(DURATION), int repeat = 0)
            : base(ACTION.DISPLAY, trigger, duration, repeat)
        {
            if (description == null)
                throw new ArgumentNullException(nameof(description));

            Action = ACTION.DISPLAY;
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

        public override void WriteCalendar(CalendarWriter writer)
        {
            if (Trigger == null || Description == null) return;

            writer.WriteStartComponent("VALARM");
            writer.AppendProperty("ACTION", Action.ToString());

            writer.AppendProperty(Trigger);

            writer.AppendProperty(Description);

            if (Duration != default(DURATION))
            {
                writer.AppendProperty(Duration);
                writer.AppendProperty("REPEAT", Repeat.ToString());
            }
            writer.WriteEndComponent("VALARM");
        }

        public override void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(DISPLAY_ALARM left, DISPLAY_ALARM right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DISPLAY_ALARM left, DISPLAY_ALARM right)
        {
            return !Equals(left, right);
        }
    }

    [DataContract]
    [KnownType(typeof(ATTACH_BINARY))]
    [KnownType(typeof(ATTACH_URI))]
    public class EMAIL_ALARM : VALARM, IEMAIL_ALARM, IEquatable<EMAIL_ALARM>, ICalendarSerializable
    {
        [DataMember]
        public DESCRIPTION Description { get; set; }

        [DataMember]
        public SUMMARY Summary { get; set; }

        [DataMember]
        [Ignore]
        public List<ATTENDEE> Attendees { get; set; }

        [DataMember]
        [Ignore]
        public List<ATTACH> Attachments { get; set; }

        public EMAIL_ALARM() : base(ACTION.EMAIL)
        {
            Action = ACTION.EMAIL;
            Attendees = new List<ATTENDEE>();
            Attachments = new List<ATTACH>();
        }

        public EMAIL_ALARM(ACTION action, TRIGGER trigger, DESCRIPTION description, SUMMARY summary,
            IEnumerable<ATTENDEE> attendees, DURATION duration = default(DURATION), int repeat = 0, IEnumerable<ATTACH> attachments = null) : base(ACTION.EMAIL, trigger, duration, repeat)
        {
            if (description == null)
                throw new ArgumentNullException(nameof(description));

            if (summary == null)
                throw new ArgumentNullException(nameof(summary));

            if (attendees.NullOrEmpty())
                throw new ArgumentNullException(nameof(attendees));

            Action = action;
            Trigger = trigger;
            Duration = duration;
            Repeat = repeat;
            Description = description;
            Summary = summary;
            Attendees = new List<ATTENDEE>(attendees);
            Attachments = new List<ATTACH>(attachments);
        }

        public bool Equals(EMAIL_ALARM other)
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
            return Equals((EMAIL_ALARM)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override void WriteCalendar(CalendarWriter writer)
        {
            if (Trigger == null || Description == null || Summary == null) return;

            writer.WriteStartComponent("VALARM");

            writer.AppendProperty("ACTION", Action.ToString());

            writer.AppendProperty(Trigger);

            writer.AppendProperty(Description);

            writer.AppendProperty(Summary);

            if (Duration != default(DURATION))
            {
                writer.AppendProperty(Duration);
                writer.AppendProperty("REPEAT", Repeat.ToString());
            }

            if (Attendees.Any()) writer.AppendProperties(Attendees);

            if (Attachments.Any()) writer.AppendProperties(Attachments);

            writer.WriteEndComponent("VALARM");
        }

        public override void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(EMAIL_ALARM left, EMAIL_ALARM right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EMAIL_ALARM left, EMAIL_ALARM right)
        {
            return !Equals(left, right);
        }
    }
}