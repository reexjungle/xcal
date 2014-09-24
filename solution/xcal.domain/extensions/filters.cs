using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using reexmonkey.foundation.essentials.contracts;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;

namespace reexmonkey.xcal.domain.extensions
{
    public static class RecurrenceFilters
    {        
        public static IEnumerable<Tuple<DATE_TIME, DATE_TIME>> FilterBySetPosDaily(this IEnumerable<Tuple<DATE_TIME, DATE_TIME>> dates, RECUR rrule)
        {
            IEnumerable<Tuple<DATE_TIME, DATE_TIME>> filtered = null;
            if (rrule.BYSETPOS.NullOrEmpty()) return dates;
            if(rrule.FREQ == FREQ.DAILY)
            {
                var pvalues = rrule.BYSETPOS.Where(x => x > 0);
                if (!rrule.BYSECOND.NullOrEmpty())
                {
                    var npos = rrule.BYSETPOS.Where(x => x < 0);
                    var apos = !npos.NullOrEmpty()? npos.Select(x => 84600 - Math.Abs(x)): null;
                    var pos = (!apos.NullOrEmpty()) ? pvalues.Union(apos) : pvalues;
                    //normalize dates to zero hour of each date
                    var ndates = dates.Select(x => new Tuple<DATE_TIME, DATE_TIME>(new DATE_TIME(x.Item1.FULLYEAR, x.Item1.MONTH, x.Item1.MDAY, 0, 0, 0), x.Item2));
                    //adjust each normalized date to second position in day
                    var adjusted = ndates.SelectMany(x => pos.Select(p => new Tuple<DATE_TIME, DATE_TIME>( x.Item1 + new DURATION(0, 0, 0, 0, (uint)p), x.Item2)));
                    filtered = dates.Intersect(adjusted);
                }
                else
                {
                    if (!rrule.BYMINUTE.NullOrEmpty())
                    {
                        var npos = rrule.BYSETPOS.Where(x => x < 0);
                        var apos = !npos.NullOrEmpty() ? npos.Select(x => 1440 - Math.Abs(x)) : null;
                        var pos = (!apos.NullOrEmpty()) ? pvalues.Union(apos) : pvalues;
                        //normalize dates to zero hour of each date
                        var ndates = dates.Select(x => new Tuple<DATE_TIME, DATE_TIME>(new DATE_TIME(x.Item1.FULLYEAR, x.Item1.MONTH, x.Item1.MDAY, 0, 0, 0), x.Item2));                        
                        //adjust each normalized date to minute position in day
                        var adjusted = ndates.SelectMany(x => pos.Select(p => new Tuple<DATE_TIME, DATE_TIME>(x.Item1 + new DURATION(0, 0, 0, (uint)p), x.Item2))); 
                        filtered = dates.Intersect(adjusted);
                    }
                    else
                    {
                        if (!rrule.BYHOUR.NullOrEmpty())
                        {
                            var npos = rrule.BYSETPOS.Where(x => x < 0);
                            var apos = !npos.NullOrEmpty() ? npos.Select(x => 24 - Math.Abs(x)) : null;
                            var pos = (!apos.NullOrEmpty()) ? pvalues.Union(apos) : pvalues;
                            //normalize dates to zero hour of each date
                            var ndates = dates.Select(x => new Tuple<DATE_TIME, DATE_TIME>(new DATE_TIME(x.Item1.FULLYEAR, x.Item1.MONTH, x.Item1.MDAY, 0, 0, 0), x.Item2));
                            //adjust each normalized date to hour position in day
                            var adjusted = ndates.SelectMany(x => pos.Select(p => new Tuple<DATE_TIME, DATE_TIME>(x.Item1 + new DURATION(0, 0, 0, (uint)p), x.Item2)));
                            filtered = dates.Intersect(adjusted);
                        }
                    }
                }
            }

            return filtered ?? dates;
        }

