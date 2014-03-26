using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ServiceStack.ServiceHost;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;

namespace reexmonkey.xcal.domain.operations
{

    #region VEVENT RFC 5546 operations

    /// <summary>
    /// Represents class to request for poisting notification of an event (incl. available recurrent instances)
    /// </summary>
    [DataContract]
    [Api ("Post notification of an event. Used primarily as a method of advertising the existence of an  event.")]
    [Route("/calendars/{ProductId}/events/publish", "GET", Summary="Publishes an event")]
    public class PublishEvent : IReturn<VCALENDAR>
    {
        /// <summary>
        /// Gets or sets the events to be published.
        /// </summary>
        [DataMember]
        public List<VEVENT> Events { get; set; }

        /// <summary>
        /// Gets or sets the time zones if any dtate time of the events refer to a timezone
        /// </summary>
        [DataMember]
        public List<VTIMEZONE> TimeZones { get; set; }

        /// <summary>
        /// Gets or sets the Product Id of the calendar, under which the events are published
        /// </summary>
        [DataMember]
        public string ProductId { get; set; }
       
    }

    /// <summary>
    /// Represents request class for the rescheduling of events
    /// </summary>
    [DataContract]
    [Api ("Reschedules an event or events. This involves a change in terms of time or recurrence intervals and possibly the location or description.")]
    [Route("/calendars/{ProductId}/event/reschedule", "PATCH", Summary="Reschedules an event or events", Notes="All the events must share the same UID")]
    public class RescheduleEvent : IReturn<VCALENDAR>
    {
        /// <summary>
        /// Gets or sets the events to be published.
        /// </summary>
        [DataMember]
        public List<VEVENT> Events { get; set; }

        /// <summary>
        /// Gets or sets the time zones if any date time of the events refer to a timezone
        /// </summary>
        [DataMember]
        public List<VTIMEZONE> TimeZones { get; set; }

        /// <summary>
        /// Gets or sets the Product Id of the calendar, under which the events are published
        /// </summary>
        [DataMember]
        public string ProductId { get; set; }

    }

    /// <summary>
    /// Represents request class to update or reconfirm events
    /// </summary>
    [DataContract]
    [Route("/calendars/{ProductId}/event/update", "PUT")]
    public class UpdateEvent : IReturn<VCALENDAR>
    {

        /// <summary>
        /// Gets or sets the Product Id of the calendar, under which the events are published
        /// </summary>
        [DataMember]
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the events to be updated or reconfirmed.
        /// </summary>
        /// <exception cref="System.ArgumentException">Thrown when organizers of events are not present, or when the list of events are null or empty</exception>
        [DataMember]
        public List<VEVENT> Events { get; set; }

        [DataMember]
        public List<VTIMEZONE> TimeZones { get; set; }
    }
    
    /// <summary>
    /// Represents request class to delegate events from calendar users to others
    /// </summary>
    [DataContract]
    [Route("/calendars/{ProductId}/event/delegate", "POST")]
    public class DelegateEvent : IReturn<VCALENDAR>
    {

        /// <summary>
        /// Gets or sets the Product Id of the calendar, under which the events are published
        /// </summary>
        [DataMember]
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the events to be delegated.
        /// </summary>
        [DataMember]
        public List<VEVENT> Events { get; set; }

    }

    /// <summary>
    /// Represents request class to update or reconfirm events
    /// </summary>
    [DataContract]
    [Route("/calendars/{ProductId}/event/organizers/change", "PUT")]
    public class ChangeOrganizer : IReturn<VCALENDAR>
    {
        [DataMember]
        public List<VEVENT> Events { get; set; }

        /// <summary>
        /// Gets or sets the Product Id of the calendar, under which the events are published
        /// </summary>
        [DataMember]
        public string ProductId { get; set; }

        public ChangeOrganizer()
        {
            this.Events = new List<VEVENT>();
        }
    }

    [DataContract]
    [Route("/calendars/{ProductId}/event/organizers/represent", "POST")]
    public class SendOnBehalf : IReturn<VCALENDAR>
    {
        [DataMember]
        public List<VEVENT> Events { get; set; }

        /// <summary>
        /// Gets or sets the Product Id of the calendar
        /// </summary>
        [DataMember]
        public string ProductId { get; set; }

        public SendOnBehalf()
        {
            this.Events = new List<VEVENT>();
        }
    }

