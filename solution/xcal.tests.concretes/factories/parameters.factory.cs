using FizzWare.NBuilder;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.tests.contracts.factories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace reexjungle.xcal.tests.concretes.factories
{
    public class ParametersFactory : IParametersFactory
    {
        private readonly ISharedFactory sharedFactory;
        private readonly IValuesFactory valuesFactory;
        private readonly RandomGenerator rndGenerator;

        public ParametersFactory(IValuesFactory valuesFactory, ISharedFactory sharedFactory)
        {
            if (valuesFactory == null) throw new ArgumentNullException("valuesFactory");
            if (sharedFactory == null) throw new ArgumentNullException("sharedFactory");

            this.valuesFactory = valuesFactory;
            this.sharedFactory = sharedFactory;

            rndGenerator = new RandomGenerator();
        }

        public DELEGATE CreateDelegate()
        {
            return CreateDelegates(1).First();
        }

        public IEnumerable<DELEGATE> CreateDelegates(int quantity)
        {
            return Builder<DELEGATE>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Addresses = valuesFactory.CreateUris(sharedFactory.CreateEmails(rndGenerator.Next(1, quantity))).ToList())
                .Build();
        }

        public FMTTYPE CreateFormatType()
        {
            return new FMTTYPE(Pick<string>.RandomItemFrom(new[]
            {
                "FMTTYPE=audio/basic",
                "FMTTYPE=video/mpeg",
                "FMTTYPE=audio/wav",
                "FMTTYPE=audio/mp3",
                "FMTTYPE=image/jpeg",
                "FMTTYPE=image/png",
                "FMTTYPE=image/bitmap",
                "FMTTYPE=text/plain",
                "FMTTYPE=text/calendar",
                "FMTTYPE=application/postscript",
                "FMTTYPE=application/msword"
            }));
        }

        public IEnumerable<FMTTYPE> CreateFormatTypes(int quantity)
        {
            var fmttypes = new List<FMTTYPE>();

            for (var i = 0; i < quantity; i++)
            {
                fmttypes.Add(CreateFormatType());
            }

            return fmttypes;
        }

        public LANGUAGE CreateLanguage()
        {
            return new LANGUAGE(Pick<string>.RandomItemFrom(new[]
           {
               "en",
               "en-US",
               "en-EN",
               "fr",
               "fr-FR",
               "fr-FR",
               "es",
               "es-ES",
               "it-IT",
               "pl-PL",
               "ro-RO"
           }));
        }

        public IEnumerable<LANGUAGE> CreateLanguages(int quantity)
        {
            var languages = new List<LANGUAGE>();
            for (int i = 0; i < quantity; i++)
            {
                languages.Add(CreateLanguage());
            }
            return languages;
        }

        public MEMBER CreateMember()
        {
            return CreateMembers(1).First();
        }

        public IEnumerable<MEMBER> CreateMembers(int quantity)
        {
            return Builder<MEMBER>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Addresses = valuesFactory.CreateUris(sharedFactory.CreateEmails(rndGenerator.Next(1, quantity))).ToList())
                .Build();
        }

        public TZID CreateTimeZoneId()
        {
            return new TZID(Pick<string>.RandomItemFrom(new[]
            {
                string.Format("/{0}", "Greenwich"),
                "America/New_York",
                "America/Los_Angeles",
                "Germany/Berlin",
                "France/Paris",
                "Cameroon/Yaoundé",
                "Spain/Madrid",
                "Italy/Rome"
            }));
        }

        public IEnumerable<TZID> CreateTimeZoneIds(int quantity)
        {
            var tzids = new List<TZID>();

            for (var i = 0; i < quantity; i++)
            {
                tzids.Add(CreateTimeZoneId());
            }
            return tzids;
        }
    }
}