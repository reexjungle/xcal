using System;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.extensions;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.operations.concretes.cached;
using reexjungle.xcal.service.operations.concretes.live;
using reexjungle.xcal.tests.concretes.factories;
using reexjungle.xcal.tests.concretes.services;
using reexjungle.xcal.tests.contracts.factories;
using reexjungle.xcal.tests.contracts.services;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.foundation.contracts;
using reexjungle.xmisc.infrastructure.concretes.operations;
using reexjungle.xmisc.infrastructure.contracts;
using ServiceStack.ServiceClient.Web;
using Xunit;

namespace reexjungle.xcal.tests.concretes.integration
{
    public abstract class EventWebServicesTests 
    {
        protected readonly IKeyGenerator<Guid> GuidKeyGenerator;
        protected readonly ICalendarFactory CalendarFactory;
        protected readonly IEventFactory EventFactory;
        protected readonly IAlarmFactory AlarmFactory;
        protected readonly IPropertiesFactory PropertiesFactory;
        protected readonly IServiceClientFactory ServiceClientFactory;
        protected readonly IEventTestService TestService;

        protected EventWebServicesTests(string baseUri)
        {
            if (string.IsNullOrWhiteSpace(baseUri)) throw new ArgumentNullException("baseUri");
            if (!Uri.IsWellFormedUriString(baseUri, UriKind.RelativeOrAbsolute)) throw new FormatException("baseUri");

            var rndGenerator = new RandomGenerator();
            GuidKeyGenerator = new SequentialGuidKeyGenerator();
            var fpiKeyGenerator = new FpiKeyGenerator(
                new ContentGenerator<ApprovalStatus>(() => Pick<ApprovalStatus>.RandomItemFrom(new[]
                {
                    ApprovalStatus.Informal, 
                    ApprovalStatus.None
                })),
                new ContentGenerator<string>(() => Pick<string>.RandomItemFrom(new[]
                {
                    "RXJG",
                    "GOGL",
                    "MSFT",
                    "YHOO"
                })),

                new ContentGenerator<string>(() => Pick<string>.RandomItemFrom(new[]
                {
                    "DTD",
                    "XSL",
                    "XML",
                    "JSON"
                })), 
                
                new ContentGenerator<string>(() => rndGenerator.Phrase(10)),
                new ContentGenerator<string>(() => Pick<string>.RandomItemFrom(new[]
                {
                    "EN",
                    "FR",
                    "DE",
                    "ES",
                    "IT",
                    "PL",
                    "RO"
                })));

            var sharedFactory = new SharedFactory();
            var valuesFactory = new ValuesFactory(GuidKeyGenerator);
            var parametersFactory = new ParametersFactory(valuesFactory, sharedFactory);
            PropertiesFactory = new PropertiesFactory(GuidKeyGenerator, valuesFactory, parametersFactory, sharedFactory);
            AlarmFactory = new AlarmFactory(GuidKeyGenerator, PropertiesFactory, valuesFactory);

            EventFactory = new EventFactory(GuidKeyGenerator, AlarmFactory, PropertiesFactory, valuesFactory);
            CalendarFactory = new CalendarFactory(GuidKeyGenerator, fpiKeyGenerator);

            ServiceClientFactory = new ServiceClientFactory();
            ServiceClientFactory.Register(() => new JsonServiceClient(baseUri));
            ServiceClientFactory.Register(() => new JsvServiceClient(baseUri));
            ServiceClientFactory.Register(() => new XmlServiceClient(baseUri));

            TestService = new EventTestService();
        }

        public void TearDown()
        {
            var client =  ServiceClientFactory.GetClient<JsonServiceClient>();
            client.Post(new FlushDatabase
            {
                Force = false
            });
        }

        [Fact]
        public void MaintainSingleEvent()
        {
            TearDown();

            var c1 = CalendarFactory.Create();
            var e1 = EventFactory.Create();
            e1.RecurrenceRule.FREQ = FREQ.WEEKLY;
            e1.Attendees = PropertiesFactory.CreateAttendees(4).ToList();
           
            var client = ServiceClientFactory.GetClient<JsonServiceClient>();
            client.Post(new AddCalendar { Calendar = c1 });
            client.Post(new AddEvent { CalendarId = c1.Id, Event = e1 });
            
            var re1 = client.Get(new FindEvent { EventId = e1.Id });
            Assert.Equal(re1.Organizer.CN, e1.Organizer.CN);
            Assert.Equal(re1.Start, e1.Start);
            Assert.Equal(re1, e1);

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

            client.Post(new PatchEvent { Transparency = TRANSP.OPAQUE, EventId = e1.Id });
            var patched = client.Get(new FindEvent { EventId = e1.Id });
            Assert.Equal(patched.Transparency, TRANSP.OPAQUE);

            client.Delete(new DeleteEvent { EventId = e1.Id });
            var deleted = client.Get(new FindEvent { EventId = e1.Id });
            Assert.Equal(deleted, null);
        }

