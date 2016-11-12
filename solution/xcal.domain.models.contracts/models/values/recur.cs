using System.Collections.Generic;

namespace reexjungle.xcal.core.domain.contracts.models.values
{
    public class RECUR
    {
        public FREQ FREQ { get; set; }
        public DATE_TIME UNTIL { get; set; }
        public uint COUNT { get; set; }
        public uint INTERVAL { get; set; }
        public List<uint> BYSECOND { get; }
        public List<uint> BYMINUTE { get; }
        public List<uint> BYHOUR { get; }
        public List<WEEKDAYNUM> BYDAY { get; }
        public List<int> BYMONTHDAY { get; }
        public List<int> BYYEARDAY { get; }
        public List<int> BYWEEKNO { get; }
        public List<uint> BYMONTH { get; }
        public WEEKDAY WKST { get; }
        public List<int> BYSETPOS { get; }
    }
}
