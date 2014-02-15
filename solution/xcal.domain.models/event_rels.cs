using System.Runtime.Serialization;

namespace reexmonkey.xcal.domain.models
{
    [DataContract]
    public class REL_EVENTS_ORGANIZERS
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-organizer relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related organizer entity
        /// </summary>
        [DataMember]
        public string OrganizerId { get; set; }
    }

    [DataContract]
    public class REL_EVENTS_RECURRENCE_IDS
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-recurrence ID relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related recurrence identifier entity
        /// </summary>
        [DataMember]
        public string RecurrenceId_Id { get; set; }
    }

    [DataContract]
    public class REL_EVENTS_RRULES
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-recurrence relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related recurrence identifier entity
        /// </summary>
        [DataMember]
        public string RecurrenceRuleId { get; set; }


    }

    [DataContract]
    public class REL_EVENTS_ATTACHMENTS
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [DataMember]
        public string AttachmentId { get; set; }
    }

    [DataContract]
    public class REL_EVENTS_ATTENDEES
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-attendee relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related attendee identifier entity
        /// </summary>
        [DataMember]
        public string AttendeeId { get; set; }
    }

    [DataContract]
    public class REL_EVENTS_COMMENTS
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-comment relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related comment entity
        /// </summary>
        [DataMember]
        public string CommentId { get; set; }
    }

    [DataContract]
    public class REL_EVENTS_CONTACTS
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-contact relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related contact entity
        /// </summary>
        [DataMember]
        public string ContactId { get; set; }
    }

    [DataContract]
    public class REL_EVENTS_RDATES
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-recurrence date relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related recurrence date entity
        /// </summary>
        [DataMember]
        public string RecurrenceDateId { get; set; }
    }

    [DataContract]
    public class REL_EVENTS_EXDATES
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-exception date relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related exception date entity
        /// </summary>
        [DataMember]
        public string ExceptionDateId { get; set; }
    }

    [DataContract]
    public class REL_EVENTS_RELATEDTOS
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-related to relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the relation entity
        /// </summary>
        [DataMember]
        public string RelatedToId { get; set; }
    }

    [DataContract]
    public class REL_EVENTS_REQSTATS
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-request status relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related request status entity
        /// </summary>
        [DataMember]
        public string ReqStatId { get; set; }
    }

    [DataContract]
    public class REL_EVENTS_RESOURCES
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-resources relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related resources-entity
        /// </summary>
        [DataMember]
        public string ResourceId { get; set; }
    }

    [DataContract]
    public class REL_EVENTS_ALARMS
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event-alarm relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related alarm entity
        /// </summary>
        [DataMember]
        public string AlarmId { get; set; }
    }

}
