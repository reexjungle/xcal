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

                var orgs = (!rorgs.Empty())? this.redis.As<ORGANIZER>().GetValues(rorgs.Select(x => x.OrganizerId).ToList()): null;
                var rids = (!rrids.Empty())? this.redis.As<RECURRENCE_ID>().GetValues(rrids.Select(x => x.RecurrenceId_Id).ToList()): null;
                var rrules = (!rrrules.Empty())? this.redis.As<RECUR>().GetValues(rrrules.Select(x => x.RecurrenceRuleId).ToList()): null;
                var attachbins = (!rattachbins.Empty())? this.redis.As<ATTACH_BINARY>().GetValues(rattachbins.Select(x => x.AttachmentId).ToList()): null;
                var attachuris = (!rattachuris.Empty())? this.redis.As<ATTACH_URI>().GetValues(rattachuris.Select(x => x.AttachmentId).ToList()): null;
                var attendees = (!rattendees.Empty())? this.redis.As<ATTENDEE>().GetValues(rattendees.Select(x => x.AttendeeId).ToList()) : null;
                var comments = (!rcomments.Empty())? this.redis.As<COMMENT>().GetValues(rcomments.Select(x => x.CommentId).ToList()): null;
                var contacts = (!rcontacts.Empty())? this.redis.As<CONTACT>().GetValues(rcontacts.Select(x => x.ContactId).ToList()): null;
                var rdates = (!rrdates.Empty())? this.redis.As<RDATE>().GetValues(rrdates.Select(x => x.RecurrenceDateId).ToList()): null;
                var exdates = (!rexdates.Empty())? this.redis.As<EXDATE>().GetValues(rexdates.Select(x => x.ExceptionDateId).ToList()): null;
                var relatedtos = (!rrelatedtos.Empty()) ? this.redis.As<RELATEDTO>().GetValues(rrelatedtos.Select(x => x.RelatedToId).ToList()): null;
                var reqstats = (!rreqstats.Empty()) ? this.redis.As<REQUEST_STATUS>().GetValues(rreqstats.Select(x => x.ReqStatsId).ToList()): null;
                var resources = (!rresources.Empty()) ? this.redis.As<RESOURCES>().GetValues(rresources.Select(x => x.ResourcesId).ToList()): null;
                var aalarms = (!raalarms.Empty()) ? this.AudioAlarmRepository.Find(raalarms.Select(x => x.AlarmId).ToList()): null;
                var dalarms = (!rdalarms.Empty()) ? this.DisplayAlarmRepository.Find(rdalarms.Select(x => x.AlarmId).ToList()): null;
                var ealarms = (!realarms.Empty()) ? this.EmailAlarmRepository.Find(realarms.Select(x => x.AlarmId).ToList()) : null;
                
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
                                    where e.Id == x.Id select y;

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
            }

            return full ?? dry;
        }

        public IEnumerable<VEVENT> Get(int? page = null)
        {
            throw new NotImplementedException();
        }

        public void Save(VEVENT entity)
        {
            throw new NotImplementedException();
        }

        public void Erase(string key)
        {
            throw new NotImplementedException();
        }

        public void SaveAll(IEnumerable<VEVENT> entities)
        {
            throw new NotImplementedException();
        }


        public void EraseAll(IEnumerable<string> keys)
        {
            throw new NotImplementedException();
        }

        public void EraseAll()
        {
            throw new NotImplementedException();
        }

        public IKeyGenerator<string> KeyGenerator { get; set; }

        public void Patch(VEVENT entity, Expression<Func<VEVENT, object>> fields, Expression<Func<VEVENT, bool>> where = null)
        {
            throw new NotImplementedException();
        }


        public VEVENT Find(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VEVENT> Find(IEnumerable<string> keys, int? skip = null)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKeys(IEnumerable<string> keys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetKeys(int? skip = null)
        {
            throw new NotImplementedException();
        }
    }
}
