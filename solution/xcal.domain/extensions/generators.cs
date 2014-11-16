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

    public static class Generators
    {
        public static List<DATE_TIME> GenerateRecurrences(this DATE_TIME start, RECUR rrule, uint window = 6)
        {

            #region Generator function

            Func<DATE_TIME, DATE_TIME, RECUR, IEnumerable<DATE_TIME>> generate = (s, max, r) =>
            {
                var dates = new List<DATE_TIME> {s};
                switch (r.FREQ)
                {
                    case FREQ.SECONDLY:
                        while (s < max) dates.Add(new DATE_TIME(s = s.ToDateTime().AddSeconds(r.INTERVAL).ToDATE_TIME(s.TimeZoneId)));

                        dates = dates
                            .LimitByMonth(r)
                            .LimitByYearDay(r)
                            .LimitByMonthDay(r)
                            .LimitByDay(r)
                            .LimitByHour(r)
                            .LimitByMinute(r)
                            .LimitBySecond(r)
                            .LimitBySetPos(r)
                            .ToList();
                        break;

                    case FREQ.MINUTELY:
                        while (s < max) dates.Add(new DATE_TIME(s = s.ToDateTime().AddMinutes(r.INTERVAL).ToDATE_TIME(s.TimeZoneId)));

                        dates = dates
                            .LimitByMonth(r)
                            .LimitByYearDay(r)
                            .LimitByMonthDay(r)
                            .LimitByDay(r)
                            .LimitByHour(r)
                            .LimitByMinute(r)
                            .ExpandBySecond(r)
                            .LimitBySetPos(r)
                            .ToList(); break;

                    case FREQ.HOURLY:
                        while (s < max) dates.Add(new DATE_TIME(s = s.ToDateTime().AddHours(r.INTERVAL).ToDATE_TIME(s.TimeZoneId)));

                        dates = dates
                            .LimitByMonth(r)
                            .LimitByYearDay(r)
                            .LimitByMonthDay(r)
                            .LimitByDay(r)
                            .LimitByHour(r)
                            .ExpandByMinute(r)
                            .ExpandBySecond(r)
                            .LimitBySetPos(r)
                            .ToList(); break;

                    case FREQ.DAILY:
                        while (s < max) dates.Add(new DATE_TIME(s = s.ToDateTime().AddDays(r.INTERVAL).ToDATE_TIME(s.TimeZoneId)));

                        dates = dates
                            .LimitByMonth(r)
                            .LimitByYearDay(r)
                            .LimitByMonthDay(r)
                            .LimitByDay(r)
                            .ExpandByHour(r)
                            .ExpandByMinute(r)
                            .ExpandBySecond(r)
                            .LimitBySetPos(r)
                            .ToList(); break;

                    case FREQ.WEEKLY:
                        while (s < max) dates.Add(new DATE_TIME(s = s.ToDateTime().AddDays(7 * r.INTERVAL).ToDATE_TIME(s.TimeZoneId)));

                        dates = dates
                            .LimitByMonth(r)
                            .ExpandByDayWeekly(r)
                            .ExpandByHour(r)
                            .ExpandByMinute(r)
                            .ExpandBySecond(r)
                            .LimitBySetPos(r)
                            .ToList(); break;

                    case FREQ.MONTHLY:
                        while (s < max) dates.Add(new DATE_TIME(s = s.ToDateTime().AddMonths((int)r.INTERVAL).ToDATE_TIME(s.TimeZoneId)));
                        dates = dates
                            .LimitByMonth(r).ToList();

                        dates = !rrule.BYMONTHDAY.NullOrEmpty()
                            ? dates.LimitByDayMonthly(r).ToList()
                            : dates.ExpandByMonth(r).ToList();

                        dates = dates.ExpandByDayMonthly(r)
                        .ExpandByHour(r)
                        .ExpandByMinute(r)
                        .ExpandBySecond(r)
                        .LimitBySetPos(r)
                        .ToList(); break;

                    case FREQ.YEARLY:
                        while (s < max) dates.Add(new DATE_TIME(s = s.ToDateTime().AddYears((int)r.INTERVAL).ToDATE_TIME(s.TimeZoneId)));

                        dates = dates
                            .ExpandByMonth(r)
                            .ExpandByWeekNo(r)
                            .ExpandByYearDay(r)
                            .ExpandByMonthDay(r).ToList();

                        if (!rrule.BYYEARDAY.NullOrEmpty() || !rrule.BYMONTHDAY.NullOrEmpty()) dates = dates.LimitByDayYearly(r).ToList();
                        else
                        {
                            if (!rrule.BYWEEKNO.NullOrEmpty()) dates = dates.ExpandByDayWeekly(r).ToList();
                            else
                            {
                                if (!rrule.BYMONTH.NullOrEmpty()) dates = dates.ExpandByDayMonthly(r).ToList();
                                else dates = dates.ExpandByDayYearly(r).ToList();
                            }
                        }

                        dates = dates
                            .ExpandByHour(r)
                            .ExpandByMinute(r)
                            .ExpandBySecond(r)
                            .LimitBySetPos(r)
                        .ToList(); break;
                }

                return dates;
            }; 

            #endregion

            #region Limit-related functions

            Func<DATE_TIME, int, RECUR, DATE_TIME> calculate_limit = (s, w, r) =>
            {
                if (r.FREQ == FREQ.SECONDLY && r.INTERVAL < 24 * 60 * 60)
                    return s.ToDateTime().AddMinutes((int)w).ToDATE_TIME(s.TimeZoneId);
                else if (r.FREQ == FREQ.MINUTELY && r.INTERVAL < 24 * 60)
                    return s.ToDateTime().AddHours((int)w).ToDATE_TIME(s.TimeZoneId);
                else if (r.FREQ == FREQ.HOURLY && r.INTERVAL < 24)
                    return s.ToDateTime().AddDays((int)w).ToDATE_TIME(s.TimeZoneId);
                else
                    return s.ToDateTime().AddMonths((int)w).ToDATE_TIME(s.TimeZoneId);
            };
 
            #endregion

            var slimit = calculate_limit(start, (int)window, rrule);

            IEnumerable<DATE_TIME> results = new List<DATE_TIME>();
            var lstart = start;

            if (rrule.UNTIL != default(DATE_TIME))
            {
                while (lstart <= rrule.UNTIL)
                {
                    results = results.Union(generate(lstart, slimit, rrule));
                    if (!results.NullOrEmpty()) 
                        lstart = results.Last();
                    else lstart = slimit <= rrule.UNTIL
                        ? slimit 
                        : new DATE_TIME(
                            rrule.UNTIL.FULLYEAR, 
                            rrule.UNTIL.MONTH, 
                            rrule.UNTIL.MDAY + 1, 
                            rrule.UNTIL.HOUR, 
                            rrule.UNTIL.MINUTE,  
                            rrule.UNTIL.SECOND, 
                            rrule.UNTIL.Type, 
                            rrule.UNTIL.TimeZoneId);

                    slimit = calculate_limit(slimit, (int)window, rrule);
                }

                results = results.Where(x => x <= rrule.UNTIL);

            }
            else if (rrule.COUNT != 0)
            {
                while (results.Count() < rrule.COUNT)
                {
                    results = results.Union(generate(lstart, slimit, rrule)); 
                    lstart = results.Last();
                    slimit = calculate_limit(slimit, (int)window, rrule);
                }
                results = results.OrderBy(r => r).Take((int)rrule.COUNT);
            }
            else
            {
                results = results.Union(generate(lstart, slimit, rrule)); 
                if (!results.NullOrEmpty()) lstart = results.Last();
                else lstart = new DATE_TIME(
                    rrule.UNTIL.FULLYEAR,
                    rrule.UNTIL.MONTH,
                    rrule.UNTIL.MDAY + 1,
                    rrule.UNTIL.HOUR,
                    rrule.UNTIL.MINUTE,
                    rrule.UNTIL.SECOND,
                    rrule.UNTIL.Type,
                    rrule.UNTIL.TimeZoneId);
            }

            return results.Except(start.ToSingleton()).ToList();

        }

    }
}