    [DataContract]
    [Route("/calendars/{ProductId}/event/forward", "POST")]
    public class ForwardEvent: IReturn<VCALENDAR>
    {
        [DataMember]
        public List<VEVENT> Events { get; set; }

        [DataMember]
        public Dictionary<UID, List<ATTENDEE>> Invited { get; set; }

        /// <summary>
        /// Gets or sets the Product Id of the calendar
        /// </summary>
        [DataMember]
        public string ProductId { get; set; }  
 
        public ForwardEvent()
        {
            this.Events = new List<VEVENT>();
            this.Invited = new Dictionary<UID, List<ATTENDEE>>();
        }
    }

    [DataContract]
    [Route("/calendars/{ProductId}/event/attendees/status/update", "PUT")]
    public class UpdateAttendeesStatus : IReturn<VCALENDAR>
    {
        [DataMember]
        public List<VEVENT> Events { get; set; }

        [DataMember]
        public Dictionary<UID, List<ATTENDEE>> Updates { get; set; }

        /// <summary>
        /// Gets or sets the Product Id of the calendar
        /// </summary>
        [DataMember]
        public string ProductId { get; set; }

        public UpdateAttendeesStatus()
        {
            this.Events = new List<VEVENT>();
            this.Updates = new Dictionary<UID, List<ATTENDEE>>();
        }

    }

    [DataContract]
    [Route("/calendars/{ProductId}/event/reply", "POST")]
    public class ReplyToEvent: IReturn<VCALENDAR>
    {

        /// <summary>
        /// Gets or sets the event to be rescheduled
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when event is null</exception>
        /// <exception cref="System.ArgumentException">Thrown when organizer event is not present</exception>
        public List<VEVENT> Events { get; set; }

        /// <summary>
        /// Gets or sets the Product Id of the calendar, under which the events are published
        /// </summary>
        [DataMember]
        public string ProductId { get; set; }

    }


    [DataContract]
    [Route("/calendars/{ProductId}/event/add", "PUT")]
    public class AddToEvent: IReturn<VCALENDAR>
    {

        [DataMember]
        public string ProductId { get; set; }

        public UID Uid { get; set; }

        [DataMember]
        public DATE_TIME StartDate { get; set; }

        [DataMember]
        public CLASS Classification { get; set; }

        [DataMember]
        public DESCRIPTION Description { get; set; }

        [DataMember]
        public GEO Geo { get; set; }

        [DataMember]
        public LOCATION Location { get; set; }

        [DataMember]
        public ORGANIZER Organizer { get; set; }

        [DataMember]
        public PRIORITY Priority { get; set; }

        [DataMember]
        public int Sequence { get; set; }

        [DataMember]
        public STATUS Status { get; set; }

        [DataMember]
        public ITEXT Summary { get; set; }

        [DataMember]
        public TRANSP Transparency { get; set; }

        [DataMember]
        public URL Url { get; set; }

        [DataMember]
        public RECURRENCE_ID RecurrenceId { get; set; }

        [DataMember]
        public RRULE RecurrenceRule { get; set; }

        [DataMember]
        public DATE_TIME EndDate { get; set; }

        [DataMember]
        public DURATION Duration { get; set; }

        [DataMember]
        public List<ATTACH_BINARY> AttachmentBinaries { get; set; }

        [DataMember]
        public List<ATTACH_URI> AttachmentUris { get; set; }

        [DataMember]
        public List<ATTENDEE> Attendees { get; set; }

        [DataMember]
        public CATEGORIES Categories { get; set; }

        [DataMember]
        public List<COMMENT> Comments { get; set; }

        [DataMember]
        public List<CONTACT> Contacts { get; set; }

        [DataMember]
        public List<EXDATE> ExceptionDates { get; set; }

        [DataMember]
        public List<REQUEST_STATUS> RequestStatuses { get; set; }

        [DataMember]
        public List<RESOURCES> Resources { get; set; }

        [DataMember]
        public List<RELATEDTO> RelatedTos { get; set; }

        [DataMember]
        public List<RDATE> RecurrenceDates { get; set; }

        [DataMember]
        public List<AUDIO_ALARM> AudioAlarms { get; set; }

        [DataMember]
        public List<DISPLAY_ALARM> DisplayAlarms { get; set; }

        [DataMember]
        public List<EMAIL_ALARM> EmailAlarms { get; set; }

        [DataMember]
        public List<VTIMEZONE> TimeZones { get; set; }
    }


    [DataContract]
    [Route("/calendars/{ProductId}/events/cancel", "POST")]
    public class CancelEvent : IReturn<VCALENDAR>
    {
        [DataMember]
        public string ProductId { get; set; }

