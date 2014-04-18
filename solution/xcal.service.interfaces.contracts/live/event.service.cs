using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Logging;
using reexmonkey.xcal.service.repositories.contracts;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.domain.operations;


namespace reexmonkey.xcal.service.interfaces.contracts.live
{
    public interface IEventService
    {
        #region  RFC 5546

        VCALENDAR Get(PublishEvent request);

        VCALENDAR Patch(RescheduleEvent request);

        VCALENDAR Put(UpdateEvent request);

        VCALENDAR Put(DelegateEvent request);

        VCALENDAR Patch(ChangeOrganizer request);

        VCALENDAR Put(SendOnBehalf request);

        VCALENDAR Put(ForwardEvent request);

        VCALENDAR Put(UpdateAttendeesStatus request);

        VCALENDAR Put(ReplyToEvent request);

        VCALENDAR Put(AddToEvent request);

        VCALENDAR Delete(CancelEvent request);

        VCALENDAR Post (RefreshEvent request);

        VCALENDAR Post(CounterEvent request);

        VCALENDAR Post(DeclineCounterEvent request);

        #endregion

        #region general

        VCALENDAR Get(FindEvent request);

        VCALENDAR Get(FindEvents request);

        VCALENDAR Get(DeleteEvent request);

        VCALENDAR Get(DeleteEvents request);

        #endregion
    }
}
