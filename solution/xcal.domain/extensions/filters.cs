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
            if (rrule.BYSETPOS.NullOrEmpty()) return dates;
            IEnumerable<Tuple<DATE_TIME, DATE_TIME>> filtered = dates;
            if(rrule.FREQ == FREQ.DAILY)
            {
                var pvalues = rrule.BYSETPOS.Where(x => x > 0);
                if (!rrule.BYSECOND.NullOrEmpty())
                {
                    var npos = rrule.BYSETPOS.Where(x => x < 0);
                    var apos = !npos.NullOrEmpty()? npos.Select(x => 84600 - Math.Abs(x)): null;
                    var pos = (!apos.NullOrEmpty()) ? pvalues.Union(apos) : pvalues;
                    //normalize dates to zero hour of each date
                    var ndates = filtered.Select(x => new Tuple<DATE_TIME, DATE_TIME>(new DATE_TIME(x.Item1.FULLYEAR, x.Item1.MONTH, x.Item1.MDAY, 0, 0, 0), x.Item2));
                    //adjust each normalized date to second position in day
                    var adjusted = ndates.SelectMany(x => pos.Select(p => new Tuple<DATE_TIME, DATE_TIME>( x.Item1 + new DURATION(0, 0, 0, 0, p), x.Item2)));
                    filtered = filtered.Intersect(adjusted);
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
                        var adjusted = ndates.SelectMany(x => pos.Select(p => new Tuple<DATE_TIME, DATE_TIME>(x.Item1 + new DURATION(0, 0, 0, p), x.Item2))); 
                        filtered = filtered.Intersect(adjusted);
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
                            var adjusted = ndates.SelectMany(x => pos.Select(p => new Tuple<DATE_TIME, DATE_TIME>(x.Item1 + new DURATION (0, 0, p), x.Item2)));
                            filtered = filtered.Intersect(adjusted);
                        }
                    }
                }
            }

            return filtered;
        }

        public static IEnumerable<Tuple<DATE_TIME, DATE_TIME>> FilterBySetPosWeekly(this IEnumerable<Tuple<DATE_TIME, DATE_TIME>> dates, RECUR rrule)
        {
            if (rrule.BYSETPOS.NullOrEmpty()) return dates;
            IEnumerable<Tuple<DATE_TIME, DATE_TIME>> filtered = dates;
            if (rrule.FREQ == FREQ.WEEKLY)
            {
                if (!rrule.BYDAY.NullOrEmpty())
                {
                    var pvalues = rrule.BYSETPOS.Where(x => x > 0);
                    var nvalues = rrule.BYSETPOS.Where(x => x < 0);
                    var norm = !nvalues.NullOrEmpty() ? nvalues.Select(x => 7 - Math.Abs(x)) : null;
                    var values = (!norm.NullOrEmpty()) ? pvalues.Union(norm) : pvalues;
                    var selected = values.Select(x => (WEEKDAY)x);
                    filtered = filtered.Where(x => selected.Contains(x.Item1.ToDateTime().DayOfWeek.ToWEEKDAY()));
                }
            }

            return filtered;
        }

        public static IEnumerable<Tuple<DATE_TIME, DATE_TIME>> FilterBySetPosMonthly(this IEnumerable<Tuple<DATE_TIME, DATE_TIME>> dates, RECUR rrule)
        {
            if (rrule.BYSETPOS.NullOrEmpty()) return dates;
            IEnumerable<Tuple<DATE_TIME, DATE_TIME>> filtered = dates;

            if (rrule.FREQ == FREQ.MONTHLY)
            {
                var pvalues = rrule.BYSETPOS.Where(x => x > 0);
                var nvalues = rrule.BYSETPOS.Where(x => x < 0);
                var ndates = new List<Tuple<DATE_TIME, DATE_TIME>>();
                var pdates = new List<Tuple<DATE_TIME, DATE_TIME>>();
                var bymonths = dates.GroupBy(x => x.Item1.MONTH); //group by months

                if (rrule.BYMONTHDAY.NullOrEmpty())
                {
                    foreach (var bymonth in bymonths)
                    {
                        if (!nvalues.NullOrEmpty())
                        {
                            var days = DateTime.DaysInMonth((int)bymonth.First().Item1.FULLYEAR, (int)bymonth.Key);
                            var normalized = nvalues.Select(x => (days + 1) - Math.Abs(x));
                            ndates.AddRange(bymonth.Where(x => normalized.Contains((int)x.Item1.MDAY)));
                        }

                        if (!pvalues.NullOrEmpty())
                            pdates.AddRange(bymonth.Where(x => pvalues.Contains((int)x.Item1.MDAY)));
                    } 
                }

                filtered = filtered.Intersect(pdates.Union(ndates));

                if (!rrule.BYDAY.NullOrEmpty())
                {
                    var nthdays = rrule.BYDAY.Where(x => x.OrdinalWeek != 0);
                    if (!nthdays.NullOrEmpty())
                        filtered = filtered.Where(x => nthdays.Contains(new WEEKDAYNUM(x.Item1.ToDateTime().WeekOfMonth(),
                            x.Item1.ToDateTime().DayOfWeek.ToWEEKDAY())));
                    else
                    {
                        var days = rrule.BYDAY.Where(x => x.OrdinalWeek == 0);
                        filtered = filtered.Where(x => days.Select(y => y.Weekday).Contains(x.Item1.ToDateTime().DayOfWeek.ToWEEKDAY()));
                    }
                }

            }
 
            return filtered;
        }

        public static IEnumerable<Tuple<DATE_TIME, DATE_TIME>> FilterBySetPosYearly(this IEnumerable<Tuple<DATE_TIME, DATE_TIME>> dates, RECUR rrule)
        {
            if (rrule.BYSETPOS.NullOrEmpty()) return dates;
            var  filtered = dates;
            if (rrule.FREQ == FREQ.YEARLY)
            {
                var pvalues = rrule.BYSETPOS.Where(x => x > 0);
                var nvalues = rrule.BYSETPOS.Where(x => x < 0);

                if (!rrule.BYDAY.NullOrEmpty())
                {

                    var ordweeks = rrule.BYDAY.Where(x => x.OrdinalWeek != 0);
                    if (!ordweeks.NullOrEmpty())
                        filtered = filtered.Where(x => ordweeks.Contains(new WEEKDAYNUM(x.Item1.ToDateTime().WeekOfYear(),
                            x.Item1.ToDateTime().DayOfWeek.ToWEEKDAY())));
                    else
                    {
                        var days = rrule.BYDAY.Where(x => x.OrdinalWeek == 0);
                        filtered = filtered.Where(x => days.Select(y => y.Weekday).Contains(x.Item1.ToDateTime().DayOfWeek.ToWEEKDAY()));
                    }
                }
                 
                if (!rrule.BYWEEKNO.NullOrEmpty())
                {
                    var ndates = new List<Tuple<DATE_TIME, DATE_TIME>>();
                    var pdates = new List<Tuple<DATE_TIME, DATE_TIME>>();
                    var byyears = filtered.GroupBy(x => x.Item1.FULLYEAR); //group by years

                    foreach (var byyear in byyears)
                    {
                        if (!nvalues.NullOrEmpty())
                        {
                            var weeks = new DateTime((int)byyear.First().Item1.FULLYEAR, 12, 31).WeekOfYear();
                            var normalized = nvalues.Select(x => (weeks + 1) - Math.Abs(x));
                            ndates.AddRange(byyear.Where(x => normalized.Contains(x.Item1.ToDateTime().WeekOfYear())));
                        }

                        if (!pvalues.NullOrEmpty())
                            pdates.AddRange(byyear.Where(x => pvalues.Contains(x.Item1.ToDateTime().WeekOfYear())));
                    }

                    filtered = filtered.Intersect(pdates.Union(ndates));
                }
                 
                if (!rrule.BYMONTH.NullOrEmpty())
                {
                    var ndates = new List<Tuple<DATE_TIME, DATE_TIME>>();
                    var pdates = new List<Tuple<DATE_TIME, DATE_TIME>>();
                    var byyears = filtered.GroupBy(x => x.Item1.FULLYEAR); //group by years

                    foreach (var byyear in byyears)
                    {
                        if (!nvalues.NullOrEmpty())
                        {
                            var normalized = nvalues.Select(x => 13 - Math.Abs(x));
                            ndates.AddRange(byyear.Where(x => normalized.Contains(x.Item1.ToDateTime().WeekOfYear())));
                        }

                        if (!pvalues.NullOrEmpty())
                            pdates.AddRange(byyear.Where(x => pvalues.Contains(x.Item1.ToDateTime().WeekOfYear())));
                    }

                    filtered = filtered.Intersect(pdates.Union(ndates));
                }
                if (!rrule.BYMONTHDAY.NullOrEmpty())
                {
                    var ndates = new List<Tuple<DATE_TIME, DATE_TIME>>();
                    var pdates = new List<Tuple<DATE_TIME, DATE_TIME>>();
                    var bymonths = dates.GroupBy(x => x.Item1.MONTH); //group by months

                    foreach (var bymonth in bymonths)
                    {
                        if (!nvalues.NullOrEmpty())
                        {
                            var days = DateTime.DaysInMonth((int)bymonth.First().Item1.FULLYEAR, (int)bymonth.Key);
                            var normalized = nvalues.Select(x => (days + 1) - Math.Abs(x));
                            ndates.AddRange(bymonth.Where(x => normalized.Contains((int)x.Item1.MDAY)));
                        }

                        if (!pvalues.NullOrEmpty())
                            pdates.AddRange(bymonth.Where(x => pvalues.Contains((int)x.Item1.MDAY)));
                    }

                    filtered = filtered.Intersect(pdates.Union(ndates));
                }                
                if (!rrule.BYYEARDAY.NullOrEmpty())
                {
                    var ndates = new List<Tuple<DATE_TIME, DATE_TIME>>();
                    var pdates = new List<Tuple<DATE_TIME, DATE_TIME>>();
                    var byyears = filtered.GroupBy(x => x.Item1.FULLYEAR); //group by years

                    foreach (var byyear in byyears)
                    {
                        if (!nvalues.NullOrEmpty())
                        {
                            var weeks = (int)byyear.First().Item1.FULLYEAR.CountYearDays();
                            var normalized = nvalues.Select(x => (weeks + 1) - Math.Abs(x));
                            ndates.AddRange(byyear.Where(x => normalized.Contains(x.Item1.ToDateTime().WeekOfYear())));
                        }

                        if (!pvalues.NullOrEmpty())
                            pdates.AddRange(byyear.Where(x => pvalues.Contains(x.Item1.ToDateTime().WeekOfYear())));
                    }

                    filtered = filtered.Intersect(pdates.Union(ndates));
                }
            }

            return filtered;
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
            var ndates = new List<Tuple<DATE_TIME, DATE_TIME>>();
            var pdates = new List<Tuple<DATE_TIME, DATE_TIME>>();
            var byyears = dates.GroupBy(x => x.Item1.FULLYEAR); //group by years
            foreach (var byyear in byyears)
            {
                if (!nvalues.NullOrEmpty())
                {
                    var days = byyear.First().Item1.FULLYEAR.CountYearDays();
                    var normalized = nvalues.Select(x => (days + 1) - Math.Abs(x));
                    ndates.AddRange(byyear.Where(x => normalized.Contains(x.Item1.ToDateTime().DayOfYear)));
                }

                if (!pvalues.NullOrEmpty())
                    pdates.AddRange(byyear.Where(x => pvalues.Contains(x.Item1.ToDateTime().DayOfYear)));
            }

            return pdates.Union(ndates);
        }

        public static IEnumerable<Tuple<DATE_TIME, DATE_TIME>> FilterByMonthDay(this IEnumerable<Tuple<DATE_TIME, DATE_TIME>> dates, RECUR rrule)
        {
            if (rrule.BYMONTHDAY.NullOrEmpty()) return dates;

            var nvalues = rrule.BYMONTHDAY.Where(x => x < 0).ToList();
            var pvalues = rrule.BYMONTHDAY.Where(x => x > 0).ToList();
            var ndates = new List<Tuple<DATE_TIME, DATE_TIME>>();
            var pdates = new List<Tuple<DATE_TIME, DATE_TIME>>();
            var bymonths = dates.GroupBy(x => x.Item1.MONTH); //group by months
            
            foreach (var bymonth in bymonths)
            {
                if(!nvalues.NullOrEmpty())
                {
                    var days = DateTime.DaysInMonth((int)bymonth.First().Item1.FULLYEAR, (int)bymonth.Key);
                    var normalized = nvalues.Select(x => (days + 1) - Math.Abs(x));
                    ndates.AddRange(bymonth.Where(x => normalized.Contains((int)x.Item1.MDAY)));
                }

                if(!pvalues.NullOrEmpty()) 
                    pdates.AddRange(bymonth.Where(x => pvalues.Contains((int)x.Item1.MDAY)));
            }

            return pdates.Union(ndates);
        }

        public static IEnumerable<Tuple<DATE_TIME, DATE_TIME>> FilterByDay(this IEnumerable<Tuple<DATE_TIME, DATE_TIME>> dates, RECUR rrule)
        {
            if (rrule.BYDAY.NullOrEmpty()) return dates;
            var weekdays = rrule.BYDAY.Select(x => x.Weekday);
            return !weekdays.NullOrEmpty()? dates.Where(x => weekdays.Contains(x.Item1.ToDateTime().DayOfWeek.ToWEEKDAY())): dates;
        }

        public static IEnumerable<Tuple<DATE_TIME, DATE_TIME>> FilterByDayMonthly(this IEnumerable<Tuple<DATE_TIME, DATE_TIME>> dates, RECUR rrule)
        {
            if (rrule.BYDAY.NullOrEmpty()) return dates;
            var ranked = rrule.BYDAY.Where(x => x.OrdinalWeek != 0);
            var unranked = rrule.BYDAY.Where(x => x.OrdinalWeek == 0);
            var matches = new List<Tuple<DATE_TIME, DATE_TIME>>();
            if (!ranked.NullOrEmpty())
            {
                foreach(var r in ranked)
                {
                   matches.AddRange(dates.Where(x => x.Item1.ToDateTime().IsNthWeekDayofMonth(r.Weekday.ToDayOfWeek(), r.OrdinalWeek)));
                }

                return matches;

            }
            else
            {
                return !unranked.NullOrEmpty()
                    ? dates.Where(x => unranked.Select(y => y.Weekday).Contains(x.Item1.ToDateTime().DayOfWeek.ToWEEKDAY()))
                    : dates;
            }
        }

        public static IEnumerable<Tuple<DATE_TIME, DATE_TIME>> FilterByDayYearly(this IEnumerable<Tuple<DATE_TIME, DATE_TIME>> dates, RECUR rrule)
        {
            if (rrule.BYDAY.NullOrEmpty()) return dates;
            var N_bydays = rrule.BYDAY.Where(x => x.OrdinalWeek != 0);
            var bydays = rrule.BYDAY.Where(x => x.OrdinalWeek == 0);
            if (!N_bydays.NullOrEmpty())
            {
                return dates.Where(x => N_bydays.Contains(new WEEKDAYNUM(x.Item1.ToDateTime().WeekOfYear(),
                        x.Item1.ToDateTime().DayOfWeek.ToWEEKDAY())));

            }
            else
            {
                return !bydays.NullOrEmpty()
                    ? dates.Where(x => bydays.Select(y => y.Weekday).Contains(x.Item1.ToDateTime().DayOfWeek.ToWEEKDAY()))
                    : dates;
            }


        }

        public static IEnumerable<Tuple<DATE_TIME, DATE_TIME>> FilterByWeekNo(this IEnumerable<Tuple<DATE_TIME, DATE_TIME>> dates, RECUR rrule)
        {
            return !rrule.BYWEEKNO.NullOrEmpty()
                ?dates.Where(x => rrule.BYWEEKNO.Contains(x.Item1.ToDateTime().WeekOfYear()))
                : dates;
        }

    }
}
