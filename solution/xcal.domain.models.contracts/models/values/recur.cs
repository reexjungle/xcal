using System;
using System.Collections.Generic;

namespace reexjungle.xcal.core.domain.contracts.models.values
{
    public sealed class RECUR
    {
        public FREQ FREQ { get; private set; }
        public DATE_TIME UNTIL { get; private set; }
        public uint COUNT { get; private set; }
        public uint INTERVAL { get; private set; }
        public List<uint> BYSECOND { get; private set; }
        public List<uint> BYMINUTE { get; private set; }
        public List<uint> BYHOUR { get; private set; }
        public List<WEEKDAYNUM> BYDAY { get; private set; }
        public List<int> BYMONTHDAY { get; private set; }
        public List<int> BYYEARDAY { get; private set; }
        public List<int> BYWEEKNO { get; private set; }
        public List<uint> BYMONTH { get; private set; }
        public WEEKDAY WKST { get; private set; }
        public List<int> BYSETPOS { get; private set; }

        public RECUR()
        {
            FREQ = FREQ.DAILY;
            UNTIL = default(DATE_TIME);
            COUNT = 0u;
            INTERVAL = 1u;
            WKST = WEEKDAY.SU;

            BYMONTH = new List<uint>();
            BYWEEKNO = new List<int>();
            BYYEARDAY = new List<int>();
            BYMONTHDAY = new List<int>();
            BYDAY = new List<WEEKDAYNUM>();
            BYHOUR = new List<uint>();
            BYMINUTE = new List<uint>();
            BYSECOND = new List<uint>();
            BYSETPOS = new List<int>();
        }

        public RECUR(FREQ freq, DATE_TIME until) : this()
        {
            FREQ = freq;
            UNTIL = until;
        }

        public RECUR(FREQ freq, uint count, uint interval)
        {
            FREQ = freq;
            COUNT = count;
            INTERVAL = interval;
        }
    }
}
