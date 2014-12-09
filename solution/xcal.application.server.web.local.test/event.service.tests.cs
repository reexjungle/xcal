using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceStack.ServiceClient.Web;
using reexjungle.foundation.essentials.concretes;
using reexjungle.infrastructure.operations.concretes;
using reexjungle.infrastructure.operations.contracts;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.domain.operations;


namespace reexjungle.xcal.application.server.web.dev.test
{
    [TestClass]
    public class EventServiceUnitTests
    {
                
        private JsonServiceClient client;
        private GuidKeyGenerator uidkeygen;
        private FPIKeyGenerator<string> fpikeygen;

        public EventServiceUnitTests()
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

        #region Event Test Utility Methods

        private void Teardown()
        {
            try
            {
                client.Post(new FlushDatabase { Mode = FlushMode.soft });

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

        private IEnumerable<VEVENT> GenerateNEvents(int n)
        {
            var events = new List<VEVENT>(n);
            for (int i = 0; i < n; i++)
            {
                var ev = new VEVENT
                {
                    Uid = new GuidKeyGenerator().GetNextKey(),
                    Organizer = new ORGANIZER
                    {
                        Id = new GuidKeyGenerator().GetNextKey(),
                        CN = string.Format("Reex Monkey {0}", i + 1),
                        Address = new URI(string.Format("reexmonkey{0}@jungle.com", i + 1)),
                        Language = new LANGUAGE("en")
                    },
                    Location = new LOCATION
                    {
                        Text = string.Format("Reex Jungle {0}", i + 1),
                        Language = new LANGUAGE("de", "DE")
                    },
                    Summary = new SUMMARY(string.Format("Reex Meeting {0}", i + 1)),
                    Description = new DESCRIPTION("Another extreme meeting for reex monkeys"),
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
                    Id = uidkeygen.GetNextKey(),
                    Address = new URI(string.Format("reex_attendee{0}@tree.com", i + 1)),
                    CN = string.Format("Reex Attendee {0}", i + 1),
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
            foreach (var ev in events)
            {
                var capacity = randomizer.Next(1, attendees.Count() + 1);
                var tmp = new List<ATTENDEE>(capacity);
                for (int i = 0; i < capacity; i++)
                {
                    tmp.Add(atts[randomizer.Next(0, capacity)]);
                }

                ev.Attendees = new List<ATTENDEE>(capacity);
                ev.Attendees.AddRange(tmp.Distinct());
            }
            return events;
        } 

        #endregion

        #region Event Maintainance Tests

        [TestMethod]
        public void MaintainSingleEvent()
        {
            this.Teardown();
            var calendar = new VCALENDAR
            {
                Id = this.uidkeygen.GetNextKey(),
                ProdId = this.fpikeygen.GetNextKey(),
                Method = METHOD.PUBLISH
            };

            var minimal = new VEVENT
            {
                Uid = new GuidKeyGenerator().GetNextKey(),
                RecurrenceRule = new RECUR
                {
                    Id = new GuidKeyGenerator().GetNextKey(),
                    FREQ = FREQ.MONTHLY,
                    UNTIL = new DATE_TIME(new DateTime(2014, 6, 25, 18, 03, 08, 0, DateTimeKind.Utc))
                },

                Organizer = new ORGANIZER
                {
                    Id = new GuidKeyGenerator().GetNextKey(),
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
                Description = new DESCRIPTION("A meeting for coding gurus, nerds, geeks and quants who enjoy programming for others such that the world becomes a better place for all producers and consumers préamble à la gare de nöel für ein wünderschönes Abend mit Bären außerdem#"),
                Start = new DATE_TIME(new DateTime(2014, 6, 15, 16, 07, 01, 0, DateTimeKind.Utc)),
                End = new DATE_TIME(new DateTime(2014, 6, 15, 18, 03, 08, 0, DateTimeKind.Utc)),
                Status = STATUS.CONFIRMED,
                Transparency = TRANSP.TRANSPARENT,
                Classification = CLASS.PUBLIC,

                Attendees = new List<ATTENDEE>
                {
                    new ATTENDEE 
                    { 
                        Id = uidkeygen.GetNextKey(),
                        Address = new URI("king.reexmonkey@jungle.com"),
                        CN = "King Reexmonkey",
                        Participation = PARTSTAT.ACCEPTED,
                        Role = ROLE.CHAIR,
                        CalendarUserType = CUTYPE.INDIVIDUAL,
                        Language = new LANGUAGE("en")
                    },

                    new ATTENDEE 
                    { 
                        Id = uidkeygen.GetNextKey(),
                        Address = new URI("example1@twitter.com"),
                        CN = "Clone A",
                        Participation = PARTSTAT.ACCEPTED,
                        Role = ROLE.REQ_PARTICIPANT,
                        CalendarUserType = CUTYPE.INDIVIDUAL,
                        Language = new LANGUAGE("en")
                    },

                    new ATTENDEE 
                    { 
                        Id = uidkeygen.GetNextKey(),
                        Address = new URI("example2@twitter.com"),
                        CN = "Clone B",
                        Participation = PARTSTAT.ACCEPTED,
                        Role = ROLE.OPT_PARTICIPANT,
                        CalendarUserType = CUTYPE.INDIVIDUAL,
                        Language = new LANGUAGE("en")
                    },
                    new ATTENDEE 
                    { 
                        Id = uidkeygen.GetNextKey(),
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
            var retrieved = this.client.Get(new FindEvent { EventId = minimal.Id });
            Assert.AreEqual(retrieved.Organizer.CN, "King Reexmonkey");
            Assert.AreEqual(retrieved.Start, minimal.Start);
            Assert.AreEqual(retrieved, minimal);

            var cal = this.client.Get(new FindCalendar { CalendarId = calendar.Id });
            //var tmp = cal.ToString();
            //var tmp2 = tmp.UnfoldLines();
            
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


            this.client.Patch(new PatchEvent { Transparency = TRANSP.OPAQUE, EventId = minimal.Id });
            var patched = this.client.Get(new FindEvent { EventId = minimal.Id });
            Assert.AreEqual(patched.Transparency, TRANSP.OPAQUE);

            this.client.Delete(new DeleteEvent { EventId = minimal.Id });
            var deleted = this.client.Get(new FindEvent { EventId = minimal.Id });
            Assert.AreEqual(deleted, null);
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
                Id = this.uidkeygen.GetNextKey(),
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
            twin2A.Priority = new PRIORITY(PRIORITYLEVEL.MEDIUM);
            twin2A.Organizer.CN = "Reex Monkey 2 Updated";

            twin3A.Start = new DATE_TIME(new DateTime(2014, 6, 16, 10, 30, 0, 0, DateTimeKind.Local));
            twin3A.Duration = new DURATION(10, 11, 00, 00);
            twin3A.Priority = new PRIORITY(PRIORITYLEVEL.LOW);
            twin3A.Organizer.CN = "Reex Monkey 3 Updated";

            this.client.Put(new UpdateEvents { Events = new List<VEVENT> { twin2A, twin3A } });
            var updated = this.client.Post(new FindEvents { EventIds = new List<string> { twin2A.Id, twin3A.Id } });
            var utwin2a = updated.Where(x => x.Id == twin2A.Id).First();
            var utwin3a = updated.Where(x => x.Id == twin3A.Id).First();

            Assert.AreEqual(utwin2a.End, twin2A.End);
            Assert.AreEqual(utwin3a.Duration, twin3A.Duration);

            Assert.AreEqual(utwin2a.Priority.Level, PRIORITYLEVEL.MEDIUM);
            Assert.AreEqual(utwin3a.Organizer.CN, "Reex Monkey 3 Updated");
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

        [TestMethod]
        public void MaintainSingleEventWithAlarms()
        {
            this.Teardown();

            var calendar = new VCALENDAR
            {
                Id = this.uidkeygen.GetNextKey(),
                ProdId = this.fpikeygen.GetNextKey(),
                Version = "2.0",
                Method = METHOD.PUBLISH
            };
            var ev = new VEVENT
            {
                Uid = uidkeygen.GetNextKey(),

                Summary = new SUMMARY("Test Meeting"),
                Description = new DESCRIPTION("A test meeting for freaks"),
                Start = new DATE_TIME(new DateTime(2014, 6, 15, 16, 07, 01, 0, DateTimeKind.Utc)),
                End = new DATE_TIME(new DateTime(2014, 6, 15, 18, 03, 08, 0, DateTimeKind.Utc)),
                Status = STATUS.CONFIRMED,
                Transparency = TRANSP.TRANSPARENT,
                Classification = CLASS.PUBLIC,
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
                    Address = new URI("organizer@gmail.com"),
                    Language = new LANGUAGE("en")
                },
                Location = new LOCATION
                {
                    Text = "Düsseldorf",
                    Language = new LANGUAGE("de", "DE")
                }
            };

            ev.Attendees.AddRange(this.GenerateNAttendees(15));

            ev.AudioAlarms.Add(new AUDIO_ALARM
            {
                Id = uidkeygen.GetNextKey(),
                Duration = new DURATION(0, 0, 0, 20, 0),
                Action = ACTION.AUDIO,
                Repeat = 1,
                Trigger = new TRIGGER
                {
                    Id = uidkeygen.GetNextKey(),
                    DateTime = new DATE_TIME(DateTime.Now.AddDays(1)),
                    Duration = new DURATION(0, 0, 0, 5),
                    Related = RELATED.START,
                    ValueType = VALUE.DATE_TIME
                },
                AttachmentUri = new ATTACH_URI
                {
                    Id = uidkeygen.GetNextKey(),
                    Content = new URI("http://localhost:8900/music/wakeup.mp3"),
                    FormatType = new FMTTYPE("file", "audio")
                }
            });

            ev.DisplayAlarms.Add(new DISPLAY_ALARM
            {
                Id = uidkeygen.GetNextKey(),
                Action = ACTION.DISPLAY,
                Duration = new DURATION(0, 0, 0, 15, 0),
                Repeat = 1,
                Description = new DESCRIPTION("This is a sample display alarm")
            });

            ev.EmailAlarms.Add(new EMAIL_ALARM
            {
                Id = uidkeygen.GetNextKey(),
                Action = ACTION.EMAIL,
                Duration = new DURATION(0, 0, 0, 15, 0),
                Repeat = 1,
                Description = new DESCRIPTION("This is a sample email alarm"),
                Trigger = new TRIGGER
                {
                    Id = uidkeygen.GetNextKey(),
                    DateTime = new DATE_TIME(DateTime.Now.AddDays(1)),
                    Duration = new DURATION(0, 0, 0, 5),
                    Related = RELATED.START,
                    ValueType = VALUE.DATE_TIME
                },
                AttachmentBinaries = new List<ATTACH_BINARY>
                {
                    new ATTACH_BINARY
                    {
                        Id = uidkeygen.GetNextKey(),
                        Content = new BINARY("Binary attachment", ENCODING.BIT8)
                    }
                
                },
                Attendees = new List<ATTENDEE> 
                { 
                                        
                    new ATTENDEE 
                    { 
                        Id = uidkeygen.GetNextKey(),
                        Address = new URI("example1@gmail.com"),
                        CN = "King Reexmonkey",
                        Participation = PARTSTAT.ACCEPTED,
                        Role = ROLE.CHAIR,
                        CalendarUserType = CUTYPE.INDIVIDUAL,
                        Language = new LANGUAGE("en")
                    },

                    new ATTENDEE 
                    { 
                        Id = uidkeygen.GetNextKey(),
                        Address = new URI("example1@twitter.com"),
                        CN = "Clone A",
                        Participation = PARTSTAT.ACCEPTED,
                        Role = ROLE.REQ_PARTICIPANT,
                        CalendarUserType = CUTYPE.INDIVIDUAL,
                        Language = new LANGUAGE("en")
                    },

                    new ATTENDEE 
                    { 
                        Id = uidkeygen.GetNextKey(),
                        Address = new URI("example2@twitter.com"),
                        CN = "Clone B",
                        Participation = PARTSTAT.ACCEPTED,
                        Role = ROLE.OPT_PARTICIPANT,
                        CalendarUserType = CUTYPE.INDIVIDUAL,
                        Language = new LANGUAGE("en")
                    },
                    new ATTENDEE 
                    { 
                        Id = uidkeygen.GetNextKey(),
                        Address = new URI("example3@twitter.com"),
                        CN = "Room 5A",
                        Participation = PARTSTAT.ACCEPTED,
                        Role = ROLE.OPT_PARTICIPANT,
                        CalendarUserType = CUTYPE.ROOM,
                        Language = new LANGUAGE("en")
                    }
                }
            });

            this.client.Post(new AddCalendar { Calendar = calendar });
            this.client.Post(new AddEvent { CalendarId = calendar.Id, Event = ev });

            var retrieved = this.client.Get(new FindEvent { EventId = ev.Id });
            Assert.AreEqual(retrieved, ev);
            Assert.AreEqual(retrieved.AudioAlarms.AreDuplicatesOf(ev.AudioAlarms), true);
            Assert.AreNotEqual(retrieved.EmailAlarms.AreDuplicatesOf(ev.EmailAlarms), false);

            ////remove email alarm and update
            ev.AudioAlarms.First().AttachmentUri.FormatType = new FMTTYPE("file", "video");
            var ealarm = ev.EmailAlarms.First();
            ev.EmailAlarms.Clear();

            this.client.Put(new UpdateEvent { Event = ev });
            retrieved = this.client.Get(new FindEvent { EventId = ev.Id });
            Assert.AreEqual(retrieved.EmailAlarms.Count, 0);

            //reinsert some alarms and update
            ev.EmailAlarms.AddRange(new EMAIL_ALARM[] { ealarm });
            this.client.Put(new UpdateEvent { Event = ev });
            retrieved = this.client.Get(new FindEvent { EventId = ev.Id });
            Assert.AreEqual(retrieved.EmailAlarms.Count, 1);

            ev.EmailAlarms.First().Description.Text = "This is a patched alarm";
            this.client.Patch(new PatchEvent { EmailAlarms = ev.EmailAlarms, EventId = ev.Id });
            var patched = this.client.Get(new FindEvent { EventId = ev.Id });
            Assert.AreEqual(patched.EmailAlarms.First().Description.Text, "This is a patched alarm");

            this.client.Delete(new DeleteEvent { EventId = ev.Id });
            var deleted = this.client.Get(new FindEvent { EventId = ev.Id });
            Assert.AreEqual(deleted, null);

        }

        #endregion

        

    }
}
