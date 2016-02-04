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
        private readonly TZID tzid;

        public EventUnitTests()
        {
            keyGenerator = new SequentialGuidKeyGenerator();
            var valuesFactory = new ValuesFactory(keyGenerator);
            var parametersFactory = new ParametersFactory(valuesFactory);
            var propertiesFactory = new PropertiesFactory(keyGenerator, valuesFactory, parametersFactory);
            var alarmFactory = new AlarmFactory(keyGenerator, propertiesFactory, valuesFactory);

            factory = new EventFactory(keyGenerator, alarmFactory, propertiesFactory, valuesFactory);

            tzid = new TZID("America", "New_York");
        }

        #region Standard (RFC5545) Recurrence Tests

        [Fact]
        public void CheckGeneratedDailyRecurrencesLimitedByCount()
        {
            var vevent = factory.Create();
            vevent.Start = new DATE_TIME(1997, 09, 02, 09, 00, 00, TimeType.LocalAndTimeZone, tzid);
            vevent.End = new DATE_TIME(1997, 09, 02, 10, 00, 00, TimeType.LocalAndTimeZone, tzid);

            //check byday filter
            vevent.RecurrenceRule = new RECUR
            {
                FREQ = FREQ.DAILY,
                COUNT = 10
            };

            var occurences = vevent.GenerateOccurrences(keyGenerator);
            Assert.Equal(occurences.Count, 10);

            //(1997 9:00 AM EDT) September 2-11
            Assert.Equal(occurences[0].Start, new DATE_TIME(1997, 09, 02, 09, 00, 00, TimeType.LocalAndTimeZone, tzid));
            Assert.Equal(occurences[1].Start, new DATE_TIME(1997, 09, 03, 09, 00, 00, TimeType.LocalAndTimeZone, tzid));
            Assert.Equal(occurences[2].Start, new DATE_TIME(1997, 09, 04, 09, 00, 00, TimeType.LocalAndTimeZone, tzid));
            Assert.Equal(occurences[3].Start, new DATE_TIME(1997, 09, 05, 09, 00, 00, TimeType.LocalAndTimeZone, tzid));
            Assert.Equal(occurences[4].Start, new DATE_TIME(1997, 09, 06, 09, 00, 00, TimeType.LocalAndTimeZone, tzid));
            Assert.Equal(occurences[5].Start, new DATE_TIME(1997, 09, 07, 09, 00, 00, TimeType.LocalAndTimeZone, tzid));
            Assert.Equal(occurences[6].Start, new DATE_TIME(1997, 09, 08, 09, 00, 00, TimeType.LocalAndTimeZone, tzid));
            Assert.Equal(occurences[7].Start, new DATE_TIME(1997, 09, 09, 09, 00, 00, TimeType.LocalAndTimeZone, tzid));
            Assert.Equal(occurences[8].Start, new DATE_TIME(1997, 09, 10, 09, 00, 00, TimeType.LocalAndTimeZone, tzid));
            Assert.Equal(occurences[9].Start, new DATE_TIME(1997, 09, 11, 09, 00, 00, TimeType.LocalAndTimeZone, tzid));
        }

        [Fact]
        public void CheckGeneratedDailyRecurrencesLimitedByUntil()
        {
            var vevent = factory.Create();
            vevent.Start = new DATE_TIME(1997, 09, 02, 09, 00, 00, TimeType.LocalAndTimeZone, tzid);
            vevent.End = new DATE_TIME(1997, 09, 02, 10, 00, 00, TimeType.LocalAndTimeZone, tzid);

            //check byday filter
            vevent.RecurrenceRule = new RECUR
            {
                FREQ = FREQ.DAILY,
                UNTIL = new DATE_TIME(1997,12,24,00,00,00, TimeType.Utc)
            };

            var occurences = vevent.GenerateOccurrences(keyGenerator);
            Assert.Equal(occurences.Count, 113);

            //(1997 9:00 AM EDT) September 2-30;October 1-25
            Assert.Equal(occurences[000].Start, new DATE_TIME(1997, 09, 02, 09, 00, 00, TimeType.LocalAndTimeZone, tzid));
            Assert.Equal(occurences[028].Start, new DATE_TIME(1997, 09, 30, 09, 00, 00, TimeType.LocalAndTimeZone, tzid));
            Assert.Equal(occurences[029].Start, new DATE_TIME(1997, 10, 01, 09, 00, 00, TimeType.LocalAndTimeZone, tzid));
            Assert.Equal(occurences[053].Start, new DATE_TIME(1997, 10, 25, 09, 00, 00, TimeType.LocalAndTimeZone, tzid));

            //(1997 9:00 AM EST) October 26-31;November 1-30;December 1-23
            Assert.Equal(occurences[054].Start, new DATE_TIME(1997, 10, 26, 09, 00, 00, TimeType.LocalAndTimeZone, tzid));
            Assert.Equal(occurences[059].Start, new DATE_TIME(1997, 10, 31, 09, 00, 00, TimeType.LocalAndTimeZone, tzid));
            Assert.Equal(occurences[060].Start, new DATE_TIME(1997, 11, 01, 09, 00, 00, TimeType.LocalAndTimeZone, tzid));
            Assert.Equal(occurences[089].Start, new DATE_TIME(1997, 11, 30, 09, 00, 00, TimeType.LocalAndTimeZone, tzid));
            Assert.Equal(occurences[090].Start, new DATE_TIME(1997, 12, 01, 09, 00, 00, TimeType.LocalAndTimeZone, tzid));
            Assert.Equal(occurences[112].Start, new DATE_TIME(1997, 12, 23, 09, 00, 00, TimeType.LocalAndTimeZone, tzid));
        }

        #endregion
    }

}
