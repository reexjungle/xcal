using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.repositories.concretes.relations;
using reexjungle.xcal.service.repositories.contracts;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.infrastructure.contracts;
using reexjungle.xmisc.technical.data.concretes.nosql;
using ServiceStack.Common;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace reexjungle.xcal.service.repositories.concretes.redis
{
    public class EventRedisRepository : IEventRepository, IRedisRepository
    {
        private readonly IKeyGenerator<Guid> keygenerator;
        private readonly IAudioAlarmRepository aalarmrepository;
        private readonly IDisplayAlarmRepository dalarmrepository;
        private readonly IEmailAlarmRepository ealarmrepository;
        private readonly IRedisClientsManager manager;

        private IRedisClient client;

        public EventRedisRepository(
            IKeyGenerator<Guid> keygenerator,
            IAudioAlarmRepository aalarmrepository,
            IDisplayAlarmRepository dalarmrepository,
            IEmailAlarmRepository ealarmrepository,
            IRedisClientsManager manager)
        {
            this.keygenerator = keygenerator;
            this.ealarmrepository = ealarmrepository;
            this.dalarmrepository = dalarmrepository;
            this.aalarmrepository = aalarmrepository;
            this.manager = manager;
        }

        private IRedisClient redis => client ?? (client = manager.GetClient());

        public IRedisClientsManager RedisClientsManager => manager;

        public VEVENT Hydrate(VEVENT dry)
        {
            if (dry != null)
            {
                var rrecurs = redis.As<REL_EVENTS_RECURS>().GetAll().Where(x => x.EventId == dry.Id).ToList();
                if (!rrecurs.NullOrEmpty())
                {
                    dry.RecurrenceRule = redis.As<RECUR>().GetById(rrecurs.FirstOrDefault().RecurId);
                }

                var rorgs = redis.As<REL_EVENTS_ORGANIZERS>().GetAll().Where(x => x.EventId == dry.Id);
                if (!rorgs.NullOrEmpty())
                {
                    dry.Organizer = redis.As<ORGANIZER>().GetById(rorgs.FirstOrDefault().OrganizerId);
                }

                var rattachbins = redis.As<REL_EVENTS_ATTACHBINS>().GetAll().Where(x => x.EventId == dry.Id);
                if (!rattachbins.NullOrEmpty())
                {
                    dry.Attachments.MergeRange(redis.As<ATTACH_BINARY>().GetByIds(rattachbins.Select(x => x.AttachmentId).ToList()));
                }

                var rattachuris = redis.As<REL_EVENTS_ATTACHURIS>().GetAll().Where(x => x.EventId == dry.Id);
                if (!rattachuris.NullOrEmpty())
                {
                    dry.Attachments.MergeRange(redis.As<ATTACH_URI>().GetByIds(rattachuris.Select(x => x.AttachmentId).ToList()));
                }

                var rattendees = redis.As<REL_EVENTS_ATTENDEES>().GetAll().Where(x => x.EventId == dry.Id);
                if (!rattendees.NullOrEmpty())
                {
                    dry.Attendees.MergeRange(redis.As<ATTENDEE>().GetByIds(rattendees.Select(x => x.AttendeeId).ToList()));
                }

                var rcomments = redis.As<REL_EVENTS_COMMENTS>().GetAll().Where(x => x.EventId == dry.Id);
                if (!rcomments.NullOrEmpty())
                {
                    dry.Comments.MergeRange(redis.As<COMMENT>().GetByIds(rcomments.Select(x => x.CommentId).ToList()));
                }

                var rcontacts = redis.As<REL_EVENTS_CONTACTS>().GetAll().Where(x => x.EventId == dry.Id);
                if (!rcontacts.NullOrEmpty())
                {
                    dry.Contacts.MergeRange(redis.As<CONTACT>().GetByIds(rcontacts.Select(x => x.ContactId).ToList()));
                }

                var rrdates = redis.As<REL_EVENTS_RDATES>().GetAll().Where(x => x.EventId == dry.Id);
                if (!rrdates.NullOrEmpty())
                {
                    dry.RecurrenceDates.MergeRange(redis.As<RDATE>().GetByIds(rrdates.Select(x => x.RecurrenceDateId).ToList()));
                }

                var rexdates = redis.As<REL_EVENTS_EXDATES>().GetAll().Where(x => x.EventId == dry.Id);
                if (!rexdates.NullOrEmpty())
                {
                    dry.ExceptionDates.MergeRange(redis.As<EXDATE>().GetByIds(rexdates.Select(x => x.ExceptionDateId).ToList()));
                }

                var rrelatedtos = redis.As<REL_EVENTS_RELATEDTOS>().GetAll().Where(x => x.EventId == dry.Id);
                if (!rrelatedtos.NullOrEmpty())
                {
                    dry.RelatedTos.MergeRange(redis.As<RELATEDTO>().GetByIds(rrelatedtos.Select(x => x.RelatedToId).ToList()));
                }

                var rreqstats = redis.As<REL_EVENTS_REQSTATS>().GetAll().Where(x => x.EventId == dry.Id);
                if (!rreqstats.NullOrEmpty())
                {
                    dry.RequestStatuses.MergeRange(redis.As<REQUEST_STATUS>().GetByIds(rreqstats.Select(x => x.ReqStatsId).ToList()));
                }

                var rresources = redis.As<REL_EVENTS_RESOURCES>().GetAll().Where(x => x.EventId == dry.Id);
                if (!rresources.NullOrEmpty())
                {
                    dry.Resources.MergeRange(redis.As<RESOURCES>().GetByIds(rresources.Select(x => x.ResourcesId).ToList()));
                }

                var raalarms = redis.As<REL_EVENTS_AUDIO_ALARMS>().GetAll().Where(x => x.EventId == dry.Id);
                if (!raalarms.NullOrEmpty())
                {
                    dry.Alarms.MergeRange(aalarmrepository.FindAll(raalarms.Select(x => x.AlarmId).ToList()));
                }

                var rdalarms = redis.As<REL_EVENTS_DISPLAY_ALARMS>().GetAll().Where(x => x.EventId == dry.Id);
                if (!rdalarms.NullOrEmpty())
                {
                    dry.Alarms.MergeRange(dalarmrepository.FindAll(rdalarms.Select(x => x.AlarmId).ToList()));
                }

                var realarms = redis.As<REL_EVENTS_EMAIL_ALARMS>().GetAll().Where(x => x.EventId == dry.Id);
                if (!realarms.NullOrEmpty())
                {
                    dry.Alarms.MergeRange(ealarmrepository
                        .HydrateAll(ealarmrepository.FindAll(realarms.Select(x => x.AlarmId).ToList())));
                }

                //TODO: retrieve IANA and x- properties of events during hydration
            }

            return dry ?? dry;
        }

        public IEnumerable<VEVENT> HydrateAll(IEnumerable<VEVENT> dry)
        {
            var full = dry.ToList();
            var keys = full.Select(x => x.Id).ToList();
            var okeys = redis.GetAllKeys().Intersect(keys.Select(x => x.ToString())).ToArray();

            #region 1. retrieve relationships

            var rrecurs = redis.As<REL_EVENTS_RECURS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rorgs = redis.As<REL_EVENTS_ORGANIZERS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rattachbins = redis.As<REL_EVENTS_ATTACHBINS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rattachuris = redis.As<REL_EVENTS_ATTACHURIS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rattendees = redis.As<REL_EVENTS_ATTENDEES>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rcomments = redis.As<REL_EVENTS_COMMENTS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rcontacts = redis.As<REL_EVENTS_CONTACTS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rrdates = redis.As<REL_EVENTS_RDATES>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rexdates = redis.As<REL_EVENTS_EXDATES>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rrelatedtos = redis.As<REL_EVENTS_RELATEDTOS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rreqstats = redis.As<REL_EVENTS_REQSTATS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rresources = redis.As<REL_EVENTS_RESOURCES>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var raalarms = redis.As<REL_EVENTS_AUDIO_ALARMS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rdalarms = redis.As<REL_EVENTS_DISPLAY_ALARMS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var realarms = redis.As<REL_EVENTS_EMAIL_ALARMS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();

            //TODO: retrieve IANA and x- properties of events during hydration

            #endregion 1. retrieve relationships

            #region 2. retrieve secondary entities

            var recurs = (!rrecurs.Empty()) ? redis.As<RECUR>().GetByIds(rrecurs.Select(x => x.RecurId)) : null;
            var organizers = (!rorgs.Empty()) ? redis.As<ORGANIZER>().GetByIds(rorgs.Select(x => x.OrganizerId)) : null;
            var attachbins = (!rattachbins.Empty()) ? redis.As<ATTACH_BINARY>().GetByIds(rattachbins.Select(x => x.AttachmentId)) : null;
            var attachuris = (!rattachuris.Empty()) ? redis.As<ATTACH_URI>().GetByIds(rattachuris.Select(x => x.AttachmentId)) : null;
            var attendees = (!rattendees.Empty()) ? redis.As<ATTENDEE>().GetByIds(rattendees.Select(x => x.AttendeeId)) : null;
            var comments = (!rcomments.Empty()) ? redis.As<COMMENT>().GetByIds(rcomments.Select(x => x.CommentId)) : null;
            var contacts = (!rcontacts.Empty()) ? redis.As<CONTACT>().GetByIds(rcontacts.Select(x => x.ContactId)) : null;
            var rdates = (!rrdates.Empty()) ? redis.As<RDATE>().GetByIds(rrdates.Select(x => x.RecurrenceDateId)) : null;
            var exdates = (!rexdates.Empty()) ? redis.As<EXDATE>().GetByIds(rexdates.Select(x => x.ExceptionDateId)) : null;
            var relatedtos = (!rrelatedtos.Empty()) ? redis.As<RELATEDTO>().GetByIds(rrelatedtos.Select(x => x.RelatedToId)) : null;
            var reqstats = (!rreqstats.Empty()) ? redis.As<REQUEST_STATUS>().GetByIds(rreqstats.Select(x => x.ReqStatsId)) : null;
            var resources = (!rresources.Empty()) ? redis.As<RESOURCES>().GetByIds(rresources.Select(x => x.ResourcesId)) : null;
            var aalarms = (!raalarms.Empty()) ? aalarmrepository.FindAll(raalarms.Select(x => x.AlarmId)) : null;
            var dalarms = (!rdalarms.Empty()) ? dalarmrepository.FindAll(rdalarms.Select(x => x.AlarmId)) : null;
            var ealarms = (!realarms.Empty()) ? ealarmrepository.HydrateAll(ealarmrepository.FindAll(realarms.Select(x => x.AlarmId))) : null;

            #endregion 2. retrieve secondary entities

            #region 3. Use Linq to stitch secondary entities to primary entities

            full.ForEach(x =>
            {
                if (!recurs.NullOrEmpty())
                {
                    var xrecurs = from y in recurs
                                  join r in rrecurs on y.Id equals r.RecurId
                                  join e in full on r.EventId equals e.Id
                                  where e.Id.Equals(x.Id)
                                  select y;
                    if (!xrecurs.NullOrEmpty()) x.RecurrenceRule = xrecurs.First();
                }

                if (!organizers.NullOrEmpty())
                {
                    var xorgs = from y in organizers
                                join r in rorgs on y.Id equals r.OrganizerId
                                join e in full on r.EventId equals e.Id
                                where e.Id.Equals(x.Id)
                                select y;
                    if (!xorgs.NullOrEmpty()) x.Organizer = xorgs.First();
                }

                if (!comments.NullOrEmpty())
                {
                    var xcomments = from y in comments
                                    join r in rcomments on y.Id equals r.CommentId
                                    join e in full on r.EventId equals e.Id
                                    where e.Id.Equals(x.Id)
                                    select y;
                    if (!xcomments.NullOrEmpty()) x.Comments.MergeRange(xcomments);
                }

                if (!attendees.NullOrEmpty())
                {
                    var xattendees = from y in attendees
                                     join r in rattendees on y.Id equals r.AttendeeId
                                     join e in full on r.EventId equals e.Id
                                     where e.Id.Equals(x.Id)
                                     select y;
                    if (!xattendees.NullOrEmpty()) x.Attendees.MergeRange(xattendees);
                }

                if (!attachbins.NullOrEmpty())
                {
                    var xattachbins = from y in attachbins
                                      join r in rattachbins on y.Id equals r.AttachmentId
                                      join e in full on r.EventId equals e.Id
                                      where e.Id.Equals(x.Id)
                                      select y;
                    if (!xattachbins.NullOrEmpty()) x.Attachments.MergeRange(xattachbins);
                }

                if (!attachuris.NullOrEmpty())
                {
                    var xattachuris = from y in attachuris
                                      join r in rattachuris on y.Id equals r.AttachmentId
                                      join e in full on r.EventId equals e.Id
                                      where e.Id.Equals(x.Id)
                                      select y;
                    if (!xattachuris.NullOrEmpty()) x.Attachments.MergeRange(xattachuris);
                }

                if (!contacts.NullOrEmpty())
                {
                    var xcontacts = from y in contacts
                                    join r in rcontacts on y.Id equals r.ContactId
                                    join e in full on r.EventId equals e.Id
                                    where e.Id.Equals(x.Id)
                                    select y;

                    if (!xcontacts.NullOrEmpty()) x.Contacts.MergeRange(xcontacts);
                }

                if (!rdates.NullOrEmpty())
                {
                    var xdates = from y in rdates
                                 join r in rrdates on y.Id equals r.RecurrenceDateId
                                 join e in full on r.EventId equals e.Id
                                 where e.Id.Equals(x.Id)
                                 select y;
                    if (!xdates.NullOrEmpty()) x.RecurrenceDates.MergeRange(xdates);
                }

                if (!exdates.NullOrEmpty())
                {
                    var xexdates = from y in exdates
                                   join r in rexdates on y.Id equals r.ExceptionDateId
                                   join e in full on r.EventId equals e.Id
                                   where e.Id.Equals(x.Id)
                                   select y;
                    if (!xexdates.NullOrEmpty()) x.ExceptionDates.MergeRange(xexdates);
                }

                if (!relatedtos.NullOrEmpty())
                {
                    var xelatedtos = from y in relatedtos
                                     join r in rrelatedtos on y.Id equals r.RelatedToId
                                     join e in full on r.EventId equals e.Id
                                     where e.Id.Equals(x.Id)
                                     select y;
                    if (!xelatedtos.NullOrEmpty()) x.RelatedTos.MergeRange(xelatedtos);
                }

                if (!reqstats.NullOrEmpty())
                {
                    var xeqstats = from y in reqstats
                                   join r in rreqstats on y.Id equals r.ReqStatsId
                                   join e in full on r.EventId equals e.Id
                                   where e.Id.Equals(x.Id)
                                   select y;
                    if (!xeqstats.NullOrEmpty()) x.RequestStatuses.MergeRange(xeqstats);
                }

                if (!resources.NullOrEmpty())
                {
                    var xesources = from y in resources
                                    join r in rresources on y.Id equals r.ResourcesId
                                    join e in full on r.EventId equals e.Id
                                    where e.Id.Equals(x.Id)
                                    select y;
                    if (!xesources.NullOrEmpty()) x.Resources.MergeRange(xesources);
                }

                if (!aalarms.NullOrEmpty())
                {
                    var xaalarms = from y in aalarms
                                   join r in raalarms on y.Id equals r.AlarmId
                                   join e in full on r.EventId equals e.Id
                                   where e.Id.Equals(x.Id)
                                   select y;
                    if (!xaalarms.NullOrEmpty()) x.Alarms.MergeRange(xaalarms);
                }

                if (!dalarms.NullOrEmpty())
                {
                    var xdalarms = from y in dalarms
                                   join r in rdalarms on y.Id equals r.AlarmId
                                   join e in full on r.EventId equals e.Id
                                   where e.Id.Equals(x.Id)
                                   select y;
                    if (!xdalarms.NullOrEmpty()) x.Alarms.MergeRange(xdalarms);
                }

                if (!ealarms.NullOrEmpty())
                {
                    var xealarms = from y in ealarms
                                   join r in realarms on y.Id equals r.AlarmId
                                   join e in full on r.EventId equals e.Id
                                   where e.Id.Equals(x.Id)
                                   select y;
                    if (!xealarms.NullOrEmpty()) x.Alarms.MergeRange(xealarms);
                }
            });

            #endregion 3. Use Linq to stitch secondary entities to primary entities

            return full ?? dry;
        }

        public IEnumerable<VEVENT> Get(int? skip = null, int? take = null)
        {
            if (skip == null || take == null) return redis.As<VEVENT>().GetAll();
            else
            {
                var events = redis.As<VEVENT>().GetAll();
                var selected = !events.NullOrEmpty()
                    ? events.Skip(skip.Value).Take(take.Value)
                    : null;
                return (!selected.NullOrEmpty()) ? HydrateAll(selected) : new List<VEVENT>();
            }
        }

        public void Save(VEVENT entity)
        {
            var keys = redis.As<VEVENT>().GetAllKeys().ToArray();
            var guids = keys.Select(x => new Guid(x));
            if (!keys.NullOrEmpty())
            {
                redis.Watch(keys);
            }

            #region retrieve attributes of event

            var recur = entity.RecurrenceRule;
            var organizer = entity.Organizer;
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

            #endregion retrieve attributes of event

            manager.ExecTrans(transaction =>
            {
                var orrecurs = redis.As<REL_EVENTS_RECURS>().GetAll().Where(x => x.EventId == entity.Id);
                var ororgs = redis.As<REL_EVENTS_ORGANIZERS>().GetAll().Where(x => x.EventId == entity.Id);
                var orattendees = redis.As<REL_EVENTS_ATTENDEES>().GetAll().Where(x => x.EventId == entity.Id);
                var orattachbins = redis.As<REL_EVENTS_ATTACHBINS>().GetAll().Where(x => x.EventId == entity.Id);
                var orattachuris = redis.As<REL_EVENTS_ATTACHURIS>().GetAll().Where(x => x.EventId == entity.Id);
                var orcontacts = redis.As<REL_EVENTS_CONTACTS>().GetAll().Where(x => x.EventId == entity.Id);
                var orcomments = redis.As<REL_EVENTS_COMMENTS>().GetAll().Where(x => x.EventId == entity.Id);
                var orrdates = redis.As<REL_EVENTS_RDATES>().GetAll().Where(x => x.EventId == entity.Id);
                var orexdates = redis.As<REL_EVENTS_EXDATES>().GetAll().Where(x => x.EventId == entity.Id);
                var orrelatedtos = redis.As<REL_EVENTS_RELATEDTOS>().GetAll().Where(x => x.EventId == entity.Id);
                var orresources = redis.As<REL_EVENTS_RESOURCES>().GetAll().Where(x => x.EventId == entity.Id);
                var orreqstats = redis.As<REL_EVENTS_REQSTATS>().GetAll().Where(x => x.EventId == entity.Id);
                var oraalarms = redis.As<REL_EVENTS_AUDIO_ALARMS>().GetAll().Where(x => x.EventId == entity.Id);
                var ordalarms = redis.As<REL_EVENTS_DISPLAY_ALARMS>().GetAll().Where(x => x.EventId == entity.Id);
                var orealarms = redis.As<REL_EVENTS_EMAIL_ALARMS>().GetAll().Where(x => x.EventId == entity.Id);
                if (organizer != null)
                {
                    transaction.QueueCommand(x => x.Store(organizer));
                    var rorganizer = new REL_EVENTS_ORGANIZERS
                    {
                        Id = keygenerator.GetNext(),
                        EventId = entity.Id,
                        OrganizerId = organizer.Id,
                    };

                    redis.MergeAll(rorganizer.ToSingleton(), ororgs, transaction);
                }
                else redis.RemoveAll(ororgs, transaction);

                if (recur != null)
                {
                    transaction.QueueCommand(x => x.Store(recur));
                    var rrecur = new REL_EVENTS_RECURS
                    {
                        Id = keygenerator.GetNext(),
                        EventId = entity.Id,
                        RecurId = recur.Id,
                    };

                    redis.MergeAll(rrecur.ToSingleton(), orrecurs, transaction);
                }
                else redis.RemoveAll(orrecurs, transaction);

                if (!attendees.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(attendees));
                    var rattendees = attendees.Select(x => new REL_EVENTS_ATTENDEES
                    {
                        Id = keygenerator.GetNext(),
                        EventId = entity.Id,
                        AttendeeId = x.Id,
                    });

                    redis.MergeAll(rattendees, orattendees, transaction);
                }
                else redis.RemoveAll(orattendees, transaction);

                if (!attachbins.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(attachbins.Distinct()));
                    var rattachbins = attachbins.Select(x => new REL_EVENTS_ATTACHBINS
                    {
                        Id = keygenerator.GetNext(),
                        EventId = entity.Id,
                        AttachmentId = x.Id,
                    });

                    redis.MergeAll(rattachbins, orattachbins, transaction);
                }
                else redis.RemoveAll(orattachbins, transaction);

                if (!attachuris.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(attachuris.Distinct()));
                    var rattachuris = attachuris.Select(x => new REL_EVENTS_ATTACHURIS
                    {
                        Id = keygenerator.GetNext(),
                        EventId = entity.Id,
                        AttachmentId = x.Id,
                    });

                    redis.MergeAll(rattachuris, orattachuris, transaction);
                }
                else redis.RemoveAll(orattachuris, transaction);

                if (!contacts.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(contacts.Distinct()));
                    var rcontacts = contacts.Select(x => new REL_EVENTS_CONTACTS
                    {
                        Id = keygenerator.GetNext(),
                        EventId = entity.Id,
                        ContactId = x.Id,
                    });

                    redis.MergeAll(rcontacts, orcontacts, transaction);
                }
                else redis.RemoveAll(orcontacts, transaction);

                if (!comments.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(comments.Distinct()));
                    var rcomments = comments.Select(x => new REL_EVENTS_COMMENTS
                    {
                        Id = keygenerator.GetNext(),
                        EventId = entity.Id,
                        CommentId = x.Id,
                    });

                    redis.MergeAll(rcomments, orcomments, transaction);
                }
                else redis.RemoveAll(orcomments, transaction);

                if (!rdates.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(rdates.Distinct()));
                    var rrdates = rdates.Select(x => new REL_EVENTS_RDATES
                    {
                        Id = keygenerator.GetNext(),
                        EventId = entity.Id,
                        RecurrenceDateId = x.Id,
                    });
                    redis.MergeAll(rrdates, orrdates, transaction);
                }
                else redis.RemoveAll(orrdates, transaction);

                if (!exdates.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(exdates.Distinct()));
                    var rexdates = exdates.Select(x => new REL_EVENTS_EXDATES
                    {
                        Id = keygenerator.GetNext(),
                        EventId = entity.Id,
                        ExceptionDateId = x.Id,
                    });

                    redis.MergeAll(rexdates, orexdates, transaction);
                }
                else redis.RemoveAll(oraalarms, transaction);

                if (!relatedtos.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(relatedtos.Distinct()));
                    var rrelatedtos = relatedtos.Select(x => new REL_EVENTS_RELATEDTOS
                    {
                        Id = keygenerator.GetNext(),
                        EventId = entity.Id,
                        RelatedToId = x.Id,
                    });
                    redis.MergeAll(rrelatedtos, orrelatedtos, transaction);
                }
                else redis.RemoveAll(oraalarms, transaction);
                if (!resources.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(resources.Distinct()));
                    var rresources = resources.Select(x => new REL_EVENTS_RESOURCES
                    {
                        Id = keygenerator.GetNext(),
                        EventId = entity.Id,
                        ResourcesId = x.Id,
                    });
                    redis.MergeAll(rresources, orresources, transaction);
                }
                else redis.RemoveAll(oraalarms, transaction);

                if (!reqstats.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(reqstats.Distinct()));
                    var rreqstats = reqstats.Select(x => new REL_EVENTS_REQSTATS
                    {
                        Id = keygenerator.GetNext(),
                        EventId = entity.Id,
                        ReqStatsId = x.Id,
                    });
                    redis.MergeAll(rreqstats, orreqstats, transaction);
                }
                else redis.RemoveAll(orreqstats, transaction);

                if (!aalarms.NullOrEmpty())
                {
                    aalarmrepository.SaveAll(aalarms.Distinct());
                    var raalarms = aalarms.Select(x => new REL_EVENTS_AUDIO_ALARMS
                    {
                        Id = keygenerator.GetNext(),
                        EventId = entity.Id,
                        AlarmId = x.Id
                    });
                    redis.MergeAll(raalarms, oraalarms, transaction);
                }
                else redis.RemoveAll(oraalarms, transaction);

                if (!dalarms.NullOrEmpty())
                {
                    dalarmrepository.SaveAll(dalarms.Distinct());
                    var rdalarms = dalarms.Select(x => new REL_EVENTS_DISPLAY_ALARMS
                    {
                        Id = keygenerator.GetNext(),
                        EventId = entity.Id,
                        AlarmId = x.Id
                    });
                    redis.MergeAll(rdalarms, ordalarms, transaction);
                }
                else redis.RemoveAll(ordalarms, transaction);

                if (!ealarms.NullOrEmpty())
                {
                    ealarmrepository.SaveAll(ealarms.Distinct());
                    var realarms = ealarms.Select(x => new REL_EVENTS_EMAIL_ALARMS
                    {
                        Id = keygenerator.GetNext(),
                        EventId = entity.Id,
                        AlarmId = x.Id
                    });
                    redis.MergeAll(realarms, orealarms, transaction);
                }
                else redis.RemoveAll(orealarms, transaction);

                transaction.QueueCommand(x => x.Store(Dehydrate(entity)));
            });
        }

        public void Erase(Guid key)
        {
            if (redis.As<VEVENT>().ContainsKey(UrnId.Create<VEVENT>(key).ToLower()))
            {
                var rrecurs = redis.As<REL_EVENTS_RECURS>().GetAll();
                var rorganizers = redis.As<REL_EVENTS_ORGANIZERS>().GetAll();
                var rattachbins = redis.As<REL_EVENTS_ATTACHBINS>().GetAll();
                var rattachuris = redis.As<REL_EVENTS_ATTACHURIS>().GetAll();
                var rattendees = redis.As<REL_EVENTS_ATTENDEES>().GetAll();
                var raalarms = redis.As<REL_EVENTS_AUDIO_ALARMS>().GetAll();
                var rcomments = redis.As<REL_EVENTS_COMMENTS>().GetAll();
                var rcontacts = redis.As<REL_EVENTS_CONTACTS>().GetAll();
                var rdalarms = redis.As<REL_EVENTS_DISPLAY_ALARMS>().GetAll();
                var realarms = redis.As<REL_EVENTS_EMAIL_ALARMS>().GetAll();
                var rexdates = redis.As<REL_EVENTS_EXDATES>().GetAll();
                var rrdates = redis.As<REL_EVENTS_RDATES>().GetAll();
                var rrelatedtos = redis.As<REL_EVENTS_RELATEDTOS>().GetAll();
                var rreqstats = redis.As<REL_EVENTS_REQSTATS>().GetAll();
                var rresources = redis.As<REL_EVENTS_RESOURCES>().GetAll();

                manager.ExecTrans(transaction =>
                {
                    if (!rrecurs.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_RECURS>()
                        .DeleteByIds(rrecurs.Where(x => x.EventId == key)));

                    if (!rorganizers.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_ORGANIZERS>()
                        .DeleteByIds(rorganizers.Where(x => x.EventId == key)));

                    if (!rattachbins.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_ATTACHBINS>()
                        .DeleteByIds(rattachbins.Where(x => x.EventId == key)));

                    if (!rattachuris.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_ATTACHURIS>()
                        .DeleteByIds(rattachuris.Where(x => x.EventId == key)));

                    if (!rattendees.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_ATTENDEES>()
                        .DeleteByIds(rattendees.Where(x => x.EventId == key)));

                    if (!raalarms.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_AUDIO_ALARMS>()
                        .DeleteByIds(raalarms.Where(x => x.EventId == key)));

                    if (!rcomments.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_COMMENTS>()
                        .DeleteByIds(rcomments.Where(x => x.EventId == key)));

                    if (!rcontacts.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_CONTACTS>()
                        .DeleteByIds(rcontacts.Where(x => x.EventId == key)));

                    if (!rdalarms.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_DISPLAY_ALARMS>()
                        .DeleteByIds(rdalarms.Where(x => x.EventId == key)));

                    if (!realarms.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_EMAIL_ALARMS>()
                        .DeleteByIds(realarms.Where(x => x.EventId == key)));

                    if (!rexdates.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_EXDATES>()
                        .DeleteByIds(rexdates.Where(x => x.EventId == key)));

                    if (!rrdates.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_RDATES>()
                        .DeleteByIds(rrdates.Where(x => x.EventId == key)));

                    if (!rrelatedtos.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_RELATEDTOS>()
                        .DeleteByIds(rrelatedtos.Where(x => x.EventId == key)));

                    if (!rreqstats.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_REQSTATS>()
                        .DeleteByIds(rreqstats.Where(x => x.EventId == key)));

                    if (!rresources.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_RESOURCES>()
                        .DeleteByIds(rresources.Where(x => x.EventId == key)));

                    transaction.QueueCommand(t => t.As<VEVENT>().DeleteById(key));
                });
            }
        }

        public void SaveAll(IEnumerable<VEVENT> entities)
        {
            var keys = redis.As<VEVENT>().GetAllKeys().ToArray();
            if (!keys.NullOrEmpty()) redis.Watch(keys);
            var okeys = entities.Select(x => x.Id);

            #region retrieve attributes of entities

            var organizers = entities.Where(x => x.Organizer != null).Select(x => x.Organizer);
            var recurs = entities.Where(x => x.RecurrenceRule != null).Select(x => x.RecurrenceRule);
            var attendees = entities.SelectMany(x => x.Attendees);
            var attachbins = entities.SelectMany(x => x.Attachments).OfType<ATTACH_BINARY>();
            var attachuris = entities.SelectMany(x => x.Attachments).OfType<ATTACH_URI>();
            var contacts = entities.SelectMany(x => x.Contacts);
            var comments = entities.SelectMany(x => x.Comments);
            var rdates = entities.SelectMany(x => x.RecurrenceDates);
            var exdates = entities.SelectMany(x => x.ExceptionDates);
            var relatedtos = entities.SelectMany(x => x.RelatedTos);
            var resources = entities.SelectMany(x => x.Resources);
            var reqstats = entities.SelectMany(x => x.RequestStatuses);
            var aalarms = entities.SelectMany(x => x.Alarms.OfType<AUDIO_ALARM>());
            var dalarms = entities.SelectMany(x => x.Alarms.OfType<DISPLAY_ALARM>());
            var ealarms = entities.SelectMany(x => x.Alarms.OfType<EMAIL_ALARM>());

            #endregion retrieve attributes of entities

            #region save event attributes

            manager.ExecTrans(transaction =>
            {
                var ororganizers = redis.As<REL_EVENTS_ORGANIZERS>().GetAll().Where(x => okeys.Contains(x.EventId));
                var orrecurs = redis.As<REL_EVENTS_RECURS>().GetAll().Where(x => okeys.Contains(x.EventId));
                var orattendees = redis.As<REL_EVENTS_ATTENDEES>().GetAll().Where(x => okeys.Contains(x.EventId));
                var orattachbins = redis.As<REL_EVENTS_ATTACHBINS>().GetAll().Where(x => okeys.Contains(x.EventId));
                var orattachuris = redis.As<REL_EVENTS_ATTACHURIS>().GetAll().Where(x => okeys.Contains(x.EventId));
                var orcontacts = redis.As<REL_EVENTS_CONTACTS>().GetAll().Where(x => okeys.Contains(x.EventId));
                var orcomments = redis.As<REL_EVENTS_COMMENTS>().GetAll().Where(x => okeys.Contains(x.EventId));
                var orrdates = redis.As<REL_EVENTS_RDATES>().GetAll().Where(x => okeys.Contains(x.EventId));
                var orexdates = redis.As<REL_EVENTS_EXDATES>().GetAll().Where(x => okeys.Contains(x.EventId));
                var orrelatedtos = redis.As<REL_EVENTS_RELATEDTOS>().GetAll().Where(x => okeys.Contains(x.EventId));
                var orresources = redis.As<REL_EVENTS_RESOURCES>().GetAll().Where(x => okeys.Contains(x.EventId));
                var orreqstats = redis.As<REL_EVENTS_REQSTATS>().GetAll().Where(x => okeys.Contains(x.EventId));
                var oraalarms = redis.As<REL_EVENTS_AUDIO_ALARMS>().GetAll().Where(x => okeys.Contains(x.EventId));
                var ordalarms = redis.As<REL_EVENTS_DISPLAY_ALARMS>().GetAll().Where(x => okeys.Contains(x.EventId));
                var orealarms = redis.As<REL_EVENTS_EMAIL_ALARMS>().GetAll().Where(x => okeys.Contains(x.EventId));

                if (!organizers.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(organizers.Distinct()));
                    var rorganizers = entities.Where(x => x.Organizer != null)
                        .Select(x => new REL_EVENTS_ORGANIZERS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = x.Id,
                            OrganizerId = x.Organizer.Id
                        });

                    redis.MergeAll(rorganizers, ororganizers, transaction);
                }
                else redis.RemoveAll(ororganizers, transaction);

                if (!recurs.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(recurs.Distinct()));
                    var rrecurs = entities.Where(x => x.RecurrenceRule != null)
                        .Select(x => new REL_EVENTS_RECURS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = x.Id,
                            RecurId = x.RecurrenceRule.Id
                        });

                    redis.MergeAll(rrecurs, orrecurs, transaction);
                }
                else redis.RemoveAll(orrecurs, transaction);

                if (!attendees.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(attendees.Distinct()));
                    var rattendees = entities.Where(x => !x.Attendees.NullOrEmpty())
                        .SelectMany(e => e.Attendees
                            .Select(x => new REL_EVENTS_ATTENDEES
                            {
                                Id = keygenerator.GetNext(),
                                EventId = e.Id,
                                AttendeeId = x.Id
                            }));

                    redis.MergeAll(rattendees, orattendees, transaction);
                }
                else redis.RemoveAll(orattendees, transaction);

                if (!attachbins.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(attachbins));
                    var rattachbins = entities.SelectMany(e => attachbins
                        .Select(x => new REL_EVENTS_ATTACHBINS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = e.Id,
                            AttachmentId = x.Id
                        }));
                    redis.MergeAll(rattachbins, orattachbins, transaction);
                }
                else redis.RemoveAll(orattachbins, transaction);

                if (!attachuris.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(attachuris));
                    var rattachuris = entities.SelectMany(e => attachuris
                        .Select(x => new REL_EVENTS_ATTACHURIS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = e.Id,
                            AttachmentId = x.Id
                        }));
                    redis.MergeAll(rattachuris, orattachuris, transaction);
                }
                else redis.RemoveAll(orattachuris, transaction);

                if (!contacts.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(contacts.Distinct()));
                    var rcontacts = entities.Where(x => !x.Contacts.NullOrEmpty()).SelectMany(e => e.Contacts
                        .Select(x => new REL_EVENTS_CONTACTS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = e.Id,
                            ContactId = x.Id
                        }));
                    redis.MergeAll(rcontacts, orcontacts, transaction);
                }
                else redis.RemoveAll(orcontacts, transaction);

                if (!comments.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(comments.Distinct()));
                    var rcomments = entities.Where(x => !x.Comments.NullOrEmpty()).SelectMany(e => e.Comments
                        .Select(x => new REL_EVENTS_COMMENTS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = e.Id,
                            CommentId = x.Id
                        }));
                    redis.MergeAll(rcomments, orcomments, transaction);
                }
                else redis.RemoveAll(orcomments, transaction);

                if (!rdates.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(rdates.Distinct()));
                    var rrdates = entities.Where(x => !x.RecurrenceDates.NullOrEmpty()).SelectMany(e => e.RecurrenceDates
                        .Select(x => new REL_EVENTS_RDATES
                        {
                            Id = keygenerator.GetNext(),
                            EventId = e.Id,
                            RecurrenceDateId = x.Id
                        }));
                    redis.MergeAll(rrdates, orrdates, transaction);
                }
                else redis.RemoveAll(orrdates, transaction);

                if (!exdates.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(exdates.Distinct()));
                    var rexdates = entities.Where(x => !x.ExceptionDates.NullOrEmpty()).SelectMany(e => e.ExceptionDates
                        .Select(x => new REL_EVENTS_EXDATES
                        {
                            Id = keygenerator.GetNext(),
                            EventId = e.Id,
                            ExceptionDateId = x.Id
                        }));
                    redis.MergeAll(rexdates, orexdates, transaction);
                }
                else redis.RemoveAll(orexdates, transaction);

                if (!relatedtos.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(relatedtos.Distinct()));
                    var rrelatedtos = entities.Where(x => !x.RelatedTos.NullOrEmpty()).SelectMany(e => e.RelatedTos
                        .Select(x => new REL_EVENTS_RELATEDTOS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = e.Id,
                            RelatedToId = x.Id
                        }));
                    redis.MergeAll(rrelatedtos, orrelatedtos, transaction);
                }
                else redis.RemoveAll(orrelatedtos, transaction);

                if (!resources.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(resources.Distinct()));
                    var rresources = entities.Where(x => !x.Resources.NullOrEmpty()).SelectMany(e => e.Resources
                        .Select(x => new REL_EVENTS_RESOURCES
                        {
                            Id = keygenerator.GetNext(),
                            EventId = e.Id,
                            ResourcesId = x.Id
                        }));
                    redis.MergeAll(rresources, orresources, transaction);
                }
                else redis.RemoveAll(orresources, transaction);

                if (!reqstats.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(reqstats.Distinct()));
                    var rreqstats = entities.Where(x => !x.RequestStatuses.NullOrEmpty()).SelectMany(e => e.RequestStatuses
                        .Select(x => new REL_EVENTS_REQSTATS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = e.Id,
                            ReqStatsId = x.Id
                        }));
                    redis.MergeAll(rreqstats, orreqstats, transaction);
                }
                else redis.RemoveAll(orreqstats, transaction);

                if (!aalarms.NullOrEmpty())
                {
                    aalarmrepository.SaveAll(aalarms.Distinct());
                    var raalarms = entities.SelectMany(e => aalarms
                        .Select(x => new REL_EVENTS_AUDIO_ALARMS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = e.Id,
                            AlarmId = x.Id
                        }));
                    redis.MergeAll(raalarms, oraalarms, transaction);
                }
                else redis.RemoveAll(oraalarms, transaction);

                if (!dalarms.NullOrEmpty())
                {
                    dalarmrepository.SaveAll(dalarms.Distinct());
                    var rdalarms = entities.SelectMany(e => dalarms
                        .Select(x => new REL_EVENTS_DISPLAY_ALARMS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = e.Id,
                            AlarmId = x.Id
                        }));
                    redis.MergeAll(rdalarms, ordalarms, transaction);
                }
                else redis.RemoveAll(ordalarms, transaction);

                if (!ealarms.NullOrEmpty())
                {
                    ealarmrepository.SaveAll(ealarms.Distinct());
                    var realarms = entities.SelectMany(e => aalarms
                        .Select(x => new REL_EVENTS_EMAIL_ALARMS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = e.Id,
                            AlarmId = x.Id
                        }));
                    redis.MergeAll(realarms, orealarms, transaction);
                }
                else redis.RemoveAll(orealarms, transaction);

                transaction.QueueCommand(x => x.StoreAll(DehydrateAll(entities)));
            });

            #endregion save event attributes
        }

        public void EraseAll(IEnumerable<Guid> keys = null)
        {
            if (!keys.NullOrEmpty())
            {
                var rrecurs = redis.As<REL_EVENTS_RECURS>().GetAll();
                var rorgs = redis.As<REL_EVENTS_ORGANIZERS>().GetAll();
                var rattachbins = redis.As<REL_EVENTS_ATTACHBINS>().GetAll();
                var rattachuris = redis.As<REL_EVENTS_ATTACHURIS>().GetAll();
                var rattendees = redis.As<REL_EVENTS_ATTENDEES>().GetAll();
                var raalarms = redis.As<REL_EVENTS_AUDIO_ALARMS>().GetAll();
                var rcomments = redis.As<REL_EVENTS_COMMENTS>().GetAll();
                var rcontacts = redis.As<REL_EVENTS_CONTACTS>().GetAll();
                var rdalarms = redis.As<REL_EVENTS_DISPLAY_ALARMS>().GetAll();
                var realarms = redis.As<REL_EVENTS_EMAIL_ALARMS>().GetAll();
                var rexdates = redis.As<REL_EVENTS_EXDATES>().GetAll();
                var rrdates = redis.As<REL_EVENTS_RDATES>().GetAll();
                var rrelatedtos = redis.As<REL_EVENTS_RELATEDTOS>().GetAll();
                var rreqstats = redis.As<REL_EVENTS_REQSTATS>().GetAll();
                var rresources = redis.As<REL_EVENTS_RESOURCES>().GetAll();

                manager.ExecTrans(transaction =>
                {
                    if (!rorgs.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_ORGANIZERS>()
                        .DeleteByIds(rorgs.Where(x => keys.Contains(x.EventId))));

                    if (!rrecurs.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_RECURS>()
                        .DeleteByIds(rrecurs.Where(x => keys.Contains(x.EventId))));

                    if (!rattachbins.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_ATTACHBINS>()
                        .DeleteByIds(rattachbins.Where(x => keys.Contains(x.EventId))));

                    if (!rattachuris.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_ATTACHURIS>()
                        .DeleteByIds(rattachuris.Where(x => keys.Contains(x.EventId))));

                    if (!rattendees.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_ATTENDEES>()
                        .DeleteByIds(rattendees.Where(x => keys.Contains(x.EventId))));

                    if (!raalarms.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_AUDIO_ALARMS>()
                        .DeleteByIds(raalarms.Where(x => keys.Contains(x.EventId))));

                    if (!rcomments.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_COMMENTS>()
                        .DeleteByIds(rcomments.Where(x => keys.Contains(x.EventId))));

                    if (!rcontacts.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_CONTACTS>()
                        .DeleteByIds(rcontacts.Where(x => keys.Contains(x.EventId))));

                    if (!rdalarms.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_DISPLAY_ALARMS>()
                        .DeleteByIds(rdalarms.Where(x => keys.Contains(x.EventId))));

                    if (!realarms.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_EMAIL_ALARMS>()
                        .DeleteByIds(realarms.Where(x => keys.Contains(x.EventId))));

                    if (!rexdates.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_EXDATES>()
                        .DeleteByIds(rexdates.Where(x => keys.Contains(x.EventId))));

                    if (!rrdates.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_RDATES>()
                        .DeleteByIds(rrdates.Where(x => keys.Contains(x.EventId))));

                    if (!rrelatedtos.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_RELATEDTOS>()
                        .DeleteByIds(rrelatedtos.Where(x => keys.Contains(x.EventId))));

                    if (!rreqstats.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_REQSTATS>()
                        .DeleteByIds(rreqstats.Where(x => keys.Contains(x.EventId))));

                    if (!rresources.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EVENTS_RESOURCES>()
                        .DeleteByIds(rresources.Where(x => keys.Contains(x.EventId))));

                    transaction.QueueCommand(t => t.As<VEVENT>().DeleteByIds(keys));
                });
            }
            else
            {
                manager.ExecTrans(transaction =>
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
                    transaction.QueueCommand(t => t.As<VEVENT>().DeleteAll());
                });
            }
        }

        public void Patch(VEVENT source, IEnumerable<string> fields, IEnumerable<Guid> keys = null)
        {
            #region construct anonymous fields using expression lambdas

            var selection = fields as IList<string> ?? fields.ToList();

            Expression<Func<VEVENT, object>> primitives = x => new
            {
                x.Uid,
                x.Start,
                x.Created,
                x.Description,
                Position = x.GeoPosition,
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
            var srelation = relations.GetMemberNames().Intersect(selection);

            //5. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection);

            #endregion construct anonymous fields using expression lambdas

            var eclient = redis.As<VEVENT>();
            if (!keys.NullOrEmpty()) redis.Watch(keys.Select(x => x.ToString()).ToArray());

            var entities = !keys.NullOrEmpty() ? eclient.GetByIds(keys).ToList() : eclient.GetAll().ToList();
            if (entities.NullOrEmpty()) return;

            var okeys = entities.Select(x => x.Id).ToArray();

            if (!srelation.NullOrEmpty())
            {
                Expression<Func<VEVENT, object>> orgexpr = x => x.Organizer;
                Expression<Func<VEVENT, object>> recurexpr = x => x.RecurrenceRule;
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

                manager.ExecTrans(transaction =>
                {
                    #region save relational aggregate attributes of entities

                    var ororgs = redis.As<REL_EVENTS_ORGANIZERS>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orrecurs = redis.As<REL_EVENTS_RECURS>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orattendees = redis.As<REL_EVENTS_ATTENDEES>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orattachbins = redis.As<REL_EVENTS_ATTACHBINS>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orattachuris = redis.As<REL_EVENTS_ATTACHURIS>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orcontacts = redis.As<REL_EVENTS_CONTACTS>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orcomments = redis.As<REL_EVENTS_COMMENTS>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orrdates = redis.As<REL_EVENTS_RDATES>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orexdates = redis.As<REL_EVENTS_EXDATES>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orrelatedtos = redis.As<REL_EVENTS_RELATEDTOS>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orresources = redis.As<REL_EVENTS_RESOURCES>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orreqstats = redis.As<REL_EVENTS_REQSTATS>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var oraalarms = redis.As<REL_EVENTS_AUDIO_ALARMS>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var ordalarms = redis.As<REL_EVENTS_DISPLAY_ALARMS>().GetAll().Where(x => okeys.Contains(x.EventId));
                    var orealarms = redis.As<REL_EVENTS_EMAIL_ALARMS>().GetAll().Where(x => okeys.Contains(x.EventId));

                    if (selection.Contains(orgexpr.GetMemberName()))
                    {
                        if (source.Organizer != null)
                        {
                            var rorgs = okeys.Select(x => new REL_EVENTS_ORGANIZERS
                            {
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                OrganizerId = source.Organizer.Id
                            });

                            redis.MergeAll(rorgs, ororgs, transaction);
                            transaction.QueueCommand(x => x.Store(source.Organizer));
                        }
                        else redis.RemoveAll(ororgs, transaction);
                    }

                    if (selection.Contains(recurexpr.GetMemberName()))
                    {
                        if (source.RecurrenceRule != null)
                        {
                            var rrecurs = okeys.Select(x => new REL_EVENTS_RECURS
                            {
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                RecurId = source.RecurrenceRule.Id
                            });

                            redis.MergeAll(rrecurs, orrecurs, transaction);
                            transaction.QueueCommand(x => x.Store(source.RecurrenceRule));
                        }
                        else redis.RemoveAll(orrecurs, transaction);
                    }

                    if (selection.Contains(attendsexpr.GetMemberName()))
                    {
                        var attendees = source.Attendees;

                        if (!attendees.NullOrEmpty())
                        {
                            var rattendees = okeys.SelectMany(x => attendees.Select(y => new REL_EVENTS_ATTENDEES
                            {
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                AttendeeId = y.Id
                            }));

                            redis.MergeAll(rattendees, orattendees, transaction);
                            transaction.QueueCommand(x => x.StoreAll(attendees.Distinct()));
                        }
                        else redis.RemoveAll(orattendees, transaction);
                    }

                    if (selection.Contains(attachsexpr.GetMemberName()))
                    {
                        var attachbins = source.Attachments.OfType<ATTACH_BINARY>();
                        if (attachbins.Any())
                        {
                            transaction.QueueCommand(x => x.StoreAll(attachbins));
                            var rattachbins = okeys.SelectMany(x => attachbins.Select(y => new REL_EVENTS_ATTACHBINS
                            {
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                AttachmentId = y.Id
                            }));

                            redis.MergeAll(rattachbins, orattachbins, transaction);
                        }
                        else redis.RemoveAll(orattachbins, transaction);

                        var attachuris = source.Attachments.OfType<ATTACH_URI>();
                        if (!attachuris.NullOrEmpty())
                        {
                            transaction.QueueCommand(x => x.StoreAll(attachuris));
                            var rattachuris = okeys.SelectMany(x => attachuris.Select(y => new REL_EVENTS_ATTACHURIS
                            {
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                AttachmentId = y.Id
                            }));
                            redis.MergeAll(rattachuris, orattachuris, transaction);
                        }
                        else redis.RemoveAll(orattachuris, transaction);
                    }

                    if (selection.Contains(contactsexpr.GetMemberName()))
                    {
                        var contacts = source.Contacts;
                        if (!contacts.NullOrEmpty())
                        {
                            transaction.QueueCommand(x => x.StoreAll(contacts.Distinct()));
                            var rcontacts = okeys.SelectMany(x => contacts.Select(y => new REL_EVENTS_CONTACTS
                            {
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                ContactId = y.Id
                            }));
                            redis.MergeAll(rcontacts, orcontacts, transaction);
                        }
                        else redis.RemoveAll(orcontacts, transaction);
                    }

                    if (selection.Contains(commentsexpr.GetMemberName()))
                    {
                        var comments = source.Contacts;
                        if (!comments.NullOrEmpty())
                        {
                            transaction.QueueCommand(x => x.StoreAll(comments.Distinct()));
                            var rcomments = okeys.SelectMany(x => comments.Select(y => new REL_EVENTS_COMMENTS
                            {
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                CommentId = y.Id
                            }));
                            redis.MergeAll(rcomments, orcomments, transaction);
                        }
                        else redis.RemoveAll(orcomments, transaction);
                    }

                    if (selection.Contains(rdatesexpr.GetMemberName()))
                    {
                        var rdates = source.Contacts;
                        if (!rdates.NullOrEmpty())
                        {
                            transaction.QueueCommand(x => x.StoreAll(rdates.Distinct()));
                            var rrdates = okeys.SelectMany(x => rdates.Select(y => new REL_EVENTS_RDATES
                            {
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                RecurrenceDateId = y.Id
                            }));
                            redis.MergeAll(rrdates, orrdates, transaction);
                        }
                        else redis.RemoveAll(orrdates, transaction);
                    }

                    if (selection.Contains(exdatesexpr.GetMemberName()))
                    {
                        var exdates = source.ExceptionDates;
                        if (!exdates.NullOrEmpty())
                        {
                            transaction.QueueCommand(x => x.StoreAll(exdates.Distinct()));
                            var rexdates = okeys.SelectMany(x => exdates.Select(y => new REL_EVENTS_EXDATES
                            {
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                ExceptionDateId = y.Id
                            }));
                            redis.MergeAll(rexdates, orexdates, transaction);
                        }
                        else redis.RemoveAll(orexdates, transaction);
                    }

                    if (selection.Contains(relatedtosexpr.GetMemberName()))
                    {
                        var relatedtos = source.RelatedTos;
                        if (!relatedtos.NullOrEmpty())
                        {
                            transaction.QueueCommand(x => x.StoreAll(relatedtos.Distinct()));
                            var rrelatedtos = okeys.SelectMany(x => relatedtos.Select(y => new REL_EVENTS_RELATEDTOS
                            {
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                RelatedToId = y.Id
                            }));

                            redis.MergeAll(rrelatedtos, orrelatedtos, transaction);
                        }
                        else redis.RemoveAll(orrelatedtos, transaction);
                    }

                    if (selection.Contains(resourcesexpr.GetMemberName()))
                    {
                        var resources = source.Resources;
                        if (!resources.NullOrEmpty())
                        {
                            transaction.QueueCommand(x => x.StoreAll(resources.Distinct()));
                            var rresources = okeys.SelectMany(x => resources.Select(y => new REL_EVENTS_RESOURCES
                            {
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                ResourcesId = y.Id
                            }));
                            redis.MergeAll(rresources, orresources, transaction);
                        }
                        else redis.RemoveAll(orresources, transaction);
                    }

                    if (selection.Contains(reqstatsexpr.GetMemberName()))
                    {
                        var reqstats = source.RequestStatuses;
                        if (!reqstats.NullOrEmpty())
                        {
                            transaction.QueueCommand(x => x.StoreAll(reqstats.Distinct()));
                            var rreqstats = okeys.SelectMany(x => reqstats.Select(y => new REL_EVENTS_REQSTATS
                            {
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                ReqStatsId = y.Id
                            }));

                            redis.MergeAll(rreqstats, orreqstats, transaction);
                        }
                        else redis.RemoveAll(orreqstats, transaction);
                    }
                    if (selection.Contains(alarmexpr.GetMemberName()))
                    {
                        var aalarms = source.Alarms.OfType<AUDIO_ALARM>();
                        if (aalarms.Any())
                        {
                            aalarmrepository.SaveAll(aalarms);
                            var raalarms = okeys.SelectMany(x => aalarms.Select(y => new REL_EVENTS_AUDIO_ALARMS
                            {
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                AlarmId = y.Id
                            }));
                            redis.MergeAll(raalarms, oraalarms, transaction);
                        }
                        else redis.RemoveAll(orealarms, transaction);

                        var dalarms = source.Alarms.OfType<DISPLAY_ALARM>();
                        if (dalarms.Any())
                        {
                            dalarmrepository.SaveAll(dalarms);
                            var rdalarms = okeys.SelectMany(x => dalarms.Select(y => new REL_EVENTS_DISPLAY_ALARMS
                            {
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                AlarmId = y.Id
                            }));
                            redis.MergeAll(rdalarms, ordalarms, transaction);
                        }
                        else redis.RemoveAll(ordalarms, transaction);

                        var ealarms = source.Alarms.OfType<EMAIL_ALARM>();
                        if (ealarms.Any())
                        {
                            ealarmrepository.SaveAll(ealarms);
                            var realarms = okeys.SelectMany(x => ealarms.Select(y => new REL_EVENTS_EMAIL_ALARMS
                            {
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                AlarmId = y.Id
                            }));
                            redis.MergeAll(realarms, orealarms, transaction);
                        }
                        else redis.RemoveAll(orealarms, transaction);
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
                Expression<Func<VEVENT, object>> posexpr = x => x.GeoPosition;
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
                    if (selection.Contains(posexpr.GetMemberName())) x.GeoPosition = source.GeoPosition;
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

                manager.ExecTrans(transaction =>
                {
                    transaction.QueueCommand(x => x.StoreAll(DehydrateAll(entities)));
                });
            }

            #endregion save (insert or update non-relational attributes
        }

        public VEVENT Find(Guid key)
        {
            var dry = redis.As<VEVENT>().GetById(key);
            return dry != null ? Hydrate(dry) : dry;
        }

        public IEnumerable<VEVENT> FindAll(IEnumerable<Guid> keys, int? skip = null, int? take = null)
        {
            if (skip == null && take == null)
            {
                var dry = redis.As<VEVENT>().GetByIds(keys);
                return !dry.NullOrEmpty() ? HydrateAll(dry) : dry;
            }
            else
            {
                var dry = redis.As<VEVENT>().GetByIds(keys).Skip(skip.Value + 1).Take(take.Value);
                return !dry.NullOrEmpty() ? HydrateAll(dry) : dry;
            }
        }

        public bool ContainsKey(Guid key)
        {
            return redis.As<VEVENT>().ContainsKey(UrnId.Create<VEVENT>(key).ToLower());
        }

        public bool ContainsKeys(IEnumerable<Guid> keys, ExpectationMode mode = ExpectationMode.Optimistic)
        {
            var matches = redis.As<VEVENT>().GetAllKeys().Intersect(keys
                .Select(x => UrnId.Create<VEVENT>(x).ToLower())).ToList();
            if (matches.NullOrEmpty()) return false;
            return mode == ExpectationMode.Pessimistic
                ? matches.Count() == keys.Count()
                : !matches.NullOrEmpty();
        }

        public IEnumerable<Guid> GetKeys(int? skip = null, int? take = null)
        {
            var keys = redis.As<VEVENT>().GetAllKeys();
            if (skip == null && take == null)
                return !keys.NullOrEmpty()
                    ? keys.Select(UrnId.GetGuidId)
                    : new List<Guid>();
            return (!keys.NullOrEmpty())
                ? keys.Select(UrnId.GetGuidId).Skip(skip.Value + 1).Take(take.Value)
                : new List<Guid>();
        }

        public IEnumerable<VEVENT> DehydrateAll(IEnumerable<VEVENT> full)
        {
            var pquery = full.AsParallel();
            pquery.ForAll(x => Dehydrate(x));
            return pquery.AsEnumerable();
        }

        public VEVENT Dehydrate(VEVENT @event)
        {
            @event.Organizer = null;
            @event.Organizer = null;
            @event.Attendees.Clear();
            @event.Attachments.Clear();
             @event.Contacts.Clear();
             @event.Comments.Clear();
            @event.RecurrenceDates.Clear();
             @event.ExceptionDates.Clear();
             @event.RelatedTos.Clear();
            @event.RequestStatuses.Clear();
             @event.Resources.Clear();
             @event.Alarms.Clear();
            return @event;
        }
    }
}