        public static IEnumerable<Tuple<DATE_TIME, DATE_TIME>> FilterBySetPosWeekly(this IEnumerable<Tuple<DATE_TIME, DATE_TIME>> dates, RECUR rrule)
        {
            IEnumerable<Tuple<DATE_TIME, DATE_TIME>> filtered = null;
            if (rrule.BYSETPOS.NullOrEmpty()) return dates;

            if (rrule.FREQ == FREQ.WEEKLY)
            {
                if (!rrule.BYDAY.NullOrEmpty())
                {
                    var pvalues = rrule.BYSETPOS.Where(x => x > 0);
                    var npos = rrule.BYSETPOS.Where(x => x < 0);
                    var apos = !npos.NullOrEmpty() ? npos.Select(x => 7 - Math.Abs(x)) : null;
                    var pos = (!apos.NullOrEmpty()) ? pvalues.Union(apos) : pvalues;
                    var selected = pos.Select(x => (WEEKDAY)x);
                    filtered = dates.Where(x => selected.Contains(x.Item1.ToDateTime().DayOfWeek.ToWEEKDAY()));
                }
            }

            return filtered?? dates;
        }

        public static IEnumerable<Tuple<DATE_TIME, DATE_TIME>> FilterBySetPosMonthly(this IEnumerable<Tuple<DATE_TIME, DATE_TIME>> dates, RECUR rrule)
        {
            IEnumerable<Tuple<DATE_TIME, DATE_TIME>> filtered = null;
            if (rrule.BYSETPOS.NullOrEmpty()) return dates;

            if (rrule.FREQ == FREQ.MONTHLY)
            {
                var pvalues = rrule.BYSETPOS.Where(x => x > 0);
                var npos = rrule.BYSETPOS.Where(x => x < 0);
                var apos = (!npos.NullOrEmpty())
                    ? npos.Select(x => x > -365 ? 366 - Math.Abs(x) : 367 - Math.Abs(x)) 
                    : null;
                var pos = (!apos.NullOrEmpty()) ? pvalues.Union(apos) : pvalues;
                if (!rrule.BYDAY.NullOrEmpty())
                {
                    var nthdays = rrule.BYDAY.Where(x => x.OrdinalWeek != 0);
                    if (!nthdays.NullOrEmpty())
                        filtered = dates.Where(x => nthdays.Contains(new WEEKDAYNUM(x.Item1.ToDateTime().WeekOfMonth(),
                            x.Item1.ToDateTime().DayOfWeek.ToWEEKDAY())));
                    else
                    {
                        var days = rrule.BYDAY.Where(x => x.OrdinalWeek == 0);
                        filtered = dates.Where(x => days.Select(y => y.Weekday).Contains(x.Item1.ToDateTime().DayOfWeek.ToWEEKDAY()));
                    }
                }

                if (!rrule.BYMONTHDAY.NullOrEmpty()) filtered = dates.Where(x => pos.Contains(x.Item1.ToDateTime().Day));
            }
 
            return filtered ?? dates;
        }

