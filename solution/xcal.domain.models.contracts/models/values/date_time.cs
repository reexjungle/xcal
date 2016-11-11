using reexjungle.xcal.core.domain.contracts.models.parameters;
using System;

namespace reexjungle.xcal.core.domain.contracts.models.values
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDATE_TIME : IDATE, ITIME
    {
        DateTime AsDateTime();

        DateTimeOffset AsDateTimeOffset(Func<ITZID, IUTC_OFFSET> func = null);
    }
}
