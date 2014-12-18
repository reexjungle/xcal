using FizzWare.NBuilder;
using reexjungle.infrastructure.operations.concretes;
using reexjungle.infrastructure.operations.contracts;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.test.units.contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace reexjungle.xcal.test.units.concretes
{
    public class EventUnitTests : IEventUnitTests
    {
        public IGuidKeyGenerator KeyGen { get; set; }

        public IEnumerable<VEVENT> GenerateEventsOfSize(int n)
        {
            var dgen = new SequentialGenerator<DateTime> { IncrementDateBy = IncrementDate.Day, Direction = GeneratorDirection.Ascending };
            dgen.StartingWith(new DateTime(2014, 06, 15));

            return Builder<VEVENT>.CreateListOfSize(n)
                .All()
                .With(x => x.Uid = this.KeyGen.GetNextKey())
                .And(x => x.Organizer = new ORGANIZER
                {
                    Id = this.KeyGen.GetNextKey(),
                    CN = Pick<string>.RandomItemFrom(new string[] { "Caesar", "Koba", "Cornelia", "Blue Eyes", "Grey", "Ash" }),
                    Address = new URI("organizer@apes.ju"),
                    Language = new LANGUAGE("en")
                })
                .And(x => x.Location = new LOCATION
                {
                    Text = "Düsseldorf",
                    AlternativeText = new URI("http://www.duesseldorf.de/de/"),
                    Language = new LANGUAGE("de", "DE")
                })
                .And(x => x.Summary = new SUMMARY("Test Meeting"))
                .And(x => x.Description = new DESCRIPTION("A meeting for coding gurus, nerds, geeks and quants who enjoy programming for others such that the world becomes a better place for all qui sonts presentes à la Gare de Nöel für ein außerordentliches wünderschönes Abend mit wünderschönen Menschen, egal ob sie arm sind oder nicht."))
                .And(x => x.Start = new DATE_TIME(dgen.Generate()))
                .And(x => x.Duration = new DURATION(0,
                    new RandomGenerator().Next(1, 7),
                    new RandomGenerator().Next(1, 12),
                    new RandomGenerator().Next(0, 59),
                    new RandomGenerator().Next(0, 59)))
                .And(x => x.Status = STATUS.CONFIRMED)
                .And(x => x.Transparency = TRANSP.TRANSPARENT)
                .And(x => x.Classification = CLASS.PUBLIC)
                .Build();
        }

        public EventUnitTests()
        {
            this.KeyGen = new GuidKeyGenerator();
        }

        public void RandomlyAttendEvents(ref IEnumerable<VEVENT> events, IEnumerable<ATTENDEE> attendees)
        {
            var atts = attendees.ToList(); var max = atts.Count;
            foreach (var x in events) x.Attendees.AddRange(Pick<ATTENDEE>.UniqueRandomList(With.Between(1, max)).From(atts));
        }

        #region Test Recurrence Rules

        [Fact]
        public void CheckRecurrenceRuleForDifferentTimeTypes()
        {
            var events = this.GenerateEventsOfSize(3).ToArray();
            var x = events[0];
            x.Start = new DATE_TIME(2014, 9, 1, 9, 0, 0, TimeType.LocalAndTimeZone, new TZID("America", "New_York"));
            x.End = new DATE_TIME(2014, 9, 1, 11, 30, 0, TimeType.LocalAndTimeZone, new TZID("America", "New_York"));
            x.RecurrenceRule = new RECUR
            {
                FREQ = FREQ.DAILY,
                INTERVAL = 1,
            };

            var Rx = x.GenerateRecurrences<VEVENT>();
            Assert.Equal(Rx.First().Start.Type, TimeType.LocalAndTimeZone);
            Assert.Equal(Rx.First().End.Type, TimeType.LocalAndTimeZone);
            Assert.Equal(Rx.First().End.TimeZoneId, new TZID("America", "New_York"));

            var y = events[1];
            y.Start = new DATE_TIME(2014, 9, 1, 9, 0, 0, TimeType.Local);
            y.End = new DATE_TIME(2014, 9, 1, 11, 30, 0, TimeType.Local);
            y.RecurrenceRule = new RECUR
            {
                FREQ = FREQ.DAILY,
                UNTIL = new DATE_TIME(2014, 9, 30, 9, 0, 0, TimeType.Utc)
            };

            var Ry = y.GenerateRecurrences<VEVENT>();
            Assert.Equal(Ry.First().Start.Type, TimeType.Local);
            Assert.Equal(Ry.First().End.Type, TimeType.Local);
            Assert.Equal(Ry.First().End.TimeZoneId, null);

            var z = events[2];
            z.Start = new DATE_TIME(2014, 9, 1, 9, 0, 0, TimeType.Utc);
            z.End = new DATE_TIME(2014, 9, 1, 11, 30, 0, TimeType.Utc);
            z.RecurrenceRule = new RECUR
            {
                FREQ = FREQ.DAILY,
                COUNT = 29,
                INTERVAL = 1
            };

            var Rz = z.GenerateRecurrences<VEVENT>();
            Assert.Equal(Rz.First().Start.Type, TimeType.Utc);
            Assert.Equal(Rz.First().End.Type, TimeType.Utc);
            Assert.Equal(Rz.First().End.TimeZoneId, null);
        }

        [Fact]
        public void CheckSecondlyRecurrenceRule()
        {
            var events = this.GenerateEventsOfSize(1).ToArray();
            var x = events[0];
            x.Start = new DATE_TIME(2014, 9, 1, 9, 0, 0, TimeType.Utc);
            x.End = new DATE_TIME(2014, 9, 1, 11, 30, 0, TimeType.Utc);
            x.RecurrenceRule = new RECUR
            {
                FREQ = FREQ.SECONDLY,
                INTERVAL = 24 * 60 * 60,
                UNTIL = new DATE_TIME(2014, 9, 30, 9, 0, 0, TimeType.Utc)
            };

            var Rx = x.GenerateRecurrences<VEVENT>();
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

            Rx = x.GenerateRecurrences<VEVENT>();
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

            Rx = x.GenerateRecurrences<VEVENT>();
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

            Rx = x.GenerateRecurrences<VEVENT>();
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

            Rx = x.GenerateRecurrences<VEVENT>();
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

            Rx = x.GenerateRecurrences<VEVENT>();
            Assert.Equal(Rx.Count(), 6);
            Assert.Equal(Rx.ElementAt(1).Start, new DATE_TIME(2014, 12, 01, 9, 20, 30, TimeType.Utc));
            Assert.Equal(Rx.ElementAt(3).Start, new DATE_TIME(2014, 12, 01, 10, 15, 30, TimeType.Utc));
        }

        [Fact]
        public void CheckMinutelyRecurrenceRule()
        {
            var events = this.GenerateEventsOfSize(1).ToArray();
            var x = events[0];
            x.Start = new DATE_TIME(2014, 01, 02, 08, 30, 00, TimeType.Utc);
            x.End = new DATE_TIME(2014, 01, 02, 11, 30, 00, TimeType.Utc);

            x.RecurrenceRule = new RECUR
            {
                FREQ = FREQ.MINUTELY,
                INTERVAL = 1,
                UNTIL = new DATE_TIME(2014, 01, 02, 08, 45, 0, TimeType.Utc),
                BYMINUTE = new List<uint> { 30, 31, 33, 35, 37, 39, 40, 45 },
                BYSECOND = new List<uint> { 0, 15, 30, 45 },
            };

            x.RecurrenceRule.BYSETPOS = new List<int> { 1, -1 };
            var Rx = x.GenerateRecurrences<VEVENT>();
            Assert.Equal(Rx.Count, 14);
            Assert.Equal(Rx.First().Start, new DATE_TIME(2014, 1, 02, 08, 30, 45, TimeType.Utc));
            Assert.Equal(Rx.Last().Start, new DATE_TIME(2014, 1, 02, 08, 45, 00, TimeType.Utc));
        }

        [Fact]
        public void CheckHourlyRecurrenceRule()
        {
            var events = this.GenerateEventsOfSize(1).ToArray();
            var x = events[0];
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

            var Rx = x.GenerateRecurrences<VEVENT>();
            Assert.Equal(Rx.Count, 6);
            Assert.Equal(Rx.First().Start, new DATE_TIME(2014, 1, 1, 9, 5, 0, TimeType.Utc));
            Assert.Equal(Rx.Last().Start, new DATE_TIME(2014, 1, 1, 11, 25, 0, TimeType.Utc));
            Assert.Equal(Rx.ElementAt(2).End, new DATE_TIME(2014, 1, 1, 12, 35, 0, TimeType.Utc));
        }

        [Fact]
        public void CheckDailyRecurrenceRule()
        {
            var events = this.GenerateEventsOfSize(1).ToArray();
            var x = events[0];
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

            var Rx = x.GenerateRecurrences<VEVENT>();
            Assert.Equal(Rx.Count, 10);
            Assert.Equal(Rx.First().Start, new DATE_TIME(2014, 06, 15, 09, 10, 0, TimeType.Utc));
            Assert.Equal(Rx.Last().Start, new DATE_TIME(2014, 06, 15, 21, 20, 0, TimeType.Utc));
        }

        [Fact]
        public void CheckWeeklyRecurrenceRule()
        {
            var events = this.GenerateEventsOfSize(1).ToArray();
            var x = events[0];
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

            var Rx = x.GenerateRecurrences<VEVENT>();
            Assert.Equal(Rx.Count, 2);
            Assert.Equal(Rx.First().Start, new DATE_TIME(2014, 12, 02, 10, 00, 0, TimeType.Utc));
            Assert.Equal(Rx.Last().Start, new DATE_TIME(2014, 12, 30, 10, 00, 0, TimeType.Utc));
        }

        [Fact]
        public void CheckMonthlyRecurrenceRule()
        {
            var events = this.GenerateEventsOfSize(1).ToArray();
            var x = events[0];
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

            var Rx = x.GenerateRecurrences<VEVENT>();
            Assert.Equal(Rx.Count, 12);
            Assert.Equal(Rx.ElementAt(7).Start, new DATE_TIME(2014, 08, 29, 09, 00, 0, TimeType.Utc));
        }

        [Fact]
        public void CheckYearlyRecurrenceRule()
        {
            var events = this.GenerateEventsOfSize(1).ToArray();
            var x = events[0];
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

            var Rx = x.GenerateRecurrences<VEVENT>();
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