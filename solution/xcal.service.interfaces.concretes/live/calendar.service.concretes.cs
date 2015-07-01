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
    public class CalendarService : Service, ICalendarService
    {
        private readonly ILogFactory logFactory;
        private readonly ICalendarRepository repository;

        private ILog log = null;

        private ILog logger
        {
            get
            {
                return log ?? (log = logFactory.GetLogger(GetType()));
            }
        }

        /// <summary> Constructor.</summary>
        /// <param name="repository"> The calendar repository. </param>
        /// <param name="logFactory"> The factory instance to use for logging operations. </param>
        public CalendarService(ICalendarRepository repository, ILogFactory logFactory)
            : base()
        {
            this.repository = repository;
            this.logFactory = logFactory;
        }

        /// <summary> Adds an iCalendar instance to the service repository. </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs. </exception>
        /// <param name="request"> The request to add the iCalendar instance </param>
        public void Post(AddCalendar request)
        {
            try
            {
                var cacheKey = UrnId.Create<VCALENDAR>(request.Calendar.Id);
                if (!repository.ContainsKey(request.Calendar.Id))
                    repository.Save(request.Calendar);

                RequestContext.RemoveFromCache(Cache, cacheKey);
            }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }

        /// <summary>  Adds an iCalendar instances to the service repository.</summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid.</exception>
        /// <exception cref="ApplicationException">Thrown when an Application error condition occurs.</exception>
        /// <param name="request"> The request to add iCalendar instancxes.</param>
        public void Post(AddCalendars request)
        {
            try
            {
                var keys = request.Calendars.Select(x => x.Id).ToArray();
                if (!repository.ContainsKeys(keys, ExpectationMode.Pessimistic))
                {
                    repository.SaveAll(request.Calendars);
                }

                RequestContext.RemoveFromCache(Cache, "urn:calendars");
            }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }

        /// <summary> Updates an iCalendar instance in the repository.</summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid.</exception>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs.</exception>
        /// <param name="request"> The request to update the iCalendar instance.</param>
        public void Put(UpdateCalendar request)
        {
            try
            {
                var cacheKey = UrnId.Create<VCALENDAR>(request.Calendar.Id);
                if (repository.ContainsKey(request.Calendar.Id))
                {
                    repository.Save(request.Calendar);
                }
                RequestContext.RemoveFromCache(Cache, cacheKey);
            }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }

        /// <summary> Updates iCalendar instances in the web repository.</summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid.</exception>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs.</exception>
        /// <param name="request"> The request to update the iCalendar instances.</param>
        public void Put(UpdateCalendars request)
        {
            try
            {
                var keys = request.Calendars.Select(x => x.Id).ToArray();

                if (repository.ContainsKeys(keys, ExpectationMode.Pessimistic))
                {
                    repository.SaveAll(request.Calendars);
                }

                RequestContext.RemoveFromCache(Cache, "urn:calendars");
            }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }

        /// <summary> Patches an iCalendar instance in the service repository.</summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid.</exception>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs.</exception>
        /// <param name="request"> The request to patch an iCalendar instance</param>
        public void Patch(PatchCalendar request)
        {
            try
            {
                var cacheKey = UrnId.Create<VCALENDAR>(request.CalendarId);
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

                var fieldlist = new List<string>();
                if (!string.IsNullOrEmpty(source.ProdId) || !string.IsNullOrEmpty(source.ProdId)) fieldlist.Add("ProdId");
                if (source.Calscale != CALSCALE.UNKNOWN) fieldlist.Add("Calscale");
                if (source.Method != METHOD.UNKNOWN) fieldlist.Add("Method");
                if (!source.Events.NullOrEmpty()) fieldlist.Add("Events");
                if (!source.ToDos.NullOrEmpty()) fieldlist.Add("ToDos");
                if (!source.FreeBusies.NullOrEmpty()) fieldlist.Add("FreeBusies");
                if (!source.Journals.NullOrEmpty()) fieldlist.Add("Journals");
                if (!source.TimeZones.NullOrEmpty()) fieldlist.Add("TimeZones");
                if (!source.IanaComponents.NullOrEmpty()) fieldlist.Add("IanaComponents");
                if (!source.XComponents.NullOrEmpty()) fieldlist.Add("XComponents");

                var fieldstr = string.Format("x => new {{ {0} }}", string.Join(", ", fieldlist.Select(x => string.Format("x.{0}", x))));
                var fieldexpr = fieldstr.CompileToExpressionFunc<VCALENDAR, object>(CodeDomLanguage.csharp, "System.dll", "System.Core.dll", typeof(VCALENDAR).Assembly.Location, typeof(IContainsKey<Guid>).Assembly.Location);
                repository.Patch(source, fieldexpr, request.CalendarId.ToSingleton());

                RequestContext.RemoveFromCache(Cache, cacheKey);
            }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }

        /// <summary> Patches the given request.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid.</exception>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs.</exception>
        /// <param name="request"> The request to patch iCalendar instances.</param>
        public void Patch(PatchCalendars request)
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

                var fieldlist = new List<string>();
                if (!string.IsNullOrEmpty(source.ProdId) || !string.IsNullOrWhiteSpace(source.ProdId)) fieldlist.Add("ProdId");
                if (source.Calscale != default(CALSCALE)) fieldlist.Add("Calscale");
                if (source.Method != default(METHOD)) fieldlist.Add("Method");
                if (!source.Events.NullOrEmpty()) fieldlist.Add("Events");
                if (!source.ToDos.NullOrEmpty()) fieldlist.Add("ToDos");
                if (!source.FreeBusies.NullOrEmpty()) fieldlist.Add("FreeBusies");
                if (!source.Journals.NullOrEmpty()) fieldlist.Add("Journals");
                if (!source.TimeZones.NullOrEmpty()) fieldlist.Add("TimeZones");
                if (!source.IanaComponents.NullOrEmpty()) fieldlist.Add("IanaComponents");
                if (!source.XComponents.NullOrEmpty()) fieldlist.Add("XComponents");

                var fieldstr = string.Format("x => new {{ {0} }}", string.Join(", ", fieldlist.Select(x => string.Format("x.{0}", x))));
                var fieldexpr = fieldstr.CompileToExpressionFunc<VCALENDAR, object>(CodeDomLanguage.csharp, "System.dll", "System.Core.dll", typeof(VCALENDAR).Assembly.Location, typeof(IContainsKey<Guid>).Assembly.Location);
                repository.Patch(source, fieldexpr, request.CalendarIds.Distinct());

                RequestContext.RemoveFromCache(Cache, "urn:calendars");
            }
            catch (ArgumentNullException ex) { logger.Error(ex.ToString()); throw; }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }

        /// <summary> Deletes a given iCalendar instance from service repository.</summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid.</exception>
        /// <exception cref="ApplicationException">Thrown when an Application error condition occurs.</exception>
        /// <param name="request"> The request to delete the iCalendar instance.</param>
        public void Delete(DeleteCalendar request)
        {
            try
            {
                var cacheKey = UrnId.Create<VCALENDAR>(request.CalendarId);
                repository.Erase(request.CalendarId);

                RequestContext.RemoveFromCache(Cache, cacheKey);
            }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }

        /// <summary> Deletes given iCalendar instances from the service repository.</summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid.</exception>
        /// <exception cref="ApplicationException">Thrown when an Application error condition occurs.</exception>
        /// <param name="request"> The request to delete.</param>
        public void Delete(DeleteCalendars request)
        {
            try
            {
                repository.EraseAll(request.CalendarIds);
                RequestContext.RemoveFromCache(Cache, "urn:calendars");
            }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
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
                IEnumerable<VCALENDAR> calendars = null;
                if (request.Page != null && request.Size != null)
                {
                    calendars = repository.FindAll(request.CalendarIds, (request.Page.Value - 1) * request.Size.Value, request.Size.Value);
                }
                else calendars = repository.FindAll(request.CalendarIds);
                return !calendars.NullOrEmpty() ? calendars.ToList() : new List<VCALENDAR>();
            }
            catch (ArgumentNullException ex) { logger.Error(ex.ToString()); throw; }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
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
                IEnumerable<VCALENDAR> calendars = null;
                if (request.Page != null && request.Size != null)
                    calendars = repository.Get((request.Page.Value - 1) * request.Size.Value, request.Size.Value);
                else
                    calendars = repository.Get();

                return !calendars.NullOrEmpty() ? calendars.ToList() : new List<VCALENDAR>();
            }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }

        /// <summary> Gets the requested iCalendar IDs.</summary>
        /// <param name="request"> The request to get.</param>
        /// <returns>A list of found VCALENDAR IDs per result page; otherwise an empty list.</returns>
        public List<Guid> Get(GetCalendarKeys request)
        {
            try
            {
                IEnumerable<Guid> keys;
                if (request.Page != null && request.Size != null)
                    keys = repository.GetKeys((request.Page.Value - 1) * request.Size.Value, request.Size.Value);
                else keys = repository.GetKeys();
                return !keys.NullOrEmpty() ? keys.ToList() : new List<Guid>();
            }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }
    }
}