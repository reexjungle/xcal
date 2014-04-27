using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.ServiceClient.Web;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.infrastructure.operations.contracts;
using reexmonkey.infrastructure.operations.concretes;
using reexmonkey.xcal.domain.operations;


namespace xcal.application.client.console.local.presentation.logic
{
    public class EventManager
    {
        private IFPIKeyGenerator fpikeygen;
        private ServiceClientBase sclient;

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
            get { return this.sclient; }
            set { this.sclient = value; }
        }


        public void PublishMinimalEvent()
        {
            var pevent = new VEVENT
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

            var events = new List<VEVENT> { pevent };
            var eventstring = pevent.ToString();
            
            //var published = sclient.Post<VCALENDAR>(new PublishEvent
            //{
            //    CalendarId = this.GuidKeyGenerator.GetNextKey(),
            //    ProductId = this.FpiKeyGenerator.GetNextKey(),
            //    Events = events,
            //    TimeZones = null
            //});

            //var publishedstring = published.ToString();

        }
    }
}
