using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using ServiceStack.ServiceHost;
using System.Collections.Generic;

namespace reexjungle.xcal.service.operations.concretes.live
{
    #region Method Constraints of VEVENT operations

    [Route("/calendars/{ProdId}/events/publish")]
    public class PublishEvents : IReturn<VCALENDAR>
    {
        public string ProdId { get; set; }

        public METHOD Method { get; set; }

        public List<VEVENT> Events { get; set; }

        public List<VTIMEZONE> TimeZones { get; set; }

        public List<IANA_COMPONENT> IANAComponents { get; set; }

        public List<X_COMPONENT> XComponents { get; set; }
    }

    #endregion Method Constraints of VEVENT operations
}