using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xmisc.foundation.concretes;
using ServiceStack.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace reexjungle.xcal.domain.extensions
{
    public static class RecurrenceEngine
    {
        private static IEnumerable<DATE_TIME> FilterByPosition<TKey>(this IList<IGrouping<TKey, DATE_TIME>> groups,
            IList<int> ppositions, IList<int> npositions)
        {
            var count = groups.Count;

            //Find dates in group indexed by the position
            var pdates = ppositions.Where(pos => pos <= count).SelectMany(pos => groups[pos - 1]);
            var ndates = npositions.Where(pos => Math.Abs(pos) <= count).SelectMany(pos => groups[count + pos]);

            return pdates.Union(ndates);
        }

        #region Limit BYSETPOS

        private static IEnumerable<DATE_TIME> LimitBySetPosSecondly(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYSETPOS.Empty()) return dates;

            var ppositions = rule.BYSETPOS.Where(x => x > 0).ToList();
            var npositions = rule.BYSETPOS.Where(x => x < 0).ToList();

            var results = Enumerable.Empty<DATE_TIME>();
            if (rule.BYSECOND.Any())
            {
                results = dates.GroupBy(date => date.SECOND).ToList()
                    .FilterByPosition(ppositions, npositions);
            }

            return results;
        }

        private static IEnumerable<DATE_TIME> LimitBySetPosMinutely(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYSETPOS.Empty()) return dates;

            var ppositions = rule.BYSETPOS.Where(x => x > 0).ToList();
            var npositions = rule.BYSETPOS.Where(x => x < 0).ToList();

            var minuteGroups = dates.GroupBy(date => date.MINUTE).ToList();
            var results = Enumerable.Empty<DATE_TIME>();

            if (rule.BYSECOND.Any())
            {
                results = minuteGroups
                    .Select(minuteGroup => minuteGroup.GroupBy(date => date.SECOND).ToList())
                    .Select(secondGroups => secondGroups.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            if (rule.BYMINUTE.Any())
            {
                results = minuteGroups.FilterByPosition(ppositions, npositions);
            }

            return results;
        }

        private static IEnumerable<DATE_TIME> LimitBySetPosHourly(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYSETPOS.Empty()) return dates;

            var ppositions = rule.BYSETPOS.Where(x => x > 0).ToList();
            var npositions = rule.BYSETPOS.Where(x => x < 0).ToList();

            var hourGroups = dates.GroupBy(date => date.HOUR).ToList();
            var results = Enumerable.Empty<DATE_TIME>();

            if (rule.BYSECOND.Any())
            {
                results = hourGroups
                    .Select(hourGroup => hourGroup.GroupBy(date => date.SECOND).ToList())
                    .Select(secondGroups => secondGroups.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            if (rule.BYMINUTE.Any())
            {
                results = hourGroups
                    .Select(hourGroup => hourGroup.GroupBy(date => date.MINUTE).ToList())
                    .Select(minuteGroups => minuteGroups.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            if (rule.BYHOUR.Any())
            {
                results = hourGroups.FilterByPosition(ppositions, npositions);
            }

            return results;
        }

        private static IEnumerable<DATE_TIME> LimitBySetPosDaily(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYSETPOS.Empty()) return dates;

            var ppositions = rule.BYSETPOS.Where(x => x > 0).ToList();
            var npositions = rule.BYSETPOS.Where(x => x < 0).ToList();

            var dayGroups = dates.GroupBy(date => date.MDAY).ToList();
            var results = Enumerable.Empty<DATE_TIME>();

            if (rule.BYSECOND.Any())
            {
                results = dayGroups
                    .Select(dayGroup => dayGroup.GroupBy(date => date.SECOND).ToList())
                    .Select(secondGroups => secondGroups.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            if (rule.BYMINUTE.Any())
            {
                results = dayGroups
                    .Select(dayGroup => dayGroup.GroupBy(date => date.MINUTE).ToList())
                    .Select(minuteGroups => minuteGroups.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            if (rule.BYHOUR.Any())
            {
                results = dayGroups
                    .Select(dayGroup => dayGroup.GroupBy(date => date.HOUR).ToList())
                    .Select(hourGroups => hourGroups.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            if (rule.BYDAY.Any())
            {
                results = dayGroups
                    .Select(dayGroup => dayGroup.GroupBy(date =>
                    {
                        var dateTime = date.ToDateTime();
                        return new WEEKDAYNUM(dateTime.Day, dateTime.DayOfWeek.ToWEEKDAY());
                    }).ToList())
                    .Select(weekDayGroups => weekDayGroups.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            if (rule.BYMONTHDAY.Any())
            {
                results = dayGroups.FilterByPosition(ppositions, npositions);
            }

            return results;
        }

        private static IEnumerable<DATE_TIME> LimitBySetPosWeekly(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYSETPOS.Empty()) return dates;

            var ppositions = rule.BYSETPOS.Where(x => x > 0).ToList();
            var npositions = rule.BYSETPOS.Where(x => x < 0).ToList();

            var weekGroups = dates.GroupBy(date => 7*date.MDAY).ToList();
            var results = Enumerable.Empty<DATE_TIME>();

            if (rule.BYSECOND.Any())
            {
                results = weekGroups
                    .Select(dayGroup => dayGroup.GroupBy(date => date.SECOND).ToList())
                    .Select(secondGroups => secondGroups.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            if (rule.BYMINUTE.Any())
            {
                results = weekGroups
                    .Select(dayGroup => dayGroup.GroupBy(date => date.MINUTE).ToList())
                    .Select(minuteGroups => minuteGroups.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            if (rule.BYHOUR.Any())
            {
                results = weekGroups
                    .Select(dayGroup => dayGroup.GroupBy(date => date.HOUR).ToList())
                    .Select(hourGroups => hourGroups.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            if (rule.BYDAY.Any())
            {
                results = weekGroups
                    .Select(weekGroup => weekGroup.GroupBy(date =>
                    {
                        var dateTime = date.ToDateTime();
                        return new WEEKDAYNUM(dateTime.Day, dateTime.DayOfWeek.ToWEEKDAY());
                    }).ToList())
                    .Select(weekDayGroups => weekDayGroups.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            return results;
        }

        private static IEnumerable<DATE_TIME> LimitBySetPosMonthly(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYSETPOS.Empty()) return dates;

            var ppositions = rule.BYSETPOS.Where(x => x > 0).ToList();
            var npositions = rule.BYSETPOS.Where(x => x < 0).ToList();

            var monthGroups = dates.GroupBy(date => date.MONTH).ToList();
            var results = Enumerable.Empty<DATE_TIME>();

            if (rule.BYSECOND.Any())
            {
                results = monthGroups
                    .Select(monthGroup => monthGroup.GroupBy(date => date.SECOND).ToList())
                    .Select(secondGroups => secondGroups.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            if (rule.BYMINUTE.Any())
            {
                results = monthGroups
                    .Select(monthGroup => monthGroup.GroupBy(date => date.MINUTE).ToList())
                    .Select(minuteGroups => minuteGroups.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            if (rule.BYHOUR.Any())
            {
                results = monthGroups
                    .Select(monthGroup => monthGroup.GroupBy(date => date.HOUR).ToList())
                    .Select(hourGroups => hourGroups.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            if (rule.BYDAY.Any())
            {
                results = monthGroups
                    .Select(monthGroup => monthGroup.GroupBy(date =>
                    {
                        var dateTime = date.ToDateTime();
                        return new WEEKDAYNUM(dateTime.Day, dateTime.DayOfWeek.ToWEEKDAY());
                    }).ToList())
                    .Select(weekDayGroups => weekDayGroups.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            if (rule.BYMONTHDAY.Any())
            {
                results = monthGroups
                    .Select(monthGroup => monthGroup.GroupBy(date => date.MDAY).ToList())
                    .Select(dayGroup => dayGroup.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            if (rule.BYMONTH.Any())
            {
                results = monthGroups.FilterByPosition(ppositions, npositions);
            }

            return results;
        }

        private static IEnumerable<DATE_TIME> LimitBySetPosYearly(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYSETPOS.Empty()) return dates;

            var ppositions = rule.BYSETPOS.Where(x => x > 0).ToList();
            var npositions = rule.BYSETPOS.Where(x => x < 0).ToList();

            var yearGroups = dates.GroupBy(date => date.FULLYEAR).ToList();
            var results = Enumerable.Empty<DATE_TIME>();

            if (rule.BYSECOND.Any())
            {
                results = yearGroups
                    .Select(yearGroup => yearGroup.GroupBy(date => date.SECOND).ToList())
                    .Select(secondGroups => secondGroups.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            if (rule.BYMINUTE.Any())
            {
                results = yearGroups
                    .Select(yearGroup => yearGroup.GroupBy(date => date.MINUTE).ToList())
                    .Select(minuteGroups => minuteGroups.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            if (rule.BYHOUR.Any())
            {
                results = yearGroups
                    .Select(yearGroup => yearGroup.GroupBy(date => date.HOUR).ToList())
                    .Select(hourGroups => hourGroups.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            if (rule.BYDAY.Any())
            {
                results = yearGroups
                    .Select(yearGroup => yearGroup.GroupBy(date =>
                    {
                        var dateTime = date.ToDateTime();
                        return new WEEKDAYNUM(dateTime.DayOfYear, dateTime.DayOfWeek.ToWEEKDAY());
                    }).ToList())
                    .Select(weekDayGroups => weekDayGroups.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            if (rule.BYMONTHDAY.Any())
            {
                results = yearGroups
                    .Select(yearGroup => yearGroup.GroupBy(date => date.MDAY).ToList())
                    .Select(dayGroup => dayGroup.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            if (rule.BYYEARDAY.Any())
            {
                results = yearGroups
                    .Select(yearGroup => yearGroup.GroupBy(date => date.ToDateTime().DayOfYear).ToList())
                    .Select(dayGroup => dayGroup.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            if (rule.BYWEEKNO.Any())
            {
                results = yearGroups
                    .Select(yearGroup => yearGroup.GroupBy(date => date.ToDateTime().WeekOfYear()).ToList())
                    .Select(weekGroup => weekGroup.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            if (rule.BYMONTH.Any())
            {
                results = yearGroups
                    .Select(yearGroup => yearGroup.GroupBy(date => date.MONTH).ToList())
                    .Select(monthGroup => monthGroup.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            return results;
        }

        #endregion Limit BYSETPOS

        #region Limit BYXXX

        public static IEnumerable<DATE_TIME> LimitByMonth(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            return rule.BYMONTH.Any()
                ? dates.Where(x => rule.BYMONTH.Contains(x.MONTH))
                : dates;
        }

        public static IEnumerable<DATE_TIME> LimitByYearDay(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYYEARDAY.NullOrEmpty()) return dates;

            //Sort out positive and negative year days.
            //Positive year days are days of the year with counting starting from 01.01.
            //Negative year days are days of the year which are counted backwards from the last day of the year.

            var pdays = rule.BYYEARDAY.Where(x => x > 0).ToList();
            var ndays = rule.BYYEARDAY.Where(x => x < 0).ToList();

            var byyears = dates.GroupBy(x => x.FULLYEAR).ToList(); //group by year
            var pdates = Enumerable.Empty<DATE_TIME>();
            var ndates = Enumerable.Empty<DATE_TIME>();

            foreach (var byyear in byyears)
            {
                pdates = pdates
                    .Union(byyear.Where(date => pdays.Contains(date.ToDateTime().DayOfYear))
                        .Select(date => date));

                ndates = ndates
                    .Union(byyear.Where(date => ndays.Contains(date.ToDateTime().DayOfYear))
                        .Select(date => date));
            }

            return pdates.Union(ndates);
        }

        public static IEnumerable<DATE_TIME> LimitByDay(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYDAY.NullOrEmpty()) return dates;

            if (rule.FREQ == FREQ.MONTHLY)
            {
                var weekdaynums = rule.BYDAY.Where(x => x.Number != 0).ToList();
                var nweekdaynums = rule.BYDAY.Where(x => x.Number == 0).ToList();
                return dates.LimitByDayMonthly(rule, weekdaynums, nweekdaynums);
            }

            if (rule.FREQ == FREQ.YEARLY)
            {
                var weekdaynums = rule.BYDAY.Where(x => x.Number != 0).ToList();
                var nweekdaynums = rule.BYDAY.Where(x => x.Number == 0).ToList();
                return dates.LimitByDayYearly(rule, weekdaynums, nweekdaynums);
            }

            //In case it is neither the monthly nor the yearly rule, then limit by the specified frequency.
            return rule.BYDAY.Any()
                ? rule.BYDAY.SelectMany(byday => dates.Where(date => date.ToDateTime().DayOfWeek.ToWEEKDAY() == byday.Weekday))
                : dates;
        }

        private static IEnumerable<DATE_TIME> LimitByDayMonthly(this IEnumerable<DATE_TIME> dates,
            RECUR rule,
            IList<WEEKDAYNUM> nweekdaynums,
            IList<WEEKDAYNUM> weekdaynums)
        {
            var results = Enumerable.Empty<DATE_TIME>();
            var nresults = Enumerable.Empty<DATE_TIME>();

            if (nweekdaynums.Any())
            {
                results = nweekdaynums
                    .SelectMany(nweekdaynum => dates.Where(date => date.ToDateTime().IsNthWeekdayOfMonth(nweekdaynum.Weekday.ToDayOfWeek(), nweekdaynum.Number)));
            }

            if (weekdaynums.Any())
                nresults = weekdaynums.SelectMany(byday => dates.Where(date => date.ToDateTime().DayOfWeek.ToWEEKDAY() == byday.Weekday));

            return results.Union(nresults);
        }

        private static IEnumerable<DATE_TIME> LimitByDayYearly(this IEnumerable<DATE_TIME> dates, 
            RECUR rule, 
            IList<WEEKDAYNUM> nweekdaynums,
            IList<WEEKDAYNUM> weekdaynums)
        {

            var results = Enumerable.Empty<DATE_TIME>();
            var nresults = Enumerable.Empty<DATE_TIME>();

            if (nweekdaynums.Any())
            {
                if (rule.BYMONTH.Any()) return dates.LimitByDayMonthly(rule, nweekdaynums, nweekdaynums);

                if (rule.BYWEEKNO.Any())
                {
                    results = nweekdaynums
                        .SelectMany(nweekdaynum => dates.Where(date => date.ToDateTime().IsNthWeekdayOfYear(nweekdaynum.Weekday.ToDayOfWeek(), nweekdaynum.Number)));

                }
            }

            if (weekdaynums.Any())
                nresults = weekdaynums.SelectMany(byday => dates.Where(date => date.ToDateTime().DayOfWeek.ToWEEKDAY() == byday.Weekday));

            return results.Union(nresults);
        }

        public static IEnumerable<DATE_TIME> LimitByMonthDay(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYMONTHDAY.NullOrEmpty()) return dates;

            var pmonthdays = rule.BYMONTHDAY.Where(x => x > 0).ToList();
            var nmonthdays = rule.BYMONTHDAY.Where(x => x < 0).ToList();

            var presults = Enumerable.Empty<DATE_TIME>();
            var nresults = Enumerable.Empty<DATE_TIME>();

            var bymonths = dates.GroupBy(x => x.MONTH).ToList(); // group by months

            if (pmonthdays.Any())
                 presults = bymonths.SelectMany(bymonth => bymonth.Where(x => pmonthdays.Contains((int) x.MDAY)));

            if (!nmonthdays.NullOrEmpty())
            {
                nresults = bymonths.SelectMany(bymonth => bymonth.Where(x =>
                {
                    var max = DateTime.DaysInMonth((int) x.FULLYEAR, (int) x.MONTH);
                    var normalized = nmonthdays.Select(n => (max + 1) + n); //max + 1 - N
                    return normalized.Contains((int) x.MDAY);
                }));
            }
            return presults.Union(nresults);
        }

        public static IEnumerable<DATE_TIME> LimitBySecond(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            return rule.BYSECOND.Any()
                ? dates.Where(x => rule.BYSECOND.Contains(x.SECOND))
                : dates;
        }

        public static IEnumerable<DATE_TIME> LimitByMinute(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            return rule.BYMINUTE.Any()
                ? dates.Where(x => rule.BYMINUTE.Contains(x.MINUTE))
                : dates;
        }

        public static IEnumerable<DATE_TIME> LimitByHour(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            return rule.BYHOUR.Any()
                ? dates.Where(x => rule.BYHOUR.Contains(x.HOUR))
                : dates;
        }

        public static IEnumerable<DATE_TIME> LimitByWeekNo(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYWEEKNO.NullOrEmpty()) return dates;

            var pweeknos = rule.BYWEEKNO.Where(x => x > 0).ToList();
            var nweeknos = rule.BYWEEKNO.Where(x => x < 0).ToList();

            var presults = Enumerable.Empty<DATE_TIME>();
            var nresults = Enumerable.Empty<DATE_TIME>();

            var byweeks = dates.GroupBy(x => x.ToDateTime().WeekOfYear()).ToList(); // group by weeks

            if (pweeknos.Any())
                presults = byweeks.SelectMany(byweek => byweek.Where(x => pweeknos.Contains(x.ToDateTime().WeekOfYear())));

            if (!nweeknos.NullOrEmpty())
            {
                nresults = byweeks.SelectMany(byweek => byweek.Where(x =>
                {
                    var max = new DateTime((int) x.FULLYEAR, 12, 31).WeekOfYear();
                    var normalized = nweeknos.Select(n => (max + 1) + n); //max + 1 - N
                    return normalized.Contains(x.ToDateTime().WeekOfYear());
                }));
            }
            return presults.Union(nresults);
        }

        #endregion Limit BYXXX

        #region Expand BYXXX

        public static IEnumerable<DATE_TIME> ExpandBySecond(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYSECOND.NullOrEmpty()) return dates;

            var results = new List<DATE_TIME>();
            foreach (var date in dates)
            {
                results.Add(date);
                results.AddRange(
                    rule.BYSECOND.Select(
                        second =>
                            new DATE_TIME(date.FULLYEAR, date.MONTH, date.MDAY, date.HOUR, date.MINUTE, second,
                                date.Type, date.TimeZoneId)));
            }

            return results;
        }

        public static IEnumerable<DATE_TIME> ExpandByMinute(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYMINUTE.NullOrEmpty()) return dates;

            var results = new List<DATE_TIME>();
            foreach (var date in dates)
            {
                results.Add(date);
                results.AddRange(
                    rule.BYMINUTE.Select(
                        minute =>
                            new DATE_TIME(date.FULLYEAR, date.MONTH, date.MDAY, date.HOUR, minute, date.SECOND,
                                date.Type, date.TimeZoneId)));
            }

            return results;
        }

        public static IEnumerable<DATE_TIME> ExpandByHour(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYHOUR.NullOrEmpty()) return dates;

            var results = new List<DATE_TIME>();
            foreach (var date in dates)
            {
                results.Add(date);
                results.AddRange(
                    rule.BYHOUR.Select(
                        hour =>
                            new DATE_TIME(date.FULLYEAR, date.MONTH, date.MDAY, hour, date.MINUTE, date.SECOND,
                                date.Type, date.TimeZoneId)));
            }

            return results;
        }

        public static IEnumerable<DATE_TIME> ExpandByDayWeekly(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYDAY.NullOrEmpty()) return dates;

            var results = dates.SelectMany(date => rule.BYDAY.SelectMany(x =>
            {
                var sdate = date.ToDateTime();
                return x.Weekday.ToDayOfWeek().GetSimilarDatesInRange(sdate, sdate.AddDays(7)).ToDATE_TIMEs(date.TimeZoneId);
            }));

            return results;
        }

        public static IEnumerable<DATE_TIME> ExpandByDayMonthly(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYDAY.NullOrEmpty()) return dates;

            var ranked = rule.BYDAY.Where(x => x.Number != 0);
            var unranked = rule.BYDAY.Where(x => x.Number == 0);

            if (!ranked.NullOrEmpty())
            {
                dates = dates.Union(dates.SelectMany(date => ranked.Select(r =>
                    r.Weekday.ToDayOfWeek()
                        .GetNthDateOfMonth(r.Number, (int) date.FULLYEAR, (int) date.MONTH, (int) date.HOUR,
                            (int) date.MINUTE, (int) date.SECOND))
                    .ToDATE_TIMEs(date.TimeZoneId)));
            }

            if (!unranked.NullOrEmpty())
            {
                dates = dates.Union(dates.SelectMany(date => unranked.SelectMany(u =>
                    u.Weekday.ToDayOfWeek()
                        .GetSimilarDatesOfMonth((int) date.FULLYEAR, (int) date.MONTH, (int) date.HOUR,
                            (int) date.MINUTE, (int) date.SECOND).ToDATE_TIMEs(date.TimeZoneId)).ToList()));
            }

            return dates.LimitByDay(rule);
        }

        public static IEnumerable<DATE_TIME> ExpandByDayYearly(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYDAY.NullOrEmpty()) return dates;

            IEnumerable<DateTime> results = new List<DateTime>();
            var ranked = rule.BYDAY.Where(x => x.Number != 0);
            var unranked = rule.BYDAY.Where(x => x.Number == 0);

            if (!ranked.NullOrEmpty())
            {
                dates = dates.Union(dates.SelectMany(date => ranked.Select(r =>
                    r.Weekday.ToDayOfWeek().GetNthDateOfYear((int) date.FULLYEAR, r.Number))
                    .ToDATE_TIMEs(date.TimeZoneId)));
            }

            if (!unranked.NullOrEmpty())
            {
                results = results.Union(dates.SelectMany(date => unranked.SelectMany(u =>
                    u.Weekday.ToDayOfWeek().GetSimilarDatesOfYear((int) date.FULLYEAR))));
            }

            return dates.LimitByDay(rule);
        }

        public static IEnumerable<DATE_TIME> ExpandByMonthDay(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYMONTHDAY.NullOrEmpty()) return dates;
            var positives = rule.BYMONTHDAY.Where(x => x > 0);
            var negatives = rule.BYMONTHDAY.Where(x => x < 0);
            if (!positives.NullOrEmpty())
            {
                dates = dates.Union(dates.SelectMany(date => positives.Select(x =>
                    new DATE_TIME(date.FULLYEAR, date.MONTH, (uint) x, date.HOUR, date.MINUTE, date.SECOND, date.Type,
                        date.TimeZoneId))));
            }

            if (!negatives.NullOrEmpty())
            {
                dates = dates.Union(dates.SelectMany(date => negatives.Select(x =>
                {
                    var day = DateTime.DaysInMonth((int) date.FULLYEAR, (int) date.MONTH) + 1 + x;
                    return new DATE_TIME(date.FULLYEAR, date.MONTH, (uint) day, date.HOUR, date.MINUTE, date.SECOND,
                        date.Type, date.TimeZoneId);
                })));
            }

            return dates.LimitByMonthDay(rule);
        }

        public static IEnumerable<DATE_TIME> ExpandByYearDay(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYYEARDAY.NullOrEmpty()) return dates;
            var positives = rule.BYYEARDAY.Where(x => x > 0);
            var negatives = rule.BYYEARDAY.Where(x => x < 0);

            if (!positives.NullOrEmpty())
            {
                dates = dates.Union(dates.SelectMany(date => positives.Select(x =>
                    new DateTime((int) date.FULLYEAR, 1, 1, (int) date.HOUR, (int) date.MINUTE, (int) date.SECOND)
                        .AddDays(x - 1).ToDATE_TIME(date.TimeZoneId))));
            }

            if (!negatives.NullOrEmpty())
            {
                dates = dates.Union(dates.SelectMany(date => negatives.Select(x =>
                    new DateTime((int) date.FULLYEAR, 12, 31, (int) date.HOUR, (int) date.MINUTE, (int) date.SECOND)
                        .AddDays(x + 1).ToDATE_TIME(date.TimeZoneId))));
            }

            return dates.LimitByYearDay(rule);
        }

        public static IEnumerable<DATE_TIME> ExpandByWeekNo(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYWEEKNO.NullOrEmpty()) return dates;
            var positives = rule.BYWEEKNO.Where(x => x > 0);
            var negatives = rule.BYWEEKNO.Where(x => x < 0);

            if (!positives.NullOrEmpty())
            {
                dates = dates.Union(dates.SelectMany(date => positives.Select(x =>
                    new DateTime((int) date.FULLYEAR, 1, 1, (int) date.HOUR, (int) date.MINUTE, (int) date.SECOND)
                        .AddWeeks(x).ToDATE_TIME(date.TimeZoneId))));
            }

            if (!negatives.NullOrEmpty())
            {
                dates = dates.Union(dates.SelectMany(date => negatives.Select(x =>
                    new DateTime((int) date.FULLYEAR, 12, 31, (int) date.HOUR, (int) date.MINUTE, (int) date.SECOND)
                        .AddWeeks(x).ToDATE_TIME(date.TimeZoneId))));
            }

            return dates.LimitByWeekNo(rule);
        }

        public static IEnumerable<DATE_TIME> ExpandByMonth(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYMONTH.Empty()) return dates;

            var results = new List<DATE_TIME>();
            foreach (var date in dates)
            {
                results.Add(date);
                results.AddRange(
                    rule.BYMONTH.Select(
                        month =>
                            new DATE_TIME(date.FULLYEAR, month, date.MDAY, date.HOUR, date.MINUTE, date.SECOND,
                                date.Type, date.TimeZoneId)));
            }

            return results;
        }

        #endregion Expand BYXXX

        #region Generate Recurrence Dates

        private static DATE_TIME CalculateWindowLimit(this RECUR rule, DATE_TIME start, int window)
        {
            if (rule.FREQ == FREQ.SECONDLY && rule.INTERVAL < 24*60*60)
            {
                return start.ToDateTime().AddMinutes(window).ToDATE_TIME(start.TimeZoneId);
            }
            if (rule.FREQ == FREQ.MINUTELY && rule.INTERVAL < 24*60)
            {
                return start.ToDateTime().AddHours(window).ToDATE_TIME(start.TimeZoneId);
            }
            if (rule.FREQ == FREQ.HOURLY && rule.INTERVAL < 24)
            {
                return start.ToDateTime().AddDays(window).ToDATE_TIME(start.TimeZoneId);
            }
            return start.ToDateTime().AddMonths(window).ToDATE_TIME(start.TimeZoneId);
        }

        public static IEnumerable<DATE_TIME> GenerateSecondlyRecurrences(this RECUR rule, DATE_TIME start, DATE_TIME end)
        {
            var dates = new List<DATE_TIME>();
            while (start < end)
            {
                dates.Add(start = start.AddSeconds(rule.INTERVAL));
            }

            //filter
            return dates
                .LimitByMonth(rule)
                .LimitByYearDay(rule)
                .LimitByMonthDay(rule)
                .LimitByDay(rule)
                .LimitByHour(rule)
                .LimitByMinute(rule)
                .LimitBySecond(rule)
                .LimitBySetPosSecondly(rule);
        }

        public static IEnumerable<DATE_TIME> GenerateMinutelyRecurrences(this RECUR rule, DATE_TIME start, DATE_TIME end)
        {
            var dates = new List<DATE_TIME>();
            while (start < end)
            {
                dates.Add(start = start.AddMinutes(rule.INTERVAL));
            }

            //filter
            return dates
                .LimitByMonth(rule)
                .LimitByYearDay(rule)
                .LimitByMonthDay(rule)
                .LimitByDay(rule)
                .LimitByHour(rule)
                .LimitByMinute(rule)
                .ExpandBySecond(rule)
                .LimitBySetPosMinutely(rule);
        }

        public static IEnumerable<DATE_TIME> GenerateHourlyRecurrences(this RECUR rule, DATE_TIME start, DATE_TIME end)
        {
            var dates = new List<DATE_TIME>();

            while (start < end) dates.Add(start = start.AddHours(rule.INTERVAL));

            return dates
                .LimitByMonth(rule)
                .LimitByYearDay(rule)
                .LimitByMonthDay(rule)
                .LimitByDay(rule)
                .LimitByHour(rule)
                .ExpandByMinute(rule)
                .ExpandBySecond(rule)
                .LimitBySetPosHourly(rule);
        }

        public static IEnumerable<DATE_TIME> GenerateDailyRecurrences(this RECUR rule, DATE_TIME start, DATE_TIME end)
        {
            var dates = new List<DATE_TIME>();
            while (start < end) dates.Add(start = start.AddDays(rule.INTERVAL));
            return dates
                .LimitByMonth(rule)
                .LimitByYearDay(rule)
                .LimitByMonthDay(rule)
                .LimitByDay(rule)
                .ExpandByHour(rule)
                .ExpandByMinute(rule)
                .ExpandBySecond(rule)
                .LimitBySetPosDaily(rule);
        }

        public static IEnumerable<DATE_TIME> GenerateWeeklyRecurrences(this RECUR rule, DATE_TIME start, DATE_TIME end)
        {
            var dates = new List<DATE_TIME>();
            while (start < end) dates.Add(start = start.AddDays(7*rule.INTERVAL));
            return dates
                .LimitByMonth(rule)
                .ExpandByDayWeekly(rule)
                .ExpandByHour(rule)
                .ExpandByMinute(rule)
                .ExpandBySecond(rule)
                .LimitBySetPosWeekly(rule);
        }

        public static IEnumerable<DATE_TIME> GenerateMonthlyRecurrences(this RECUR rule, DATE_TIME start, DATE_TIME end)
        {
            var dates = new List<DATE_TIME>();
            while (start < end) dates.Add(start = start.AddMonths((int) rule.INTERVAL));

            var results = dates.LimitByMonth(rule);

            results = rule.BYMONTHDAY.Any()
                ? results.LimitByDay(rule)
                : results.ExpandByMonth(rule);

            return results
                .ExpandByDayMonthly(rule)
                .ExpandByHour(rule)
                .ExpandByMinute(rule)
                .ExpandBySecond(rule)
                .LimitBySetPosMonthly(rule);
        }

        public static IEnumerable<DATE_TIME> GenerateYearlyRecurrences(this RECUR rule, DATE_TIME start, DATE_TIME end)
        {
            var dates = new List<DATE_TIME>();
            while (start < end) dates.Add(start = start.AddYears((int) rule.INTERVAL));

            var results = dates
                .ExpandByMonth(rule)
                .ExpandByWeekNo(rule)
                .ExpandByYearDay(rule)
                .ExpandByMonthDay(rule);

            if (rule.BYYEARDAY.Any() || rule.BYMONTHDAY.Any())
            {
                results = results.LimitByDay(rule);
            }
            else
            {
                results = rule.BYWEEKNO.Any()
                    ? results.ExpandByDayWeekly(rule)
                    : (rule.BYMONTH.Any()
                        ? results.ExpandByDayMonthly(rule)
                        : results.ExpandByDayYearly(rule));
            }

            return results
                .ExpandByHour(rule)
                .ExpandByMinute(rule)
                .ExpandBySecond(rule)
                .LimitBySetPosYearly(rule);
        }

        public static IEnumerable<DATE_TIME> GenerateRecurrences(this RECUR rule, DATE_TIME start, DATE_TIME end)
        {
            switch (rule.FREQ)
            {
                case FREQ.SECONDLY:
                    return rule.GenerateSecondlyRecurrences(start, end);

                case FREQ.MINUTELY:
                    return rule.GenerateMinutelyRecurrences(start, end);

                case FREQ.HOURLY:
                    return rule.GenerateHourlyRecurrences(start, end);

                case FREQ.DAILY:
                    return rule.GenerateDailyRecurrences(start, end);

                case FREQ.WEEKLY:
                    return rule.GenerateWeeklyRecurrences(start, end);

                case FREQ.MONTHLY:
                    return rule.GenerateMonthlyRecurrences(start, end);

                case FREQ.YEARLY:
                    return rule.GenerateYearlyRecurrences(start, end);
            }
            return Enumerable.Empty<DATE_TIME>();
        }

        public static IEnumerable<DATE_TIME> GenerateRecurrences(this RECUR rule, DATE_TIME start, uint window = 6)
        {
            IEnumerable<DATE_TIME> recurrences;

            var limit = rule.CalculateWindowLimit(start, (int) window);
            var current = start;

            if (rule.UNTIL != default(DATE_TIME)) //rule is based on UNTIL constraint
            {
                recurrences = rule.UNTIL <= limit //UNTIL lies within window period ?
                    ? rule.GenerateRecurrences(current, rule.UNTIL)
                    : rule.GenerateRecurrences(limit, limit + (rule.UNTIL - limit));
            }
            else if (rule.COUNT != 0) // rule is based on COUNT constraint
            {
                var temp = new List<DATE_TIME>();
                do
                {
                    temp.AddRange(rule.GenerateRecurrences(current, limit));
                    current = temp.Last();
                    limit = rule.CalculateWindowLimit(current, (int) window);
                } while (temp.Count < rule.COUNT);

                recurrences = temp.OrderBy(r => r).Take((int) rule.COUNT);
            }
            else //rule is neither based on UNTIL nor COUNT  => generate "forever" (bounded by limit)
            {
                recurrences = rule.GenerateRecurrences(start, limit);
            }

            return recurrences;
        }

        #endregion Generate Recurrence Dates
    }
}