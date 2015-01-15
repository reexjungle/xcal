using FizzWare.NBuilder;
using reexjungle.foundation.essentials.concretes;
using reexjungle.infrastructure.operations.concretes;
using reexjungle.infrastructure.operations.contracts;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.domain.operations;
using reexjungle.xcal.test.server.integration.contracts;
using reexjungle.xcal.test.units.concretes;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace reexjungle.xcal.test.server.integration.concretes.web
{
    public abstract class EventWebServicesTests : IWebServiceIntegrationTests
    {
        protected CalendarUnitTests ctests = new CalendarUnitTests();
        protected EventUnitTests evtests = new EventUnitTests();
        protected PropertiesUnitTests ptests = new PropertiesUnitTests();
        protected AlarmUnitTests altests = new AlarmUnitTests();
        protected JsonWebServiceTestFactory factory = null;

        public EventWebServicesTests()
        {
            this.factory = new JsonWebServiceTestFactory(null);
        }

        public void Initialize()
        {
            this.ctests.FPIKeyGen = Builder<StringFPIKeyGenerator>
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

        [Fact]
        public void MaintainSingleEvent()
        {
            this.TearDown();
            this.Initialize();

            var c1 = this.ctests.GenerateCalendarsOfSize(1).FirstOrDefault();

            var events = this.evtests.GenerateEventsOfSize(1);
            var e1 = events.FirstOrDefault();
            e1.RecurrenceRule = new RECUR
                {
                    Id = this.evtests.KeyGen.GetNextKey(),
                    FREQ = FREQ.MONTHLY
                };

            e1.Attendees = this.ptests.GenerateAttendeesOfSize(4).ToList();

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

            var calendar = this.ctests.GenerateCalendarsOfSize(1).FirstOrDefault();
            var events = this.evtests.GenerateEventsOfSize(5);
            this.evtests.RandomlyAttendEvents(ref events, this.ptests.GenerateAttendeesOfSize(10));
            var keys = events.Select(x => x.Id).ToList();

            var client = this.factory.GetClient();

            client.Post(new AddCalendar { Calendar = calendar });
            client.Post(new AddEvents { CalendarId = calendar.Id, Events = events.ToList() });
            var r1 = client.Get(new GetEvents { Page = 1, Size = 100 });
            Assert.Equal(r1.Count, events.Count());
            var e1 = events.Where(x => x.Organizer == events.ElementAt(0).Organizer).FirstOrDefault();
            var p1 = r1.Where(x => x.Organizer == events.ElementAt(0).Organizer).FirstOrDefault();
            var e2 = events.Where(x => x.Organizer == events.ElementAt(1).Organizer).FirstOrDefault();
            var p2 = r1.Where(x => x.Organizer == events.ElementAt(1).Organizer).FirstOrDefault();

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
            var u1 = r2.Where(x => x == e1).FirstOrDefault();
            var u2 = r2.Where(x => x == e2).FirstOrDefault();

            var u1str = u1.ToString();
            var u2str = u2.ToString();

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

            u1 = r2.Where(x => x.Id == e1.Id).FirstOrDefault();
            u2 = r2.Where(x => x.Id == e2.Id).FirstOrDefault();

            Assert.Equal(u1.Attendees.Count, te1);
            Assert.Equal(u2.Attendees.Count, te2);
            u1.Organizer.Language = new LANGUAGE("fr");

            keys = client.Get(new GetEvents { Page = 1, Size = int.MaxValue }).Select(x => x.Id).ToList();
            client.Patch(new PatchEvents
            {
                EventIds = keys,
                Transparency = TRANSP.OPAQUE,
                Classification = CLASS.CONFIDENTIAL,
                Priority = new PRIORITY(PRIORITYLEVEL.HIGH),
                Organizer = u1.Organizer,
                Attendees = u1.Attendees,
            });

            var patched = client.Post(new FindEvents { EventIds = keys });
            foreach (var result in patched)
            {
                Assert.Equal(result.Organizer.Language.Tag, "fr");
                Assert.Equal(result.Transparency, TRANSP.OPAQUE);
                Assert.Equal(result.Classification, CLASS.CONFIDENTIAL);
                Assert.Equal(result.Priority, new PRIORITY(PRIORITYLEVEL.HIGH));
                Assert.Equal(result.Attendees.AreDuplicatesOf(u1.Attendees), true);
            }

            client.Patch(new PatchEvents
            {
                Start = u1.Start,
                End = u1.End,
                Duration = u1.Duration,
                Categories = u1.Categories,
                Description = new DESCRIPTION("Patched again!!!"),
                Transparency = TRANSP.TRANSPARENT,
                Classification = CLASS.PRIVATE,
                Priority = new PRIORITY(PRIORITYLEVEL.MEDIUM),
                Organizer = u1.Organizer,
                Attendees = u1.Attendees
            });

            patched = client.Get(new GetEvents { Page = 1, Size = int.MaxValue });
            foreach (var result in patched)
            {
                Assert.Equal(result.Transparency, TRANSP.TRANSPARENT);
                Assert.Equal(result.Classification, CLASS.PRIVATE);
                Assert.Equal(result.Priority, new PRIORITY(PRIORITYLEVEL.MEDIUM));
                Assert.Equal(result.Attendees.Count, u1.Attendees.Count());
                Assert.Equal(result.Attendees.AreDuplicatesOf(u1.Attendees), true);
            }

            client.Delete(new DeleteEvents { EventIds = keys });
            var deleted = client.Post(new FindEvents { EventIds = keys });
            Assert.Equal(deleted.Count, 0);
        }

        [Fact]
        public void MaintainSingleEventWithAlarms()
        {
            this.TearDown();
            this.Initialize();

            var calendar = this.ctests.GenerateCalendarsOfSize(1).FirstOrDefault();

            var events = this.evtests.GenerateEventsOfSize(1);
            this.evtests.RandomlyAttendEvents(ref events, this.ptests.GenerateAttendeesOfSize(10));
            var e1 = events.FirstOrDefault();

            e1.AudioAlarms = this.altests.GenerateAudioAlarmsOfSize(5).ToList();
            e1.DisplayAlarms = this.altests.GenerateDisplayAlarmsOfSize(5).ToList();
            e1.EmailAlarms = this.altests.GenerateEmailAlarmsOfSize(3).ToList();

            var client = this.factory.GetClient();
            client.Post(new AddCalendar { Calendar = calendar });
            client.Post(new AddEvent { CalendarId = calendar.Id, Event = e1 });

            var re1 = client.Get(new FindEvent { EventId = e1.Id });
            Assert.Equal(re1, e1);
            Assert.Equal(re1.AudioAlarms.AreDuplicatesOf(e1.AudioAlarms), true);
            Assert.NotEqual(re1.EmailAlarms.AreDuplicatesOf(e1.EmailAlarms), false);

            ////remove email alarm and update
            e1.AudioAlarms.FirstOrDefault().AttachmentUri.FormatType = new FMTTYPE("file", "video");
            var ealarm = e1.EmailAlarms.FirstOrDefault();
            e1.EmailAlarms.Clear();

            client.Put(new UpdateEvent { Event = e1 });
            re1 = client.Get(new FindEvent { EventId = e1.Id });
            Assert.Equal(re1.EmailAlarms.Count, 0);

            //reinsert some alarms and update
            e1.EmailAlarms.AddRange(new EMAIL_ALARM[] { ealarm });
            client.Put(new UpdateEvent { Event = e1 });
            re1 = client.Get(new FindEvent { EventId = e1.Id });
            Assert.Equal(re1.EmailAlarms.Count, 1);

            e1.EmailAlarms.FirstOrDefault().Description.Text = "This is a patched alarm";
            client.Patch(new PatchEvent { EmailAlarms = e1.EmailAlarms, EventId = e1.Id });
            var patched = client.Get(new FindEvent { EventId = e1.Id });
            Assert.Equal(patched.EmailAlarms.FirstOrDefault().Description.Text, "This is a patched alarm");

            client.Delete(new DeleteEvent { EventId = e1.Id });
            var deleted = client.Get(new FindEvent { EventId = e1.Id });
            Assert.Equal(deleted, null);
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