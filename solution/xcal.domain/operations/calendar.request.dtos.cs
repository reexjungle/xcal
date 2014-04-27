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
    [Route("/calendars/create", "POST")]
    public class CreateCalendar: IReturn<VCALENDAR>
    {
        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public CALSCALE Scale { get; set; }

        [DataMember]
        public string ProductId { get; set; }

        [DataMember]
        public XCOMPONENTS Tag { get; set; }

        [DataMember]
        public List<VTIMEZONE> TimeZones { get; set; }

        [DataMember]
        public List<VEVENT> Events { get; set; }

    }

    [DataContract]
    [Route("/calendars/add", "POST")]
    public class AddCalendar: IReturnVoid
    {
        [DataMember]
        public VCALENDAR Calendar { get; set; }
    }

    [DataContract]
    [Route("/calendars/batch/add", "POST")]
    public class AddCalendars: IReturnVoid
    {
        [DataMember]
        public List<VCALENDAR> Calendars { get; set; }
    }


    [DataContract]
    [Route("/calendars/update", "POST")]
    public class UpdateCalendar: IReturn<VCALENDAR>
    {
        [DataMember]
        public VCALENDAR Calendar { get; set; }
    }


    [DataContract]
    [Route("/calendars/update", "POST")]
    public class UpdateCalendars : IReturn<List<VCALENDAR>>
    {
        [DataMember]
        public List<VCALENDAR> Calendars { get; set; }
    }

    [DataContract]
    [Route("/calendars/{CalendarId}/patch", "POST")]
    public class PatchCalendar: IReturn<VCALENDAR>
    {
        [DataMember]
        public string CalendarId { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public CALSCALE Scale { get; set; }

        [DataMember]
        public string ProductId { get; set; }

        public XCOMPONENTS Tag { get; set; }

        public List<VTIMEZONE> TimeZones { get; set; }
    }

    [DataContract]
    [Route("/calendars/batch/patch", "POST")]
    public class PatchCalendars : IReturn<List<VCALENDAR>>
    {
        [DataMember]
        public List<string> CalendarIds { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public CALSCALE Scale { get; set; }

        [DataMember]
        public string ProductId { get; set; }

        public XCOMPONENTS Tag { get; set; }

        public List<VTIMEZONE> TimeZones { get; set; }
    }


}
