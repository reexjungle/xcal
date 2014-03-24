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
using reexmonkey.xcal.domain.extensions;
using reexmonkey.xcal.service.repositories.contracts;
using reexmonkey.xcal.service.interfaces.contracts.live;

namespace reexmonkey.xcal.service.interfaces.concretes.live
{
    public class EventService: Service, IEventService
    {
        private ILog logger;
        private ICalendarRepository repository;

        public ILog Logger 
        {
            get { return this.logger; } 
            set
            {
                if (value == null) throw new ArgumentNullException("Logger");
                this.logger = value;
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

        public EventService() : base() { }
        public EventService(ICalendarRepository repository, ILog logger)
            : base()
        {
            this.CalendarRepository = repository;
            this.Logger = logger;
        }

        #region VEVENT services based on RFC 5546

        public VCALENDAR Post(PublishEvent request)
        {
            VCALENDAR calendar = null;
            try
            {
                calendar = (this.repository.Find(request.ProductId)) ?? 
                    new VCALENDAR { ProdId = request.ProductId, Method = METHOD.PUBLISH };

                this.repository.EventRepository.SaveAll(request.Events);

                var existing = calendar.Components.OfType<VEVENT>();

                //get new incoming evenzs
                var incoming = (!existing.NullOrEmpty())?
                    request.Events.Except(existing):
                    request.Events;

                //determine conflicting existing events
                var conflicts = (!existing.NullOrEmpty())? existing.Intersect(request.Events): null;
                if(!conflicts.NullOrEmpty())
                {
                    calendar.Components.RemoveAll(x => (x is VEVENT && conflicts.Contains(((VEVENT)x))));
                    calendar.Components.AddRange(conflicts);
                }
                    
                calendar.Components.AddRange(incoming);
                this.repository.Save(calendar);

            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); }
            catch (Exception ex) { this.logger.Error(ex.ToString()); }
            return calendar;
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
                    x => new { x.Start, x.End, x.Description, x.Location, x.RecurrenceRule, x.Sequence, x.LastModified}, 
                    p => p.Uid == source.Uid);

                var existing = calendar.Components.OfType<VEVENT>();

                //get new incoming events
                var incoming = (!existing.NullOrEmpty()) ?
                    request.Events.Except(existing) :
                    request.Events;

                //determine conflicting existing events
                var conflicts = (!existing.NullOrEmpty()) ? existing.Intersect(request.Events) : null;
                if (!conflicts.NullOrEmpty())
                {
                    calendar.Components.RemoveAll(x => (x is VEVENT && conflicts.Contains(((VEVENT)x))));
                    calendar.Components.AddRange(conflicts);
                }

                calendar.Components.AddRange(incoming);
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
                    x => new {x.Summary, x.Geo, x.Priority, x.Transparency, x.Status, x.Attendees, x.Attachments, x.Categories, x.Classification, x.Comments, x.Contacts, x.Sequence, x.LastModified },
                    p => p.Uid == patch.Uid);

                var existing = calendar.Components.OfType<VEVENT>();
                //get new incoming evenzs
                var incoming = (!existing.NullOrEmpty()) ?
                    request.Events.Except(existing) :
                    request.Events;

                //determine conflicting existing events
                var conflicts = (!existing.NullOrEmpty()) ? existing.Intersect(request.Events) : null;
                if (!conflicts.NullOrEmpty())
                {
                    calendar.Components.RemoveAll(x => (x is VEVENT && conflicts.Contains(((VEVENT)x))));
                    calendar.Components.AddRange(conflicts);
                }

                calendar.Components.AddRange(incoming);
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
