using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.infrastructure.contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace reexjungle.xcal.domain.extensions
{
    /// <summary>
    ///
    /// </summary>
    public static class EventExtensions
    {
        public static List<VEVENT> GenerateOccurrences(this VEVENT vevent, IKeyGenerator<Guid> keyGenerator, uint window = 6)
        {
            if (keyGenerator == null) throw new ArgumentNullException(nameof(keyGenerator));

            var occurrences = new List<VEVENT> { vevent };
            var dates = vevent.RecurrenceRule.GenerateRecurrences(vevent.Start, window).ToList();
            if (vevent.RecurrenceDates.Any())
            {
                var recurrentDates = vevent
                    .RecurrenceDates
                    .Where(x => x.DateTimes.Any())
                    .SelectMany(x => x.DateTimes);

                var recurrentPeriods = vevent
                    .RecurrenceDates
                    .Where(x => x.Periods.Any())
                    .SelectMany(x => x.Periods);

                var recurrentDatesList = recurrentDates as IList<DATE_TIME> ?? recurrentDates.ToList();
                if (recurrentDatesList.Any())
                {
                    dates.AddRange(recurrentDatesList);
                }

                var recurrentPeriodsList = recurrentPeriods as IList<PERIOD> ?? recurrentPeriods.ToList();
                if (recurrentPeriodsList.Any())
                {
                    dates.AddRange(recurrentPeriodsList.Select(x => x.Start));
                }
            }

            if (vevent.ExceptionDates.Any())
            {
                var exdates = vevent
                    .ExceptionDates
                    .Where(x => x.DateTimes.Any())
                    .SelectMany(x => x.DateTimes);

                var exceptionDatesList = exdates as IList<DATE_TIME> ?? exdates.ToList();
                if (exceptionDatesList.Any())
                    dates = dates.Except(exceptionDatesList).ToList();
            }

            foreach (var date in dates.Except(vevent.Start.ToSingleton()))
            {
                var now = DateTime.UtcNow.AsDATE_TIME();
                var instance = new VEVENT(vevent)
                {
                    Id = keyGenerator.GetNext(),
                    Created = now,
                    Datestamp = now,
                    LastModified = now,
                    Start = date,
                    End = date + vevent.Duration,
                    RecurrenceRule = null
                };

                instance.RecurrenceId = new RECURRENCE_ID
                {
                    Id = instance.Id,
                    Range = RANGE.THISANDFUTURE,
                    TimeZoneId = date.TimeZoneId,
                    Value = instance.Start
                };
                occurrences.Add(instance);
            }

            return occurrences;
        }

        public static List<VEVENT> GetNextOccurences(this IList<VEVENT> vevents, IKeyGenerator<Guid> keyGenerator, uint window = 6)
        {
            if (vevents.NullOrEmpty()) return vevents.ToList();

            var first = vevents.First();
            var last = vevents.Last();

            var copy = new VEVENT(first)
            {
                Start = last.Start,
                End = last.End
            };

            return copy.GenerateOccurrences(keyGenerator, window);
        }
    }
}