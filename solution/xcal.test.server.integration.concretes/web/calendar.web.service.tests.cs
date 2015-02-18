using FizzWare.NBuilder;
using reexjungle.infrastructure.concretes.operations;
using reexjungle.infrastructure.contracts;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.operations.concretes.cached;
using reexjungle.xcal.service.operations.concretes.live;
using reexjungle.xcal.test.server.integration.contracts;
using reexjungle.xcal.test.units.concretes;
using reexjungle.xcal.test.units.contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace reexjungle.xcal.test.server.integration.concretes.web
{
    public abstract class CalendarWebServicesTests : IWebServiceIntegrationTests
    {
        private CalendarUnitTests caltests = new CalendarUnitTests();
        private EventUnitTests evtests = new EventUnitTests();

        protected JsonWebServiceTestFactory factory = null;

        public CalendarWebServicesTests()
        {
            this.factory = new JsonWebServiceTestFactory(null);
        }

        public void Initialize()
        {
            this.caltests.FPIKeyGen = Builder<StringFPIKeyGenerator>
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
        public void MaintainSingleCalendar()
        {
            this.TearDown();
            this.Initialize();

            var c1 = caltests.GenerateCalendarsOfSize(1).First();
            var client = this.factory.GetClient();
            client.Post(new AddCalendar { Calendar = c1 });
            var f1 = client.Get(new FindCalendar { CalendarId = c1.Id });
            Assert.Equal(f1.Calscale, c1.Calscale);
            Assert.Equal(f1.ProdId, c1.ProdId);
            Assert.Equal(f1.Method, c1.Method);
            Assert.Equal(f1.Version, c1.Version);
            Assert.Equal(f1.Id, c1.Id);

            c1.Method = METHOD.REQUEST;
            c1.Version = "3.0";
            c1.Calscale = CALSCALE.HEBREW;
            client.Put(new UpdateCalendar { Calendar = c1 });
            var u1 = client.Get(new FindCalendar { CalendarId = c1.Id });
            Assert.Equal(u1.Calscale, CALSCALE.HEBREW);
            Assert.Equal(u1.Version, "3.0");
            Assert.Equal(u1.Method, METHOD.REQUEST);
            Assert.Equal(u1, c1);

            client.Patch(new PatchCalendar { Scale = CALSCALE.JULIAN, CalendarId = c1.Id });
            var p1 = client.Get(new FindCalendar { CalendarId = c1.Id });
            Assert.Equal(p1.Calscale, CALSCALE.JULIAN);

            client.Delete(new DeleteCalendar { CalendarId = c1.Id });
            var d1 = client.Get(new FindCalendar { CalendarId = c1.Id });
            Assert.Equal(d1, null);
        }

        [Fact]
        public void MaintainSingleCalendarCached()
        {
            this.TearDown();
            this.Initialize();

            var c1 = caltests.GenerateCalendarsOfSize(1).First();
            var client = this.factory.GetClient();
            client.Post(new AddCalendar { Calendar = c1 });
            var f1 = client.Get(new FindCalendarCached { CalendarId = c1.Id });
            Assert.Equal(f1.Calscale, c1.Calscale);
            Assert.Equal(f1.ProdId, c1.ProdId);
            Assert.Equal(f1.Method, c1.Method);
            Assert.Equal(f1.Version, c1.Version);
            Assert.Equal(f1.Id, c1.Id);

            c1.Method = METHOD.REQUEST;
            c1.Version = "3.0";
            c1.Calscale = CALSCALE.HEBREW;
            client.Put(new UpdateCalendar { Calendar = c1 });
            var u1 = client.Get(new FindCalendarCached { CalendarId = c1.Id });
            Assert.Equal(u1.Calscale, CALSCALE.HEBREW);
            Assert.Equal(u1.Version, "3.0");
            Assert.Equal(u1.Method, METHOD.REQUEST);
            Assert.Equal(u1, c1);

            client.Patch(new PatchCalendar { Scale = CALSCALE.JULIAN, CalendarId = c1.Id });
            var p1 = client.Get(new FindCalendarCached { CalendarId = c1.Id });
            Assert.Equal(p1.Calscale, CALSCALE.JULIAN);

            client.Delete(new DeleteCalendar { CalendarId = c1.Id });
            var d1 = client.Get(new FindCalendarCached { CalendarId = c1.Id });
            Assert.Equal(d1, VCALENDAR.Empty);
        }

        [Fact]
        public void MaintainMultipleCalendars()
        {
            this.TearDown();
            this.Initialize();

            var cals = this.caltests.GenerateCalendarsOfSize(5).ToList();

            //customize calendars
            cals[0].Method = METHOD.PUBLISH;
            cals[0].Version = "1.0";
            cals[1].Method = METHOD.REQUEST;
            cals[1].Version = "1.0";
            cals[2].Method = METHOD.REFRESH;
            cals[2].Version = "3.0";
            cals[3].Method = METHOD.ADD;
            cals[2].Version = "3.0";
            cals[4].Method = METHOD.CANCEL;
            cals[4].Version = "1.0";

            var client = this.factory.GetClient();
            client.Post(new AddCalendars { Calendars = cals.ToList() });
            var keys = cals.Select(x => x.Id).ToList();

            var rcals = client.Post(new FindCalendars { CalendarIds = keys });
            Assert.Equal(rcals.Count, 5);
            Assert.Equal(rcals.Where(x => x.Calscale == CALSCALE.GREGORIAN).Count(), 5);
            Assert.Equal(rcals.Where(x => x.ProdId == cals[0].ProdId).Count(), 5);
            Assert.Equal(rcals.Where(x => x.Version == "1.0").Count(), 3);
            Assert.Equal(rcals.Where(x => x.Version == "3.0").FirstOrDefault().Method, METHOD.REFRESH);

            cals[3].Calscale = CALSCALE.ISLAMIC;
            cals[4].Version = "2.0";
            client.Put(new UpdateCalendars { Calendars = cals.ToList() });
            rcals = client.Post(new FindCalendars { CalendarIds = keys });
            Assert.Equal(rcals.Where(x => x.Id == keys[3]).FirstOrDefault().Calscale, CALSCALE.ISLAMIC);
            Assert.Equal(rcals.Where(x => x.Id == keys[4]).FirstOrDefault().Version, "2.0");
            Assert.Equal(rcals.Where(x => x.Calscale == CALSCALE.GREGORIAN).Count(), 4);
            Assert.Equal(rcals.Where(x => x.Version == "2.0").Count(), 2);

            client.Patch(new PatchCalendars
            {
                Scale = CALSCALE.JULIAN,
                CalendarIds = new List<string> { keys[0], keys[1], keys[2] }
            });

            rcals = client.Post(new FindCalendars { CalendarIds = keys });
            Assert.Equal(rcals.Where(x => x.Calscale == CALSCALE.JULIAN).Count(), 3);
            Assert.Equal(rcals.Where(x => x.Id == keys[4]).FirstOrDefault().Calscale, CALSCALE.GREGORIAN);

            rcals = client.Get(new GetCalendars { Page = 1, Size = 2 });
            Assert.Equal(rcals.Count(), 2);

            rcals = client.Get(new GetCalendars { Page = 1, Size = 10 });
            Assert.Equal(rcals.Count(), 5);

            rcals = client.Get(new GetCalendars { Page = 2, Size = 5 });
            Assert.Equal(rcals.Count(), 0);

            rcals = client.Get(new GetCalendars { Page = 3, Size = 50 });
            Assert.Equal(rcals.Count(), 0);
        }

        [Fact]
        public void MaintainMultipleCalendarsCached()
        {
            this.TearDown();
            this.Initialize();

            var cals = this.caltests.GenerateCalendarsOfSize(5).ToList();

            //customize calendars
            cals[0].Method = METHOD.PUBLISH;
            cals[0].Version = "1.0";
            cals[1].Method = METHOD.REQUEST;
            cals[1].Version = "1.0";
            cals[2].Method = METHOD.REFRESH;
            cals[2].Version = "3.0";
            cals[3].Method = METHOD.ADD;
            cals[2].Version = "3.0";
            cals[4].Method = METHOD.CANCEL;
            cals[4].Version = "1.0";

            var client = this.factory.GetClient();
            client.Post(new AddCalendars { Calendars = cals.ToList() });
            var keys = cals.Select(x => x.Id).ToList();

            var rcals = client.Post(new FindCalendarsCached { CalendarIds = keys });
            Assert.Equal(rcals.Count, 5);
            Assert.Equal(rcals.Where(x => x.Calscale == CALSCALE.GREGORIAN).Count(), 5);
            Assert.Equal(rcals.Where(x => x.ProdId == cals[0].ProdId).Count(), 5);
            Assert.Equal(rcals.Where(x => x.Version == "1.0").Count(), 3);
            Assert.Equal(rcals.Where(x => x.Version == "3.0").FirstOrDefault().Method, METHOD.REFRESH);

            cals[3].Calscale = CALSCALE.ISLAMIC;
            cals[4].Version = "2.0";
            client.Put(new UpdateCalendars { Calendars = cals.ToList() });
            rcals = client.Post(new FindCalendarsCached { CalendarIds = keys });
            Assert.Equal(rcals.Where(x => x.Id == keys[3]).FirstOrDefault().Calscale, CALSCALE.ISLAMIC);
            Assert.Equal(rcals.Where(x => x.Id == keys[4]).FirstOrDefault().Version, "2.0");
            Assert.Equal(rcals.Where(x => x.Calscale == CALSCALE.GREGORIAN).Count(), 4);
            Assert.Equal(rcals.Where(x => x.Version == "2.0").Count(), 2);

            client.Patch(new PatchCalendars
            {
                Scale = CALSCALE.JULIAN,
                CalendarIds = new List<string> { keys[0], keys[1], keys[2] }
            });

            rcals = client.Post(new FindCalendarsCached { CalendarIds = keys });
            Assert.Equal(rcals.Where(x => x.Calscale == CALSCALE.JULIAN).Count(), 3);
            Assert.Equal(rcals.Where(x => x.Id == keys[4]).FirstOrDefault().Calscale, CALSCALE.GREGORIAN);

            rcals = client.Get(new GetCalendarsCached { Page = 1, Size = 2 });
            Assert.Equal(rcals.Count(), 2);

            rcals = client.Get(new GetCalendarsCached { Page = 1, Size = 10 });
            Assert.Equal(rcals.Count(), 5);

            rcals = client.Get(new GetCalendarsCached { Page = 2, Size = 5 });
            Assert.Equal(rcals.Count(), 0);

            rcals = client.Get(new GetCalendarsCached { Page = 3, Size = 50 });
            Assert.Equal(rcals.Count(), 0);
        }

        [Fact]
        public void MaintainSingleCalendarWithEvents()
        {
            this.TearDown();
            this.Initialize();

            var cal = this.caltests.GenerateCalendarsOfSize(1).First();
            var evs = this.evtests.GenerateEventsOfSize(5).ToList();

            cal.Events.AddRange(evs);

            var client = this.factory.GetClient();
            client.Post(new AddCalendar { Calendar = cal });

            var retrieved = client.Get(new FindCalendar { CalendarId = cal.Id });
            Assert.Equal(retrieved.Calscale, CALSCALE.GREGORIAN);
            Assert.Equal(retrieved.ProdId, cal.ProdId);
            Assert.Equal(retrieved.Events.Count, 5);
            Assert.Equal(retrieved, cal);

            cal.Method = METHOD.REQUEST;
            cal.Version = "3.0";
            cal.Calscale = CALSCALE.HEBREW;

            //remove 4 events and update
            cal.Events.RemoveRange(0, 4);

            client.Put(new UpdateCalendar { Calendar = cal });
            retrieved = client.Get(new FindCalendar { CalendarId = cal.Id });
            Assert.Equal(retrieved.Calscale, CALSCALE.HEBREW);
            Assert.Equal(retrieved.Version, "3.0");
            Assert.Equal(retrieved.Method, METHOD.REQUEST);
            Assert.Equal(retrieved.Events.Count, 1);
            Assert.Equal(retrieved.Events[0], evs[4]);
            Assert.Equal(retrieved, cal);

            //reinsert some events and update
            cal.Events.AddRange(new VEVENT[] { evs[0], evs[1] });
            client.Put(new UpdateCalendar { Calendar = cal });
            retrieved = client.Get(new FindCalendar { CalendarId = cal.Id });
            Assert.Equal(retrieved.Events.Count, 3);

            client.Patch(new PatchCalendar { Scale = CALSCALE.JULIAN, CalendarId = cal.Id });
            var patched = client.Get(new FindCalendar { CalendarId = cal.Id });
            Assert.Equal(patched.Calscale, CALSCALE.JULIAN);

            client.Delete(new DeleteCalendar { CalendarId = cal.Id });
            var deleted = client.Get(new FindCalendar { CalendarId = cal.Id });
            Assert.Equal(deleted, null);
        }

        [Fact]
        public void MaintainMultipleCalendarsWithEvents()
        {
            this.TearDown();
            this.Initialize();

            //multiple calendars
            var cals = this.caltests.GenerateCalendarsOfSize(5).ToList();

            //customize calendars
            cals[0].Method = METHOD.PUBLISH;
            cals[0].Version = "1.0";
            cals[1].Method = METHOD.REQUEST;
            cals[1].Version = "2.0";
            cals[2].Method = METHOD.REFRESH;
            cals[2].Version = "3.0";
            cals[3].Method = METHOD.ADD;
            cals[3].Version = "4.0";
            cals[4].Method = METHOD.CANCEL;
            cals[4].Version = "5.0";

            //multiple events
            var evs = this.evtests.GenerateEventsOfSize(5).ToList();
            cals[0].Events.AddRange(new VEVENT[] { evs[0], evs[1], evs[2] });
            cals[1].Events.AddRange(new VEVENT[] { evs[2] });
            cals[2].Events.AddRange(new VEVENT[] { evs[0], evs[1], evs[2], evs[3] });
            cals[3].Events.AddRange(new VEVENT[] { evs[2], evs[4] });
            cals[4].Events.AddRange(evs);

            var client = this.factory.GetClient();
            client.Post(new AddCalendars { Calendars = cals.ToList() });
            var keys = cals.Select(x => x.Id).ToList();

            var retrieved = client.Post(new FindCalendars { CalendarIds = keys });
            Assert.Equal(retrieved.Count, 5);
            Assert.Equal(retrieved.Where(x => x.Version.StartsWith("1")).First().Events.Count, 3);
            Assert.Equal(retrieved.Where(x => x.Version.StartsWith("2")).First().Events.Count, 1);
            Assert.Equal(retrieved.Where(x => x.Version.StartsWith("3")).First().Events.Count, 4);
            Assert.Equal(retrieved.Where(x => x.Version.StartsWith("4")).First().Events.Count, 2);
            Assert.Equal(retrieved.Where(x => x.Version.StartsWith("5")).First().Events.Count, 5);

            //remove 1 event from each and update
            foreach (var cal in cals) cal.Events.RemoveRange(0, 1);

            client.Put(new UpdateCalendars { Calendars = cals.ToList() });
            retrieved = client.Post(new FindCalendars { CalendarIds = keys });
            Assert.Equal(retrieved.Where(x => x.Version.StartsWith("1")).First().Events.Count, 2);
            Assert.Equal(retrieved.Where(x => x.Version.StartsWith("2")).First().Events.Count, 0);
            Assert.Equal(retrieved.Where(x => x.Version.StartsWith("3")).First().Events.Count, 3);
            Assert.Equal(retrieved.Where(x => x.Version.StartsWith("4")).First().Events.Count, 1);
            Assert.Equal(retrieved.Where(x => x.Version.StartsWith("5")).First().Events.Count, 4);

            //add some more events and update
            cals[0].Events.AddRange(new VEVENT[] { evs[0], evs[1], evs[2], evs[3] });
            cals[1].Events.AddRange(new VEVENT[] { evs[1], evs[4] });
            cals[2].Events.AddRange(new VEVENT[] { evs[1], evs[4] });
            cals[3].Events.AddRange(new VEVENT[] { evs[3], evs[2], evs[4] });
            cals[4].Events.Add(evs[3]);
            client.Put(new UpdateCalendars { Calendars = cals.ToList() });

            retrieved = client.Post(new FindCalendars { CalendarIds = keys });
            Assert.Equal(retrieved.Where(x => x.Version.StartsWith("1")).First().Events.Distinct().Count(), 4);
            Assert.Equal(retrieved.Where(x => x.Version.StartsWith("2")).First().Events.Distinct().Count(), 2);
            Assert.Equal(retrieved.Where(x => x.Version.StartsWith("3")).First().Events.Distinct().Count(), 4);
            Assert.Equal(retrieved.Where(x => x.Version.StartsWith("4")).First().Events.Distinct().Count(), 3);
            Assert.Equal(retrieved.Where(x => x.Version.StartsWith("5")).First().Events.Distinct().Count(), 4);

            client.Patch(new PatchCalendars { CalendarIds = keys, Method = METHOD.REQUEST, Scale = CALSCALE.INDIAN, Events = new List<VEVENT> { evs[3] } });
            var patched = client.Post(new FindCalendars { CalendarIds = keys });
            foreach (var result in patched)
            {
                Assert.Equal(result.Calscale, CALSCALE.INDIAN);
                Assert.Equal(result.Method, METHOD.REQUEST);
                Assert.Equal(result.Events.Count, 1);
                Assert.Equal(result.Events.First().Id, evs[3].Id);
            }

            client.Delete(new DeleteCalendars { CalendarIds = keys });
            var deleted = client.Post(new FindCalendars { CalendarIds = keys });
            Assert.Equal(deleted.Count, 0);
        }
    }

    public class CalendarRemoteWebServiceTestsDev1 : CalendarWebServicesTests
    {
        public CalendarRemoteWebServiceTestsDev1()
            : base()
        {
            this.factory.BaseUri = Properties.Settings.Default.remote_dev1_uri;
        }
    }

    public class CalendarRemoteWebServiceTestsDev2 : CalendarWebServicesTests
    {
        public CalendarRemoteWebServiceTestsDev2()
            : base()
        {
            this.factory.BaseUri = Properties.Settings.Default.remote_dev2_uri;
        }
    }

    public class CalendarLocalWebServiceTests : CalendarWebServicesTests
    {
        public CalendarLocalWebServiceTests()
            : base()
        {
            this.factory.BaseUri = Properties.Settings.Default.localhost_uri;
        }
    }
}