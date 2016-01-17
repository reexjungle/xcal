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
using reexjungle.xmisc.infrastructure.contracts;

namespace reexjungle.xcal.service.interfaces.concretes.live
{
    public class EventWebService : Service, IEventWebService
    {
        private readonly ILogFactory factory;
        private readonly ICalendarRepository calendarRepository;
        private readonly IEventRepository eventRepository;
        private readonly ICacheKeyBuilder<Guid> keyBuilder; 

        private ILog log;
        private ILog logger
        {
            get { return log ?? (log = factory.GetLogger(GetType())); }
        }

        public EventWebService(IEventRepository eventRepository, ICalendarRepository calendarRepository, ICacheKeyBuilder<Guid> keyBuilder, ILogFactory factory)
        {
            if (eventRepository == null) throw new ArgumentNullException("eventRepository");
            if (calendarRepository == null) throw new ArgumentNullException("calendarRepository");
            if (keyBuilder == null) throw new ArgumentNullException("keyBuilder");
            if (factory == null) throw new ArgumentNullException("factory");

            this.calendarRepository = calendarRepository;
            this.factory = factory;
            this.keyBuilder = keyBuilder;
            this.eventRepository = eventRepository;
        }

        public void Post(AddEvent request)
        {
            try
            {
                if (calendarRepository.ContainsKey(request.CalendarId))
                {
                    if (!eventRepository.ContainsKey(request.Event.Id))
                    {
                        var calendar = calendarRepository.Find(request.CalendarId);
                        calendar.Events.MergeRange(request.Event.ToSingleton());
                        calendarRepository.Save(calendar);

                        var cacheKey = keyBuilder.Build(request, x => x.Event.Id).ToString();
                        RequestContext.RemoveFromCache(Cache, cacheKey);

                    }
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

                        var cacheKey = keyBuilder.Build(request, x => keys).ToString();
                        RequestContext.RemoveFromCache(Cache, cacheKey);
                    }

                }

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
                logger.Error(ex.ToString()); 
                throw;
            }
        }