        public static IEnumerable<Tuple<DATE_TIME, DATE_TIME>> FilterBySetPosYearly(this IEnumerable<Tuple<DATE_TIME, DATE_TIME>> dates, RECUR rrule)
        {
            IEnumerable<Tuple<DATE_TIME, DATE_TIME>> filtered = null;
            if (rrule.BYSETPOS.NullOrEmpty()) return dates;

            if (rrule.FREQ == FREQ.YEARLY)
            {
                var pvalues = rrule.BYSETPOS.Where(x => x > 0);
                var npos = rrule.BYSETPOS.Where(x => x < 0);

                if (!rrule.BYDAY.NullOrEmpty())
                {
                    var apos = (!npos.NullOrEmpty())
                        ? npos.Select(x => x > -52 ? 53 - Math.Abs(x) : 54 - Math.Abs(x))
                        : null;

                    var pos = (!apos.NullOrEmpty()) ? pvalues.Union(apos) : pvalues;
                    var nthdays = rrule.BYDAY.Where(x => x.OrdinalWeek != 0);
                    if (!nthdays.NullOrEmpty())
                        filtered = dates.Where(x => nthdays.Contains(new WEEKDAYNUM(x.Item1.ToDateTime().WeekOfYear(),
                            x.Item1.ToDateTime().DayOfWeek.ToWEEKDAY())));
                    else
                    {
                        var days = rrule.BYDAY.Where(x => x.OrdinalWeek == 0);
                        filtered = dates.Where(x => days.Select(y => y.Weekday).Contains(x.Item1.ToDateTime().DayOfWeek.ToWEEKDAY()));
                    }
                }
                 
                if (!rrule.BYWEEKNO.NullOrEmpty())
                {
                    var apos = (!npos.NullOrEmpty())
                        ? npos.Select(x => x > -52 ? 53 - Math.Abs(x) : 54 - Math.Abs(x))
                        : null;
                    var pos = (!apos.NullOrEmpty()) ? pvalues.Union(apos) : pvalues;
                    filtered = dates.Where(x => rrule.BYWEEKNO.Contains(x.Item1.ToDateTime().WeekOfYear()));
                }
                 
                if (!rrule.BYMONTH.NullOrEmpty())
                {
                    var apos = (!npos.NullOrEmpty()) ? npos.Select(x => 13 - Math.Abs(x)): null;
                    var pos = (!apos.NullOrEmpty()) ? pvalues.Union(apos) : pvalues;
                }
                if (!rrule.BYMONTHDAY.NullOrEmpty())
                {
                    var apos = (!npos.NullOrEmpty())
                        ? npos.Select(x =>
                            x > -28 ? 29 - Math.Abs(x) :
                            x > -29 ? 30 - Math.Abs(x) :
                            x > -30 ? 31 - Math.Abs(x) :
                            32 - Math.Abs(x))
                        : null;
                   filtered =  dates.Where(x => apos.Contains(x.Item1.ToDateTime().Day));
                }                
                if (!rrule.BYYEARDAY.NullOrEmpty())
                {
                    var apos = (!npos.NullOrEmpty())
                            ? npos.Select(x => x > -365 ? 366 - Math.Abs(x) : 367 - Math.Abs(x))
                            : null;

                    filtered = dates.Where(x => apos.Contains(x.Item1.ToDateTime().DayOfYear));
                }
            }

            return filtered ?? dates;
        }

        public static IEnumerable<Tuple<DATE_TIME, DATE_TIME>> FilterBySecond(this IEnumerable<Tuple<DATE_TIME, DATE_TIME>> dates, RECUR rrule)
        {
            return !rrule.BYSECOND.NullOrEmpty() ?dates.Where(x => rrule.BYSECOND.Contains(x.Item1.SECOND)) :dates;
        }

        public static IEnumerable<Tuple<DATE_TIME, DATE_TIME>> FilterByMinute(this IEnumerable<Tuple<DATE_TIME, DATE_TIME>> dates, RECUR rrule)
        {
            return !rrule.BYMINUTE.NullOrEmpty() ? dates.Where(x => rrule.BYMINUTE.Contains(x.Item1.MINUTE)): dates;
        }

        public static IEnumerable<Tuple<DATE_TIME, DATE_TIME>> FilterByHour(this IEnumerable<Tuple<DATE_TIME, DATE_TIME>> dates, RECUR rrule)
        {
            return !rrule.BYHOUR.NullOrEmpty() ?dates.Where(x => rrule.BYHOUR.Contains(x.Item1.HOUR)) : dates;
        }

        public static IEnumerable<Tuple<DATE_TIME, DATE_TIME>> FilterByMonth(this IEnumerable<Tuple<DATE_TIME, DATE_TIME>> dates, RECUR rrule)
        {
            return !rrule.BYMONTH.NullOrEmpty() ? dates.Where(x => rrule.BYMONTH.Contains(x.Item1.MONTH)): dates;
        }

        public static IEnumerable<Tuple<DATE_TIME, DATE_TIME>> FilterByYearDay(this IEnumerable<Tuple<DATE_TIME, DATE_TIME>> dates, RECUR rrule)
        {
            if (rrule.BYYEARDAY.NullOrEmpty()) return dates;
            var nvalues = rrule.BYYEARDAY.Where(x => x < 0).ToList();
            var pvalues = rrule.BYYEARDAY.Where(x => x > 0).ToList();
            var converted = (nvalues.NullOrEmpty())
                ? nvalues.Select(x => x > -366 ? 366 - Math.Abs(x) : 367 - Math.Abs(x))
                : null;
            var values = (!converted.NullOrEmpty()) ? pvalues.Union(converted) : pvalues;
            return dates.Where(x => values.Contains(x.Item1.ToDateTime().DayOfYear));
        }

