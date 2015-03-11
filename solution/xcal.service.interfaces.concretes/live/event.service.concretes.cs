using reexjungle.foundation.essentials.concretes;
using reexjungle.foundation.essentials.contracts;
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
    //[Authenticate]
    public class EventService : Service, IEventService
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

        public EventService()
            : base()
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

        public void Post(AddEvent request)
        {
            try
            {
                var cacheKey = UrnId.Create<VEVENT>(request.Event.Id);
                if (this.repository.ContainsKey(request.CalendarId))
                {
                    if (!this.repository.EventRepository.ContainsKey(request.Event.Id))
                    {
                        var calendar = this.repository.Find(request.CalendarId);
                        calendar.Events.MergeRange(request.Event.ToSingleton());
                        this.repository.Save(calendar);
                    }
                }

                base.RequestContext.RemoveFromCache(base.Cache, cacheKey);
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public void Post(AddEvents request)
        {
            try
            {
                if (this.repository.ContainsKey(request.CalendarId))
                {
                    var keys = request.Events.Select(x => x.Id).ToArray();
                    if (!this.repository.EventRepository.ContainsKeys(keys))
                    {
                        var calendar = this.repository.Find(request.CalendarId);
                        calendar.Events.MergeRange(request.Events);
                        this.repository.Save(calendar);
                    }
                }

                base.RequestContext.RemoveFromCache(base.Cache, "urn:events");
            }
            catch (ArgumentNullException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public void Put(UpdateEvent request)
        {
            try
            {
                var cacheKey = UrnId.Create<VEVENT>(request.Event.Id);
                if (this.repository.EventRepository.ContainsKey(request.Event.Id))
                    this.repository.EventRepository.Save(request.Event);

                base.RequestContext.RemoveFromCache(base.Cache, cacheKey);
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public void Put(UpdateEvents request)
        {
            try
            {
                var keys = request.Events.Select(x => x.Id).ToArray();
                if (this.repository.EventRepository.ContainsKeys(keys))
                {
                    this.repository.EventRepository.SaveAll(request.Events);
                }

                base.RequestContext.RemoveFromCache(base.Cache, "urn:events");
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public void Patch(PatchEvent request)
        {
            try
            {
                var cacheKey = UrnId.Create<VEVENT>(request.EventId);

                var source = new VEVENT
                {
                    Start = request.Start,
                    Classification = request.Classification,
                    Description = request.Description,
                    Position = request.Position,
                    Location = request.Location,
                    Organizer = request.Organizer,
                    Priority = request.Priority,
                    Sequence = request.Sequence,
                    Status = request.Status,
                    Summary = request.Summary,
                    Transparency = request.Transparency,
                    Url = request.Url,
                    RecurrenceRule = request.RecurrenceRule,
                    End = request.End,
                    Duration = request.Duration,
                    AttachmentBinaries = request.AttachmentBinaries,
                    AttachmentUris = request.AttachmentUris,
                    Attendees = request.Attendees,
                    Categories = request.Categories,
                    Comments = request.Comments,
                    Contacts = request.Contacts,
                    ExceptionDates = request.ExceptionDates,
                    RequestStatuses = request.RequestStatuses,
                    Resources = request.Resources,
                    RelatedTos = request.RelatedTos,
                    AudioAlarms = request.AudioAlarms,
                    DisplayAlarms = request.DisplayAlarms,
                    EmailAlarms = request.EmailAlarms,
                    IANAProperties = request.IANAProperties,
                    XProperties = request.XProperties
                };

                var fieldlist = new List<string>();
                if (source.Start != default(DATE_TIME)) fieldlist.Add("Start");
                if (source.Classification != default(CLASS)) fieldlist.Add("Classification");
                if (source.Description != null) fieldlist.Add("Description");
                if (source.Position != default(GEO)) fieldlist.Add("Position");
                if (source.Location != null) fieldlist.Add("Location");
                if (source.Organizer != null) fieldlist.Add("Organizer");
                if (source.Priority != default(PRIORITY)) fieldlist.Add("Priority");
                if (source.Sequence != default(int)) fieldlist.Add("Sequence");
                if (source.Status != default(STATUS)) fieldlist.Add("Status");
                if (source.Summary != null) fieldlist.Add("Summary");
                if (source.Transparency != default(TRANSP)) fieldlist.Add("Transparency");
                if (source.Url != default(URI)) fieldlist.Add("Url");
                if (source.RecurrenceRule != null) fieldlist.Add("RecurrenceRule");
                if (source.End != default(DATE_TIME)) fieldlist.Add("End");
                if (source.Duration != default(DURATION)) fieldlist.Add("Duration");
                if (source.Categories != null) fieldlist.Add("Categories");
                if (source.AttachmentBinaries != null) fieldlist.Add("AttachmentBinaries");
                if (source.AttachmentUris != null) fieldlist.Add("AttachmentUris");
                if (source.Attendees != null) fieldlist.Add("Attendees");
                if (source.Comments != null) fieldlist.Add("Comments");
                if (source.Contacts != null) fieldlist.Add("Contacts");
                if (source.ExceptionDates != null) fieldlist.Add("ExceptionDates");
                if (source.RequestStatuses != null) fieldlist.Add("RequestStatuses");
                if (source.Resources != null) fieldlist.Add("Resources");
                if (source.RelatedTos != null) fieldlist.Add("RelatedTos");
                if (source.AudioAlarms != null) fieldlist.Add("AudioAlarms");
                if (source.DisplayAlarms != null) fieldlist.Add("DisplayAlarms");
                if (source.EmailAlarms != null) fieldlist.Add("EmailAlarms");
                if (source.IANAProperties != null) fieldlist.Add("IANAProperties");
                if (source.XProperties != null) fieldlist.Add("XProperties");

                var fieldstr = string.Format("x => new {{ {0} }}", string.Join(", ", fieldlist.Select(x => string.Format("x.{0}", x))));
                var fieldexpr = fieldstr.CompileToExpressionFunc<VEVENT, object>(CodeDomLanguage.csharp, new string[] { "System.dll", "System.Core.dll", typeof(VEVENT).Assembly.Location, typeof(IContainsKey<string>).Assembly.Location });

                this.repository.EventRepository.Patch(source, fieldexpr, request.EventId.ToSingleton());
                base.RequestContext.RemoveFromCache(base.Cache, cacheKey);
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public void Patch(PatchEvents request)
        {
            try
            {
                var source = new VEVENT
                {
                    Start = request.Start,
                    Classification = request.Classification,
                    Description = request.Description,
                    Position = request.Position,
                    Location = request.Location,
                    Organizer = request.Organizer,
                    Priority = request.Priority,
                    Sequence = request.Sequence,
                    Status = request.Status,
                    Summary = request.Summary,
                    Transparency = request.Transparency,
                    Url = request.Url,
                    RecurrenceRule = request.RecurrenceRule,
                    End = request.End,
                    Duration = request.Duration,
                    AttachmentBinaries = request.AttachmentBinaries,
                    AttachmentUris = request.AttachmentUris,
                    Attendees = request.Attendees,
                    Categories = request.Categories,
                    Comments = request.Comments,
                    Contacts = request.Contacts,
                    ExceptionDates = request.ExceptionDates,
                    RequestStatuses = request.RequestStatuses,
                    Resources = request.Resources,
                    RelatedTos = request.RelatedTos,
                    AudioAlarms = request.AudioAlarms,
                    DisplayAlarms = request.DisplayAlarms,
                    EmailAlarms = request.EmailAlarms,
                    IANAProperties = request.IANAProperties,
                    XProperties = request.XProperties
                };

                var fieldlist = new List<string>();
                if (source.Start != default(DATE_TIME)) fieldlist.Add("Start");
                if (source.Classification != default(CLASS)) fieldlist.Add("Classification");
                if (source.Description != null) fieldlist.Add("Description");
                if (source.Position != default(GEO)) fieldlist.Add("Position");
                if (source.Location != null) fieldlist.Add("Location");
                if (source.Organizer != null) fieldlist.Add("Organizer");
                if (source.Priority != default(PRIORITY)) fieldlist.Add("Priority");
                if (source.Sequence != default(int)) fieldlist.Add("Sequence");
                if (source.Status != default(STATUS)) fieldlist.Add("Status");
                if (source.Summary != null) fieldlist.Add("Summary");
                if (source.Transparency != default(TRANSP)) fieldlist.Add("Transparency");
                if (source.Url != default(URI)) fieldlist.Add("Url");
                if (source.RecurrenceRule != null) fieldlist.Add("RecurrenceRule");
                if (source.End != default(DATE_TIME)) fieldlist.Add("End");
                if (source.Duration != default(DURATION)) fieldlist.Add("Duration");
                if (source.Categories != null) fieldlist.Add("Categories");
                if (source.AttachmentBinaries != null) fieldlist.Add("AttachmentBinaries");
                if (source.AttachmentUris != null) fieldlist.Add("AttachmentUris");
                if (source.Attendees != null) fieldlist.Add("Attendees");
                if (source.Comments != null) fieldlist.Add("Comments");
                if (source.Contacts != null) fieldlist.Add("Contacts");
                if (source.ExceptionDates != null) fieldlist.Add("ExceptionDates");
                if (source.RequestStatuses != null) fieldlist.Add("RequestStatuses");
                if (source.Resources != null) fieldlist.Add("Resources");
                if (source.RelatedTos != null) fieldlist.Add("RelatedTos");
                if (source.AudioAlarms != null) fieldlist.Add("AudioAlarms");
                if (source.DisplayAlarms != null) fieldlist.Add("DisplayAlarms");
                if (source.EmailAlarms != null) fieldlist.Add("EmailAlarms");
                if (source.IANAProperties != null) fieldlist.Add("IANAProperties");
                if (source.XProperties != null) fieldlist.Add("XProperties");

                var fieldstr = string.Format("x => new {{ {0} }}", string.Join(", ", fieldlist.Select(x => string.Format("x.{0}", x))));
                var fieldexpr = fieldstr.CompileToExpressionFunc<VEVENT, object>(CodeDomLanguage.csharp, new string[] { "System.dll", "System.Core.dll", typeof(VEVENT).Assembly.Location, typeof(IContainsKey<string>).Assembly.Location });
                this.repository.EventRepository.Patch(source, fieldexpr, request.EventIds);

                base.RequestContext.RemoveFromCache(base.Cache, "urn:events");
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public void Delete(DeleteEvent request)
        {
            try
            {
                var cacheKey = UrnId.Create<VEVENT>(request.EventId);
                this.repository.EventRepository.Erase(request.EventId);
                base.RequestContext.RemoveFromCache(base.Cache, cacheKey);
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public void Delete(DeleteEvents request)
        {
            try
            {
                this.repository.EventRepository.EraseAll(request.EventIds);
                base.RequestContext.RemoveFromCache(base.Cache, "urn:events");
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public VEVENT Get(FindEvent request)
        {
            try
            {
                return this.repository.EventRepository.Find(request.EventId);
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public List<VEVENT> Post(FindEvents request)
        {
            try
            {
                IEnumerable<VEVENT> events = null;
                if (request.Page != null && request.Size != null)
                {
                    events = this.repository.EventRepository.FindAll(request.EventIds, (request.Page.Value - 1) * request.Size.Value, request.Size.Value);
                }
                else events = this.repository.EventRepository.FindAll(request.EventIds);

                return !events.NullOrEmpty() ? events.ToList() : new List<VEVENT>();
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public List<VEVENT> Get(GetEvents request)
        {
            try
            {
                IEnumerable<VEVENT> events = null;
                if (request.Page != null && request.Size != null)
                    events = this.repository.EventRepository.Get((request.Page.Value - 1) * request.Size.Value, request.Size.Value);
                else
                    events = this.repository.EventRepository.Get();
                return !events.NullOrEmpty() ? events.ToList() : new List<VEVENT>();
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public List<string> Get(GetEventKeys request)
        {
            try
            {
                IEnumerable<string> keys = null;
                if (request.Page != null && request.Size != null)
                    keys = this.repository.EventRepository.GetKeys((request.Page.Value - 1) * request.Size.Value, request.Size.Value);
                else
                    keys = this.repository.EventRepository.GetKeys();
                return !keys.NullOrEmpty() ? keys.ToList() : new List<string>();
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }
    }
}