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
    [Route("/calendars/create")]
    public class CreateCalendar: IReturn<VCALENDAR>
    {
        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public CALSCALE Scale { get; set; }

        [DataMember]
        public string ProductId { get; set; }

    }
}