        public static IEnumerable<Tuple<DATE_TIME, DATE_TIME>> FilterByMonthDay(this IEnumerable<Tuple<DATE_TIME, DATE_TIME>> dates, RECUR rrule)
        {
            if (rrule.BYMONTHDAY.NullOrEmpty()) return dates;
            var nvalues = rrule.BYMONTHDAY.Where(x => x < 0).ToList();
            var pvalues = rrule.BYMONTHDAY.Where(x => x > 0).ToList();
            var converted = (nvalues.NullOrEmpty())
                ? nvalues.Select(x =>
                    x > -28 ? 29 - Math.Abs(x) :
                    x > -29 ? 30 - Math.Abs(x) :
                    x > -30 ? 31 - Math.Abs(x) : 32 - Math.Abs(x))
                : null;
            var values = (!converted.NullOrEmpty()) ? pvalues.Union(converted) : pvalues;
            return dates.Where(x => values.Contains(x.Item1.ToDateTime().Day));
        }

        public static IEnumerable<Tuple<DATE_TIME, DATE_TIME>> FilterByDay(this IEnumerable<Tuple<DATE_TIME, DATE_TIME>> dates, RECUR rrule)
        {
            if (rrule.BYDAY.NullOrEmpty()) return dates;
            var weekdays = rrule.BYDAY.Select(x => x.Weekday).ToList();
            return dates.Where(x => weekdays.Contains(x.Item1.ToDateTime().DayOfWeek.ToWEEKDAY()));
        }

        public static IEnumerable<Tuple<DATE_TIME, DATE_TIME>> FilterByDayMonthly(this IEnumerable<Tuple<DATE_TIME, DATE_TIME>> dates, RECUR rrule)
        {
            if (rrule.BYDAY.NullOrEmpty()) return dates;
            IEnumerable<Tuple<DATE_TIME, DATE_TIME>> filtered = null;
            var N_bydays = rrule.BYDAY.Where(x => x.OrdinalWeek != 0);
            var bydays = rrule.BYDAY.Where(x => x.OrdinalWeek == 0);
            if (!N_bydays.NullOrEmpty())
                filtered = dates.Where(x => N_bydays.Contains(new WEEKDAYNUM(x.Item1.ToDateTime().WeekOfMonth(),
                        x.Item1.ToDateTime().DayOfWeek.ToWEEKDAY()))).ToList();
            return !filtered.NullOrEmpty()
                ? dates.Where(x => bydays.Select(y => y.Weekday).Contains(x.Item1.ToDateTime().DayOfWeek.ToWEEKDAY()))
                : dates;

        }

        public static IEnumerable<Tuple<DATE_TIME, DATE_TIME>> FilterByDayYearly(this IEnumerable<Tuple<DATE_TIME, DATE_TIME>> dates, RECUR rrule)
        {
            if (rrule.BYDAY.NullOrEmpty()) return dates;
            IEnumerable<Tuple<DATE_TIME, DATE_TIME>> filtered = null;
            var N_bydays = rrule.BYDAY.Where(x => x.OrdinalWeek != 0);
            var bydays = rrule.BYDAY.Where(x => x.OrdinalWeek == 0);
            if (!N_bydays.NullOrEmpty())
                filtered = dates.Where(x => N_bydays.Contains(new WEEKDAYNUM(x.Item1.ToDateTime().WeekOfYear(),
                        x.Item1.ToDateTime().DayOfWeek.ToWEEKDAY())));

            return !filtered.NullOrEmpty()
                ? dates.Where(x => bydays.Select(y => y.Weekday).Contains(x.Item1.ToDateTime().DayOfWeek.ToWEEKDAY()))
                : dates;
        }

        public static IEnumerable<Tuple<DATE_TIME, DATE_TIME>> FilterByWeekNo(this IEnumerable<Tuple<DATE_TIME, DATE_TIME>> dates, RECUR rrule)
        {
            return !rrule.BYWEEKNO.NullOrEmpty()
                ?dates.Where(x => rrule.BYWEEKNO.Contains(x.Item1.ToDateTime().WeekOfYear()))
                : dates;
        }

    }
}
