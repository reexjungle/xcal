using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface;
using ServiceStack.FluentValidation;
using reexmonkey.crosscut.essentials.concretes;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.domain.operations;
using reexmonkey.xcal.service.repositories.contracts;
using reexmonkey.xcal.service.interfaces.contracts.live;

namespace reexmonkey.xcal.service.interfaces.concretes.live
{
    public class EventService: Service, IEventService
    {
        public ILog Logger { get; set; }

        public ICalendarRepository CalendarRepository { get; set; }

        public EventService() : base() { }

        public EventService(ICalendarRepository repository, ILog logger): base()
        {
            if (repository == null) throw new ArgumentNullException("repository");
            if (logger == null) throw new ArgumentNullException("logger");
            this.CalendarRepository = repository;
            this.Logger = logger;
        }

        #region VEVENT services based on RFC 5546

        public VCALENDAR Post(PublishEvents request)
        {
            VCALENDAR calendar = null;
            try
            {
                calendar = new VCALENDAR { ProdId = request.ProductId, Method = METHOD.PUBLISH };
                var oevents = calendar.Components.OfType<VEVENT>();
                var toadd = request.Events.Except(oevents, new EqualByStringId<VEVENT>());
                if (!toadd.NullOrEmpty()) calendar.Components.AddRange(toadd);
                this.CalendarRepository.Save(calendar);
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
            return calendar;
        }

        public VCALENDAR Put(RescheduleEvents request)
        {
            VCALENDAR calendar = null;
            try
            {
                calendar = new VCALENDAR { ProdId = request.ProductId, Method = METHOD.REQUEST };
                var oevents = calendar.Components.OfType<VEVENT>();
                var toadd = request.Events.Except(oevents, new EqualByStringId<VEVENT>());
                if (!toadd.NullOrEmpty()) calendar.Components.AddRange(toadd);
                this.CalendarRepository.Save(calendar);
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
            return calendar;
        }

        public VCALENDAR Put(UpdateEvent request)
        {
            throw new NotImplementedException();
        }

        public VCALENDAR Post(DelegateEvent request)
        {
            throw new NotImplementedException();
        }

        public VCALENDAR Put(ChangeOrganizer request)
        {
            throw new NotImplementedException();
        }

        public VCALENDAR Post(SendOnBehalf request)
        {
            throw new NotImplementedException();
        }

        public VCALENDAR Post(ForwardEvent request)
        {
            throw new NotImplementedException();
        }

        public VCALENDAR Put(UpdateAttendeesStatus request)
        {
            throw new NotImplementedException();
        }

        public VCALENDAR Post(ReplyToEvent request)
        {
            throw new NotImplementedException();
        }

        public VCALENDAR Put(AddToEvent request)
        {
            throw new NotImplementedException();
        }

        public VCALENDAR Post(CancelEvent request)
        {
            throw new NotImplementedException();
        }

        public VCALENDAR Post(RefreshEvent request)
        {
            throw new NotImplementedException();
        }

        public VCALENDAR Post(CounterEvent request)
        {
            throw new NotImplementedException();
        }

        public VCALENDAR Post(DeclineCounterEvent request)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region VEVENT general services

        public VCALENDAR Put(DelegateEvent request)
        {
            throw new NotImplementedException();
        }

        public VCALENDAR Patch(ChangeOrganizer request)
        {
            throw new NotImplementedException();
        }

        public VCALENDAR Put(SendOnBehalf request)
        {
            throw new NotImplementedException();
        }

        public VCALENDAR Put(ForwardEvent request)
        {
            throw new NotImplementedException();
        }

        public VCALENDAR Put(ReplyToEvent request)
        {
            throw new NotImplementedException();
        }

        public VCALENDAR Delete(CancelEvent request)
        {
            throw new NotImplementedException();
        }

        public VCALENDAR Get(FindEvent request)
        {
            throw new NotImplementedException();
        }

        public VCALENDAR Get(FindEvents request)
        {
            throw new NotImplementedException();
        }

        public VCALENDAR Get(DeleteEvent request)
        {
            throw new NotImplementedException();
        }

        public VCALENDAR Get(DeleteEvents request)
        {
            throw new NotImplementedException();
        } 

        #endregion

    }
}
