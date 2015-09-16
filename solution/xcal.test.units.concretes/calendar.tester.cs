using FizzWare.NBuilder;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.test.units.contracts;
using reexjungle.xmisc.foundation.contracts;
using reexjungle.xmisc.infrastructure.concretes.operations;
using reexjungle.xmisc.infrastructure.contracts;
using System;
using System.Collections.Generic;

namespace reexjungle.xcal.test.units.concretes
{
    public class CalendarTester : ICalendarTester
    {
        private readonly IKeyGenerator<Guid> guidKeyGenerator;
        private readonly IKeyGenerator<string> fpiKeyGenerator;

        public CalendarTester()
        {
            this.guidKeyGenerator = guidKeyGenerator ?? new SequentialGuidKeyGenerator();

            var statusGenerator = new ContentGenerator<ApprovalStatus>(new List<ApprovalStatus>
            {
                ApprovalStatus.Informal,
                ApprovalStatus.None,
                ApprovalStatus.Standard
            });

            var authorGenerator = new ContentGenerator<string>(new List<string> { "RXJG", "MSFT", "GOGL", "YHOO" });
            var productGenerator = new ContentGenerator<string>(new List<string> { "VCALENDAR", "ICALENDAR", "CALDAV" });
            var descriptionGenerator = new ContentGenerator<string>(new List<string>
            {
                new RandomGenerator().Phrase(new RandomGenerator().Next(3, 7)),
                new RandomGenerator().Phrase(new RandomGenerator().Next(5, 10)),
                new RandomGenerator().Phrase(new RandomGenerator().Next(15, 25)),
            });

            var languageGenerator = new ContentGenerator<string>(new List<string> { "EN", "FR", "DE", "IT", "ES", "PL" });
            var referenceGenerator = new ContentGenerator<string>(new List<string> { "ISO", "IEEE", "WPO", "UN" });
            this.fpiKeyGenerator = fpiKeyGenerator ?? new FpiStringKeyGenerator(statusGenerator,
                authorGenerator,
                productGenerator,
                descriptionGenerator,
                languageGenerator,
                referenceGenerator);
        }

        public IEnumerable<VCALENDAR> GenerateCalendarsOfSize(int n)
        {
            return Builder<VCALENDAR>.CreateListOfSize(5)
                .All()
                    .With(x => x.ProdId = fpiKeyGenerator.GetNext())
                    .And(x => x.Id = guidKeyGenerator.GetNext())
                .Build();
        }
    }
}