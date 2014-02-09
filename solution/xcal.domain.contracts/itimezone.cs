using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reexmonkey.xcal.domain.contracts
{
    public interface ITIMEZONE : ICOMPONENT
    {
        ITZIDPROP TimeZoneIdProperty { get; set; }
        ITZURL Url { get; set; }
        IDATE_TIME LastModified { get; set; }
        List<ISTANDARD> StandardTimes { get; set; }
        List<IDAYLIGHT> DaylightSaveTimes { get; set; }
    }

    public interface IObservance: ICOMPONENT
    { 
        IDATE_TIME StartDate { get; set; }
        ITZOFFSETFROM TimeZoneOffsetFrom { get; set; }
        ITZOFFSETTO TimeZoneOffsetTo { get; set; }
        IRECUR RecurrenceRule { get; set; }
        List<ITEXT> Comments { get; set; }
        List<IRDATE> RecurrenceDates { get; set; }
        List<ITZNAME> Names { get; set; }    
    }


    public interface ISTANDARD : IObservance { }

    public interface IDAYLIGHT : IObservance { }


}
