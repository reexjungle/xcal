using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.operations.concretes.live;
using System;
using System.Collections.Generic;

namespace reexjungle.xcal.service.interfaces.contracts.live
{
    /// <summary>
    /// Specifies a service interface for iCalendar objects
    /// </summary>
    public interface ICalendarWebService
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        void Post(AddCalendar request);

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        void Post(AddCalendars request);

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        void Put(UpdateCalendar request);

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        void Put(UpdateCalendars request);

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        void Post(PatchCalendar request);

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        void Post(PatchCalendars request);

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        void Delete(DeleteCalendar request);

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        void Post(DeleteCalendars request);

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VCALENDAR Get(FindCalendar request);

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        List<VCALENDAR> Post(FindCalendars request);

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        List<VCALENDAR> Get(GetCalendars request);

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        List<Guid> Get(GetCalendarKeys request);
    }
}