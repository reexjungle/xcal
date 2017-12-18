using System;
using System.Collections.Generic;

namespace reexjungle.xcal.core.domain.contracts.models.values
{
    public sealed class RECUR : IEquatable<RECUR>
    {
        public FREQ FREQ { get; }
        public DATE_TIME UNTIL { get; }
        public uint COUNT { get; }
        public uint INTERVAL { get; }
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

        public RECUR(RECUR other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            FREQ = other.FREQ;
            UNTIL = other.UNTIL;
            COUNT = other.COUNT;
            INTERVAL = other.INTERVAL;
            WKST = other.WKST;
            BYSECOND = other.BYSECOND != null ? new List<uint>(other.BYSECOND) : new List<uint>();
            BYMINUTE = other.BYMINUTE != null ? new List<uint>(other.BYMINUTE): new List<uint>();
            BYHOUR = other.BYHOUR != null ? new List<uint>(other.BYHOUR): new List<uint>();
            BYDAY = other.BYDAY != null ? new List<WEEKDAYNUM>(other.BYDAY): new List<WEEKDAYNUM>();
            BYMONTHDAY = other.BYMONTHDAY != null ? new List<int>(other.BYMONTHDAY): new List<int>();
            BYYEARDAY = other.BYYEARDAY != null ? new List<int>(other.BYYEARDAY): new List<int>();
            BYWEEKNO = other.BYWEEKNO != null ? new List<int>(other.BYWEEKNO): new List<int>();
            BYMONTH = other.BYMONTH != null ? new List<uint>(other.BYMONTH): new List<uint>();
            BYSETPOS = other.BYSETPOS != null ? new List<int>(other.BYSETPOS): new List<int>();
        }

        public bool Equals(RECUR other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return FREQ == other.FREQ 
                && UNTIL.Equals(other.UNTIL) 
                && COUNT == other.COUNT 
                && INTERVAL == other.INTERVAL 
                && Equals(BYSECOND, other.BYSECOND) && Equals(BYMINUTE, other.BYMINUTE) && Equals(BYHOUR, other.BYHOUR) && Equals(BYDAY, other.BYDAY) && Equals(BYMONTHDAY, other.BYMONTHDAY) && Equals(BYYEARDAY, other.BYYEARDAY) && Equals(BYWEEKNO, other.BYWEEKNO) && Equals(BYMONTH, other.BYMONTH) && WKST == other.WKST && Equals(BYSETPOS, other.BYSETPOS);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is RECUR && Equals((RECUR) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) FREQ;
                hashCode = (hashCode * 397) ^ UNTIL.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) COUNT;
                hashCode = (hashCode * 397) ^ (int) INTERVAL;
                hashCode = (hashCode * 397) ^ (BYSECOND != null ? BYSECOND.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BYMINUTE != null ? BYMINUTE.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BYHOUR != null ? BYHOUR.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BYDAY != null ? BYDAY.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BYMONTHDAY != null ? BYMONTHDAY.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BYYEARDAY != null ? BYYEARDAY.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BYWEEKNO != null ? BYWEEKNO.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BYMONTH != null ? BYMONTH.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) WKST;
                hashCode = (hashCode * 397) ^ (BYSETPOS != null ? BYSETPOS.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(RECUR left, RECUR right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RECUR left, RECUR right)
        {
            return !Equals(left, right);
        }
    }
}
