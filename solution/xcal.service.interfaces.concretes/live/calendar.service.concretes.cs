using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.interfaces.contracts.live;
using reexjungle.xcal.service.operations.concretes.live;
using reexjungle.xcal.service.repositories.contracts;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.foundation.contracts;
using reexjungle.xmisc.infrastructure.concretes.io;
using reexjungle.xmisc.infrastructure.contracts;
using ServiceStack.Common;
using ServiceStack.Logging;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace reexjungle.xcal.service.interfaces.concretes.live
{
    /// <summary> Represents a service for iCalendar instances. </summary>
    public class CalendarWebService : Service, ICalendarWebService
    {
        private readonly ILogFactory logFactory;
        private readonly ICalendarRepository repository;
        private readonly ICacheKeyBuilder<Guid> keyBuilder;
        private readonly string nullkey;

        private ILog log;
        private ILog logger
        {
            get
            {
                return log ?? (log = logFactory.GetLogger(GetType()));
            }
        }

        /// <summary> Constructor.</summary>
        /// <param name="repository"> The calendar repository. </param>
        /// <param name="keyBuilder"></param>
        /// <param name="logFactory"> The factory instance to use for logging operations. </param>
        public CalendarWebService(ICalendarRepository repository, ICacheKeyBuilder<Guid> keyBuilder, ILogFactory logFactory)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            if (keyBuilder == null) throw new ArgumentNullException("keyBuilder");
            if (logFactory == null) throw new ArgumentNullException("logFactory");

            this.repository = repository;
            this.logFactory = logFactory;
            this.keyBuilder = keyBuilder;
            nullkey = keyBuilder.NullKey.ToString();
        }

        /// <summary> Adds an iCalendar instance to the service repository. </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs. </exception>
        /// <param name="request"> The request to add the iCalendar instance </param>
        public void Post(AddCalendar request)
        {
            try
            {
                if (!repository.ContainsKey(request.Calendar.Id))
                {
                    repository.Save(request.Calendar);

                    var key = keyBuilder.Build(request, x => x.Calendar.Id).ToString();
                    RequestContext.RemoveFromCache(Cache, key);
                }

            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (ApplicationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>  Adds an iCalendar instances to the service repository.</summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid.</exception>
        /// <exception cref="ApplicationException">Thrown when an Application error condition occurs.</exception>
        /// <param name="request"> The request to add iCalendar instancxes.</param>
        public void Post(AddCalendars request)
        {
            try
            {
                var keys = request.Calendars.Select(x => x.Id).Distinct();
                if (!repository.ContainsKeys(keys, ExpectationMode.Pessimistic))
                {
                    repository.SaveAll(request.Calendars);

                    var key = keyBuilder.Build(request, x => keys).ToString();
                    RequestContext.RemoveFromCache(Cache, key, nullkey);
                }
            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (ApplicationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
        }

        /// <summary> Updates an iCalendar instance in the repository.</summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid.</exception>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs.</exception>
        /// <param name="request"> The request to update the iCalendar instance.</param>
        public void Put(UpdateCalendar request)
        {
            try
            {
                if (repository.ContainsKey(request.Calendar.Id))
                {
                    repository.Save(request.Calendar);
                    var key = keyBuilder.Build(request, x => x.Calendar.Id).ToString();
                    RequestContext.RemoveFromCache(Cache, key, nullkey);

                }
            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (ApplicationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString()); 
                throw;
            }
        }

        /// <summary> Updates iCalendar instances in the web repository.</summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid.</exception>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs.</exception>
        /// <param name="request"> The request to update the iCalendar instances.</param>
        public void Put(UpdateCalendars request)
        {
            try
            {
                var keys = request.Calendars.Select(x => x.Id).Distinct();
                if (repository.ContainsKeys(keys, ExpectationMode.Pessimistic))
                {
                    repository.SaveAll(request.Calendars);

                    var key = keyBuilder.Build(request, x => keys).ToString();
                    RequestContext.RemoveFromCache(Cache, key, nullkey);
                }
            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (ApplicationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString()); 
                throw;
            }
        }

        /// <summary> Patches an iCalendar instance in the service repository.</summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid.</exception>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs.</exception>
        /// <param name="request"> The request to patch an iCalendar instance</param>
        public void Post(PatchCalendar request)
        {
            try
            {
                var source = new VCALENDAR
                {
                    ProdId = request.ProductId,
                    Calscale = request.Scale,
                    Version = request.Version,
                    Method = request.Method,
                    Events = request.Events,
                    ToDos = request.ToDos,
                    FreeBusies = request.FreeBusies,
                    Journals = request.Journals,
                    TimeZones = request.TimeZones,
                    IanaComponents = request.IanaComponents,
                    XComponents = request.XComponents
                };

                var fields = new List<string>();
                if (!string.IsNullOrEmpty(source.ProdId) || !string.IsNullOrEmpty(source.ProdId))
                    fields.Add("ProdId");
                if (source.Calscale != CALSCALE.UNKNOWN) fields.Add("Calscale");
                if (source.Method != METHOD.UNKNOWN) fields.Add("Method");
                if (!source.Events.NullOrEmpty()) fields.Add("Events");
                if (!source.ToDos.NullOrEmpty()) fields.Add("ToDos");
                if (!source.FreeBusies.NullOrEmpty()) fields.Add("FreeBusies");
                if (!source.Journals.NullOrEmpty()) fields.Add("Journals");
                if (!source.TimeZones.NullOrEmpty()) fields.Add("TimeZones");
                if (!source.IanaComponents.NullOrEmpty()) fields.Add("IanaComponents");
                if (!source.XComponents.NullOrEmpty()) fields.Add("XComponents");


                repository.Patch(source, fields, request.CalendarId.ToSingleton());

                var key = keyBuilder.Build(request, x => x.CalendarId).ToString();
                RequestContext.RemoveFromCache(Cache, key);

            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (ApplicationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString()); 
                throw;
            }
        }

        /// <summary> Patches the given request.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid.</exception>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs.</exception>
        /// <param name="request"> The request to patch iCalendar instances.</param>
        public void Post(PatchCalendars request)
        {
            try
            {
                var source = new VCALENDAR
                {
                    ProdId = request.ProductId,
                    Calscale = request.Scale,
                    Version = request.Version,
                    Method = request.Method,
                    Events = request.Events,
                    ToDos = request.ToDos,
                    FreeBusies = request.FreeBusies,
                    Journals = request.Journals,
                    TimeZones = request.TimeZones,
                    IanaComponents = request.IanaComponents,
                    XComponents = request.XComponents
                };

                var fields = new List<string>();
                if (!string.IsNullOrEmpty(source.ProdId) || !string.IsNullOrWhiteSpace(source.ProdId))
                    fields.Add("ProdId");
                if (source.Calscale != default(CALSCALE)) fields.Add("Calscale");
                if (source.Method != default(METHOD)) fields.Add("Method");
                if (!source.Events.NullOrEmpty()) fields.Add("Events");
                if (!source.ToDos.NullOrEmpty()) fields.Add("ToDos");
                if (!source.FreeBusies.NullOrEmpty()) fields.Add("FreeBusies");
                if (!source.Journals.NullOrEmpty()) fields.Add("Journals");
                if (!source.TimeZones.NullOrEmpty()) fields.Add("TimeZones");
                if (!source.IanaComponents.NullOrEmpty()) fields.Add("IanaComponents");
                if (!source.XComponents.NullOrEmpty()) fields.Add("XComponents");

                var calendarIds = !request.CalendarIds.NullOrEmpty()
                    ? request.CalendarIds.Distinct()
                    : null;

                repository.Patch(source, fields, calendarIds);

                var key = keyBuilder.Build(request, x => calendarIds).ToString();
                RequestContext.RemoveFromCache(Cache, key, nullkey);

            }
            catch (ArgumentNullException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (ApplicationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString()); throw;
            }
        }

        /// <summary> Deletes a given iCalendar instance from service repository.</summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid.</exception>
        /// <exception cref="ApplicationException">Thrown when an Application error condition occurs.</exception>
        /// <param name="request"> The request to delete the iCalendar instance.</param>
        public void Delete(DeleteCalendar request)
        {
            try
            {
                repository.Erase(request.CalendarId);

                var key = keyBuilder.Build(request, x => x.CalendarId).ToString();
                RequestContext.RemoveFromCache(Cache, key);

            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (ApplicationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString()); throw;
            }
        }

        /// <summary> Deletes given iCalendar instances from the service repository.</summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid.</exception>
        /// <exception cref="ApplicationException">Thrown when an Application error condition occurs.</exception>
        /// <param name="request"> The request to delete.</param>
        public void Post(DeleteCalendars request)
        {
            try
            {
                var calendarIds = request.CalendarIds.NullOrEmpty()
                    ? Enumerable.Empty<Guid>()
                    : request.CalendarIds.Distinct();

                repository.EraseAll(calendarIds);

                var key = keyBuilder.Build(request, x => calendarIds).ToString();
                RequestContext.RemoveFromCache(Cache, key, nullkey);

            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (ApplicationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString()); 
                throw;
            }
        }

        /// <summary> Gets the requested iCalendar instance.</summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid.</exception>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs.</exception>
        /// <param name="request"> The request to retrieve the iCalendar instance.</param>
        /// <returns> A VCALENDAR object when found; otherwise null.</returns>
        public VCALENDAR Get(FindCalendar request)
        {
            try
            {
                return repository.Find(request.CalendarId);
            }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }

        /// <summary> Gets the requested iCalendar instances specified by keys</summary>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null.</exception>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid.</exception>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs.</exception>
        /// <param name="request"> The request to retrieve iCalendar instances.</param>
        /// <returns>A list of found VCALENDAR objects; otherwise an empty list</returns>
        public List<VCALENDAR> Post(FindCalendars request)
        {
            try
            {
                return request.Page != null && request.Size != null
                    ? repository.FindAll(request.CalendarIds, (request.Page.Value - 1) * request.Size.Value, request.Size.Value).ToList()
                    : repository.FindAll(request.CalendarIds).ToList();
            }
            catch (ArgumentNullException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (ApplicationException ex)
            {
                logger.Error(ex.ToString()); 
                throw;
            }
        }

        /// <summary> Gets paginated set of iCalendar instances.</summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid.</exception>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs.</exception>
        /// <param name="request"> The request to get.</param>
        /// <returns>A list of found VCALENDAR objects per result page; otherwise an empty list.</returns>
        public List<VCALENDAR> Get(GetCalendars request)
        {
            try
            {
                return request.Page != null && request.Size != null
                    ? repository.Get((request.Page.Value - 1)*request.Size.Value, request.Size.Value).ToList()
                    : repository.Get().ToList();
            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (ApplicationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString()); 
                throw;
            }
        }

        /// <summary> Gets the requested iCalendar IDs.</summary>
        /// <param name="request"> The request to get.</param>
        /// <returns>A list of found VCALENDAR IDs per result page; otherwise an empty list.</returns>
        public List<Guid> Get(GetCalendarKeys request)
        {
            try
            {
                return request.Page != null && request.Size != null
                    ? repository.GetKeys((request.Page.Value - 1)*request.Size.Value, request.Size.Value).ToList()
                    : repository.GetKeys().ToList();
            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (ApplicationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
        }
    }
}