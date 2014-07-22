using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceStack.ServiceClient.Web;
using reexmonkey.foundation.essentials.contracts;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.infrastructure.operations.concretes;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.domain.operations;


namespace reexmonkey.xcal.application.server.web.dev.test
{
    [TestClass]
    public class EventServiceUnitTests
    {
                
        private JsonServiceClient client;
        private GuidKeyGenerator guidkeygen;
        private FPIKeyGenerator<string> fpikeygen;

        public EventServiceUnitTests()
        {
            client = new JsonServiceClient(Properties.Settings.Default.local_server);
            guidkeygen = new GuidKeyGenerator();
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
            try
            {
                client.Post(new FlushDatabase { Hard = false });

            }
            catch (WebServiceException ex)
            {
                System.Diagnostics.Debug.WriteLine("Message: {0}", ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack-Trace {0}", ex.StackTrace);
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Debug.WriteLine("Message: {0}", ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack-Trace {0}", ex.StackTrace);
            }
        }

        [TestMethod]
        public void MaintainSingleEvent()
        {
            this.Teardown();
            var calendar = new VCALENDAR
            {
                Id = this.guidkeygen.GetNextKey(),
                ProdId = this.fpikeygen.GetNextKey(),
                Method = METHOD.PUBLISH
            };

            var minimal = new VEVENT 
            {
                Uid = new GuidKeyGenerator().GetNextKey(),
                RecurrenceId = new RECURRENCE_ID
                {
                    Id = new GuidKeyGenerator().GetNextKey(),
                    Range = RANGE.THISANDFUTURE,
                    Value = new DATE_TIME(new DateTime(2014, 6, 15, 16, 07, 01, 0, DateTimeKind.Utc))
                },
                RecurrenceRule = new RECUR
                {
                    Id = new GuidKeyGenerator().GetNextKey(),
                    FREQ = FREQ.DAILY,
                    Format = RecurFormat.DateTime,
                    UNTIL = new DATE_TIME(new DateTime(2014, 6, 25, 18, 03, 08, 0, DateTimeKind.Utc))
                },

                Organizer = new ORGANIZER
                {
                    Id = new GuidKeyGenerator().GetNextKey(),
                    CN = "Emmanuel Ngwane",
                    Address = new URI("ngwanemk@gmail.com"),
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
                Classification = CLASS.PUBLIC,

                Attendees = new List<ATTENDEE>
                {
                    new ATTENDEE 
                    { 
                        Id = guidkeygen.GetNextKey(),
                        Address = new URI("ngwanemk@ygmail.com"),
                        CN = "Emmanuel Ngwane",
                        Participation = PARTSTAT.ACCEPTED,
                        Role = ROLE.CHAIR,
                        CalendarUserType = CUTYPE.INDIVIDUAL,
                        Language = new LANGUAGE("en")
                    },

                    new ATTENDEE 
                    { 
                        Id = guidkeygen.GetNextKey(),
                        Address = new URI("example1@twitter.com"),
                        CN = "Clone A",
                        Participation = PARTSTAT.ACCEPTED,
                        Role = ROLE.REQ_PARTICIPANT,
                        CalendarUserType = CUTYPE.INDIVIDUAL,
                        Language = new LANGUAGE("en")
                    },

                    new ATTENDEE 
                    { 
                        Id = guidkeygen.GetNextKey(),
                        Address = new URI("example2@twitter.com"),
                        CN = "Clone B",
                        Participation = PARTSTAT.ACCEPTED,
                        Role = ROLE.OPT_PARTICIPANT,
                        CalendarUserType = CUTYPE.INDIVIDUAL,
                        Language = new LANGUAGE("en")
                    },
                    new ATTENDEE 
                    { 
                        Id = guidkeygen.GetNextKey(),
                        Address = new URI("example3@twitter.com"),
                        CN = "Room 5A",
                        Participation = PARTSTAT.ACCEPTED,
                        Role = ROLE.OPT_PARTICIPANT,
                        CalendarUserType = CUTYPE.ROOM,
                        Language = new LANGUAGE("en")
                    }

                }
            };

            this.client.Post(new AddCalendar { Calendar = calendar });
            this.client.Post(new AddEvent { CalendarId = calendar.Id, Event = minimal });
            var retrieved = this.client.Get(new FindEvent { EventId = minimal.Id});
            Assert.AreEqual(retrieved.Organizer.CN, "Emmanuel Ngwane");
            Assert.AreEqual(retrieved.Start, minimal.Start);
            Assert.AreEqual(retrieved, minimal);

            minimal.Start = new DATE_TIME(new DateTime(2014, 6, 16, 10, 30, 0, 0, DateTimeKind.Utc));
            minimal.Duration = new DURATION(1, 5, 2, 30);
            minimal.RecurrenceRule.FREQ = FREQ.WEEKLY;
            minimal.Organizer.CN = "Robot Clone";

            this.client.Put(new UpdateEvent { Event = minimal });
            retrieved = this.client.Get(new FindEvent { EventId = minimal.Id });
            Assert.AreEqual(retrieved.End, minimal.End);
            Assert.AreEqual(retrieved.RecurrenceRule.FREQ, FREQ.WEEKLY);
            Assert.AreEqual(retrieved.Organizer.CN, "Robot Clone");
            Assert.AreEqual(retrieved, minimal);

            minimal.Attendees.RemoveRange(0, 2);
            this.client.Put(new UpdateEvent { Event = minimal });
            retrieved = this.client.Get(new FindEvent { EventId = minimal.Id });
            Assert.AreEqual(retrieved.Attendees.Count, 2);


            this.client.Patch(new PatchEvent { Transparency = TRANSP.OPAQUE, EventId = minimal.Id});
            var patched = this.client.Get(new FindEvent { EventId = minimal.Id });
            Assert.AreEqual(patched.Transparency, TRANSP.OPAQUE);

            this.client.Delete(new DeleteEvent { EventId = minimal.Id });
            var deleted = this.client.Get(new FindEvent { EventId = minimal.Id });
            Assert.AreEqual(deleted, null);
        }

                
        private IEnumerable<VEVENT> GenerateNEvents(int n)
        {
            var events = new List<VEVENT>(n);
            for (int i = 0; i < n; i++)
            {
                var ev = new VEVENT
                {
                    Uid = new GuidKeyGenerator().GetNextKey(),
                    RecurrenceId = new RECURRENCE_ID
                    {
                        Id = new GuidKeyGenerator().GetNextKey(),
                        Range = RANGE.THISANDFUTURE,
                        Value = new DATE_TIME(new DateTime(2014, 6, 15, 16, 07, 01, 0, DateTimeKind.Utc))
                    },
                    RecurrenceRule = new RECUR
                    {
                        Id = new GuidKeyGenerator().GetNextKey(),
                        FREQ = FREQ.DAILY,
                        Format = RecurFormat.DateTime,
                        UNTIL = new DATE_TIME(new DateTime(2014, 6, 25, 18, 03, 08, 0, DateTimeKind.Utc))
                    },
                    Organizer = new ORGANIZER
                    {
                        Id = new GuidKeyGenerator().GetNextKey(),
                        CN = string.Format("Risk Monkey {0}", i+1),
                        Address = new URI(string.Format("riskmonkey{0}@jungle.com", i+1)),
                        Language = new LANGUAGE("en")
                    },
                    Location = new LOCATION
                    {
                        Text = string.Format("Risk Jungle {0}", i+1),
                        Language = new LANGUAGE("de", "DE")
                    },

                    Summary = new SUMMARY(string.Format("Test Meeting {0}", i+1)),
                    Description = new DESCRIPTION("Another test meeting for risk monkeys"),
                    Start = new DATE_TIME(new DateTime(2014, 6, 15, 16, 07, 01, 0, DateTimeKind.Utc)),
                    End = new DATE_TIME(new DateTime(2014, 6, 15, 18, 03, 08, 0, DateTimeKind.Utc)),
                    Status = STATUS.CONFIRMED,
                    Transparency = TRANSP.TRANSPARENT,
                    Classification = CLASS.PUBLIC,

                };

                events.Add(ev);
            }
            return events;

        }

        private IEnumerable<ATTENDEE> GenerateNAttendees(int n)
        {
            var attendees = new List<ATTENDEE>(n);
            for (int i = 0; i < n; i++)
            {
                var att = new ATTENDEE
                {
                    Id = guidkeygen.GetNextKey(),
                    Address = new URI(string.Format("riskmonkey{0}@twitter.com", i+1)),
                    CN = string.Format("Risk Monkey {0}", i+1),
                    Participation = PARTSTAT.ACCEPTED,
                    Role = ROLE.REQ_PARTICIPANT,
                    CalendarUserType = CUTYPE.INDIVIDUAL,
                    Language = new LANGUAGE("en")
                };

                attendees.Add(att);
            }
            return attendees;
        }

        private IEnumerable<VEVENT> RandomlyAttendEvents(ref IEnumerable<VEVENT> events, IEnumerable<ATTENDEE> attendees)
        {
            var randomizer = new Random();
            var atts = attendees.ToArray();
            foreach(var ev in events)
            {
                var capacity = randomizer.Next(1, attendees.Count() + 1);
                var tmp = new List<ATTENDEE>(capacity);
                for (int i = 0; i< capacity; i++)
                {
                    tmp.Add(atts[randomizer.Next(0, capacity)]);
                }

                ev.Attendees = new List<ATTENDEE>(capacity);
                ev.Attendees.AddRange(tmp.Distinct());
            }
            return events;
        }

        [TestMethod]
        public void MaintainMultipleEvents()
        {
            this.Teardown();
            var events = this.GenerateNEvents(5);
            var attendees = this.GenerateNAttendees(60);
            this.RandomlyAttendEvents(ref events, attendees);
            

            var calendar = new VCALENDAR
            {
                Id = this.guidkeygen.GetNextKey(),
                ProdId = this.fpikeygen.GetNextKey(),
                Method = METHOD.PUBLISH
            };

            var keys = events.Select(x => x.Id).ToList();
            this.client.Post(new AddCalendar { Calendar = calendar });
            this.client.Post(new AddEvents { CalendarId = calendar.Id, Events = events.ToList() });
            var retrieved = this.client.Get(new GetEvents { Page = 1, Size = 100 });
            Assert.AreEqual(retrieved.Count, events.Count());
            var twin2A = events.Where(x => x.Organizer.CN.Contains("2")).First();
            var twin2B = retrieved.Where(x => x.Organizer.CN.Contains("2")).First();
            var twin3A = events.Where(x => x.Organizer.CN.Contains("3")).First();
            var twin3B = retrieved.Where(x => x.Organizer.CN.Contains("3")).First();
            Assert.AreEqual(twin2A, twin2B);

            twin2A.Start = new DATE_TIME(new DateTime(2014, 6, 16, 10, 30, 0, 0, DateTimeKind.Utc));
            twin2A.Duration = new DURATION(1, 5, 2, 30);
            twin2A.RecurrenceRule.FREQ = FREQ.WEEKLY;
            twin2A.Organizer.CN = "Risk Monkey 2 Updated";

            twin3A.Start = new DATE_TIME(new DateTime(2014, 6, 16, 10, 30, 0, 0, DateTimeKind.Local));
            twin3A.Duration = new DURATION(10, 11, 00, 00);
            twin3A.RecurrenceRule.FREQ = FREQ.MONTHLY;
            twin3A.Organizer.CN = "Risk Monkey 3 Updated";

            this.client.Put(new UpdateEvents { Events = new List<VEVENT> { twin2A, twin3A } });
            var updated = this.client.Post(new FindEvents { EventIds = new List<string> { twin2A.Id, twin3A.Id} });
            var utwin2a = updated.Where(x => x.Id == twin2A.Id).First();
            var utwin3a = updated.Where(x => x.Id == twin3A.Id).First();

            Assert.AreEqual(utwin2a.End, twin2A.End);
            Assert.AreEqual(utwin3a.Duration, twin3A.Duration);

            Assert.AreEqual(utwin2a.RecurrenceRule.FREQ, FREQ.WEEKLY);
            Assert.AreEqual(utwin3a.Organizer.CN, "Risk Monkey 3 Updated");
            Assert.AreEqual(utwin2a, twin2A);
            Assert.AreEqual(utwin3a, twin3A);

            twin2A.Attendees.RemoveRange(0, 1);
            twin3A.Attendees.RemoveRange(0, 1);

            var tot_att_twin2a = twin2A.Attendees.Count;
            var tot_att_twin3a = twin3A.Attendees.Count;

            this.client.Put(new UpdateEvents { Events = new List<VEVENT> { twin2A, twin3A } });
            updated = this.client.Post(new FindEvents { EventIds = updated.Select(x => x.Id).ToList() });


            utwin2a = updated.Where(x => x.Id == twin2A.Id).First();
            utwin3a = updated.Where(x => x.Id == twin3A.Id).First();
            
            Assert.AreEqual(utwin2a.Attendees.Count, tot_att_twin2a);
            Assert.AreEqual(utwin3a.Attendees.Count, tot_att_twin3a);

            utwin2a.Organizer.Language = new LANGUAGE("fr");

            this.client.Patch(new PatchEvents 
            { 
                EventIds = keys,
                Transparency = TRANSP.OPAQUE, 
                Classification = CLASS.CONFIDENTIAL,
                Priority = new PRIORITY(PRIORITYLEVEL.HIGH),
                Organizer = utwin2a.Organizer,
                Attendees = utwin2a.Attendees
            });

            var patched = this.client.Post(new FindEvents { EventIds = keys });
            foreach (var result in patched)
            {
                Assert.AreEqual(result.Organizer.Language.Tag, "fr");
                Assert.AreEqual(result.Transparency, TRANSP.OPAQUE);
                Assert.AreEqual(result.Classification, CLASS.CONFIDENTIAL);
                Assert.AreEqual(result.Priority, new PRIORITY(PRIORITYLEVEL.HIGH));
                Assert.AreEqual(result.Attendees.Count, utwin2a.Attendees.Count);
                Assert.AreEqual(result.Attendees.AreDuplicatesOf(utwin2a.Attendees), true);
            }

            this.client.Delete(new DeleteEvents { EventIds = keys });
            var deleted = this.client.Post(new FindEvents { EventIds = keys });
            Assert.AreEqual(deleted.Count, 0);

        }

    }
}
