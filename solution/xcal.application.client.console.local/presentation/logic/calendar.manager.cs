using reexmonkey.infrastructure.operations.contracts;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.domain.operations;
using ServiceStack.ServiceClient.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xcal.application.client.console.local.presentation.logic
{
    public class CalendarManager
    {
        private IFPIKeyGenerator fpikeygen;
        private ServiceClientBase client;

        public IGuidKeyGenerator GuidKeyGenerator { get; set; }

        public IFPIKeyGenerator FpiKeyGenerator
        {
            get { return this.fpikeygen; }
            set
            {
                if (value == null) throw new ArgumentNullException("FpiKeyGenerator");
                this.fpikeygen = value;
            }
        }

        public ServiceClientBase ServiceClient
        {
            get { return this.client; }
            set { this.client = value; }
        }

        public void MaintainMultipleCalendars()
        {
            client.Post(new FlushDatabase { Hard = false });

            var events = new VEVENT[5];
            for (int i = 0; i < 5; i++)
            {
                events[i] = new VEVENT
                {
                    Uid = this.GuidKeyGenerator.GetNextKey(),
                    RecurrenceId = new RECURRENCE_ID
                    {
                        Id = this.GuidKeyGenerator.GetNextKey(),
                        Range = RANGE.THISANDFUTURE,
                        Value = new DATE_TIME(new DateTime(2014, 6, 15, 16, 07, 01, 0, DateTimeKind.Utc))
                    },
                    RecurrenceRule = new RECUR
                    {
                        Id = this.GuidKeyGenerator.GetNextKey(),
                        FREQ = FREQ.DAILY,
                        Format = RecurFormat.DateTime,
                        UNTIL = new DATE_TIME(new DateTime(2014, 6, 25, 18, 03, 08, 0, DateTimeKind.Utc))
                    },

                    Organizer = new ORGANIZER
                    {
                        Id = this.GuidKeyGenerator.GetNextKey(),
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
            }

            var calendars = new VCALENDAR[5];
            for (int i = 0; i < 5; i++)
            {
                calendars[i] = new VCALENDAR
                {
                    Id = this.GuidKeyGenerator.GetNextKey(),
                    ProdId = this.FpiKeyGenerator.GetNextKey(),
                    Events = events.ToList()
                };
            }

            //customize calendars
            calendars[0].Method = METHOD.PUBLISH;
            calendars[1].Method = METHOD.REQUEST;
            calendars[2].Method = METHOD.REFRESH;
            calendars[3].Method = METHOD.ADD;
            calendars[4].Method = METHOD.CANCEL;

            this.client.Post(new AddCalendars { Calendars = calendars.ToList() });
            var keys = calendars.Select(x => x.Id).ToList();

            var retrieved = this.client.Post(new FindCalendars { CalendarIds = keys });

            calendars[0].Method = METHOD.REQUEST;
            calendars[0].Version = "3.0";
            calendars[0].Calscale = CALSCALE.HEBREW;

            //remove 4 events and update
            calendars[0].Events.RemoveRange(0, 4);



            this.client.Put(new UpdateCalendar { Calendar = calendars[0] });
            var found = this.client.Get(new FindCalendar { CalendarId = calendars[0].Id });

            //calendars[3].Calscale = CALSCALE.ISLAMIC;
            //calendars[4].Version = "2.0";
            //this.client.Put(new UpdateCalendars { Calendars = calendars.ToList() });
            
            retrieved = this.client.Post(new FindCalendars { CalendarIds = keys });
            this.client.Delete(new DeleteCalendar { CalendarId = found.Id });
            //this.client.Patch(new PatchCalendars
            //{
            //    Scale = CALSCALE.JULIAN,
            //    CalendarIds = new List<string> { keys[0], keys[1], keys[2] }
            //});

            //retrieved = this.client.Post(new FindCalendars
            //{
            //    CalendarIds = new List<string> { keys[0], keys[1], keys[2] }
            //});

        }

    }
}
