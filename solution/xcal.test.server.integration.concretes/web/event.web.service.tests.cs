using FizzWare.NBuilder;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.operations.concretes.cached;
using reexjungle.xcal.service.operations.concretes.live;
using reexjungle.xcal.test.server.integration.concretes.Properties;
using reexjungle.xcal.test.server.integration.contracts;
using reexjungle.xcal.test.units.concretes;
using reexjungle.xcal.test.units.contracts;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.foundation.contracts;
using reexjungle.xmisc.infrastructure.concretes.operations;
using reexjungle.xmisc.infrastructure.contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace reexjungle.xcal.test.server.integration.concretes.web
{
    public abstract class EventWebServicesTests : IWebServiceIntegrationTests
    {
        protected readonly ICalendarUnitTest calendarUnitTest;
        protected readonly IEventUnitTest eventUnitTest;
        protected IPropertiesUnitTest propertiesUnitTest;
        protected IAlarmUnitTest alarmUnitTest;
        private readonly IKeyGenerator<Guid> guidKeyGenerator;
        private readonly IKeyGenerator<string> fpiKeyGenerator;
        protected JsonWebServiceTestFactory factory;

        protected EventWebServicesTests()
        {
            var statusGenerator = new ContentGenerator<ApprovalStatus>(new List<ApprovalStatus>
            {
                ApprovalStatus.Informal,
                ApprovalStatus.None,
                ApprovalStatus.Standard
            });

            var authorGenerator = new ContentGenerator<string>(new List<string> { "RXJG", "MSFT", "GOGL", "YHOO" });
            var productGenerator = new ContentGenerator<string>(new List<string> { "VCALENDAR", "ICALENDAR", "CALDAV" });
            var descriptionGenerator = new ContentGenerator<string>(new List<string>
            {
                new RandomGenerator().Phrase(new RandomGenerator().Next(3, 7)),
                new RandomGenerator().Phrase(new RandomGenerator().Next(5, 10)),
                new RandomGenerator().Phrase(new RandomGenerator().Next(15, 25)),
            });

            var languageGenerator = new ContentGenerator<string>(new List<string> { "EN", "FR", "DE", "IT", "ES", "PL" });
            var referenceGenerator = new ContentGenerator<string>(new List<string> { "ISO", "IEEE", "WPO", "UN" });

            fpiKeyGenerator = new FpiStringKeyGenerator(
                statusGenerator,
                authorGenerator,
                productGenerator,
                descriptionGenerator,
                languageGenerator,
                referenceGenerator);

            guidKeyGenerator = new SequentialGuidKeyGenerator();

            calendarUnitTest = new CalendarUnitTest(guidKeyGenerator, fpiKeyGenerator);

            eventUnitTest = new EventUnitTest(guidKeyGenerator);

            propertiesUnitTest = new PropertiesUnitTest(guidKeyGenerator);

            alarmUnitTest = new AlarmUnitTest(guidKeyGenerator, propertiesUnitTest);

            factory = new JsonWebServiceTestFactory(null);
        }

        public void TearDown()
        {
            factory.GetClient().Post(new FlushDatabase { Mode = FlushMode.soft });
        }

        //[Fact]
        //public void MaintainSingleEvent()
        //{
        //    TearDown();

        //    var c1 = calendarUnitTest.GenerateCalendarsOfSize(1).First();

        //    var events = eventUnitTest.GenerateEventsOfSize(1);
        //    var e1 = events.First();
        //    e1.RecurrenceRule = new RECUR
        //        {
        //            Id = guidKeyGenerator.GetNext(),
        //            FREQ = FREQ.MONTHLY
        //        };

        //    e1.Attendees = propertiesUnitTest.GenerateAttendeesOfSize(4).ToList();

        //    var client = factory.GetClient();
        //    client.Post(new AddCalendar { Calendar = c1 });
        //    client.Post(new AddEvent { CalendarId = c1.Id, Event = e1 });
        //    var re1 = client.Get(new FindEvent { EventId = e1.Id });
        //    Assert.Equal(re1.Organizer.CN, e1.Organizer.CN);
        //    Assert.Equal(re1.Start, e1.Start);
        //    Assert.Equal(re1, e1);

        //    var rcal = client.Get(new FindCalendar { CalendarId = c1.Id });
        //    e1.Start = new DATE_TIME(new DateTime(2014, 6, 16, 10, 30, 0, 0, DateTimeKind.Utc));
        //    e1.Duration = new DURATION(1, 5, 2, 30);
        //    e1.RecurrenceRule.FREQ = FREQ.WEEKLY;
        //    e1.Organizer.CN = "Robot Clone";

        //    client.Put(new UpdateEvent { Event = e1 });
        //    re1 = client.Get(new FindEvent { EventId = e1.Id });
        //    Assert.Equal(re1.End, e1.End);
        //    Assert.Equal(re1.RecurrenceRule.FREQ, FREQ.WEEKLY);
        //    Assert.Equal(re1.Organizer.CN, "Robot Clone");
        //    Assert.Equal(re1, e1);

        //    e1.Attendees.RemoveRange(0, 2);
        //    client.Put(new UpdateEvent { Event = e1 });
        //    re1 = client.Get(new FindEvent { EventId = e1.Id });
        //    Assert.Equal(re1.Attendees.Count, 2);

        //    client.Patch(new PatchEvent { Transparency = TRANSP.OPAQUE, EventId = e1.Id });
        //    var patched = client.Get(new FindEvent { EventId = e1.Id });
        //    Assert.Equal(patched.Transparency, TRANSP.OPAQUE);

        //    client.Delete(new DeleteEvent { EventId = e1.Id });
        //    var deleted = client.Get(new FindEvent { EventId = e1.Id });
        //    Assert.Equal(deleted, null);
        //}

        //public void MaintainSingleEventCached()
        //{
        //    TearDown();

        //    var c1 = calendarUnitTest.GenerateCalendarsOfSize(1).FirstOrDefault();

        //    var events = eventUnitTest.GenerateEventsOfSize(1);
        //    var e1 = events.FirstOrDefault();
        //    e1.RecurrenceRule = new RECUR
        //    {
        //        Id = guidKeyGenerator.GetNext(),
        //        FREQ = FREQ.MONTHLY
        //    };

        //    e1.Attendees = propertiesUnitTest.GenerateAttendeesOfSize(4).ToList();

        //    var client = factory.GetClient();
        //    client.Post(new AddCalendar { Calendar = c1 });
        //    client.Post(new AddEvent { CalendarId = c1.Id, Event = e1 });
        //    var re1 = client.Get(new FindEventCached { EventId = e1.Id });
        //    Assert.Equal(re1.Organizer.CN, e1.Organizer.CN);
        //    Assert.Equal(re1.Start, e1.Start);
        //    Assert.Equal(re1, e1);

        //    var rcal = client.Get(new FindCalendarCached { CalendarId = c1.Id });
        //    e1.Start = new DATE_TIME(new DateTime(2014, 6, 16, 10, 30, 0, 0, DateTimeKind.Utc));
        //    e1.Duration = new DURATION(1, 5, 2, 30);
        //    e1.RecurrenceRule.FREQ = FREQ.WEEKLY;
        //    e1.Organizer.CN = "Robot Clone";

        //    client.Put(new UpdateEvent { Event = e1 });
        //    re1 = client.Get(new FindEventCached { EventId = e1.Id });
        //    Assert.Equal(re1.End, e1.End);
        //    Assert.Equal(re1.RecurrenceRule.FREQ, FREQ.WEEKLY);
        //    Assert.Equal(re1.Organizer.CN, "Robot Clone");
        //    Assert.Equal(re1, e1);

        //    e1.Attendees.RemoveRange(0, 2);
        //    client.Put(new UpdateEvent { Event = e1 });
        //    re1 = client.Get(new FindEventCached { EventId = e1.Id });
        //    Assert.Equal(re1.Attendees.Count, 2);

        //    client.Patch(new PatchEvent { Transparency = TRANSP.OPAQUE, EventId = e1.Id });
        //    var patched = client.Get(new FindEventCached { EventId = e1.Id });
        //    Assert.Equal(patched.Transparency, TRANSP.OPAQUE);

        //    client.Delete(new DeleteEvent { EventId = e1.Id });
        //    var deleted = client.Get(new FindEventCached { EventId = e1.Id });
        //    Assert.Equal(deleted, VEVENT.Empty);
        //}

        //[Fact]
        //public void MaintainMultipleEvents()
        //{
        //    TearDown();

        //    var calendar = calendarUnitTest.GenerateCalendarsOfSize(1).FirstOrDefault();
        //    var events = eventUnitTest.GenerateEventsOfSize(5);
        //    eventUnitTest.RandomlyAttendEvents(ref events, propertiesUnitTest.GenerateAttendeesOfSize(10));
        //    var keys = events.Select(x => x.Id).ToList();

        //    var client = factory.GetClient();

        //    client.Post(new AddCalendar { Calendar = calendar });
        //    client.Post(new AddEvents { CalendarId = calendar.Id, Events = events.ToList() });
        //    var r1 = client.Get(new GetEvents { Page = 1, Size = 100 });
        //    Assert.Equal(r1.Count, events.Count());
        //    var e1 = events.FirstOrDefault(x => x.Organizer == events.ElementAt(0).Organizer);
        //    var p1 = r1.FirstOrDefault(x => x.Organizer == events.ElementAt(0).Organizer);
        //    var e2 = events.FirstOrDefault(x => x.Organizer == events.ElementAt(1).Organizer);
        //    var p2 = r1.FirstOrDefault(x => x.Organizer == events.ElementAt(1).Organizer);

        //    e1.Start = new DATE_TIME(new DateTime(2014, 6, 16, 10, 30, 0, 0, DateTimeKind.Utc));
        //    e1.Duration = new DURATION(0, 1, 2, 30);
        //    e1.Priority = new PRIORITY(PRIORITYLEVEL.MEDIUM);
        //    e1.Description.Text = string.Format("{0} >>Updated<<", e1.Description.Text);
        //    e1.RecurrenceRule = new RECUR
        //    {
        //        Id = "test123",
        //        FREQ = FREQ.DAILY,
        //        INTERVAL = 1,
        //        UNTIL = new DATE_TIME(2014, 06, 30, 06, 0, 0, TimeType.Utc),
        //        BYHOUR = new List<uint> { 9, 12, 15, 18, 21 },
        //        BYMINUTE = new List<uint> { 5, 10, 15, 20, 25 },
        //        BYSETPOS = new List<int> { 2, -2 }
        //    };

        //    var Rx = e1.GenerateRecurrences<VEVENT>(guidKeyGenerator);

        //    e2.Start = new DATE_TIME(new DateTime(2014, 6, 16, 10, 30, 0, 0, DateTimeKind.Local));
        //    e2.Duration = new DURATION(0, 1, 10, 00);
        //    e2.Priority = new PRIORITY(PRIORITYLEVEL.LOW);
        //    e2.Description.Text = string.Format("{0} >>Updated<<", e2.Description.Text);

        //    client.Put(new UpdateEvents { Events = new List<VEVENT> { e1, e2 }.Concat(Rx).ToList() });

        //    var r2 = client.Post(new FindEvents { EventIds = new List<string> { e1.Id, e2.Id } });
        //    var u1 = r2.FirstOrDefault(x => x == e1);
        //    var u2 = r2.FirstOrDefault(x => x == e2);

        //    Assert.Equal(u1.End, e1.End);
        //    Assert.Equal(u2.Duration, e2.Duration);

        //    Assert.Equal(u1.Priority.Level, PRIORITYLEVEL.MEDIUM);
        //    Assert.Equal(u2.Description, e2.Description);
        //    Assert.Equal(u1, e1);
        //    Assert.Equal(u2, e2);

        //    e1.Attendees.RemoveRange(0, 1);
        //    e2.Attendees.RemoveRange(0, 1);

        //    var te1 = e1.Attendees.Count;
        //    var te2 = e2.Attendees.Count;

        //    client.Put(new UpdateEvents { Events = new List<VEVENT> { e1, e2 } });
        //    r2 = client.Post(new FindEvents { EventIds = r2.Select(x => x.Id).ToList() });

        //    u1 = r2.FirstOrDefault(x => x.Id == e1.Id);
        //    u2 = r2.FirstOrDefault(x => x.Id == e2.Id);

        //    Assert.Equal(u1.Attendees.Count, te1);
        //    Assert.Equal(u2.Attendees.Count, te2);
        //    u1.Organizer.Language = new LANGUAGE("fr");

        //    keys = client.Get(new GetEvents { Page = 1, Size = int.MaxValue }).Select(x => x.Id).ToList();
        //    client.Patch(new PatchEvents
        //    {
        //        EventIds = keys,
        //        Transparency = TRANSP.OPAQUE,
        //        Classification = CLASS.CONFIDENTIAL,
        //        Priority = new PRIORITY(PRIORITYLEVEL.HIGH),
        //        Organizer = u1.Organizer,
        //        Attendees = u1.Attendees,
        //    });

        //    var patched = client.Post(new FindEvents { EventIds = keys });
        //    foreach (var result in patched)
        //    {
        //        Assert.Equal(result.Organizer.Language.Tag, "fr");
        //        Assert.Equal(result.Transparency, TRANSP.OPAQUE);
        //        Assert.Equal(result.Classification, CLASS.CONFIDENTIAL);
        //        Assert.Equal(result.Priority, new PRIORITY(PRIORITYLEVEL.HIGH));
        //        Assert.Equal(result.Attendees.IsEquivalentOf(u1.Attendees), true);
        //    }

        //    client.Patch(new PatchEvents
        //    {
        //        Start = u1.Start,
        //        End = u1.End,
        //        Duration = u1.Duration,
        //        Categories = u1.Categories,
        //        Description = new DESCRIPTION("Patched again!!!"),
        //        Transparency = TRANSP.TRANSPARENT,
        //        Classification = CLASS.PRIVATE,
        //        Priority = new PRIORITY(PRIORITYLEVEL.MEDIUM),
        //        Organizer = u1.Organizer,
        //        Attendees = u1.Attendees
        //    });

        //    patched = client.Get(new GetEvents { Page = 1, Size = int.MaxValue });
        //    foreach (var result in patched)
        //    {
        //        Assert.Equal(result.Transparency, TRANSP.TRANSPARENT);
        //        Assert.Equal(result.Classification, CLASS.PRIVATE);
        //        Assert.Equal(result.Priority, new PRIORITY(PRIORITYLEVEL.MEDIUM));
        //        Assert.Equal(result.Attendees.Count, u1.Attendees.Count());
        //        Assert.Equal(result.Attendees.IsEquivalentOf(u1.Attendees), true);
        //    }

        //    //client.Delete(new DeleteEvents { EventIds = keys });
        //    //var deleted = client.Post(new FindEvents { EventIds = keys });
        //    //Assert.Equal(deleted.Count, 0);
        //}

        //[Fact]
        //public void MaintainMultipleEventsCached()
        //{
        //    TearDown();

        //    var calendar = calendarUnitTest.GenerateCalendarsOfSize(1).FirstOrDefault();
        //    var events = eventUnitTest.GenerateEventsOfSize(5);
        //    eventUnitTest.RandomlyAttendEvents(ref events, propertiesUnitTest.GenerateAttendeesOfSize(10));
        //    var keys = events.Select(x => x.Id).ToList();

        //    var client = factory.GetClient();

        //    client.Post(new AddCalendar { Calendar = calendar });
        //    client.Post(new AddEvents { CalendarId = calendar.Id, Events = events.ToList() });
        //    var r1 = client.Get(new GetEventsCached { Page = 1, Size = 100 });
        //    Assert.Equal(r1.Count, events.Count());
        //    var e1 = events.FirstOrDefault(x => x.Organizer == events.ElementAt(0).Organizer);
        //    var e2 = events.FirstOrDefault(x => x.Organizer == events.ElementAt(1).Organizer);

        //    e1.Start = new DATE_TIME(new DateTime(2014, 6, 16, 10, 30, 0, 0, DateTimeKind.Utc));
        //    e1.Duration = new DURATION(0, 1, 2, 30);
        //    e1.Priority = new PRIORITY(PRIORITYLEVEL.MEDIUM);
        //    e1.Description.Text = string.Format("{0} >>Updated<<", e1.Description.Text);
        //    var len = e1.Description.Text.Length;

        //    e2.Start = new DATE_TIME(new DateTime(2014, 6, 16, 10, 30, 0, 0, DateTimeKind.Local));
        //    e2.Duration = new DURATION(0, 1, 10, 00);
        //    e2.Priority = new PRIORITY(PRIORITYLEVEL.LOW);
        //    e2.Description.Text = string.Format("{0} >>Updated<<", e2.Description.Text);

        //    client.Put(new UpdateEvents { Events = new List<VEVENT> { e1, e2 } });

        //    var r2 = client.Post(new FindEventsCached { EventIds = new List<string> { e1.Id, e2.Id } });
        //    var u1 = r2.FirstOrDefault(x => x == e1);
        //    var u2 = r2.FirstOrDefault(x => x == e2);

        //    Assert.Equal(u1.End, e1.End);
        //    Assert.Equal(u2.Duration, e2.Duration);

        //    Assert.Equal(u1.Priority.Level, PRIORITYLEVEL.MEDIUM);
        //    Assert.Equal(u2.Description, e2.Description);
        //    Assert.Equal(u1, e1);
        //    Assert.Equal(u2, e2);

        //    e1.Attendees.RemoveRange(0, 1);
        //    e2.Attendees.RemoveRange(0, 1);

        //    var te1 = e1.Attendees.Count;
        //    var te2 = e2.Attendees.Count;

        //    client.Put(new UpdateEvents { Events = new List<VEVENT> { e1, e2 } });
        //    r2 = client.Post(new FindEventsCached { EventIds = r2.Select(x => x.Id).ToList() });

        //    u1 = r2.FirstOrDefault(x => x.Id == e1.Id);
        //    u2 = r2.FirstOrDefault(x => x.Id == e2.Id);

        //    Assert.Equal(u1.Attendees.Count, te1);
        //    Assert.Equal(u2.Attendees.Count, te2);
        //    u1.Organizer.Language = new LANGUAGE("fr");

        //    keys = client.Get(new GetEvents { Page = 1, Size = int.MaxValue }).Select(x => x.Id).ToList();
        //    client.Patch(new PatchEvents
        //    {
        //        EventIds = keys,
        //        Transparency = TRANSP.OPAQUE,
        //        Classification = CLASS.CONFIDENTIAL,
        //        Priority = new PRIORITY(PRIORITYLEVEL.HIGH),
        //        Organizer = u1.Organizer,
        //        Attendees = u1.Attendees,
        //    });

        //    var patched = client.Post(new FindEventsCached { EventIds = keys });
        //    foreach (var result in patched)
        //    {
        //        Assert.Equal(result.Organizer.Language.Tag, "fr");
        //        Assert.Equal(result.Transparency, TRANSP.OPAQUE);
        //        Assert.Equal(result.Classification, CLASS.CONFIDENTIAL);
        //        Assert.Equal(result.Priority, new PRIORITY(PRIORITYLEVEL.HIGH));
        //        Assert.Equal(result.Attendees.IsEquivalentOf(u1.Attendees), true);
        //    }

        //    client.Patch(new PatchEvents
        //    {
        //        Start = u1.Start,
        //        End = u1.End,
        //        Duration = u1.Duration,
        //        Categories = u1.Categories,
        //        Description = new DESCRIPTION("Patched again!!!"),
        //        Transparency = TRANSP.TRANSPARENT,
        //        Classification = CLASS.PRIVATE,
        //        Priority = new PRIORITY(PRIORITYLEVEL.MEDIUM),
        //        Organizer = u1.Organizer,
        //        Attendees = u1.Attendees
        //    });

        //    patched = client.Get(new GetEventsCached { Page = 1, Size = int.MaxValue });
        //    foreach (var result in patched)
        //    {
        //        Assert.Equal(result.Transparency, TRANSP.TRANSPARENT);
        //        Assert.Equal(result.Classification, CLASS.PRIVATE);
        //        Assert.Equal(result.Priority, new PRIORITY(PRIORITYLEVEL.MEDIUM));
        //        Assert.Equal(result.Attendees.Count, u1.Attendees.Count());
        //        Assert.Equal(result.Attendees.IsEquivalentOf(u1.Attendees), true);
        //    }

        //    client.Delete(new DeleteEvents { EventIds = keys });
        //    var deleted = client.Post(new FindEventsCached { EventIds = keys });
        //    Assert.Equal(deleted.Count, 0);
        //}

        //[Fact]
        //public void MaintainSingleEventWithAlarms()
        //{
        //    TearDown();

        //    var calendar = calendarUnitTest.GenerateCalendarsOfSize(1).FirstOrDefault();

        //    var events = eventUnitTest.GenerateEventsOfSize(1);
        //    eventUnitTest.RandomlyAttendEvents(ref events, propertiesUnitTest.GenerateAttendeesOfSize(10));
        //    var e1 = events.FirstOrDefault();

        //    e1.AudioAlarms = alarmUnitTest.GenerateAudioAlarmsOfSize(5).ToList();
        //    e1.DisplayAlarms = alarmUnitTest.GenerateDisplayAlarmsOfSize(5).ToList();
        //    e1.EmailAlarms = alarmUnitTest.GenerateEmailAlarmsOfSize(3).ToList();

        //    var client = factory.GetClient();
        //    client.Post(new AddCalendar { Calendar = calendar });
        //    client.Post(new AddEvent { CalendarId = calendar.Id, Event = e1 });

        //    var re1 = client.Get(new FindEvent { EventId = e1.Id });
        //    Assert.Equal(re1, e1);
        //    Assert.Equal(re1.AudioAlarms.IsEquivalentOf(e1.AudioAlarms), true);
        //    Assert.NotEqual(re1.EmailAlarms.IsEquivalentOf(e1.EmailAlarms), false);

        //    ////remove email alarm and update
        //    e1.AudioAlarms.FirstOrDefault().AttachmentUri.FormatType = new FMTTYPE("file", "video");
        //    var ealarm = e1.EmailAlarms.FirstOrDefault();
        //    e1.EmailAlarms.Clear();

        //    client.Put(new UpdateEvent { Event = e1 });
        //    re1 = client.Get(new FindEvent { EventId = e1.Id });
        //    Assert.Equal(re1.EmailAlarms.Count, 0);

        //    //reinsert some alarms and update
        //    e1.EmailAlarms.AddRange(new EMAIL_ALARM[] { ealarm });
        //    client.Put(new UpdateEvent { Event = e1 });
        //    re1 = client.Get(new FindEvent { EventId = e1.Id });
        //    Assert.Equal(re1.EmailAlarms.Count, 1);

        //    e1.EmailAlarms.FirstOrDefault().Description.Text = "This is a patched alarm";
        //    client.Patch(new PatchEvent { EmailAlarms = e1.EmailAlarms, EventId = e1.Id });
        //    var patched = client.Get(new FindEvent { EventId = e1.Id });
        //    Assert.Equal(patched.EmailAlarms.FirstOrDefault().Description.Text, "This is a patched alarm");

        //    //client.Delete(new DeleteEvent { EventId = e1.Id });
        //    //var deleted = client.Get(new FindEvent { EventId = e1.Id });
        //    //Assert.Equal(deleted, null);
        //}

        //[Fact]
        //public void MaintainSingleEventWithAlarmsCached()
        //{
        //    TearDown();

        //    var calendar = calendarUnitTest.GenerateCalendarsOfSize(1).FirstOrDefault();

        //    var events = eventUnitTest.GenerateEventsOfSize(1);
        //    eventUnitTest.RandomlyAttendEvents(ref events, propertiesUnitTest.GenerateAttendeesOfSize(10));
        //    var e1 = events.FirstOrDefault();

        //    e1.AudioAlarms = alarmUnitTest.GenerateAudioAlarmsOfSize(5).ToList();
        //    e1.DisplayAlarms = alarmUnitTest.GenerateDisplayAlarmsOfSize(5).ToList();
        //    e1.EmailAlarms = alarmUnitTest.GenerateEmailAlarmsOfSize(3).ToList();

        //    var client = factory.GetClient();
        //    client.Post(new AddCalendar { Calendar = calendar });
        //    client.Post(new AddEvent { CalendarId = calendar.Id, Event = e1 });

        //    var re1 = client.Get(new FindEventCached { EventId = e1.Id });
        //    Assert.Equal(re1, e1);
        //    Assert.Equal(re1.AudioAlarms.IsEquivalentOf(e1.AudioAlarms), true);
        //    Assert.NotEqual(re1.EmailAlarms.IsEquivalentOf(e1.EmailAlarms), false);

        //    ////remove email alarm and update
        //    e1.AudioAlarms.FirstOrDefault().AttachmentUri.FormatType = new FMTTYPE("file", "video");
        //    var ealarm = e1.EmailAlarms.FirstOrDefault();
        //    e1.EmailAlarms.Clear();

        //    client.Put(new UpdateEvent { Event = e1 });
        //    re1 = client.Get(new FindEventCached { EventId = e1.Id });
        //    Assert.Equal(re1.EmailAlarms.Count, 0);

        //    //reinsert some alarms and update
        //    e1.EmailAlarms.AddRange(new EMAIL_ALARM[] { ealarm });
        //    client.Put(new UpdateEvent { Event = e1 });
        //    re1 = client.Get(new FindEventCached { EventId = e1.Id });
        //    Assert.Equal(re1.EmailAlarms.Count, 1);

        //    var _emailAlarm = e1.EmailAlarms.FirstOrDefault();
        //    if (_emailAlarm != null)
        //        _emailAlarm.Description.Text = "This is a patched alarm";
        //    client.Patch(new PatchEvent { EmailAlarms = e1.EmailAlarms, EventId = e1.Id });
        //    var patched = client.Get(new FindEventCached { EventId = e1.Id });
        //    var _alarm = patched.EmailAlarms.FirstOrDefault();
        //    if (_alarm != null)
        //        Assert.Equal(_alarm.Description.Text, "This is a patched alarm");

        //    //client.Delete(new DeleteEvent { EventId = e1.Id });
        //    //var deleted = client.Get(new FindEvent { EventId = e1.Id });
        //    //Assert.Equal(deleted, null);
        //}
    }

    public class EventRemoteWebServiceTestsDev1 : EventWebServicesTests
    {
        public EventRemoteWebServiceTestsDev1()
        {
            factory.BaseUri = Settings.Default.remote_dev1_uri;
        }
    }

    public class EventRemoteWebServiceTestsDev2 : EventWebServicesTests
    {
        public EventRemoteWebServiceTestsDev2()
        {
            factory.BaseUri = Settings.Default.remote_dev2_uri;
        }
    }

    public class EventLocalWebServiceTests : EventWebServicesTests
    {
        public EventLocalWebServiceTests()
        {
            factory.BaseUri = Settings.Default.localhost_uri;
        }
    }
}