        public void Put(UpdateEvent request)
        {
            try
            {
                if (eventRepository.ContainsKey(request.Event.Id))
                {
                    eventRepository.Save(request.Event);

                    var cacheKey = keyBuilder.Build(request, x => x.Event.Id).ToString();
                    RequestContext.RemoveFromCache(Cache, cacheKey);

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

        public void Put(UpdateEvents request)
        {
            try
            {
                var keys = request.Events.Select(x => x.Id).ToArray();
                if (eventRepository.ContainsKeys(keys))
                {
                    eventRepository.SaveAll(request.Events);
                
                    var cacheKey = keyBuilder.Build(request, x => keys).ToString();
                    RequestContext.RemoveFromCache(Cache, cacheKey);

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

        public void Post(PatchEvent request)
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

                var fields = new List<string>();
                if (source.Start != default(DATE_TIME)) fields.Add("Start");
                if (source.Classification != default(CLASS)) fields.Add("Classification");
                if (source.Description != null) fields.Add("Description");
                if (source.Position != default(GEO)) fields.Add("Position");
                if (source.Location != null) fields.Add("Location");
                if (source.Organizer != null) fields.Add("Organizer");
                if (source.Priority != default(PRIORITY)) fields.Add("Priority");
                if (source.Sequence != default(int)) fields.Add("Sequence");
                if (source.Status != default(STATUS)) fields.Add("Status");
                if (source.Summary != null) fields.Add("Summary");
                if (source.Transparency != default(TRANSP)) fields.Add("Transparency");
                if (source.Url != default(URL)) fields.Add("Url");
                if (source.RecurrenceRule != null) fields.Add("RecurrenceRule");
                if (source.End != default(DATE_TIME)) fields.Add("End");
                if (source.Duration != default(DURATION)) fields.Add("Duration");
                if (source.Categories != null) fields.Add("Categories");
                if (source.AttachmentBinaries != null) fields.Add("AttachmentBinaries");
                if (source.AttachmentUris != null) fields.Add("AttachmentUris");
                if (source.Attendees != null) fields.Add("Attendees");
                if (source.Comments != null) fields.Add("Comments");
                if (source.Contacts != null) fields.Add("Contacts");
                if (source.ExceptionDates != null) fields.Add("ExceptionDates");
                if (source.RequestStatuses != null) fields.Add("RequestStatuses");
                if (source.Resources != null) fields.Add("Resources");
                if (source.RelatedTos != null) fields.Add("RelatedTos");
                if (source.AudioAlarms != null) fields.Add("AudioAlarms");
                if (source.DisplayAlarms != null) fields.Add("DisplayAlarms");
                if (source.EmailAlarms != null) fields.Add("EmailAlarms");
                if (source.IANAProperties != null) fields.Add("IANAProperties");
                if (source.XProperties != null) fields.Add("XProperties");

                eventRepository.Patch(source, fields, request.EventId.ToSingleton());

                var cacheKey = keyBuilder.Build(request, x => x.EventId).ToString();
                RequestContext.RemoveFromCache(Cache, cacheKey);
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

        public void Post(PatchEvents request)
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

                var fields = new List<string>();
                if (source.Start != default(DATE_TIME)) fields.Add("Start");
                if (source.Classification != default(CLASS)) fields.Add("Classification");
                if (source.Description != null) fields.Add("Description");
                if (source.Position != default(GEO)) fields.Add("Position");
                if (source.Location != null) fields.Add("Location");
                if (source.Organizer != null) fields.Add("Organizer");
                if (source.Priority != default(PRIORITY)) fields.Add("Priority");
                if (source.Sequence != default(int)) fields.Add("Sequence");
                if (source.Status != default(STATUS)) fields.Add("Status");
                if (source.Summary != null) fields.Add("Summary");
                if (source.Transparency != default(TRANSP)) fields.Add("Transparency");
                if (source.Url != default(URL)) fields.Add("Url");
                if (source.RecurrenceRule != null) fields.Add("RecurrenceRule");
                if (source.End != default(DATE_TIME)) fields.Add("End");
                if (source.Duration != default(DURATION)) fields.Add("Duration");
                if (source.Categories != null) fields.Add("Categories");
                if (source.AttachmentBinaries != null) fields.Add("AttachmentBinaries");
                if (source.AttachmentUris != null) fields.Add("AttachmentUris");
                if (source.Attendees != null) fields.Add("Attendees");
                if (source.Comments != null) fields.Add("Comments");
                if (source.Contacts != null) fields.Add("Contacts");
                if (source.ExceptionDates != null) fields.Add("ExceptionDates");
                if (source.RequestStatuses != null) fields.Add("RequestStatuses");
                if (source.Resources != null) fields.Add("Resources");
                if (source.RelatedTos != null) fields.Add("RelatedTos");
                if (source.AudioAlarms != null) fields.Add("AudioAlarms");
                if (source.DisplayAlarms != null) fields.Add("DisplayAlarms");
                if (source.EmailAlarms != null) fields.Add("EmailAlarms");
                if (source.IANAProperties != null) fields.Add("IANAProperties");
                if (source.XProperties != null) fields.Add("XProperties");
                
                eventRepository.Patch(source, fields, request.EventIds);

                var cacheKey = keyBuilder.Build(request, x => x.EventIds).ToString();
                RequestContext.RemoveFromCache(Cache, cacheKey);
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

        public void Delete(DeleteEvent request)
        {
            try
            {
                eventRepository.Erase(request.EventId);
                RequestContext.RemoveFromCache(Cache, keyBuilder.Build(request, x => x.EventId).ToString());
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

        public void Post(DeleteEvents request)
        {
            try
            {
                eventRepository.EraseAll(request.EventIds);
                RequestContext.RemoveFromCache(Cache, keyBuilder.Build(request, x => x.EventIds).ToString());
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

        public VEVENT Get(FindEvent request)
        {
            try
            {
                return eventRepository.Find(request.EventId);
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

        public List<VEVENT> Post(FindEvents request)
        {
            try
            {
                return request.Page != null && request.Size != null
                    ? eventRepository.FindAll(request.EventIds, (request.Page.Value - 1)*request.Size.Value,
                        request.Size.Value).ToList()
                    : eventRepository.FindAll(request.EventIds).ToList();
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

        public List<VEVENT> Get(GetEvents request)
        {
            try
            {
                return request.Page != null && request.Size != null
                    ? eventRepository.Get((request.Page.Value - 1)*request.Size.Value, request.Size.Value).ToList()
                    : eventRepository.Get().ToList();

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

        public List<Guid> Get(GetEventKeys request)
        {
            try
            {
                return request.Page != null && request.Size != null
                    ? eventRepository.GetKeys((request.Page.Value - 1)*request.Size.Value, request.Size.Value).ToList()
                    : eventRepository.GetKeys().ToList();
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