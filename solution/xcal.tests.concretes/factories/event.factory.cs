using FizzWare.NBuilder;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.tests.contracts.factories;
using reexjungle.xmisc.infrastructure.contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using reexjungle.xcal.domain.contracts;
using ServiceStack.Common.Extensions;

namespace reexjungle.xcal.tests.concretes.factories
{
    public class EventFactory : IEventFactory
    {
        private readonly IKeyGenerator<Guid> keyGenerator;
        private readonly RandomGenerator rndGenerator;
        private readonly IAlarmFactory alarmFactory;
        private readonly IPropertiesFactory propertiesFactory;
        private readonly IValuesFactory valuesFactory;

        public EventFactory(IKeyGenerator<Guid> keyGenerator, IAlarmFactory alarmFactory, IPropertiesFactory propertiesFactory, IValuesFactory valuesFactory)
        {
            if (keyGenerator == null) throw new ArgumentNullException("keyGenerator");
            if (propertiesFactory == null) throw new ArgumentNullException("propertiesFactory");
            if (valuesFactory == null) throw new ArgumentNullException("valuesFactory");

            this.keyGenerator = keyGenerator;
            this.propertiesFactory = propertiesFactory;
            this.valuesFactory = valuesFactory;
            this.alarmFactory = alarmFactory;

            rndGenerator = new RandomGenerator();
        }

        public VEVENT Create()
        {
            return Create(1).First();
        }

        public IEnumerable<VEVENT> Create(int quantity)
        {
            return Builder<VEVENT>.CreateListOfSize(quantity)
                .All()
                    .With(x => x.Id = keyGenerator.GetNext())
                    .And(x => x.Attendees = propertiesFactory.CreateAttendees(rndGenerator.Next(1, quantity)).ToList())
                    .And(x => x.AttachmentBinaries = propertiesFactory.CreateAttachBinaries(rndGenerator.Next(1, quantity)).ToList())
                    .And(x => x.AttachmentUris = propertiesFactory.CreateAttachUris(rndGenerator.Next(1, quantity)).ToList())
                    .And(x => x.RecurrenceRule = valuesFactory.CreateRecurrence())
                    .And(x => x.Classification = Pick<CLASS>.RandomItemFrom(new []
                    {
                        CLASS.CONFIDENTIAL, 
                        CLASS.PRIVATE,
                        CLASS.PUBLIC, 
                    }))
                    .And(x => x.Organizer = propertiesFactory.CreateOrganizer())
                    .And(x => x.Categories = propertiesFactory.CreateCategories())
                    .And(x => x.Description = propertiesFactory.CreateDescription())
                    .And(x => x.Comments = propertiesFactory.CreateComments(rndGenerator.Next(1, quantity)).ToList())
                    .And(x => x.Contacts = propertiesFactory.CreateContacts(rndGenerator.Next(1, quantity)).ToList())
                    .And(x => x.Summary = propertiesFactory.CreateSummary())
                    .And(x => x.Location = propertiesFactory.CreateLocation())
                    .And(x => x.Start = valuesFactory.CreateDateTime())
                    .And(x => x.Duration = valuesFactory.CreateDuration())
                    .And(x => x.Status = Pick<STATUS>.RandomItemFrom(new []
                    {
                        STATUS.CANCELLED, 
                        STATUS.COMPLETED, 
                        STATUS.CONFIRMED, 
                        STATUS.DRAFT, 
                        STATUS.FINAL, 
                        STATUS.IN_PROCESS, 
                        STATUS.NEEDS_ACTION, 
                        STATUS.TENTATIVE
                    }))
                    .And(x => x.Transparency = Pick<TRANSP>.RandomItemFrom(new []
                    {
                        TRANSP.OPAQUE, 
                        TRANSP.TRANSPARENT
                    }))
                    .And(x => x.ExceptionDates = propertiesFactory.CreateExceptionDates(rndGenerator.Next(1, quantity)).ToList())
                    .And(x => x.RecurrenceDates = propertiesFactory.CreateRecurrenceDates(rndGenerator.Next(1, quantity)).ToList())
                    .And(x => x.AudioAlarms = alarmFactory.CreateAudioAlarms(rndGenerator.Next(1, quantity)).ToList())
                    .And(x => x.DisplayAlarms = alarmFactory.CreateDisplayAlarms(rndGenerator.Next(1, quantity)).ToList())
                    .And(x => x.EmailAlarms = alarmFactory.CreateEmailAlarms(rndGenerator.Next(1, quantity)).ToList())
                .Build();
        }
    }
}