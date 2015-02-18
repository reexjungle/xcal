using reexjungle.foundation.essentials.concretes;
using reexjungle.foundation.essentials.contracts;
using reexjungle.infrastructure.concretes.operations;
using reexjungle.infrastructure.contracts;
using reexjungle.infrastructure.io.concretes;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.interfaces.contracts.live;
using reexjungle.xcal.service.operations.concretes.live;
using reexjungle.xcal.service.repositories.contracts;
using ServiceStack.Common;
using ServiceStack.Logging;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace reexjungle.xcal.service.interfaces.concretes.live
{
    public class CalendarService : Service, ICalendarService
    {
        private ILogFactory logfactory;
        private ICalendarRepository repository;

        private ILog log = null;

        private ILog logger
        {
            get { return (log != null) ? this.log : this.logfactory.GetLogger(this.GetType()); }
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

        public CalendarService()
            : base()
        {
            this.CalendarRepository = this.TryResolve<ICalendarRepository>();
            this.LogFactory = this.TryResolve<ILogFactory>();
        }

        public CalendarService(ICalendarRepository repository, ILogFactory logger)
            : base()
        {
            this.CalendarRepository = repository;
            this.LogFactory = logger;
        }

        public void Post(AddCalendar request)
        {
            try
            {
                if (!this.repository.ContainsKey(request.Calendar.Id))
                    this.repository.Save(request.Calendar);

                base.RequestContext.RemoveFromCache(base.Cache, "urn:calendars");
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public void Post(AddCalendars request)
        {
            try
            {
                var keys = request.Calendars.Select(x => x.Id).ToArray();
                if (!this.repository.ContainsKeys(keys, ExpectationMode.pessimistic))
                {
                    this.repository.SaveAll(request.Calendars);
                }

                base.RequestContext.RemoveFromCache(base.Cache, "urn:calendars");
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public void Put(UpdateCalendar request)
        {
            try
            {
                var cacheKey = UrnId.Create<VCALENDAR>(request.Calendar.Id);
                if (this.repository.ContainsKey(request.Calendar.Id))
                {
                    this.repository.Save(request.Calendar);
                }
                base.RequestContext.RemoveFromCache(base.Cache, cacheKey);
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public void Put(UpdateCalendars request)
        {
            try
            {
                var keys = request.Calendars.Select(x => x.Id).ToArray();

                if (this.repository.ContainsKeys(keys, ExpectationMode.pessimistic))
                {
                    this.repository.SaveAll(request.Calendars);
                }

                base.RequestContext.RemoveFromCache(base.Cache, "urn:calendars");
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

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
                var fieldexpr = fieldstr.CompileToExpressionFunc<VCALENDAR, object>(CodeDomLanguage.csharp, new string[] { "System.dll", "System.Core.dll", typeof(VCALENDAR).Assembly.Location, typeof(IContainsKey<string>).Assembly.Location });
                this.repository.Patch(source, fieldexpr, request.CalendarId.ToSingleton());

                base.RequestContext.RemoveFromCache(base.Cache, cacheKey);
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

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
                var fieldexpr = fieldstr.CompileToExpressionFunc<VCALENDAR, object>(CodeDomLanguage.csharp, new string[] { "System.dll", "System.Core.dll", typeof(VCALENDAR).Assembly.Location, typeof(IContainsKey<string>).Assembly.Location });
                this.repository.Patch(source, fieldexpr, request.CalendarIds.Distinct());

                base.RequestContext.RemoveFromCache(base.Cache, "urn:calendars");
            }
            catch (ArgumentNullException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public void Delete(DeleteCalendar request)
        {
            try
            {
                var cacheKey = UrnId.Create<VCALENDAR>(request.CalendarId);
                this.repository.Erase(request.CalendarId);

                base.RequestContext.RemoveFromCache(base.Cache, cacheKey);
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public void Delete(DeleteCalendars request)
        {
            try
            {
                this.repository.EraseAll(request.CalendarIds);
                base.RequestContext.RemoveFromCache(base.Cache, "urn:calendars");
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public VCALENDAR Get(FindCalendar request)
        {
            try
            {
                return this.repository.Find(request.CalendarId);
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public List<VCALENDAR> Post(FindCalendars request)
        {
            try
            {
                IEnumerable<VCALENDAR> calendars = null;
                if (request.Page != null && request.Size != null)
                {
                    calendars = this.repository.FindAll(request.CalendarIds, (request.Page.Value - 1) * request.Size.Value, request.Size.Value);
                }
                else calendars = this.repository.FindAll(request.CalendarIds);
                return !calendars.NullOrEmpty() ? calendars.ToList() : new List<VCALENDAR>();
            }
            catch (ArgumentNullException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public List<VCALENDAR> Get(GetCalendars request)
        {
            try
            {
                IEnumerable<VCALENDAR> calendars = null;
                if (request.Page != null && request.Size != null)
                    calendars = this.repository.Get((request.Page.Value - 1) * request.Size.Value, request.Size.Value);
                else
                    calendars = this.repository.Get();

                return !calendars.NullOrEmpty() ? calendars.ToList() : new List<VCALENDAR>();
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }
    }
}