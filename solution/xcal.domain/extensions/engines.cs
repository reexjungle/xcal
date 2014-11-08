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
    public static class RecurrenceEngine
    {

        #region Limit BYSETPOS

        public static IEnumerable<DATE_TIME> LimitBySetPos(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            if (rrule.BYSETPOS.NullOrEmpty()) return dates;
            
            var positives = rrule.BYSETPOS.Where(x => x > 0);
            var negatives = rrule.BYSETPOS.Where(x => x < 0);
            
            IEnumerable<DATE_TIME> results = new List<DATE_TIME>();

            IEnumerable<IGrouping<uint, DATE_TIME>> groups = null;

            //Group per recurring instance
            switch (rrule.FREQ)
            {
                case FREQ.SECONDLY:
                    groups = dates.GroupBy(x => x.SECOND); break;
                case FREQ.MINUTELY:
                    groups = dates.GroupBy(x => x.MINUTE); break;
                case FREQ.HOURLY:
                    groups = dates.GroupBy(x => x.HOUR); break;
                case FREQ.DAILY:
                    groups = dates.GroupBy(x => x.MDAY); break;
                case FREQ.WEEKLY:
                    groups = dates.GroupBy(x => 7 * x.MDAY); break;
                case FREQ.MONTHLY:
                    groups = dates.GroupBy(x => x.MONTH); break;
                case FREQ.YEARLY:
                    groups = dates.GroupBy(x => x.FULLYEAR); break;
            }

            var g = groups.ToList();

            foreach (var group in groups)
            {
                var array = group.OrderBy(x => x).ToArray(); var len = array.Count();
                results = results.Union(positives.Where(p => p >= 1 && p <= len).Select(p => array[p - 1]));
                results = results.Union(negatives.Where(n => Math.Abs(n) >= 1 && Math.Abs(n) <= len).Select(n => array[len + n])); 
            }

            return results;
        }

        #endregion

        #region Limit BYXXX

        public static IEnumerable<DATE_TIME> LimitBySecond(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            return !rrule.BYSECOND.NullOrEmpty() ? dates.Where(x => rrule.BYSECOND.Contains(x.SECOND)) : dates;
        }

        public static IEnumerable<DATE_TIME> LimitByMinute(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            return !rrule.BYMINUTE.NullOrEmpty() ? dates.Where(x => rrule.BYMINUTE.Contains(x.MINUTE)) : dates;
        }

        public static IEnumerable<DATE_TIME> LimitByHour(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            return !rrule.BYHOUR.NullOrEmpty() ? dates.Where(x => rrule.BYHOUR.Contains(x.HOUR)) : dates;
        }

        public static IEnumerable<DATE_TIME> LimitByDay(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            if (rrule.BYDAY.NullOrEmpty()) return dates;
            return rrule.BYDAY.SelectMany(x => dates.Where(d => d.ToDateTime().DayOfWeek == x.Weekday.ToDayOfWeek()));
        }

        public static IEnumerable<DATE_TIME> LimitByDayMonthly(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            if (rrule.BYDAY.NullOrEmpty()) return dates;

            var ranked = rrule.BYDAY.Where(x => x.OrdinalWeek != 0);
            var unranked = rrule.BYDAY.Where(x => x.OrdinalWeek == 0);

            IEnumerable<DATE_TIME> results = new List<DATE_TIME>();
            if (!ranked.NullOrEmpty())
            {
                results = results.Union(ranked.SelectMany(r => 
                    dates.Where(x => x.ToDateTime().IsNthWeekdayOfMonth(r.Weekday.ToDayOfWeek(), r.OrdinalWeek))));
            }

            if (!unranked.NullOrEmpty())
            {
                results = results.Union(unranked.SelectMany(u =>
                    dates.Where(x => x.ToDateTime().DayOfWeek == u.Weekday.ToDayOfWeek())));
            }

            return results;
        }

        public static IEnumerable<DATE_TIME> LimitByDayYearly(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            if (rrule.BYDAY.NullOrEmpty()) return dates;

            var ranked = rrule.BYDAY.Where(x => x.OrdinalWeek != 0);
            var unranked = rrule.BYDAY.Where(x => x.OrdinalWeek == 0);

            IEnumerable<DATE_TIME> results = new List<DATE_TIME>();
            if (!ranked.NullOrEmpty())
            {
                results = results.Union(ranked.SelectMany(r =>
                    dates.Where(x => x.ToDateTime().IsNthWeekdayOfYear(r.Weekday.ToDayOfWeek(), r.OrdinalWeek))));
            }


            if (!unranked.NullOrEmpty())
            {
                results = results.Union(unranked.SelectMany(u =>
                    dates.Where(x => x.ToDateTime().DayOfWeek == u.Weekday.ToDayOfWeek())));
            }

            return results;
        }

        public static IEnumerable<DATE_TIME> LimitByMonthDay(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            if (rrule.BYMONTHDAY.NullOrEmpty()) return dates;

            var positives = rrule.BYMONTHDAY.Where(x => x > 0).ToList();
            var negatives = rrule.BYMONTHDAY.Where(x => x < 0).ToList();

            IEnumerable<DATE_TIME> results = new List<DATE_TIME>();
            var bymonths = dates.GroupBy(x => x.MONTH); // group by months

            if (!positives.NullOrEmpty())
                results = results.Union(bymonths.SelectMany(bymonth => bymonth.Where(x => positives.Contains((int)x.MDAY))));

            if (!negatives.NullOrEmpty())
            {
                results = results.Union(bymonths.SelectMany(bymonth => bymonth.Where(x =>
                {
                    var max = DateTime.DaysInMonth((int)x.FULLYEAR, (int)x.MONTH);
                    var normalized = negatives.Select(n => (max + 1) + n); //max + 1 - N
                    return normalized.Contains((int)x.MDAY);
                })));
            }
            return results;
        }

        public static IEnumerable<DATE_TIME> LimitByYearDay(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            if (rrule.BYYEARDAY.NullOrEmpty()) return dates;

            var positives = rrule.BYYEARDAY.Where(x => x > 0).ToList();
            var negatives = rrule.BYYEARDAY.Where(x => x < 0).ToList();

            IEnumerable<DATE_TIME> results = new List<DATE_TIME>();
            var byyears = dates.GroupBy(x => x.FULLYEAR); //group by year

            if (!positives.NullOrEmpty())
                results = results.Union(byyears.SelectMany(byyear => byyear.Where(x => positives.Contains(x.ToDateTime().DayOfYear))));

            if (!negatives.NullOrEmpty())
            {
                results = results.Union(byyears.SelectMany(byyear => byyear.Where(x =>
                {
                    var max = new DateTime((int)x.FULLYEAR, 12, 31).DayOfYear;
                    var normalized = negatives.Select(n => (max + 1) + n); //max + 1 - N
                    return normalized.Contains(x.ToDateTime().DayOfYear);
                })));
            }

            return results;
        }

        public static IEnumerable<DATE_TIME> LimitByMonth(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            return !rrule.BYMONTH.NullOrEmpty() ? dates.Where(x => rrule.BYMONTH.Contains(x.MONTH)) : dates;
        }

        public static IEnumerable<DATE_TIME> LimitByWeekNo(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            if (rrule.BYWEEKNO.NullOrEmpty()) return dates;

            var positives = rrule.BYWEEKNO.Where(x => x > 0).ToList();
            var negatives = rrule.BYWEEKNO.Where(x => x < 0).ToList();

            IEnumerable<DATE_TIME> results = new List<DATE_TIME>();
            var byweeks = dates.GroupBy(x => x.ToDateTime().WeekOfYear()); // group by weeks

            if (!positives.NullOrEmpty())
                results = results.Union(byweeks.SelectMany(byweek => byweek.Where(x => positives.Contains(x.ToDateTime().WeekOfYear()))));

            if (!negatives.NullOrEmpty())
            {
                results = results.Union(byweeks.SelectMany(byweek => byweek.Where(x =>
                {
                    var max = new DateTime((int)x.FULLYEAR, 12, 31).WeekOfYear();
                    var normalized = negatives.Select(n => (max + 1) + n); //max + 1 - N
                    return normalized.Contains(x.ToDateTime().WeekOfYear());
                })));
            }
            return results;
        }

        #endregion

        #region Expand BYXXX
        
        public static IEnumerable<DATE_TIME> ExpandBySecond(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            if (rrule.BYSECOND.NullOrEmpty()) return dates;
            dates = dates.Union(dates.SelectMany(date => rrule.BYSECOND.Select(x => 
                new DATE_TIME(date.FULLYEAR, date.MONTH, date.MDAY, date.HOUR, date.MINUTE, x, date.Type, date.TimeZoneId))));
            
            return dates.LimitBySecond(rrule);
        }

        public static IEnumerable<DATE_TIME> ExpandByMinute(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            if (rrule.BYMINUTE.NullOrEmpty()) return dates;
            dates = dates.Union(dates.SelectMany(date => rrule.BYMINUTE.Select(x =>
                new DATE_TIME(date.FULLYEAR, date.MONTH, date.MDAY, date.HOUR, x, date.SECOND, date.Type, date.TimeZoneId))));

            return dates.LimitByMinute(rrule);
        }

        public static IEnumerable<DATE_TIME> ExpandByHour(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            if (rrule.BYHOUR.NullOrEmpty()) return dates;
            dates = dates.Union(dates.SelectMany(date => rrule.BYHOUR.Select(x =>
                new DATE_TIME(date.FULLYEAR, date.MONTH, date.MDAY, x, date.MINUTE, date.SECOND, date.Type, date.TimeZoneId))));

            return dates.LimitByHour(rrule);
        }

        public static  IEnumerable<DATE_TIME> ExpandByDayWeekly(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            if (rrule.BYDAY.NullOrEmpty()) return dates;

            dates = dates.Union(dates.SelectMany(date => rrule.BYDAY.SelectMany(x => 
            {
                var d = date.ToDateTime();
                return  x.Weekday.ToDayOfWeek().GetSimilarDatesInRange(d, d.AddDays(7)).ToDATE_TIMEs(date.TimeZoneId);
            })));

            return dates.LimitByDay(rrule);
        }

        public static IEnumerable<DATE_TIME> ExpandByDayMonthly(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            if (rrule.BYDAY.NullOrEmpty()) return dates;

            var ranked = rrule.BYDAY.Where(x => x.OrdinalWeek != 0);
            var unranked = rrule.BYDAY.Where(x => x.OrdinalWeek == 0);

            if (!ranked.NullOrEmpty())
            {
                dates = dates.Union(dates.SelectMany(date => ranked.Select(r =>
                    r.Weekday.ToDayOfWeek().GetNthDateOfMonth(r.OrdinalWeek, (int)date.MONTH, (int)date.FULLYEAR)).ToDATE_TIMEs(date.TimeZoneId)));
            }

            if (!unranked.NullOrEmpty())
            {
                dates = dates.Union(dates.SelectMany(date => unranked.SelectMany(u => u.Weekday.ToDayOfWeek().GetSimilarDatesOfMonth((int)date.MONTH, (int)date.FULLYEAR).ToDATE_TIMEs(date.TimeZoneId)).ToList()));

            }

            return dates.LimitByDayMonthly(rrule);
        }

        public static IEnumerable<DATE_TIME> ExpandByDayYearly(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            if (rrule.BYDAY.NullOrEmpty()) return dates;

            IEnumerable<DateTime> results = new List<DateTime>();
            var ranked = rrule.BYDAY.Where(x => x.OrdinalWeek != 0);
            var unranked = rrule.BYDAY.Where(x => x.OrdinalWeek == 0);

            if (!ranked.NullOrEmpty())
            {
                dates = dates.Union(dates.SelectMany(date => ranked.Select(r =>
                    r.Weekday.ToDayOfWeek().GetNthDateOfYear((int)date.FULLYEAR, r.OrdinalWeek)).ToDATE_TIMEs(date.TimeZoneId)));
            }

            if (!unranked.NullOrEmpty())
            {
                results = results.Union(dates.SelectMany(date => unranked.SelectMany(u =>
                    u.Weekday.ToDayOfWeek().GetSimilarDatesOfYear((int)date.FULLYEAR))));
            }

            return dates.LimitByDayYearly(rrule);
        }

        public static IEnumerable<DATE_TIME> ExpandByMonthDay(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            if (rrule.BYMONTHDAY.NullOrEmpty()) return dates;
            var positives = rrule.BYYEARDAY.Where(x => x > 0);
            var negatives = rrule.BYYEARDAY.Where(x => x < 0);
            if (!positives.NullOrEmpty())
            {
                dates = dates.Union(dates.SelectMany(date => positives.Select(x =>
                    new DATE_TIME(date.FULLYEAR, date.MONTH, (uint)x, date.HOUR, date.MINUTE, date.SECOND, date.Type, date.TimeZoneId)))); 
            }

            if (!negatives.NullOrEmpty())
            {
                dates = dates.Union(dates.SelectMany(date => negatives.Select(x =>
                {
                    var day =  DateTime.DaysInMonth((int)date.FULLYEAR, (int)date.MONTH) + 1 + x;
                    return new DATE_TIME(date.FULLYEAR, date.MONTH, (uint)day, date.HOUR, date.MINUTE, date.SECOND, date.Type, date.TimeZoneId);
                }))); 
            }

            return dates.LimitByMonthDay(rrule);
        }

        public static IEnumerable<DATE_TIME> ExpandByYearDay(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            if (rrule.BYMONTHDAY.NullOrEmpty()) return dates;
            var positives = rrule.BYYEARDAY.Where(x => x > 0);
            var negatives = rrule.BYYEARDAY.Where(x => x < 0);

            if (!positives.NullOrEmpty())
            {
                dates = dates.Union(dates.SelectMany(date => positives.Select(x =>
                     new DateTime((int)date.FULLYEAR, 1, 1, (int)date.HOUR, (int)date.MINUTE, (int)date.SECOND).AddDays(x - 1).ToDATE_TIME(date.TimeZoneId))));
            }

            if (!negatives.NullOrEmpty())
            {
                dates = dates.Union(dates.SelectMany(date => negatives.Select(x =>
                     new DateTime((int)date.FULLYEAR, 12, 31, (int)date.HOUR, (int)date.MINUTE, (int)date.SECOND).AddDays(x + 1).ToDATE_TIME(date.TimeZoneId))));
            }

            return dates.LimitByYearDay(rrule);
        }
        
        public static IEnumerable<DATE_TIME> ExpandByWeekNo(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        { 
            if (rrule.BYWEEKNO.NullOrEmpty()) return dates;
            var positives = rrule.BYWEEKNO.Where(x => x > 0);
            var negatives = rrule.BYWEEKNO.Where(x => x < 0);


            if (!positives.NullOrEmpty())
            {
                dates = dates.Union(dates.SelectMany(date => positives.Select(x =>
                    new DateTime((int)date.FULLYEAR, 1, 1, (int)date.HOUR, (int)date.MINUTE, (int)date.SECOND).AddWeeks(x).ToDATE_TIME(date.TimeZoneId))));
            }

            if (!negatives.NullOrEmpty())
            {
                dates = dates.Union(dates.SelectMany(date => negatives.Select(x =>
                    new DateTime((int)date.FULLYEAR, 12, 31, (int)date.HOUR, (int)date.MINUTE, (int)date.SECOND).AddWeeks(x).ToDATE_TIME(date.TimeZoneId))));
            }

            return dates.LimitByWeekNo(rrule);
        }

        public static IEnumerable<DATE_TIME> ExpandByMonth(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            if (rrule.BYMONTH.NullOrEmpty()) return dates;
            dates = dates.Union(dates.SelectMany(date => rrule.BYMONTH.Select(x =>
                new DATE_TIME(date.FULLYEAR, x, date.MDAY, date.HOUR, date.MINUTE, date.SECOND, date.Type, date.TimeZoneId))));

            return dates.LimitByMonth(rrule);
        }

        #endregion



    }
}
