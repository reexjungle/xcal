using System;
using System.Collections.Generic;
using System.Linq;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.domain.extensions;
using reexjungle.xcal.tests.concretes.factories;
using reexjungle.xcal.tests.contracts.factories;
using reexjungle.xmisc.infrastructure.concretes.operations;
using reexjungle.xmisc.infrastructure.contracts;
using Xunit;

namespace reexjungle.xcal.tests.concretes.units
{
    public class EventUnitTests
    {
        private readonly IEventFactory factory;
        private readonly IKeyGenerator<Guid> keyGenerator;

        public EventUnitTests()
        {
            keyGenerator = new SequentialGuidKeyGenerator();
            var sharedFactory = new SharedFactory();
            var valuesFactory = new ValuesFactory(keyGenerator);
            var parametersFactory = new ParametersFactory(valuesFactory, sharedFactory);
            var propertiesFactory = new PropertiesFactory(keyGenerator, valuesFactory, parametersFactory, sharedFactory);
            var alarmFactory = new AlarmFactory(keyGenerator, propertiesFactory, valuesFactory);
            factory = new EventFactory(keyGenerator, alarmFactory, propertiesFactory, valuesFactory);
        }

        #region Test Recurrence Rules

        [Fact]
        public void CheckRecurrenceRuleForDifferentTimeTypes()
        {
            var created = factory.Create(3);
            var events = created as IList<VEVENT> ?? created.ToList();
            var event1 = events[0];
            event1.Start = new DATE_TIME(2014, 9, 1, 9, 0, 0, TimeType.LocalAndTimeZone, new TZID("America", "New_York"));
            event1.End = new DATE_TIME(2014, 9, 1, 11, 30, 0, TimeType.LocalAndTimeZone, new TZID("America", "New_York"));
            event1.RecurrenceRule = new RECUR
            {
                FREQ = FREQ.DAILY,
                INTERVAL = 1,
            };

            var rx = event1.GenerateRecurrences(keyGenerator);
            Assert.Equal(rx.First().Start.Type, TimeType.LocalAndTimeZone);
            Assert.Equal(rx.First().End.Type, TimeType.LocalAndTimeZone);
            Assert.Equal(rx.First().End.TimeZoneId, new TZID("America", "New_York"));

            var y = events[1];
            y.Start = new DATE_TIME(2014, 9, 1, 9, 0, 0);
            y.End = new DATE_TIME(2014, 9, 1, 11, 30, 0);
            y.RecurrenceRule = new RECUR
            {
                FREQ = FREQ.DAILY,
                UNTIL = new DATE_TIME(2014, 9, 30, 9, 0, 0, TimeType.Utc)
            };

            var ry = y.GenerateRecurrences(keyGenerator);
            Assert.Equal(ry.First().Start.Type, TimeType.Local);
            Assert.Equal(ry.First().End.Type, TimeType.Local);
            Assert.Equal(ry.First().End.TimeZoneId, null);

            var z = events[2];
            z.Start = new DATE_TIME(2014, 9, 1, 9, 0, 0, TimeType.Utc);
            z.End = new DATE_TIME(2014, 9, 1, 11, 30, 0, TimeType.Utc);
            z.RecurrenceRule = new RECUR
            {
                FREQ = FREQ.DAILY,
                COUNT = 29,
                INTERVAL = 1
            };

            var rz = z.GenerateRecurrences(keyGenerator);
            Assert.Equal(rz.First().Start.Type, TimeType.Utc);
            Assert.Equal(rz.First().End.Type, TimeType.Utc);
            Assert.Equal(rz.First().End.TimeZoneId, null);
        }

