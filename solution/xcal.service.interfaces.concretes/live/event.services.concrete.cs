using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface;
using ServiceStack.FluentValidation;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.domain.operations;
using reexmonkey.xcal.domain.extensions;
using reexmonkey.xcal.service.repositories.contracts;
using reexmonkey.xcal.service.interfaces.contracts.live;

namespace reexmonkey.xcal.service.interfaces.concretes.live
{
    public class EventService: Service, IEventService
    {
        private ILogFactory logfactory;
        private ICalendarRepository repository;

        private ILog log = null;
        private ILog logger
        {
            get { return (log != null)? this.log: this.logfactory.GetLogger(this.GetType()); }
        }

        public ILogFactory LogFactory 
        {
            get { return this.logfactory; } 
            set
            {
                if (value == null) throw new ArgumentNullException("Logger");
                this.logfactory = value;
                this.log = logfactory.GetLogger(this.GetType());
            }
        }
        public ICalendarRepository CalendarRepository
        {
            get { return this.repository; }
            set 
            {
                if (value == null) throw new ArgumentNullException("CalendarRepository");
                this.repository = value; 
            }
        }

        public EventService() : base() 
        {
            this.CalendarRepository = this.TryResolve<ICalendarRepository>();
            this.LogFactory = this.TryResolve<ILogFactory>();
        }

        public EventService(ICalendarRepository repository, ILogFactory logger)
            : base()
        {
            this.CalendarRepository = repository;
            this.LogFactory = logger;
        }

        #region VEVENT services based on RFC 5546

        public VCALENDAR Post(PublishEvent request)
        {

            VCALENDAR calendar = null;
            try
            {
                calendar = (this.repository.Find(request.ProductId)) ?? 
                    new VCALENDAR { ProdId = request.ProductId, Method = METHOD.PUBLISH };

                calendar.Components.AddRangeComplement(request.Events);
                this.repository.Save(calendar);
            }

            catch (ArgumentNullException ex) { this.logger.Error(ex.ToString()); }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); }
            catch (Exception ex) { this.logger.Error(ex.ToString()); }
            return this.repository.Hydrate(calendar);
        }

        public VCALENDAR Patch(RescheduleEvent request)
        {
            VCALENDAR calendar = null;
            try
            {
                calendar = (this.repository.Find(request.ProductId)) ??
                    new VCALENDAR { ProdId = request.ProductId, Method = METHOD.REQUEST };

                var source = request.Events.FirstOrDefault();
                this.repository.EventRepository.Patch(source,
                    x => new { x.Start, x.End, x.Description, x.Location, x.RecurrenceRule, x.Sequence, x.LastModified },
                    p => p.Uid == source.Uid);

                calendar.Components.AddRangeComplement(request.Events);
                this.repository.Save(calendar);
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
            return calendar;
        }

        public VCALENDAR Put(UpdateEvent request)
        {
            VCALENDAR calendar = null;
            try
            {
                calendar = (this.repository.Find(request.ProductId)) ??
                    new VCALENDAR { ProdId = request.ProductId, Method = METHOD.REQUEST };

                var patch = request.Events.FirstOrDefault();
                this.repository.EventRepository.Patch(patch,
                    x => new { x.Summary, x.Geo, x.Priority, x.Transparency, x.Status, x.Attendees, x.Attachments, x.Categories, x.Classification, x.Comments, x.Contacts, x.Sequence, x.LastModified },
                    p => p.Uid == patch.Uid);

                calendar.Components.AddRangeComplement(request.Events);
                this.repository.Save(calendar);
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
            return calendar;
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
