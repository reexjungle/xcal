﻿using System;
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
        public static List<Tuple<DATE_TIME, DATE_TIME>> GenerateRecurrences(this DATE_TIME start, DATE_TIME end, RECUR rrule, uint window = 6)
        {
            Func<DATE_TIME, DATE_TIME, DATE_TIME,  RECUR, IEnumerable<Tuple<DATE_TIME, DATE_TIME>>> 
                generate_and_filter = (s, e, max, r) =>
                {
                    var generated = new List<Tuple<DATE_TIME, DATE_TIME>>();
                    IEnumerable<Tuple<DATE_TIME, DATE_TIME>> filtered = null;

                    if (r.FREQ == FREQ.SECONDLY)
                    {
                        while (s < max)
                            generated.Add(new Tuple<DATE_TIME, DATE_TIME>(s = s.ToDateTime().AddSeconds(r.INTERVAL).ToDATE_TIME(s.TimeZoneId),
                                e = e.ToDateTime().AddSeconds(r.INTERVAL).ToDATE_TIME(e.TimeZoneId)));
                        filtered = generated
                            .FilterByMonth(r)
                            .FilterByYearDay(r)
                            .FilterByMonthDay(r)
                            .FilterByDay(r)
                            .FilterByHour(r)
                            .FilterByMinute(r)
                            .FilterBySecond(r);
                    }
                    else if (r.FREQ == FREQ.MINUTELY)
                    {
                        while (s < max)
                            generated.Add(new Tuple<DATE_TIME, DATE_TIME>(s = s.ToDateTime().AddMinutes(r.INTERVAL).ToDATE_TIME(s.TimeZoneId),
                                e = e.ToDateTime().AddMinutes(r.INTERVAL).ToDATE_TIME(e.TimeZoneId)));
                        filtered = generated
                            .FilterByMonth(r)
                            .FilterByYearDay(r)
                            .FilterByMonthDay(r)
                            .FilterByDay(r)
                            .FilterByHour(r)
                            .FilterByMinute(r)
                            .FilterBySecond(r);
                    }
                    else if (r.FREQ == FREQ.HOURLY)
                    {
                        while (s < max)
                            generated.Add(new Tuple<DATE_TIME, DATE_TIME>(s = s.ToDateTime().AddHours(r.INTERVAL).ToDATE_TIME(s.TimeZoneId),
                                e = e.ToDateTime().AddHours(r.INTERVAL).ToDATE_TIME(e.TimeZoneId)));
                        filtered = generated
                            .FilterByMonth(r)
                            .FilterByYearDay(r)
                            .FilterByMonthDay(r)
                            .FilterByDay(r)
                            .FilterByHour(r)
                            .FilterByMinute(r)
                            .FilterBySecond(r);
                    }
                    else if (r.FREQ == FREQ.DAILY)
                    {
                        while (s < max)
                            generated.Add(new Tuple<DATE_TIME, DATE_TIME>(s = s.ToDateTime().AddDays(r.INTERVAL).ToDATE_TIME(s.TimeZoneId),
                                e = e.ToDateTime().AddDays(r.INTERVAL).ToDATE_TIME(e.TimeZoneId)));
                        filtered = generated
                            .FilterByMonth(r)
                            .FilterByMonthDay(r)
                            .FilterByDayMonthly(r)
                            .FilterByHour(r)
                            .FilterByMinute(r)
                            .FilterBySecond(r)
                            .FilterBySetPosDaily(r);
                    }
                    else if (r.FREQ == FREQ.WEEKLY)
                    {
                        while (s < max)
                            generated.Add(new Tuple<DATE_TIME, DATE_TIME>(s = s.ToDateTime().AddDays(7 * r.INTERVAL).ToDATE_TIME(s.TimeZoneId),
                                e = e.ToDateTime().AddDays(7 * r.INTERVAL).ToDATE_TIME(e.TimeZoneId)));
                        filtered = generated
                            .FilterByMonth(r)
                            .FilterByDay(r)
                            .FilterByHour(r)
                            .FilterByMinute(r)
                            .FilterBySecond(r)
                            .FilterBySetPosWeekly(r);
                    }
                    else if (r.FREQ == FREQ.MONTHLY)
                    {
                        while (s < max)
                            generated.Add(new Tuple<DATE_TIME, DATE_TIME>(s = s.ToDateTime().AddMonths((int)r.INTERVAL).ToDATE_TIME(s.TimeZoneId),
                                e = e.ToDateTime().AddMonths((int)r.INTERVAL).ToDATE_TIME(e.TimeZoneId)));
                        filtered = generated
                            .FilterByMonth(r)
                            .FilterByMonthDay(r)
                            .FilterByDayMonthly(r)
                            .FilterByHour(r)
                            .FilterByMinute(r)
                            .FilterBySecond(r)
                            .FilterBySetPosMonthly(r);
                    }
                    else if (r.FREQ == FREQ.YEARLY)
                    {
                        while (s < max)
                            generated.Add(new Tuple<DATE_TIME, DATE_TIME>(s = s.ToDateTime().AddYears((int)r.INTERVAL).ToDATE_TIME(s.TimeZoneId),
                                e = e.ToDateTime().AddYears((int)r.INTERVAL).ToDATE_TIME(e.TimeZoneId)));
                        filtered = generated
                            .FilterByMonth(r)
                            .FilterByMonthDay(r)
                            .FilterByDayYearly(r)
                            .FilterByHour(r)
                            .FilterByMinute(r)
                            .FilterBySecond(r)
                            .FilterBySetPosYearly(r);
                    }
                
                    return filtered;
                };

            var slimit = start.ToDateTime().AddMonths((int)window).ToDATE_TIME(start.TimeZoneId);
            var results = new List<Tuple<DATE_TIME, DATE_TIME>>();

            var lstart = start;
            var lend = end;

            if (rrule.UNTIL != default(DATE_TIME))
            {
                while (lstart <= rrule.UNTIL)
                {
                    results.AddRange(generate_and_filter(lstart, lend, slimit, rrule));
                    if (!results.NullOrEmpty())
                    {
                        lstart = results.Last().Item1;
                        lend = results.Last().Item2;
                    }
                    else lstart = new DATE_TIME(
                        rrule.UNTIL.FULLYEAR, 
                        rrule.UNTIL.MONTH, 
                        rrule.UNTIL.MDAY + 1, 
                        rrule.UNTIL.HOUR, 
                        rrule.UNTIL.MINUTE,  
                        rrule.UNTIL.SECOND );
                    
                    slimit = slimit.ToDateTime().AddMonths((int)window).ToDATE_TIME();
                }

                results = results.OrderBy(r => r.Item1).TakeWhile(x => x.Item1 <= rrule.UNTIL).ToList();

            }
            else if (rrule.COUNT != 0)
            {
                while (results.Count() < rrule.COUNT)
                {
                    results.AddRange(generate_and_filter(lstart, lend, slimit, rrule));
                    lstart = results.Last().Item1;
                    lend = results.Last().Item2;
                    slimit = slimit.ToDateTime().AddMonths((int)window).ToDATE_TIME();
                }

                results = results.OrderBy(r => r.Item1).Take((int)rrule.COUNT).ToList();
            }
            else
            {
                results.AddRange(generate_and_filter(lstart, lend, slimit, rrule));
                if (!results.NullOrEmpty())
                {
                    lstart = results.Last().Item1;
                    lend = results.Last().Item2;
                }
                else lstart = new DATE_TIME(
                    rrule.UNTIL.FULLYEAR,
                    rrule.UNTIL.MONTH,
                    rrule.UNTIL.MDAY + 1,
                    rrule.UNTIL.HOUR,
                    rrule.UNTIL.MINUTE,
                    rrule.UNTIL.SECOND);

                results = results.OrderBy(r => r.Item1).ToList();
            }

            return results;

        }

    }
}
