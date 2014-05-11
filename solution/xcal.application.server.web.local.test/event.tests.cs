using System;
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
            client = new JsonServiceClient(Properties.Settings.Default.test_server);
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
            client.Post(new FlushDatabase { Reset = false });
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
                Classification = CLASS.PUBLIC
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

            this.client.Put(new UpdateEvent { Event = minimal });
            retrieved = this.client.Get(new FindEvent { EventId = minimal.Id });
            Assert.AreEqual(retrieved.End, minimal.End);
            Assert.AreEqual(retrieved.RecurrenceRule.FREQ, FREQ.WEEKLY);
            Assert.AreEqual(retrieved.Organizer.CN, "Emmanuel Ngwane");
            Assert.AreEqual(retrieved, minimal);

            this.client.Patch(new PatchEvent { Transparency = TRANSP.OPAQUE, EventId = minimal.Id});
            var patched = this.client.Get(new FindEvent { EventId = minimal.Id });
            Assert.AreEqual(patched.Transparency, TRANSP.OPAQUE);

            this.client.Delete(new DeleteEvent { EventId = minimal.Id });
            var deleted = this.client.Get(new FindEvent { EventId = minimal.Id });
            Assert.AreEqual(deleted, null);
        }
    }
}