        [Fact]
        public void CheckSecondlyRecurrenceRule()
        {
            var x = factory.Create();
            x.Start = new DATE_TIME(2014, 9, 1, 9, 0, 0, TimeType.Utc);
            x.End = new DATE_TIME(2014, 9, 1, 11, 30, 0, TimeType.Utc);
            x.RecurrenceRule = new RECUR
            {
                FREQ = FREQ.SECONDLY,
                INTERVAL = 24 * 60 * 60,
                UNTIL = new DATE_TIME(2014, 9, 30, 9, 0, 0, TimeType.Utc)
            };

            var Rx = x.GenerateRecurrences(keyGenerator);
            Assert.Equal(Rx.Count, 29);
            Assert.Equal(Rx.Last().Start, new DATE_TIME(2014, 9, 30, 9, 0, 0, TimeType.Utc));

            //check bymonth filter
            x.Start = new DATE_TIME(2014, 1, 1, 9, 0, 0, TimeType.Utc);
            x.End = new DATE_TIME(2014, 1, 1, 11, 30, 0, TimeType.Utc);

            x.RecurrenceRule = new RECUR
            {
                FREQ = FREQ.SECONDLY,
                INTERVAL = 24 * 60 * 60,
                UNTIL = new DATE_TIME(2014, 12, 31, 23, 59, 59, TimeType.Utc),
                BYMONTH = new List<uint> { 1, 3, 9, 7 }
            };

            Rx = x.GenerateRecurrences(keyGenerator);
            Assert.Equal(Rx.Count, 122);
            Assert.Equal(Rx.Last().Start, new DATE_TIME(2014, 09, 30, 9, 0, 0, TimeType.Utc));

            //check byyeardday filter
            x.RecurrenceRule = new RECUR
            {
                FREQ = FREQ.SECONDLY,
                INTERVAL = 24 * 60 * 60,
                UNTIL = new DATE_TIME(2014, 12, 31, 23, 59, 59, TimeType.Utc),
                BYYEARDAY = new List<int> { -31, 36, 38, 40 }
            };

            Rx = x.GenerateRecurrences(keyGenerator);
            Assert.Equal(Rx.First().Start.MDAY, 5u);
            Assert.Equal(Rx.Last().Start.MONTH, 12u);

            //check bymonthday filter
            x.RecurrenceRule = new RECUR
            {
                FREQ = FREQ.SECONDLY,
                INTERVAL = 24 * 60 * 60,
                UNTIL = new DATE_TIME(2014, 12, 31, 23, 59, 59, TimeType.Utc),
                BYMONTHDAY = new List<int> { 1, -1 }
            };

            Rx = x.GenerateRecurrences(keyGenerator);
            Assert.Equal(Rx.Count(), 23);
            Assert.Equal(Rx.First().Start, new DATE_TIME(2014, 1, 31, 9, 0, 0, TimeType.Utc));
            Assert.Equal(Rx.Last().End, new DATE_TIME(2014, 12, 31, 11, 30, 0, TimeType.Utc));

            //check byday filter
            x.Start = new DATE_TIME(2014, 11, 1, 10, 15, 0, TimeType.Utc);
            x.End = new DATE_TIME(2014, 11, 1, 12, 45, 0, TimeType.Utc);
            x.RecurrenceRule = new RECUR
            {
                FREQ = FREQ.SECONDLY,
                INTERVAL = 24 * 60 * 60,
                UNTIL = new DATE_TIME(2014, 11, 30, 23, 59, 59, TimeType.Utc),
                BYDAY = new List<WEEKDAYNUM>
                {
                    new WEEKDAYNUM(WEEKDAY.MO),
                    new WEEKDAYNUM(WEEKDAY.TU),
                    new WEEKDAYNUM(WEEKDAY.WE),
                    new WEEKDAYNUM(WEEKDAY.TH),
                    new WEEKDAYNUM(WEEKDAY.FR),
                }
            };

            Rx = x.GenerateRecurrences(keyGenerator);
            Assert.Equal(Rx.Count(), 20);
            Assert.Equal(Rx.First().Start, new DATE_TIME(2014, 11, 03, 10, 15, 0, TimeType.Utc));
            Assert.Equal(Rx.Last().Start, new DATE_TIME(2014, 11, 28, 10, 15, 0, TimeType.Utc));

            //check byhour, byminute and bysecond filters
            x.Start = new DATE_TIME(2014, 12, 1, 08, 30, 30, TimeType.Utc);
            x.End = new DATE_TIME(2014, 12, 1, 08, 30, 0, TimeType.Utc);
            x.RecurrenceRule = new RECUR
            {
                FREQ = FREQ.SECONDLY,
                INTERVAL = 60,
                UNTIL = new DATE_TIME(2014, 12, 01, 10, 30, 30, TimeType.Utc),
                BYHOUR = new List<uint> { 9, 10 },
                BYMINUTE = new List<uint> { 15, 20, 25 },
                BYSECOND = new List<uint> { 30 }
            };

            Rx = x.GenerateRecurrences(keyGenerator);
            Assert.Equal(Rx.Count(), 6);
            Assert.Equal(Rx.ElementAt(1).Start, new DATE_TIME(2014, 12, 01, 9, 20, 30, TimeType.Utc));
            Assert.Equal(Rx.ElementAt(3).Start, new DATE_TIME(2014, 12, 01, 10, 15, 30, TimeType.Utc));
        }

