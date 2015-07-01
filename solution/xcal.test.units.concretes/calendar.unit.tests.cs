using FizzWare.NBuilder;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.test.units.contracts;
using reexjungle.xmisc.infrastructure.contracts;
using System;
using System.Collections.Generic;

namespace reexjungle.xcal.test.units.concretes
{
    public class CalendarUnitTest : ICalendarUnitTest
    {
        private readonly IKeyGenerator<Guid> guidKeyGenerator;
        private readonly IKeyGenerator<string> fpiKeyGenerator;

        public CalendarUnitTest(IKeyGenerator<Guid> guidKeyGenerator, IKeyGenerator<string> fpiKeyGenerator)
        {
            if (guidKeyGenerator == null) throw new ArgumentNullException("guidKeyGenerator");
            if (fpiKeyGenerator == null) throw new ArgumentNullException("fpiKeyGenerator");

            this.guidKeyGenerator = guidKeyGenerator;
            this.fpiKeyGenerator = fpiKeyGenerator;
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