        [Fact]
        public void MaintainSingleEventCached()
        {
            TearDown();

            var c1 = CalendarFactory.Create();
            var e1 = EventFactory.Create();
            e1.Attendees = PropertiesFactory.CreateAttendees(4).ToList();

            var client = ServiceClientFactory.GetClient<JsonServiceClient>();
            client.Post(new AddCalendar { Calendar = c1 });
            client.Post(new AddEvent { CalendarId = c1.Id, Event = e1 });
            var re1 = client.Get(new FindEventCached { EventId = e1.Id });
            Assert.Equal(re1.Organizer.CN, e1.Organizer.CN);
            Assert.Equal(re1.Start, e1.Start);
            Assert.Equal(re1, e1);

            e1.Start = new DATE_TIME(new DateTime(2014, 6, 16, 10, 30, 0, 0, DateTimeKind.Utc));
            e1.Duration = new DURATION(1, 5, 2, 30);
            e1.RecurrenceRule.FREQ = FREQ.WEEKLY;
            e1.Organizer.CN = "Robot Clone";

            client.Put(new UpdateEvent { Event = e1 });
            re1 = client.Get(new FindEventCached { EventId = e1.Id });
            Assert.Equal(re1.End, e1.End);
            Assert.Equal(re1.RecurrenceRule.FREQ, FREQ.WEEKLY);
            Assert.Equal(re1.Organizer.CN, "Robot Clone");
            Assert.Equal(re1, e1);

            e1.Attendees.RemoveRange(0, 2);
            client.Put(new UpdateEvent { Event = e1 });
            re1 = client.Get(new FindEventCached { EventId = e1.Id });
            Assert.Equal(re1.Attendees.Count, 2);

            client.Post(new PatchEvent { Transparency = TRANSP.OPAQUE, EventId = e1.Id });
            var patched = client.Get(new FindEventCached { EventId = e1.Id });
            Assert.Equal(patched.Transparency, TRANSP.OPAQUE);

            client.Delete(new DeleteEvent { EventId = e1.Id });
            var deleted = client.Get(new FindEventCached { EventId = e1.Id });
            Assert.Equal(deleted, VEVENT.Empty);
        }

