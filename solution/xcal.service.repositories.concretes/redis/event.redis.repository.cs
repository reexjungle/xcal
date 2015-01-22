using reexjungle.foundation.essentials.concretes;
using reexjungle.foundation.essentials.contracts;
using reexjungle.infrastructure.contracts;
using reexjungle.technical.data.concretes.extensions.redis;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.repositories.concretes.relations;
using reexjungle.xcal.service.repositories.contracts;
using ServiceStack.Common;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace reexjungle.xcal.service.repositories.concretes.redis
{
    public class EventRedisRepository : IEventRedisRepository
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

        public EventRedisRepository()
        {
        }

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
                var rrecurs = this.redis.As<REL_EVENTS_RECURS>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rrecurs.NullOrEmpty())
                {
                    full.RecurrenceRule = this.redis.As<RECUR>().GetById(rrecurs.FirstOrDefault().RecurId);
                }

                var rorgs = this.redis.As<REL_EVENTS_ORGANIZERS>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rorgs.NullOrEmpty())
                {
                    full.Organizer = this.redis.As<ORGANIZER>().GetById(rorgs.FirstOrDefault().OrganizerId);
                }

                var rattachbins = this.redis.As<REL_EVENTS_ATTACHBINS>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rattachbins.NullOrEmpty())
                {
                    full.AttachmentBinaries.MergeRange(this.redis.As<ATTACH_BINARY>().GetByIds(rattachbins.Select(x => x.AttachmentId).ToList()));
                }

                var rattachuris = this.redis.As<REL_EVENTS_ATTACHURIS>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rattachuris.NullOrEmpty())
                {
                    full.AttachmentUris.MergeRange(this.redis.As<ATTACH_URI>().GetByIds(rattachuris.Select(x => x.AttachmentId).ToList()));
                }

                var rattendees = this.redis.As<REL_EVENTS_ATTENDEES>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rattendees.NullOrEmpty())
                {
                    full.Attendees.MergeRange(this.redis.As<ATTENDEE>().GetByIds(rattendees.Select(x => x.AttendeeId).ToList()));
                }

                var rcomments = this.redis.As<REL_EVENTS_COMMENTS>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rcomments.NullOrEmpty())
                {
                    full.Comments.MergeRange(this.redis.As<COMMENT>().GetByIds(rcomments.Select(x => x.CommentId).ToList()));
                }

                var rcontacts = this.redis.As<REL_EVENTS_CONTACTS>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rcontacts.NullOrEmpty())
                {
                    full.Contacts.MergeRange(this.redis.As<CONTACT>().GetByIds(rcontacts.Select(x => x.ContactId).ToList()));
                }

                var rrdates = this.redis.As<REL_EVENTS_RDATES>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rrdates.NullOrEmpty())
                {
                    full.RecurrenceDates.MergeRange(this.redis.As<RDATE>().GetByIds(rrdates.Select(x => x.RecurrenceDateId).ToList()));
                }

                var rexdates = this.redis.As<REL_EVENTS_EXDATES>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rexdates.NullOrEmpty())
                {
                    full.ExceptionDates.MergeRange(this.redis.As<EXDATE>().GetByIds(rexdates.Select(x => x.ExceptionDateId).ToList()));
                }

                var rrelatedtos = this.redis.As<REL_EVENTS_RELATEDTOS>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rrelatedtos.NullOrEmpty())
                {
                    full.RelatedTos.MergeRange(this.redis.As<RELATEDTO>().GetByIds(rrelatedtos.Select(x => x.RelatedToId).ToList()));
                }

                var rreqstats = this.redis.As<REL_EVENTS_REQSTATS>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rreqstats.NullOrEmpty())
                {
                    full.RequestStatuses.MergeRange(this.redis.As<REQUEST_STATUS>().GetByIds(rreqstats.Select(x => x.ReqStatsId).ToList()));
                }

                var rresources = this.redis.As<REL_EVENTS_RESOURCES>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rresources.NullOrEmpty())
                {
                    full.Resources.MergeRange(this.redis.As<RESOURCES>().GetByIds(rresources.Select(x => x.ResourcesId).ToList()));
                }

                var raalarms = this.redis.As<REL_EVENTS_AUDIO_ALARMS>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!raalarms.NullOrEmpty())
                {
                    full.AudioAlarms.MergeRange(this.AudioAlarmRepository.FindAll(raalarms.Select(x => x.AlarmId).ToList()));
                }

                var rdalarms = this.redis.As<REL_EVENTS_DISPLAY_ALARMS>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!rdalarms.NullOrEmpty())
                {
                    full.DisplayAlarms.MergeRange(this.DisplayAlarmRepository.FindAll(rdalarms.Select(x => x.AlarmId).ToList()));
                }

                var realarms = this.redis.As<REL_EVENTS_EMAIL_ALARMS>().GetAll().Where(x => x.EventId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!realarms.NullOrEmpty())
                {
                    full.EmailAlarms.MergeRange(this.EmailAlarmRepository
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
                var keys = full.Select(x => x.Id).ToList();
                var okeys = this.redis.GetAllKeys().Intersect(keys).ToArray();

                #region 1. retrieve relationships

                var rrecurs = this.redis.As<REL_EVENTS_RECURS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
                var rorgs = this.redis.As<REL_EVENTS_ORGANIZERS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
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

                #endregion 1. retrieve relationships

                #region 2. retrieve secondary entities

                var recurs = (!rrecurs.Empty()) ? this.redis.As<RECUR>().GetByIds(rrecurs.Select(x => x.RecurId)) : null;
                var organizers = (!rorgs.Empty()) ? this.redis.As<ORGANIZER>().GetByIds(rorgs.Select(x => x.OrganizerId)) : null;
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

                #endregion 2. retrieve secondary entities

                #region 3. Use Linq to stitch secondary entities to primary entities

                full.ForEach(x =>
                {
                    if (!recurs.NullOrEmpty())
                    {
                        var xrecurs = from y in recurs
                                      join r in rrecurs on y.Id equals r.RecurId
                                      join e in full on r.EventId equals e.Id
                                      where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                      select y;
                        if (!xrecurs.NullOrEmpty()) x.RecurrenceRule = xrecurs.First();
                    }

                    if (!organizers.NullOrEmpty())
                    {
                        var xorgs = from y in organizers
                                    join r in rorgs on y.Id equals r.OrganizerId
                                    join e in full on r.EventId equals e.Id
                                    where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                    select y;
                        if (!xorgs.NullOrEmpty()) x.Organizer = xorgs.First();
                    }

                    if (!comments.NullOrEmpty())
                    {
                        var xcomments = from y in comments
                                        join r in rcomments on y.Id equals r.CommentId
                                        join e in full on r.EventId equals e.Id
                                        where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                        select y;
                        if (!xcomments.NullOrEmpty()) x.Comments.MergeRange(xcomments);
                    }

                    if (!attendees.NullOrEmpty())
                    {
                        var xattendees = from y in attendees
                                         join r in rattendees on y.Id equals r.AttendeeId
                                         join e in full on r.EventId equals e.Id
                                         where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                         select y;
                        if (!xattendees.NullOrEmpty()) x.Attendees.MergeRange(xattendees);
                    }

                    if (!attachbins.NullOrEmpty())
                    {
                        var xattachbins = from y in attachbins
                                          join r in rattachbins on y.Id equals r.AttachmentId
                                          join e in full on r.EventId equals e.Id
                                          where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                          select y;
                        if (!xattachbins.NullOrEmpty()) x.AttachmentBinaries.MergeRange(xattachbins);
                    }

                    if (!attachuris.NullOrEmpty())
                    {
                        var xattachuris = from y in attachuris
                                          join r in rattachuris on y.Id equals r.AttachmentId
                                          join e in full on r.EventId equals e.Id
                                          where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                          select y;
                        if (!xattachuris.NullOrEmpty()) x.AttachmentUris.MergeRange(xattachuris);
                    }

                    if (!contacts.NullOrEmpty())
                    {
                        var xcontacts = from y in contacts
                                        join r in rcontacts on y.Id equals r.ContactId
                                        join e in full on r.EventId equals e.Id
                                        where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                        select y;

                        if (!xcontacts.NullOrEmpty()) x.Contacts.MergeRange(xcontacts);
                    }

                    if (!rdates.NullOrEmpty())
                    {
                        var xdates = from y in rdates
                                     join r in rrdates on y.Id equals r.RecurrenceDateId
                                     join e in full on r.EventId equals e.Id
                                     where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                     select y;
                        if (!xdates.NullOrEmpty()) x.RecurrenceDates.MergeRange(xdates);
                    }

                    if (!exdates.NullOrEmpty())
                    {
                        var xexdates = from y in exdates
                                       join r in rexdates on y.Id equals r.ExceptionDateId
                                       join e in full on r.EventId equals e.Id
                                       where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                       select y;
                        if (!xexdates.NullOrEmpty()) x.ExceptionDates.MergeRange(xexdates);
                    }

                    if (!relatedtos.NullOrEmpty())
                    {
                        var xelatedtos = from y in relatedtos
                                         join r in rrelatedtos on y.Id equals r.RelatedToId
                                         join e in full on r.EventId equals e.Id
                                         where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                         select y;
                        if (!xelatedtos.NullOrEmpty()) x.RelatedTos.MergeRange(xelatedtos);
                    }

                    if (!reqstats.NullOrEmpty())
                    {
                        var xeqstats = from y in reqstats
                                       join r in rreqstats on y.Id equals r.ReqStatsId
                                       join e in full on r.EventId equals e.Id
                                       where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                       select y;
                        if (!xeqstats.NullOrEmpty()) x.RequestStatuses.MergeRange(xeqstats);
                    }

                    if (!resources.NullOrEmpty())
                    {
                        var xesources = from y in resources
                                        join r in rresources on y.Id equals r.ResourcesId
                                        join e in full on r.EventId equals e.Id
                                        where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                        select y;
                        if (!xesources.NullOrEmpty()) x.Resources.MergeRange(xesources);
                    }

                    if (!aalarms.NullOrEmpty())
                    {
                        var xaalarms = from y in aalarms
                                       join r in raalarms on y.Id equals r.AlarmId
                                       join e in full on r.EventId equals e.Id
                                       where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                       select y;
                        if (!xaalarms.NullOrEmpty()) x.AudioAlarms.MergeRange(xaalarms);
                    }

                    if (!dalarms.NullOrEmpty())
                    {
                        var xdalarms = from y in dalarms
                                       join r in rdalarms on y.Id equals r.AlarmId
                                       join e in full on r.EventId equals e.Id
                                       where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                       select y;
                        if (!xdalarms.NullOrEmpty()) x.DisplayAlarms.MergeRange(xdalarms);
                    }

                    if (!ealarms.NullOrEmpty())
                    {
                        var xealarms = from y in ealarms
                                       join r in realarms on y.Id equals r.AlarmId
                                       join e in full on r.EventId equals e.Id
                                       where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                       select y;
                        if (!xealarms.NullOrEmpty()) x.EmailAlarms.MergeRange(xealarms);
                    }
                });

                #endregion 3. Use Linq to stitch secondary entities to primary entities

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
                    var events = this.redis.As<VEVENT>().GetAll();
                    var selected = !events.NullOrEmpty()
                        ? events.Skip(skip.Value).Take(take.Value)
                        : null;
                    return (!selected.NullOrEmpty()) ? this.HydrateAll(selected) : new List<VEVENT>();
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public void Save(VEVENT entity)
        {
            try
            {
                var keys = this.redis.As<VEVENT>().GetAllKeys().ToArray();
                if (!keys.NullOrEmpty()) this.redis.Watch(keys);

                #region retrieve attributes of event

                var recur = entity.RecurrenceRule;
                var organizer = entity.Organizer;
                var attendees = entity.Attendees;
                var attachbins = entity.AttachmentBinaries;
                var attachuris = entity.AttachmentUris;
                var contacts = entity.Contacts;
                var comments = entity.Comments;
                var rdates = entity.RecurrenceDates;
                var exdates = entity.ExceptionDates;
                var relatedtos = entity.RelatedTos;
                var resources = entity.Resources;
                var reqstats = entity.RequestStatuses;
                var aalarms = entity.AudioAlarms;
                var dalarms = entity.DisplayAlarms;
                var ealarms = entity.EmailAlarms;
                var ianas = entity.IANAProperties;
                var xprops = entity.XProperties;

                #endregion retrieve attributes of event

                this.manager.ExecTrans(transaction =>
                {
                    var orrecurs = this.redis.As<REL_EVENTS_RECURS>().GetAll().Where(x => x.EventId == entity.Id);
                    var ororgs = this.redis.As<REL_EVENTS_ORGANIZERS>().GetAll().Where(x => x.EventId == entity.Id);
                    var orattendees = this.redis.As<REL_EVENTS_ATTENDEES>().GetAll().Where(x => x.EventId == entity.Id);
                    var orattachbins = this.redis.As<REL_EVENTS_ATTACHBINS>().GetAll().Where(x => x.EventId == entity.Id);
                    var orattachuris = this.redis.As<REL_EVENTS_ATTACHURIS>().GetAll().Where(x => x.EventId == entity.Id);
                    var orcontacts = this.redis.As<REL_EVENTS_CONTACTS>().GetAll().Where(x => x.EventId == entity.Id);
                    var orcomments = this.redis.As<REL_EVENTS_COMMENTS>().GetAll().Where(x => x.EventId == entity.Id);
                    var orrdates = this.redis.As<REL_EVENTS_RDATES>().GetAll().Where(x => x.EventId == entity.Id);
                    var orexdates = this.redis.As<REL_EVENTS_EXDATES>().GetAll().Where(x => x.EventId == entity.Id);
                    var orrelatedtos = this.redis.As<REL_EVENTS_RELATEDTOS>().GetAll().Where(x => x.EventId == entity.Id);
                    var orresources = this.redis.As<REL_EVENTS_RESOURCES>().GetAll().Where(x => x.EventId == entity.Id);
                    var orreqstats = this.redis.As<REL_EVENTS_REQSTATS>().GetAll().Where(x => x.EventId == entity.Id);
                    var oraalarms = this.redis.As<REL_EVENTS_AUDIO_ALARMS>().GetAll().Where(x => x.EventId == entity.Id);
                    var ordalarms = this.redis.As<REL_EVENTS_DISPLAY_ALARMS>().GetAll().Where(x => x.EventId == entity.Id);
                    var orealarms = this.redis.As<REL_EVENTS_EMAIL_ALARMS>().GetAll().Where(x => x.EventId == entity.Id);
                    var orianas = this.redis.As<REL_EVENTS_IANA_PROPERTIES>().GetAll().Where(x => x.EventId == entity.Id);
                    var orxprops = this.redis.As<REL_EVENTS_X_PROPERTIES>().GetAll().Where(x => x.EventId == entity.Id);

                    if (organizer != null)
                    {
                        transaction.QueueCommand(x => x.Store(organizer));
                        var rorganizer = new REL_EVENTS_ORGANIZERS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            OrganizerId = organizer.Id,
                        };

                        this.redis.MergeAll(rorganizer.ToSingleton(), ororgs, transaction);
                    }
                    else this.redis.RemoveAll(ororgs, transaction);

                    if (recur != null)
                    {
                        transaction.QueueCommand(x => x.Store(recur));
                        var rrecur = new REL_EVENTS_RECURS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            RecurId = recur.Id,
                        };

                        this.redis.MergeAll(rrecur.ToSingleton(), orrecurs, transaction);
                    }
                    else this.redis.RemoveAll(orrecurs, transaction);

                    if (!attendees.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(attendees));
                        var rattendees = attendees.Select(x => new REL_EVENTS_ATTENDEES
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            AttendeeId = x.Id,
                        });

                        this.redis.MergeAll(rattendees, orattendees, transaction);
                    }
                    else this.redis.RemoveAll(orattendees, transaction);

                    if (!attachbins.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(attachbins.Distinct()));
                        var rattachbins = attachbins.Select(x => new REL_EVENTS_ATTACHBINS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            AttachmentId = x.Id,
                        });

                        this.redis.MergeAll(rattachbins, orattachbins, transaction);
                    }
                    else this.redis.RemoveAll(orattachbins, transaction);

                    if (!attachuris.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(attachuris.Distinct()));
                        var rattachuris = attachuris.Select(x => new REL_EVENTS_ATTACHURIS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            AttachmentId = x.Id,
                        });

                        this.redis.MergeAll(rattachuris, orattachuris, transaction);
                    }
                    else this.redis.RemoveAll(orattachuris, transaction);

                    if (!contacts.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(contacts.Distinct()));
                        var rcontacts = contacts.Select(x => new REL_EVENTS_CONTACTS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            ContactId = x.Id,
                        });

                        this.redis.MergeAll(rcontacts, orcontacts, transaction);
                    }
                    else this.redis.RemoveAll(orcontacts, transaction);

                    if (!comments.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(comments.Distinct()));
                        var rcomments = comments.Select(x => new REL_EVENTS_COMMENTS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            CommentId = x.Id,
                        });

                        this.redis.MergeAll(rcomments, orcomments, transaction);
                    }
                    else this.redis.RemoveAll(orcomments, transaction);

                    if (!rdates.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(rdates.Distinct()));
                        var rrdates = rdates.Select(x => new REL_EVENTS_RDATES
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            RecurrenceDateId = x.Id,
                        });
                        this.redis.MergeAll(rrdates, orrdates, transaction);
                    }
                    else this.redis.RemoveAll(orrdates, transaction);

                    if (!exdates.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(exdates.Distinct()));
                        var rexdates = exdates.Select(x => new REL_EVENTS_EXDATES
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            ExceptionDateId = x.Id,
                        });

                        this.redis.MergeAll(rexdates, orexdates, transaction);
                    }
                    else this.redis.RemoveAll(oraalarms, transaction);

                    if (!relatedtos.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(relatedtos.Distinct()));
                        var rrelatedtos = relatedtos.Select(x => new REL_EVENTS_RELATEDTOS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            RelatedToId = x.Id,
                        });
                        this.redis.MergeAll(rrelatedtos, orrelatedtos, transaction);
                    }
                    else this.redis.RemoveAll(oraalarms, transaction);
                    if (!resources.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(resources.Distinct()));
                        var rresources = resources.Select(x => new REL_EVENTS_RESOURCES
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            ResourcesId = x.Id,
                        });
                        this.redis.MergeAll(rresources, orresources, transaction);
                    }
                    else this.redis.RemoveAll(oraalarms, transaction);

                    if (!reqstats.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(reqstats.Distinct()));
                        var rreqstats = reqstats.Select(x => new REL_EVENTS_REQSTATS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            ReqStatsId = x.Id,
                        });
                        this.redis.MergeAll(rreqstats, orreqstats, transaction);
                    }
                    else this.redis.RemoveAll(orreqstats, transaction);

                    if (!aalarms.NullOrEmpty())
                    {
                        this.AudioAlarmRepository.SaveAll(aalarms.Distinct());
                        var raalarms = aalarms.Select(x => new REL_EVENTS_AUDIO_ALARMS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            AlarmId = x.Id
                        });
                        this.redis.MergeAll(raalarms, oraalarms, transaction);
                    }
                    else this.redis.RemoveAll(oraalarms, transaction);

                    if (!dalarms.NullOrEmpty())
                    {
                        this.DisplayAlarmRepository.SaveAll(dalarms.Distinct());
                        var rdalarms = dalarms.Select(x => new REL_EVENTS_DISPLAY_ALARMS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            AlarmId = x.Id
                        });
                        this.redis.MergeAll(rdalarms, ordalarms, transaction);
                    }
                    else this.redis.RemoveAll(ordalarms, transaction);

                    if (!ealarms.NullOrEmpty())
                    {
                        this.EmailAlarmRepository.SaveAll(ealarms.Distinct());
                        var realarms = ealarms.Select(x => new REL_EVENTS_EMAIL_ALARMS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            AlarmId = x.Id
                        });
                        this.redis.MergeAll(realarms, orealarms, transaction);
                    }
                    else this.redis.RemoveAll(orealarms, transaction);

                    if (!ianas.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(ianas.Values.Distinct()));
                        var rianas = ianas.Select(x => new REL_EVENTS_IANA_PROPERTIES
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            IanaPropertyId = x.Key
                        });
                        this.redis.MergeAll(rianas, orianas, transaction);
                    }
                    else this.redis.RemoveAll(orianas, transaction);

                    if (!xprops.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(xprops.Values.Distinct()));
                        var rxprops = xprops.Select(x => new REL_EVENTS_X_PROPERTIES
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            XPropertyId = x.Key
                        });

                        this.redis.MergeAll(rxprops, orxprops, transaction);
                    }
                    else this.redis.RemoveAll(orxprops, transaction);

                    transaction.QueueCommand(x => x.Store(this.Dehydrate(entity)));
                });
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public void Erase(string key)
        {
            try
            {
                if (this.redis.As<VEVENT>().ContainsKey(UrnId.Create<VEVENT>(key).ToLower()))
                {
                    var rrecurs = this.redis.As<REL_EVENTS_RECURS>().GetAll();
                    var rorganizers = this.redis.As<REL_EVENTS_ORGANIZERS>().GetAll();
                    var rattachbins = this.redis.As<REL_EVENTS_ATTACHBINS>().GetAll();
                    var rattachuris = this.redis.As<REL_EVENTS_ATTACHURIS>().GetAll();
                    var rattendees = this.redis.As<REL_EVENTS_ATTENDEES>().GetAll();
                    var raalarms = this.redis.As<REL_EVENTS_AUDIO_ALARMS>().GetAll();
                    var rcomments = this.redis.As<REL_EVENTS_COMMENTS>().GetAll();
                    var rcontacts = this.redis.As<REL_EVENTS_CONTACTS>().GetAll();
                    var rdalarms = this.redis.As<REL_EVENTS_DISPLAY_ALARMS>().GetAll();
                    var realarms = this.redis.As<REL_EVENTS_EMAIL_ALARMS>().GetAll();
                    var rexdates = this.redis.As<REL_EVENTS_EXDATES>().GetAll();
                    var rrdates = this.redis.As<REL_EVENTS_RDATES>().GetAll();
                    var rrelatedtos = this.redis.As<REL_EVENTS_RELATEDTOS>().GetAll();
                    var rreqstats = this.redis.As<REL_EVENTS_REQSTATS>().GetAll();
                    var rresources = this.redis.As<REL_EVENTS_RESOURCES>().GetAll();
                    var rianas = this.redis.As<REL_EVENTS_IANA_PROPERTIES>().GetAll();
                    var xprops = this.redis.As<REL_EVENTS_X_PROPERTIES>().GetAll();

                    this.manager.ExecTrans(transaction =>
                    {
                        if (!rrecurs.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_RECURS>()
                            .DeleteByIds(rrecurs.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!rorganizers.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_ORGANIZERS>()
                            .DeleteByIds(rorganizers.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!rattachbins.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_ATTACHBINS>()
                            .DeleteByIds(rattachbins.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!rattachuris.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_ATTACHURIS>()
                            .DeleteByIds(rattachuris.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!rattendees.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_ATTENDEES>()
                            .DeleteByIds(rattendees.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!raalarms.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_AUDIO_ALARMS>()
                            .DeleteByIds(raalarms.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!rcomments.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_COMMENTS>()
                            .DeleteByIds(rcomments.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!rcontacts.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_CONTACTS>()
                            .DeleteByIds(rcontacts.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!rdalarms.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_DISPLAY_ALARMS>()
                            .DeleteByIds(rdalarms.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!realarms.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_EMAIL_ALARMS>()
                            .DeleteByIds(realarms.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!rexdates.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_EXDATES>()
                            .DeleteByIds(rexdates.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!rrdates.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_RDATES>()
                            .DeleteByIds(rrdates.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!rrelatedtos.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_RELATEDTOS>()
                            .DeleteByIds(rrelatedtos.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!rreqstats.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_REQSTATS>()
                            .DeleteByIds(rreqstats.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!rresources.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_RESOURCES>()
                            .DeleteByIds(rresources.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!rianas.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_IANA_PROPERTIES>()
                            .DeleteByIds(rianas.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!xprops.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_X_PROPERTIES>()
                            .DeleteByIds(xprops.Where(x => x.EventId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        transaction.QueueCommand(t => t.As<VEVENT>().DeleteById(key));
                    });
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public void SaveAll(IEnumerable<VEVENT> entities)
        {
            try
            {
                var keys = this.redis.As<VEVENT>().GetAllKeys().ToArray();
                if (!keys.NullOrEmpty()) this.redis.Watch(keys);
                var okeys = entities.Select(x => x.Id);

                #region retrieve attributes of entities

                var organizers = entities.Where(x => x.Organizer != null).Select(x => x.Organizer);
                var recurs = entities.Where(x => x.RecurrenceRule != null).Select(x => x.RecurrenceRule);
                var attendees = entities.Where(x => !x.Attendees.NullOrEmpty()).SelectMany(x => x.Attendees);
                var attachbins = entities.Where(x => !x.AttachmentBinaries.NullOrEmpty()).SelectMany(x => x.AttachmentBinaries);
                var attachuris = entities.Where(x => !x.AttachmentUris.NullOrEmpty()).SelectMany(x => x.AttachmentUris);
                var contacts = entities.Where(x => !x.Contacts.NullOrEmpty()).SelectMany(x => x.Contacts);
                var comments = entities.Where(x => !x.Comments.NullOrEmpty()).SelectMany(x => x.Comments);
                var rdates = entities.Where(x => !x.RecurrenceDates.NullOrEmpty()).SelectMany(x => x.RecurrenceDates);
                var exdates = entities.Where(x => !x.ExceptionDates.NullOrEmpty()).SelectMany(x => x.ExceptionDates);
                var relatedtos = entities.Where(x => !x.RelatedTos.NullOrEmpty()).SelectMany(x => x.RelatedTos);
                var resources = entities.Where(x => !x.Resources.NullOrEmpty()).SelectMany(x => x.Resources);
                var reqstats = entities.Where(x => !x.Resources.NullOrEmpty()).SelectMany(x => x.RequestStatuses);
                var aalarms = entities.Where(x => !x.AudioAlarms.NullOrEmpty()).SelectMany(x => x.AudioAlarms);
                var dalarms = entities.Where(x => !x.DisplayAlarms.NullOrEmpty()).SelectMany(x => x.DisplayAlarms);
                var ealarms = entities.Where(x => !x.EmailAlarms.NullOrEmpty()).SelectMany(x => x.EmailAlarms);
                var ianas = entities.Where(x => !x.IANAProperties.NullOrEmpty()).SelectMany(x => x.IANAProperties.Values);
                var xprops = entities.Where(x => !x.IANAProperties.NullOrEmpty()).SelectMany(x => x.XProperties.Values);

                #endregion retrieve attributes of entities

                #region save event attributes

                this.manager.ExecTrans(transaction =>
                {
                    var ororganizers = this.redis.As<REL_EVENTS_ORGANIZERS>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orrecurs = this.redis.As<REL_EVENTS_RECURS>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orattendees = this.redis.As<REL_EVENTS_ATTENDEES>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orattachbins = this.redis.As<REL_EVENTS_ATTACHBINS>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orattachuris = this.redis.As<REL_EVENTS_ATTACHURIS>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orcontacts = this.redis.As<REL_EVENTS_CONTACTS>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orcomments = this.redis.As<REL_EVENTS_COMMENTS>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orrdates = this.redis.As<REL_EVENTS_RDATES>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orexdates = this.redis.As<REL_EVENTS_EXDATES>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orrelatedtos = this.redis.As<REL_EVENTS_RELATEDTOS>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orresources = this.redis.As<REL_EVENTS_RESOURCES>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orreqstats = this.redis.As<REL_EVENTS_REQSTATS>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var oraalarms = this.redis.As<REL_EVENTS_AUDIO_ALARMS>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var ordalarms = this.redis.As<REL_EVENTS_DISPLAY_ALARMS>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orealarms = this.redis.As<REL_EVENTS_EMAIL_ALARMS>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orianas = this.redis.As<REL_EVENTS_IANA_PROPERTIES>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orxprops = this.redis.As<REL_EVENTS_X_PROPERTIES>().GetAll().Where(x => okeys.Contains(x.EventId));

                    if (!organizers.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(organizers.Distinct()));
                        var rorganizers = entities.Where(x => x.Organizer != null)
                            .Select(x => new REL_EVENTS_ORGANIZERS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x.Id,
                                    OrganizerId = x.Organizer.Id
                                });

                        this.redis.MergeAll(rorganizers, ororganizers, transaction);
                    }
                    else this.redis.RemoveAll(ororganizers, transaction);

                    if (!recurs.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(recurs.Distinct()));
                        var rrecurs = entities.Where(x => x.RecurrenceRule != null)
                            .Select(x => new REL_EVENTS_RECURS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x.Id,
                                RecurId = x.RecurrenceRule.Id
                            });

                        this.redis.MergeAll(rrecurs, orrecurs, transaction);
                    }
                    else this.redis.RemoveAll(orrecurs, transaction);

                    if (!attendees.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(attendees.Distinct()));
                        var rattendees = entities.Where(x => !x.Attendees.NullOrEmpty())
                            .SelectMany(e => e.Attendees
                                .Select(x => new REL_EVENTS_ATTENDEES
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                AttendeeId = x.Id
                            }));

                        this.redis.MergeAll(rattendees, orattendees, transaction);
                    }
                    else this.redis.RemoveAll(orattendees, transaction);

                    if (!attachbins.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(attachbins.Distinct()));
                        var rattachbins = entities.Where(x => !x.AttachmentBinaries.NullOrEmpty()).SelectMany(e => e.AttachmentBinaries
                            .Select(x => new REL_EVENTS_ATTACHBINS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                AttachmentId = x.Id
                            }));
                        this.redis.MergeAll(rattachbins, orattachbins, transaction);
                    }
                    else this.redis.RemoveAll(orattachbins, transaction);

                    if (!attachuris.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(attachuris.Distinct()));
                        var rattachuris = entities.Where(x => !x.AttachmentUris.NullOrEmpty()).SelectMany(e => e.AttachmentUris
                            .Select(x => new REL_EVENTS_ATTACHURIS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                AttachmentId = x.Id
                            }));
                        this.redis.MergeAll(rattachuris, orattachuris, transaction);
                    }
                    else this.redis.RemoveAll(orattachuris, transaction);

                    if (!contacts.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(contacts.Distinct()));
                        var rcontacts = entities.Where(x => !x.Contacts.NullOrEmpty()).SelectMany(e => e.Contacts
                            .Select(x => new REL_EVENTS_CONTACTS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                ContactId = x.Id
                            }));
                        this.redis.MergeAll(rcontacts, orcontacts, transaction);
                    }
                    else this.redis.RemoveAll(orcontacts, transaction);

                    if (!comments.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(comments.Distinct()));
                        var rcomments = entities.Where(x => !x.Comments.NullOrEmpty()).SelectMany(e => e.Comments
                            .Select(x => new REL_EVENTS_COMMENTS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                CommentId = x.Id
                            }));
                        this.redis.MergeAll(rcomments, orcomments, transaction);
                    }
                    else this.redis.RemoveAll(orcomments, transaction);

                    if (!rdates.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(rdates.Distinct()));
                        var rrdates = entities.Where(x => !x.RecurrenceDates.NullOrEmpty()).SelectMany(e => e.RecurrenceDates
                            .Select(x => new REL_EVENTS_RDATES
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                RecurrenceDateId = x.Id
                            }));
                        this.redis.MergeAll(rrdates, orrdates, transaction);
                    }
                    else this.redis.RemoveAll(orrdates, transaction);

                    if (!exdates.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(exdates.Distinct()));
                        var rexdates = entities.Where(x => !x.ExceptionDates.NullOrEmpty()).SelectMany(e => e.ExceptionDates
                            .Select(x => new REL_EVENTS_EXDATES
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                ExceptionDateId = x.Id
                            }));
                        this.redis.MergeAll(rexdates, orexdates, transaction);
                    }
                    else this.redis.RemoveAll(orexdates, transaction);

                    if (!relatedtos.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(relatedtos.Distinct()));
                        var rrelatedtos = entities.Where(x => !x.RelatedTos.NullOrEmpty()).SelectMany(e => e.RelatedTos
                            .Select(x => new REL_EVENTS_RELATEDTOS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                RelatedToId = x.Id
                            }));
                        this.redis.MergeAll(rrelatedtos, orrelatedtos, transaction);
                    }
                    else this.redis.RemoveAll(orrelatedtos, transaction);

                    if (!resources.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(resources.Distinct()));
                        var rresources = entities.Where(x => !x.Resources.NullOrEmpty()).SelectMany(e => e.Resources
                            .Select(x => new REL_EVENTS_RESOURCES
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                ResourcesId = x.Id
                            }));
                        this.redis.MergeAll(rresources, orresources, transaction);
                    }
                    else this.redis.RemoveAll(orresources, transaction);

                    if (!reqstats.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(reqstats.Distinct()));
                        var rreqstats = entities.Where(x => !x.RequestStatuses.NullOrEmpty()).SelectMany(e => e.RequestStatuses
                            .Select(x => new REL_EVENTS_REQSTATS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                ReqStatsId = x.Id
                            }));
                        this.redis.MergeAll(rreqstats, orreqstats, transaction);
                    }
                    else this.redis.RemoveAll(orreqstats, transaction);

                    if (!aalarms.NullOrEmpty())
                    {
                        this.AudioAlarmRepository.SaveAll(aalarms.Distinct());
                        var raalarms = entities.Where(x => !x.AudioAlarms.NullOrEmpty()).SelectMany(e => e.AudioAlarms
                            .Select(x => new REL_EVENTS_AUDIO_ALARMS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                AlarmId = x.Id
                            }));
                        this.redis.MergeAll(raalarms, oraalarms, transaction);
                    }
                    else this.redis.RemoveAll(oraalarms, transaction);

                    if (!dalarms.NullOrEmpty())
                    {
                        this.DisplayAlarmRepository.SaveAll(dalarms.Distinct());
                        var rdalarms = entities.Where(x => !x.DisplayAlarms.NullOrEmpty()).SelectMany(e => e.DisplayAlarms
                            .Select(x => new REL_EVENTS_DISPLAY_ALARMS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                AlarmId = x.Id
                            }));
                        this.redis.MergeAll(rdalarms, ordalarms, transaction);
                    }
                    else this.redis.RemoveAll(ordalarms, transaction);

                    if (!ealarms.NullOrEmpty())
                    {
                        this.EmailAlarmRepository.SaveAll(ealarms.Distinct());
                        var realarms = entities.Where(x => !x.EmailAlarms.NullOrEmpty()).SelectMany(e => e.EmailAlarms
                            .Select(x => new REL_EVENTS_EMAIL_ALARMS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                AlarmId = x.Id
                            }));
                        this.redis.MergeAll(realarms, orealarms, transaction);
                    }
                    else this.redis.RemoveAll(orealarms, transaction);

                    if (!ianas.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(ianas.Distinct()));
                        var rianas = entities.Where(x => !x.IANAProperties.NullOrEmpty()).SelectMany(e => e.IANAProperties
                            .Select(x => new REL_EVENTS_IANA_PROPERTIES
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                IanaPropertyId = x.Key
                            }));
                        this.redis.MergeAll(rianas, orianas, transaction);
                    }
                    else this.redis.RemoveAll(orianas, transaction);

                    if (!xprops.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(xprops.Distinct()));
                        var rxprops = entities.Where(x => !x.XProperties.NullOrEmpty()).SelectMany(e => e.XProperties
                            .Select(x => new REL_EVENTS_X_PROPERTIES
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                XPropertyId = x.Key
                            }));
                        this.redis.MergeAll(rxprops, orxprops, transaction);
                    }
                    else this.redis.RemoveAll(orxprops, transaction);

                    transaction.QueueCommand(x => x.StoreAll(this.DehydrateAll(entities)));
                });

                #endregion save event attributes
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public void EraseAll(IEnumerable<string> keys = null)
        {
            try
            {
                if (!keys.NullOrEmpty())
                {
                    var rrecurs = this.redis.As<REL_EVENTS_RECURS>().GetAll();
                    var rorgs = this.redis.As<REL_EVENTS_ORGANIZERS>().GetAll();
                    var rattachbins = this.redis.As<REL_EVENTS_ATTACHBINS>().GetAll();
                    var rattachuris = this.redis.As<REL_EVENTS_ATTACHURIS>().GetAll();
                    var rattendees = this.redis.As<REL_EVENTS_ATTENDEES>().GetAll();
                    var raalarms = this.redis.As<REL_EVENTS_AUDIO_ALARMS>().GetAll();
                    var rcomments = this.redis.As<REL_EVENTS_COMMENTS>().GetAll();
                    var rcontacts = this.redis.As<REL_EVENTS_CONTACTS>().GetAll();
                    var rdalarms = this.redis.As<REL_EVENTS_DISPLAY_ALARMS>().GetAll();
                    var realarms = this.redis.As<REL_EVENTS_EMAIL_ALARMS>().GetAll();
                    var rexdates = this.redis.As<REL_EVENTS_EXDATES>().GetAll();
                    var rrdates = this.redis.As<REL_EVENTS_RDATES>().GetAll();
                    var rrelatedtos = this.redis.As<REL_EVENTS_RELATEDTOS>().GetAll();
                    var rreqstats = this.redis.As<REL_EVENTS_REQSTATS>().GetAll();
                    var rresources = this.redis.As<REL_EVENTS_RESOURCES>().GetAll();
                    var rianas = this.redis.As<REL_EVENTS_IANA_PROPERTIES>().GetAll();
                    var xprops = this.redis.As<REL_EVENTS_X_PROPERTIES>().GetAll();

                    this.manager.ExecTrans(transaction =>
                    {
                        if (!rorgs.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_ORGANIZERS>()
                            .DeleteByIds(rorgs.Where(x => keys.Contains(x.EventId, StringComparer.OrdinalIgnoreCase))));

                        if (!rrecurs.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_RECURS>()
                            .DeleteByIds(rrecurs.Where(x => keys.Contains(x.EventId, StringComparer.OrdinalIgnoreCase))));

                        if (!rattachbins.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_ATTACHBINS>()
                            .DeleteByIds(rattachbins.Where(x => keys.Contains(x.EventId, StringComparer.OrdinalIgnoreCase))));

                        if (!rattachuris.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_ATTACHURIS>()
                            .DeleteByIds(rattachuris.Where(x => keys.Contains(x.EventId, StringComparer.OrdinalIgnoreCase))));

                        if (!rattendees.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_ATTENDEES>()
                            .DeleteByIds(rattendees.Where(x => keys.Contains(x.EventId, StringComparer.OrdinalIgnoreCase))));

                        if (!raalarms.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_AUDIO_ALARMS>()
                            .DeleteByIds(raalarms.Where(x => keys.Contains(x.EventId, StringComparer.OrdinalIgnoreCase))));

                        if (!rcomments.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_COMMENTS>()
                            .DeleteByIds(rcomments.Where(x => keys.Contains(x.EventId, StringComparer.OrdinalIgnoreCase))));

                        if (!rcontacts.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_CONTACTS>()
                            .DeleteByIds(rcontacts.Where(x => keys.Contains(x.EventId, StringComparer.OrdinalIgnoreCase))));

                        if (!rdalarms.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_DISPLAY_ALARMS>()
                            .DeleteByIds(rdalarms.Where(x => keys.Contains(x.EventId, StringComparer.OrdinalIgnoreCase))));

                        if (!realarms.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_EMAIL_ALARMS>()
                            .DeleteByIds(realarms.Where(x => keys.Contains(x.EventId, StringComparer.OrdinalIgnoreCase))));

                        if (!rexdates.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_EXDATES>()
                            .DeleteByIds(rexdates.Where(x => keys.Contains(x.EventId, StringComparer.OrdinalIgnoreCase))));

                        if (!rrdates.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_RDATES>()
                            .DeleteByIds(rrdates.Where(x => keys.Contains(x.EventId, StringComparer.OrdinalIgnoreCase))));

                        if (!rrelatedtos.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_RELATEDTOS>()
                            .DeleteByIds(rrelatedtos.Where(x => keys.Contains(x.EventId, StringComparer.OrdinalIgnoreCase))));

                        if (!rreqstats.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_REQSTATS>()
                            .DeleteByIds(rreqstats.Where(x => keys.Contains(x.EventId, StringComparer.OrdinalIgnoreCase))));

                        if (!rresources.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_RESOURCES>()
                            .DeleteByIds(rresources.Where(x => keys.Contains(x.EventId, StringComparer.OrdinalIgnoreCase))));

                        if (!rianas.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_IANA_PROPERTIES>()
                            .DeleteByIds(rianas.Where(x => keys.Contains(x.EventId, StringComparer.OrdinalIgnoreCase))));

                        if (!xprops.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_X_PROPERTIES>()
                            .DeleteByIds(xprops.Where(x => keys.Contains(x.EventId, StringComparer.OrdinalIgnoreCase))));

                        transaction.QueueCommand(t => t.As<VEVENT>().DeleteByIds(keys));
                    });
                }
                else
                {
                    this.manager.ExecTrans(transaction =>
                    {
                        transaction.QueueCommand(t => t.As<REL_EVENTS_ORGANIZERS>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_EVENTS_RECURS>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_EVENTS_ATTACHBINS>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_EVENTS_ATTACHURIS>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_EVENTS_ATTENDEES>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_EVENTS_AUDIO_ALARMS>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_EVENTS_COMMENTS>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_EVENTS_CONTACTS>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_EVENTS_DISPLAY_ALARMS>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_EVENTS_EMAIL_ALARMS>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_EVENTS_EXDATES>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_EVENTS_RDATES>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_EVENTS_RELATEDTOS>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_EVENTS_REQSTATS>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_EVENTS_RESOURCES>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_EVENTS_IANA_PROPERTIES>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_EVENTS_X_PROPERTIES>());
                        transaction.QueueCommand(t => t.As<VEVENT>().DeleteAll());
                    });
                }
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
                x.Duration,
                x.Categories,
                x.RecurrenceId,
            };

            Expression<Func<VEVENT, object>> relations = x => new
            {
                x.Organizer,
                x.RecurrenceRule,
                x.Attendees,
                x.AttachmentBinaries,
                x.AttachmentUris,
                x.Contacts,
                x.Comments,
                x.RecurrenceDates,
                x.ExceptionDates,
                x.RelatedTos,
                x.Resources,
                x.RequestStatuses,
                x.AudioAlarms,
                x.DisplayAlarms,
                x.EmailAlarms,
                x.IANAProperties,
                x.XProperties
            };

            //4. Get list of selected relationals
            var srelation = relations.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);

            //5. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);

            #endregion construct anonymous fields using expression lambdas

            try
            {
                var eclient = this.redis.As<VEVENT>();
                if (!keys.NullOrEmpty()) this.redis.Watch(keys.ToArray());

                var entities = !keys.NullOrEmpty() ? eclient.GetByIds(keys).ToList() : eclient.GetAll().ToList();
                if (entities.NullOrEmpty()) return;

                var okeys = entities.Select(x => x.Id).ToArray();

                if (!srelation.NullOrEmpty())
                {
                    Expression<Func<VEVENT, object>> orgexpr = x => x.Organizer;
                    Expression<Func<VEVENT, object>> recurexpr = x => x.RecurrenceRule;
                    Expression<Func<VEVENT, object>> attendsexpr = y => y.Attendees;
                    Expression<Func<VEVENT, object>> attachbinsexpr = y => y.AttachmentBinaries;
                    Expression<Func<VEVENT, object>> attachurisexpr = y => y.AttachmentUris;
                    Expression<Func<VEVENT, object>> contactsexpr = y => y.Contacts;
                    Expression<Func<VEVENT, object>> commentsexpr = y => y.Comments;
                    Expression<Func<VEVENT, object>> rdatesexpr = y => y.RecurrenceDates;
                    Expression<Func<VEVENT, object>> exdatesexpr = y => y.ExceptionDates;
                    Expression<Func<VEVENT, object>> relatedtosexpr = y => y.RelatedTos;
                    Expression<Func<VEVENT, object>> resourcesexpr = y => y.Resources;
                    Expression<Func<VEVENT, object>> reqstatsexpr = y => y.RequestStatuses;
                    Expression<Func<VEVENT, object>> aalarmexpr = y => y.AudioAlarms;
                    Expression<Func<VEVENT, object>> dalarmexpr = y => y.DisplayAlarms;
                    Expression<Func<VEVENT, object>> ealarmexpr = y => y.EmailAlarms;
                    Expression<Func<VEVENT, object>> ianaexpr = y => y.IANAProperties;
                    Expression<Func<VEVENT, object>> xpropsexpr = y => y.XProperties;

                    this.manager.ExecTrans(transaction =>
                    {
                        #region save relational aggregate attributes of entities

                        var ororgs = this.redis.As<REL_EVENTS_ORGANIZERS>().GetAll().Where(x => okeys.Contains(x.EventId));
                        var orrecurs = this.redis.As<REL_EVENTS_RECURS>().GetAll().Where(x => okeys.Contains(x.EventId));
                        var orattendees = this.redis.As<REL_EVENTS_ATTENDEES>().GetAll().Where(x => okeys.Contains(x.EventId));
                        var orattachbins = this.redis.As<REL_EVENTS_ATTACHBINS>().GetAll().Where(x => okeys.Contains(x.EventId));
                        var orattachuris = this.redis.As<REL_EVENTS_ATTACHURIS>().GetAll().Where(x => okeys.Contains(x.EventId));
                        var orcontacts = this.redis.As<REL_EVENTS_CONTACTS>().GetAll().Where(x => okeys.Contains(x.EventId));
                        var orcomments = this.redis.As<REL_EVENTS_COMMENTS>().GetAll().Where(x => okeys.Contains(x.EventId));
                        var orrdates = this.redis.As<REL_EVENTS_RDATES>().GetAll().Where(x => okeys.Contains(x.EventId));
                        var orexdates = this.redis.As<REL_EVENTS_EXDATES>().GetAll().Where(x => okeys.Contains(x.EventId));
                        var orrelatedtos = this.redis.As<REL_EVENTS_RELATEDTOS>().GetAll().Where(x => okeys.Contains(x.EventId));
                        var orresources = this.redis.As<REL_EVENTS_RESOURCES>().GetAll().Where(x => okeys.Contains(x.EventId));
                        var orreqstats = this.redis.As<REL_EVENTS_REQSTATS>().GetAll().Where(x => okeys.Contains(x.EventId));
                        var oraalarms = this.redis.As<REL_EVENTS_AUDIO_ALARMS>().GetAll().Where(x => okeys.Contains(x.EventId));
                        var ordalarms = this.redis.As<REL_EVENTS_DISPLAY_ALARMS>().GetAll().Where(x => okeys.Contains(x.EventId));
                        var orealarms = this.redis.As<REL_EVENTS_EMAIL_ALARMS>().GetAll().Where(x => okeys.Contains(x.EventId));
                        var orianas = this.redis.As<REL_EVENTS_IANA_PROPERTIES>().GetAll().Where(x => okeys.Contains(x.EventId));
                        var orxprops = this.redis.As<REL_EVENTS_X_PROPERTIES>().GetAll().Where(x => okeys.Contains(x.EventId));

                        if (selection.Contains(orgexpr.GetMemberName()))
                        {
                            if (source.Organizer != null)
                            {
                                var rorgs = okeys.Select(x => new REL_EVENTS_ORGANIZERS
                                {
                                    Id = KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    OrganizerId = source.Organizer.Id
                                });

                                this.redis.MergeAll(rorgs, ororgs, transaction);
                                transaction.QueueCommand(x => x.Store(source.Organizer));
                            }
                            else this.redis.RemoveAll(ororgs, transaction);
                        }

                        if (selection.Contains(recurexpr.GetMemberName()))
                        {
                            if (source.RecurrenceRule != null)
                            {
                                var rrecurs = okeys.Select(x => new REL_EVENTS_RECURS
                                {
                                    Id = KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    RecurId = source.RecurrenceRule.Id
                                });

                                this.redis.MergeAll(rrecurs, orrecurs, transaction);
                                transaction.QueueCommand(x => x.Store(source.RecurrenceRule));
                            }
                            else this.redis.RemoveAll(orrecurs, transaction);
                        }

                        if (selection.Contains(attendsexpr.GetMemberName()))
                        {
                            var attendees = source.Attendees;

                            if (!attendees.NullOrEmpty())
                            {
                                var rattendees = okeys.SelectMany(x => attendees.Select(y => new REL_EVENTS_ATTENDEES
                                {
                                    Id = KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    AttendeeId = y.Id
                                }));

                                this.redis.MergeAll(rattendees, orattendees, transaction);
                                transaction.QueueCommand(x => x.StoreAll(attendees.Distinct()));
                            }
                            else this.redis.RemoveAll(orattendees, transaction);
                        }

                        if (selection.Contains(attachbinsexpr.GetMemberName()))
                        {
                            var attachbins = source.AttachmentBinaries;
                            if (!attachbins.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => x.StoreAll(attachbins.Distinct()));
                                var rattachbins = okeys.SelectMany(x => attachbins.Select(y => new REL_EVENTS_ATTACHBINS
                                {
                                    Id = KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    AttachmentId = y.Id
                                }));

                                this.redis.MergeAll(rattachbins, orattachbins, transaction);
                            }
                            else this.redis.RemoveAll(orattachbins, transaction);
                        }
                        if (selection.Contains(attachurisexpr.GetMemberName()))
                        {
                            var attachuris = source.AttachmentUris;
                            if (!attachuris.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => x.StoreAll(attachuris.Distinct()));
                                var rattachuris = okeys.SelectMany(x => attachuris.Select(y => new REL_EVENTS_ATTACHURIS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    AttachmentId = y.Id
                                }));
                                this.redis.MergeAll(rattachuris, orattachuris, transaction);
                            }
                            else this.redis.RemoveAll(orattachuris, transaction);
                        }

                        if (selection.Contains(contactsexpr.GetMemberName()))
                        {
                            var contacts = source.Contacts;
                            if (!contacts.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => x.StoreAll(contacts.Distinct()));
                                var rcontacts = okeys.SelectMany(x => contacts.Select(y => new REL_EVENTS_CONTACTS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    ContactId = y.Id
                                }));
                                this.redis.MergeAll(rcontacts, orcontacts, transaction);
                            }
                            else this.redis.RemoveAll(orcontacts, transaction);
                        }

                        if (selection.Contains(commentsexpr.GetMemberName()))
                        {
                            var comments = source.Contacts;
                            if (!comments.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => x.StoreAll(comments.Distinct()));
                                var rcomments = okeys.SelectMany(x => comments.Select(y => new REL_EVENTS_COMMENTS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    CommentId = y.Id
                                }));
                                this.redis.MergeAll(rcomments, orcomments, transaction);
                            }
                            else this.redis.RemoveAll(orcomments, transaction);
                        }

                        if (selection.Contains(rdatesexpr.GetMemberName()))
                        {
                            var rdates = source.Contacts;
                            if (!rdates.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => x.StoreAll(rdates.Distinct()));
                                var rrdates = okeys.SelectMany(x => rdates.Select(y => new REL_EVENTS_RDATES
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    RecurrenceDateId = y.Id
                                }));
                                this.redis.MergeAll(rrdates, orrdates, transaction);
                            }
                            else this.redis.RemoveAll(orrdates, transaction);
                        }

                        if (selection.Contains(exdatesexpr.GetMemberName()))
                        {
                            var exdates = source.ExceptionDates;
                            if (!exdates.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => x.StoreAll(exdates.Distinct()));
                                var rexdates = okeys.SelectMany(x => exdates.Select(y => new REL_EVENTS_EXDATES
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    ExceptionDateId = y.Id
                                }));
                                this.redis.MergeAll(rexdates, orexdates, transaction);
                            }
                            else this.redis.RemoveAll(orexdates, transaction);
                        }

                        if (selection.Contains(relatedtosexpr.GetMemberName()))
                        {
                            var relatedtos = source.RelatedTos;
                            if (!relatedtos.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => x.StoreAll(relatedtos.Distinct()));
                                var rrelatedtos = okeys.SelectMany(x => relatedtos.Select(y => new REL_EVENTS_RELATEDTOS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    RelatedToId = y.Id
                                }));

                                this.redis.MergeAll(rrelatedtos, orrelatedtos, transaction);
                            }
                            else this.redis.RemoveAll(orrelatedtos, transaction);
                        }

                        if (selection.Contains(resourcesexpr.GetMemberName()))
                        {
                            var resources = source.Resources;
                            if (!resources.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => x.StoreAll(resources.Distinct()));
                                var rresources = okeys.SelectMany(x => resources.Select(y => new REL_EVENTS_RESOURCES
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    ResourcesId = y.Id
                                }));
                                this.redis.MergeAll(rresources, orresources, transaction);
                            }
                            else this.redis.RemoveAll(orresources, transaction);
                        }

                        if (selection.Contains(reqstatsexpr.GetMemberName()))
                        {
                            var reqstats = source.RequestStatuses;
                            if (!reqstats.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => x.StoreAll(reqstats.Distinct()));
                                var rreqstats = okeys.SelectMany(x => reqstats.Select(y => new REL_EVENTS_REQSTATS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    ReqStatsId = y.Id
                                }));

                                this.redis.MergeAll(rreqstats, orreqstats, transaction);
                            }
                            else this.redis.RemoveAll(orreqstats, transaction);
                        }
                        if (selection.Contains(aalarmexpr.GetMemberName()))
                        {
                            var aalarms = source.AudioAlarms;
                            if (!aalarms.NullOrEmpty())
                            {
                                this.AudioAlarmRepository.SaveAll(aalarms.Distinct());
                                var raalarms = okeys.SelectMany(x => aalarms.Select(y => new REL_EVENTS_AUDIO_ALARMS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    AlarmId = y.Id
                                }));
                                this.redis.MergeAll(raalarms, oraalarms, transaction);
                            }
                            else this.redis.RemoveAll(orealarms, transaction);
                        }
                        if (selection.Contains(dalarmexpr.GetMemberName()))
                        {
                            var dalarms = source.DisplayAlarms;
                            if (!dalarms.NullOrEmpty())
                            {
                                this.DisplayAlarmRepository.SaveAll(dalarms.Distinct());
                                var rdalarms = okeys.SelectMany(x => dalarms.Select(y => new REL_EVENTS_DISPLAY_ALARMS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    AlarmId = y.Id
                                }));
                                this.redis.MergeAll(rdalarms, ordalarms, transaction);
                            }
                            else this.redis.RemoveAll(ordalarms, transaction);
                        }
                        if (selection.Contains(ealarmexpr.GetMemberName()))
                        {
                            var ealarms = source.EmailAlarms;
                            if (!ealarms.NullOrEmpty())
                            {
                                this.EmailAlarmRepository.SaveAll(ealarms.Distinct());
                                var realarms = okeys.SelectMany(x => ealarms.Select(y => new REL_EVENTS_EMAIL_ALARMS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    AlarmId = y.Id
                                }));
                                this.redis.MergeAll(realarms, orealarms, transaction);
                            }
                            else this.redis.RemoveAll(orealarms, transaction);
                        }

                        if (selection.Contains(ianaexpr.GetMemberName()))
                        {
                            var ianas = source.IANAProperties.Values;
                            if (!ianas.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => x.StoreAll(ianas.Distinct()));
                                var rianas = okeys.SelectMany(x => source.IANAProperties
                                    .Select(y => new REL_EVENTS_IANA_PROPERTIES
                                    {
                                        Id = this.KeyGenerator.GetNextKey(),
                                        EventId = x,
                                        IanaPropertyId = y.Key
                                    }));

                                this.redis.MergeAll(rianas, orianas, transaction);
                            }
                            else this.redis.RemoveAll(orianas, transaction);
                        }

                        if (selection.Contains(xpropsexpr.GetMemberName()))
                        {
                            var xprops = source.XProperties.Values;
                            if (!xprops.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => x.StoreAll(xprops.Distinct()));
                                var rxprops = okeys.SelectMany(x => source.XProperties
                                    .Select(y => new REL_EVENTS_X_PROPERTIES
                                    {
                                        Id = this.KeyGenerator.GetNextKey(),
                                        EventId = x,
                                        XPropertyId = y.Key
                                    }));
                                this.redis.MergeAll(rxprops, orxprops, transaction);
                            }
                            else this.redis.RemoveAll(orxprops, transaction);
                        }

                        #endregion save relational aggregate attributes of entities
                    });
                }

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
                    Expression<Func<VEVENT, object>> rridexpr = x => x.RecurrenceId;
                    Expression<Func<VEVENT, object>> endexpr = x => x.End;
                    Expression<Func<VEVENT, object>> durexpr = x => x.Duration;
                    Expression<Func<VEVENT, object>> catexpr = x => x.Categories;

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
                        if (selection.Contains(rridexpr.GetMemberName())) x.RecurrenceId = source.RecurrenceId;
                        if (selection.Contains(endexpr.GetMemberName())) x.End = source.End;
                        if (selection.Contains(durexpr.GetMemberName())) x.Duration = source.Duration;
                        if (selection.Contains(catexpr.GetMemberName())) x.Categories = source.Categories;
                    });

                    this.manager.ExecTrans(transaction =>
                    {
                        transaction.QueueCommand(x => x.StoreAll(this.DehydrateAll(entities)));
                    });
                }

                #endregion save (insert or update non-relational attributes
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
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
            if (skip == null && take == null)
            {
                var dry = this.redis.As<VEVENT>().GetByIds(keys);
                return !dry.NullOrEmpty() ? this.HydrateAll(dry) : dry;
            }
            else
            {
                var dry = this.redis.As<VEVENT>().GetByIds(keys).Skip(skip.Value + 1).Take(take.Value);
                return !dry.NullOrEmpty() ? this.HydrateAll(dry) : dry;
            }
        }

        public bool ContainsKey(string key)
        {
            return this.redis.As<VEVENT>().ContainsKey(UrnId.Create<VEVENT>(key).ToLower());
        }

        public bool ContainsKeys(IEnumerable<string> keys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            var matches = this.redis.As<VEVENT>().GetAllKeys().Intersect(keys
                .Select(x => UrnId.Create<VEVENT>(x).ToLower()).ToArray());
            if (matches.NullOrEmpty()) return false;
            return mode == ExpectationMode.pessimistic
                ? matches.Count() == keys.Count()
                : !matches.NullOrEmpty();
        }

        public IEnumerable<string> GetKeys(int? skip = null, int? take = null)
        {
            var keys = this.redis.As<VEVENT>().GetAllKeys();
            if (skip == null && take == null)
                return !keys.NullOrEmpty()
                    ? keys.Select(x => UrnId.GetStringId(x))
                    : new List<string>();
            else
                return (!keys.NullOrEmpty())
                    ? keys.Select(x => UrnId.GetStringId(x)).Skip(skip.Value + 1).Take(take.Value)
                    : new List<string>();
        }

        public IEnumerable<VEVENT> DehydrateAll(IEnumerable<VEVENT> full)
        {
            try
            {
                var pquery = full.AsParallel();
                pquery.ForAll(x => this.Dehydrate(x));
                return pquery.AsEnumerable();
            }
            catch (ArgumentNullException) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (AggregateException) { throw; }
        }

        public VEVENT Dehydrate(VEVENT full)
        {
            var dry = full;
            if (dry.Organizer != null) dry.Organizer = null;
            if (dry.RecurrenceRule != null) dry.Organizer = null;
            if (!dry.Attendees.NullOrEmpty()) dry.Attendees.Clear();
            if (!dry.AttachmentBinaries.NullOrEmpty()) dry.AttachmentBinaries.Clear();
            if (!dry.AttachmentUris.NullOrEmpty()) dry.AttachmentUris.Clear();
            if (!dry.Contacts.NullOrEmpty()) dry.Contacts.Clear();
            if (!dry.Comments.NullOrEmpty()) dry.Comments.Clear();
            if (!dry.RecurrenceDates.NullOrEmpty()) dry.RecurrenceDates.Clear();
            if (!dry.ExceptionDates.NullOrEmpty()) dry.ExceptionDates.Clear();
            if (!dry.RelatedTos.NullOrEmpty()) dry.RelatedTos.Clear();
            if (!dry.RequestStatuses.NullOrEmpty()) dry.RequestStatuses.Clear();
            if (!dry.Resources.NullOrEmpty()) dry.Resources.Clear();
            if (!dry.AudioAlarms.NullOrEmpty()) dry.AudioAlarms.Clear();
            if (!dry.DisplayAlarms.NullOrEmpty()) dry.DisplayAlarms.Clear();
            if (!dry.EmailAlarms.NullOrEmpty()) dry.EmailAlarms.Clear();
            return dry;
        }
    }
}