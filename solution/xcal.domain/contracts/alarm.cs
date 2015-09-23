using reexjungle.xcal.domain.models;
using System.Collections.Generic;

namespace reexjungle.xcal.domain.contracts
{
    /// <summary>
    /// Specifies a contract for alarm objects associated to an iCalendar object.
    /// </summary>
    public interface IALARM
    {
        /// <summary>
        /// Gets or sets the action invoked for the alarm, when the alarm is triggered.
        /// </summary>
        /// <value>
        /// The invoked action for the alarm.
        /// </value>
        ACTION Action { get; set; }

        /// <summary>
        /// Gets or sets when the alarm will trigger.
        /// </summary>
        /// <value>
        /// The trigger for the alarm.The default value type is DURATION.
        /// The value type can be set to a DATE-TIME value type,
        /// in which case the value MUST specify a UTC-formatted DATE-TIME value.
        /// </value>
        TRIGGER Trigger { get; set; }

        /// <summary>
        /// Gets or sets the duration of the alarm.
        /// </summary>
        /// <value>
        /// The specified duration for the alarm.
        /// </value>
        DURATION Duration { get; set; }

        /// <summary>
        /// Gets or sets the number of times the alarm should be repeated after the trigger.
        /// </summary>
        /// <value>
        /// The count to repeat the alarm after the trigger.
        /// </value>
        int Repeat { get; set; }
    }

    /// <summary>
    /// Specifies a contract for audio alarm objects associated to an iCalendar object.
    /// </summary>
    public interface IAUDIO_ALARM : IALARM
    {
        /// <summary>
        /// Gets or sets the sound resource of the alarm that is rendered when the alarm is triggered.
        /// </summary>
        /// <value>
        /// The sound resource of the alarm.
        /// </value>
        ATTACH_BINARY AttachmentBinary { get; set; }

        /// <summary>
        /// Gets or sets the URI link to a sound resource that is rendered when the alarm is triggered.
        /// </summary>
        /// <value>
        /// The URI to the sound resource.
        /// </value>
        ATTACH_URI AttachmentUri { get; set; }
    }

    /// <summary>
    /// Specifies a contract for display alarm objects associated to an iCalendar object.
    /// </summary>
    public interface IDISPLAY_ALARM : IALARM
    {
        /// <summary>
        /// Gets or sets the description, which contains the text to be displayed when the alarm is triggered.
        /// </summary>
        /// <value>
        /// The description containing the text.
        /// </value>
        DESCRIPTION Description { get; set; }
    }

    /// <summary>
    /// Specifies an email contract for alarm objects associated to an iCalendar object.
    /// </summary>
    public interface IEMAIL_ALARM : IALARM
    {
        /// <summary>
        /// Gets or sets the description, which contains the text to be used as the message body of the email alarm.
        /// </summary>
        /// <value>
        /// The description containing the message body text.
        /// </value>
        DESCRIPTION Description { get; set; }

        /// <summary>
        /// Gets or sets the summary, which contains text to be used as the subject of the email alarm.
        /// </summary>
        /// <value>
        /// The summary containing the subject text.
        /// </value>
        SUMMARY Summary { get; set; }

        /// <summary>
        /// Gets or sets the attendees, whose email addresses are used to receive the email message.
        /// </summary>
        /// <value>
        /// The attendees, whose email addresses are used.
        /// </value>
        List<ATTENDEE> Attendees { get; set; }

        /// <summary>
        /// Gets or sets the email attachments to be sent when the alarm email message is sent.
        /// </summary>
        /// <value>
        /// The email attachments.
        /// </value>
        List<ATTACH_BINARY> AttachmentBinaries { get; set; }

        /// <summary>
        /// Gets or sets the URI links of email attachments, when the email message is sent.
        /// </summary>
        /// <value>
        /// The email attachments.
        /// </value>
        List<ATTACH_URI> AttachmentUris { get; set; }
    }
}