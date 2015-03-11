using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.operations.concretes.live;
using reexjungle.xcal.service.repositories.contracts;
using ServiceStack.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reexjungle.xcal.service.interfaces.contracts.live
{
    /// <summary>
    /// Specifies a service interface for iCalendar objects
    /// </summary>
    public interface ICalendarService
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
        void Patch(PatchCalendar request);

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        void Patch(PatchCalendars request);

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        void Delete(DeleteCalendar request);

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        void Delete(DeleteCalendars request);

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
        List<string> Get(GetCalendarKeys request);
    }
}