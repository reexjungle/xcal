using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using reexmonkey.xcal.domain.models;
using reexmonkey.crosscut.essentials.contracts;
using reexmonkey.crosscut.essentials.concretes;
using reexmonkey.xcal.service.repositories.contracts;

namespace reexmonkey.xcal.service.repositories.concretes
{
    public class EventRedisRepository: IEventRedisRepository
    {
        private IRedisClientsManager manager = null;
        private int? take= null;
        private IRedisClient client = null;
        private IRedisClient redis
        {
            get
            {
                return (client != null) ? client : this.manager.GetClient();
            }
        }
        private IKeyGenerator<string> keygen;

        private IAudioAlarmRepository aalarmrepository = null;
        private IDisplayAlarmRepository dalarmrepository = null;
        private IEmailAlarmRepository ealarmrepository = null;

        public IRedisClientsManager RedisClientsManager
        {
            get { return this.manager; }
            set
            {
                if (value == null) throw new ArgumentNullException("RedisClientsManager");
                this.manager = value;
            }
        }

        public int? Take
        {
            get { return this.take; }
            set 
            {
                if (take == null) throw new ArgumentNullException("Take");
                this.take = value;
            }
        }

        public IAudioAlarmRepository AudioAlarmRepository
        {
            get { return this.aalarmrepository; }
            set 
            {
                if (value == null) throw new ArgumentNullException("AudioAlarmRepository");
                this.aalarmrepository = value;
            }
        }

        public IDisplayAlarmRepository DisplayAlarmRepository
        {
            get { return this.dalarmrepository; }
            set
            {
                if (value == null) throw new ArgumentNullException("DisplayAlarmRepository");
                this.dalarmrepository = value;
            }
        }

        public IEmailAlarmRepository EmailAlarmRepository
        {
            get { return this.ealarmrepository; }
            set
            {
                if (value == null) throw new ArgumentNullException("EmailAlarmRepository");
                this.EmailAlarmRepository = value;
            }
        }

        public EventRedisRepository(){}

        public EventRedisRepository(IRedisClientsManager manager, int? take = null)
        {
            this.RedisClientsManager = manager;
            this.Take = take;
            this.client = manager.GetClient();
        }

        public EventRedisRepository(IRedisClientsManager manager, 
            IAudioAlarmRepository aalarmrepository, 
            IDisplayAlarmRepository dalarmrepository,
            IEmailAlarmRepository ealarmrepository,
            int? take = null)
        {
            this.RedisClientsManager = manager;
            this.client = manager.GetClient();
            this.Take = take;
            this.AudioAlarmRepository = aalarmrepository;
            this.DisplayAlarmRepository = dalarmrepository;
            this.EmailAlarmRepository = ealarmrepository;
        }

