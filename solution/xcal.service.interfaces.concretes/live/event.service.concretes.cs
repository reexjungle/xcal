using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.interfaces.contracts.live;
using reexjungle.xcal.service.operations.concretes.live;
using reexjungle.xcal.service.repositories.contracts;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.foundation.contracts;
using reexjungle.xmisc.infrastructure.concretes.io;
using ServiceStack.Common;
using ServiceStack.Logging;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace reexjungle.xcal.service.interfaces.concretes.live
{
    public class EventService : Service, IEventService
    {
        private readonly ILogFactory logFactory;
        private readonly ICalendarRepository calendarRepository;
        private readonly IEventRepository eventRepository;

        private ILog log;

        private ILog logger
        {
            get { return log ?? (log = logFactory.GetLogger(GetType())); }
        }

        public ILogFactory LogFactory
        {
            get { return logFactory; }
        }

        public EventService(IEventRepository eventRepository, ICalendarRepository calendarRepository, ILogFactory logFactory)
        {
            if (eventRepository == null) throw new ArgumentNullException("eventRepository");
            if (calendarRepository == null) throw new ArgumentNullException("calendarRepository");
            if (logFactory == null) throw new ArgumentNullException("logFactory");

            this.calendarRepository = calendarRepository;
            this.logFactory = logFactory;
            this.eventRepository = eventRepository;
        }

        public void Post(AddEvent request)
        {
            try
            {
                var cacheKey = UrnId.Create<VEVENT>(request.Event.Id);
                if (calendarRepository.ContainsKey(request.CalendarId))
                {
                    if (!eventRepository.ContainsKey(request.Event.Id))
                    {
                        var calendar = calendarRepository.Find(request.CalendarId);
                        calendar.Events.MergeRange(request.Event.ToSingleton());
                        calendarRepository.Save(calendar);
                    }
                }

                RequestContext.RemoveFromCache(Cache, cacheKey);
            }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }

        public void Post(AddEvents request)
        {
            try
            {
                if (calendarRepository.ContainsKey(request.CalendarId))
                {
                    var keys = request.Events.Select(x => x.Id).ToArray();
                    if (!eventRepository.ContainsKeys(keys))
                    {
                        var calendar = calendarRepository.Find(request.CalendarId);
                        calendar.Events.MergeRange(request.Events);
                        calendarRepository.Save(calendar);
                    }
                }

                RequestContext.RemoveFromCache(Cache, "urn:events");
            }
            catch (ArgumentNullException ex) { logger.Error(ex.ToString()); throw; }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }

        public void Put(UpdateEvent request)
        {
            try
            {
                var cacheKey = UrnId.Create<VEVENT>(request.Event.Id);
                if (eventRepository.ContainsKey(request.Event.Id))
                    eventRepository.Save(request.Event);

                RequestContext.RemoveFromCache(Cache, cacheKey);
            }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }

        public void Put(UpdateEvents request)
        {
            try
            {
                var keys = request.Events.Select(x => x.Id).ToArray();
                if (eventRepository.ContainsKeys(keys))
                {
                    eventRepository.SaveAll(request.Events);
                }

                RequestContext.RemoveFromCache(Cache, "urn:events");
            }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
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
                var fieldexpr = fieldstr.CompileToExpressionFunc<VEVENT, object>(CodeDomLanguage.csharp, new string[] { "System.dll", "System.Core.dll", typeof(VEVENT).Assembly.Location, typeof(IContainsKey<Guid>).Assembly.Location });

                eventRepository.Patch(source, fieldexpr, request.EventId.ToSingleton());
                RequestContext.RemoveFromCache(Cache, cacheKey);
            }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
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
                var fieldexpr = fieldstr.CompileToExpressionFunc<VEVENT, object>(CodeDomLanguage.csharp, new string[] { "System.dll", "System.Core.dll", typeof(VEVENT).Assembly.Location, typeof(IContainsKey<Guid>).Assembly.Location });
                eventRepository.Patch(source, fieldexpr, request.EventIds);

                RequestContext.RemoveFromCache(Cache, "urn:events");
            }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }

        public void Delete(DeleteEvent request)
        {
            try
            {
                var cacheKey = UrnId.Create<VEVENT>(request.EventId);
                eventRepository.Erase(request.EventId);
                RequestContext.RemoveFromCache(Cache, cacheKey);
            }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }

        public void Delete(DeleteEvents request)
        {
            try
            {
                eventRepository.EraseAll(request.EventIds);
                RequestContext.RemoveFromCache(Cache, "urn:events");
            }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }

        public VEVENT Get(FindEvent request)
        {
            try
            {
                return eventRepository.Find(request.EventId);
            }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }

        public List<VEVENT> Post(FindEvents request)
        {
            try
            {
                IEnumerable<VEVENT> events = null;
                if (request.Page != null && request.Size != null)
                {
                    events = eventRepository.FindAll(request.EventIds, (request.Page.Value - 1) * request.Size.Value, request.Size.Value);
                }
                else events = eventRepository.FindAll(request.EventIds);

                return !events.NullOrEmpty() ? events.ToList() : new List<VEVENT>();
            }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }

        public List<VEVENT> Get(GetEvents request)
        {
            try
            {
                IEnumerable<VEVENT> events = null;
                if (request.Page != null && request.Size != null)
                    events = eventRepository.Get((request.Page.Value - 1) * request.Size.Value, request.Size.Value);
                else
                    events = eventRepository.Get();
                return !events.NullOrEmpty() ? events.ToList() : new List<VEVENT>();
            }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }

        public List<Guid> Get(GetEventKeys request)
        {
            try
            {
                IEnumerable<Guid> keys;
                if (request.Page != null && request.Size != null)
                    keys = eventRepository.GetKeys((request.Page.Value - 1) * request.Size.Value, request.Size.Value);
                else
                    keys = eventRepository.GetKeys();
                return !keys.NullOrEmpty() ? keys.ToList() : new List<Guid>();
            }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }
    }
}