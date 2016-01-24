using System;
using System.Collections.Generic;
using System.Linq;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xmisc.foundation.concretes;

namespace reexjungle.xcal.domain.extensions
{
    public static class RecurrenceEngine
    {
        #region Limit BYSETPOS


        private static IEnumerable<DATE_TIME> FilterByPosition<TKey>(this IList<IGrouping<TKey, DATE_TIME>> groups, IList<int> ppositions, IList<int> npositions )
        {
            var count = groups.Count();
            
            //Find dates in group indexed by the position
            var pdates = ppositions.Where(pos => pos <= count).SelectMany(pos => groups[pos - 1]);
            var ndates = npositions.Where(pos => Math.Abs(pos) <= count).SelectMany(pos => groups[count + pos]);

            return pdates.Union(ndates);

        }

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

            var weekGroups = dates.GroupBy(date => 7 * date.MDAY).ToList();
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

            var pYearDays = rule.BYYEARDAY.Where(x => x > 0).ToList();
            var nYearDays = rule.BYYEARDAY.Where(x => x < 0).ToList();

            var results = Enumerable.Empty<DATE_TIME>();
            var groups = dates.GroupBy(x => x.FULLYEAR).ToList(); //group by year

            if (pYearDays.Any())
            {
                results = results
                    .Concat(groups
                    .SelectMany(group => group
                        .Where(x => pYearDays.Contains(x.ToDateTime().DayOfYear))));
            }

            if (nYearDays.Any())
            {
                results = results
                    .Concat(groups.SelectMany(byyear => byyear.Where(x =>
                    {
                        var max = new DateTime((int)x.FULLYEAR, 12, 31).DayOfYear;

                        var normalized = nYearDays.Select(n => (max + 1) + n); //max + 1 - N, where n = -N
                        return normalized.Contains(x.ToDateTime().DayOfYear);
                    })));
            }

            return results.OrderBy(x => x);
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

        public static IEnumerable<DATE_TIME> LimitByDay(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            return dates;
            //return rule.BYDAY.Any() 
            //    ? dates.Where(x =>
            //    {
            //        var ord = 
            //        rule.BYDAY.Contains(new WEEKDAYNUM(x.));
            //    });
        }

        public static IEnumerable<DATE_TIME> LimitByDayMonthly(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYDAY.NullOrEmpty()) return dates;

            var ranked = rule.BYDAY.Where(x => x.OrdinalWeek != 0);
            var unranked = rule.BYDAY.Where(x => x.OrdinalWeek == 0);

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

            return results.OrderBy(x => x);
        }

        public static IEnumerable<DATE_TIME> LimitByDayYearly(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYDAY.NullOrEmpty()) return dates;

            var ranked = rule.BYDAY.Where(x => x.OrdinalWeek != 0);
            var unranked = rule.BYDAY.Where(x => x.OrdinalWeek == 0);

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