        public VEVENT Hydrate(VEVENT dry)
        {
            VEVENT full = null;
            var eclient = this.redis.As<VEVENT>();
            if (eclient.ContainsKey(dry.Id))
            {
                full = eclient.GetValue(dry.Id);

                var rorgs = this.redis.As<REL_EVENTS_ORGANIZERS>().GetAll().Where(x => x.EventId == full.Id);
                if (!rorgs.NullOrEmpty()) full.Organizer = this.redis.As<ORGANIZER>().GetValue(rorgs.FirstOrDefault().OrganizerId);

                var rrids = this.redis.As<REL_EVENTS_RECURRENCE_IDS>().GetAll().Where(x => x.EventId == full.Id);
                if (!rrids.NullOrEmpty()) full.RecurrenceId = this.redis.As<RECURRENCE_ID>().GetValue(rrids.FirstOrDefault().RecurrenceId_Id);

                var rrules = this.redis.As<REL_EVENTS_RRULES>().GetAll().Where(x => x.EventId == full.Id);
                if (!rrules.NullOrEmpty()) full.RecurrenceRule = this.redis.As<RECUR>().GetValue(rrules.FirstOrDefault().RecurrenceRuleId);

                var rattachbins = this.redis.As<REL_EVENTS_ATTACHBINS>().GetAll().Where(x => x.EventId == full.Id);
                if (!rattachbins.NullOrEmpty())
                {
                    full.Attachments.AddRangeComplement(this.redis.As<ATTACH_BINARY>().GetValues(rattachbins.Select(x => x.AttachmentId).ToList()));
                }

                var rattachuris = this.redis.As<REL_EVENTS_ATTACHURIS>().GetAll().Where(x => x.EventId == full.Id);
                if (!rattachuris.NullOrEmpty())
                {
                    full.Attachments.AddRangeComplement(this.redis.As<ATTACH_URI>().GetValues(rattachuris.Select(x => x.AttachmentId).ToList()));
                }

                var rattendees = this.redis.As<REL_EVENTS_ATTENDEES>().GetAll().Where(x => x.EventId == full.Id);
                if (!rattendees.NullOrEmpty())
                {
                    full.Attendees.AddRangeComplement(this.redis.As<ATTENDEE>().GetValues(rattendees.Select(x => x.AttendeeId).ToList()));
                }

                var rcomments = this.redis.As<REL_EVENTS_COMMENTS>().GetAll().Where(x => x.EventId == full.Id);
                if (!rcomments.NullOrEmpty())
                {
                    full.Comments.AddRangeComplement(this.redis.As<COMMENT>().GetValues(rcomments.Select(x => x.CommentId).ToList()));
                }

                var rcontacts = this.redis.As<REL_EVENTS_CONTACTS>().GetAll().Where(x => x.EventId == full.Id);
                if (!rcontacts.NullOrEmpty())
                {
                    full.Contacts.AddRangeComplement(this.redis.As<CONTACT>().GetValues(rcontacts.Select(x => x.ContactId).ToList()));
                }

                var rrdates = this.redis.As<REL_EVENTS_RDATES>().GetAll().Where(x => x.EventId == full.Id);
                if (!rrdates.NullOrEmpty())
                {
                    full.RecurrenceDates.AddRangeComplement(this.redis.As<RDATE>().GetValues(rrdates.Select(x => x.RecurrenceDateId).ToList()));
                }

                var rexdates = this.redis.As<REL_EVENTS_EXDATES>().GetAll().Where(x => x.EventId == full.Id);
                if (!rexdates.NullOrEmpty())
                {
                    full.ExceptionDates.AddRangeComplement(this.redis.As<EXDATE>().GetValues(rexdates.Select(x => x.ExceptionDateId).ToList()));
                }

                var rrelatedtos = this.redis.As<REL_EVENTS_RELATEDTOS>().GetAll().Where(x => x.EventId == full.Id);
                if (!rrelatedtos.NullOrEmpty())
                {
                    full.RelatedTos.AddRangeComplement(this.redis.As<RELATEDTO>().GetValues(rrelatedtos.Select(x => x.RelatedToId).ToList()));
                }

                var rreqstats = this.redis.As<REL_EVENTS_REQSTATS>().GetAll().Where(x => x.EventId == full.Id);
                if (!rreqstats.NullOrEmpty())
                {
                    full.RequestStatuses.AddRangeComplement(this.redis.As<REQUEST_STATUS>().GetValues(rreqstats.Select(x => x.ReqStatsId).ToList()));
                }

                var rresources = this.redis.As<REL_EVENTS_RESOURCES>().GetAll().Where(x => x.EventId == full.Id);
                if (!rresources.NullOrEmpty())
                {
                    full.Resources.AddRangeComplement(this.redis.As<RESOURCES>().GetValues(rresources.Select(x => x.ResourcesId).ToList()));
                }

                var raalarms = this.redis.As<REL_EVENTS_AUDIO_ALARMS>().GetAll().Where(x => x.EventId == full.Id);
                if(!raalarms.NullOrEmpty())
                {
                    full.Alarms.AddRangeComplement(this.AudioAlarmRepository.Find(raalarms.Select(x => x.AlarmId).ToList()));
                }

                var rdalarms = this.redis.As<REL_EVENTS_DISPLAY_ALARMS>().GetAll().Where(x => x.EventId == full.Id);
                if (!rdalarms.NullOrEmpty())
                {
                    full.Alarms.AddRangeComplement(this.DisplayAlarmRepository.Find(rdalarms.Select(x => x.AlarmId).ToList()));
                }

                var realarms = this.redis.As<REL_EVENTS_EMAIL_ALARMS>().GetAll().Where(x => x.EventId == full.Id);
                if (!realarms.NullOrEmpty())
                {
                    full.Alarms.AddRangeComplement(this.EmailAlarmRepository.Find(realarms.Select(x => x.AlarmId).ToList())); 
                }
            }

            return full ?? dry;
        }

