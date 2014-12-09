using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceStack.ServiceClient.Web;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.domain.operations;
using reexjungle.infrastructure.operations.concretes;
using reexjungle.infrastructure.operations.contracts;
using System.Collections.Generic;

namespace reexjungle.xcal.application.server.web.dev.test
{
    [TestClass]
    public class CalendarServiceUnitTests
    {
        private JsonServiceClient client;
        private GuidKeyGenerator uidkeygen;
        private FPIKeyGenerator<string> fpikeygen;

        public CalendarServiceUnitTests()
        {
            client = new JsonServiceClient(Properties.Settings.Default.local_server);
            uidkeygen = new GuidKeyGenerator();
            fpikeygen = new FPIKeyGenerator<string>
            {
                Owner = Properties.Settings.Default.fpiOwner,
                Authority = Properties.Settings.Default.fpiAuthority,
                Description = Properties.Settings.Default.fpiDescription,
                LanguageId = Properties.Settings.Default.fpiLanguageId
            };
        }

        private void Teardown()
        {
            client.Post(new FlushDatabase { Mode = FlushMode.soft });
        }

        [TestMethod]
        public void MaintainSingleCalendar()
        {
            this.Teardown();
            var calendar = new VCALENDAR
            {
                Id = this.uidkeygen.GetNextKey(),
                ProdId = this.fpikeygen.GetNextKey(),
                Method = METHOD.PUBLISH
            };

            this.client.Post(new AddCalendar  { Calendar = calendar });

            var retrieved = this.client.Get(new FindCalendar { CalendarId = calendar.Id });
            Assert.AreEqual(retrieved.Calscale, CALSCALE.GREGORIAN);
            Assert.AreEqual(retrieved.ProdId, calendar.ProdId);
            Assert.AreEqual(retrieved, calendar);

            calendar.Method = METHOD.REQUEST;
            calendar.Version = "3.0";
            calendar.Calscale = CALSCALE.HEBREW;
            
            this.client.Put(new UpdateCalendar { Calendar = calendar });
            var updated = this.client.Get(new FindCalendar { CalendarId = calendar.Id });
            Assert.AreEqual(updated.Calscale, CALSCALE.HEBREW);
            Assert.AreEqual(updated.Version, "3.0");
            Assert.AreEqual(updated.Method, METHOD.REQUEST);
            Assert.AreEqual(updated, calendar);

            this.client.Patch(new PatchCalendar { Scale = CALSCALE.JULIAN, CalendarId = calendar.Id });
            var patched = this.client.Get(new FindCalendar { CalendarId = calendar.Id });
            Assert.AreEqual(patched.Calscale, CALSCALE.JULIAN);

            this.client.Delete(new DeleteCalendar { CalendarId = calendar.Id });
            var deleted = this.client.Get(new FindCalendar { CalendarId = calendar.Id });
            Assert.AreEqual(deleted, null);

        }

