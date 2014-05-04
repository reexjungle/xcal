using System;
using System.Linq;
using System.Collections.Generic;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.domain.operations;
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


        public VEVENT Post(AddEvent request)
        {
            throw new NotImplementedException();
        }

        public List<VEVENT> Post(AddEvents request)
        {
            throw new NotImplementedException();
        }

        public VEVENT Put(UpdateEvent request)
        {
            throw new NotImplementedException();
        }

        public List<VEVENT> Put(UpdateEvents request)
        {
            throw new NotImplementedException();
        }

        public VEVENT Patch(PatchEvent request)
        {
            throw new NotImplementedException();
        }

        public List<VEVENT> Patch(PatchEvents request)
        {
            throw new NotImplementedException();
        }

        public void Delete(DeleteEvent request)
        {
            throw new NotImplementedException();
        }

        public void Delete(DeleteEvents request)
        {
            throw new NotImplementedException();
        }

        public VEVENT Get(FindEvent request)
        {
            throw new NotImplementedException();
        }

        public List<VEVENT> Get(FindEvents request)
        {
            throw new NotImplementedException();
        }
    }
}