        public IEnumerable<VEVENT> Hydrate(IEnumerable<VEVENT> dry)
        {
            List<VEVENT> full = null;
            var eclient = this.redis.As<VEVENT>();
            var keys = dry.Select(x => x.Id).Distinct().ToList();
            if (eclient.GetAllKeys().Intersect(keys).Count() == keys.Count()) //all keys are found
            {
                full = eclient.GetValues(keys);

                #region 1. retrieve relationships
                
                var rorgs = this.redis.As<REL_EVENTS_ORGANIZERS>().GetAll().Where(x => keys.Contains(x.EventId));
                var rrids = this.redis.As<REL_EVENTS_RECURRENCE_IDS>().GetAll().Where(x => keys.Contains(x.EventId));
                var rrrules = this.redis.As<REL_EVENTS_RRULES>().GetAll().Where(x => keys.Contains(x.EventId));
                var rattachbins = this.redis.As<REL_EVENTS_ATTACHBINS>().GetAll().Where(x => keys.Contains(x.EventId));
                var rattachuris = this.redis.As<REL_EVENTS_ATTACHURIS>().GetAll().Where(x => keys.Contains(x.EventId));
                var rattendees = this.redis.As<REL_EVENTS_ATTENDEES>().GetAll().Where(x => keys.Contains(x.EventId));
                var rcomments = this.redis.As<REL_EVENTS_COMMENTS>().GetAll().Where(x => keys.Contains(x.EventId));
                var rcontacts = this.redis.As<REL_EVENTS_CONTACTS>().GetAll().Where(x => keys.Contains(x.EventId));
                var rrdates = this.redis.As<REL_EVENTS_RDATES>().GetAll().Where(x => keys.Contains(x.EventId));
                var rexdates = this.redis.As<REL_EVENTS_EXDATES>().GetAll().Where(x => keys.Contains(x.EventId));
                var rrelatedtos = this.redis.As<REL_EVENTS_RELATEDTOS>().GetAll().Where(x => keys.Contains(x.EventId));
                var rreqstats = this.redis.As<REL_EVENTS_REQSTATS>().GetAll().Where(x => keys.Contains(x.EventId));
                var rresources = this.redis.As<REL_EVENTS_RESOURCES>().GetAll().Where(x => keys.Contains(x.EventId));
                var raalarms = this.redis.As<REL_EVENTS_AUDIO_ALARMS>().GetAll().Where(x => keys.Contains(x.EventId));
                var rdalarms = this.redis.As<REL_EVENTS_DISPLAY_ALARMS>().GetAll().Where(x => keys.Contains(x.EventId));
                var realarms = this.redis.As<REL_EVENTS_EMAIL_ALARMS>().GetAll().Where(x => keys.Contains(x.EventId)); 
                
                #endregion

                #region 2. retrieve secondary entities
                
                var orgs = (!rorgs.Empty()) ? this.redis.As<ORGANIZER>().GetValues(rorgs.Select(x => x.OrganizerId).ToList()) : null;
                var rids = (!rrids.Empty()) ? this.redis.As<RECURRENCE_ID>().GetValues(rrids.Select(x => x.RecurrenceId_Id).ToList()) : null;
                var rrules = (!rrrules.Empty()) ? this.redis.As<RECUR>().GetValues(rrrules.Select(x => x.RecurrenceRuleId).ToList()) : null;
                var attachbins = (!rattachbins.Empty()) ? this.redis.As<ATTACH_BINARY>().GetValues(rattachbins.Select(x => x.AttachmentId).ToList()) : null;
                var attachuris = (!rattachuris.Empty()) ? this.redis.As<ATTACH_URI>().GetValues(rattachuris.Select(x => x.AttachmentId).ToList()) : null;
                var attendees = (!rattendees.Empty()) ? this.redis.As<ATTENDEE>().GetValues(rattendees.Select(x => x.AttendeeId).ToList()) : null;
                var comments = (!rcomments.Empty()) ? this.redis.As<COMMENT>().GetValues(rcomments.Select(x => x.CommentId).ToList()) : null;
                var contacts = (!rcontacts.Empty()) ? this.redis.As<CONTACT>().GetValues(rcontacts.Select(x => x.ContactId).ToList()) : null;
                var rdates = (!rrdates.Empty()) ? this.redis.As<RDATE>().GetValues(rrdates.Select(x => x.RecurrenceDateId).ToList()) : null;
                var exdates = (!rexdates.Empty()) ? this.redis.As<EXDATE>().GetValues(rexdates.Select(x => x.ExceptionDateId).ToList()) : null;
                var relatedtos = (!rrelatedtos.Empty()) ? this.redis.As<RELATEDTO>().GetValues(rrelatedtos.Select(x => x.RelatedToId).ToList()) : null;
                var reqstats = (!rreqstats.Empty()) ? this.redis.As<REQUEST_STATUS>().GetValues(rreqstats.Select(x => x.ReqStatsId).ToList()) : null;
                var resources = (!rresources.Empty()) ? this.redis.As<RESOURCES>().GetValues(rresources.Select(x => x.ResourcesId).ToList()) : null;
                var aalarms = (!raalarms.Empty()) ? this.AudioAlarmRepository.Find(raalarms.Select(x => x.AlarmId).ToList()) : null;
                var dalarms = (!rdalarms.Empty()) ? this.DisplayAlarmRepository.Find(rdalarms.Select(x => x.AlarmId).ToList()) : null;
                var ealarms = (!realarms.Empty()) ? this.EmailAlarmRepository.Find(realarms.Select(x => x.AlarmId).ToList()) : null;
                
                #endregion                
                
                #region 3. Use Linq to stitch secondary entities to primary entities
               
                full.ForEach(x =>
                {
                    if (!orgs.NullOrEmpty())
                    {
                        var xorgs = from y in orgs
                                    join r in rorgs on y.Id equals r.OrganizerId
                                    join e in full on r.EventId equals e.Id
                                    where e.Id == x.Id
                                    select y;
                        if (!xorgs.NullOrEmpty()) x.Organizer = xorgs.FirstOrDefault();
                    }

                    if (!rids.NullOrEmpty())
                    {
                        var xrids = from y in rids
                                    join r in rrids on y.Id equals r.RecurrenceId_Id
                                    join e in full on r.EventId equals e.Id
                                    where e.Id == x.Id
                                    select y;
                        if (!xrids.NullOrEmpty()) x.RecurrenceId = xrids.FirstOrDefault();
                    }

                    if (!rrules.NullOrEmpty())
                    {
                        var xrules = from y in rrules
                                     join r in rrrules on y.Id equals r.RecurrenceRuleId
                                     join e in full on r.EventId equals e.Id
                                     where e.Id == x.Id
                                     select y;
                        if (!xrules.NullOrEmpty()) x.RecurrenceRule = xrules.FirstOrDefault();
                    }

                    if (!comments.NullOrEmpty())
                    {
                        var xcomments = from y in comments
                                        join r in rcomments on y.Id equals r.CommentId
                                        join e in full on r.EventId equals e.Id
                                        where e.Id == x.Id
                                        select y;
                        if (!xcomments.NullOrEmpty()) x.Comments.AddRangeComplement(xcomments);
                    }

                    if (!attendees.NullOrEmpty())
                    {
                        var xattendees = from y in attendees
                                         join r in rattendees on y.Id equals r.AttendeeId
                                         join e in full on r.EventId equals e.Id
                                         where e.Id == x.Id
                                         select y;
                        if (!xattendees.NullOrEmpty()) x.Attendees.AddRange(xattendees);
                    }

                    if (!attachbins.NullOrEmpty())
                    {
                        var xattachbins = from y in attachbins
                                          join r in rattachbins on y.Id equals r.AttachmentId
                                          join e in full on r.EventId equals e.Id
                                          where e.Id == x.Id
                                          select y;
                        if (!xattachbins.NullOrEmpty()) x.Attachments.AddRangeComplement(xattachbins);

                    }

                    if (!attachuris.NullOrEmpty())
                    {
                        var xattachuris = from y in attachuris
                                          join r in rattachuris on y.Id equals r.AttachmentId
                                          join e in full on r.EventId equals e.Id
                                          where e.Id == x.Id
                                          select y;
                        if (!xattachuris.NullOrEmpty()) x.Attachments.AddRangeComplement(xattachuris);
                    }

                    var xcontacts = from y in contacts
                                    join r in rcontacts on y.Id equals r.ContactId
                                    join e in full on r.EventId equals e.Id
                                    where e.Id == x.Id
                                    select y;

                    if (!xcontacts.NullOrEmpty()) x.Contacts.AddRangeComplement(xcontacts);

                    if (!rdates.NullOrEmpty())
                    {
                        var xdates = from y in rdates
                                     join r in rrdates on y.Id equals r.RecurrenceDateId
                                     join e in full on r.EventId equals e.Id
                                     where e.Id == x.Id
                                     select y;
                        if (!xdates.NullOrEmpty()) x.RecurrenceDates.AddRangeComplement(xdates);
                    }

                    if (!exdates.NullOrEmpty())
                    {
                        var xexdates = from y in exdates
                                       join r in rexdates on y.Id equals r.ExceptionDateId
                                       join e in full on r.EventId equals e.Id
                                       where e.Id == x.Id
                                       select y;
                        if (!xexdates.NullOrEmpty()) x.ExceptionDates.AddRangeComplement(xexdates);
                    }

                    if (!relatedtos.NullOrEmpty())
                    {
                        var xelatedtos = from y in relatedtos
                                         join r in rrelatedtos on y.Id equals r.RelatedToId
                                         join e in full on r.EventId equals e.Id
                                         where e.Id == x.Id
                                         select y;
                        if (!xelatedtos.NullOrEmpty()) x.RelatedTos.AddRangeComplement(xelatedtos);
                    }

                    if (!reqstats.NullOrEmpty())
                    {
                        var xeqstats = from y in reqstats
                                       join r in rreqstats on y.Id equals r.ReqStatsId
                                       join e in full on r.EventId equals e.Id
                                       where e.Id == x.Id
                                       select y;
                        if (!xeqstats.NullOrEmpty()) x.RequestStatuses.AddRangeComplement(xeqstats);

                    }

                    if (!resources.NullOrEmpty())
                    {
                        var xesources = from y in resources
                                        join r in rresources on y.Id equals r.ResourcesId
                                        join e in full on r.EventId equals e.Id
                                        where e.Id == x.Id
                                        select y;
                        if (!xesources.NullOrEmpty()) x.Resources.AddRangeComplement(xesources);
                    }

                    if (!aalarms.NullOrEmpty())
                    {
                        var xaalarms = from y in aalarms
                                       join r in raalarms on y.Id equals r.AlarmId
                                       join e in full on r.EventId equals e.Id
                                       where e.Id == x.Id
                                       select y;
                        if (!xaalarms.NullOrEmpty()) x.Alarms.AddRangeComplement(xaalarms);
                    }

                    if (!dalarms.NullOrEmpty())
                    {
                        var xdalarms = from y in dalarms
                                       join r in rdalarms on y.Id equals r.AlarmId
                                       join e in full on r.EventId equals e.Id
                                       where e.Id == x.Id
                                       select y;
                        if (!xdalarms.NullOrEmpty()) x.Alarms.AddRangeComplement(xdalarms);

                    }

                    if (!ealarms.NullOrEmpty())
                    {
                        var xealarms = from y in ealarms
                                       join r in realarms on y.Id equals r.AlarmId
                                       join e in full on r.EventId equals e.Id
                                       where e.Id == x.Id
                                       select y;
                        if (!xealarms.NullOrEmpty()) x.Alarms.AddRangeComplement(xealarms);
                    }
                }); 
                #endregion
            }

            return full ?? dry;
        }