        [Fact]
        public void MaintainMultipleEvents()
        {
            TearDown();

            var calendar = CalendarFactory.Create();
            var events = EventFactory.Create(5).ToList();
            events[0].Attendees = PropertiesFactory.CreateAttendees(4).ToList();
            events[1].Attendees = PropertiesFactory.CreateAttendees(5).ToList();

            var client = ServiceClientFactory.GetClient<JsonServiceClient>();

            client.Post(new AddCalendar { Calendar = calendar });
            client.Post(new AddEvents { CalendarId = calendar.Id, Events = events.ToList() });
            var r1 = client.Get(new GetEvents { Page = 1, Size = 100 });
            Assert.Equal(r1.Count, events.Count());

            var e1 = events[0];
            var e2 = events[1];

            e1.Start = new DATE_TIME(new DateTime(2014, 6, 16, 10, 30, 0, 0, DateTimeKind.Utc));
            e1.Duration = new DURATION(0, 1, 2, 30);
            e1.Priority = new PRIORITY(PRIORITYLEVEL.MEDIUM);
            e1.Description.Text = string.Format("{0} >>Updated<<", e1.Description.Text);
            e1.RecurrenceRule = new RECUR
            {
                Id = Guid.NewGuid(),
                FREQ = FREQ.DAILY,
                INTERVAL = 1,
                UNTIL = new DATE_TIME(2014, 06, 30, 06, 0, 0, TimeType.Utc),
                BYHOUR = new List<uint> { 9, 12, 15, 18, 21 },
                BYMINUTE = new List<uint> { 5, 10, 15, 20, 25 },
                BYSETPOS = new List<int> { 2, -2 }
            };

            var Rx = e1.GenerateRecurrences(GuidKeyGenerator);

            e2.Start = new DATE_TIME(new DateTime(2014, 6, 16, 10, 30, 0, 0, DateTimeKind.Local));
            e2.Duration = new DURATION(0, 1, 10, 00);
            e2.Priority = new PRIORITY(PRIORITYLEVEL.LOW);
            e2.Description.Text = string.Format("{0} >>Updated<<", e2.Description.Text);

            client.Put(new UpdateEvents { Events = new List<VEVENT> { e1, e2 }.Concat(Rx).ToList() });

            var r2 = client.Post(new FindEvents { EventIds = new List<Guid> { e1.Id, e2.Id } });
            var u1 = r2.First(x => x == e1);
            var u2 = r2.First(x => x == e2);

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

            u1 = r2.First(x => x.Id == e1.Id);
            u2 = r2.First(x => x.Id == e2.Id);

            Assert.Equal(u1.Attendees.Count, te1);
            Assert.Equal(u2.Attendees.Count, te2);
            u1.Organizer.Language = new LANGUAGE("fr");

            var keys = client.Get(new GetEvents { Page = 1, Size = int.MaxValue }).Select(x => x.Id).ToList();
            client.Post(new PatchEvents
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
                Assert.Equal(result.Attendees.IsEquivalentOf(u1.Attendees), true);
            }

            client.Post(new PatchEvents
            {
                Start = u1.Start,
                End = u1.End,
                Duration = u1.Duration,
                Categories = u1.Categories,
                Description = new DESCRIPTION("Patched again!!!"),
                Transparency = TRANSP.TRANSPARENT,
                Classification = CLASS.PRIVATE,
                Priority = new PRIORITY(PRIORITYLEVEL.MEDIUM),
                Organizer = u2.Organizer,
                Attendees = u2.Attendees
            });

            patched = client.Get(new GetEvents { Page = 1, Size = int.MaxValue });
            foreach (var patch in patched)
            {
                Assert.Equal(patch.Organizer, u2.Organizer);
                Assert.Equal(patch.Duration, u1.Duration);
                Assert.Equal(patch.Categories, u1.Categories);
                Assert.Equal(patch.Transparency, TRANSP.TRANSPARENT);
                Assert.Equal(patch.Classification, CLASS.PRIVATE);
                Assert.Equal(patch.Priority, new PRIORITY(PRIORITYLEVEL.MEDIUM));
                Assert.Equal(patch.Attendees.Count, u2.Attendees.Count());
                Assert.Equal(patch.Attendees.IsEquivalentOf(u2.Attendees), true);
            }


            var okeys = client.Get(new GetEventKeys {  Page = 1, Size = int.MaxValue });

            Assert.Equal(okeys.IsEquivalentOf(keys), true);

            client.Post(new DeleteEvents {  EventIds = keys });
            
            var deleted = client.Post(new FindEvents { EventIds = keys });
           
            Assert.Equal(deleted.Count, 0);
        }

