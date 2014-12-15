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

namespace reexjungle.xcal.test.server.integration.concretes
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

        private void RandomlyAttendEvents(ref IEnumerable<VEVENT> events, IEnumerable<ATTENDEE> attendees)
        {
            var atts = attendees.ToList();
            foreach (var x in events) x.Attendees.Add(Pick<ATTENDEE>.RandomItemFrom(atts));
        }

        [Fact]
        public void MaintainSingleEvent()
        {
            this.TearDown();
            this.Initialize();

            var c1 = new VCALENDAR
            {
                Id = this.keygen.GetNextKey(),
                ProdId = this.fkeygen.GetNextKey(),
                Method = METHOD.PUBLISH
            };

            var events = this.GenerateEventsOfSize(1);
            var e1 = events.First();
            e1.RecurrenceRule = new RECUR
                {
                    Id = keygen.GetNextKey(),
                    FREQ = FREQ.MONTHLY
                };
            e1.Description = new DESCRIPTION("A meeting for coding gurus, nerds, geeks and quants who enjoy programming for others such that the world becomes a better place for all producers and consumers préamble à la gare de nöel für ein wünderschönes Abend mit Bären außerdem.");

            e1.Attendees = this.GenerateAttendeesOfSize(4).ToList();

            var client = new JsonWebServicesTests(this.baseUri).CreateServiceClient();
            client.Post(new AddCalendar { Calendar = c1 });
            client.Post(new AddEvent { CalendarId = c1.Id, Event = e1 });
            var re1 = client.Get(new FindEvent { EventId = e1.Id });
            Assert.Equal(re1.Organizer.CN, "Caesar");
            Assert.Equal(re1.Start, e1.Start);
            Assert.Equal(re1, e1);

            var rcal = client.Get(new FindCalendar { CalendarId = c1.Id });
            e1.Start = new DATE_TIME(new DateTime(2014, 6, 16, 10, 30, 0, 0, DateTimeKind.Utc));
            e1.End = new DATE_TIME(new DateTime(2014, 6, 16, 11, 30, 0, 0, DateTimeKind.Utc));
            e1.Duration = new DURATION(1, 5, 2, 30);
            e1.RecurrenceRule.FREQ = FREQ.WEEKLY;
            e1.Organizer.CN = "Robot Clone";

            client.Put(new UpdateEvent { Event = e1 });
            re1 = client.Get(new FindEvent { EventId = e1.Id });
            Assert.Equal(re1.End, e1.End);
            Assert.Equal(re1.RecurrenceRule.FREQ, FREQ.WEEKLY);
            Assert.Equal(re1.Organizer.CN, "Robot Clone");
            Assert.Equal(re1, e1);

            e1.Attendees.RemoveRange(0, 2);
            client.Put(new UpdateEvent { Event = e1 });
            re1 = client.Get(new FindEvent { EventId = e1.Id });
            Assert.Equal(re1.Attendees.Count, 2);

            client.Patch(new PatchEvent { Transparency = TRANSP.OPAQUE, EventId = e1.Id });
            var patched = client.Get(new FindEvent { EventId = e1.Id });
            Assert.Equal(patched.Transparency, TRANSP.OPAQUE);

            client.Delete(new DeleteEvent { EventId = e1.Id });
            var deleted = client.Get(new FindEvent { EventId = e1.Id });
            Assert.Equal(deleted, null);
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