        public IEnumerable<VEVENT> Get(int? skip = null)
        {
            var eclient = this.redis.As<VEVENT>();
            var dry = (skip != null) ?
                eclient.GetAll().Skip(skip.Value).Take(take.Value)
                : eclient.GetAll();
            return (!dry.NullOrEmpty()) ? this.Hydrate(dry) : dry;
        }

        public void Save(VEVENT entity)
        {
            using (var transaction = this.redis.CreateTransaction())
            {
                try
                {

                    #region retrieve attributes of entity

                    var org = entity.Organizer as ORGANIZER;
                    var rid = entity.RecurrenceId as RECURRENCE_ID;
                    var rrule = entity.RecurrenceRule as RECUR;
                    var attendees = entity.Attendees.OfType<ATTENDEE>();
                    var attachbins = entity.Attachments.OfType<ATTACH_BINARY>();
                    var attachuris = entity.Attachments.OfType<ATTACH_URI>();
                    var contacts = entity.Contacts.OfType<CONTACT>();
                    var comments = entity.Comments.OfType<COMMENT>();
                    var rdates = entity.RecurrenceDates.OfType<RDATE>();
                    var exdates = entity.ExceptionDates.OfType<EXDATE>();
                    var relatedtos = entity.RelatedTos.OfType<RELATEDTO>();
                    var resources = entity.Resources.OfType<RESOURCES>();
                    var reqstats = entity.RequestStatuses.OfType<REQUEST_STATUS>();
                    var aalarms = entity.Alarms.OfType<AUDIO_ALARM>();
                    var dalarms = entity.Alarms.OfType<DISPLAY_ALARM>();
                    var ealarms = entity.Alarms.OfType<EMAIL_ALARM>();

                    #endregion

                    #region save attributes and  relations
                    
                    var eclient = this.redis.As<VEVENT>();
                    transaction.QueueCommand(x => eclient.Store(entity));

                    if (!attendees.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => this.redis.As<ATTENDEE>().StoreAll(attendees));
                        var rattendees = attendees.Select(x => new REL_EVENTS_ATTENDEES
                        {
                            Id = KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            AttendeeId = x.Id
                        });

                        var rclient = this.redis.As<REL_EVENTS_ATTENDEES>();
                        var orattendees = rclient.GetAll().Where(x => x.EventId == entity.Id);
                        transaction.QueueCommand(x => rclient.StoreAll(
                            !orattendees.NullOrEmpty()
                            ? rattendees.Except(orattendees)
                            : rattendees));
                    }

                    if (!attachbins.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => this.redis.As<ATTACH_BINARY>().StoreAll(attachbins));
                        var rattachbins = attachbins.Select(x => new REL_EVENTS_ATTACHBINS
                        {
                            Id = KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            AttachmentId = x.Id
                        });

                        var rclient = this.redis.As<REL_EVENTS_ATTACHBINS>();
                        var orattachbins = rclient.GetAll().Where(x => x.EventId == entity.Id);
                        transaction.QueueCommand(x => rclient.StoreAll(
                            !orattachbins.NullOrEmpty()
                            ? rattachbins.Except(orattachbins)
                            : rattachbins));
                    }

                    if (!attachuris.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => this.redis.As<ATTACH_URI>().StoreAll(attachuris));
                        var rattachuris = attachuris.Select(x => new REL_EVENTS_ATTACHURIS
                        {
                            Id = KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            AttachmentId = x.Id
                        });

                        var rclient = this.redis.As<REL_EVENTS_ATTACHURIS>();
                        var orattachuris = rclient.GetAll().Where(x => x.EventId == entity.Id);
                        transaction.QueueCommand(x => rclient.StoreAll(
                            !orattachuris.NullOrEmpty()
                            ? rattachuris.Except(orattachuris)
                            : rattachuris));
                    }