        [Fact]
        public void CheckMinutelyRecurrenceRule()
        {
            var x = factory.Create();
            x.Start = new DATE_TIME(2014, 01, 02, 08, 30, 00, TimeType.Utc);
            x.End = new DATE_TIME(2014, 01, 02, 11, 30, 00, TimeType.Utc);

            x.RecurrenceRule = new RECUR
            {
                FREQ = FREQ.MINUTELY,
                INTERVAL = 1,
                UNTIL = new DATE_TIME(2014, 01, 02, 08, 45, 0, TimeType.Utc),
                BYMINUTE = new List<uint> {30, 31, 33, 35, 37, 39, 40, 45},
                BYSECOND = new List<uint> {0, 15, 30, 45},
                BYSETPOS = new List<int> {1, -1},
            };

            var Rx = x.GenerateRecurrences(keyGenerator);
            Assert.Equal(Rx.Count, 14);
            Assert.Equal(Rx.First().Start, new DATE_TIME(2014, 1, 02, 08, 30, 45, TimeType.Utc));
            Assert.Equal(Rx.Last().Start, new DATE_TIME(2014, 1, 02, 08, 45, 00, TimeType.Utc));
        }

        [Fact]
        public void CheckHourlyRecurrenceRule()
        {
            var x = factory.Create();
            x.Start = new DATE_TIME(2014, 1, 1, 9, 0, 0, TimeType.Utc);
            x.End = new DATE_TIME(2014, 1, 1, 11, 30, 0, TimeType.Utc);

            //check byday filter
            x.RecurrenceRule = new RECUR
            {
                FREQ = FREQ.HOURLY,
                INTERVAL = 1,
                UNTIL = new DATE_TIME(2014, 1, 1, 12, 0, 0, TimeType.Utc),
                BYMINUTE = new List<uint> { 5, 10, 15, 20, 25 },
                BYSETPOS = new List<int> { 1, -1 }
            };

            var Rx = x.GenerateRecurrences(keyGenerator);
            Assert.Equal(Rx.Count, 6);
            Assert.Equal(Rx.First().Start, new DATE_TIME(2014, 1, 1, 9, 5, 0, TimeType.Utc));
            Assert.Equal(Rx.Last().Start, new DATE_TIME(2014, 1, 1, 11, 25, 0, TimeType.Utc));
            Assert.Equal(Rx.ElementAt(2).End, new DATE_TIME(2014, 1, 1, 12, 35, 0, TimeType.Utc));
        }

        [Fact]
        public void CheckDailyRecurrenceRule()
        {
            var x = factory.Create();
            x.Start = new DATE_TIME(2014, 06, 15, 06, 0, 0, TimeType.Utc);
            x.End = new DATE_TIME(2014, 06, 15, 08, 45, 0, TimeType.Utc);

            x.RecurrenceRule = new RECUR
            {
                FREQ = FREQ.DAILY,
                INTERVAL = 1,
                UNTIL = new DATE_TIME(2014, 06, 16, 06, 0, 0, TimeType.Utc),
                BYHOUR = new List<uint> { 9, 12, 15, 18, 21 },
                BYMINUTE = new List<uint> { 5, 10, 15, 20, 25 },
                BYSETPOS = new List<int> { 2, -2 }
            };

            var Rx = x.GenerateRecurrences(keyGenerator);
            Assert.Equal(Rx.Count, 10);
            Assert.Equal(Rx.First().Start, new DATE_TIME(2014, 06, 15, 09, 10, 0, TimeType.Utc));
            Assert.Equal(Rx.Last().Start, new DATE_TIME(2014, 06, 15, 21, 20, 0, TimeType.Utc));
        }

        [Fact]
        public void CheckWeeklyRecurrenceRule()
        {
            var x = factory.Create();
            x.Start = new DATE_TIME(2014, 12, 1, 10, 0, 0, TimeType.Utc);
            x.End = new DATE_TIME(2014, 12, 1, 15, 30, 0, TimeType.Utc);
            x.ExceptionDates = new List<EXDATE>
            {
                new EXDATE{ DateTimes = new List<DATE_TIME>
                {
                    new DATE_TIME(2014,12,24, 10,0,0, TimeType.Utc),
                    new DATE_TIME(2014,12,25, 10,0,0, TimeType.Utc),
                    new DATE_TIME(2014,12,26, 10,0,0, TimeType.Utc),
                    new DATE_TIME(2014,12,31, 10,0,0, TimeType.Utc),
                } }
            };

            //check byday filter
            x.RecurrenceRule = new RECUR
            {
                FREQ = FREQ.WEEKLY,
                INTERVAL = 1,
                UNTIL = new DATE_TIME(2014, 12, 31, 9, 45, 0, TimeType.Utc),
                BYDAY = new List<WEEKDAYNUM>
                {
                  new WEEKDAYNUM (WEEKDAY.MO),
                  new WEEKDAYNUM (WEEKDAY.TU),
                  new WEEKDAYNUM (WEEKDAY.FR)
                },
                BYSETPOS = new List<int> { 2, -1 }
            };

            var Rx = x.GenerateRecurrences(keyGenerator);
            Assert.Equal(Rx.Count, 2);
            Assert.Equal(Rx.First().Start, new DATE_TIME(2014, 12, 02, 10, 00, 0, TimeType.Utc));
            Assert.Equal(Rx.Last().Start, new DATE_TIME(2014, 12, 30, 10, 00, 0, TimeType.Utc));
        }

