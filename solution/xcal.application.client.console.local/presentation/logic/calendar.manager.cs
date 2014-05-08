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
            client.Delete(new DeleteCalendars());

            var calendars = new VCALENDAR[5];
            for (int i = 0; i < 5; i++)
            {
                calendars[i] = new VCALENDAR
                {
                    Id = this.GuidKeyGenerator.GetNextKey(),
                    ProdId = this.FpiKeyGenerator.GetNextKey()
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

            var retrieved = this.client.Get(new FindCalendars { CalendarIds = keys });

            calendars[3].Calscale = CALSCALE.ISLAMIC;
            calendars[4].Version = "2.0";
            this.client.Put(new UpdateCalendars { Calendars = calendars.ToList() });
            retrieved = this.client.Get(new FindCalendars { CalendarIds = keys });

            this.client.Patch(new PatchCalendars
            {
                Scale = CALSCALE.JULIAN,
                CalendarIds = new List<string> { keys[0], keys[1], keys[2] }
            });

            retrieved = this.client.Get(new FindCalendars
            {
                CalendarIds = new List<string> { keys[0], keys[1], keys[2] }
            });

        }

    }
}
