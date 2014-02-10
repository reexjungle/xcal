using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ServiceStack.ServiceHost;
using reexmonkey.crosscut.essentials.contracts;
using reexmonkey.crosscut.essentials.concretes;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;

namespace uniable.uview.domains.operations
{

    #region VEVENT methods based on RFC 5546

    /// <summary>
    /// Represents class to request for the publishing of events
    /// </summary>
    [DataContract]
    [Route("/calendars/{ProductId}/events/publish", "POST")]
    public class PublishEvents : IReturn<VCALENDAR>
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
        
        /// <summary>
        /// Constructor of request class.
        /// </summary>
        /// <param name="events">The list of events to publish</param>
        public PublishEvents(string prodid, List<VEVENT> events, List<VTIMEZONE> timezones = null)
        {
            this.ProductId = prodid;
            this.Events = events;
            this.TimeZones = timezones;
        }
    }

    /// <summary>
    /// Represents request class for the rescheduling of events
    /// </summary>
    [DataContract]
    [Route("/calendars/{ProductId}/event/reschedule", "POST")]
    public class RescheduleEvents : IReturn<VCALENDAR>
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

        /// <summary>
        /// Constructor of request class.
        /// </summary>
        /// <param name="events">The list of events to publish</param>
        public RescheduleEvents(string prodid, List<VEVENT> events, List<VTIMEZONE> timezones = null)
        {
            this.ProductId = prodid;
            this.Events = events;
            this.TimeZones = timezones;
        }

        public RescheduleEvents(string prodid, params VEVENT[] events)
        {
            this.ProductId = prodid;
            this.Events = new List<VEVENT>(events);
        }
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

        public List<VTIMEZONE> TimeZones { get; set; }

        /// <summary>
        /// Constructor of request class.
        /// </summary>
        /// <param name="events">The list of events to publish</param>
        public UpdateEvent(string prodid, List<VEVENT> events, List<VTIMEZONE> timezones = null)
        {
            this.ProductId = prodid;
            this.Events = events;
            this.TimeZones = timezones;
        }

        public UpdateEvent( string prodid, params VEVENT[] events)
        {
            this.ProductId = prodid;
            this.Events = events.ToList();
        }
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

        public DelegateEvent(string prodid, List<VEVENT> events)
        {
            events.ThrowIfNullOrEmpty("Events MUST be available to reschedule!");
            this.ProductId = prodid;
            this.Events = events;
        }

        public DelegateEvent(string prodid, params VEVENT[] events)
        {
            events.ThrowIfNullOrEmpty("Events MUST be available to reschedule!");
            this.ProductId = prodid;
            this.Events = events.ToList();
        }
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
        
        public ChangeOrganizer(string prodid, List<VEVENT> events)
        {
            this.ProductId = prodid;
            this.Events = events;
        }

        public ChangeOrganizer(string prodid, params VEVENT[] events)
        {
            this.ProductId = prodid;
            this.Events = events.ToList();
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

        public SendOnBehalf(string prodid, List<VEVENT> events)
        {
            this.ProductId = prodid;
        }

        public SendOnBehalf(string prodid, params VEVENT[] events)
        {
            this.ProductId = prodid;
            this.Events = events.ToList();
        }
    }

    [DataContract]
    [Route("/calendars/{ProductId}/event/forward", "POST")]
    public class ForwardEvent: IReturn<VCALENDAR>
    {
        [DataMember]
        public List<VEVENT> Events { get; set; }

        [DataMember]
        public Dictionary<UID, List<ATTENDEE>> Guests { get; set; }

        /// <summary>
        /// Gets or sets the Product Id of the calendar
        /// </summary>
        [DataMember]
        public string ProductId { get; set; }
    
        public ForwardEvent(string prodid, Dictionary<UID, List<ATTENDEE>> guests, List<VEVENT> events)
        {
            this.ProductId = prodid;
            this.Events = events;
            this.Guests = guests;
        }

        public ForwardEvent(string prodid, Dictionary<UID, List<ATTENDEE>> guests, params VEVENT[] events)
        {
            this.ProductId = prodid;
            this.Events = events.ToList();
            this.Guests = guests;
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

        public UpdateAttendeesStatus(string prodid, Dictionary<UID, List<ATTENDEE>> updates, List<VEVENT> events)
        {
            this.ProductId = prodid;
            this.Updates = updates;
            this.Events = events;
        }

        public UpdateAttendeesStatus(string prodid, Dictionary<UID, List<ATTENDEE>> updates, params VEVENT[] events)
        {
            this.ProductId = prodid;
            this.Updates = updates;
            this.Events = events.ToList();
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

        public ReplyToEvent(string prodid, List<VEVENT> events)
        {
            this.ProductId = prodid;
            this.Events = events;
        }

        public ReplyToEvent(string prodid, params VEVENT[] events)
        {
            this.ProductId = prodid;
            this.Events = events.ToList();
        }
    }


    [DataContract]
    [Route("/calendars/{ProductId}/event/add", "PUT")]
    [KnownType(typeof(ATTACH_BINARY))]
    [KnownType(typeof(ATTACH_URI))]
    [KnownType(typeof(ATTENDEE))]
    [KnownType(typeof(CATEGORIES))]
    [KnownType(typeof(COMMENT))]
    [KnownType(typeof(CONTACT))]
    [KnownType(typeof(EXDATE))]
    [KnownType(typeof(REQUEST_STATUS))]
    [KnownType(typeof(RESOURCES))]
    [KnownType(typeof(RELATEDTO))]
    [KnownType(typeof(RDATE))]
    [KnownType(typeof(AUDIO_ALARM))]
    [KnownType(typeof(DISPLAY_ALARM))]
    [KnownType(typeof(EMAIL_ALARM))]
    public class AddToEvent: IReturn<VCALENDAR>
    {

        [DataMember]
        public string ProductId { get; set; }

        public UID Uid { get; set; }

        [DataMember]
        public IDATE_TIME StartDate { get; set; }

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
        public IPRIORITY Priority { get; set; }

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
        public List<IATTACH> Attachments { get; set; }

        [DataMember]
        public List<IATTENDEE> Attendees { get; set; }

        [DataMember]
        public ICATEGORIES Categories { get; set; }

        [DataMember]
        public List<ITEXT> Comments { get; set; }

        [DataMember]
        public List<ICONTACT> Contacts { get; set; }

        [DataMember]
        public List<IEXDATE> ExceptionDates { get; set; }

        [DataMember]
        public List<IREQUEST_STATUS> RequestStatuses { get; set; }

        [DataMember]
        public List<IRESOURCES> Resources { get; set; }

        [DataMember]
        public List<IRELATEDTO> RelatedTos { get; set; }

        [DataMember]
        public List<IRDATE> RecurrenceDates { get; set; }

        [DataMember]
        public List<IALARM> Alarms { get; set; }

        [DataMember]
        public List<VTIMEZONE> TimeZones { get; set; }

        public AddToEvent(UID uid, List<VTIMEZONE> timezones = null)
        {
            this.Uid = uid;
            this.TimeZones = timezones;
        }
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

        public CancelEvent(string prodid, List<VEVENT> events, List<VTIMEZONE> timezones = null)
        {
            this.ProductId = prodid;
            this.Events = events;
            this.TimeZones = timezones;
        }

        public CancelEvent(string prodid, params VEVENT[] events)
        {
            this.ProductId = prodid;
            this.Events = events.ToList();
        }

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

 
    #region retrieve events

    [DataContract]
    [Route("/calendars/{CalendarKey}/events/keys", "GET")]
    [Route("/calendars/{CalendarKey}/events/keys/{Page}", "GET")]
    [Route("/calendars/{CalendarKey}/events/keys/page/{Page}", "GET")]
    public class RetrieveEventKeys: IReturn<List<string>>
    {
        [DataMember]
        public string CalendarKey { get; set; }

        [DataMember]
        public int? Page { get; set; }
    }

    [DataContract]
    [Route("/calendars/events/keys", "GET")]
    [Route("/calendars/events/keys/{Page}", "GET")]
    [Route("/calendars/events/keys/page/{Page}", "GET")]
    public class RetrieveEventKeysFromCalendars : IReturn<List<string>>
    {
        [DataMember]
        public List<string> CalendarKeys { get; set; }

        [DataMember]
        public int? Page { get; set; }
    }


    [DataContract]
    [Route("/calendars/{ProductId}/events/sparse/{Page}", "GET")]
    [Route("/calendars/{ProductId}/events/sparse/page/{Page}", "GET")]
    [Route("/calendars/{ProductId}/events/sparse", "GET")]
    [Route("/calendars/{ProductId}/events/sparse/page/{Page}", "GET")]
    public class RetrieveSparseEvents : IReturn<List<VEVENT>>
    {
        [DataMember]
        public string ProductId { get; set; }

        [DataMember]
        public int? Page { get; set; }
    }

    [DataContract]
    [Route("/calendars/events/sparse/{Page}", "GET")]
    [Route("/calendars/events/sparse/page/{Page}", "GET")]
    [Route("/calendars/events/sparse", "GET")]
    [Route("/calendars/events/sparse/page/{Page}", "GET")]
    public class RetrieveSparseEventsFromCalendars : IReturn<List<VEVENT>>
    {
        [DataMember]
        public List<string> ProductIds { get; set; }

        [DataMember]
        public int? Page { get; set; }
    }


    [DataContract]
    [Route("/calendars/{ProductId}/events/location/{Uid}/{Page}", "GET")]
    [Route("/calendars/{ProductId}/events/location/{Uid}/page/{Page}", "GET")]
    public class RetrieveEvent : IReturn<VEVENT>
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
    public class RetrieveEvents: IReturn<List<VEVENT>> 
    {
        [DataMember]
        public string ProductId { get; set; }

        [DataMember]
        public int? Page { get; set; }
    }


    [DataContract]
    [Route("/calendars/events", "GET")]
    [Route("/calendars/events/{Page}", "GET")]
    [Route("/calendars/events/page/{Page}", "GET")]
    public class RetrieveEventsFromCalendars : IReturn<List<VEVENT>>
    {
        [DataMember]
        public List<string> ProductIds { get; set; }

        [DataMember]
        public int? Page { get; set; }
    }

#endregion

    #region CRUD single event

    [DataContract]
    [Route("/calendars/{ProductId}/event/create", "POST")]
    public class CreateVEvent : IReturn<VEVENT>
    {
        [DataMember]
        public string ProductId {get; set;}

        [DataMember]
        public VEVENT Event { get; set; }
    }

    [Route("/calendars/{ProductId}/event/patch", "PATCH")]   
    [DataContract]
    public class PatchEvent : IReturn<VEVENT>
    {
        [DataMember]
        public string ProductId{get; set;}

        [DataMember]
        public VEVENT Patch {get; set;}
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

    #endregion

    #region CRUD mulltiple events

    [DataContract]
    [Route("/calendars/{ProductId}/events/create", "POST")]
    public class CreateEvents : IReturn<VEVENT>
    {
                
        [DataMember]
        public string ProductId { get; set; }

        [DataMember]
        public List<VEVENT> Events { get; set; }

        public CreateEvents()
        {
            this.Events = new List<VEVENT>();
        }

        public CreateEvents(IEnumerable<VEVENT> events)
        {
            this.Events.AddRange(events);
        }
    }


    [DataContract]
    [Route("/calendars/{ProductId}/events/patch", "PATCH")]
    public class PatchEvents : IReturn<List<VEVENT>>
    {
        [DataMember]
        public string ProductId { get; set; }

        [DataMember]
        public IEnumerable<VEVENT> Patches { get; set; }
    
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



    [DataContract]
    [Route("/calendars/events/delete", "DELETE")]
    public class DeleteEventsFromCalendars : IReturnVoid
    {
        [DataMember]
        public List<string> ProductIds { get; set; }

        [DataMember]
        public List<string> Uids { get; set; }

    }

    #endregion

    #region event details

    #region Event Details - Organizers

    [DataContract]
    [Route("/calendars/events/organizers", "GET")]
    [Route("/calendars/events/organizers/{Page}", "GET")]
    [Route("/calendars/events/organizers/page/{Page}", "GET")]
    public class RetrieveOrganizersFromEvents : IReturn<List<ORGANIZER>>
    {
        [DataMember]
        public List<string> Uids { get; set; }

        [DataMember]
        public int? Page { get; set; }
    }

    [DataContract]
    [Route("/calendars/events/attendees", "GET")]
    [Route("/calendars/events/attendees/{Page}", "GET")]
    [Route("/calendars/events/attendees/page/{Page}", "GET")]
    public class RetrieveAttendeesFromEvents : IReturn<List<ATTENDEE>>
    {
        [DataMember]
        public List<string> Uids { get; set; }

        [DataMember]
        public int? Page { get; set; }
    }

    [DataContract]
    [Route("/calendars/events/attendees", "GET")]
    [Route("/calendars/events/attendees/{Page}", "GET")]
    [Route("/calendars/events/attendees/page/{Page}", "GET")]
    public class RetrieveContactsFromEvents : IReturn<List<CONTACT>>
    {
        [DataMember]
        public List<string> Uids { get; set; }

        [DataMember]
        public int? Page { get; set; }
    }


    [DataContract]
    [Route("/calendars/events/comments", "GET")]
    [Route("/calendars/events/comments/{Page}", "GET")]
    [Route("/calendars/events/comments/page/{Page}", "GET")]
    public class RetrieveCommentsFromEvents : IReturn<List<COMMENT>>
    {
        [DataMember]
        public List<string> Uids { get; set; }

        [DataMember]
        public int? Page { get; set; }
    }

    [DataContract]
    [Route("/calendars/events/resources", "GET")]
    [Route("/calendars/events/resources/{Page}", "GET")]
    [Route("/calendars/events/resources/page/{Page}", "GET")]
    public class RetrieveResourcesFromEvents : IReturn<List<RESOURCES>>
    {
        [DataMember]
        public List<string> Uids { get; set; }

        [DataMember]
        public int? Page { get; set; }
    }

    #endregion

   

#endregion



}
