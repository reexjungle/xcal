using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ServiceStack.ServiceHost;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;

namespace reexmonkey.xcal.domain.operations
{
    #region search and retrieve calendars


    /// <summary>
    /// Represents a search operation to get a collection of VCALENDARS by the product ID of the calendar
    /// </summary>
    [DataContract]
    [Api("Gets a calendars identified by its key. Retrieved results can be paged")]
    [Route("/calendar/{ProductId}", "GET")]
    public class RetrieveCalendar : IReturn<VCALENDAR>
    {
        [DataMember]
        public string ProductId { get; set; }
    }


    /// <summary>
    /// Retrieves all available calendars from the system
    /// </summary>
    [Api("Retrieves all available calendars and constituent components from the system")]
    [DataContract]
    [Route("/calendars/page/{Page}", "GET")]
    public class RetrieveCalendars : IReturn<List<VCALENDAR>>
    {

        [DataMember]
        List<string> ProductIds{get; set;}

        /// <summary>
        /// Page number of paged calendars
        /// </summary>
        [DataMember]        
        [ApiMember(Name = "Page", Description = "Page number of paged calendars", ParameterType = "path", DataType = "int", IsRequired = true)
        public int? Page { get; set; }
    }


    #endregion

}