        [Fact]
        public void MaintainMultipleEventsCached()
        {
            TearDown();

            var calendar = CalendarFactory.Create();
            var events = EventFactory.Create(5).ToList();
            events[0].Attendees = PropertiesFactory.CreateAttendees(4).ToList();
            events[1].Attendees = PropertiesFactory.CreateAttendees(5).ToList();

            var client = ServiceClientFactory.GetClient<JsonServiceClient>();

            client.Post(new AddCalendar { Calendar = calendar });
            client.Post(new AddEvents { CalendarId = calendar.Id, Events = events.ToList() });
            var r1 = client.Get(new GetEventsCached { Page = 1, Size = 100 });
            Assert.Equal(r1.Count, events.Count());

            var e1 = events[0];
            var e2 = events[1];

            e1.Start = new DATE_TIME(new DateTime(2014, 6, 16, 10, 30, 0, 0, DateTimeKind.Utc));
            e1.Duration = new DURATION(0, 1, 2, 30);
            e1.Priority = new PRIORITY(PRIORITYLEVEL.MEDIUM);
            e1.Description.Text = string.Format("{0} >>Updated<<", e1.Description.Text);

            e2.Start = new DATE_TIME(new DateTime(2014, 6, 16, 10, 30, 0, 0, DateTimeKind.Local));
            e2.Duration = new DURATION(0, 1, 10);
            e2.Priority = new PRIORITY(PRIORITYLEVEL.LOW);
            e2.Description.Text = string.Format("{0} >>Updated<<", e2.Description.Text);

            client.Put(new UpdateEvents { Events = new List<VEVENT> { e1, e2 } });

            var r2 = client.Post(new FindEventsCached { EventIds = new List<Guid> { e1.Id, e2.Id } });
            var u1 = r2.First(x => x == e1);
            var u2 = r2.First(x => x == e2);

            Assert.Equal(u1.End, e1.End);
            Assert.Equal(u2.Duration, e2.Duration);

            Assert.Equal(u1.Priority.Level, PRIORITYLEVEL.MEDIUM);
            Assert.Equal(u2.Description, e2.Description);
            Assert.Equal(u1, e1);
            Assert.Equal(u2, e2);

            e1.Attendees.RemoveRange(0, 1); //3
            e2.Attendees.RemoveRange(0, 1); //4

            client.Put(new UpdateEvents { Events = new List<VEVENT> { e1, e2 } });
            r2 = client.Post(new FindEventsCached { EventIds = new List<Guid> { e1.Id, e2.Id } });

            u1 = r2.First(x => x.Id == e1.Id);
            u2 = r2.First(x => x.Id == e2.Id);

            Assert.Equal(u1.Attendees.Count, e1.Attendees.Count);
            Assert.Equal(u2.Attendees.Count, e2.Attendees.Count);
            u1.Organizer.Language = new LANGUAGE("fr");

            var keys = client.Get(new GetEventKeysCached { Page = 1, Size = int.MaxValue });
            client.Post(new PatchEvents
            {
                EventIds = keys,
                Transparency = TRANSP.OPAQUE,
                Classification = CLASS.CONFIDENTIAL,
                Priority = new PRIORITY(PRIORITYLEVEL.HIGH),
                Organizer = u1.Organizer,
                Attendees = u1.Attendees,
            });

            var patched = client.Post(new FindEventsCached { EventIds = keys });
            foreach (var result in patched)
            {
                Assert.Equal(result.Organizer.Language.Tag, "fr");
                Assert.Equal(result.Transparency, TRANSP.OPAQUE);
                Assert.Equal(result.Classification, CLASS.CONFIDENTIAL);
                Assert.Equal(result.Priority, new PRIORITY(PRIORITYLEVEL.HIGH));
                Assert.Equal(result.Attendees.IsEquivalentOf(u1.Attendees), true);
            }

            client.Post(new PatchEvents
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
                Attendees = u2.Attendees
            });

            patched = client.Get(new GetEventsCached
            {
                Page = 1,
                Size = 3
            });

            foreach (var patch in patched)
            {
                Assert.Equal(patch.Organizer, u1.Organizer);
                Assert.Equal(patch.Duration, u1.Duration);
                Assert.Equal(patch.Categories, u1.Categories);
                Assert.Equal(patch.Transparency, TRANSP.TRANSPARENT);
                Assert.Equal(patch.Classification, CLASS.PRIVATE);
                Assert.Equal(patch.Priority, new PRIORITY(PRIORITYLEVEL.MEDIUM));
                Assert.Equal(patch.Attendees.Count, u2.Attendees.Count());
                Assert.Equal(patch.Attendees.IsEquivalentOf(u2.Attendees), true);
            }


            client.Post(new DeleteEvents { EventIds = keys });
            var deleted = client.Post(new FindEventsCached { EventIds = keys });
            Assert.Equal(deleted.Count, 0);
        }

