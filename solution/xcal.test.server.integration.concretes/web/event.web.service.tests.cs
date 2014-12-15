using FizzWare.NBuilder;
using reexjungle.infrastructure.operations.concretes;
using reexjungle.infrastructure.operations.contracts;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.domain.operations;
using reexjungle.xcal.test.server.integration.contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace reexjungle.xcal.test.server.integration.concretes.web
{
    public abstract class EventWebServicesTests : IWebServiceTests
    {
        protected string baseUri = string.Empty;
        protected GuidKeyGenerator keygen = new GuidKeyGenerator();
        protected StringFPIKeyGenerator fkeygen = new StringFPIKeyGenerator();

        public void Initialize()
        {
            this.fkeygen = Builder<StringFPIKeyGenerator>
                .CreateNew()
                .With(x => x.Owner = Pick<string>.RandomItemFrom(new List<string> { "reexjungle", "reexmonkey" }))
                .And(x => x.Authority = Pick<Authority>.RandomItemFrom(new List<Authority> { Authority.ISO, Authority.None, Authority.NonStandard }))
                .And(x => x.Description = "Test iCalendar Service Provider")
                .And(x => x.LanguageId = Pick<string>.RandomItemFrom(new List<string> { "EN", "FR", "DE" }))
                .Build();
        }

        public void TearDown()
        {
            var client = new JsonWebServicesTests(this.baseUri).CreateServiceClient();
            client.Post(new FlushDatabase { Mode = FlushMode.soft });
        }

        private IEnumerable<VEVENT> GenerateEventsOfSize(int n)
        {
            var dgen = new SequentialGenerator<DateTime> { Increment = new DateTime(2014, 06, 15), IncrementDateBy = IncrementDate.Day, Direction = GeneratorDirection.Ascending };

            return Builder<VEVENT>.CreateListOfSize(n)
                .All()
                .With(x => x.Uid = keygen.GetNextKey())
                .And(x => x.Organizer = new ORGANIZER
                {
                    Id = keygen.GetNextKey(),
                    CN = "Caesar",
                    Address = new URI("caesar@apes.je"),
                    Language = new LANGUAGE("en")
                })
                .And(x => x.Location = new LOCATION
                {
                    Text = "Düsseldorf",
                    AlternativeText = new URI("http://www.duesseldorf.de/de/"),
                    Language = new LANGUAGE("de", "DE")
                })
                .And(x => x.Summary = new SUMMARY("Test Meeting"))
                .And(x => x.Start = new DATE_TIME(dgen.Generate()))
                .And(x => x.End = new DATE_TIME(x.Start +
                    new DURATION(0, new RandomGenerator().Next(1, 5), new RandomGenerator().Next(1, 5), new RandomGenerator().Next(0, 59), new RandomGenerator().Next(0, 59))))
                .And(x => x.Status = STATUS.CONFIRMED)
                .And(x => x.Transparency = TRANSP.TRANSPARENT)
                .And(x => x.Classification = CLASS.PUBLIC)
                .Build();
        }

        private IEnumerable<ATTENDEE> GenerateAttendeesOfSize(int n)
        {
            return Builder<ATTENDEE>.CreateListOfSize(n)
                .All()
                .With(x => x.Id = keygen.GetNextKey())
                .And(x => x.CN = new RandomGenerator().Phrase(new RandomGenerator().Next(3, 7)))
                .And(x => x.Address = new URI(string.Format("{0}@apes.je", x.CN.ToLower())))
                .And(x => x.Role = Pick<ROLE>.RandomItemFrom(new List<ROLE> { ROLE.CHAIR, ROLE.NON_PARTICIPANT, ROLE.OPT_PARTICIPANT, ROLE.REQ_PARTICIPANT }))
                .And(x => x.Participation = Pick<PARTSTAT>.RandomItemFrom(new List<PARTSTAT> { PARTSTAT.ACCEPTED, PARTSTAT.COMPLETED, PARTSTAT.DECLINED, PARTSTAT.NEEDS_ACTION, PARTSTAT.TENTATIVE }))
                .And(x => x.CalendarUserType = Pick<CUTYPE>.RandomItemFrom(new List<CUTYPE> { CUTYPE.GROUP, CUTYPE.INDIVIDUAL, CUTYPE.RESOURCE, CUTYPE.ROOM }))
                .And(x => x.Language = new LANGUAGE(Pick<string>.RandomItemFrom(new List<string> { "en", "fr", "de" })))
                .Build();
        }

        private IEnumerable<VEVENT> RandomlyAttendEvents(ref IEnumerable<VEVENT> events, IEnumerable<ATTENDEE> attendees)
        {
            var atts = attendees.ToList();
            return events.Select(x => { x.Attendees.Add(Pick<ATTENDEE>.RandomItemFrom(atts)); return x; });
        }

        public class EventRemoteWebServiceTestsDev1 : EventWebServicesTests
        {
            public EventRemoteWebServiceTestsDev1()
                : base()
            {
                this.baseUri = Properties.Settings.Default.remote_dev1_uri;
            }
        }

        public class EventRemoteWebServiceTestsDev2 : EventWebServicesTests
        {
            public EventRemoteWebServiceTestsDev2()
                : base()
            {
                this.baseUri = Properties.Settings.Default.remote_dev2_uri;
            }
        }

        public class EventLocalWebServiceTests : EventWebServicesTests
        {
            public EventLocalWebServiceTests()
                : base()
            {
                this.baseUri = Properties.Settings.Default.localhost_uri;
            }
        }
    }
}