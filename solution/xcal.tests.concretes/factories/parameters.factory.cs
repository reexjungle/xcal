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
        private readonly IValuesFactory valuesFactory;
        private readonly RandomGenerator rndGenerator;

        public ParametersFactory(IValuesFactory valuesFactory)
        {
            if (valuesFactory == null) throw new ArgumentNullException(nameof(valuesFactory));
            this.valuesFactory = valuesFactory;
            rndGenerator = new RandomGenerator();
        }

        public DELEGATED_FROM CreateDelegator(IEnumerable<string> usernames, int quantity)
        {
            return new DELEGATED_FROM
            {
                Addresses = valuesFactory.CreateEmails(usernames, rndGenerator.Next(1, quantity)).ToList()
            };
        }

        public IEnumerable<DELEGATED_FROM> CreateDelegators(IEnumerable<string> usernames, int quantity)
        {
            return Builder<DELEGATED_FROM>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Addresses = valuesFactory.CreateEmails(usernames, rndGenerator.Next(1, quantity)).ToList())
                .Build();
        }

        public IEnumerable<DELEGATED_TO> CreateDelegatees(IEnumerable<string> usernames, int quantity)
        {
            return Builder<DELEGATED_TO>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Addresses = valuesFactory.CreateEmails(usernames, rndGenerator.Next(1, quantity)).ToList())
                .Build();
        }

        public DELEGATED_TO CreateDelegatee(IEnumerable<string> usernames, int quantity)
        {
            return new DELEGATED_TO
            {
                Addresses = valuesFactory.CreateEmails(usernames, rndGenerator.Next(1, quantity)).ToList()
            };
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
            return new LANGUAGE("LANGUAGE=" + Pick<string>.RandomItemFrom(new[]
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

        public MEMBER CreateMember(IEnumerable<string> usernames, int quantity)
        {
            return new MEMBER
            {
                Addresses = valuesFactory.CreateEmails(usernames, rndGenerator.Next(1, quantity)).ToList()
            };
        }

        public IEnumerable<MEMBER> CreateMembers(IEnumerable<string> usernames, int quantity)
        {
            return Builder<MEMBER>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Addresses = valuesFactory.CreateEmails(usernames, rndGenerator.Next(1, quantity)).ToList())
                .Build();
        }

        public TZID CreateTimeZoneId()
        {
            return new TZID("TZID=" + Pick<string>.RandomItemFrom(new[]
            {
                string.Format("/{0}", "Greenwich"),
                "America/New_York",
                "America/Los_Angeles",
                "Germany/Berlin",
                "France/Paris",
                "Cameroon/Yaoundé",
                "Spain/Madrid",
                "Italy/Rome",
                "England/London",
                "Portugal/Lisbon",
                "Sweden/Stockholm",
                "Denmark/Copenhagen"
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

        public ALTREP CreateAlternativeTextRepresentation()
        {
            return new ALTREP
            {
                Uri = valuesFactory.CreateUri()
            };
        }

        public IEnumerable<ALTREP> CreateAlternativeTextRepresentations(int quantity)
        {
            var altreps = new List<ALTREP>();
            for (var i = 0; i < quantity; i++)
            {
                altreps.Add(CreateAlternativeTextRepresentation());
            }
            return altreps;
        }

        public DIR CreateDirectory()
        {
            return new DIR
            {
                Uri = valuesFactory.CreateUri()
            };
        }

        public IEnumerable<DIR> CreateDirectories(int quantity)
        {
            var dirs = new List<DIR>();
            for (var i = 0; i < quantity; i++)
            {
                dirs.Add(CreateDirectory());
            }
            return dirs;
        }

        public SENT_BY CreateSentBy(string username)
        {
            return new SENT_BY(valuesFactory.CreateEmail(username));
        }

        public IEnumerable<SENT_BY> CreateSentBys(IEnumerable<string> usernames, int quantity)
        {
            var sentbys = new List<SENT_BY>();
            var names = usernames as IList<string> ?? usernames.ToList();
            for (var i = 0; i < quantity; i++)
            {
                sentbys.Add(CreateSentBy(Pick<string>.RandomItemFrom(names)));
            }
            return sentbys;
        }
    }
}