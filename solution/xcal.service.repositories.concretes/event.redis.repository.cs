using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using reexmonkey.xcal.domain.models;
using reexmonkey.foundation.essentials.contracts;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.xcal.service.repositories.contracts;
using reexmonkey.technical.data.concretes.extensions.redis;
using reexmonkey.infrastructure.operations.contracts;

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
                this.client = manager.GetClient();
            }
        }

        public int? Take
        {
            get { return this.take; }
            set 
            {
                if (value == null) throw new ArgumentNullException("Take");
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
                this.ealarmrepository = value;
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

                var rrules = this.redis.As<REL_EVENTS_RECURS>().GetAll().Where(x => x.EventId == full.Id);
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
                    full.Alarms.AddRangeComplement(this.EmailAlarmRepository
                        .Hydrate(this.EmailAlarmRepository.Find(realarms.Select(x => x.AlarmId).ToList()))); 
                }
            }

            return full ?? dry;
        }

        public IEnumerable<VEVENT> Hydrate(IEnumerable<VEVENT> dry)
        {
            List<VEVENT> full = null;
            var eclient = this.redis.As<VEVENT>();
            var keys = dry.Select(x => x.Id).Distinct().ToList();

            full = eclient.GetByIds(keys).ToList();

            #region 1. retrieve relationships

            var rorgs = this.redis.As<REL_EVENTS_ORGANIZERS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rrids = this.redis.As<REL_EVENTS_RECURRENCE_IDS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rrrules = this.redis.As<REL_EVENTS_RECURS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rattachbins = this.redis.As<REL_EVENTS_ATTACHBINS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rattachuris = this.redis.As<REL_EVENTS_ATTACHURIS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rattendees = this.redis.As<REL_EVENTS_ATTENDEES>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rcomments = this.redis.As<REL_EVENTS_COMMENTS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rcontacts = this.redis.As<REL_EVENTS_CONTACTS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rrdates = this.redis.As<REL_EVENTS_RDATES>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rexdates = this.redis.As<REL_EVENTS_EXDATES>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rrelatedtos = this.redis.As<REL_EVENTS_RELATEDTOS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rreqstats = this.redis.As<REL_EVENTS_REQSTATS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rresources = this.redis.As<REL_EVENTS_RESOURCES>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var raalarms = this.redis.As<REL_EVENTS_AUDIO_ALARMS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rdalarms = this.redis.As<REL_EVENTS_DISPLAY_ALARMS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var realarms = this.redis.As<REL_EVENTS_EMAIL_ALARMS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();

            #endregion

            #region 2. retrieve secondary entities

            var orgs = (!rorgs.Empty()) ? this.redis.As<ORGANIZER>().GetByIds(rorgs.Select(x => x.OrganizerId)) : null;
            var rids = (!rrids.Empty()) ? this.redis.As<RECURRENCE_ID>().GetByIds(rrids.Select(x => x.RecurrenceId_Id)) : null;
            var rrules = (!rrrules.Empty()) ? this.redis.As<RECUR>().GetByIds(rrrules.Select(x => x.RecurrenceRuleId)) : null;
            var attachbins = (!rattachbins.Empty()) ? this.redis.As<ATTACH_BINARY>().GetByIds(rattachbins.Select(x => x.AttachmentId)) : null;
            var attachuris = (!rattachuris.Empty()) ? this.redis.As<ATTACH_URI>().GetByIds(rattachuris.Select(x => x.AttachmentId)) : null;
            var attendees = (!rattendees.Empty()) ? this.redis.As<ATTENDEE>().GetByIds(rattendees.Select(x => x.AttendeeId)) : null;
            var comments = (!rcomments.Empty()) ? this.redis.As<COMMENT>().GetByIds(rcomments.Select(x => x.CommentId)) : null;
            var contacts = (!rcontacts.Empty()) ? this.redis.As<CONTACT>().GetByIds(rcontacts.Select(x => x.ContactId)) : null;
            var rdates = (!rrdates.Empty()) ? this.redis.As<RDATE>().GetByIds(rrdates.Select(x => x.RecurrenceDateId)) : null;
            var exdates = (!rexdates.Empty()) ? this.redis.As<EXDATE>().GetByIds(rexdates.Select(x => x.ExceptionDateId)) : null;
            var relatedtos = (!rrelatedtos.Empty()) ? this.redis.As<RELATEDTO>().GetByIds(rrelatedtos.Select(x => x.RelatedToId)) : null;
            var reqstats = (!rreqstats.Empty()) ? this.redis.As<REQUEST_STATUS>().GetByIds(rreqstats.Select(x => x.ReqStatsId)) : null;
            var resources = (!rresources.Empty()) ? this.redis.As<RESOURCES>().GetByIds(rresources.Select(x => x.ResourcesId)) : null;
            var aalarms = (!raalarms.Empty()) ? this.AudioAlarmRepository.Find(raalarms.Select(x => x.AlarmId)) : null;
            var dalarms = (!rdalarms.Empty()) ? this.DisplayAlarmRepository.Find(rdalarms.Select(x => x.AlarmId)) : null;
            var ealarms = (!realarms.Empty()) ? this.EmailAlarmRepository.Hydrate(this.EmailAlarmRepository.Find(realarms.Select(x => x.AlarmId))) : null;

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

                if (!contacts.NullOrEmpty())
                {
                    var xcontacts = from y in contacts
                                    join r in rcontacts on y.Id equals r.ContactId
                                    join e in full on r.EventId equals e.Id
                                    where e.Id == x.Id
                                    select y;

                    if (!xcontacts.NullOrEmpty()) x.Contacts.AddRangeComplement(xcontacts);
                }


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

                    var org = entity.Organizer;
                    var rid = entity.RecurrenceId;
                    var rrule = entity.RecurrenceRule;
                    var attendees = entity.Attendees;
                    var attachbins = entity.Attachments.OfType<ATTACH_BINARY>();
                    var attachuris = entity.Attachments.OfType<ATTACH_URI>();
                    var contacts = entity.Contacts;
                    var comments = entity.Comments;
                    var rdates = entity.RecurrenceDates;
                    var exdates = entity.ExceptionDates;
                    var relatedtos = entity.RelatedTos;
                    var resources = entity.Resources;
                    var reqstats = entity.RequestStatuses;
                    var aalarms = entity.Alarms.OfType<AUDIO_ALARM>();
                    var dalarms = entity.Alarms.OfType<DISPLAY_ALARM>();
                    var ealarms = entity.Alarms.OfType<EMAIL_ALARM>();

                    #endregion

                    #region save attributes and  relations
                    
                    transaction.QueueCommand(x => this.redis.As<VEVENT>().Store(entity));

                    if(org != null)
                    {
                        transaction.QueueCommand(x => x.Store(org));
                        var rorg = new REL_EVENTS_ORGANIZERS 
                        { 
                            Id = KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            OrganizerId = org.Id
                        };
                        var rclient = this.redis.As<REL_EVENTS_ORGANIZERS>();
                        var ororgs = rclient.GetAll().Where(x => x.EventId == entity.Id);
                        transaction.QueueCommand(x => rclient.StoreAll(
                            !ororgs.NullOrEmpty()
                            ? rorg.ToSingleton().Except(ororgs)
                            : rorg.ToSingleton()));
                    }

                    if (rid != null)
                    {
                        transaction.QueueCommand(x => this.redis.As<RECURRENCE_ID>().Store(rid));
                        var rrid = new REL_EVENTS_RECURRENCE_IDS
                        {
                            Id = KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            RecurrenceId_Id = rid.Id
                        };

                        var rclient = this.redis.As<REL_EVENTS_RECURRENCE_IDS>();
                        var orrids = rclient.GetAll().Where(x => x.EventId == entity.Id);
                        transaction.QueueCommand(x => rclient.StoreAll(
                            !orrids.NullOrEmpty()
                            ? rrid.ToSingleton().Except(orrids)
                            : rrid.ToSingleton()));
                    }

                    if (rrule != null)
                    {
                        transaction.QueueCommand(x => this.redis.As<RECUR>().Store(rrule));
                        var rrrule = new REL_EVENTS_RECURS
                        {
                            Id = KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            RecurrenceRuleId = rid.Id
                        };

                        var rclient = this.redis.As<REL_EVENTS_RECURS>();
                        var orrrules = rclient.GetAll().Where(x => x.EventId == entity.Id);
                        transaction.QueueCommand(x => rclient.StoreAll(
                            !orrrules.NullOrEmpty()
                            ? rrrule.ToSingleton().Except(orrrules)
                            : rrrule.ToSingleton()));
                    }

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
                        transaction.QueueCommand(x => this.redis.As<TEXTUAL>().StoreAll(contacts));
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
                        transaction.QueueCommand(x => this.redis.As<TEXTUAL>().StoreAll(contacts));
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
                        transaction.QueueCommand(x => x.StoreAll(rdates));
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
                        transaction.QueueCommand(x => x.StoreAll(exdates));
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
                        transaction.QueueCommand(x => x.StoreAll(resources));
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
                        transaction.QueueCommand(x => x.StoreAll(reqstats));
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
                eclient.DeleteRelatedEntities<REL_EVENTS_RECURS>(key); 
                eclient.DeleteById(key);
            }
        }

        public void SaveAll(IEnumerable<VEVENT> entities)
        {
            #region 1. retrieve attributes of entities

            var orgs = entities.Where(x => x.Organizer != null).Select(x => x.Organizer);
            var rids = entities.Where(x => x.RecurrenceId != null).Select(x => x.RecurrenceId);
            var rrules = entities.Where(x => x.RecurrenceRule != null).Select(x => x.RecurrenceRule);
            var attendees = entities.Where(x => !x.Attendees.NullOrEmpty()).SelectMany(x => x.Attendees);
            var attachbins = entities.Where(x => !x.Attachments.NullOrEmpty() && !x.Attachments.OfType<ATTACH_BINARY>().NullOrEmpty())
                .SelectMany(x => x.Attachments.OfType<ATTACH_BINARY>());
            var attachuris = entities.Where(x => !x.Attachments.NullOrEmpty() && !x.Attachments.OfType<ATTACH_URI>().NullOrEmpty())
                .SelectMany(x => x.Attachments.OfType<ATTACH_URI>());
            var contacts = entities.Where(x => !x.Contacts.NullOrEmpty()).SelectMany(x => x.Contacts);
            var comments = entities.Where(x => !x.Comments.NullOrEmpty()).SelectMany(x => x.Comments);
            var rdates = entities.Where(x => !x.RecurrenceDates.NullOrEmpty()).SelectMany(x => x.RecurrenceDates);
            var exdates = entities.Where(x => !x.ExceptionDates.NullOrEmpty()).SelectMany(x => x.ExceptionDates);
            var relatedtos = entities.Where(x => !x.RelatedTos.NullOrEmpty()).SelectMany(x => x.RelatedTos);
            var resources = entities.Where(x => !x.Resources.NullOrEmpty()).SelectMany(x => x.Resources);
            var reqstats = entities.Where(x => !x.Resources.NullOrEmpty()).SelectMany(x => x.RequestStatuses);
            var aalarms = entities.Where(x => !x.Alarms.NullOrEmpty() && !x.Alarms.OfType<AUDIO_ALARM>().NullOrEmpty())
                .SelectMany(x => x.Alarms.OfType<AUDIO_ALARM>());
            var dalarms = entities.Where(x => !x.Alarms.NullOrEmpty() && !x.Alarms.OfType<DISPLAY_ALARM>().NullOrEmpty())
                .SelectMany(x => x.Alarms.OfType<DISPLAY_ALARM>());
            var ealarms = entities.Where(x => !x.Alarms.NullOrEmpty() && !x.Alarms.OfType<EMAIL_ALARM>().NullOrEmpty())
                .SelectMany(x => x.Alarms.OfType<EMAIL_ALARM>());

            #endregion

            #region 2. save aggregate attribbutes of entities

            this.manager.ExecTrans(transaction =>
            {
                try
                {
                    var keys = entities.Select(x => x.Id).ToArray();
                    if (!orgs.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(orgs));
                        var rorgs = entities.Where(x => x.Organizer != null)
                            .Select(e => new REL_EVENTS_ORGANIZERS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                OrganizerId = e.Organizer.Id
                            });

                        var rclient = this.redis.As<REL_EVENTS_ORGANIZERS>();
                        var ororgs = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                        transaction.QueueCommand(x => x.StoreAll(!ororgs.NullOrEmpty()
                            ? rorgs.Except(ororgs)
                            : rorgs));
                    }

                    if (!rids.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(rids));
                        var rrids = entities.Where(x => x.RecurrenceId != null)
                            .Select(e => new REL_EVENTS_RECURRENCE_IDS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                RecurrenceId_Id = e.RecurrenceId.Id
                            });
                        var rclient = this.redis.As<REL_EVENTS_RECURRENCE_IDS>();
                        var orrids = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                        transaction.QueueCommand(x => x.StoreAll((!orrids.NullOrEmpty())
                            ? rrids.Except(orrids)
                            : rrids));
                    }

                    if (!rrules.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(rrules));
                        var rrrules = entities.Where(x => x.RecurrenceRule != null)
                            .Select(e => new REL_EVENTS_RECURS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                RecurrenceRuleId = e.RecurrenceRule.Id
                            });

                        var rclient = this.redis.As<REL_EVENTS_RECURS>();
                        var orrrules = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                        transaction.QueueCommand(x => x.StoreAll((!orrrules.NullOrEmpty())
                            ? rrrules.Except(orrrules)
                            : rrrules));
                    }

                    if (!attendees.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(attendees));
                        var rattendees = entities.Where(x => !x.Attendees.NullOrEmpty())
                            .SelectMany(e => e.Attendees
                                .Select(x => new REL_EVENTS_ATTENDEES
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = e.Id,
                                    AttendeeId = x.Id
                                }));

                        var rclient = this.redis.As<REL_EVENTS_ATTENDEES>();
                        var orattendees = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                        transaction.QueueCommand(x => x.StoreAll((!orattendees.NullOrEmpty())
                            ? rattendees.Except(orattendees)
                            : rattendees));
                    }

                    if (!attachbins.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => this.redis.As<ATTACH_BINARY>().StoreAll(attachbins));
                        var rattachbins = entities.Where(x => !x.Attachments.OfType<ATTACH_BINARY>().NullOrEmpty())
                            .SelectMany(e => e.Attachments.OfType<ATTACH_BINARY>()
                                .Select(x => new REL_EVENTS_ATTACHBINS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = e.Id,
                                    AttachmentId = x.Id
                                }));
                        var rclient = this.redis.As<REL_EVENTS_ATTACHBINS>();
                        var orattachbins = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                        transaction.QueueCommand(x => x.StoreAll((!orattachbins.NullOrEmpty())
                            ? rattachbins.Except(orattachbins)
                            : rattachbins));
                    }

                    if (!attachuris.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(attachuris));
                        var rattachuris = entities.Where(x => !x.Attachments.OfType<ATTACH_URI>().NullOrEmpty())
                            .SelectMany(e => e.Attachments.OfType<ATTACH_URI>()
                                .Select(x => new REL_EVENTS_ATTACHURIS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = e.Id,
                                    AttachmentId = x.Id
                                }));
                        var rclient = this.redis.As<REL_EVENTS_ATTACHURIS>();
                        var orattachuris = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                        transaction.QueueCommand(x => x.StoreAll((!orattachuris.NullOrEmpty())
                            ? rattachuris.Except(orattachuris)
                            : rattachuris));
                    }

                    if (!contacts.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(contacts));
                        var rcontacts = entities.Where(x => !x.Contacts.NullOrEmpty())
                            .SelectMany(e => e.Contacts
                                .Select(x => new REL_EVENTS_CONTACTS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = e.Id,
                                    ContactId = x.Id
                                }));
                        var rclient = this.redis.As<REL_EVENTS_CONTACTS>();
                        var orcontacts = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                        transaction.QueueCommand(x => x.StoreAll((!orcontacts.NullOrEmpty())
                            ? rcontacts.Except(orcontacts)
                            : rcontacts));
                    }

                    if (!comments.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(comments));
                        var rcomments = entities.Where(x => !x.Comments.NullOrEmpty())
                            .SelectMany(e => e.Comments
                                .Select(x => new REL_EVENTS_COMMENTS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = e.Id,
                                    CommentId = x.Id
                                }));
                        var rclient = this.redis.As<REL_EVENTS_COMMENTS>();
                        var orcomments = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                        transaction.QueueCommand(x => x.StoreAll((!orcomments.NullOrEmpty())
                            ? rcomments.Except(orcomments)
                            : rcomments));
                    }

                    if (!rdates.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(rdates));
                        var rrdates = entities.Where(x => !x.RecurrenceDates.NullOrEmpty())
                            .SelectMany(e => e.RecurrenceDates.Select(x => new REL_EVENTS_RDATES
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                RecurrenceDateId = x.Id
                            }));
                        var rclient = this.redis.As<REL_EVENTS_RDATES>();
                        var orrdates = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                        transaction.QueueCommand(x => x.StoreAll((!orrdates.NullOrEmpty())
                            ? rrdates.Except(orrdates)
                            : rrdates));
                    }

                    if (!exdates.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(exdates));
                        var rexdates = entities.Where(x => !x.ExceptionDates.NullOrEmpty())
                            .SelectMany(e => e.ExceptionDates.Select(x => new REL_EVENTS_EXDATES
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                ExceptionDateId = x.Id
                            }));
                        var rclient = this.redis.As<REL_EVENTS_EXDATES>();
                        var orexdates = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                        transaction.QueueCommand(x => x.StoreAll((!orexdates.NullOrEmpty())
                            ? rexdates.Except(orexdates)
                            : rexdates));
                    }

                    if (!relatedtos.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(relatedtos));
                        var rrelateds = entities.Where(x => !x.RelatedTos.NullOrEmpty())
                            .SelectMany(e => e.RelatedTos.Select(x => new REL_EVENTS_RELATEDTOS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                RelatedToId = x.Id
                            }));
                        var rclient = this.redis.As<REL_EVENTS_RELATEDTOS>();
                        var orrelatedtos = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                        transaction.QueueCommand(x => x.StoreAll((!orrelatedtos.NullOrEmpty())
                            ? rrelateds.Except(orrelatedtos)
                            : rrelateds));
                    }

                    if (!resources.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(resources));
                        var rresources = entities.Where(x => !x.Resources.NullOrEmpty())
                            .SelectMany(e => e.Resources.Select(x => new REL_EVENTS_RESOURCES
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                ResourcesId = x.Id
                            }));
                        var rclient = this.redis.As<REL_EVENTS_RESOURCES>();
                        var orresources = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                        transaction.QueueCommand(x => x.StoreAll((!orresources.NullOrEmpty())
                            ? rresources.Except(orresources)
                            : rresources));
                    }

                    if (!reqstats.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(reqstats));
                        var rreqstats = entities.Where(x => !x.RequestStatuses.NullOrEmpty())
                            .SelectMany(e => e.RequestStatuses.Select(x => new REL_EVENTS_REQSTATS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                ReqStatsId = x.Id
                            }));
                        var rclient = this.redis.As<REL_EVENTS_REQSTATS>();
                        var orreqstats = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                        transaction.QueueCommand(x => x.StoreAll((!orreqstats.NullOrEmpty())
                            ? rreqstats.Except(orreqstats)
                            : rreqstats));
                    }

                    if (!aalarms.NullOrEmpty())
                    {
                        this.AudioAlarmRepository.SaveAll(aalarms);
                        var raalarms = entities.Where(x => !x.Alarms.OfType<AUDIO_ALARM>().NullOrEmpty())
                            .SelectMany(e => e.RequestStatuses.OfType<AUDIO_ALARM>().Select(x => new REL_EVENTS_AUDIO_ALARMS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                AlarmId = x.Id
                            }));
                        var rclient = this.redis.As<REL_EVENTS_AUDIO_ALARMS>();
                        var oraalarms = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                        transaction.QueueCommand(x => x.StoreAll((!oraalarms.NullOrEmpty())
                            ? raalarms.Except(oraalarms)
                            : raalarms));
                    }

                    if (!dalarms.NullOrEmpty())
                    {
                        this.DisplayAlarmRepository.SaveAll(dalarms);
                        var rdalarms = entities.Where(x => !x.Alarms.OfType<DISPLAY_ALARM>().NullOrEmpty()).SelectMany(e => e.RequestStatuses.OfType<DISPLAY_ALARM>().Select(x => new REL_EVENTS_DISPLAY_ALARMS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = e.Id,
                            AlarmId = x.Id
                        }));
                        var rclient = this.redis.As<REL_EVENTS_DISPLAY_ALARMS>();
                        var ordalarms = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                        transaction.QueueCommand(x => x.StoreAll((!ordalarms.NullOrEmpty())
                            ? rdalarms.Except(ordalarms)
                            : rdalarms));
                    }

                    if (!ealarms.NullOrEmpty())
                    {
                        this.EmailAlarmRepository.SaveAll(ealarms);
                        var realarms = entities.Where(x => !x.Alarms.OfType<EMAIL_ALARM>().NullOrEmpty())
                            .SelectMany(e => e.RequestStatuses.OfType<EMAIL_ALARM>().Select(x => new REL_EVENTS_EMAIL_ALARMS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                AlarmId = x.Id
                            }));

                        var rclient = this.redis.As<REL_EVENTS_EMAIL_ALARMS>();
                        var orealarms = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                        transaction.QueueCommand(x => x.StoreAll((!orealarms.NullOrEmpty())
                            ? realarms.Except(orealarms)
                            : realarms));
                    }

                    transaction.QueueCommand(x => x.StoreAll(this.Dehydrate(entities)));
                }
                catch (RedisResponseException) { throw; }
                catch (RedisException) { throw; }
                catch (InvalidOperationException) { throw; }
            });

            #endregion
            
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
                        eclient.DeleteRelatedEntities<REL_EVENTS_RECURS>(k);
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

        public void Patch(VEVENT source, Expression<Func<VEVENT, object>> fields, Expression<Func<VEVENT, bool>> predicate = null)
        {
            #region construct anonymous fields using expression lambdas
            
            var selection = fields.GetMemberNames();

            Expression<Func<VEVENT, object>> primitives = x => new
            {
                x.Uid,
                x.Start,
                x.Created,
                x.Description,
                Geo = x.Position,
                x.LastModified,
                x.Location,
                x.Priority,
                x.Sequence,
                x.Status,
                x.Summary,
                x.Transparency,
                x.Url,
                x.End,
                x.Duration
            };

            Expression<Func<VEVENT, object>> relations = x => new
            {
                x.Organizer,
                x.RecurrenceId,
                x.RecurrenceRule,
                x.Attendees,
                x.Attachments,
                x.Contacts,
                x.Comments,
                x.RecurrenceDates,
                x.ExceptionDates,
                x.RelatedTos,
                x.Resources,
                x.RequestStatuses,
                x.Alarms
            };

            //4. Get list of selected relationals
            var srelation = relations.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

            //5. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

            #endregion

            #region save (insert or update) relational attributes

            if (!srelation.NullOrEmpty())
            {
                Expression<Func<VEVENT, object>> orgexpr = y => y.Organizer;
                Expression<Func<VEVENT, object>> ridexpr = y => y.RecurrenceId;
                Expression<Func<VEVENT, object>> rruleexpr = y => y.RecurrenceRule;
                Expression<Func<VEVENT, object>> attendsexpr = y => y.Attendees;
                Expression<Func<VEVENT, object>> attachsexpr = y => y.Attachments;
                Expression<Func<VEVENT, object>> contactsexpr = y => y.Contacts;
                Expression<Func<VEVENT, object>> commentsexpr = y => y.Comments;
                Expression<Func<VEVENT, object>> rdatesexpr = y => y.RecurrenceDates;
                Expression<Func<VEVENT, object>> exdatesexpr = y => y.ExceptionDates;
                Expression<Func<VEVENT, object>> relatedtosexpr = y => y.RelatedTos;
                Expression<Func<VEVENT, object>> resourcesexpr = y => y.Resources;
                Expression<Func<VEVENT, object>> reqstatsexpr = y => y.RequestStatuses;
                Expression<Func<VEVENT, object>> alarmexpr = y => y.Alarms;

                string[] keys = null;
                var eclient = this.redis.As<VEVENT>();
                    
                keys = (predicate != null)
                        ? eclient.GetAll().Where(predicate.Compile()).Select(x => x.Id).ToArray()
                        : eclient.GetAllKeys().ToArray();



                #region save relational aggregate attributes of entities

                using (var transaction = this.redis.CreateTransaction())
                {

                    try
                    {
                                                
                        if (selection.Contains(orgexpr.GetMemberName()))
                        {
                            //get events-organizers relations
                            var org = source.Organizer;                                                
                            if(org != null)
                            {

                                transaction.QueueCommand(x => x.Store(org));
                                var rorgs = keys.Select( x => new REL_EVENTS_ORGANIZERS
                                {
                                    Id = KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    OrganizerId = org.Id
                                });
                                var rclient = this.redis.As<REL_EVENTS_ORGANIZERS>();
                                var ororgs = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                                transaction.QueueCommand(x => rclient.StoreAll(!ororgs.NullOrEmpty()
                                    ? rorgs.Except(ororgs)
                                    : rorgs));
                            }

                        }

                        if(selection.Contains(ridexpr.GetMemberName()))
                        {
                            var rid = source.RecurrenceId;
                            if(rid != null)
                            {
                                transaction.QueueCommand(x => this.redis.As<RECURRENCE_ID>().Store(rid));
                                var rrids = keys.Select(x => new REL_EVENTS_RECURRENCE_IDS
                                {
                                    Id = KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    RecurrenceId_Id = rid.Id
                                });

                                var rclient = this.redis.As<REL_EVENTS_RECURRENCE_IDS>();
                                var orrids = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                                transaction.QueueCommand(x => x.StoreAll(
                                    !orrids.NullOrEmpty()
                                    ? rrids.Except(orrids)
                                    : rrids));
                            }
                        }


                        if (selection.Contains(rruleexpr.GetMemberName()))
                        {
                            var rrule = source.RecurrenceRule;
                            if (rrule != null)
                            {
                                transaction.QueueCommand(x => x.Store(rrule));
                                var rrrules = keys.Select( x => new REL_EVENTS_RECURS
                                {
                                    Id = KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    RecurrenceRuleId = rrule.Id
                                });

                                var rclient = this.redis.As<REL_EVENTS_RECURS>();
                                var orrrules = rclient.GetAll().Where(x => x.EventId == source.Id);
                                transaction.QueueCommand(x => x.StoreAll(
                                    !orrrules.NullOrEmpty()
                                    ? rrrules.Except(orrrules)
                                    : rrrules));
                            }
                        }

                        if (selection.Contains(attendsexpr.GetMemberName()))
                        {
                            var attendees = source.Attendees;
                            if (!attendees.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => this.redis.As<ATTENDEE>().StoreAll(attendees));
                                var rattendees = keys.SelectMany(x => attendees.Select(y => new REL_EVENTS_ATTENDEES
                                {
                                    Id = KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    AttendeeId = y.Id
                                }));

                                var rclient = this.redis.As<REL_EVENTS_ATTENDEES>();
                                var orattendees = rclient.GetAll().Where(x => x.EventId == source.Id);
                                transaction.QueueCommand(x => x.StoreAll(
                                    !orattendees.NullOrEmpty()
                                    ? rattendees.Except(orattendees)
                                    : rattendees));
                            }
                        }

                        if (selection.Contains(attachsexpr.GetMemberName()))
                        {
                            var attachbins = source.Attachments.OfType<ATTACH_BINARY>();
                            if (!attachbins.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => this.redis.As<ATTACH_BINARY>().StoreAll(attachbins));
                                var rattachbins = keys.SelectMany(x => attachbins.Select(y => new REL_EVENTS_ATTACHBINS
                                {
                                    Id = KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    AttachmentId = y.Id
                                }));
                                
                                var rclient = this.redis.As<REL_EVENTS_ATTACHBINS>();
                                var orattachbins = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                                transaction.QueueCommand(x => x.StoreAll(!orattachbins.NullOrEmpty()
                                    ? rattachbins.Except(orattachbins)
                                    : rattachbins));
                            }

                            var attachuris = source.Attachments.OfType<ATTACH_URI>();
                            if (!attachuris.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => x.StoreAll(attachuris));
                                var rattachuris = keys.SelectMany(x => attachuris.Select(y => new REL_EVENTS_ATTACHURIS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    AttachmentId = y.Id
                                }));
                                var rclient = this.redis.As<REL_EVENTS_ATTACHURIS>();
                                var orattachuris = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                                transaction.QueueCommand(x => x.StoreAll(!orattachuris.NullOrEmpty()
                                    ? rattachuris.Except(orattachuris)
                                    : rattachuris));
                            }
                        }

                        if (selection.Contains(contactsexpr.GetMemberName()))
                        {
                            var contacts = source.Contacts;
                            if (!contacts.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => x.StoreAll(contacts));
                                var rcontacts = keys.SelectMany(x => contacts.Select(y => new REL_EVENTS_CONTACTS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    ContactId = y.Id
                                }));
                                var rclient = this.redis.As<REL_EVENTS_CONTACTS>();
                                var orcontacts = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                                transaction.QueueCommand(x => x.StoreAll(!orcontacts.NullOrEmpty()
                                    ? rcontacts.Except(orcontacts)
                                    : rcontacts));
                            }
                        }

                        if (selection.Contains(commentsexpr.GetMemberName()))
                        {
                            var comments = source.Contacts;
                            if (!comments.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => x.StoreAll(comments));
                                var rcomments = keys.SelectMany(x => comments.Select(y => new REL_EVENTS_COMMENTS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    CommentId = y.Id
                                }));
                                var rclient = this.redis.As<REL_EVENTS_COMMENTS>();
                                var orcomments = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                                transaction.QueueCommand(x => x.StoreAll(!orcomments.NullOrEmpty()
                                    ? rcomments.Except(orcomments)
                                    : rcomments));
                            }
                        }

                        if (selection.Contains(rdatesexpr.GetMemberName()))
                        {
                            var rdates = source.Contacts;
                            if (!rdates.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => x.StoreAll(rdates));
                                var rrdates = keys.SelectMany(x => rdates.Select(y => new REL_EVENTS_RDATES
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    RecurrenceDateId = y.Id
                                }));
                                var rclient = this.redis.As<REL_EVENTS_RDATES>();
                                var orrdates = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                                transaction.QueueCommand(x => x.StoreAll(!orrdates.NullOrEmpty()
                                    ? rrdates.Except(orrdates)
                                    : rrdates));
                            }
                        }

                        if (selection.Contains(exdatesexpr.GetMemberName()))
                        {
                            var exdates = source.ExceptionDates;
                            if (!exdates.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => x.StoreAll(exdates));
                                var rexdates = keys.SelectMany(x => exdates.Select(y => new REL_EVENTS_EXDATES
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    ExceptionDateId = y.Id
                                }));
                                var rclient = this.redis.As<REL_EVENTS_EXDATES>();
                                var orrexdates = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                                transaction.QueueCommand(x => x.StoreAll(!orrexdates.NullOrEmpty()
                                    ? rexdates.Except(orrexdates)
                                    : rexdates));
                            }
                        }

                        if (selection.Contains(relatedtosexpr.GetMemberName()))
                        {
                            var relatedtos = source.RelatedTos;
                            if (!relatedtos.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => x.StoreAll(relatedtos));
                                var rrelatedtos = keys.SelectMany(x => relatedtos.Select(y => new REL_EVENTS_RELATEDTOS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    RelatedToId = y.Id
                                }));
                                var rclient = this.redis.As<REL_EVENTS_RELATEDTOS>();
                                var orrelatedtos = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                                transaction.QueueCommand(x => x.StoreAll(!orrelatedtos.NullOrEmpty()
                                    ? rrelatedtos.Except(orrelatedtos)
                                    : rrelatedtos));
                            }
                        }

                        if (selection.Contains(resourcesexpr.GetMemberName()))
                        {
                            var resources = source.Resources;
                            if (!resources.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => x.StoreAll(resources));
                                var rresources = keys.SelectMany(x => resources.Select(y => new REL_EVENTS_RESOURCES
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    ResourcesId = y.Id
                                }));
                                var rclient = this.redis.As<REL_EVENTS_RESOURCES>();
                                var orresources = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                                transaction.QueueCommand(x => x.StoreAll(!orresources.NullOrEmpty()
                                    ? rresources.Except(orresources)
                                    : rresources));
                            }
                        }

                        if (selection.Contains(reqstatsexpr.GetMemberName()))
                        {
                            var reqstats = source.RequestStatuses;
                            if (!reqstats.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => x.StoreAll(reqstats));
                                var rreqstats = keys.SelectMany(x => reqstats.Select(y => new REL_EVENTS_REQSTATS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    ReqStatsId = y.Id
                                }));
                                var rclient = this.redis.As<REL_EVENTS_REQSTATS>();
                                var orreqstats = rclient.GetAll().Where(x => keys.Contains(x.EventId));
                                transaction.QueueCommand(x => x.StoreAll(!orreqstats.NullOrEmpty()
                                    ? rreqstats.Except(orreqstats)
                                    : rreqstats));
                            }
                        }

                    }
                    catch (RedisResponseException)
                    {
                        throw;
                    }
                    catch (RedisException)
                    {
                        throw;
                    }
                }

                #endregion

                

            } 

            #endregion

            //TODO: rewrite this patch method - include section for patching primitives

        }

        public VEVENT Find(string key)
        {
            return this.redis.As<VEVENT>().GetById(key);
        }

        public IEnumerable<VEVENT> Find(IEnumerable<string> keys, int? skip = null)
        {
            IEnumerable<VEVENT> dry = null;
            
            var eclient = this.redis.As<VEVENT>();
            var dkeys = keys.Distinct().ToList();
            dry = (skip != null)
                ? eclient.GetByIds(dkeys).Skip(skip.Value).Take(take.Value)
                : eclient.GetByIds(dkeys);
            return dry;
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
            if(!full.Attendees.NullOrEmpty())full.Attendees.Clear();
            if (!full.Attachments.NullOrEmpty()) full.Attachments.Clear();
            if (!full.Contacts.NullOrEmpty()) full.Contacts.Clear();
            if (!full.Comments.NullOrEmpty()) full.Comments.Clear();
            if (!full.RecurrenceDates.NullOrEmpty()) full.RecurrenceDates.Clear();
            if (!full.ExceptionDates.NullOrEmpty()) full.ExceptionDates.Clear();
            if (!full.RelatedTos.NullOrEmpty()) full.RelatedTos.Clear();
            if (!full.RequestStatuses.NullOrEmpty()) full.RequestStatuses.Clear();
            if (!full.Resources.NullOrEmpty()) full.Resources.Clear();
            if (!full.Alarms.NullOrEmpty()) full.Alarms.Clear();
            return full;
        }
    }
}