        [TestMethod]
        public void MaintainMultipleCalendars()
        {
            this.Teardown();
            var calendars = new VCALENDAR[5];
            for (int i = 0; i < 5; i++)
            {
                calendars[i] = new VCALENDAR 
                {
                    Id = this.uidkeygen.GetNextKey(),
                    ProdId = this.fpikeygen.GetNextKey()
                };
            }

            //customize calendars
            calendars[0].Method = METHOD.PUBLISH;
            calendars[0].Version = "1.0";
            calendars[1].Method = METHOD.REQUEST;
            calendars[1].Version = "1.0";
            calendars[2].Method = METHOD.REFRESH;
            calendars[2].Version = "3.0";
            calendars[3].Method = METHOD.ADD;
            calendars[2].Version = "3.0";
            calendars[4].Method = METHOD.CANCEL;
            calendars[4].Version = "1.0";

            this.client.Post(new AddCalendars { Calendars = calendars.ToList() });
            var keys = calendars.Select(x => x.Id).ToList();

            var retrieved = this.client.Post(new FindCalendars { CalendarIds = keys});
            Assert.AreEqual(retrieved.Count, 5);
            Assert.AreEqual(retrieved.Where(x => x.Calscale == CALSCALE.GREGORIAN).Count(), 5);
            Assert.AreEqual(retrieved.Where(x => x.ProdId == calendars[0].ProdId).Count(), 5);
            Assert.AreEqual(retrieved.Where(x => x.Version == "1.0").Count(), 3);
            Assert.AreEqual(retrieved.Where(x => x.Version == "3.0").FirstOrDefault().Method, METHOD.REFRESH);

            calendars[3].Calscale = CALSCALE.ISLAMIC;
            calendars[4].Version = "2.0";
            this.client.Put(new UpdateCalendars { Calendars = calendars.ToList() });
            retrieved = this.client.Post(new FindCalendars { CalendarIds = keys });
            Assert.AreEqual(retrieved.Where(x => x.Id == keys[3]).FirstOrDefault().Calscale, CALSCALE.ISLAMIC);
            Assert.AreEqual(retrieved.Where(x => x.Id == keys[4]).FirstOrDefault().Version, "2.0");
            Assert.AreEqual(retrieved.Where(x => x.Calscale == CALSCALE.GREGORIAN).Count(), 4);
            Assert.AreEqual(retrieved.Where(x => x.Version == "2.0").Count(), 2);


            this.client.Patch(new PatchCalendars 
            { 
                Scale = CALSCALE.JULIAN, 
                CalendarIds = new List<string> {keys[0], keys[1], keys[2] } 
            });
            
            retrieved = this.client.Post(new FindCalendars  {CalendarIds = keys });
            Assert.AreEqual(retrieved.Where(x => x.Calscale == CALSCALE.JULIAN).Count(), 3);
            Assert.AreEqual(retrieved.Where(x => x.Id == keys[4]).FirstOrDefault().Calscale, CALSCALE.GREGORIAN);

            retrieved = this.client.Get(new GetCalendars { Page = 1, Size = 2 });
            Assert.AreEqual(retrieved.Count(), 2);

            retrieved = this.client.Get(new GetCalendars { Page = 1, Size = 10 });
            Assert.AreEqual(retrieved.Count(), 5);

            retrieved = this.client.Get(new GetCalendars { Page = 2, Size = 5 });
            Assert.AreEqual(retrieved.Count(), 0);

            retrieved = this.client.Get(new GetCalendars { Page = 3, Size = 50 });
            Assert.AreEqual(retrieved.Count(), 0);
        }

        [TestMethod]
        public void MaintainSingleCalendarWithEvents()
        {
            this.Teardown();
            var calendar = new VCALENDAR
            {
                Id = this.uidkeygen.GetNextKey(),
                ProdId = this.fpikeygen.GetNextKey(),
                Method = METHOD.PUBLISH
            };

            var events = new VEVENT[5];
            for (int i = 0; i < 5; i++)
            {
                events[i] = new VEVENT
                {
                    Uid = uidkeygen.GetNextKey(),
                    RecurrenceId = new RECURRENCE_ID
                    {
                        Id = uidkeygen.GetNextKey(),
                        Range = RANGE.THISANDFUTURE,
                        Value = new DATE_TIME(new DateTime(2014, 6, 15, 16, 07, 01, 0, DateTimeKind.Utc))
                    },
                    RecurrenceRule = new RECUR
                    {
                        Id = uidkeygen.GetNextKey(),
                        FREQ = FREQ.DAILY,
                        UNTIL = new DATE_TIME(new DateTime(2014, 6, 25, 18, 03, 08, 0, DateTimeKind.Utc))
                    },

                    Organizer = new ORGANIZER
                    {
                        Id = uidkeygen.GetNextKey(),
                        CN = "King Reexmonkey",
                        Address = new URI("example@gmail.com"),
                        Language = new LANGUAGE("en")
                    },
                    Location = new LOCATION
                    {
                        Text = "Düsseldorf",
                        Language = new LANGUAGE("de", "DE")
                    },

                    Summary = new SUMMARY("Test Meeting"),
                    Description = new DESCRIPTION("A test meeting for freaks"),
                    Start = new DATE_TIME(new DateTime(2014, 6, 15, 16, 07, 01, 0, DateTimeKind.Utc)),
                    End = new DATE_TIME(new DateTime(2014, 6, 15, 18, 03, 08, 0, DateTimeKind.Utc)),
                    Status = STATUS.CONFIRMED,
                    Transparency = TRANSP.TRANSPARENT,
                    Classification = CLASS.PUBLIC
                };
            }

            calendar.Events.AddRange(events);
            this.client.Post(new AddCalendar { Calendar = calendar });

            var retrieved = this.client.Get(new FindCalendar { CalendarId = calendar.Id });
            Assert.AreEqual(retrieved.Calscale, CALSCALE.GREGORIAN);
            Assert.AreEqual(retrieved.ProdId, calendar.ProdId);
            Assert.AreEqual(retrieved.Events.Count, 5);
            Assert.AreEqual(retrieved, calendar);

            calendar.Method = METHOD.REQUEST;
            calendar.Version = "3.0";
            calendar.Calscale = CALSCALE.HEBREW;

            //remove 4 events and update
            calendar.Events.RemoveRange(0, 4);

            this.client.Put(new UpdateCalendar { Calendar = calendar });
            retrieved = this.client.Get(new FindCalendar { CalendarId = calendar.Id });
            Assert.AreEqual(retrieved.Calscale, CALSCALE.HEBREW);
            Assert.AreEqual(retrieved.Version, "3.0");
            Assert.AreEqual(retrieved.Method, METHOD.REQUEST);
            Assert.AreEqual(retrieved.Events.Count, 1);
            Assert.AreEqual(retrieved.Events[0], events[4]);
            Assert.AreEqual(retrieved, calendar);

            //reinsert some events and update
            calendar.Events.AddRange(new VEVENT[]{events[0], events[1]});
            this.client.Put(new UpdateCalendar { Calendar = calendar });
            retrieved = this.client.Get(new FindCalendar { CalendarId = calendar.Id });
            Assert.AreEqual(retrieved.Events.Count, 3);

            this.client.Patch(new PatchCalendar { Scale = CALSCALE.JULIAN, CalendarId = calendar.Id });
            var patched = this.client.Get(new FindCalendar { CalendarId = calendar.Id });
            Assert.AreEqual(patched.Calscale, CALSCALE.JULIAN);

            this.client.Delete(new DeleteCalendar { CalendarId = calendar.Id });
            var deleted = this.client.Get(new FindCalendar { CalendarId = calendar.Id });
            Assert.AreEqual(deleted, null);
        }