        [Fact]
        public void MaintainSingleEventWithAlarms()
        {
            TearDown();

            var calendar = CalendarFactory.Create();

            var @event = EventFactory.Create();
            TestService.RandomlyAttend(@event, PropertiesFactory.CreateAttendees(10));
            var e1 = @event;

            e1.AudioAlarms = AlarmFactory.CreateAudioAlarms(5).ToList();
            e1.DisplayAlarms = AlarmFactory.CreateDisplayAlarms(5).ToList();
            e1.EmailAlarms = AlarmFactory.CreateEmailAlarms(3).ToList();

            var client = ServiceClientFactory.GetClient<JsonServiceClient>();
            client.Post(new AddCalendar { Calendar = calendar });
            client.Post(new AddEvent { CalendarId = calendar.Id, Event = e1 });

            var re1 = client.Get(new FindEvent { EventId = e1.Id });
            Assert.Equal(re1, e1);
            Assert.Equal(re1.AudioAlarms.IsEquivalentOf(e1.AudioAlarms), true);
            Assert.NotEqual(re1.EmailAlarms.IsEquivalentOf(e1.EmailAlarms), false);

            ////remove email alarm and update
            e1.AudioAlarms.First().AttachmentUri.FormatType = new FMTTYPE("file", "video");
            var ealarm = e1.EmailAlarms.FirstOrDefault();
            e1.EmailAlarms.Clear();

            client.Put(new UpdateEvent { Event = e1 });
            re1 = client.Get(new FindEvent { EventId = e1.Id });
            Assert.Equal(re1.EmailAlarms.Count, 0);

            //reinsert some alarms and update
            e1.EmailAlarms.AddRange(new[] { ealarm });
            client.Put(new UpdateEvent { Event = e1 });
            re1 = client.Get(new FindEvent { EventId = e1.Id });
            Assert.Equal(re1.EmailAlarms.Count, 1);

            e1.EmailAlarms.First().Description.Text = "This is a patched alarm";
            client.Post(new PatchEvent { EmailAlarms = e1.EmailAlarms, EventId = e1.Id });
            var patched = client.Get(new FindEvent { EventId = e1.Id });
            Assert.Equal(patched.EmailAlarms.First().Description.Text, "This is a patched alarm");

            client.Delete(new DeleteEvent { EventId = e1.Id });
            var deleted = client.Get(new FindEvent { EventId = e1.Id });
            Assert.Equal(deleted, null);
        }

        [Fact]
        public void MaintainSingleEventWithAlarmsCached()
        {
            TearDown();

            var calendar = CalendarFactory.Create();
            var @event = EventFactory.Create();

            var e1 = @event;
            e1.Attendees = PropertiesFactory.CreateAttendees(10).ToList();
            e1.AudioAlarms = AlarmFactory.CreateAudioAlarms(5).ToList();
            e1.DisplayAlarms = AlarmFactory.CreateDisplayAlarms(5).ToList();
            e1.EmailAlarms = AlarmFactory.CreateEmailAlarms(3).ToList();

            var client = ServiceClientFactory.GetClient<JsonServiceClient>();
            client.Post(new AddCalendar { Calendar = calendar });
            client.Post(new AddEvent { CalendarId = calendar.Id, Event = e1 });

            var re1 = client.Get(new FindEventCached { EventId = e1.Id });
            Assert.Equal(re1, e1);
            Assert.Equal(re1.AudioAlarms.IsEquivalentOf(e1.AudioAlarms), true);
            Assert.NotEqual(re1.EmailAlarms.IsEquivalentOf(e1.EmailAlarms), false);

            ////remove email alarm and update
            e1.AudioAlarms.First().AttachmentUri.FormatType = new FMTTYPE("file", "video");
            var ealarm = e1.EmailAlarms.FirstOrDefault();
            e1.EmailAlarms.Clear();

            client.Put(new UpdateEvent { Event = e1 });
            re1 = client.Get(new FindEventCached { EventId = e1.Id });
            Assert.Equal(re1.EmailAlarms.Count, 0);

            //reinsert some alarms and update
            e1.EmailAlarms.AddRange(new[] { ealarm });
            client.Put(new UpdateEvent { Event = e1 });
            re1 = client.Get(new FindEventCached { EventId = e1.Id });
            Assert.Equal(re1.EmailAlarms.Count, 1);

            var emailAlarm = e1.EmailAlarms.First();
            emailAlarm.Description.Text = "This is a patched alarm";
            client.Post(new PatchEvent { EmailAlarms = e1.EmailAlarms, EventId = e1.Id });
            var patched = client.Get(new FindEventCached { EventId = e1.Id });
            var alarm = patched.EmailAlarms.FirstOrDefault();
            if (alarm != null)
                Assert.Equal(alarm.Description.Text, "This is a patched alarm");

            client.Delete(new DeleteEvent { EventId = e1.Id });
            var deleted = client.Get(new FindEvent { EventId = e1.Id });
            Assert.Equal(deleted, null);
        }
    }

    [Collection("Web Service Tests")]
    public class EventLocalWebServiceTests : EventWebServicesTests
    {
        public EventLocalWebServiceTests()
            : base(Properties.Settings.Default.localhost_uri)
        {
        }
    }
}