                    if (!contacts.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => this.redis.As<CONTACT>().StoreAll(contacts));
                        var rcontacts = contacts.Select(x => new REL_EVENTS_CONTACTS
                        {
                            Id = KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            ContactId = x.Id
                        });

                        var rclient = this.redis.As<REL_EVENTS_CONTACTS>();
                        var orcontacts = rclient.GetAll().Where(x => x.EventId == entity.Id);
                        transaction.QueueCommand(x => rclient.StoreAll(
                            !orcontacts.NullOrEmpty()
                            ? rcontacts.Except(orcontacts)
                            : rcontacts));
                    }

                    if (!comments.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => this.redis.As<CONTACT>().StoreAll(contacts));
                        var rcontacts = comments.Select(x => new REL_EVENTS_CONTACTS
                        {
                            Id = KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            ContactId = x.Id
                        });

                        var rclient = this.redis.As<REL_EVENTS_CONTACTS>();
                        var orcontacts = rclient.GetAll().Where(x => x.EventId == entity.Id);
                        transaction.QueueCommand(x => rclient.StoreAll(
                            !orcontacts.NullOrEmpty()
                            ? rcontacts.Except(orcontacts)
                            : rcontacts));
                    }

                    if (!rdates.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => this.redis.As<RDATE>().StoreAll(rdates));
                        var rrdates = rdates.Select(x => new REL_EVENTS_RDATES
                        {
                            Id = KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            RecurrenceDateId = x.Id
                        });

                        var rclient = this.redis.As<REL_EVENTS_RDATES>();
                        var orrdates = rclient.GetAll().Where(x => x.EventId == entity.Id);
                        transaction.QueueCommand(x => rclient.StoreAll(
                            !orrdates.NullOrEmpty()
                            ? rrdates.Except(orrdates)
                            : rrdates));
                    }

                    if (!exdates.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => this.redis.As<EXDATE>().StoreAll(exdates));
                        var rexdates = exdates.Select(x => new REL_EVENTS_EXDATES
                        {
                            Id = KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            ExceptionDateId = x.Id
                        });

                        var rclient = this.redis.As<REL_EVENTS_EXDATES>();
                        var orexdates = rclient.GetAll().Where(x => x.EventId == entity.Id);
                        transaction.QueueCommand(x => rclient.StoreAll(
                            !orexdates.NullOrEmpty()
                            ? rexdates.Except(orexdates)
                            : rexdates));
                    }

                    if (!relatedtos.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => this.redis.As<RELATEDTO>().StoreAll(relatedtos));
                        var rrelatedtos = relatedtos.Select(x => new REL_EVENTS_RELATEDTOS
                        {
                            Id = KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            RelatedToId = x.Id
                        });

                        var rclient = this.redis.As<REL_EVENTS_RELATEDTOS>();
                        var orrelatedtos = rclient.GetAll().Where(x => x.EventId == entity.Id);
                        transaction.QueueCommand(x => rclient.StoreAll(
                            !orrelatedtos.NullOrEmpty()
                            ? rrelatedtos.Except(orrelatedtos)
                            : rrelatedtos));
                    }

                    if (!resources.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => this.redis.As<RESOURCES>().StoreAll(resources));
                        var rresources = resources.Select(x => new REL_EVENTS_RESOURCES
                        {
                            Id = KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            ResourcesId = x.Id
                        });

                        var rclient = this.redis.As<REL_EVENTS_RESOURCES>();
                        var orresources = rclient.GetAll().Where(x => x.EventId == entity.Id);
                        transaction.QueueCommand(x => rclient.StoreAll(
                            !orresources.NullOrEmpty()
                            ? rresources.Except(orresources)
                            : rresources));
                    }

                    if (!reqstats.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => this.redis.As<REQUEST_STATUS>().StoreAll(reqstats));
                        var rreqstats = resources.Select(x => new REL_EVENTS_REQSTATS
                        {
                            Id = KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            ReqStatsId = x.Id
                        });

                        var rclient = this.redis.As<REL_EVENTS_REQSTATS>();
                        var orreqstats = rclient.GetAll().Where(x => x.EventId == entity.Id);
                        transaction.QueueCommand(x => rclient.StoreAll(
                            !orreqstats.NullOrEmpty()
                            ? rreqstats.Except(orreqstats)
                            : rreqstats));
                    }

                    if (!aalarms.NullOrEmpty())
                    {
                        this.AudioAlarmRepository.SaveAll(aalarms);
                        var ralarms = resources.Select(x => new REL_EVENTS_AUDIO_ALARMS
                        {
                            Id = KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            AlarmId = x.Id
                        });

                        var rclient = this.redis.As<REL_EVENTS_AUDIO_ALARMS>();
                        var oralarms = rclient.GetAll().Where(x => x.EventId == entity.Id);
                        transaction.QueueCommand(x => rclient.StoreAll(
                            !oralarms.NullOrEmpty()
                            ? ralarms.Except(oralarms)
                            : ralarms));
                    }

                    if (!dalarms.NullOrEmpty())
                    {
                        this.DisplayAlarmRepository.SaveAll(dalarms);
                        var ralarms = resources.Select(x => new REL_EVENTS_DISPLAY_ALARMS
                        {
                            Id = KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            AlarmId = x.Id
                        });

                        var rclient = this.redis.As<REL_EVENTS_DISPLAY_ALARMS>();
                        var oralarms = rclient.GetAll().Where(x => x.EventId == entity.Id);
                        transaction.QueueCommand(x => rclient.StoreAll(
                            !oralarms.NullOrEmpty()
                            ? ralarms.Except(oralarms)
                            : ralarms));
                    }

                    if (!ealarms.NullOrEmpty())
                    {
                        this.EmailAlarmRepository.SaveAll(ealarms);
                        var ralarms = resources.Select(x => new REL_EVENTS_EMAIL_ALARMS
                        {
                            Id = KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            AlarmId = x.Id
                        });

                        var rclient = this.redis.As<REL_EVENTS_EMAIL_ALARMS>();
                        var oralarms = rclient.GetAll().Where(x => x.EventId == entity.Id);
                        transaction.QueueCommand(x => rclient.StoreAll(
                            !oralarms.NullOrEmpty()
                            ? ralarms.Except(oralarms)
                            : ralarms));
                    } 
                    
                    #endregion

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void Erase(string key)
        {
            var eclient = this.redis.As<VEVENT>();
            if (eclient.ContainsKey(key))
            {
                eclient.DeleteRelatedEntities<REL_EVENTS_ATTACHBINS>(key);
                eclient.DeleteRelatedEntities<REL_EVENTS_ATTACHURIS>(key);
                eclient.DeleteRelatedEntities<REL_EVENTS_ATTENDEES>(key);
                eclient.DeleteRelatedEntities<REL_EVENTS_AUDIO_ALARMS>(key);
                eclient.DeleteRelatedEntities<REL_EVENTS_COMMENTS>(key);
                eclient.DeleteRelatedEntities<REL_EVENTS_CONTACTS>(key);
                eclient.DeleteRelatedEntities<REL_EVENTS_DISPLAY_ALARMS>(key);
                eclient.DeleteRelatedEntities<REL_EVENTS_EMAIL_ALARMS>(key);
                eclient.DeleteRelatedEntities<REL_EVENTS_EXDATES>(key);
                eclient.DeleteRelatedEntities<REL_EVENTS_ORGANIZERS>(key);
                eclient.DeleteRelatedEntities<REL_EVENTS_RDATES>(key);
                eclient.DeleteRelatedEntities<REL_EVENTS_RECURRENCE_IDS>(key);
                eclient.DeleteRelatedEntities<REL_EVENTS_RELATEDTOS>(key);
                eclient.DeleteRelatedEntities<REL_EVENTS_REQSTATS>(key);
                eclient.DeleteRelatedEntities<REL_EVENTS_RESOURCES>(key);
                eclient.DeleteRelatedEntities<REL_EVENTS_RRULES>(key); 
                eclient.DeleteById(key);
            }
        }

        public void SaveAll(IEnumerable<VEVENT> entities)
        {
            throw new NotImplementedException();
        }

        public void EraseAll(IEnumerable<string> keys = null)
        {
            var eclient = this.redis.As<VEVENT>();
            Action<IRedisTypedClient<VEVENT>, string> cascade_on_delete = (e, k) =>
                {
                    if(e.ContainsKey(k))
                    {
                        eclient.DeleteRelatedEntities<REL_EVENTS_ATTACHBINS>(k);
                        eclient.DeleteRelatedEntities<REL_EVENTS_ATTACHURIS>(k);
                        eclient.DeleteRelatedEntities<REL_EVENTS_ATTENDEES>(k);
                        eclient.DeleteRelatedEntities<REL_EVENTS_AUDIO_ALARMS>(k);
                        eclient.DeleteRelatedEntities<REL_EVENTS_COMMENTS>(k);
                        eclient.DeleteRelatedEntities<REL_EVENTS_CONTACTS>(k);
                        eclient.DeleteRelatedEntities<REL_EVENTS_DISPLAY_ALARMS>(k);
                        eclient.DeleteRelatedEntities<REL_EVENTS_EMAIL_ALARMS>(k);
                        eclient.DeleteRelatedEntities<REL_EVENTS_EXDATES>(k);
                        eclient.DeleteRelatedEntities<REL_EVENTS_ORGANIZERS>(k);
                        eclient.DeleteRelatedEntities<REL_EVENTS_RDATES>(k);
                        eclient.DeleteRelatedEntities<REL_EVENTS_RECURRENCE_IDS>(k);
                        eclient.DeleteRelatedEntities<REL_EVENTS_RELATEDTOS>(k);
                        eclient.DeleteRelatedEntities<REL_EVENTS_REQSTATS>(k);
                        eclient.DeleteRelatedEntities<REL_EVENTS_RESOURCES>(k);
                        eclient.DeleteRelatedEntities<REL_EVENTS_RRULES>(k);
                    }
                };

            if (keys == null)
            {
                eclient.GetAllKeys().ForEach(x => cascade_on_delete(eclient, x));
                eclient.DeleteAll();
            }
            else
            {
                var dkeys = keys.Distinct().ToList();
                dkeys.ForEach(x => cascade_on_delete(eclient, x));
                eclient.DeleteByIds(dkeys);
            }
        }

        public IKeyGenerator<string> KeyGenerator 
        {
            get { return this.keygen; }
            set
            {
                if (value == null) throw new ArgumentNullException("KeyGenerator");
                this.keygen = value;
            }
        }

        public void Patch(VEVENT entity, Expression<Func<VEVENT, object>> fields, Expression<Func<VEVENT, bool>> where = null)
        {
            throw new NotImplementedException();
        }

        public VEVENT Find(string key)
        {
            var dry = this.redis.As<VEVENT>().GetValue(key);
            return (dry != null) ? this.Hydrate(dry) : dry;
        }

        public IEnumerable<VEVENT> Find(IEnumerable<string> keys, int? skip = null)
        {
            IEnumerable<VEVENT> dry = null;
            var eclient = this.redis.As<VEVENT>();
            var dkeys = keys.Distinct().ToList();
            if (eclient.GetAllKeys().Intersect(dkeys).Count() == dkeys.Count())
            {
                dry = (skip != null) ?
                    eclient.GetValues(dkeys).Skip(skip.Value).Take(take.Value)
                    : eclient.GetValues(dkeys);
            }
            return (!dry.NullOrEmpty()) ? this.Hydrate(dry) : dry;
        }

        public bool ContainsKey(string key)
        {
            return this.redis.As<VEVENT>().ContainsKey(key);
        }

        public bool ContainsKeys(IEnumerable<string> keys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            var matches = this.redis.As<VEVENT>().GetAllKeys().Intersect(keys);
            if (matches.NullOrEmpty()) return false;
            return mode == ExpectationMode.pessimistic
                ? matches.Count() == keys.Count()
                : !matches.NullOrEmpty();
        }

        public IEnumerable<string> GetKeys(int? skip = null)
        {
            if (skip == null) return this.redis.As<VEVENT>().GetAllKeys();
            else
            {
                var keys = this.redis.As<VEVENT>().GetAllKeys();
                return (!keys.NullOrEmpty())? keys.Skip(skip.Value).Take(take.Value): keys;
            }
        }

        public IEnumerable<VEVENT> Dehydrate(IEnumerable<VEVENT> full)
        {
            var dry = full.Select(x => 
            {
                return this.Dehydrate(x);
            });

            return dry;
        }

        public VEVENT Dehydrate(VEVENT full)
        {
            full.Organizer = null;
            full.RecurrenceId = null;
            full.RecurrenceRule = null;
            full.Attendees.Clear();
            full.Attachments.Clear();
            full.Contacts.Clear();
            full.Comments.Clear();
            full.RecurrenceDates.Clear();
            full.ExceptionDates.Clear();
            full.RelatedTos.Clear();
            full.RequestStatuses.Clear();
            full.Resources.Clear();
            full.Alarms.Clear();
            return full;
        }
    }
}
