using reexjungle.xmisc.foundation.contracts;
using System;
using System.Collections.Generic;

namespace reexjungle.xcal.core.domain.contracts.models.values
{
    public class RECUR : IContainsKey<Guid>
    {
        public Guid Id { get; private set; }
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
            Id = Guid.NewGuid();
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

        public RECUR(Guid id, FREQ freq, DATE_TIME until) : this(freq, until)
        {
            Id = id;
        }

        public RECUR(FREQ freq, uint count, uint interval)
        {
            FREQ = freq;
            COUNT = count;
            INTERVAL = interval;
        }

        public RECUR(Guid id, FREQ freq, uint count, uint interval) : this(freq, count, interval)
        {
            Id = id;
        }

    }
}
