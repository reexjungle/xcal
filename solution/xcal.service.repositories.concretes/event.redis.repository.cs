using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using ServiceStack.Common;
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

        public IKeyGenerator<string> KeyGenerator
        {
            get { return this.keygen; }
            set
            {
                if (value == null) throw new ArgumentNullException("KeyGenerator");
                this.keygen = value;
            }
        }

        public EventRedisRepository(){}

        public EventRedisRepository(IRedisClientsManager manager)
        {
            this.RedisClientsManager = manager;
            this.client = manager.GetClient();
        }

        public EventRedisRepository(IRedisClientsManager manager, 
            IAudioAlarmRepository aalarmrepository, 
            IDisplayAlarmRepository dalarmrepository,
            IEmailAlarmRepository ealarmrepository)
        {
            this.RedisClientsManager = manager;
            this.AudioAlarmRepository = aalarmrepository;
            this.DisplayAlarmRepository = dalarmrepository;
            this.EmailAlarmRepository = ealarmrepository;
        }

        public VEVENT Hydrate(VEVENT dry)
        {
            var full = dry;
            if (full != null)
            {
                var rorgs = this.redis.As<REL_EVENTS_ORGANIZERS>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rorgs.NullOrEmpty()) full.Organizer = this.redis.As<ORGANIZER>().GetById(rorgs.FirstOrDefault().OrganizerId);

                var rrids = this.redis.As<REL_EVENTS_RECURRENCE_IDS>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rrids.NullOrEmpty()) full.RecurrenceId = this.redis.As<RECURRENCE_ID>().GetById(rrids.FirstOrDefault().RecurrenceId_Id);

                var rrules = this.redis.As<REL_EVENTS_RECURS>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rrules.NullOrEmpty()) full.RecurrenceRule = this.redis.As<RECUR>().GetById(rrules.FirstOrDefault().RecurrenceRuleId);

                var rattachbins = this.redis.As<REL_EVENTS_ATTACHBINS>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rattachbins.NullOrEmpty())
                {
                    full.Attachments.AddRangeComplement(this.redis.As<ATTACH_BINARY>().GetByIds(rattachbins.Select(x => x.AttachmentId).ToList()));
                }

                var rattachuris = this.redis.As<REL_EVENTS_ATTACHURIS>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rattachuris.NullOrEmpty())
                {
                    full.Attachments.AddRangeComplement(this.redis.As<ATTACH_URI>().GetByIds(rattachuris.Select(x => x.AttachmentId).ToList()));
                }

                var rattendees = this.redis.As<REL_EVENTS_ATTENDEES>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rattendees.NullOrEmpty())
                {
                    full.Attendees.AddRangeComplement(this.redis.As<ATTENDEE>().GetByIds(rattendees.Select(x => x.AttendeeId).ToList()));
                }

                var rcomments = this.redis.As<REL_EVENTS_COMMENTS>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rcomments.NullOrEmpty())
                {
                    full.Comments.AddRangeComplement(this.redis.As<COMMENT>().GetByIds(rcomments.Select(x => x.CommentId).ToList()));
                }

                var rcontacts = this.redis.As<REL_EVENTS_CONTACTS>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rcontacts.NullOrEmpty())
                {
                    full.Contacts.AddRangeComplement(this.redis.As<CONTACT>().GetByIds(rcontacts.Select(x => x.ContactId).ToList()));
                }

                var rrdates = this.redis.As<REL_EVENTS_RDATES>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rrdates.NullOrEmpty())
                {
                    full.RecurrenceDates.AddRangeComplement(this.redis.As<RDATE>().GetByIds(rrdates.Select(x => x.RecurrenceDateId).ToList()));
                }

                var rexdates = this.redis.As<REL_EVENTS_EXDATES>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rexdates.NullOrEmpty())
                {
                    full.ExceptionDates.AddRangeComplement(this.redis.As<EXDATE>().GetByIds(rexdates.Select(x => x.ExceptionDateId).ToList()));
                }

                var rrelatedtos = this.redis.As<REL_EVENTS_RELATEDTOS>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rrelatedtos.NullOrEmpty())
                {
                    full.RelatedTos.AddRangeComplement(this.redis.As<RELATEDTO>().GetByIds(rrelatedtos.Select(x => x.RelatedToId).ToList()));
                }

                var rreqstats = this.redis.As<REL_EVENTS_REQSTATS>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rreqstats.NullOrEmpty())
                {
                    full.RequestStatuses.AddRangeComplement(this.redis.As<REQUEST_STATUS>().GetByIds(rreqstats.Select(x => x.ReqStatsId).ToList()));
                }

                var rresources = this.redis.As<REL_EVENTS_RESOURCES>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rresources.NullOrEmpty())
                {
                    full.Resources.AddRangeComplement(this.redis.As<RESOURCES>().GetByIds(rresources.Select(x => x.ResourcesId).ToList()));
                }

                var raalarms = this.redis.As<REL_EVENTS_AUDIO_ALARMS>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if(!raalarms.NullOrEmpty())
                {
                    full.Alarms.AddRangeComplement(this.AudioAlarmRepository.FindAll(raalarms.Select(x => x.AlarmId).ToList()));
                }

                var rdalarms = this.redis.As<REL_EVENTS_DISPLAY_ALARMS>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rdalarms.NullOrEmpty())
                {
                    full.Alarms.AddRangeComplement(this.DisplayAlarmRepository.FindAll(rdalarms.Select(x => x.AlarmId).ToList()));
                }

                var realarms = this.redis.As<REL_EVENTS_EMAIL_ALARMS>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!realarms.NullOrEmpty())
                {
                    full.Alarms.AddRangeComplement(this.EmailAlarmRepository
                        .HydrateAll(this.EmailAlarmRepository.FindAll(realarms.Select(x => x.AlarmId).ToList()))); 
                }

                //TODO: retrieve IANA and x- properties of events during hydration

            }

            return full ?? dry;
        }

        public IEnumerable<VEVENT> HydrateAll(IEnumerable<VEVENT> dry)
        {
            try
            {
                var full = dry.ToList();
                var keys = full.Select(x => x.Id).Distinct().ToList();

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

                //TODO: retrieve IANA and x- properties of events during hydration 

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
                var aalarms = (!raalarms.Empty()) ? this.AudioAlarmRepository.FindAll(raalarms.Select(x => x.AlarmId)) : null;
                var dalarms = (!rdalarms.Empty()) ? this.DisplayAlarmRepository.FindAll(rdalarms.Select(x => x.AlarmId)) : null;
                var ealarms = (!realarms.Empty()) ? this.EmailAlarmRepository.HydrateAll(this.EmailAlarmRepository.FindAll(realarms.Select(x => x.AlarmId))) : null;

                #endregion

                #region 3. Use Linq to stitch secondary entities to primary entities

                full.ForEach(x =>
                {
                    if (!orgs.NullOrEmpty())
                    {
                        var xorgs = from y in orgs
                                    join r in rorgs on y.Id equals r.OrganizerId
                                    join e in full on r.EventId equals e.Id
                                    where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                    select y;
                        if (!xorgs.NullOrEmpty()) x.Organizer = xorgs.FirstOrDefault();
                    }

                    if (!rids.NullOrEmpty())
                    {
                        var xrids = from y in rids
                                    join r in rrids on y.Id equals r.RecurrenceId_Id
                                    join e in full on r.EventId equals e.Id
                                    where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                    select y;
                        if (!xrids.NullOrEmpty()) x.RecurrenceId = xrids.FirstOrDefault();
                    }

                    if (!rrules.NullOrEmpty())
                    {
                        var xrules = from y in rrules
                                     join r in rrrules on y.Id equals r.RecurrenceRuleId
                                     join e in full on r.EventId equals e.Id
                                     where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                     select y;
                        if (!xrules.NullOrEmpty()) x.RecurrenceRule = xrules.FirstOrDefault();
                    }

                    if (!comments.NullOrEmpty())
                    {
                        var xcomments = from y in comments
                                        join r in rcomments on y.Id equals r.CommentId
                                        join e in full on r.EventId equals e.Id
                                        where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                        select y;
                        if (!xcomments.NullOrEmpty()) x.Comments.AddRangeComplement(xcomments);
                    }

                    if (!attendees.NullOrEmpty())
                    {
                        var xattendees = from y in attendees
                                         join r in rattendees on y.Id equals r.AttendeeId
                                         join e in full on r.EventId equals e.Id
                                         where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                         select y;
                        if (!xattendees.NullOrEmpty()) x.Attendees.AddRange(xattendees);
                    }

                    if (!attachbins.NullOrEmpty())
                    {
                        var xattachbins = from y in attachbins
                                          join r in rattachbins on y.Id equals r.AttachmentId
                                          join e in full on r.EventId equals e.Id
                                          where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                          select y;
                        if (!xattachbins.NullOrEmpty()) x.Attachments.AddRangeComplement(xattachbins);

                    }

                    if (!attachuris.NullOrEmpty())
                    {
                        var xattachuris = from y in attachuris
                                          join r in rattachuris on y.Id equals r.AttachmentId
                                          join e in full on r.EventId equals e.Id
                                          where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                          select y;
                        if (!xattachuris.NullOrEmpty()) x.Attachments.AddRangeComplement(xattachuris);
                    }

                    if (!contacts.NullOrEmpty())
                    {
                        var xcontacts = from y in contacts
                                        join r in rcontacts on y.Id equals r.ContactId
                                        join e in full on r.EventId equals e.Id
                                        where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                        select y;

                        if (!xcontacts.NullOrEmpty()) x.Contacts.AddRangeComplement(xcontacts);
                    }


                    if (!rdates.NullOrEmpty())
                    {
                        var xdates = from y in rdates
                                     join r in rrdates on y.Id equals r.RecurrenceDateId
                                     join e in full on r.EventId equals e.Id
                                     where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                     select y;
                        if (!xdates.NullOrEmpty()) x.RecurrenceDates.AddRangeComplement(xdates);
                    }

                    if (!exdates.NullOrEmpty())
                    {
                        var xexdates = from y in exdates
                                       join r in rexdates on y.Id equals r.ExceptionDateId
                                       join e in full on r.EventId equals e.Id
                                       where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                       select y;
                        if (!xexdates.NullOrEmpty()) x.ExceptionDates.AddRangeComplement(xexdates);
                    }

                    if (!relatedtos.NullOrEmpty())
                    {
                        var xelatedtos = from y in relatedtos
                                         join r in rrelatedtos on y.Id equals r.RelatedToId
                                         join e in full on r.EventId equals e.Id
                                         where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                         select y;
                        if (!xelatedtos.NullOrEmpty()) x.RelatedTos.AddRangeComplement(xelatedtos);
                    }

                    if (!reqstats.NullOrEmpty())
                    {
                        var xeqstats = from y in reqstats
                                       join r in rreqstats on y.Id equals r.ReqStatsId
                                       join e in full on r.EventId equals e.Id
                                       where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                       select y;
                        if (!xeqstats.NullOrEmpty()) x.RequestStatuses.AddRangeComplement(xeqstats);

                    }

                    if (!resources.NullOrEmpty())
                    {
                        var xesources = from y in resources
                                        join r in rresources on y.Id equals r.ResourcesId
                                        join e in full on r.EventId equals e.Id
                                        where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                        select y;
                        if (!xesources.NullOrEmpty()) x.Resources.AddRangeComplement(xesources);
                    }

                    if (!aalarms.NullOrEmpty())
                    {
                        var xaalarms = from y in aalarms
                                       join r in raalarms on y.Id equals r.AlarmId
                                       join e in full on r.EventId equals e.Id
                                       where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                       select y;
                        if (!xaalarms.NullOrEmpty()) x.Alarms.AddRangeComplement(xaalarms);
                    }

                    if (!dalarms.NullOrEmpty())
                    {
                        var xdalarms = from y in dalarms
                                       join r in rdalarms on y.Id equals r.AlarmId
                                       join e in full on r.EventId equals e.Id
                                       where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                       select y;
                        if (!xdalarms.NullOrEmpty()) x.Alarms.AddRangeComplement(xdalarms);

                    }

                    if (!ealarms.NullOrEmpty())
                    {
                        var xealarms = from y in ealarms
                                       join r in realarms on y.Id equals r.AlarmId
                                       join e in full on r.EventId equals e.Id
                                       where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                       select y;
                        if (!xealarms.NullOrEmpty()) x.Alarms.AddRangeComplement(xealarms);
                    }
                });

                #endregion

                return full ?? dry;
            }
            catch (ArgumentNullException) { throw; }
        }

        public IEnumerable<VEVENT> Get(int? skip = null, int? take = null)
        {
            try
            {
                if (skip == null && take == null) return this.redis.As<VEVENT>().GetAll();
                else
                {
                    var allkeys = this.redis.As<VEVENT>().GetAllKeys();
                    var selected = !allkeys.NullOrEmpty()
                        ? allkeys.Skip(skip.Value).Take(take.Value)
                        : new List<string>();
                    var dry = this.redis.As<VEVENT>().GetValues(selected.ToList());
                    return !dry.NullOrEmpty() ? this.HydrateAll(dry) : dry;
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public void Save(VEVENT entity)
        {
            this.manager.ExecTrans(transaction =>
            {
                try
                {
                    var keys = this.redis.As<VEVENT>().GetAllKeys().ToArray();
                    if (!keys.NullOrEmpty()) this.redis.Watch(keys);

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
                    var ianaprops = entity.IANAProperties;
                    var xprops = entity.XProperties;

                    #endregion

                    #region save attributes and  relations

                    transaction.QueueCommand(x => this.redis.As<VEVENT>().Store(entity));

                    if (org != null)
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

                    if (!ianaprops.NullOrEmpty())
                    {
                        //TODO: Save IANA properties of event
                    }

                    if (!xprops.NullOrEmpty())
                    {
                        //TODO: Save x-properties of event
                    }

                    #endregion
                }
                catch (ArgumentNullException) { throw; }
                catch (RedisResponseException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
                catch (RedisException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
                catch (InvalidOperationException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
            });
        }

        public void Erase(string key)
        {
            try
            {
                var eclient = this.redis.As<VEVENT>();
                if (eclient.ContainsKey(key))
                {

                    var rattachbins = this.redis.As<REL_EVENTS_ATTACHBINS>().GetAll();
                    if (!rattachbins.NullOrEmpty()) this.redis.As<REL_EVENTS_ATTACHBINS>()
                        .DeleteByIds(rattachbins.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase)));

                    var rattachuris = this.redis.As<REL_EVENTS_ATTACHURIS>().GetAll();
                    if (!rattachuris.NullOrEmpty()) this.redis.As<REL_EVENTS_ATTACHURIS>()
                        .DeleteByIds(rattachuris.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase)));

                    var rattendees = this.redis.As<REL_EVENTS_ATTENDEES>().GetAll();
                    if (!rattachbins.NullOrEmpty()) this.redis.As<REL_EVENTS_ATTENDEES>()
                        .DeleteByIds(rattendees.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase)));

                    var raalarms = this.redis.As<REL_EVENTS_AUDIO_ALARMS>().GetAll();
                    if (!raalarms.NullOrEmpty()) this.redis.As<REL_EVENTS_AUDIO_ALARMS>()
                        .DeleteByIds(raalarms.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase)));

                    var rcomments = this.redis.As<REL_EVENTS_COMMENTS>().GetAll();
                    if (!rcomments.NullOrEmpty()) this.redis.As<REL_EVENTS_COMMENTS>()
                        .DeleteByIds(rcomments.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase)));

                    var rcontacts = this.redis.As<REL_EVENTS_CONTACTS>().GetAll();
                    if (!rcontacts.NullOrEmpty()) this.redis.As<REL_EVENTS_CONTACTS>()
                        .DeleteByIds(rcontacts.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase)));

                    var rdalarms = this.redis.As<REL_EVENTS_DISPLAY_ALARMS>().GetAll();
                    if (!rdalarms.NullOrEmpty()) this.redis.As<REL_EVENTS_DISPLAY_ALARMS>()
                        .DeleteByIds(rdalarms.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase)));

                    var realarms = this.redis.As<REL_EVENTS_EMAIL_ALARMS>().GetAll();
                    if (!realarms.NullOrEmpty()) this.redis.As<REL_EVENTS_EMAIL_ALARMS>()
                        .DeleteByIds(realarms.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase)));

                    var rexdates = this.redis.As<REL_EVENTS_EXDATES>().GetAll();
                    if (!rexdates.NullOrEmpty()) this.redis.As<REL_EVENTS_EXDATES>()
                        .DeleteByIds(rexdates.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase)));

                    var rorgs = this.redis.As<REL_EVENTS_ORGANIZERS>().GetAll();
                    if (!rorgs.NullOrEmpty()) this.redis.As<REL_EVENTS_ORGANIZERS>()
                        .DeleteByIds(rorgs.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase)));

                    var rrdates = this.redis.As<REL_EVENTS_RDATES>().GetAll();
                    if (!rrdates.NullOrEmpty()) this.redis.As<REL_EVENTS_RDATES>()
                        .DeleteByIds(rrdates.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase)));

                    var rrids = this.redis.As<REL_EVENTS_RECURRENCE_IDS>().GetAll();
                    if (!rrids.NullOrEmpty()) this.redis.As<REL_EVENTS_RECURRENCE_IDS>()
                        .DeleteByIds(rrids.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase)));

                    var rrelatedtos = this.redis.As<REL_EVENTS_RELATEDTOS>().GetAll();
                    if (!rrelatedtos.NullOrEmpty()) this.redis.As<REL_EVENTS_RELATEDTOS>()
                        .DeleteByIds(rrelatedtos.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase)));

                    var rreqstats = this.redis.As<REL_EVENTS_REQSTATS>().GetAll();
                    if (!rreqstats.NullOrEmpty()) this.redis.As<REL_EVENTS_REQSTATS>()
                        .DeleteByIds(rreqstats.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase)));

                    var rresources = this.redis.As<REL_EVENTS_RESOURCES>().GetAll();
                    if (!rresources.NullOrEmpty()) this.redis.As<REL_EVENTS_RESOURCES>()
                        .DeleteByIds(rresources.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase)));

                    var rrrules = this.redis.As<REL_EVENTS_RECURS>().GetAll();
                    if (!rrrules.NullOrEmpty()) this.redis.As<REL_EVENTS_RECURS>()
                        .DeleteByIds(rrrules.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase)));

                    //TODO Erase IANA Properties of event

                    //TODO Erase X-Properties of event

                    eclient.DeleteById(key);
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
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

            var ianaprops = entities.Where(x => !x.IANAProperties.NullOrEmpty()).SelectMany(x => x.IANAProperties);
            var xprops = entities.Where(x => !x.IANAProperties.NullOrEmpty()).SelectMany(x => x.XProperties);

            #endregion

            #region 2. save aggregate attribbutes of entities

            this.manager.ExecTrans(transaction =>
            {
                try
                {
                    var eclient = this.redis.As<VEVENT>();
                    //watch keys
                    var okeys = eclient.GetAllKeys().ToArray();
                    if (!okeys.NullOrEmpty()) this.redis.Watch(okeys);

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

                    if(!ianaprops.NullOrEmpty())
                    {
                        //TODO: Save all IANA properties of events
                    }

                    if(!xprops.NullOrEmpty())
                    {
                        //TODO: Save all x-properties of events
                    }

                    transaction.QueueCommand(x => x.StoreAll(this.Dehydrate(entities)));

                }
                catch (ArgumentNullException) { throw; }
                catch (RedisResponseException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
                catch (RedisException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
                catch (InvalidOperationException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
            });

            #endregion
            
        }

        public void EraseAll(IEnumerable<string> keys = null)
        {
            try
            {
                var eclient = this.redis.As<VEVENT>();
                var allkeys = eclient.GetAllKeys().Select(x => UrnId.GetStringId(x));
                if (!allkeys.NullOrEmpty())
                {
                    var found = allkeys.Intersect(keys);
                    var rattachbins = this.redis.As<REL_EVENTS_ATTACHBINS>().GetAll();
                    if (!rattachbins.NullOrEmpty()) this.redis.As<REL_EVENTS_ATTACHBINS>()
                        .DeleteByIds(rattachbins.Where(x => found.Contains(x.EventId, StringComparer.OrdinalIgnoreCase)));

                    var rattachuris = this.redis.As<REL_EVENTS_ATTACHURIS>().GetAll();
                    if (!rattachuris.NullOrEmpty()) this.redis.As<REL_EVENTS_ATTACHURIS>()
                        .DeleteByIds(rattachuris.Where(x => found.Contains(x.EventId, StringComparer.OrdinalIgnoreCase)));

                    var rattendees = this.redis.As<REL_EVENTS_ATTENDEES>().GetAll();
                    if (!rattendees.NullOrEmpty()) this.redis.As<REL_EVENTS_ATTENDEES>()
                        .DeleteByIds(rattendees.Where(x => found.Contains(x.EventId, StringComparer.OrdinalIgnoreCase)));

                    var raalarms = this.redis.As<REL_EVENTS_AUDIO_ALARMS>().GetAll();
                    if (!raalarms.NullOrEmpty()) this.redis.As<REL_EVENTS_AUDIO_ALARMS>()
                        .DeleteByIds(raalarms.Where(x => found.Contains(x.EventId, StringComparer.OrdinalIgnoreCase)));

                    var rcomments = this.redis.As<REL_EVENTS_COMMENTS>().GetAll();
                    if (!rcomments.NullOrEmpty()) this.redis.As<REL_EVENTS_COMMENTS>()
                        .DeleteByIds(rcomments.Where(x => found.Contains(x.EventId, StringComparer.OrdinalIgnoreCase)));

                    var rcontacts = this.redis.As<REL_EVENTS_CONTACTS>().GetAll();
                    if (!rcontacts.NullOrEmpty()) this.redis.As<REL_EVENTS_CONTACTS>()
                        .DeleteByIds(rcontacts.Where(x => found.Contains(x.EventId, StringComparer.OrdinalIgnoreCase)));

                    var rdalarms = this.redis.As<REL_EVENTS_DISPLAY_ALARMS>().GetAll();
                    if (!rdalarms.NullOrEmpty()) this.redis.As<REL_EVENTS_DISPLAY_ALARMS>()
                        .DeleteByIds(rdalarms.Where(x => found.Contains(x.EventId, StringComparer.OrdinalIgnoreCase)));

                    var realarms = this.redis.As<REL_EVENTS_EMAIL_ALARMS>().GetAll();
                    if (!realarms.NullOrEmpty()) this.redis.As<REL_EVENTS_EMAIL_ALARMS>()
                        .DeleteByIds(realarms.Where(x => found.Contains(x.EventId, StringComparer.OrdinalIgnoreCase)));

                    var rexdates = this.redis.As<REL_EVENTS_EXDATES>().GetAll();
                    if (!rexdates.NullOrEmpty()) this.redis.As<REL_EVENTS_EXDATES>()
                        .DeleteByIds(rexdates.Where(x => found.Contains(x.EventId, StringComparer.OrdinalIgnoreCase)));

                    var rorgs = this.redis.As<REL_EVENTS_ORGANIZERS>().GetAll();
                    if (!rorgs.NullOrEmpty()) this.redis.As<REL_EVENTS_ORGANIZERS>()
                        .DeleteByIds(rorgs.Where(x => found.Contains(x.EventId, StringComparer.OrdinalIgnoreCase)));

                    var rrdates = this.redis.As<REL_EVENTS_RDATES>().GetAll();
                    if (!rrdates.NullOrEmpty()) this.redis.As<REL_EVENTS_RDATES>()
                        .DeleteByIds(rrdates.Where(x => found.Contains(x.EventId, StringComparer.OrdinalIgnoreCase)));

                    var rrids = this.redis.As<REL_EVENTS_RECURRENCE_IDS>().GetAll();
                    if (!rrids.NullOrEmpty()) this.redis.As<REL_EVENTS_RECURRENCE_IDS>()
                        .DeleteByIds(rrids.Where(x => found.Contains(x.EventId, StringComparer.OrdinalIgnoreCase)));

                    var rrelatedtos = this.redis.As<REL_EVENTS_RELATEDTOS>().GetAll();
                    if (!rrelatedtos.NullOrEmpty()) this.redis.As<REL_EVENTS_RELATEDTOS>()
                        .DeleteByIds(rrelatedtos.Where(x => found.Contains(x.EventId, StringComparer.OrdinalIgnoreCase)));

                    var rreqstats = this.redis.As<REL_EVENTS_REQSTATS>().GetAll();
                    if (!rreqstats.NullOrEmpty()) this.redis.As<REL_EVENTS_REQSTATS>()
                        .DeleteByIds(rreqstats.Where(x => found.Contains(x.EventId, StringComparer.OrdinalIgnoreCase)));

                    var rresources = this.redis.As<REL_EVENTS_RESOURCES>().GetAll();
                    if (!rresources.NullOrEmpty()) this.redis.As<REL_EVENTS_RESOURCES>()
                        .DeleteByIds(rresources.Where(x => found.Contains(x.EventId, StringComparer.OrdinalIgnoreCase)));

                    var rrrules = this.redis.As<REL_EVENTS_RECURS>().GetAll();
                    if (!rrrules.NullOrEmpty()) this.redis.As<REL_EVENTS_RECURS>()
                        .DeleteByIds(rrrules.Where(x => found.Contains(x.EventId, StringComparer.OrdinalIgnoreCase)));

                    //TODO Erase All IANA Properties of events

                    //TODO Erase All X-Properties of events

                    eclient.DeleteByIds(found);
                }
                else eclient.DeleteAll();
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public void Patch(VEVENT source, Expression<Func<VEVENT, object>> fields, IEnumerable<string> keys = null)
        {
            #region construct anonymous fields using expression lambdas
            
            var selection = fields.GetMemberNames();

            Expression<Func<VEVENT, object>> primitives = x => new
            {
                x.Uid,
                x.Start,
                x.Created,
                x.Description,
                x.Position,
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
                x.Alarms,
                x.IANAProperties,
                x.XProperties
            };

            //4. Get list of selected relationals
            var srelation = relations.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

            //5. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

            #endregion

            this.manager.ExecTrans(transaction =>
            {
                try
                {
                    var eclient = this.redis.As<VEVENT>();
                    var okeys = eclient.GetAllKeys().ToArray();
                    if (!okeys.NullOrEmpty()) this.redis.Watch(okeys);

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
                        Expression<Func<VEVENT, object>> ianaexpr = y => y.IANAProperties;
                        Expression<Func<VEVENT, object>> xpropsexpr = y => y.XProperties;

                        #region save relational aggregate attributes of entities

                        if (selection.Contains(orgexpr.GetMemberName()))
                        {
                            //get events-organizers relations
                            var org = source.Organizer;
                            if (org != null)
                            {

                                transaction.QueueCommand(x => x.Store(org));
                                var rorgs = keys.Select(x => new REL_EVENTS_ORGANIZERS
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

                        if (selection.Contains(ridexpr.GetMemberName()))
                        {
                            var rid = source.RecurrenceId;
                            if (rid != null)
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
                                var rrrules = keys.Select(x => new REL_EVENTS_RECURS
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

                        if (selection.Contains(ianaexpr.GetMemberName()))
                        {
                            //TODO: patch all iana properties
                        }

                        if (selection.Contains(xpropsexpr.GetMemberName()))
                        {
                            //TODO: patch all x-properties
                        }

                        #endregion

                    }

                    #endregion

                    #region save (insert or update non-relational attributes

                    if (!sprimitives.NullOrEmpty())
                    {
                        Expression<Func<VEVENT, object>> startexpr = x => x.Start;
                        Expression<Func<VEVENT, object>> classexpr = x => x.Classification;
                        Expression<Func<VEVENT, object>> descexpr = x => x.Description;
                        Expression<Func<VEVENT, object>> posexpr = x => x.Position;
                        Expression<Func<VEVENT, object>> locexpr = x => x.Location;
                        Expression<Func<VEVENT, object>> priexpr = x => x.Priority;
                        Expression<Func<VEVENT, object>> seqexpr = x => x.Sequence;
                        Expression<Func<VEVENT, object>> statexpr = x => x.Status;
                        Expression<Func<VEVENT, object>> summexpr = x => x.Summary;
                        Expression<Func<VEVENT, object>> transpexpr = x => x.Transparency;
                        Expression<Func<VEVENT, object>> urlexpr = x => x.Url;
                        Expression<Func<VEVENT, object>> rruleexpr = x => x.RecurrenceRule;
                        Expression<Func<VEVENT, object>> endexpr = x => x.End;
                        Expression<Func<VEVENT, object>> durexpr = x => x.Duration;
                        Expression<Func<VEVENT, object>> catexpr = x => x.Categories;

                        var entities = eclient.GetByIds(keys).ToList();
                        entities.ForEach(x =>
                        {
                            if (selection.Contains(startexpr.GetMemberName())) x.Start = source.Start;
                            if (selection.Contains(classexpr.GetMemberName())) x.Classification = source.Classification;
                            if (selection.Contains(descexpr.GetMemberName())) x.Description = source.Description;
                            if (selection.Contains(posexpr.GetMemberName())) x.Position = source.Position;
                            if (selection.Contains(locexpr.GetMemberName())) x.Location = source.Location;
                            if (selection.Contains(priexpr.GetMemberName())) x.Priority = source.Priority;
                            if (selection.Contains(seqexpr.GetMemberName())) x.Sequence = source.Sequence;
                            if (selection.Contains(statexpr.GetMemberName())) x.Status = source.Status;
                            if (selection.Contains(summexpr.GetMemberName())) x.Summary = source.Summary;
                            if (selection.Contains(transpexpr.GetMemberName())) x.Transparency = source.Transparency;
                            if (selection.Contains(urlexpr.GetMemberName())) x.Url = source.Url;
                            if (selection.Contains(rruleexpr.GetMemberName())) x.RecurrenceRule = source.RecurrenceRule;
                            if (selection.Contains(endexpr.GetMemberName())) x.End = source.End;
                            if (selection.Contains(durexpr.GetMemberName())) x.Duration = source.Duration;
                            if (selection.Contains(catexpr.GetMemberName())) x.Categories = source.Categories;
                        });

                        transaction.QueueCommand(x => x.StoreAll(this.Dehydrate(entities)));

                    }


                    #endregion
                }
                catch (ArgumentNullException) { throw; }
                catch (RedisResponseException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
                catch (RedisException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
                catch (InvalidOperationException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
            });

        }

        public VEVENT Find(string key)
        {
            try
            {
                var dry = this.redis.As<VEVENT>().GetById(key);
                return dry != null ? this.Hydrate(dry) : dry;
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public IEnumerable<VEVENT> FindAll(IEnumerable<string> keys, int? skip = null, int? take = null)
        {
            try
            {
                var allkeys = this.redis.As<VEVENT>().GetAllKeys();
                if (skip == null && take == null)
                {
                    var filtered = !keys.NullOrEmpty()
                        ? allkeys.Intersect(keys.Select(x => UrnId.GetStringId(x)))
                        : new List<string>();
                    var dry = this.redis.As<VEVENT>().GetByIds(filtered);
                    return !dry.NullOrEmpty() ? this.HydrateAll(dry) : dry;
                }
                else
                {
                    var filtered = !keys.NullOrEmpty()
                        ? allkeys.Intersect(keys.Select(x => UrnId.GetStringId(x)))
                        : new List<string>();
                    var dry = this.redis.As<VEVENT>().GetByIds(filtered);
                    return !dry.NullOrEmpty() ? this.HydrateAll(dry) : dry;
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public bool ContainsKey(string key)
        {
            try
            {
                return this.redis.As<VEVENT>().ContainsKey(key);
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public bool ContainsKeys(IEnumerable<string> keys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            try
            {
                var matches = this.redis.As<VEVENT>().GetAllKeys().Intersect(keys);
                if (matches.NullOrEmpty()) return false;
                return mode == ExpectationMode.pessimistic
                    ? matches.Count() == keys.Count()
                    : !matches.NullOrEmpty();
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public IEnumerable<string> GetKeys(int? skip = null, int? take = null)
        {
            try
            {
                var keys = this.redis.As<VEVENT>().GetAllKeys();
                if (skip == null && take == null)
                    return !keys.NullOrEmpty()
                        ? keys.Select(x => UrnId.GetStringId(x))
                        : new List<string>();
                else
                    return (!keys.NullOrEmpty())
                        ? keys.Select(x => UrnId.GetStringId(x)).Skip(skip.Value).Take(take.Value)
                        : new List<string>();
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public IEnumerable<VEVENT> Dehydrate(IEnumerable<VEVENT> full)
        {
            try
            {
                return full.Select(x => { return this.Dehydrate(x); });

            }
            catch (ArgumentNullException) { throw; }
        }

        public VEVENT Dehydrate(VEVENT full)
        {
            try
            {
                full.Organizer = null;
                full.RecurrenceId = null;
                full.RecurrenceRule = null;
                if (!full.Attendees.NullOrEmpty()) full.Attendees.Clear();
                if (!full.Attachments.NullOrEmpty()) full.Attachments.Clear();
                if (!full.Contacts.NullOrEmpty()) full.Contacts.Clear();
                if (!full.Comments.NullOrEmpty()) full.Comments.Clear();
                if (!full.RecurrenceDates.NullOrEmpty()) full.RecurrenceDates.Clear();
                if (!full.ExceptionDates.NullOrEmpty()) full.ExceptionDates.Clear();
                if (!full.RelatedTos.NullOrEmpty()) full.RelatedTos.Clear();
                if (!full.RequestStatuses.NullOrEmpty()) full.RequestStatuses.Clear();
                if (!full.Resources.NullOrEmpty()) full.Resources.Clear();
                if (!full.Alarms.NullOrEmpty()) full.Alarms.Clear();
                if (!full.IANAProperties.NullOrEmpty()) full.IANAProperties.Clear();
                if (!full.XProperties.NullOrEmpty()) full.XProperties.Clear();
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            return full;
        }
    }
}