            return results.OrderBy(x => x);
        }

        public static IEnumerable<DATE_TIME> LimitByMonthDay(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYMONTHDAY.NullOrEmpty()) return dates;

            var positives = rule.BYMONTHDAY.Where(x => x > 0).ToList();
            var negatives = rule.BYMONTHDAY.Where(x => x < 0).ToList();

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
            return results.OrderBy(x => x);
        }

        public static IEnumerable<DATE_TIME> LimitByWeekNo(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYWEEKNO.NullOrEmpty()) return dates;

            var positives = rule.BYWEEKNO.Where(x => x > 0).ToList();
            var negatives = rule.BYWEEKNO.Where(x => x < 0).ToList();

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
            return results.OrderBy(x => x);
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
                results.AddRange(rule.BYSECOND.Select(second => new DATE_TIME(date.FULLYEAR, date.MONTH, date.MDAY, date.HOUR, date.MINUTE, second, date.Type, date.TimeZoneId)));
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
                results.AddRange(rule.BYMINUTE.Select(minute => new DATE_TIME(date.FULLYEAR, date.MONTH, date.MDAY, date.HOUR, minute, date.SECOND, date.Type, date.TimeZoneId)));
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
                results.AddRange(rule.BYHOUR.Select(hour => new DATE_TIME(date.FULLYEAR, date.MONTH, date.MDAY, hour, date.MINUTE, date.SECOND, date.Type, date.TimeZoneId)));
            }

            return results;
        }

        public static IEnumerable<DATE_TIME> ExpandByDayWeekly(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYDAY.NullOrEmpty()) return dates;

            dates = dates.Union(dates.SelectMany(date => rule.BYDAY.SelectMany(x =>
            {
                var d = date.ToDateTime();
                return x.Weekday.ToDayOfWeek().GetSimilarDatesInRange(d, d.AddDays(7)).ToDATE_TIMEs(date.TimeZoneId);
            })));

            return dates.LimitByDay(rule);
        }

        public static IEnumerable<DATE_TIME> ExpandByDayMonthly(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYDAY.NullOrEmpty()) return dates;

            var ranked = rule.BYDAY.Where(x => x.OrdinalWeek != 0);
            var unranked = rule.BYDAY.Where(x => x.OrdinalWeek == 0);

            if (!ranked.NullOrEmpty())
            {
                dates = dates.Union(dates.SelectMany(date => ranked.Select(r =>
                    r.Weekday.ToDayOfWeek()
                    .GetNthDateOfMonth(r.OrdinalWeek, (int)date.FULLYEAR, (int)date.MONTH, (int)date.HOUR, (int)date.MINUTE, (int)date.SECOND))
                    .ToDATE_TIMEs(date.TimeZoneId)));
            }

            if (!unranked.NullOrEmpty())
            {
                dates = dates.Union(dates.SelectMany(date => unranked.SelectMany(u =>
                    u.Weekday.ToDayOfWeek()
                    .GetSimilarDatesOfMonth((int)date.FULLYEAR, (int)date.MONTH, (int)date.HOUR, (int)date.MINUTE, (int)date.SECOND).ToDATE_TIMEs(date.TimeZoneId)).ToList()));
            }

            return dates.LimitByDayMonthly(rule);
        }

        public static IEnumerable<DATE_TIME> ExpandByDayYearly(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYDAY.NullOrEmpty()) return dates;

            IEnumerable<DateTime> results = new List<DateTime>();
            var ranked = rule.BYDAY.Where(x => x.OrdinalWeek != 0);
            var unranked = rule.BYDAY.Where(x => x.OrdinalWeek == 0);

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

            return dates.LimitByDayYearly(rule);
        }

        public static IEnumerable<DATE_TIME> ExpandByMonthDay(this IEnumerable<DATE_TIME> dates, RECUR rule)
        {
            if (rule.BYMONTHDAY.NullOrEmpty()) return dates;
            var positives = rule.BYMONTHDAY.Where(x => x > 0);
            var negatives = rule.BYMONTHDAY.Where(x => x < 0);
            if (!positives.NullOrEmpty())
            {
                dates = dates.Union(dates.SelectMany(date => positives.Select(x =>
                    new DATE_TIME(date.FULLYEAR, date.MONTH, (uint)x, date.HOUR, date.MINUTE, date.SECOND, date.Type, date.TimeZoneId))));
            }

            if (!negatives.NullOrEmpty())
            {
                dates = dates.Union(dates.SelectMany(date => negatives.Select(x =>
                {
                    var day = DateTime.DaysInMonth((int)date.FULLYEAR, (int)date.MONTH) + 1 + x;
                    return new DATE_TIME(date.FULLYEAR, date.MONTH, (uint)day, date.HOUR, date.MINUTE, date.SECOND, date.Type, date.TimeZoneId);
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
                     new DateTime((int)date.FULLYEAR, 1, 1, (int)date.HOUR, (int)date.MINUTE, (int)date.SECOND).AddDays(x - 1).ToDATE_TIME(date.TimeZoneId))));
            }

            if (!negatives.NullOrEmpty())
            {
                dates = dates.Union(dates.SelectMany(date => negatives.Select(x =>
                     new DateTime((int)date.FULLYEAR, 12, 31, (int)date.HOUR, (int)date.MINUTE, (int)date.SECOND).AddDays(x + 1).ToDATE_TIME(date.TimeZoneId))));
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
                    new DateTime((int)date.FULLYEAR, 1, 1, (int)date.HOUR, (int)date.MINUTE, (int)date.SECOND).AddWeeks(x).ToDATE_TIME(date.TimeZoneId))));
            }

            if (!negatives.NullOrEmpty())
            {
                dates = dates.Union(dates.SelectMany(date => negatives.Select(x =>
                    new DateTime((int)date.FULLYEAR, 12, 31, (int)date.HOUR, (int)date.MINUTE, (int)date.SECOND).AddWeeks(x).ToDATE_TIME(date.TimeZoneId))));
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
                results.AddRange(rule.BYMONTH.Select(month => new DATE_TIME(date.FULLYEAR, month, date.MDAY, date.HOUR, date.MINUTE, date.SECOND, date.Type, date.TimeZoneId)));
            }

            return results;
        }

        #endregion Expand BYXXX

        #region Generate Recurrence Dates

        private static DATE_TIME CalculateWindowLimit(this RECUR rule, DATE_TIME start, int window)
        {
            if (rule.FREQ == FREQ.SECONDLY && rule.INTERVAL < 24 * 60 * 60)
            {
                return start.ToDateTime().AddMinutes(window).ToDATE_TIME(start.TimeZoneId);
            }
            if (rule.FREQ == FREQ.MINUTELY && rule.INTERVAL < 24 * 60)
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
            while (start < end) dates.Add(start = start.AddDays(7 * rule.INTERVAL));
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
            while (start < end) dates.Add(start = start.AddMonths((int)rule.INTERVAL));

            var results = dates.LimitByMonth(rule);

            results = rule.BYMONTHDAY.Any()
                ? results.LimitByDayMonthly(rule)
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
            while (start < end) dates.Add(start = start.AddYears((int)rule.INTERVAL));

            var results = dates
                .ExpandByMonth(rule)
                .ExpandByWeekNo(rule)
                .ExpandByYearDay(rule)
                .ExpandByMonthDay(rule);

            if (rule.BYYEARDAY.Any() || rule.BYMONTHDAY.Any())
            {
                results = results.LimitByDayYearly(rule);
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

            var limit = rule.CalculateWindowLimit(start, (int)window);
            var current = start;

            if (rule.UNTIL != default(DATE_TIME)) //rule is based on UNTIL constraint
            {
                recurrences = rule.UNTIL <= limit   //UNTIL lies within window period ?
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
                    limit = rule.CalculateWindowLimit(current, (int)window);
                } while (temp.Count < rule.COUNT);

                recurrences = temp.OrderBy(r => r).Take((int)rule.COUNT);
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