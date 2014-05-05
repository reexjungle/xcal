using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceStack.ServiceClient.Web;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.domain.operations;
using reexmonkey.infrastructure.operations.concretes;
using System.Collections.Generic;

namespace reexmonkey.xcal.application.server.web.dev.test
{
    [TestClass]
    public class CalendarServiceUnitTests
    {
        private JsonServiceClient client;
        private GuidKeyGenerator guidkeygen;
        private FPIKeyGenerator<string> fpikeygen;

        public CalendarServiceUnitTests()
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
            client.Delete(new DeleteCalendars());
        }

        [TestMethod]
        public void MaintainSingleCalendar()
        {
            this.Teardown();
            var calendar = new VCALENDAR
            {
                Id = this.guidkeygen.GetNextKey(),
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

            //this.Teardown();

        }

        [TestMethod]
        public void MaintainMultipleCalendars()
        {
            this.Teardown();
            var calendars = new VCALENDAR[5];
            for(int i = 0; i < 5; i++ )
            {
                calendars[i] = new VCALENDAR 
                {
                    Id = this.guidkeygen.GetNextKey(),
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
            calendars[4].Method = METHOD.CANCEL;
            calendars[4].Version = "1.0";

            this.client.Post(new AddCalendars { Calendars = calendars.ToList() });
            var keys = calendars.Select(x => x.Id).ToList();

            var retrieved = this.client.Get(new FindCalendars { CalendarIds = keys});
            Assert.AreEqual(retrieved.Count, 5);
            Assert.AreEqual(retrieved.Where(x => x.Calscale == CALSCALE.GREGORIAN).Count(), 5);
            Assert.AreEqual(retrieved.Where(x => x.ProdId == calendars[0].ProdId).Count(), 5);
            Assert.AreEqual(retrieved.Where(x => x.Version == "1.0").Count(), 3);
            Assert.AreEqual(retrieved.Where(x => x.Version == "3.0").FirstOrDefault().Method, METHOD.REFRESH);

            this.client.Patch(new PatchCalendars 
            { 
                Scale = CALSCALE.JULIAN, 
                CalendarIds = new List<string> {keys[0], keys[1], keys[2] } 
            });
            var patched = this.client.Get(new FindCalendars 
            { 
                CalendarIds = new List<string> { keys[0], keys[1], keys[2] } 
            });
            Assert.AreEqual(patched.Where(x => x.Calscale == CALSCALE.JULIAN).Count(), 3);
            Assert.AreEqual(calendars.Where(x => x.Id == keys[3]).FirstOrDefault().Calscale, CALSCALE.GREGORIAN);

            //this.Teardown();
        }
    }
}