        [DataMember]
        public List<VEVENT> Events { get; set; }

        public List<VTIMEZONE> TimeZones { get; set; }
    }


    [DataContract]
    [Route("/calendars/{ProductId}/events/refresh", "POST")]
    public class RefreshEvent : IReturn<VCALENDAR>
    {
        [DataMember]
        public string ProductId { get; set; }

        [DataMember]
        public ATTENDEE Attendee {get; set;}

        [DataMember]
        public DATE_TIME DateTimeStamp {get; set;}

        [DataMember]
        public ORGANIZER Organizer {get; set;}

        [DataMember]
        public UID Uid {get; set;}

        [DataMember]
        public List<COMMENT> Comments { get; set; }

        [DataMember]
        public RECURRENCE_ID RecurId { get; set; }

        [DataMember]
        public List<VTIMEZONE> TimeZones{get; set;}

        public RefreshEvent(string prodid, UID uid, ATTENDEE attendee, ORGANIZER organizer, DATE_TIME dtstamp, List<VTIMEZONE> timezones = null)
        {
            this.ProductId = prodid;
            this.Uid = uid;
            this.Attendee = attendee;
            this.Organizer = organizer;
            this.DateTimeStamp = dtstamp;
            this.TimeZones = timezones;
        }

    }

    [DataContract]
    [Route("/calendars/{ProductId}/events/counter", "POST")]
    public class CounterEvent: IReturn<VCALENDAR>
    {
        [DataMember]
        public string ProductId { get; set; }

        [DataMember]
        public VEVENT Alternative { get; set; }

        [DataMember]
        public List<VTIMEZONE> TimeZones { get; set; }

        public CounterEvent(string prodid, VEVENT alternative, List<VTIMEZONE> timezones = null)
        {
            this.ProductId = prodid;
            this.Alternative = alternative;
            this.TimeZones = timezones;
        }
    }

    [DataContract]
    [Route("/calendars/{ProductId}/event/counter", "POST")]
    public class DeclineCounterEvent: IReturn<VCALENDAR>
    {
                
        [DataMember]
        public string ProductId { get; set; }

        [DataMember]
        public List<VEVENT> Events { get; set; }

        public List<VTIMEZONE> TimeZones { get; set; }

        public DeclineCounterEvent(string prodid, List<VEVENT> events, List<VTIMEZONE> timezones = null)
        {
            this.ProductId = prodid;
            this.Events = events;
            this.TimeZones = timezones;
        }

        public DeclineCounterEvent(string prodid, params VEVENT[] events)
        {
            this.ProductId = prodid;
            this.Events = events.ToList();
        }
    }

    #endregion

    #region VEVENT general operations

    [DataContract]
    [Route("/calendars/{ProductId}/events/{Uid}/{Page}", "GET")]
    [Route("/calendars/{ProductId}/events/page/{Page}", "GET")]
    [Route("/calendars/{ProductId}/events/uid/{Uid}/{Page}", "GET")]
    [Route("/calendars/{ProductId}/events/uid/{Uid}/page/{Page}", "GET")]
    public class FindEvent: IReturn<VCALENDAR>
    {
        [DataMember]
        public string ProductId { get; set; }

        [DataMember]
        public string Uid { get; set; }
    }

    [DataContract]
    [Route("/calendars/{ProductId}/events", "GET")]
    [Route("/calendars/{ProductId}/events/{Page}", "GET")]
    [Route("/calendars/{ProductId}/events/page/{Page}", "GET")]
    [Route("/calendars/{ProductId}/events/page/{Page}", "GET")]
    public class FindEvents: IReturn<List<VCALENDAR>>
    {
        [DataMember]
        public string ProductId { get; set; }

        [DataMember]
        List<string> Uids { get; set; }

        [DataMember]
        public int? Page { get; set; }
    }

    [Route("/calendars/{ProductId}/event/{Uid}/delete", "DELETE")]
    [DataContract]
    public class DeleteEvent : IReturnVoid
    {
        [DataMember]
        public string ProductId { get; set; }

        [DataMember]
        public string Uid { get; set; }
    }


    [DataContract]
    [Route("/calendars/{ProductId}/events/delete", "DELETE")]
    public class DeleteEvents : IReturnVoid
    {
        [DataMember]
        public string ProductId { get; set; }

        [DataMember]
        public List<string> Uids { get; set; }
    }

    #endregion

}
