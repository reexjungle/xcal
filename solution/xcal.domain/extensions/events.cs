using System;
using System.Collections.Generic;
using System.Linq;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.infrastructure.contracts;

namespace reexjungle.xcal.domain.extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class EventExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <param name="keyGenerator"></param>
        /// <returns></returns>
        public static List<VEVENT> GenerateRecurrences(this VEVENT @event, IKeyGenerator<Guid> keyGenerator)
        {
            if (keyGenerator == null) throw new ArgumentNullException("keyGenerator");

            var recurs = new List<VEVENT>();
            var dates = @event.RecurrenceRule.GenerateRecurrentDates(@event.Start);
            if (@event.RecurrenceDates.Any())
            {
                var recurrentDates = @event
                    .RecurrenceDates
                    .Where(x => x.DateTimes.Any())
                    .SelectMany(x => x.DateTimes);

                var recurrentPeriods = @event
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

            if (@event.ExceptionDates.Any())
            {
                var exdates = @event
                    .ExceptionDates
                    .Where(x => x.DateTimes.Any())
                    .SelectMany(x => x.DateTimes);

                var exceptionDatesList = exdates as IList<DATE_TIME> ?? exdates.ToList();
                if (exceptionDatesList.Any()) 
                    dates = dates.Except(exceptionDatesList).ToList();
            }

            foreach (var recurrence in dates)
            {
                var now = DateTime.UtcNow.ToDATE_TIME();
                var instance = new VEVENT(@event)
                {
                    Id = keyGenerator.GetNext(),
                    Created = now,
                    Datestamp = now,
                    LastModified = now,
                    Start = recurrence,
                    End = recurrence + @event.Duration,
                    RecurrenceRule = null
                };

                instance.RecurrenceId = new RECURRENCE_ID
                {
                    Id = instance.Id,
                    Range = RANGE.THISANDFUTURE,
                    TimeZoneId = recurrence.TimeZoneId,
                    Value = instance.Start
                };
                recurs.Add(instance);
            }

            return recurs;
        }
    }
}
