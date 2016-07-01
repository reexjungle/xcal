using System;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.tests.contracts.factories;
using reexjungle.xmisc.infrastructure.contracts;

namespace reexjungle.xcal.tests.concretes.factories
{
    public class AlarmFactory: IAlarmFactory
    {
        private readonly IKeyGenerator<Guid> keyGenerator;
        private readonly RandomGenerator rndGenerator;
        private readonly IPropertiesFactory propertiesFactory;
        private readonly IValuesFactory valuesFactory;

        public AlarmFactory(IKeyGenerator<Guid> keyGenerator, IPropertiesFactory propertiesFactory, IValuesFactory valuesFactory)
        {
            if (keyGenerator == null) throw new ArgumentNullException(nameof(keyGenerator));
            if (propertiesFactory == null) throw new ArgumentNullException(nameof(propertiesFactory));

            this.keyGenerator = keyGenerator;
            this.rndGenerator = new RandomGenerator();
            this.propertiesFactory = propertiesFactory;
            this.valuesFactory = valuesFactory;
        }


        public AUDIO_ALARM CreateAudioAlarm()
        {
            return CreateAudioAlarms(1).First();
        }

        public DISPLAY_ALARM CreateDisplayAlarm()
        {
            return CreateDisplayAlarms(1).First();
        }

        public EMAIL_ALARM CreateEmailAlarm()
        {
            return CreateEmailAlarms(1).First();
        }

        public IEnumerable<AUDIO_ALARM> CreateAudioAlarms(int quantity)
        {
            return Builder<AUDIO_ALARM>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Id = keyGenerator.GetNext())
                .And(x => x.Action = ACTION.AUDIO)
                .And(x => x.Duration = valuesFactory.CreateDuration())
                .And(x => x.Trigger = propertiesFactory.CreateTrigger())
                .And(x => x.Repeat = rndGenerator.Next(0, 10))
                .And(x => x.Attachment = propertiesFactory.CreateAttachBinary())
                .Build();
        }

        public IEnumerable<DISPLAY_ALARM> CreateDisplayAlarms(int quantity)
        {
            return Builder<DISPLAY_ALARM>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Id = keyGenerator.GetNext())
                .And(x => x.Action = ACTION.DISPLAY)
                .And(x => x.Duration = valuesFactory.CreateDuration())
                .And(x => x.Trigger = propertiesFactory.CreateTrigger())
                .And(x => x.Repeat = rndGenerator.Next(0, 10))
                .And(x => x.Description = propertiesFactory.CreateDescription())
                .Build();
        }

        public IEnumerable<EMAIL_ALARM> CreateEmailAlarms(int quantity)
        {
            return Builder<EMAIL_ALARM>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Id = keyGenerator.GetNext())
                .And(x => x.Action = ACTION.EMAIL)
                .And(x => x.Duration = valuesFactory.CreateDuration())
                .And(x => x.Trigger = propertiesFactory.CreateTrigger())
                .And(x => x.Repeat = rndGenerator.Next(0, 10))
                .And(x => x.Description = propertiesFactory.CreateDescription())
                .Build();
        }
    }
}