        [Fact]
        public void CheckMonthlyRecurrenceRule()
        {
            var x = factory.Create();
            x.Start = new DATE_TIME(2014, 1, 1, 9, 0, 0, TimeType.Utc);
            x.End = new DATE_TIME(2014, 1, 1, 11, 30, 0, TimeType.Utc);

            //check byday filter
            x.RecurrenceRule = new RECUR
            {
                FREQ = FREQ.MONTHLY,
                INTERVAL = 1,
                UNTIL = new DATE_TIME(2014, 12, 31, 23, 59, 59, TimeType.Utc),
                BYDAY = new List<WEEKDAYNUM>
                {
                    new WEEKDAYNUM(WEEKDAY.MO),
                    new WEEKDAYNUM(WEEKDAY.TU),
                    new WEEKDAYNUM(WEEKDAY.WE),
                    new WEEKDAYNUM(WEEKDAY.TH),
                    new WEEKDAYNUM(WEEKDAY.FR),
                },
                BYSETPOS = new List<int> { -1 }
            };

            var Rx = x.GenerateRecurrences(keyGenerator);
            Assert.Equal(Rx.Count, 12);
            Assert.Equal(Rx.ElementAt(7).Start, new DATE_TIME(2014, 08, 29, 09, 00, 0, TimeType.Utc));
        }

        [Fact]
        public void CheckYearlyRecurrenceRule()
        {
            var x = factory.Create();
            x.Start = new DATE_TIME(2014, 1, 1, 7, 0, 0, TimeType.Utc);
            x.End = new DATE_TIME(2014, 1, 1, 10, 15, 0, TimeType.Utc);

            //check byday filter
            x.RecurrenceRule = new RECUR
            {
                FREQ = FREQ.YEARLY,
                INTERVAL = 1,
                UNTIL = new DATE_TIME(2016, 12, 31, 23, 59, 59, TimeType.Utc),
                BYMONTH = new List<uint> { 3, 9 },
                BYMONTHDAY = new List<int> { 15, 25 },
                BYDAY = new List<WEEKDAYNUM>
                {
                    new WEEKDAYNUM(WEEKDAY.MO),
                    new WEEKDAYNUM(WEEKDAY.TU),
                    new WEEKDAYNUM(WEEKDAY.WE),
                    new WEEKDAYNUM(WEEKDAY.TH),
                    new WEEKDAYNUM(WEEKDAY.FR),
                },
                BYSETPOS = new List<int> { -1 }
            };

            var Rx = x.GenerateRecurrences(keyGenerator);
            Assert.Equal(Rx.Count, 6);
            Assert.Equal(Rx.ElementAt(0).Start, new DATE_TIME(2014, 03, 25, 07, 00, 0, TimeType.Utc));
            Assert.Equal(Rx.ElementAt(1).Start, new DATE_TIME(2014, 09, 25, 07, 00, 0, TimeType.Utc));
            Assert.Equal(Rx.ElementAt(2).Start, new DATE_TIME(2015, 03, 25, 07, 00, 0, TimeType.Utc));
            Assert.Equal(Rx.ElementAt(3).Start, new DATE_TIME(2015, 09, 25, 07, 00, 0, TimeType.Utc));
            Assert.Equal(Rx.ElementAt(4).Start, new DATE_TIME(2016, 03, 25, 07, 00, 0, TimeType.Utc));
            Assert.Equal(Rx.ElementAt(5).Start, new DATE_TIME(2016, 09, 15, 07, 00, 0, TimeType.Utc));
        }

        [Fact]
        public void CheckNoRecurrenceRule()
        {
            //TODO: Test case where no recurrence rule is applied but usage of recurrence and exception dates
        }

        #endregion Test Recurrence Rules
    }

}
