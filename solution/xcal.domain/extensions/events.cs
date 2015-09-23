using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
            var dates = @event.Start.GenerateRecurrences(@event.RecurrenceRule);
            if (!@event.RecurrenceDates.NullOrEmpty())
            {
                var rdates = @event.RecurrenceDates.Where(x => !x.DateTimes.NullOrEmpty()).SelectMany(x => x.DateTimes).ToList();
                var rperiods = @event.RecurrenceDates.Where(x => !x.Periods.NullOrEmpty()).SelectMany(x => x.Periods).ToList();

                if (!rdates.NullOrEmpty()) dates.AddRange(rdates);
                if (!rperiods.NullOrEmpty()) dates.AddRange(rperiods.Select(x => x.Start));
            }

            if (!@event.ExceptionDates.NullOrEmpty())
            {
                var exdates = @event.ExceptionDates.Where(x => !x.DateTimes.NullOrEmpty()).SelectMany(x => x.DateTimes).ToList();
                if (!exdates.NullOrEmpty()) dates = dates.Except(exdates).ToList();
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
