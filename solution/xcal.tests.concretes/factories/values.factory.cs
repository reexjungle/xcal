using FizzWare.NBuilder;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.extensions;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.tests.contracts.factories;
using reexjungle.xmisc.infrastructure.contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace reexjungle.xcal.tests.concretes.factories
{
    public class ValuesFactory : IValuesFactory
    {
        private readonly IKeyGenerator<Guid> keyGenerator;
        private readonly RandomGenerator rndGenerator;
        private readonly SequentialGenerator<DateTime> dateTimeGenerator;

        public ValuesFactory(IKeyGenerator<Guid> keyGenerator)
        {
            if (keyGenerator == null) throw new ArgumentNullException("keyGenerator");
            this.keyGenerator = keyGenerator;

            rndGenerator = new RandomGenerator();
            dateTimeGenerator = new SequentialGenerator<DateTime>
            {
                IncrementDateBy = Pick<IncrementDate>.RandomItemFrom(new[]
                {
                    IncrementDate.Second,
                    IncrementDate.Minute,
                    IncrementDate.Hour,
                    IncrementDate.Day,
                    IncrementDate.Month,
                    IncrementDate.Year,
                }),
                Direction = GeneratorDirection.Ascending
            };
        }

        private List<uint> Generate(int quantity, uint min, uint max)
        {
            var numbers = new List<uint>();
            for (var q = 0; q < quantity; q++)
            {
                numbers.Add(rndGenerator.Next(min, max));
            }
            return numbers;
        }

        private List<int> Generate(int quantity, int min, int max)
        {
            var numbers = new List<int>();
            for (var q = 0; q < quantity; q++)
            {
                numbers.Add(rndGenerator.Next(min, max));
            }
            return numbers;
        }

        private List<uint> Generate(FREQ freq)
        {
            switch (freq)
            {
                case FREQ.SECONDLY:
                    return Generate(1, 1u, 59u);

                case FREQ.MINUTELY:
                    return Generate(1, 1u, 59u);

                case FREQ.HOURLY:
                    return Generate(1, 1u, 23u);

                case FREQ.WEEKLY:
                    return Generate(1, 1u, 53u);

                case FREQ.MONTHLY:
                    return Generate(1, 1u, 12u);

                case FREQ.YEARLY:
                    return Generate(1, 1u, 9999u);

                default:
                    return Generate(1, 1u, 366u);
            }
        }

        private static WEEKDAY PickRandomWeekday()
        {
            return Pick<WEEKDAY>.RandomItemFrom(new[]
            {
                WEEKDAY.MO,
                WEEKDAY.TU,
                WEEKDAY.WE,
                WEEKDAY.TH,
                WEEKDAY.FR,
                WEEKDAY.SA,
                WEEKDAY.SU,
            });
        }

        public BINARY CreateBinary()
        {
            return CreateBinaries(1).First();
        }

        public IEnumerable<BINARY> CreateBinaries(int quantity)
        {
            return Builder<BINARY>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Value = Pick<string>.RandomItemFrom(new[] { rndGenerator.Phrase(rndGenerator.Next(1, 100)) }))
                .And(x => x.Encoding = Pick<ENCODING>.RandomItemFrom(new[] { ENCODING.BASE64, ENCODING.BIT8 }))
                .Build();
        }

        public DATE CreateDate()
        {
            return CreateDates(1).First();
        }

        public IEnumerable<DATE> CreateDates(int quantity)
        {
            dateTimeGenerator.StartingWith(DateTime.Now);

            var dates = new List<DateTime>();

            for (var i = 0; i < quantity; i++)
            {
                dates.Add(dateTimeGenerator.Generate());
            }
            return dates.ToDATEs();
        }

        public DATE_TIME CreateDateTime()
        {
            return CreateDateTimes(1).First();
        }

        public IEnumerable<DATE_TIME> CreateDateTimes(int quantity)
        {
            dateTimeGenerator.StartingWith(DateTime.Now);

            var datetimes = new List<DateTime>();

            for (var i = 0; i < quantity; i++)
            {
                datetimes.Add(dateTimeGenerator.Generate());
            }
            return datetimes.ToDATE_TIMEs();
        }

        public TIME CreateTime()
        {
            return CreateTimes(1).First();
        }

        public IEnumerable<TIME> CreateTimes(int quantity)
        {
            dateTimeGenerator.StartingWith(DateTime.Now);

            var times = new List<DateTime>();

            for (var i = 0; i < quantity; i++)
            {
                times.Add(dateTimeGenerator.Generate());
            }
            return times.ToTIMEs();
        }

        public DURATION CreateDuration()
        {
            return CreateDurations(1).First();
        }

        public IEnumerable<DURATION> CreateDurations(int quantity)
        {
            var durations = new List<DURATION>();
            for (var i = 0; i < quantity; i++)
            {
                durations.Add(new DURATION(
                    rndGenerator.Next(0, 54),
                    rndGenerator.Next(0, 366),
                    rndGenerator.Next(0, 23),
                    rndGenerator.Next(0, 59),
                    rndGenerator.Next(0, 59)));
            }
            return durations;
        }

        public WEEKDAYNUM CreateWeekdaynum()
        {
            return new WEEKDAYNUM(rndGenerator.Next(1, 54), PickRandomWeekday());
        }

        public IEnumerable<WEEKDAYNUM> CreateWeekdaynums(int quantity)
        {
            var weekdaynums = new List<WEEKDAYNUM>();
            for (var i = 0; i < quantity; i++)
            {
                weekdaynums.Add(CreateWeekdaynum());
            }
            return weekdaynums;
        }

        public UTC_OFFSET CreateUtcOffset()
        {
            return new UTC_OFFSET(
                rndGenerator.Next(0, 23),
                rndGenerator.Next(0, 59),
                rndGenerator.Next(0, 59));
        }

        public IEnumerable<UTC_OFFSET> CreateUtcOffsets(int quantity)
        {
            var offsets = new List<UTC_OFFSET>();
            for (var i = 0; i < quantity; i++)
            {
                offsets.Add(CreateUtcOffset());
            }
            return offsets;
        }

        public PERIOD CreatePeriod()
        {
            var chance = rndGenerator.Next(1, 10000);
            var start = CreateDateTime();
            var duration = new DURATION(
                    rndGenerator.Next(0, 366),
                    rndGenerator.Next(0, 23),
                    rndGenerator.Next(0, 59));

            return chance % 2 == 0
                ? new PERIOD(start, duration)
                : new PERIOD(start, start + duration);
        }

        public IEnumerable<PERIOD> CreatePeriods(int quantity)
        {
            var periods = new List<PERIOD>();
            for (var i = 0; i < quantity; i++)
            {
                periods.Add(CreatePeriod());
            }
            return periods;
        }

        public RECUR CreateRecurrence()
        {
            return CreateRecurrences(1).First();
        }

        public IEnumerable<RECUR> CreateRecurrences(int quantity)
        {
            return Builder<RECUR>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Id = keyGenerator.GetNext())
                .And(x => x.BYDAY = CreateWeekdaynums(rndGenerator.Next(0, quantity)).ToList())
                .And(x => x.BYHOUR = Generate(rndGenerator.Next(0, quantity), 0u, 23u))
                .And(x => x.BYMINUTE = Generate(rndGenerator.Next(0, quantity), 0u, 59u))
                .And(x => x.BYMONTH = Generate(rndGenerator.Next(0, quantity), 1u, 12u))
                .And(x => x.BYMONTHDAY = Generate(rndGenerator.Next(0, quantity), -31, 31))
                .And(x => x.BYSECOND = Generate(rndGenerator.Next(0, quantity), 0u, 59u))
                .And(x => x.BYSETPOS = Generate(rndGenerator.Next(0, quantity), -366, 366))
                .And(x => x.BYWEEKNO = Generate(rndGenerator.Next(0, quantity), -53, 53))
                .And(x => x.BYYEARDAY = Generate(rndGenerator.Next(0, quantity), -366, 366))
                .And(x => x.FREQ = Pick<FREQ>.RandomItemFrom(new[]
                {
                    FREQ.SECONDLY,
                    FREQ.MINUTELY,
                    FREQ.HOURLY,
                    FREQ.DAILY,
                    FREQ.WEEKLY,
                    FREQ.MONTHLY,
                    FREQ.YEARLY
                }))
                .And(x => x.COUNT = rndGenerator.Next(0u, 366u))
                .And(x => x.INTERVAL = Generate(x.FREQ).First())
                .And(x => x.UNTIL = CreateDateTime())
                .And(x => x.WKST = PickRandomWeekday())
                .Build();
        }

        public URI CreateUri(string uri)
        {
            return !string.IsNullOrWhiteSpace(uri) ? new URI(uri): default(URI);
        }

        public IEnumerable<URI> CreateUris(IEnumerable<string> uris)
        {
            return uris !=null ? uris.Select(CreateUri): Enumerable.Empty<URI>();
        }
    }
}