using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface;
using ServiceStack.FluentValidation;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.domain.operations;
using reexmonkey.xcal.service.repositories.contracts;

namespace reexmonkey.xcal.service.interfaces.live
{
    public class EventService: Service
    {
        public ILog Logger { get; set; }
        public IEventRepository Repository { get; set; }
        public EventService(): base()
        {
            //this.Logger = LogManager.GetLogger(typeof(EventService));
            //this.Repository = this.ResolveService<IEventRepository>();
        }

        public EventService(IEventRepository repository, ILog logger): base()
        {
            if (repository == null) throw new ArgumentNullException("repository");
            if (logger == null) throw new ArgumentNullException("logger");
            this.Repository = repository;
            this.Logger = logger;
        }

        #region VEVENT services based on RFC 5546

        public VCALENDAR Post(PublishEvents request)
        {
            VCALENDAR calendar = null;

            try
            {

            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }

            return calendar;
        }

        public VCALENDAR Post(RescheduleEvents request)
        {
            throw new NotImplementedException();
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


    }
}
