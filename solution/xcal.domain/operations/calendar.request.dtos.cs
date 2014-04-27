using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ServiceStack.ServiceHost;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;

namespace reexmonkey.xcal.domain.operations
{
    [DataContract]
    [Route("/calendars/add", "POST")]
    public class AddCalendar: IReturn<VCALENDAR>
    {
        [DataMember]
        public VCALENDAR Calendar { get; set; }
    }

    [DataContract]
    [Route("/calendars/batch/add", "POST")]
    public class AddCalendars : IReturn<List<VCALENDAR>>
    {
        [DataMember]
        public List<VCALENDAR> Calendars { get; set; }
    }

    [DataContract]
    [Route("/calendars/update", "PUT")]
    public class UpdateCalendar: IReturn<VCALENDAR>
    {
        [DataMember]
        public VCALENDAR Calendar { get; set; }
    }

    [DataContract]
    [Route("/calendars/update", "PUT")]
    public class UpdateCalendars : IReturn<List<VCALENDAR>>
    {
        [DataMember]
        public List<VCALENDAR> Calendars { get; set; }
    }

    [DataContract]
    [Route("/calendars/{CalendarId}/patch", "PATCH")]
    public class PatchCalendar: IReturn<VCALENDAR>
    {
        [DataMember]
        public string CalendarId { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public CALSCALE Scale { get; set; }

        [DataMember]
        public METHOD Method { get; set; }

        [DataMember]
        public string ProductId { get; set; }

        [DataMember]
        public List<VEVENT> Events { get; set; }

        [DataMember]
        public List<VTODO> Todos { get; set; }

        [DataMember]
        public List<VFREEBUSY> FreeBusies { get; set; }

        [DataMember]
        public List<VJOURNAL> Journals { get; set; }

        [DataMember]
        public List<VTIMEZONE> TimeZones { get; set; }

        [DataMember]
        public List<IANA_COMPONENT> IanaComponents { get; set; }

        [DataMember]
        public List<XCOMPONENT> XComponents { get; set; }
    }

    [DataContract]
    [Route("/calendars/batch/patch", "PATCH")]
    public class PatchCalendars : IReturn<List<VCALENDAR>>
    {
        [DataMember]
        public List<string> CalendarIds { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public CALSCALE Scale { get; set; }

        [DataMember]
        public METHOD Method { get; set; }

        [DataMember]
        public string ProductId { get; set; }

        [DataMember]
        public List<VEVENT> Events { get; set; }

        [DataMember]
        public List<VTODO> Todos { get; set; }

        [DataMember]
        public List<VFREEBUSY> FreeBusies { get; set; }

        [DataMember]
        public List<VJOURNAL> Journals { get; set; }

        [DataMember]
        public List<VTIMEZONE> TimeZones { get; set; }

        [DataMember]
        public List<IANA_COMPONENT> IanaComponents { get; set; }

        [DataMember]
        public List<XCOMPONENT> XComponents { get; set; }

    }

    [DataContract]
    [Route("/calendars/{CalendarId}/delete", "DELETE")]
    public class DeleteCalendar: IReturnVoid
    {
        [DataMember]
        public string CalendarId { get; set; }
    }

    [DataContract]
    [Route("/calendars/batch/delete", "DELETE")]
    public class DeleteCalendars : IReturnVoid
    {
        [DataMember]
        public List<string> CalendarIds { get; set; }
    }

    [DataContract]
    [Route("/calendars/{CalendarId}/get", "GET")]
    public class GetCalendar : IReturn<VCALENDAR>
    {
        [DataMember]
        public string CalendarId { get; set; }
    }

    [DataContract]
    [Route("/calendars/batch/get", "GET")]
    [Route("/calendars/batch/get/{Page}/{Take}", "GET")]
    [Route("/calendars/batch/get/page/{Page}/{Take}", "GET")]
    [Route("/calendars/batch/get/page/{Page}/take/{Take}", "GET")]
    public class GetCalendars : IReturn<List<VCALENDAR>>
    {
        [DataMember]
        public List<string> CalendarIds { get; set; }

        [DataMember]
        public int? Page { get; set; }

        [DataMember]
        public int? Take { get; set; }

    }

}


