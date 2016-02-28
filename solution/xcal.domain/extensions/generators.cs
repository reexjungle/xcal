using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xmisc.foundation.concretes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace reexjungle.xcal.domain.extensions
{
    public static class Generators
    {
        private static List<DATE_TIME> GenerateDates(this RECUR rule, DATE_TIME start, DATE_TIME end)
        {
            var dates = new List<DATE_TIME> { start };
            switch (rule.FREQ)
            {
                case FREQ.SECONDLY:
                    {
                        while (start < end) dates.Add(start = start.AddSeconds(rule.INTERVAL));

                        dates = dates
                            .LimitByMonth(rule)
                            .LimitByYearDay(rule)
                            .LimitByMonthDay(rule)
                            .LimitByDay(rule)
                            .LimitByHour(rule)
                            .LimitByMinute(rule)
                            .LimitBySecond(rule)
                            .LimitBySetPos(rule).ToList();
                        break;
                    }

                case FREQ.MINUTELY:
                    {
                        while (start < end) dates.Add(start = start.AddMinutes(rule.INTERVAL));

                        dates
                            .LimitByMonth(rule)
                            .LimitByYearDay(rule)
                            .LimitByMonthDay(rule)
                            .LimitByDay(rule)
                            .LimitByHour(rule)
                            .LimitByMinute(rule)
                            .ExpandBySecond(rule)
                            .LimitBySetPos(rule);
                        break;
                    }
                case FREQ.HOURLY:
                    {
                        while (start < end) dates.Add(start = start.AddHours(rule.INTERVAL));

                        dates
                            .LimitByMonth(rule)
                            .LimitByYearDay(rule)
                            .LimitByMonthDay(rule)
                            .LimitByDay(rule)
                            .LimitByHour(rule)
                            .ExpandByMinute(rule)
                            .ExpandBySecond(rule)
                            .LimitBySetPos(rule);
                        break;
                    }

                case FREQ.DAILY:
                    {
                        while (start < end) dates.Add(start = start.AddDays(rule.INTERVAL));
                       dates = dates
                            .LimitByMonth(rule)
                            .LimitByYearDay(rule)
                            .LimitByMonthDay(rule)
                            .LimitByDay(rule)
                            .ExpandByHour(rule)
                            .ExpandByMinute(rule)
                            .ExpandBySecond(rule)
                            .LimitBySetPos(rule)
                            .ToList();
                        break;
                    }

                case FREQ.WEEKLY:
                    {
                        while (start < end) dates.Add(start = start.AddDays(7 * rule.INTERVAL));
                        dates
                            .LimitByMonth(rule)
                            .ExpandByDayWeekly(rule)
                            .ExpandByHour(rule)
                            .ExpandByMinute(rule)
                            .ExpandBySecond(rule)
                            .LimitBySetPos(rule)
                            ;
                        break;
                    }

                case FREQ.MONTHLY:
                    {
                        while (start < end) dates.Add(start = start.AddMonths((int)rule.INTERVAL));
                        dates.LimitByMonth(rule);

                        if (rule.BYMONTHDAY.Any())
                        {
                            dates.LimitByDayMonthly(rule);
                        }
                        else
                        {
                            dates.ExpandByMonth(rule);
                        }

                        dates.ExpandByDayMonthly(rule)
                            .ExpandByHour(rule)
                            .ExpandByMinute(rule)
                            .ExpandBySecond(rule)
                            .LimitBySetPos(rule);
                        break;
                    }
                case FREQ.YEARLY:
                    {
                        while (start < end) dates.Add(start = start.AddYears((int)rule.INTERVAL));

                        dates = dates
                            .ExpandByMonth(rule)
                            .ExpandByWeekNo(rule)
                            .ExpandByYearDay(rule)
                            .ExpandByMonthDay(rule).ToList();

                        if (rule.BYYEARDAY.Any() || rule.BYMONTHDAY.Any())
                        {
                            dates.LimitByDayYearly(rule);
                        }
                        else
                        {
                            if (rule.BYWEEKNO.Any())
                            {
                                dates.ExpandByDayWeekly(rule);
                            }
                            else
                            {
                                if (rule.BYMONTH.Any())
                                    dates.ExpandByDayMonthly(rule);
                                else
                                    dates.ExpandByDayYearly(rule);
                            }
                        }

                        dates
                            .ExpandByHour(rule)
                            .ExpandByMinute(rule)
                            .ExpandBySecond(rule)
                            .LimitBySetPos(rule);
                        break;
                    }
            }

            return dates;
        }

        private static DATE_TIME DetermineDateLimit(this RECUR rule, DATE_TIME start, int window)
        {
            if (rule.FREQ == FREQ.SECONDLY && rule.INTERVAL < 24 * 60 * 60)
                return start.ToDateTime().AddMinutes(window).ToDATE_TIME(start.TimeZoneId);
            if (rule.FREQ == FREQ.MINUTELY && rule.INTERVAL < 24 * 60)
                return start.ToDateTime().AddHours(window).ToDATE_TIME(start.TimeZoneId);
            if (rule.FREQ == FREQ.HOURLY && rule.INTERVAL < 24)
                return start.ToDateTime().AddDays(window).ToDATE_TIME(start.TimeZoneId);
            return start.ToDateTime().AddMonths(window).ToDATE_TIME(start.TimeZoneId);
        }

        public static List<DATE_TIME> GenerateRecurrentDates(this RECUR rule, DATE_TIME start, uint window = 6)
        {
            var limit = rule.DetermineDateLimit(start, (int)window);

            IEnumerable<DATE_TIME> results = Enumerable.Empty<DATE_TIME>();

            var current = start;
            if (rule.UNTIL != default(DATE_TIME))
            {
                while (current <= rule.UNTIL)
                {
                    results = results.Concat(rule.GenerateDates(current, limit));
                    if (results.Any())
                    {
                        current = results.Last();
                    }
                    else 
                    {
                        current = limit <= rule.UNTIL
                        ? limit
                        : new DATE_TIME(
                            rule.UNTIL.FULLYEAR,
                            rule.UNTIL.MONTH,
                            rule.UNTIL.MDAY + 1,
                            rule.UNTIL.HOUR,
                            rule.UNTIL.MINUTE,
                            rule.UNTIL.SECOND,
                            rule.UNTIL.Type,
                            rule.UNTIL.TimeZoneId);
                    }

                    limit = rule.DetermineDateLimit(current, (int)window);
                }

                results = results.Where(x => x <= rule.UNTIL);
            }
            else if (rule.COUNT != 0)
            {
                while (results.Count() < rule.COUNT)
                {
                    results = results.Concat(rule.GenerateDates(current, limit));
                    current = results.Last();
                    limit = rule.DetermineDateLimit(current, (int)window);
                }
                results = results.OrderBy(r => r).Take((int)rule.COUNT);
            }
            else
            {
                results = results.Concat(rule.GenerateDates(current, limit)); ;
                if (!results.NullOrEmpty()) current = results.Last();
                else current = new DATE_TIME(
                    rule.UNTIL.FULLYEAR,
                    rule.UNTIL.MONTH,
                    rule.UNTIL.MDAY + 1,
                    rule.UNTIL.HOUR,
                    rule.UNTIL.MINUTE,
                    rule.UNTIL.SECOND,
                    rule.UNTIL.Type,
                    rule.UNTIL.TimeZoneId);
            }

            return results.Except(start.ToSingleton()).ToList();
        }
    }
}