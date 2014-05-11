using System;
using System.Linq;
using System.Collections.Generic;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.infrastructure.io.concretes;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.domain.operations;
using reexmonkey.xcal.service.repositories.contracts;
using reexmonkey.xcal.service.interfaces.contracts.live;
using reexmonkey.foundation.essentials.contracts;

namespace reexmonkey.xcal.service.interfaces.concretes.live
{
    public class EventService: Service, IEventService
    {
        private ILogFactory logfactory;
        private ICalendarRepository repository;

        private ILog log = null;
        private ILog logger
        {
            get { return (log != null)? this.log: this.logfactory.GetLogger(this.GetType()); }
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

        public EventService() : base() 
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
                if (this.repository.ContainsKey(request.CalendarId))
                {
                    if (!this.repository.EventRepository.ContainsKey(request.Event.Id)) 
                        this.repository.EventRepository.Save(request.Event);
                }
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
                        this.repository.EventRepository.SaveAll(request.Events);
                }
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public void Put(UpdateEvent request)
        {
            try
            {
                if (this.repository.EventRepository.ContainsKey(request.Event.Id))
                    this.repository.EventRepository.Save(request.Event);

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
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public void Patch(PatchEvent request)
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
                    Contacts= request.Contacts,
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
                if (source.Position != null) fieldlist.Add("Position");
                if (source.Location != null) fieldlist.Add("Location");
                if (source.Organizer != null) fieldlist.Add("Organizer");
                if (source.Priority != default(PRIORITY)) fieldlist.Add("Priority");
                if (source.Sequence != default(int)) fieldlist.Add("Sequence");
                if (source.Status != default(STATUS)) fieldlist.Add("Status");
                if (source.Summary != null) fieldlist.Add("Summary");
                if (source.Transparency != default(TRANSP)) fieldlist.Add("Transparency");
                if (source.Url != null) fieldlist.Add("Url");
                if (source.RecurrenceRule != null) fieldlist.Add("RecurrenceRule");
                if (source.End != default(DATE_TIME)) fieldlist.Add("End");
                if (source.Duration != default(DURATION)) fieldlist.Add("Duration");
                if (!source.AttachmentBinaries.NullOrEmpty()) fieldlist.Add("AttachmentBinaries");
                if (!source.AttachmentUris.NullOrEmpty()) fieldlist.Add("AttachmentUris");
                if (!source.Attendees.NullOrEmpty()) fieldlist.Add("Attendees");
                if (source.Categories != null) fieldlist.Add("Categories");
                if (!source.Comments.NullOrEmpty()) fieldlist.Add("Comments");
                if (!source.Contacts.NullOrEmpty()) fieldlist.Add("Contacts");
                if (!source.ExceptionDates.NullOrEmpty()) fieldlist.Add("ExceptionDates");
                if (!source.RequestStatuses.NullOrEmpty()) fieldlist.Add("RequestStatuses");
                if (!source.Resources.NullOrEmpty()) fieldlist.Add("Resources");
                if (!source.RelatedTos.NullOrEmpty()) fieldlist.Add("RelatedTos");
                if (!source.AudioAlarms.NullOrEmpty()) fieldlist.Add("AudioAlarms");
                if (!source.DisplayAlarms.NullOrEmpty()) fieldlist.Add("DisplayAlarms");
                if (!source.EmailAlarms.NullOrEmpty()) fieldlist.Add("EmailAlarms");
                if (!source.IANAProperties.NullOrEmpty()) fieldlist.Add("IANAProperties");
                if (!source.XProperties.NullOrEmpty()) fieldlist.Add("XProperties");

                var fieldstr = string.Format("x => new {{ {0} }}", string.Join(", ", fieldlist.Select(x => string.Format("x.{0}", x))));
                var fieldexpr = fieldstr.CompileToExpressionFunc<VEVENT, object>(CodeDomLanguage.csharp, new string[] { "System.dll", "System.Core.dll", typeof(VEVENT).Assembly.Location, typeof(IContainsKey<string>).Assembly.Location });
                this.repository.EventRepository.Patch(source, fieldexpr, request.EventId.ToSingleton());
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
                if (source.Position != null) fieldlist.Add("Position");
                if (source.Location != null) fieldlist.Add("Location");
                if (source.Organizer != null) fieldlist.Add("Organizer");
                if (source.Priority != default(PRIORITY)) fieldlist.Add("Priority");
                if (source.Sequence != default(int)) fieldlist.Add("Sequence");
                if (source.Status != default(STATUS)) fieldlist.Add("Status");
                if (source.Summary != null) fieldlist.Add("Summary");
                if (source.Transparency != default(TRANSP)) fieldlist.Add("Transparency");
                if (source.Url != null) fieldlist.Add("Url");
                if (source.RecurrenceRule != null) fieldlist.Add("RecurrenceRule");
                if (source.End != default(DATE_TIME)) fieldlist.Add("End");
                if (source.Duration != default(DURATION)) fieldlist.Add("Duration");
                if (!source.AttachmentBinaries.NullOrEmpty()) fieldlist.Add("AttachmentBinaries");
                if (!source.AttachmentUris.NullOrEmpty()) fieldlist.Add("AttachmentUris");
                if (!source.Attendees.NullOrEmpty()) fieldlist.Add("Attendees");
                if (source.Categories != null) fieldlist.Add("Categories");
                if (!source.Comments.NullOrEmpty()) fieldlist.Add("Comments");
                if (!source.Contacts.NullOrEmpty()) fieldlist.Add("Contacts");
                if (!source.ExceptionDates.NullOrEmpty()) fieldlist.Add("ExceptionDates");
                if (!source.RequestStatuses.NullOrEmpty()) fieldlist.Add("RequestStatuses");
                if (!source.Resources.NullOrEmpty()) fieldlist.Add("Resources");
                if (!source.RelatedTos.NullOrEmpty()) fieldlist.Add("RelatedTos");
                if (!source.AudioAlarms.NullOrEmpty()) fieldlist.Add("AudioAlarms");
                if (!source.DisplayAlarms.NullOrEmpty()) fieldlist.Add("DisplayAlarms");
                if (!source.EmailAlarms.NullOrEmpty()) fieldlist.Add("EmailAlarms");
                if (!source.IANAProperties.NullOrEmpty()) fieldlist.Add("IANAProperties");
                if (!source.XProperties.NullOrEmpty()) fieldlist.Add("XProperties");

                var fieldstr = string.Format("x => new {{ {0} }}", string.Join(", ", fieldlist.Select(x => string.Format("x.{0}", x))));
                var fieldexpr = fieldstr.CompileToExpressionFunc<VEVENT, object>(CodeDomLanguage.csharp, new string[] { "System.dll", "System.Core.dll", typeof(VEVENT).Assembly.Location, typeof(IContainsKey<string>).Assembly.Location });
                this.repository.EventRepository.Patch(source, fieldexpr, request.EventIds);
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public void Delete(DeleteEvent request)
        {
            try
            {
                this.repository.EventRepository.Erase(request.EventId);
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
    }
}
