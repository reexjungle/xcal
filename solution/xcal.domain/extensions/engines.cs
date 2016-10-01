using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xmisc.foundation.concretes;
using ServiceStack.Common;
using System;
using System.Collections.Generic;
using System.Linq;

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

        private static DATE_TIME EquivalentWeekDate(this WEEKDAYNUM weekdaynum, DATE_TIME reference) => reference.AddDays((int)weekdaynum.Weekday - (int)reference.GetWeekday());

        private static DATE_TIME EquivalentMonthDate(this WEEKDAYNUM weekdaynun, DATE_TIME reference) => weekdaynun.Weekday
    .AsDayOfWeek()
    .GetNthDateOfMonth(weekdaynun.NthOccurrence, (int)reference.FULLYEAR, (int)reference.MONTH,
        (int)reference.HOUR, (int)reference.MINUTE, (int)reference.SECOND)
    .AsDATE_TIME(reference.TimeZoneId);

        private static DATE_TIME EquivalentYearDate(this WEEKDAYNUM weekdaynun, DATE_TIME reference) => weekdaynun.Weekday
    .AsDayOfWeek()
    .GetNthDateOfYear(weekdaynun.NthOccurrence, (int)reference.FULLYEAR).AsDATE_TIME(reference.TimeZoneId);

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
                        var dateTime = date.AsDateTime();
                        return new WEEKDAYNUM(dateTime.Day, dateTime.DayOfWeek.AsWEEKDAY());
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

            var weekGroups = dates.GroupBy(date => date.AsDateTime().WeekOfYear()).ToList();
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
                        var dateTime = date.AsDateTime();
                        return new WEEKDAYNUM(dateTime.Day, dateTime.DayOfWeek.AsWEEKDAY());
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
                        var dateTime = date.AsDateTime();
                        return new WEEKDAYNUM(dateTime.Day, dateTime.DayOfWeek.AsWEEKDAY());
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
                        var dateTime = date.AsDateTime();
                        return new WEEKDAYNUM(dateTime.DayOfYear, dateTime.DayOfWeek.AsWEEKDAY());
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
                    .Select(yearGroup => yearGroup.GroupBy(date => date.AsDateTime().DayOfYear).ToList())
                    .Select(dayGroup => dayGroup.FilterByPosition(ppositions, npositions))
                    .Aggregate(results, (current, temp) => current.Union(temp));
            }

            if (rule.BYWEEKNO.Any())
            {
                results = yearGroups
                    .Select(yearGroup => yearGroup.GroupBy(date => date.AsDateTime().WeekOfYear()).ToList())
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

        public static IEnumerable<DATE_TIME> LimitByMonth(this IEnumerable<DATE_TIME> dates, RECUR rule) => rule.BYMONTH.Any()
    ? dates.Where(x => rule.BYMONTH.Contains(x.MONTH))
    : dates;

        public static IEnumerable<DATE_TIME> LimitByYearDay(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYYEARDAY.Empty()) return dates;

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
                    .Union(byyear.Where(date => pdays.Contains(date.AsDateTime().DayOfYear))
                        .Select(date => date));

                ndates = ndates
                    .Union(byyear.Where(date => ndays.Contains(date.AsDateTime().DayOfYear))
                        .Select(date => date));
            }

            return pdates.Union(ndates);
        }

        public static IEnumerable<DATE_TIME> LimitByDay(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYDAY.Empty()) return dates;

            if (rule.FREQ == FREQ.MONTHLY)
            {
                var weekdaynums = rule.BYDAY.Where(x => x.NthOccurrence != 0).ToList();
                var nweekdaynums = rule.BYDAY.Where(x => x.NthOccurrence == 0).ToList();
                return dates.LimitByDayMonthly(rule, weekdaynums, nweekdaynums);
            }

            if (rule.FREQ == FREQ.YEARLY)
            {
                var weekdaynums = rule.BYDAY.Where(x => x.NthOccurrence != 0).ToList();
                var nweekdaynums = rule.BYDAY.Where(x => x.NthOccurrence == 0).ToList();
                return dates.LimitByDayYearly(rule, weekdaynums, nweekdaynums);
            }

            //In case it is neither the monthly nor the yearly rule, then limit by the specified frequency.
            return rule.BYDAY.Any()
                ? rule.BYDAY.SelectMany(
                    byday => dates.Where(date => date.AsDateTime().DayOfWeek.AsWEEKDAY() == byday.Weekday))
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
                    .SelectMany(
                        nweekdaynum =>
                            dates.Where(
                                date =>
                                    date.AsDateTime()
                                        .IsNthWeekdayOfMonth(nweekdaynum.Weekday.AsDayOfWeek(),
                                            nweekdaynum.NthOccurrence)));
            }

            if (weekdaynums.Any())
                nresults =
                    weekdaynums.SelectMany(
                        byday => dates.Where(date => date.AsDateTime().DayOfWeek.AsWEEKDAY() == byday.Weekday));

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
                        .SelectMany(
                            nweekdaynum =>
                                dates.Where(
                                    date =>
                                        date.AsDateTime()
                                            .IsNthWeekdayOfYear(nweekdaynum.Weekday.AsDayOfWeek(),
                                                nweekdaynum.NthOccurrence)));
                }
            }

            if (weekdaynums.Any())
                nresults =
                    weekdaynums.SelectMany(
                        byday => dates.Where(date => date.AsDateTime().DayOfWeek.AsWEEKDAY() == byday.Weekday));

            return results.Union(nresults);
        }

        public static IEnumerable<DATE_TIME> LimitByMonthDay(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYMONTHDAY.Empty()) return dates;

            var pmonthdays = rule.BYMONTHDAY.Where(x => x > 0).ToList();
            var nmonthdays = rule.BYMONTHDAY.Where(x => x < 0).ToList();

            var presults = Enumerable.Empty<DATE_TIME>();
            var nresults = Enumerable.Empty<DATE_TIME>();

            var bymonths = dates.GroupBy(x => x.MONTH).ToList(); // group by months

            if (pmonthdays.Any())
                presults = bymonths.SelectMany(bymonth => bymonth.Where(x => pmonthdays.Contains((int)x.MDAY)));

            if (!nmonthdays.NullOrEmpty())
            {
                nresults = bymonths.SelectMany(bymonth => bymonth.Where(x =>
                {
                    var max = DateTime.DaysInMonth((int)x.FULLYEAR, (int)x.MONTH);
                    var normalized = nmonthdays.Select(n => (max + 1) + n); //max + 1 - N
                    return normalized.Contains((int)x.MDAY);
                }));
            }
            return presults.Union(nresults);
        }

        public static IEnumerable<DATE_TIME> LimitBySecond(this IEnumerable<DATE_TIME> dates, RECUR rule) => rule.BYSECOND.Any()
    ? dates.Where(x => rule.BYSECOND.Contains(x.SECOND))
    : dates;

        public static IEnumerable<DATE_TIME> LimitByMinute(this IEnumerable<DATE_TIME> dates, RECUR rule) => rule.BYMINUTE.Any()
    ? dates.Where(x => rule.BYMINUTE.Contains(x.MINUTE))
    : dates;

        public static IEnumerable<DATE_TIME> LimitByHour(this IEnumerable<DATE_TIME> dates, RECUR rule) => rule.BYHOUR.Any()
    ? dates.Where(x => rule.BYHOUR.Contains(x.HOUR))
    : dates;

        public static IEnumerable<DATE_TIME> LimitByWeekNo(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYWEEKNO.Empty()) return dates;

            var pweeknos = rule.BYWEEKNO.Where(x => x > 0).ToList();
            var nweeknos = rule.BYWEEKNO.Where(x => x < 0).ToList();

            var presults = Enumerable.Empty<DATE_TIME>();
            var nresults = Enumerable.Empty<DATE_TIME>();

            var byweeks = dates.GroupBy(date => date.AsDateTime().WeekOfYear()).ToList(); // group by week of years

            if (pweeknos.Any())
            {
                presults = byweeks
                    .SelectMany(byweek => byweek.Where(date => pweeknos.Contains(date.AsDateTime().WeekOfYear())));
            }

            if (!nweeknos.NullOrEmpty())
            {
                nresults = byweeks.SelectMany(byweek => byweek.Where(date =>
                {
                    var max = new DateTime((int)date.FULLYEAR, 12, 31).WeekOfYear();
                    var normalized = nweeknos.Select(n => (max + 1) + n); //max + 1 - N
                    return normalized.Contains(date.AsDateTime().WeekOfYear());
                }));
            }
            return presults.Union(nresults);
        }

        #endregion Limit BYXXX

        #region Expand BYXXX

        public static IEnumerable<DATE_TIME> ExpandBySecond(this IEnumerable<DATE_TIME> dates, RECUR rule) => rule.BYSECOND.Empty()
    ? dates
    : dates
        .SelectMany(date => rule
            .BYSECOND.Select(
                second =>
                    new DATE_TIME(date.FULLYEAR, date.MONTH, date.MDAY, date.HOUR, date.MINUTE, second,
                        date.Type, date.TimeZoneId)));

        public static IEnumerable<DATE_TIME> ExpandByMinute(this IEnumerable<DATE_TIME> dates, RECUR rule) => rule.BYMINUTE.Empty()
    ? dates
    : dates
        .SelectMany(date => rule
            .BYMINUTE.Select(
                minute =>
                    new DATE_TIME(date.FULLYEAR, date.MONTH, date.MDAY, date.HOUR, minute, date.SECOND,
                        date.Type, date.TimeZoneId)));

        public static IEnumerable<DATE_TIME> ExpandByHour(this IEnumerable<DATE_TIME> dates, RECUR rule) => rule.BYHOUR.Empty()
    ? dates
    : dates
        .SelectMany(date => rule
            .BYHOUR.Select(
                hour =>
                    new DATE_TIME(date.FULLYEAR, date.MONTH, date.MDAY, hour, date.MINUTE, date.SECOND,
                        date.Type, date.TimeZoneId)));

        public static IEnumerable<DATE_TIME> ExpandByDay(this IEnumerable<DATE_TIME> dates, RECUR rule) => rule.BYDAY.Empty()
    ? dates
    : dates.SelectMany(date => rule
        .BYDAY
        .Select(byday => byday.EquivalentWeekDate(date)));

        public static IEnumerable<DATE_TIME> ExpandByDayMonthly(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            var weekdaynums = rule.BYDAY.Where(x => x.NthOccurrence != 0).ToList();
            var nweekdaynums = rule.BYDAY.Where(x => x.NthOccurrence == 0).ToList();

            //if weekdays have ordinal position within month
            var presults = dates.SelectMany(date => nweekdaynums.Select(byday => byday.EquivalentMonthDate(date)));

            //if weekdays do not have ordinal position within month
            var nresults = dates.SelectMany(date => weekdaynums.Select(byday => byday.EquivalentMonthDate(date)));

            return presults.Union(nresults);
        }

        public static IEnumerable<DATE_TIME> ExpandByDayYearly(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            var weekdaynums = rule.BYDAY.Where(x => x.NthOccurrence != 0).ToList();
            var nweekdaynums = rule.BYDAY.Where(x => x.NthOccurrence == 0).ToList();

            //if weekdays have ordinal position within month
            var presults = dates.SelectMany(date => nweekdaynums.Select(byday => byday.EquivalentYearDate(date)));

            //if weekdays do not have ordinal position within month
            var nresults = dates.SelectMany(date => weekdaynums.Select(byday => byday.EquivalentYearDate(date)));

            return presults.Union(nresults);
        }

        public static IEnumerable<DATE_TIME> ExpandByMonthDay(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYMONTHDAY.Empty()) return dates;

            var results = new List<DATE_TIME>();

            var pmonthdays = rule.BYMONTHDAY.Where(x => x > 0).ToList();
            var nmonthdays = rule.BYMONTHDAY.Where(x => x < 0).ToList();

            var presults = dates.SelectMany(date => pmonthdays
                .Select(
                    pmonthday =>
                        new DATE_TIME(date.FULLYEAR, date.MONTH, (uint)pmonthday, date.HOUR, date.MINUTE,
                            date.SECOND)));

            var nresults = dates.SelectMany(date => nmonthdays
                .Select(nmonthday =>
                {
                    var adjusted = DateTime.DaysInMonth((int)date.FULLYEAR, (int)date.MONTH) + nmonthday;
                    return new DATE_TIME(date.FULLYEAR, date.MONTH, (uint)adjusted, date.HOUR, date.MINUTE,
                        date.SECOND);
                }));

            return presults.Union(nresults);
        }

        public static IEnumerable<DATE_TIME> ExpandByYearDay(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYYEARDAY.Empty()) return dates;

            var results = new List<DATE_TIME>();

            var pyeardays = rule.BYYEARDAY.Where(x => x > 0).ToList();
            var nyeardays = rule.BYYEARDAY.Where(x => x < 0).ToList();

            var presults = dates.SelectMany(date => pyeardays
                .Select(
                    pyearday =>
                        new DATE_TIME(date.FULLYEAR, 1, 1, date.HOUR, date.MINUTE, date.SECOND).AddDays(
                            pyearday - 1)));

            var nresults = dates.SelectMany(date => nyeardays
                .Select(
                    nyearday =>
                        new DATE_TIME(date.FULLYEAR, 12, 31, date.HOUR, date.MINUTE, date.SECOND).AddDays(
                            nyearday + 1)));

            return presults.Union(nresults);
        }

        public static IEnumerable<DATE_TIME> ExpandByWeekNo(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYWEEKNO.NullOrEmpty()) return dates;

            var pweeknos = rule.BYWEEKNO.Where(x => x > 0).ToList();
            var nweeknos = rule.BYWEEKNO.Where(x => x < 0).ToList();

            var presults = dates.SelectMany(date => pweeknos
                .Select(
                    pweekno =>
                        new DATE_TIME(date.FULLYEAR, 1, 1, date.HOUR, date.MINUTE, date.SECOND).AddWeeks(
                            pweekno - 1)));

            var nresults = dates.SelectMany(date => nweeknos
                .Select(
                    nweekno =>
                        new DATE_TIME(date.FULLYEAR, 12, 31, date.HOUR, date.MINUTE, date.SECOND).AddWeeks(
                            nweekno + 1)));

            return presults.Union(nresults);
        }

        public static IEnumerable<DATE_TIME> ExpandByMonth(this IEnumerable<DATE_TIME> dates, RECUR rule) => rule.BYMONTH.Empty()
    ? dates
    : dates.SelectMany(date => rule.BYMONTH.Select(
        month =>
            new DATE_TIME(date.FULLYEAR, month, date.MDAY, date.HOUR, date.MINUTE, date.SECOND,
                date.Type, date.TimeZoneId)));

        #endregion Expand BYXXX

        #region Generate Recurrence Dates

        private static DATE_TIME CalculateWindowLimit(this RECUR rule, DATE_TIME start, int window)
        {
            if (rule.FREQ == FREQ.SECONDLY && rule.INTERVAL < 24 * 60 * 60)
            {
                return start.AsDateTime().AddMinutes(window).AsDATE_TIME(start.TimeZoneId);
            }
            if (rule.FREQ == FREQ.MINUTELY && rule.INTERVAL < 24 * 60)
            {
                return start.AsDateTime().AddHours(window).AsDATE_TIME(start.TimeZoneId);
            }
            if (rule.FREQ == FREQ.HOURLY && rule.INTERVAL < 24)
            {
                return start.AsDateTime().AddDays(window).AsDATE_TIME(start.TimeZoneId);
            }

            if (rule.FREQ == FREQ.YEARLY)
            {
                return start.AsDateTime().AddYears(window).AsDATE_TIME(start.TimeZoneId);
            }

            //Default window period defined in months
            return start.AsDateTime().AddMonths(window).AsDATE_TIME(start.TimeZoneId);
        }

        public static IEnumerable<DATE_TIME> GenerateSecondlyRecurrences(this RECUR rule, DATE_TIME start, DATE_TIME end)
        {
            var dates = new List<DATE_TIME> { start };
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
            var dates = new List<DATE_TIME> { start };
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
            var dates = new List<DATE_TIME> { start };

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
            var dates = new List<DATE_TIME> { start };
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
            var dates = new List<DATE_TIME> { start };
            while (start < end) dates.Add(start = start.AddDays(7 * rule.INTERVAL));
            return dates
                .LimitByMonth(rule)
                .ExpandByDay(rule)
                .ExpandByHour(rule)
                .ExpandByMinute(rule)
                .ExpandBySecond(rule)
                .LimitBySetPosWeekly(rule);
        }

        public static IEnumerable<DATE_TIME> GenerateMonthlyRecurrences(this RECUR rule, DATE_TIME start, DATE_TIME end)
        {
            var dates = new List<DATE_TIME> { start };
            while (start < end) dates.Add(start = start.AddMonths((int)rule.INTERVAL));

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
            var dates = new List<DATE_TIME> { start };
            while (start < end) dates.Add(start = start.AddYears((int)rule.INTERVAL));

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
                    ? results.ExpandByDay(rule)
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

            var limit = rule.CalculateWindowLimit(start, (int)window);
            var current = start;

            if (rule.UNTIL != default(DATE_TIME)) //rule is based on UNTIL constraint
            {
                var temp = rule.UNTIL <= limit //UNTIL lies within window period ?
                    ? rule.GenerateRecurrences(current, rule.UNTIL)
                    : rule.GenerateRecurrences(limit, limit + (rule.UNTIL - limit));

                recurrences = temp.TakeWhile(date => date < rule.UNTIL);
            }
            else if (rule.COUNT != 0) // rule is based on COUNT constraint
            {
                var temp = new List<DATE_TIME>();
                do
                {
                    temp.AddRange(rule.GenerateRecurrences(current, limit));
                    current = temp.Last();
                    limit = rule.CalculateWindowLimit(current, (int)window);
                } while (temp.Count < rule.COUNT);

                recurrences = temp.Take((int)rule.COUNT);
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