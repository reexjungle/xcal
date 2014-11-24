using reexmonkey.foundation.essentials.concretes;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace reexmonkey.xcal.domain.extensions
{
    public static class RecurrenceEngine
    {
        #region Limit BYSETPOS


        /// <summary>
        /// Filters dates by indexed position within recurrence interval
        /// </summary>
        /// <param name="dates">The dates to filter</param>
        /// <param name="rrule">The recurrence rule</param>
        /// <returns></returns>
        public static IEnumerable<DATE_TIME> LimitBySetPos(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            if (rrule.BYSETPOS.NullOrEmpty()) return dates;

            var positives = rrule.BYSETPOS.Where(x => x > 0);
            var negatives = rrule.BYSETPOS.Where(x => x < 0);

            IEnumerable<DATE_TIME> results = new List<DATE_TIME>();

            //Group per recurring instance
            switch (rrule.FREQ)
            {
                case FREQ.SECONDLY: results = results.Union(dates.LimitBySetPosSecondly(rrule, positives, negatives)); break;
                case FREQ.MINUTELY: results = results.Union(dates.LimitBySetPosMinutely(rrule, positives, negatives)); break;
                case FREQ.HOURLY: results = results.Union(dates.LimitBySetPosHourly(rrule, positives, negatives)); break;
                case FREQ.DAILY: results = results.Union(dates.LimitBySetPosDaily(rrule, positives, negatives)); break;
                case FREQ.WEEKLY: results = results.Union(dates.LimitBySetPosWeekly(rrule, positives, negatives)); break;
                case FREQ.MONTHLY: results = results.Union(dates.LimitBySetPosMonthly(rrule, positives, negatives)); break;
                case FREQ.YEARLY: results = results.Union(dates.LimitBySetPosYearly(rrule, positives, negatives)); break;
                default:
                    return dates.OrderBy(x => x);
            }

            return results;
        }

        private static IEnumerable<DATE_TIME> LimitBySetPosSecondly(this IEnumerable<DATE_TIME> dates, RECUR rrule, IEnumerable<int> positives, IEnumerable<int> negatives)
        {
            //return dates since an interval of 1 second cannot be subdivided anymore
            return dates.OrderBy(x => x);
        }

        private static IEnumerable<DATE_TIME> LimitBySetPosMinutely(this IEnumerable<DATE_TIME> dates, RECUR rrule, IEnumerable<int> positives, IEnumerable<int> negatives)
        {
            IEnumerable<DATE_TIME> results = new List<DATE_TIME>();

            var bymins = dates.GroupBy(d => d.FULLYEAR)
                .SelectMany(d => d.GroupBy(f => f.MONTH))
                .SelectMany(d => d.GroupBy(m => m.MDAY))
                .SelectMany(d => d.GroupBy(dd => dd.HOUR))
                .SelectMany(d => d.GroupBy(hh => hh.MINUTE));

            foreach (var bymin in bymins)
            {
                var occurences = bymin.GroupBy(x => x.SECOND).SelectMany(x => x).ToArray();
                var len = occurences.Count();
                results = results.Union(positives.Where(p => p >= 1 && p <= len).Select(p => occurences[p - 1]));
                results = results.Union(negatives.Where(n => Math.Abs(n) >= 1 && Math.Abs(n) <= len).Select(n => occurences[len + n]));
            }
            return results.OrderBy(x => x);
        }

        private static IEnumerable<DATE_TIME> LimitBySetPosHourly(this IEnumerable<DATE_TIME> dates, RECUR rrule, IEnumerable<int> positives, IEnumerable<int> negatives)
        {
            IEnumerable<DATE_TIME> results = new List<DATE_TIME>();

            var byhours = dates.GroupBy(d => d.FULLYEAR)
                .SelectMany(d => d.GroupBy(f => f.MONTH))
                .SelectMany(d => d.GroupBy(m => m.MDAY))
                .SelectMany(d => d.GroupBy(dd => dd.HOUR));

            if (!rrule.BYSECOND.NullOrEmpty())
            {
                var bymins = byhours.SelectMany(hh => hh.GroupBy(x => x.MINUTE));
                foreach (var bymin in bymins)
                {
                    var occurrences = bymin.GroupBy(x => x.SECOND).SelectMany(x => x).ToArray();
                    var len = occurrences.Count();
                    results = results.Union(positives.Where(p => p >= 1 && p <= len).Select(p => occurrences[p - 1]));
                    results = results.Union(negatives.Where(n => Math.Abs(n) >= 1 && Math.Abs(n) <= len).Select(n => occurrences[len + n]));
                }
            }
            else //empty BYSECOND
            {
                if (!rrule.BYMINUTE.NullOrEmpty())
                {
                    foreach (var hh in byhours)
                    {
                        var occurrences = hh.GroupBy(x => x.MINUTE).SelectMany(x => x).ToArray();
                        var len = occurrences.Count();
                        results = results.Union(positives.Where(p => p >= 1 && p <= len).Select(p => occurrences[p - 1]));
                        results = results.Union(negatives.Where(n => Math.Abs(n) >= 1 && Math.Abs(n) <= len).Select(n => occurrences[len + n]));
                    }
                }
            }

            return results.OrderBy(x => x);
        }

        private static IEnumerable<DATE_TIME> LimitBySetPosDaily(this IEnumerable<DATE_TIME> dates, RECUR rrule, IEnumerable<int> positives, IEnumerable<int> negatives)
        {
            IEnumerable<DATE_TIME> results = new List<DATE_TIME>();

            var byyears = dates.GroupBy(x => x.FULLYEAR);
            var bymonths = byyears.SelectMany(x => x.GroupBy(m => m.MONTH));
            var bydays = bymonths.SelectMany(x => x.GroupBy(d => d.MDAY));
            if (!rrule.BYSECOND.NullOrEmpty())
            {
                var byhours = bydays.SelectMany(x => x.GroupBy(h => h.HOUR));
                var bymins = byhours.SelectMany(byhour => byhour.GroupBy(x => x.MINUTE));
                foreach (var bymin in bymins)
                {
                    var occurrences = bymin.GroupBy(x => x.SECOND).SelectMany(x => x).ToArray();
                    var len = occurrences.Count();
                    results = results.Union(positives.Where(p => p >= 1 && p <= len).Select(p => occurrences[p - 1]));
                    results = results.Union(negatives.Where(n => Math.Abs(n) >= 1 && Math.Abs(n) <= len).Select(n => occurrences[len + n]));
                }
            }
            else //empty BYSECOND
            {
                if (!rrule.BYMINUTE.NullOrEmpty())
                {
                    var byhours = bydays.SelectMany(x => x.GroupBy(h => h.HOUR));

                    foreach (var byhour in byhours)
                    {
                        var occurrences = byhour.GroupBy(x => x.MINUTE).SelectMany(x => x).ToArray();
                        var len = occurrences.Count();
                        results = results.Union(positives.Where(p => p >= 1 && p <= len).Select(p => occurrences[p - 1]));
                        results = results.Union(negatives.Where(n => Math.Abs(n) >= 1 && Math.Abs(n) <= len).Select(n => occurrences[len + n]));
                    }
                }
                else //empty BYMINUTE
                {
                    if (!rrule.BYHOUR.NullOrEmpty())
                    {
                        foreach (var byday in bydays)
                        {
                            var occurrences = byday.GroupBy(x => x.HOUR).SelectMany(x => x).ToArray();
                            var len = occurrences.Count();
                            results = results.Union(positives.Where(p => p >= 1 && p <= len).Select(p => occurrences[p - 1]));
                            results = results.Union(negatives.Where(n => Math.Abs(n) >= 1 && Math.Abs(n) <= len).Select(n => occurrences[len + n]));
                        }
                    }
                }
            }

            return results.OrderBy(x => x);
        }

        private static IEnumerable<DATE_TIME> LimitBySetPosWeekly(this IEnumerable<DATE_TIME> dates, RECUR rrule, IEnumerable<int> positives, IEnumerable<int> negatives)
        {
            IEnumerable<DATE_TIME> results = new List<DATE_TIME>();

            var byyears = dates.GroupBy(x => x.FULLYEAR);
            var bymonths = byyears.SelectMany(x => x.GroupBy(m => m.MONTH));
            //limit BYSETPOS for BYMINUTE
            if (!rrule.BYSECOND.NullOrEmpty())
            {
                var bydays = bymonths.SelectMany(x => x.GroupBy(d => d.ToDateTime().WeekOfMonth()));
                var byhours = bydays.SelectMany(x => x.GroupBy(h => h.HOUR));
                var bymins = byhours.SelectMany(byhour => byhour.GroupBy(x => x.MINUTE));
                foreach (var bymin in bymins)
                {
                    var occurrences = bymin.GroupBy(x => x.SECOND).SelectMany(x => x).ToArray(); var len = occurrences.Count();
                    results = results.Union(positives.Where(p => p >= 1 && p <= len).Select(p => occurrences[p - 1]));
                    results = results.Union(negatives.Where(n => Math.Abs(n) >= 1 && Math.Abs(n) <= len).Select(n => occurrences[len + n]));
                }
            }
            else //empty BYSECOND; check BYMINUTE; limit BYSETPOS for BYHOUR
            {
                if (!rrule.BYMINUTE.NullOrEmpty())
                {
                    var bydays = bymonths.SelectMany(x => x.GroupBy(d => d.ToDateTime().WeekOfMonth()));
                    var byhours = bydays.SelectMany(x => x.GroupBy(h => h.HOUR));
                    foreach (var byhour in byhours)
                    {
                        var occurrences = byhour.GroupBy(x => x.MINUTE).SelectMany(x => x).ToArray();
                        var len = occurrences.Count();
                        results = results.Union(positives.Where(p => p >= 1 && p <= len).Select(p => occurrences[p - 1]));
                        results = results.Union(negatives.Where(n => Math.Abs(n) >= 1 && Math.Abs(n) <= len).Select(n => occurrences[len + n]));
                    }
                }
                else //empty BYMINUTE: check BYHOUR; limit BYSETPOS for BYDAY
                {
                    if (!rrule.BYHOUR.NullOrEmpty())
                    {
                        var bydays = bymonths.SelectMany(x => x.GroupBy(d => d.ToDateTime().WeekOfMonth()));
                        foreach (var byday in bydays)
                        {
                            var occurrences = byday.GroupBy(x => x.HOUR).SelectMany(x => x).ToArray();
                            var len = occurrences.Count();
                            results = results.Union(positives.Where(p => p >= 1 && p <= len).Select(p => occurrences[p - 1]));
                            results = results.Union(negatives.Where(n => Math.Abs(n) >= 1 && Math.Abs(n) <= len).Select(n => occurrences[len + n]));
                        }
                    }
                    else //empty BYHOUR; check BYDAY;limit BYSETPOS for BYWEEK
                    {
                        if (!rrule.BYDAY.NullOrEmpty())
                        {
                            foreach (var bymonth in bymonths)
                            {
                                var occurrences = bymonth.GroupBy(x => x.ToDateTime().WeekOfMonth()).SelectMany(x => x).ToArray();
                                var len = occurrences.Count();
                                results = results.Union(positives.Where(p => p >= 1 && p <= len).Select(p => occurrences[p - 1]));
                                results = results.Union(negatives.Where(n => Math.Abs(n) >= 1 && Math.Abs(n) <= len).Select(n => occurrences[len + n]));

                            }
                        }
                    }
                }
            }
            return results.OrderBy(x => x);
        }

        private static IEnumerable<DATE_TIME> LimitBySetPosMonthly(this IEnumerable<DATE_TIME> dates, RECUR rrule, IEnumerable<int> positives, IEnumerable<int> negatives)
        {
            IEnumerable<DATE_TIME> results = new List<DATE_TIME>();

            var byyears = dates.GroupBy(x => x.FULLYEAR);
            var bymonths = byyears.SelectMany(x => x.GroupBy(m => m.MONTH));

            //limit BYSETPOS for BYMINUTE
            if (!rrule.BYSECOND.NullOrEmpty())
            {
                var bydays = bymonths.SelectMany(x => x.GroupBy(d => d.MDAY));
                var byhours = bydays.SelectMany(x => x.GroupBy(h => h.HOUR));
                var bymins = byhours.SelectMany(byhour => byhour.GroupBy(x => x.MINUTE));
                foreach (var bymin in bymins)
                {
                    var occurrences = bymin.GroupBy(x => x.SECOND).SelectMany(x => x).ToArray(); var len = occurrences.Count();
                    results = results.Union(positives.Where(p => p >= 1 && p <= len).Select(p => occurrences[p - 1]));
                    results = results.Union(negatives.Where(n => Math.Abs(n) >= 1 && Math.Abs(n) <= len).Select(n => occurrences[len + n]));
                }
            }
            else //empty BYSECOND; check BYMINUTE; limit BYSETPOS for BYHOUR
            {
                if (!rrule.BYMINUTE.NullOrEmpty())
                {
                    var bydays = bymonths.SelectMany(x => x.GroupBy(d => d.MDAY));
                    var byhours = bydays.SelectMany(x => x.GroupBy(h => h.HOUR));
                    foreach (var byhour in byhours)
                    {
                        var occurrences = byhour.GroupBy(x => x.MINUTE).SelectMany(x => x).ToArray();
                        var len = occurrences.Count();
                        results = results.Union(positives.Where(p => p >= 1 && p <= len).Select(p => occurrences[p - 1]));
                        results = results.Union(negatives.Where(n => Math.Abs(n) >= 1 && Math.Abs(n) <= len).Select(n => occurrences[len + n]));
                    }
                }
                else //empty BYMINUTE: check BYHOUR; limit BYSETPOS for BYDAY
                {
                    if (!rrule.BYHOUR.NullOrEmpty())
                    {
                        var bydays = bymonths.SelectMany(x => x.GroupBy(d => d.MDAY));
                        foreach (var byday in bydays)
                        {
                            var occurrences = byday.GroupBy(x => x.HOUR).SelectMany(x => x).ToArray();
                            var len = occurrences.Count();
                            results = results.Union(positives.Where(p => p >= 1 && p <= len).Select(p => occurrences[p - 1]));
                            results = results.Union(negatives.Where(n => Math.Abs(n) >= 1 && Math.Abs(n) <= len).Select(n => occurrences[len + n]));
                        }
                    }
                    else //empty BYHOUR; check BYDAY or BYMONTHDAY; limit BYSETPOS for BYMONTH
                    {
                        if (!rrule.BYDAY.NullOrEmpty() || !rrule.BYMONTHDAY.NullOrEmpty())
                        {
                            foreach (var bymonth in bymonths)
                            {
                                var occurrences = bymonth.GroupBy(x => x.MDAY).SelectMany(x => x).ToArray();
                                var len = occurrences.Count();
                                results = results.Union(positives.Where(p => p >= 1 && p <= len).Select(p => occurrences[p - 1]));
                                results = results.Union(negatives.Where(n => Math.Abs(n) >= 1 && Math.Abs(n) <= len).Select(n => occurrences[len + n]));

                            }
                        }
                    }
                }
            }

            return results.OrderBy(x => x);
        }

        private static IEnumerable<DATE_TIME> LimitBySetPosYearly(this IEnumerable<DATE_TIME> dates, RECUR rrule, IEnumerable<int> positives, IEnumerable<int> negatives)
        {
            IEnumerable<DATE_TIME> results = new List<DATE_TIME>();

            var byyears = dates.GroupBy(x => x.FULLYEAR);

            //limit BYSETPOS for BYMINUTE
            if (!rrule.BYSECOND.NullOrEmpty())
            {
                var bymonths = byyears.SelectMany(x => x.GroupBy(m => m.MONTH));
                var bydays = bymonths.SelectMany(x => x.GroupBy(d => d.MDAY));
                var byhours = bydays.SelectMany(x => x.GroupBy(h => h.HOUR));
                var bymins = byhours.SelectMany(byhour => byhour.GroupBy(x => x.MINUTE));
                foreach (var bymin in bymins)
                {
                    var occurrences = bymin.GroupBy(x => x.SECOND).SelectMany(x => x).ToArray(); var len = occurrences.Count();
                    results = results.Union(positives.Where(p => p >= 1 && p <= len).Select(p => occurrences[p - 1]));
                    results = results.Union(negatives.Where(n => Math.Abs(n) >= 1 && Math.Abs(n) <= len).Select(n => occurrences[len + n]));
                }
            }
            else //empty BYSECOND; check BYMINUTE; limit BYSETPOS for BYHOUR
            {
                if (!rrule.BYMINUTE.NullOrEmpty())
                {
                    var bymonths = byyears.SelectMany(x => x.GroupBy(m => m.MONTH));
                    var bydays = bymonths.SelectMany(x => x.GroupBy(d => d.MDAY));
                    var byhours = bydays.SelectMany(x => x.GroupBy(h => h.HOUR));
                    foreach (var byhour in byhours)
                    {
                        var occurrences = byhour.GroupBy(x => x.MINUTE).SelectMany(x => x).ToArray();
                        var len = occurrences.Count();
                        results = results.Union(positives.Where(p => p >= 1 && p <= len).Select(p => occurrences[p - 1]));
                        results = results.Union(negatives.Where(n => Math.Abs(n) >= 1 && Math.Abs(n) <= len).Select(n => occurrences[len + n]));
                    }
                }
                else //empty BYMINUTE: check BYHOUR; limit BYSETPOS for BYDAY
                {
                    if (!rrule.BYHOUR.NullOrEmpty())
                    {
                        var bymonths = byyears.SelectMany(x => x.GroupBy(m => m.MONTH));
                        var bydays = bymonths.SelectMany(x => x.GroupBy(d => d.MDAY));
                        foreach (var byday in bydays)
                        {
                            var occurrences = byday.GroupBy(x => x.HOUR).SelectMany(x => x).ToArray();
                            var len = occurrences.Count();
                            results = results.Union(positives.Where(p => p >= 1 && p <= len).Select(p => occurrences[p - 1]));
                            results = results.Union(negatives.Where(n => Math.Abs(n) >= 1 && Math.Abs(n) <= len).Select(n => occurrences[len + n]));
                        }
                    }
                    else //empty BYHOUR; check BYDAY or BYMONTHDAY
                    {
                        if (!rrule.BYDAY.NullOrEmpty() || !rrule.BYMONTHDAY.NullOrEmpty())
                        {
                            var bymonths = byyears.SelectMany(x => x.GroupBy(m => m.MONTH));
                            foreach (var bymonth in bymonths)
                            {
                                var occurrences = bymonth.GroupBy(x => x.MDAY).SelectMany(x => x).ToArray();
                                var len = occurrences.Count();
                                results = results.Union(positives.Where(p => p >= 1 && p <= len).Select(p => occurrences[p - 1]));
                                results = results.Union(negatives.Where(n => Math.Abs(n) >= 1 && Math.Abs(n) <= len).Select(n => occurrences[len + n]));

                            }
                        }
                        else  //empty BYDAY and empty BYMONTHDAY; check BYYEARDAY
                        {
                            if (!rrule.BYYEARDAY.NullOrEmpty())
                            {
                                foreach (var byyear in byyears)
                                {
                                    var occurrences = byyear.GroupBy(x => x.ToDateTime().DayOfYear).SelectMany(x => x).ToArray();
                                    var len = occurrences.Count();
                                    results = results.Union(positives.Where(p => p >= 1 && p <= len).Select(p => occurrences[p - 1]));
                                    results = results.Union(negatives.Where(n => Math.Abs(n) >= 1 && Math.Abs(n) <= len).Select(n => occurrences[len + n]));

                                }
                            }
                            else //empty BYYEARDAY; check BYWEEKNO
                            {
                                if (!rrule.BYWEEKNO.NullOrEmpty())
                                {
                                    foreach (var byyear in byyears)
                                    {
                                        var occurrences = byyear.GroupBy(x => x.ToDateTime().WeekOfYear()).SelectMany(x => x).ToArray();
                                        var len = occurrences.Count();
                                        results = results.Union(positives.Where(p => p >= 1 && p <= len).Select(p => occurrences[p - 1]));
                                        results = results.Union(negatives.Where(n => Math.Abs(n) >= 1 && Math.Abs(n) <= len).Select(n => occurrences[len + n]));

                                    } 
                                }
                                else //empty BYYEARDAY; check BYMONTH
                                {
                                    if(!rrule.BYMONTH.NullOrEmpty())
                                    {
                                        foreach (var byyear in byyears)
                                        {
                                            var occurrences = byyear.GroupBy(x => x.MONTH).SelectMany(x => x).ToArray();
                                            var len = occurrences.Count();
                                            results = results.Union(positives.Where(p => p >= 1 && p <= len).Select(p => occurrences[p - 1]));
                                            results = results.Union(negatives.Where(n => Math.Abs(n) >= 1 && Math.Abs(n) <= len).Select(n => occurrences[len + n]));
                                        } 
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return results.OrderBy(x => x);
        }

        #endregion Limit BYSETPOS

        #region Limit BYXXX

        public static IEnumerable<DATE_TIME> LimitBySecond(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            return !rrule.BYSECOND.NullOrEmpty() ? dates.Where(x => rrule.BYSECOND.Contains(x.SECOND)).OrderBy(x => x) : dates;
        }

        public static IEnumerable<DATE_TIME> LimitByMinute(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            return !rrule.BYMINUTE.NullOrEmpty() ? dates.Where(x => rrule.BYMINUTE.Contains(x.MINUTE)).OrderBy(x => x) : dates;
        }

        public static IEnumerable<DATE_TIME> LimitByHour(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            return !rrule.BYHOUR.NullOrEmpty() ? dates.Where(x => rrule.BYHOUR.Contains(x.HOUR)).OrderBy(x => x) : dates;
        }

        public static IEnumerable<DATE_TIME> LimitByDay(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            if (rrule.BYDAY.NullOrEmpty()) return dates;
            return rrule.BYDAY.SelectMany(x => dates.Where(d => d.ToDateTime().DayOfWeek == x.Weekday.ToDayOfWeek())).OrderBy(x => x);
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

            return results.OrderBy(x => x);
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

            return results.OrderBy(x => x);
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
            return results.OrderBy(x => x);
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

            return results.OrderBy(x => x);
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
            return results.OrderBy(x => x);
        }

        #endregion Limit BYXXX

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

        public static IEnumerable<DATE_TIME> ExpandByDayWeekly(this IEnumerable<DATE_TIME> dates, RECUR rrule)
        {
            if (rrule.BYDAY.NullOrEmpty()) return dates;

            dates = dates.Union(dates.SelectMany(date => rrule.BYDAY.SelectMany(x =>
            {
                var d = date.ToDateTime();
                return x.Weekday.ToDayOfWeek().GetSimilarDatesInRange(d, d.AddDays(7)).ToDATE_TIMEs(date.TimeZoneId);
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
                    var day = DateTime.DaysInMonth((int)date.FULLYEAR, (int)date.MONTH) + 1 + x;
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

        #endregion Expand BYXXX
    }
}