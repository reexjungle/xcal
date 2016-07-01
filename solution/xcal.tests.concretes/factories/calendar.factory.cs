using FizzWare.NBuilder;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.tests.contracts.factories;
using reexjungle.xmisc.infrastructure.contracts;
using System;
using System.Collections.Generic;
using reexjungle.xmisc.foundation.concretes;

namespace reexjungle.xcal.tests.concretes.factories
{
    public class CalendarFactory : ICalendarFactory
    {
        private readonly IKeyGenerator<Guid> guidKeyGenerator;
        private readonly IKeyGenerator<Fpi> fpiKeyGenerator;

        public CalendarFactory(
            IKeyGenerator<Guid> guidKeyGenerator, 
            IKeyGenerator<Fpi> fpiKeyGenerator)
        {
            if (guidKeyGenerator == null) throw new ArgumentNullException(nameof(guidKeyGenerator));
            if (fpiKeyGenerator == null) throw new ArgumentNullException(nameof(fpiKeyGenerator));

            this.guidKeyGenerator = guidKeyGenerator;
            this.fpiKeyGenerator = fpiKeyGenerator;
        }

        public VCALENDAR Create()
        {
            return new VCALENDAR
            {
                Id = guidKeyGenerator.GetNext(),
                ProdId = fpiKeyGenerator.GetNext().ToString(),
                Calscale = Pick<CALSCALE>.RandomItemFrom(new[]
                {
                    CALSCALE.CHINESE,
                    CALSCALE.GREGORIAN,
                    CALSCALE.HEBREW,
                    CALSCALE.INDIAN,
                    CALSCALE.ISLAMIC
                }),

                Method = Pick<METHOD>.RandomItemFrom(new[]
                {
                    METHOD.ADD,
                    METHOD.CANCEL,
                    METHOD.DECLINECOUNTER,
                    METHOD.COUNTER,
                    METHOD.PUBLISH,
                    METHOD.REFRESH,
                    METHOD.REPLY,
                    METHOD.REQUEST,
                })
            };
        }

        public IEnumerable<VCALENDAR> Create(int quantity)
        {
            return Builder<VCALENDAR>.CreateListOfSize(5)
                .All()
                    .With(x => x.ProdId = fpiKeyGenerator.GetNext().ToString())
                    .And(x => x.Id = guidKeyGenerator.GetNext())
                .Build();
        }
    }
}