using FizzWare.NBuilder;
using reexjungle.foundation.essentials.concretes;
using reexjungle.foundation.essentials.contracts;
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
        protected GuidKeyGenerator keygen = null;
        protected StringFPIKeyGenerator fkeygen = null;
        protected JsonWebServiceTestFactory factory = null;

        public EventWebServicesTests()
        {
            this.factory = new JsonWebServiceTestFactory(null);
            this.keygen = new GuidKeyGenerator();
            this.fkeygen = new StringFPIKeyGenerator();
        }

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
            this.factory.GetClient().Post(new FlushDatabase { Mode = FlushMode.soft });
        }

        protected IEnumerable<VEVENT> GenerateEventsOfSize(int n)
        {
            var dgen = new SequentialGenerator<DateTime> { IncrementDateBy = IncrementDate.Day, Direction = GeneratorDirection.Ascending };
            dgen.StartingWith(new DateTime(2014, 06, 15));

            return Builder<VEVENT>.CreateListOfSize(n)
                .All()
                .With(x => x.Uid = keygen.GetNextKey())
                .And(x => x.Organizer = new ORGANIZER
                {
                    Id = keygen.GetNextKey(),
                    CN = Pick<string>.RandomItemFrom(new string[] { "Caesar", "Koba", "Cornelia", "Blue Eyes", "Grey", "Ash" }),
                    Address = new URI("organizer@apes.ju"),
                    Language = new LANGUAGE("en")
                })
                .And(x => x.Location = new LOCATION
                {
                    Text = "Düsseldorf",
                    AlternativeText = new URI("http://www.duesseldorf.de/de/"),
                    Language = new LANGUAGE("de", "DE")
                })
                .And(x => x.Summary = new SUMMARY("Test Meeting"))
                .And(x => x.Description = new DESCRIPTION("A meeting for coding gurus, nerds, geeks and quants who enjoy programming for others such that the world becomes a better place for all qui sonts presentes à la Gare de Nöel für ein außerordentliches wünderschönes Abend mit Bären aaaaaaaaaaaaa."))
                .And(x => x.Start = new DATE_TIME(dgen.Generate()))
                .And(x => x.Duration = new DURATION(0,
                    new RandomGenerator().Next(1, 7),
                    new RandomGenerator().Next(1, 12),
                    new RandomGenerator().Next(0, 59),
                    new RandomGenerator().Next(0, 59)))
                .And(x => x.Status = STATUS.CONFIRMED)
                .And(x => x.Transparency = TRANSP.TRANSPARENT)
                .And(x => x.Classification = CLASS.PUBLIC)
                .Build();
        }

        protected IEnumerable<ATTENDEE> GenerateAttendeesOfSize(int n)
        {
            return Builder<ATTENDEE>.CreateListOfSize(n)
                .All()
                .With(x => x.Id = keygen.GetNextKey())
                .And(x => x.CN = Pick<string>.RandomItemFrom(new string[] { "Caesar", "Koba", "Cornelia", "Blue Eyes", "Grey", "Ash" }))
                .And(x => x.Address = new URI(string.Format("{0}@apes.je", x.CN.Replace(" ", ".").ToLower())))
                .And(x => x.Role = Pick<ROLE>.RandomItemFrom(new List<ROLE> { ROLE.CHAIR, ROLE.NON_PARTICIPANT, ROLE.OPT_PARTICIPANT, ROLE.REQ_PARTICIPANT }))
                .And(x => x.Participation = Pick<PARTSTAT>.RandomItemFrom(new List<PARTSTAT> { PARTSTAT.ACCEPTED, PARTSTAT.COMPLETED, PARTSTAT.DECLINED, PARTSTAT.NEEDS_ACTION, PARTSTAT.TENTATIVE }))
                .And(x => x.CalendarUserType = Pick<CUTYPE>.RandomItemFrom(new List<CUTYPE> { CUTYPE.GROUP, CUTYPE.INDIVIDUAL, CUTYPE.RESOURCE, CUTYPE.ROOM }))
                .And(x => x.Language = new LANGUAGE(Pick<string>.RandomItemFrom(new List<string> { "en", "fr", "de" })))
                .Build();
        }

        protected void RandomlyAttendEvents(ref IEnumerable<VEVENT> events, IEnumerable<ATTENDEE> attendees)
        {
            var atts = attendees.ToList(); var max = atts.Count;
            foreach (var x in events) x.Attendees.AddRange(Pick<ATTENDEE>.UniqueRandomList(With.Between(1, max)).From(atts));
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

            e1.Attendees = this.GenerateAttendeesOfSize(4).ToList();

            var client = this.factory.GetClient();
            client.Post(new AddCalendar { Calendar = c1 });
            client.Post(new AddEvent { CalendarId = c1.Id, Event = e1 });
            var re1 = client.Get(new FindEvent { EventId = e1.Id });
            Assert.Equal(re1.Organizer.CN, e1.Organizer.CN);
            Assert.Equal(re1.Start, e1.Start);
            Assert.Equal(re1, e1);

            var rcal = client.Get(new FindCalendar { CalendarId = c1.Id });
            e1.Start = new DATE_TIME(new DateTime(2014, 6, 16, 10, 30, 0, 0, DateTimeKind.Utc));
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

        [Fact]
        public void MaintainMultipleEvents()
        {
            this.TearDown();
            this.Initialize();

            var events = this.GenerateEventsOfSize(5);
            var attendees = this.GenerateAttendeesOfSize(10);
            this.RandomlyAttendEvents(ref events, attendees);

            var calendar = new VCALENDAR
            {
                Id = this.keygen.GetNextKey(),
                ProdId = this.fkeygen.GetNextKey(),
                Method = METHOD.PUBLISH
            };

            var keys = events.Select(x => x.Id).ToList();

            var client = this.factory.GetClient();

            client.Post(new AddCalendar { Calendar = calendar });
            client.Post(new AddEvents { CalendarId = calendar.Id, Events = events.ToList() });
            var r1 = client.Get(new GetEvents { Page = 1, Size = 100 });
            Assert.Equal(r1.Count, events.Count());
            var e1 = events.Where(x => x.Organizer == events.ElementAt(0).Organizer).First();
            var p1 = r1.Where(x => x.Organizer == events.ElementAt(0).Organizer).First();
            var e2 = events.Where(x => x.Organizer == events.ElementAt(1).Organizer).First();
            var p2 = r1.Where(x => x.Organizer == events.ElementAt(1).Organizer).First();

            e1.Start = new DATE_TIME(new DateTime(2014, 6, 16, 10, 30, 0, 0, DateTimeKind.Utc));
            e1.Duration = new DURATION(0, 1, 2, 30);
            e1.Priority = new PRIORITY(PRIORITYLEVEL.MEDIUM);
            e1.Description.Text = string.Format("{0} >>Updated<<", e1.Description.Text);
            var len = e1.Description.Text.Length;

            e2.Start = new DATE_TIME(new DateTime(2014, 6, 16, 10, 30, 0, 0, DateTimeKind.Local));
            e2.Duration = new DURATION(0, 1, 10, 00);
            e2.Priority = new PRIORITY(PRIORITYLEVEL.LOW);
            e2.Description.Text = string.Format("{0} >>Updated<<", e2.Description.Text);

            client.Put(new UpdateEvents { Events = new List<VEVENT> { e1, e2 } });

            var r2 = client.Post(new FindEvents { EventIds = new List<string> { e1.Id, e2.Id } });
            var u1 = r2.Where(x => x == e1).First();
            var u2 = r2.Where(x => x == e2).First();

            Assert.Equal(u1.End, e1.End);
            Assert.Equal(u2.Duration, e2.Duration);

            Assert.Equal(u1.Priority.Level, PRIORITYLEVEL.MEDIUM);
            Assert.Equal(u2.Description, e2.Description);
            Assert.Equal(u1, e1);
            Assert.Equal(u2, e2);

            e1.Attendees.RemoveRange(0, 1);
            e2.Attendees.RemoveRange(0, 1);

            var te1 = e1.Attendees.Count;
            var te2 = e2.Attendees.Count;

            client.Put(new UpdateEvents { Events = new List<VEVENT> { e1, e2 } });
            r2 = client.Post(new FindEvents { EventIds = r2.Select(x => x.Id).ToList() });

            u1 = r2.Where(x => x.Id == e1.Id).First();
            u2 = r2.Where(x => x.Id == e2.Id).First();

            Assert.Equal(u1.Attendees.Count, te1);
            Assert.Equal(u2.Attendees.Count, te2);
            u1.Organizer.Language = new LANGUAGE("fr");

            client.Patch(new PatchEvents
            {
                EventIds = keys,
                Transparency = TRANSP.OPAQUE,
                Classification = CLASS.CONFIDENTIAL,
                Priority = new PRIORITY(PRIORITYLEVEL.HIGH),
                Organizer = u1.Organizer,
                Attendees = u1.Attendees
            });

            var patched = client.Post(new FindEvents { EventIds = keys });
            foreach (var result in patched)
            {
                Assert.Equal(result.Organizer.Language.Tag, "fr");
                Assert.Equal(result.Transparency, TRANSP.OPAQUE);
                Assert.Equal(result.Classification, CLASS.CONFIDENTIAL);
                Assert.Equal(result.Priority, new PRIORITY(PRIORITYLEVEL.HIGH));
                Assert.Equal(result.Attendees.Count, u1.Attendees.Count);
                Assert.Equal(result.Attendees.AreDuplicatesOf(u1.Attendees), true);
            }

            client.Delete(new DeleteEvents { EventIds = keys });
            var deleted = client.Post(new FindEvents { EventIds = keys });
            Assert.Equal(deleted.Count, 0);
        }
    }

    public class EventRemoteWebServiceTestsDev1 : EventWebServicesTests
    {
        public EventRemoteWebServiceTestsDev1()
            : base()
        {
            this.factory.BaseUri = Properties.Settings.Default.remote_dev1_uri;
        }
    }

    public class EventRemoteWebServiceTestsDev2 : EventWebServicesTests
    {
        public EventRemoteWebServiceTestsDev2()
            : base()
        {
            this.factory.BaseUri = Properties.Settings.Default.remote_dev2_uri;
        }
    }

    public class EventLocalWebServiceTests : EventWebServicesTests
    {
        public EventLocalWebServiceTests()
            : base()
        {
            this.factory.BaseUri = Properties.Settings.Default.localhost_uri;
        }
    }
}