        [TestMethod]
        public void MaintainMultipleCalendarsWithEvents()
        {
            this.Teardown();

            //multiple calendars
            var calendars = new VCALENDAR[5];
            for (int i = 0; i < 5; i++)
            {
                calendars[i] = new VCALENDAR
                {
                    Id = this.uidkeygen.GetNextKey(),
                    ProdId = this.fpikeygen.GetNextKey()
                };
            }

            //customize calendars
            calendars[0].Method = METHOD.PUBLISH;
            calendars[0].Version = "1.0";
            calendars[1].Method = METHOD.REQUEST;
            calendars[1].Version = "2.0";
            calendars[2].Method = METHOD.REFRESH;
            calendars[2].Version = "3.0";
            calendars[3].Method = METHOD.ADD;
            calendars[3].Version = "4.0";
            calendars[4].Method = METHOD.CANCEL;
            calendars[4].Version = "5.0";

            //multiple events
            var events = new VEVENT[5];
            for (int i = 0; i < 5; i++)
            {
                events[i] = new VEVENT
                {
                    Uid = uidkeygen.GetNextKey(),
                    RecurrenceId = new RECURRENCE_ID
                    {
                        Id = uidkeygen.GetNextKey(),
                        Range = RANGE.THISANDFUTURE,
                        Value = new DATE_TIME(new DateTime(2014, 6, 15, 16, 07, 01, 0, DateTimeKind.Utc))
                    },
                    RecurrenceRule = new RECUR
                    {
                        Id = uidkeygen.GetNextKey(),
                        FREQ = FREQ.DAILY,
                        UNTIL = new DATE_TIME(new DateTime(2014, 6, 25, 18, 03, 08, 0, DateTimeKind.Utc))
                    },

                    Organizer = new ORGANIZER
                    {
                        Id = uidkeygen.GetNextKey(),
                        CN = "King Reexmonkey",
                        Address = new URI("king.reexmonkey@jungle.com"),
                        Language = new LANGUAGE("en")
                    },
                    Location = new LOCATION
                    {
                        Text = "Düsseldorf",
                        Language = new LANGUAGE("de", "DE")
                    },

                    Summary = new SUMMARY("Test Meeting"),
                    Description = new DESCRIPTION("A test meeting for freaks"),
                    Start = new DATE_TIME(new DateTime(2014, 6, 15, 16, 07, 01, 0, DateTimeKind.Utc)),
                    End = new DATE_TIME(new DateTime(2014, 6, 15, 18, 03, 08, 0, DateTimeKind.Utc)),
                    Status = STATUS.CONFIRMED,
                    Transparency = TRANSP.TRANSPARENT,
                    Classification = CLASS.PUBLIC
                };
            }

            calendars[0].Events.AddRange(new VEVENT[]{events[0], events[1], events[2]}); 
            calendars[1].Events.AddRange(new VEVENT[]{events[2]}); 
            calendars[2].Events.AddRange(new VEVENT[]{events[0], events[1], events[2], events[3]});
            calendars[3].Events.AddRange(new VEVENT[]{events[2], events[4]}); 
            calendars[4].Events.AddRange(events);

            this.client.Post(new AddCalendars { Calendars = calendars.ToList() });
            var keys = calendars.Select(x => x.Id).ToList();

            var retrieved = this.client.Post(new FindCalendars { CalendarIds = keys });
            Assert.AreEqual(retrieved.Count, 5);
            Assert.AreEqual(retrieved.Where(x => x.Version.StartsWith("1")).First().Events.Count, 3);
            Assert.AreEqual(retrieved.Where(x => x.Version.StartsWith("2")).First().Events.Count, 1);
            Assert.AreEqual(retrieved.Where(x => x.Version.StartsWith("3")).First().Events.Count, 4);
            Assert.AreEqual(retrieved.Where(x => x.Version.StartsWith("4")).First().Events.Count, 2);
            Assert.AreEqual(retrieved.Where(x => x.Version.StartsWith("5")).First().Events.Count, 5);

            //remove 1 event from each and update
            foreach(var cal in calendars) cal.Events.RemoveRange(0, 1);

            this.client.Put(new UpdateCalendars {  Calendars = calendars.ToList() });
            retrieved = this.client.Post(new FindCalendars { CalendarIds = keys });
            Assert.AreEqual(retrieved.Where(x => x.Version.StartsWith("1")).First().Events.Count, 2);
            Assert.AreEqual(retrieved.Where(x => x.Version.StartsWith("2")).First().Events.Count, 0);
            Assert.AreEqual(retrieved.Where(x => x.Version.StartsWith("3")).First().Events.Count, 3);
            Assert.AreEqual(retrieved.Where(x => x.Version.StartsWith("4")).First().Events.Count, 1);
            Assert.AreEqual(retrieved.Where(x => x.Version.StartsWith("5")).First().Events.Count, 4);


            //add some more events and update
            calendars[0].Events.AddRange(new VEVENT[] { events[0], events[1], events[2], events[3] });
            calendars[1].Events.AddRange(new VEVENT[] { events[1], events[4] });
            calendars[2].Events.AddRange(new VEVENT[] { events[1], events[4] });
            calendars[3].Events.AddRange(new VEVENT[] {events[3], events[2], events[4] });
            calendars[4].Events.Add(events[3]);
            this.client.Put(new UpdateCalendars { Calendars = calendars.ToList() });

            retrieved = this.client.Post(new FindCalendars { CalendarIds = keys });
            Assert.AreEqual(retrieved.Where(x => x.Version.StartsWith("1")).First().Events.Distinct().Count(), 4);
            Assert.AreEqual(retrieved.Where(x => x.Version.StartsWith("2")).First().Events.Distinct().Count(), 2);
            Assert.AreEqual(retrieved.Where(x => x.Version.StartsWith("3")).First().Events.Distinct().Count(), 4);
            Assert.AreEqual(retrieved.Where(x => x.Version.StartsWith("4")).First().Events.Distinct().Count(), 3);
            Assert.AreEqual(retrieved.Where(x => x.Version.StartsWith("5")).First().Events.Distinct().Count(), 4);


            this.client.Patch(new PatchCalendars {CalendarIds = keys, Method = METHOD.REQUEST, Scale = CALSCALE.INDIAN, Events = new List<VEVENT> { events[3] } });
            var patched = this.client.Post(new FindCalendars { CalendarIds = keys  });
            foreach (var result in patched)
            {
                Assert.AreEqual(result.Calscale, CALSCALE.INDIAN);
                Assert.AreEqual(result.Method, METHOD.REQUEST);
                Assert.AreEqual(result.Events.Count, 1);
                Assert.AreEqual(result.Events.First().Id, events[3].Id);
            }

            this.client.Delete(new DeleteCalendars { CalendarIds = keys });
            var deleted = this.client.Post(new FindCalendars { CalendarIds = keys });
            Assert.AreEqual(deleted.Count, 0);
        }
    }
}
