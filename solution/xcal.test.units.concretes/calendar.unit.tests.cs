using FizzWare.NBuilder;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.test.units.contracts;
using reexjungle.xmisc.infrastructure.concretes.operations;
using reexjungle.xmisc.infrastructure.contracts;
using System.Collections.Generic;

namespace reexjungle.xcal.test.units.concretes
{
    public class CalendarUnitTests : ICalendarUnitTests
    {
        public IGuidKeyGenerator KeyGen { get; set; }

        public IFPIKeyGenerator FPIKeyGen { get; set; }

        public CalendarUnitTests()
        {
            this.KeyGen = new GuidKeyGenerator();
            this.FPIKeyGen = Builder<StringFPIKeyGenerator>
                .CreateNew()
                .With(x => x.Owner = Pick<string>.RandomItemFrom(new List<string> { "reexjungle", "reexmonkey" }))
                .And(x => x.Authority = Pick<Authority>.RandomItemFrom(new List<Authority> { Authority.ISO, Authority.None, Authority.NonStandard }))
                .And(x => x.Description = "Test iCalendar Service Provider")
                .And(x => x.LanguageId = Pick<string>.RandomItemFrom(new List<string> { "EN", "FR", "DE" }))
                .Build();
        }

        public IEnumerable<VCALENDAR> GenerateCalendarsOfSize(int n)
        {
            return Builder<VCALENDAR>.CreateListOfSize(5)
                .All()
                    .With(x => x.ProdId = this.FPIKeyGen.GetNextKey())
                    .And(x => x.Id = this.KeyGen.GetNextKey())
                .Build();
        }
    }
}