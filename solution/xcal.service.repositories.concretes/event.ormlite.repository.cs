using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using ServiceStack.OrmLite;
using reexmonkey.technical.data.contracts;
using reexmonkey.technical.data.concretes.extensions.ormlite;
using reexmonkey.crosscut.goodies.concretes;
using reexmonkey.crosscut.essentials.contracts;
using reexmonkey.crosscut.essentials.concretes;
using reexmonkey.crosscut.io.concretes;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.service.repositories.contracts;

namespace reexmonkey.xcal.service.repositories.concretes
{
    /// <summary>
    /// Reüpresents a a repository of events connected to an ORMlite source
    /// </summary>
    public class EventOrmLiteRepository: IEventOrmLiteRepository
    {
        private IDbConnection conn = null;
        private IDbConnectionFactory factory = null;
        private int? pages = null;

        private IDbConnection db
        {
            get { return (this.conn) ?? factory.OpenDbConnection(); }
        }
        public IDbConnectionFactory DbConnectionFactory
        {
            get { return this.factory; }
            set 
            {
                if (value == null) throw new ArgumentNullException("Null factory");
                this.factory = value; 
            }
        }
        public int? Pages
        {
            get { return this.pages; }
            set 
            {
                if (value == null) throw new ArgumentNullException("Null pages");
                this.pages = value; 
            }
        }
        public IKeyGenerator<string> KeyGenerator { get; set; }


        private IAudioAlarmRepository aalarmrepository = null;
        private IDisplayAlarmRepository dalarmrepository = null;
        private IEmailAlarmRepository ealarmrepository = null;

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

        public EventOrmLiteRepository() { }
        public EventOrmLiteRepository(IDbConnectionFactory factory, int? pages)
        {
            this.DbConnectionFactory = factory;
            this.Pages = pages;
            this.conn = this.factory.OpenDbConnection();
        }
        public EventOrmLiteRepository(IDbConnection connection, int? pages)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            this.conn = connection; 
            this.Pages = pages;
        }

        //cleanup
        public virtual void Dispose()
        {
            if (this.conn != null) this.conn.Dispose();
        }

        public VEVENT Find(string fkey, string pkey)
        {
            VEVENT dry = null;
            try
            {
                dry = db.Select<VEVENT, VCALENDAR, REL_CALENDARS_EVENTS>(
                    r => r.Uid,
                    e => e.Uid == pkey,
                    r => r.ProdId,
                    c => c.ProdId == fkey).FirstOrDefault();
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
            return (dry != null) ? this.Hydrate(dry) : null;
        }

        public IEnumerable<VEVENT> Find(IEnumerable<string> keys, int? page = null)
        {
            IEnumerable<VEVENT> dry = null;
            try
            {
                dry = db.Select<VEVENT>(q => Sql.In(q.Uid, keys.ToArray()), page, pages);
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
            return (!dry.NullOrEmpty()) ? this.Hydrate(dry) : null;
        }

        public IEnumerable<VEVENT> Find(IEnumerable<string> fkeys, IEnumerable<string> pkeys = null, int? page = null)
        {
            IEnumerable<VEVENT> dry = null;
            try
            {
                if (!pkeys.NullOrEmpty())
                {
                    dry = db.Select<VEVENT, VCALENDAR, REL_CALENDARS_EVENTS>(
                    r => r.Uid,
                    e => Sql.In(e.Uid, pkeys.ToArray()),
                    r => r.ProdId,
                    c => Sql.In(c.ProdId, fkeys.ToArray()),
                    Conjunctor.AND, JoinMode.INNER, true, page, pages); 
                }
                else
                {
                    dry = db.Select<VEVENT, VCALENDAR, REL_CALENDARS_EVENTS>(
                    r => r.Uid,
                    r => r.ProdId,
                    c => Sql.In(c.ProdId, fkeys.ToArray()),
                    JoinMode.INNER, true, page, pages); 
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
            return (!dry.NullOrEmpty()) ? this.Hydrate(dry) : null;
        }

        public IEnumerable<VEVENT> Get(int? page = null)
        {
            IEnumerable<VEVENT> dry = null;
            try
            {
                dry = db.Select<VEVENT>(page, pages);
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }

            return (!dry.NullOrEmpty()) ? this.Hydrate(dry) : null;
        }

        public void Save(VEVENT entity)
        {
            try
            {
                //Save dry event entity i.e. without related details
                db.Save(entity);

                //1. retrieve entity details
                var org = (entity.Organizer != null && entity.Organizer is ORGANIZER) ? (entity.Organizer as ORGANIZER) : null;
                var rid = (entity.RecurrenceId != null && entity.RecurrenceId is RECURRENCE_ID) ? (entity.RecurrenceId as RECURRENCE_ID) : null;
                var rrule = (entity.RecurrenceRule != null && entity.RecurrenceRule is RECUR) ? (entity.RecurrenceRule as RECUR) : null;
                var attends = (!entity.Attendees.OfType<ATTENDEE>().Empty()) ? entity.Attendees.OfType<ATTENDEE>() : null;
                var attachbins = (!entity.Attachments.OfType<ATTACH_BINARY>().Empty()) ? (entity.Attachments.OfType<ATTACH_BINARY>()) : null;
                var attachuris = (!entity.Attachments.OfType<ATTACH_URI>().Empty()) ? (entity.Attachments.OfType<ATTACH_URI>()) : null;
                var contacts = (!entity.Contacts.OfType<CONTACT>().Empty()) ? (entity.Contacts.OfType<CONTACT>()) : null;
                var comments = (!entity.Comments.OfType<COMMENT>().Empty()) ? (entity.Comments.OfType<COMMENT>()) : null;
                var rdates = (!entity.RecurrenceDates.OfType<RDATE>().Empty()) ? (entity.RecurrenceDates.OfType<RDATE>()) : null;
                var exdates = (!entity.ExceptionDates.OfType<EXDATE>().Empty()) ? (entity.ExceptionDates.OfType<EXDATE>()) : null;
                var relateds = (!entity.RelatedTos.OfType<RELATEDTO>().Empty()) ? (entity.RelatedTos.OfType<RELATEDTO>()) : null;
                var resources = (!entity.Resources.OfType<RESOURCES>().Empty()) ? (entity.Resources.OfType<RESOURCES>()) : null;
                var reqstats = (!entity.RequestStatuses.OfType<REQUEST_STATUS>().Empty()) ? (entity.RequestStatuses.OfType<REQUEST_STATUS>()) : null;
                var aalarms = (!entity.Alarms.OfType<AUDIO_ALARM>().Empty()) ? (entity.Alarms.OfType<AUDIO_ALARM>()) : null;
                var dalarms = (!entity.Alarms.OfType<DISPLAY_ALARM>().Empty()) ? (entity.Alarms.OfType<DISPLAY_ALARM>()) : null;
                var ealarms = (!entity.Alarms.OfType<EMAIL_ALARM>().Empty()) ? (entity.Alarms.OfType<EMAIL_ALARM>()) : null;

                //2. save details
                if (org != null) db.Save(org);
                if (rid != null) db.Save(rid);
                if (rrule != null) db.Save(rrule);
                if (!attends.NullOrEmpty()) db.SaveAll(attends);
                if (!attachbins.NullOrEmpty()) db.SaveAll(attachbins);
                if (!attachuris.NullOrEmpty()) db.SaveAll(attachuris);
                if (!contacts.NullOrEmpty()) db.SaveAll(contacts);
                if (!comments.NullOrEmpty()) db.SaveAll(comments);
                if (!rdates.NullOrEmpty()) db.SaveAll(rdates);
                if (!exdates.NullOrEmpty()) db.SaveAll(exdates);
                if (!relateds.NullOrEmpty()) db.SaveAll(relateds);
                if (!resources.NullOrEmpty()) db.SaveAll(resources);
                if (!reqstats.NullOrEmpty()) db.SaveAll(reqstats);
                if (!aalarms.NullOrEmpty()) this.AudioAlarmRepository.SaveAll(aalarms);
                if (!dalarms.NullOrEmpty()) this.DisplayAlarmRepository.SaveAll(dalarms);
                if (!ealarms.NullOrEmpty()) this.EmailAlarmRepository.SaveAll(ealarms);

                //3. construct relations from entity details
                var rorg = (entity.Organizer != null && entity.Organizer is ORGANIZER) ?
                    (new REL_EVENTS_ORGANIZERS {Id = this.KeyGenerator.GetNextKey(),  Uid = entity.Id, OrganizerId = (entity.Organizer as ORGANIZER).Id }) : null;
                var rrid = (entity.RecurrenceId != null && entity.RecurrenceId is RECURRENCE_ID) ?
                    (new REL_EVENTS_RECURRENCE_IDS { Id = this.KeyGenerator.GetNextKey(), Uid = entity.Id, RecurrenceId_Id = (entity.RecurrenceId as RECURRENCE_ID).Id }) : null;
                var rrrule = (entity.RecurrenceRule != null && entity.RecurrenceRule is RECUR) ?
                    (new REL_EVENTS_RRULES { Id = this.KeyGenerator.GetNextKey(), Uid = entity.Id, RecurrenceRuleId = (entity.RecurrenceRule as RECUR).Id }) : null;
                var rattends = (entity.Attendees.OfType<ATTENDEE>().Count() > 0) ? entity.Attendees.OfType<ATTENDEE>()
                    .Select(x => new REL_EVENTS_ATTENDEES { Id = this.KeyGenerator.GetNextKey(), Uid = entity.Id, AttendeeId = x.Id }) : null;
                var rattachbins = (entity.Attachments.OfType<ATTACH_BINARY>().Count() > 0) ?
                    (entity.Attachments.OfType<ATTACH_BINARY>().Select(x => new REL_EVENTS_ATTACHBINS { Id = this.KeyGenerator.GetNextKey(), Uid = entity.Id, AttachmentId = x.Id })) : null;
                var rattachuris = (entity.Attachments.OfType<ATTACH_URI>().Count() > 0) ?
                    (entity.Attachments.OfType<ATTACH_URI>().Select(x => new REL_EVENTS_ATTACHURIS { Id = this.KeyGenerator.GetNextKey(), Uid = entity.Id, AttachmentId = x.Id })) : null;
                var rcontacts = (entity.Contacts.OfType<CONTACT>().Count() > 0) ?
                    (entity.Contacts.OfType<CONTACT>().Select(x => new REL_EVENTS_CONTACTS { Id = this.KeyGenerator.GetNextKey(), Uid = entity.Id, ContactId = x.Id })) : null;
                var rcomments = (entity.Contacts.OfType<COMMENT>().Count() > 0) ?
                    (entity.Contacts.OfType<COMMENT>().Select(x => new REL_EVENTS_COMMENTS { Id = this.KeyGenerator.GetNextKey(), Uid = entity.Id, CommentId = x.Id })) : null;
                var rrdates = (entity.RecurrenceDates.OfType<RDATE>().Count() > 0) ?
                    (entity.RecurrenceDates.OfType<RDATE>().Select(x => new REL_EVENTS_RDATES { Id = this.KeyGenerator.GetNextKey(), Uid = entity.Id, RecurrenceDateId = x.Id })) : null;
                var rexdates = (entity.ExceptionDates.OfType<EXDATE>().Count() > 0) ?
                    (entity.ExceptionDates.OfType<EXDATE>().Select(x => new REL_EVENTS_EXDATES { Id = this.KeyGenerator.GetNextKey(), Uid = entity.Id, ExceptionDateId = x.Id })) : null;
                var rrelateds = (entity.RelatedTos.OfType<RELATEDTO>().Count() > 0) ?
                    (entity.RelatedTos.OfType<RELATEDTO>().Select(x => new REL_EVENTS_RELATEDTOS { Id = this.KeyGenerator.GetNextKey(), Uid = entity.Id, RelatedToId = x.Id })) : null;
                var rresources = (entity.Resources.OfType<RESOURCES>().Count() > 0) ?
                    (entity.Resources.OfType<RESOURCES>().Select(x => new REL_EVENTS_RESOURCES { Id = this.KeyGenerator.GetNextKey(), Uid = entity.Id, ResourcesId = x.Id })) : null;
                var rreqstats = (entity.RequestStatuses.OfType<REQUEST_STATUS>().Count() > 0) ?
                    (entity.RequestStatuses.OfType<REQUEST_STATUS>().Select(x => new REL_EVENTS_REQSTATS { Id = this.KeyGenerator.GetNextKey(), Uid = entity.Id, ReqStatsId = x.Id })) : null;
                var raalarms = (entity.Alarms.OfType<AUDIO_ALARM>().Count() > 0) ?
                    (entity.Alarms.OfType<AUDIO_ALARM>().Select(x => new REL_EVENTS_AUDIO_ALARMS { Id = this.KeyGenerator.GetNextKey(), Uid = entity.Id, AlarmId = x.Id })) : null;
                var rdalarms = (entity.Alarms.OfType<DISPLAY_ALARM>().Count() > 0) ?
                    (entity.Alarms.OfType<DISPLAY_ALARM>().Select(x => new REL_EVENTS_DISPLAY_ALARMS { Id = this.KeyGenerator.GetNextKey(), Uid = entity.Id, AlarmId = x.Id })) : null;
                var realarms = (entity.Alarms.OfType<EMAIL_ALARM>().Count() > 0) ?
                    (entity.Alarms.OfType<EMAIL_ALARM>().Select(x => new REL_EVENTS_EMAIL_ALARMS { Id = this.KeyGenerator.GetNextKey(), Uid = entity.Id, AlarmId = x.Id })) : null;
                
                //4. retrieve existing entity-details relations
                var ororgs = (org != null) ?db.Select<REL_EVENTS_ORGANIZERS>(q => q.Uid == entity.Id && q.OrganizerId == org.Id) : null;
                var orrids = (rid != null) ? db.Select<REL_EVENTS_RECURRENCE_IDS>(q => q.Uid == entity.Id && q.RecurrenceId_Id == rid.Id) : null;
                var orrrules = (rrule != null) ? db.Select<REL_EVENTS_RRULES>(q => q.Uid == entity.Id && q.RecurrenceRuleId == rrule.Id) : null;
                var orattends = (!attends.NullOrEmpty()) ?
                    db.Select<REL_EVENTS_ATTENDEES>(q => q.Uid == entity.Id && Sql.In(q.AttendeeId, attends.Select(x => x.Id).ToArray())) : null;
                var orattachbins = (!attachbins.NullOrEmpty()) ?
                    db.Select<REL_EVENTS_ATTACHBINS>(q => q.Uid == entity.Id && Sql.In(q.AttachmentId, attachbins.Select(x => x.Id).ToArray())) : null;
                var orattachuris = (!attachuris.NullOrEmpty()) ?
                    db.Select<REL_EVENTS_ATTACHURIS>(q => q.Uid == entity.Id && Sql.In(q.AttachmentId, attachuris.Select(x => x.Id).ToArray())) : null;
                var orcontacts = (!contacts.NullOrEmpty()) ?
                    db.Select<REL_EVENTS_CONTACTS>(q => q.Uid == entity.Id && Sql.In(q.ContactId, contacts.Select(x => x.Id).ToArray())) : null;
                var orcomments = (!comments.NullOrEmpty()) ?
                    db.Select<REL_EVENTS_COMMENTS>(q => q.Uid == entity.Id && Sql.In(q.CommentId, comments.Select(x => x.Id).ToArray())) : null;
                var orrdates = (!rdates.NullOrEmpty()) ?
                    db.Select<REL_EVENTS_RDATES>(q => q.Uid == entity.Id && Sql.In(q.RecurrenceDateId, rdates.Select(x => x.Id).ToArray())) : null;
                var orexdates = (!exdates.NullOrEmpty()) ?
                    db.Select<REL_EVENTS_EXDATES>(q => q.Uid == entity.Id && Sql.In(q.ExceptionDateId, exdates.Select(x => x.Id).ToArray())) : null;
                var orrelateds = (!relateds.NullOrEmpty()) ?
                    db.Select<REL_EVENTS_RELATEDTOS>(q => q.Uid == entity.Id && Sql.In(q.RelatedToId, relateds.Select(x => x.Id).ToArray())) : null;
                var orresources = (!resources.NullOrEmpty()) ?
                    db.Select<REL_EVENTS_RESOURCES>(q => q.Uid == entity.Id && Sql.In(q.ResourcesId, resources.Select(x => x.Id).ToArray())) : null;
                var orreqstats = (!reqstats.NullOrEmpty()) ?
                    db.Select<REL_EVENTS_REQSTATS>(q => q.Uid == entity.Id && Sql.In(q.ReqStatsId, reqstats.Select(x => x.Id))) : null;
                var oraalarms = (!aalarms.NullOrEmpty()) ? db.Select<REL_EVENTS_AUDIO_ALARMS>(q => q.Uid == entity.Id && Sql.In(q.Uid, aalarms.Select(x => x.Id).ToArray())) : null;
                var ordalarms = (!dalarms.NullOrEmpty()) ? db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => q.Uid == entity.Id && Sql.In(q.Uid, dalarms.Select(x => x.Id).ToArray())) : null;
                var orealarms = (!ealarms.NullOrEmpty()) ? db.Select<REL_EVENTS_EMAIL_ALARMS>(q => q.Uid == entity.Id && Sql.In(q.Uid, ealarms.Select(x => x.Id).ToArray())) : null;


                //5. save non-existing entity-details relations
                if (rorg != null && !ororgs.Contains(rorg)) db.Save(rorg);
                if (rrid != null && !orrids.Contains(rrid)) db.Save(rrid);
                if (rrrule != null && !orrrules.Contains(rrrule)) db.Save(rrule);
                if (!rattends.NullOrEmpty() && !rattends.Except(orattends).NullOrEmpty()) db.SaveAll(rattends.Except(orattends));
                if (!rattachbins.NullOrEmpty() && !rattachbins.Except(orattachbins).NullOrEmpty()) db.SaveAll(rattachbins.Except(orattachbins));
                if (!rattachuris.NullOrEmpty() && !rattachuris.Except(orattachuris).NullOrEmpty()) db.SaveAll(rattachuris.Except(orattachuris));
                if (!rcontacts.NullOrEmpty() && !rcontacts.Except(orcontacts).NullOrEmpty()) db.SaveAll(rcontacts.Except(orcontacts));
                if (!rcomments.NullOrEmpty() && !rcomments.Except(orcomments).NullOrEmpty()) db.SaveAll(rcomments.Except(orcomments));
                if (!rrdates.NullOrEmpty() && !rrdates.Except(orrdates).NullOrEmpty()) db.SaveAll(rrdates.Except(orrdates));
                if (!rexdates.NullOrEmpty() && !rexdates.Except(orexdates).NullOrEmpty()) db.SaveAll(rexdates.Except(orexdates));
                if (!rrelateds.NullOrEmpty() && !rrelateds.Except(orrelateds).NullOrEmpty()) db.SaveAll(rrelateds.Except(orrelateds));
                if (!rresources.NullOrEmpty() && !rresources.Except(orresources).NullOrEmpty()) db.SaveAll(rresources.Except(orresources));
                if (!rreqstats.NullOrEmpty() && !rreqstats.Except(orreqstats).NullOrEmpty()) db.SaveAll(rreqstats.Except(orreqstats));
                if (!raalarms.NullOrEmpty() && !raalarms.Except(oraalarms).NullOrEmpty()) db.SaveAll(raalarms.Except(oraalarms));
                if (!rdalarms.NullOrEmpty() && !rdalarms.Except(ordalarms).NullOrEmpty()) db.SaveAll(rdalarms.Except(ordalarms));
                if (!realarms.NullOrEmpty() && !realarms.Except(orealarms).NullOrEmpty()) db.SaveAll(realarms.Except(orealarms));
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public void Patch(VEVENT source, Expression<Func<VEVENT, object>> fields, Expression<Func<VEVENT, bool>> where = null)
        {
            try
            {

                //1. Get fields slected for patching
                var selection = fields.GetMemberNames();

                //2.Get list of all non-related event details (primitives)
                Expression<Func<VEVENT, object>> primitives = x => new
                {
                    x.Uid,
                    x.Start,
                    x.Created,
                    x.Description,
                    x.Geo,
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

                //3.Get list of all related event details (relational)
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

                //5. Patch relations
                if (!srelation.NullOrEmpty())
                {
                    //5.1. Derive expression for each relation
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

                    var uids = (where != null) ? db.SelectParam<VEVENT>(q => q.Uid, where).ToArray(): db.SelectParam<VEVENT>(q => q.Uid).ToArray();
                    if (selection.Contains(orgexpr.GetMemberName()))
                    {
                        //get events-organizers relations
                        var org = (source.Organizer != null && source.Organizer is ORGANIZER) ? (source.Organizer as ORGANIZER) : null;
                        if (org != null && !uids.NullOrEmpty())
                        {
                            db.Save(org);
                            var rorgs = uids.Select(x => new REL_EVENTS_ORGANIZERS { Id = this.KeyGenerator.GetNextKey(), Uid = x, OrganizerId = org.Id });
                            var ororgs = db.Select<REL_EVENTS_ORGANIZERS>(q => Sql.In(q.Uid, uids) && q.OrganizerId == (source.Organizer as ORGANIZER).Id);
                            if (!rorgs.NullOrEmpty() && !rorgs.Except(ororgs).NullOrEmpty()) db.SaveAll(rorgs.Except(ororgs));
                        }                                         
                    }
                    if (selection.Contains(rruleexpr.GetMemberName()))
                    {
                        var rrule = (source.RecurrenceRule != null && source.RecurrenceRule is RECUR) ? (source.RecurrenceRule as RECUR) : null;
                        if (rrule != null && !uids.NullOrEmpty())
                        {
                            db.Save(rrule);
                            var rrrules = uids.Select(x => new REL_EVENTS_RRULES { Id = this.KeyGenerator.GetNextKey(), Uid = x, RecurrenceRuleId = rrule.Id });
                            var orrrules = db.Select<REL_EVENTS_RRULES>(q => Sql.In(q.Uid, uids) && q.RecurrenceRuleId == (source.RecurrenceRule as RECUR).Id);
                            if (!rrrules.NullOrEmpty() && !rrrules.Except(orrrules).NullOrEmpty()) db.SaveAll(rrrules.Except(orrrules));
                        } 
                    }

                    if (selection.Contains(attendsexpr.GetMemberName()))
                    {
                        var attends = (!source.Attendees.OfType<ATTENDEE>().NullOrEmpty()) ? source.Attendees.OfType<ATTENDEE>() : null;
                        if (!attends.NullOrEmpty() && !uids.NullOrEmpty())
                        {
                            db.SaveAll(attends);
                            var rattends = uids.SelectMany(x => attends.Select(y => new REL_EVENTS_ATTENDEES { Id = this.KeyGenerator.GetNextKey(), Uid = x, AttendeeId = y.Id }));
                            var orattends = db.Select<REL_EVENTS_ATTENDEES>(q => Sql.In(q.Uid, uids) && Sql.In(q.AttendeeId, attends.Select(x => x.Id).ToArray()));
                            if (!rattends.NullOrEmpty() && !rattends.Except(orattends).NullOrEmpty()) db.SaveAll(rattends.Except(orattends));
                        } 
                    }

                    if(selection.Contains(attachsexpr.GetMemberName()))
                    {
                        var attachbins = (!source.Attachments.OfType<ATTACH_BINARY>().NullOrEmpty()) ? source.Attachments.OfType<ATTACH_BINARY>() : null;
                        if (!attachbins.NullOrEmpty() && !uids.NullOrEmpty())
                        {
                            db.SaveAll(attachbins);
                            var rattachbins = uids.SelectMany(x => attachbins.Select(y => new REL_EVENTS_ATTACHBINS { Id = this.KeyGenerator.GetNextKey(), Uid = x, AttachmentId = y.Id }));
                            var orattachbins = db.Select<REL_EVENTS_ATTACHBINS>(q => Sql.In(q.Uid, uids) && Sql.In(q.AttachmentId, attachbins.Select(x => x.Id).ToArray()));
                            if (!rattachbins.NullOrEmpty() && !rattachbins.Except(orattachbins).NullOrEmpty()) db.SaveAll(rattachbins.Except(orattachbins));

                        }                   

                        var attachuris = (!source.Attachments.OfType<ATTACH_BINARY>().NullOrEmpty()) ? source.Attachments.OfType<ATTACH_BINARY>() : null;
                        if (!attachuris.NullOrEmpty() && !uids.NullOrEmpty())
                        {
                            db.SaveAll(attachuris);
                            var rattachuris = uids.SelectMany(x => attachuris.Select(y => new REL_EVENTS_ATTACHURIS { Id = this.KeyGenerator.GetNextKey(), Uid = x, AttachmentId = y.Id }));
                            var orattachuris = db.Select<REL_EVENTS_ATTACHURIS>(q => Sql.In(q.Uid, uids) && Sql.In(q.AttachmentId, attachuris.Select(x => x.Id).ToArray()));
                            if (!rattachuris.NullOrEmpty() && !rattachuris.Except(orattachuris).NullOrEmpty()) db.SaveAll(rattachuris.Except(orattachuris));
                        }
                    }

                    if (selection.Contains(contactsexpr.GetMemberName()))
                    {
                        var contacts = (!source.Contacts.OfType<CONTACT>().NullOrEmpty()) ? source.Contacts.OfType<CONTACT>() : null;
                        if (!contacts.NullOrEmpty() && !uids.NullOrEmpty())
                        {
                            db.SaveAll(contacts);
                            var rcontacts = uids.SelectMany(x => contacts.Select(y => new REL_EVENTS_CONTACTS { Id = this.KeyGenerator.GetNextKey(), Uid = x, ContactId = y.Id }));
                            var orcontacts = db.Select<REL_EVENTS_CONTACTS>(q => Sql.In(q.Uid, uids) && Sql.In(q.ContactId, contacts.Select(x => x.Id).ToArray()));
                            if (!rcontacts.NullOrEmpty() && !rcontacts.Except(orcontacts).NullOrEmpty()) db.SaveAll(rcontacts.Except(orcontacts));
                        }
                    }

                    if (selection.Contains(commentsexpr.GetMemberName()))
                    {
                        var comments = (!source.Contacts.OfType<COMMENT>().NullOrEmpty()) ? source.Contacts.OfType<COMMENT>() : null;
                        if (!comments.NullOrEmpty() && !uids.NullOrEmpty())
                        {
                            db.SaveAll(comments);
                            var rcomments = uids.SelectMany(x => comments.Select(y => new REL_EVENTS_COMMENTS { Id = this.KeyGenerator.GetNextKey(), Uid = x, CommentId = y.Id }));
                            var orcomments = db.Select<REL_EVENTS_COMMENTS>(q => Sql.In(q.Uid, uids) && Sql.In(q.CommentId, comments.Select(x => x.Id).ToArray()));
                            if (!rcomments.NullOrEmpty() && !rcomments.Except(orcomments).NullOrEmpty()) db.SaveAll(rcomments.Except(orcomments));
                        }
                    }

                    if (selection.Contains(rdatesexpr.GetMemberName()))
                    {
                        var rdates = (!source.Contacts.OfType<RDATE>().NullOrEmpty()) ? source.Contacts.OfType<RDATE>() : null;
                        if (!rdates.NullOrEmpty() && !uids.NullOrEmpty())
                        {
                            db.SaveAll(rdates);
                            var rrdates = uids.SelectMany(x => rdates.Select(y => new REL_EVENTS_RDATES { Id = this.KeyGenerator.GetNextKey(), Uid = x, RecurrenceDateId = y.Id }));
                            var ordates = db.Select<REL_EVENTS_RDATES>(q => Sql.In(q.Uid, uids) && Sql.In(q.RecurrenceDateId, rdates.Select(x => x.Id).ToArray()));
                            if (!rdates.NullOrEmpty() && !rrdates.Except(ordates).NullOrEmpty()) db.SaveAll(rrdates.Except(ordates));
                        }
                    }

                    if (selection.Contains(exdatesexpr.GetMemberName()))
                    {
                        var exdates = (!source.Contacts.OfType<EXDATE>().NullOrEmpty()) ? source.Contacts.OfType<EXDATE>() : null;
                        if (!exdates.NullOrEmpty() && !uids.NullOrEmpty())
                        {
                            db.SaveAll(exdates);
                            var rexdates = uids.SelectMany(x => exdates.Select(y => new REL_EVENTS_EXDATES { Id = this.KeyGenerator.GetNextKey(), Uid = x, ExceptionDateId = y.Id }));
                            var orexdates = db.Select<REL_EVENTS_EXDATES>(q => Sql.In(q.Uid, uids) && Sql.In(q.ExceptionDateId, exdates.Select(x => x.Id).ToArray()));
                            if (!exdates.NullOrEmpty() && !rexdates.Except(orexdates).NullOrEmpty()) db.SaveAll(rexdates.Except(orexdates));
                        }
                    }

                    if (selection.Contains(relatedtosexpr.GetMemberName()))
                    {
                        var relatedtos = (!source.Contacts.OfType<RELATEDTO>().NullOrEmpty()) ? source.Contacts.OfType<RELATEDTO>() : null;
                        if (!relatedtos.NullOrEmpty() && !uids.NullOrEmpty())
                        {
                            db.SaveAll(relatedtos);
                            var rrelatedtos = uids.SelectMany(x => relatedtos.Select(y => new REL_EVENTS_RELATEDTOS { Id = this.KeyGenerator.GetNextKey(), Uid = x, RelatedToId = y.Id }));
                            var orrelatedtos = db.Select<REL_EVENTS_RELATEDTOS>(q => Sql.In(q.Uid, uids) && Sql.In(q.RelatedToId, relatedtos.Select(x => x.Id).ToArray()));
                            if (!relatedtos.NullOrEmpty() && !rrelatedtos.Except(orrelatedtos).NullOrEmpty()) db.SaveAll(rrelatedtos.Except(orrelatedtos));
                        }
                    }

                    if (selection.Contains(resourcesexpr.GetMemberName()))
                    {
                        var resources = (!source.Contacts.OfType<RESOURCES>().NullOrEmpty()) ? source.Contacts.OfType<RESOURCES>() : null;
                        if (!resources.NullOrEmpty() && !uids.NullOrEmpty())
                        {
                            db.SaveAll(resources);
                            var rresources = uids.SelectMany(x => resources.Select(y => new REL_EVENTS_RESOURCES { Id = this.KeyGenerator.GetNextKey(), Uid = x, ResourcesId = y.Id }));
                            var orresources = db.Select<REL_EVENTS_RESOURCES>(q => Sql.In(q.Uid, uids) && Sql.In(q.ResourcesId, resources.Select(x => x.Id).ToArray()));
                            if (!resources.NullOrEmpty() && !rresources.Except(orresources).NullOrEmpty()) db.SaveAll(rresources.Except(orresources));
                        }
                    }

                    if (selection.Contains(reqstatsexpr.GetMemberName()))
                    {
                        var reqstats = (!source.Contacts.OfType<REQUEST_STATUS>().NullOrEmpty()) ? source.Contacts.OfType<REQUEST_STATUS>() : null;
                        if (!reqstats.NullOrEmpty() && !uids.NullOrEmpty())
                        {
                            db.SaveAll(reqstats);
                            var rreqstats = uids.SelectMany(x => reqstats.Select(y => new REL_EVENTS_REQSTATS { Id = this.KeyGenerator.GetNextKey(), Uid = x, ReqStatsId = y.Id }));
                            var orreqstats = db.Select<REL_EVENTS_REQSTATS>(q => Sql.In(q.Uid, uids) && Sql.In(q.ReqStatsId, reqstats.Select(x => x.Id).ToArray()));
                            if (!reqstats.NullOrEmpty() && !rreqstats.Except(orreqstats).NullOrEmpty()) db.SaveAll(rreqstats.Except(orreqstats));
                        }
                    }

                    if (selection.Contains(alarmexpr.GetMemberName()))
                    {
                        var aalarms = (!source.Contacts.OfType<AUDIO_ALARM>().NullOrEmpty()) ? source.Contacts.OfType<AUDIO_ALARM>() : null;
                        if (!aalarms.NullOrEmpty() && !uids.NullOrEmpty())
                        {
                            AudioAlarmRepository.SaveAll(aalarms);
                            var raalarms = uids.SelectMany(x => aalarms.Select(y => new REL_EVENTS_AUDIO_ALARMS { Id = this.KeyGenerator.GetNextKey(), Uid = x, AlarmId = y.Id }));
                            var oraalarms = db.Select<REL_EVENTS_AUDIO_ALARMS>(q => Sql.In(q.Uid, uids) && Sql.In(q.AlarmId, aalarms.Select(x => x.Id).ToArray()));
                            if (!aalarms.NullOrEmpty() && !raalarms.Except(oraalarms).NullOrEmpty()) db.SaveAll(raalarms.Except(oraalarms));
                        }
                        var dalarms = (!source.Contacts.OfType<DISPLAY_ALARM>().NullOrEmpty()) ? source.Contacts.OfType<DISPLAY_ALARM>() : null;
                        if (!dalarms.NullOrEmpty() && !uids.NullOrEmpty())
                        {
                            DisplayAlarmRepository.SaveAll(dalarms);
                            var rdalarms = uids.SelectMany(x => dalarms.Select(y => new REL_EVENTS_DISPLAY_ALARMS { Id = this.KeyGenerator.GetNextKey(), Uid = x, AlarmId = y.Id }));
                            var ordalarms = db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => Sql.In(q.Uid, uids) && Sql.In(q.AlarmId, dalarms.Select(x => x.Id).ToArray()));
                            if (!dalarms.NullOrEmpty() && !rdalarms.Except(ordalarms).NullOrEmpty()) db.SaveAll(rdalarms.Except(ordalarms));
                        }

                        var ealarms = (!source.Contacts.OfType<EMAIL_ALARM>().NullOrEmpty()) ? source.Contacts.OfType<EMAIL_ALARM>() : null;
                        if (!ealarms.NullOrEmpty() && !uids.NullOrEmpty())
                        {
                            EmailAlarmRepository.SaveAll(ealarms);
                            var realarms = uids.SelectMany(x => ealarms.Select(y => new REL_EVENTS_EMAIL_ALARMS { Id = this.KeyGenerator.GetNextKey(), Uid = x, AlarmId = y.Id }));
                            var orealarms = db.Select<REL_EVENTS_EMAIL_ALARMS>(q => Sql.In(q.Uid, uids) && Sql.In(q.AlarmId, dalarms.Select(x => x.Id).ToArray()));
                            if (!ealarms.NullOrEmpty() && !realarms.Except(orealarms).NullOrEmpty()) db.SaveAll(realarms.Except(orealarms));
                        }

                    } 
                    
                }

                //6. Get list of selected primitives
                var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

                //7. Update matching event primitives
                if(!sprimitives.NullOrEmpty())
                {
                    var patchstr = string.Format("f => new {{ {0} }}", string.Join(", ", sprimitives.Select(x => string.Format("f.{0}", x))));
                    var patchexpr = patchstr.CompileToExpressionFunc<VEVENT, object>(CodeDomLanguage.csharp, Utilities.GetReferencedAssemblyNamesFromEntryAssembly());
                    db.UpdateOnly<VEVENT, object>(source, patchexpr, where);
                }

            }
            catch (NotImplementedException) { throw; }
            catch (System.Security.SecurityException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public void Erase(string key)
        {
            try
            {
                db.Delete<VEVENT>(q => q.Uid.ToUpper() == key.ToUpper());
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public void SaveAll(IEnumerable<VEVENT> entities)
        {
            try
            {
                     db.SaveAll(entities);
                    
                    //1. retrieve details of events
                    var orgs = entities.Where(x => x.Organizer != null && x.Organizer is ORGANIZER).Select(x => x.Organizer as ORGANIZER);
                    var rids = entities.Where(x => x.RecurrenceId != null && x.RecurrenceId is RECURRENCE_ID).Select(x => x.RecurrenceId as RECURRENCE_ID);
                    var rrules = entities.Where(x => x.RecurrenceRule != null && x.RecurrenceRule is RECUR).Select(x => x.RecurrenceRule as RECUR);
                    var attends = entities.Where(x => x.Attendees.OfType<ATTENDEE>().Count() > 0).SelectMany(x => x.Attendees.OfType<ATTENDEE>());
                    var attachbins = entities.Where(x => x.Attachments.OfType<ATTACH_BINARY>().Count() > 0).SelectMany(x => x.Attachments.OfType<ATTACH_BINARY>());
                    var attachuris = entities.Where(x => x.Attachments.OfType<ATTACH_URI>().Count() > 0).SelectMany(x => x.Attachments.OfType<ATTACH_URI>());
                    var contacts = entities.Where(x => x.Contacts.OfType<CONTACT>().Count() > 0).SelectMany(x => x.Contacts.OfType<CONTACT>());
                    var comments = entities.Where(x => x.Comments.OfType<COMMENT>().Count() > 0).SelectMany(x => x.Comments.OfType<COMMENT>());
                    var rdates = entities.Where(x => x.RecurrenceDates.OfType<RDATE>().Count() > 0).SelectMany(x => x.RecurrenceDates.OfType<RDATE>());
                    var exdates = entities.Where(x => x.ExceptionDates.OfType<EXDATE>().Count() > 0).SelectMany(x => x.ExceptionDates.OfType<EXDATE>());
                    var relateds = entities.Where(x => x.RelatedTos.OfType<RELATEDTO>().Count() > 0).SelectMany(x => x.RelatedTos.OfType<RELATEDTO>());
                    var resources = entities.Where(x => x.Resources.OfType<RESOURCES>().Count() > 0).SelectMany(x => x.Resources.OfType<RESOURCES>());
                    var reqstats = entities.Where(x => x.RequestStatuses.OfType<REQUEST_STATUS>().Count() > 0).SelectMany(x => x.RequestStatuses.OfType<REQUEST_STATUS>());
                    var aalarms = entities.Where(x => x.Alarms.OfType<AUDIO_ALARM>().Count() > 0).SelectMany(x => x.Alarms.OfType<AUDIO_ALARM>());
                    var dalarms = entities.Where(x => x.Alarms.OfType<DISPLAY_ALARM>().Count() > 0).SelectMany(x => x.Alarms.OfType<DISPLAY_ALARM>());
                    var ealarms = entities.Where(x => x.Alarms.OfType<EMAIL_ALARM>().Count() > 0).SelectMany(x => x.Alarms.OfType<EMAIL_ALARM>());


                    //2. save details of events
                    if (!orgs.NullOrEmpty()) db.SaveAll(orgs);
                    if (!rids.NullOrEmpty()) db.SaveAll(rids);
                    if (!rrules.NullOrEmpty()) db.SaveAll(rrules);
                    if (!attends.NullOrEmpty()) db.SaveAll(attends);
                    if (!attachbins.NullOrEmpty()) db.SaveAll(attachbins);
                    if (!attachuris.NullOrEmpty()) db.SaveAll(attachuris);
                    if (!contacts.NullOrEmpty()) db.SaveAll(contacts);
                    if (!comments.NullOrEmpty()) db.SaveAll(comments);
                    if (!rdates.NullOrEmpty()) db.SaveAll(rdates);
                    if (!exdates.NullOrEmpty()) db.SaveAll(exdates);
                    if (!relateds.NullOrEmpty()) db.SaveAll(relateds);
                    if (!resources.NullOrEmpty()) db.SaveAll(resources);
                    if (!reqstats.NullOrEmpty()) db.SaveAll(reqstats);
                    if (!aalarms.NullOrEmpty()) this.AudioAlarmRepository.SaveAll(aalarms);
                    if (!dalarms.NullOrEmpty()) this.DisplayAlarmRepository.SaveAll(dalarms);
                    if (!ealarms.NullOrEmpty()) this.EmailAlarmRepository.SaveAll(ealarms);

                    //3. construct available relations
                    var rorgs = entities.Where(x => x.Organizer != null && x.Organizer is ORGANIZER)
                        .Select(e => new REL_EVENTS_ORGANIZERS { Id = this.KeyGenerator.GetNextKey(), Uid = e.Id, OrganizerId = (e.Organizer as ORGANIZER).Id });
                    var rrids = entities.Where(x => x.RecurrenceId != null && x.RecurrenceId is ORGANIZER)
                        .Select(e => new REL_EVENTS_RECURRENCE_IDS { Id = this.KeyGenerator.GetNextKey(), Uid = e.Id, RecurrenceId_Id = (e.RecurrenceId as RECURRENCE_ID).Id });
                    var rrrules = entities.Where(x => x.RecurrenceRule != null && x.RecurrenceRule is RECUR)
                        .Select(e => new REL_EVENTS_RRULES { Id = this.KeyGenerator.GetNextKey(), Uid = e.Id, RecurrenceRuleId = (e.RecurrenceRule as RECUR).Id });
                    var rattends = entities.Where(x => x.Attendees.OfType<ATTENDEE>().Count() > 0)
                        .SelectMany(e => e.Attendees.OfType<ATTENDEE>().Select(x => new REL_EVENTS_ATTENDEES { Id = this.KeyGenerator.GetNextKey(), Uid = e.Id, AttendeeId = x.Id }));
                    var rattachbins = entities.Where(x => x.Attachments.OfType<ATTACH_BINARY>().Count() > 0)
                        .SelectMany(e => e.Attachments.OfType<ATTACH_BINARY>().Select(x => new REL_EVENTS_ATTACHBINS { Id = this.KeyGenerator.GetNextKey(), Uid = e.Id, AttachmentId = x.Id }));
                    var rattachuris = entities.Where(x => x.Attachments.OfType<ATTACH_URI>().Count() > 0)
                        .SelectMany(e => e.Attachments.OfType<ATTACH_URI>().Select(x => new REL_EVENTS_ATTACHURIS { Id = this.KeyGenerator.GetNextKey(), Uid = e.Id, AttachmentId = x.Id }));
                    var rcontacts = entities.Where(x => x.Contacts.OfType<CONTACT>().Count() > 0)
                        .SelectMany(e => e.Contacts.OfType<CONTACT>().Select(x => new REL_EVENTS_CONTACTS { Id = this.KeyGenerator.GetNextKey(), Uid = e.Id, ContactId = x.Id }));
                    var rcomments = entities.Where(x => x.Contacts.OfType<COMMENT>().Count() > 0)
                        .SelectMany(e => e.Contacts.OfType<COMMENT>().Select(x => new REL_EVENTS_COMMENTS { Id = this.KeyGenerator.GetNextKey(), Uid = e.Id, CommentId = x.Id })); 
                    var rrdates = entities.Where(x => x.RecurrenceDates.OfType<RDATE>().Count() > 0)
                        .SelectMany(e => e.RecurrenceDates.OfType<RDATE>().Distinct(new EqualByStringId<RDATE>())
                        .Select(x => new REL_EVENTS_RDATES { Id = this.KeyGenerator.GetNextKey(), Uid = e.Id, RecurrenceDateId = x.Id })); 
                    var rexdates = entities.Where(x => x.ExceptionDates.OfType<EXDATE>().Count() > 0)
                        .SelectMany(e => e.ExceptionDates.OfType<EXDATE>().Select(x => new REL_EVENTS_EXDATES { Id = this.KeyGenerator.GetNextKey(), Uid = e.Id, ExceptionDateId = x.Id })); 
                    var rrelateds = entities.Where(x => x.RelatedTos.OfType<RELATEDTO>().Count() > 0)
                        .SelectMany(e => e.RelatedTos.OfType<RELATEDTO>().Select(x => new REL_EVENTS_RELATEDTOS { Id = this.KeyGenerator.GetNextKey(), Uid = e.Id, RelatedToId = x.Id }));
                    var rresources = entities.Where(x => x.Resources.OfType<RESOURCES>().Count() > 0)
                        .SelectMany(e => e.Resources.OfType<RESOURCES>().Select(x => new REL_EVENTS_RESOURCES { Id = this.KeyGenerator.GetNextKey(), Uid = e.Id, ResourcesId = x.Id }));
                    var rreqstats = entities.Where(x => x.RequestStatuses.OfType<REQUEST_STATUS>().Count() > 0)
                        .SelectMany(e => e.RequestStatuses.OfType<REQUEST_STATUS>().Select(x => new REL_EVENTS_REQSTATS { Id = this.KeyGenerator.GetNextKey(), Uid = e.Id, ReqStatsId = x.Id }));
                    var raalarms = entities.Where(x => x.Alarms.OfType<AUDIO_ALARM>().Count() > 0)
                        .SelectMany(e => e.RequestStatuses.OfType<AUDIO_ALARM>().Select(x => new REL_EVENTS_AUDIO_ALARMS { Id = this.KeyGenerator.GetNextKey(), Uid = e.Id, AlarmId = x.Id }));
                    var rdalarms = entities.Where(x => x.Alarms.OfType<DISPLAY_ALARM>().Count() > 0)
                        .SelectMany(e => e.RequestStatuses.OfType<DISPLAY_ALARM>().Select(x => new REL_EVENTS_DISPLAY_ALARMS {Id = this.KeyGenerator.GetNextKey(), Uid = e.Id, AlarmId = x.Id }));
                    var realarms = entities.Where(x => x.Alarms.OfType<EMAIL_ALARM>().Count() > 0)
                        .SelectMany(e => e.RequestStatuses.OfType<EMAIL_ALARM>().Select(x => new REL_EVENTS_EMAIL_ALARMS { Id = this.KeyGenerator.GetNextKey(), Uid = e.Id, AlarmId = x.Id }));

                    //4. retrieve existing relations
                    var ororgs = (!orgs.NullOrEmpty()) ?  
                        db.Select<REL_EVENTS_ORGANIZERS>(q => Sql.In(q.Uid, entities.Select(x => x.Id)) && Sql.In(q.OrganizerId, orgs.Select(x => x.Id).ToArray())): 
                        new List<REL_EVENTS_ORGANIZERS>();
                    var orrids = (!rids.NullOrEmpty()) ?  
                        db.Select<REL_EVENTS_RECURRENCE_IDS>(q => Sql.In(q.Uid, entities.Select(x => x.Id)) && Sql.In(q.RecurrenceId_Id, rids.Select(x => x.Id).ToArray())): 
                        new List<REL_EVENTS_RECURRENCE_IDS>();
                    var orrrules = (!rrules.NullOrEmpty()) ?
                        db.Select<REL_EVENTS_RRULES>(q => Sql.In(q.Uid, entities.Select(x => x.Id)) && Sql.In(q.RecurrenceRuleId, rrules.Select(x => x.Id).ToArray())) : 
                        new List<REL_EVENTS_RRULES>();
                    var orattends = (!attends.NullOrEmpty()) ?
                        db.Select<REL_EVENTS_ATTENDEES>(q => Sql.In(q.Uid, entities.Select(x => x.Id)) && Sql.In(q.AttendeeId, attends.Select(x => x.Id).ToArray())) : 
                        new List<REL_EVENTS_ATTENDEES>();
                    var orattachbins = (!attachbins.NullOrEmpty()) ?
                        db.Select<REL_EVENTS_ATTACHBINS>(q => Sql.In(q.Uid, entities.Select(x => x.Id)) && Sql.In(q.AttachmentId, attachbins.Select(x => x.Id).ToArray())) : 
                        new List<REL_EVENTS_ATTACHBINS>();
                    var orattachuris = (!attachuris.NullOrEmpty()) ?
                        db.Select<REL_EVENTS_ATTACHURIS>(q => Sql.In(q.Uid, entities.Select(x => x.Id)) && Sql.In(q.AttachmentId, attachuris.Select(x => x.Id))) : 
                        new List<REL_EVENTS_ATTACHURIS>();
                    var orcontacts = (!contacts.NullOrEmpty()) ?
                        db.Select<REL_EVENTS_CONTACTS>(q => Sql.In(q.Uid, entities.Select(x => x.Id)) && Sql.In(q.ContactId, contacts.Select(x => x.Id).ToArray())) : 
                        new List<REL_EVENTS_CONTACTS>();
                    var orcomments = (!comments.NullOrEmpty()) ?
                        db.Select<REL_EVENTS_COMMENTS>(q => Sql.In(q.Uid, entities.Select(x => x.Id)) && Sql.In(q.CommentId, comments.Select(x => x.Id).ToArray())) : 
                        new List<REL_EVENTS_COMMENTS>();
                    var orrdates = (!rdates.NullOrEmpty()) ?
                        db.Select<REL_EVENTS_RDATES>(q => Sql.In(q.Uid, entities.Select(x => x.Id)) && Sql.In(q.RecurrenceDateId, rdates.Select(x => x.Id).ToArray())) : 
                        new List<REL_EVENTS_RDATES>();
                    var orexdates = (!exdates.NullOrEmpty()) ?
                        db.Select<REL_EVENTS_EXDATES>(q => Sql.In(q.Uid, entities.Select(x => x.Id)) && Sql.In(q.ExceptionDateId, exdates.Select(x => x.Id).ToArray())) : 
                        new List<REL_EVENTS_EXDATES>();
                    var orrelateds = (!relateds.NullOrEmpty()) ?
                        db.Select<REL_EVENTS_RELATEDTOS>(q => Sql.In(q.Uid, entities.Select(x => x.Id)) && Sql.In(q.RelatedToId, relateds.Select(x => x.Id).ToArray())) : 
                        new List<REL_EVENTS_RELATEDTOS>();
                    var orresources = (!resources.NullOrEmpty()) ?
                        db.Select<REL_EVENTS_RESOURCES>(q => Sql.In(q.Uid, entities.Select(x => x.Id)) && Sql.In(q.ResourcesId, resources.Select(x => x.Id).ToArray())) : 
                        new List<REL_EVENTS_RESOURCES>();
                    var orreqstats = (!reqstats.NullOrEmpty()) ?
                        db.Select<REL_EVENTS_REQSTATS>(q => Sql.In(q.Uid, entities.Select(x => x.Id)) && Sql.In(q.ReqStatsId, reqstats.Select(x => x.Id).ToArray())) : 
                        new List<REL_EVENTS_REQSTATS>();
                    var oraalarms = (!raalarms.NullOrEmpty()) ?
                        db.Select<REL_EVENTS_AUDIO_ALARMS>(q => Sql.In(q.Uid, entities.Select(x => x.Id)) && Sql.In(q.AlarmId, aalarms.Select(x => x.Id).ToArray())) :
                        new List<REL_EVENTS_AUDIO_ALARMS>();
                    var ordalarms = (!rdalarms.NullOrEmpty()) ?
                        db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => Sql.In(q.Uid, entities.Select(x => x.Id)) && Sql.In(q.AlarmId, dalarms.Select(x => x.Id).ToArray())) :
                        new List<REL_EVENTS_DISPLAY_ALARMS>();
                    var orealarms = (!realarms.NullOrEmpty()) ?
                        db.Select<REL_EVENTS_EMAIL_ALARMS>(q => Sql.In(q.Uid, entities.Select(x => x.Id)) && Sql.In(q.AlarmId, ealarms.Select(x => x.Id).ToArray())) :
                        new List<REL_EVENTS_EMAIL_ALARMS>();


                    //5. save non-existing entity-details relations
                    if (!rorgs.NullOrEmpty() && rorgs.Except(ororgs).NullOrEmpty()) db.SaveAll(rorgs.Except(ororgs));
                    if (!rrids.NullOrEmpty() && rrids.Except(orrids).NullOrEmpty()) db.SaveAll(rrids.Except(orrids));
                    if (!rrrules.NullOrEmpty() && rrrules.Except(orrrules).NullOrEmpty()) db.SaveAll(rrrules.Except(orrrules));
                    if (!rattends.NullOrEmpty() && !rattends.Except(orattends).NullOrEmpty()) db.SaveAll(rattends.Except(orattends));
                    if (!rattachbins.NullOrEmpty() && !rattachbins.Except(orattachbins).NullOrEmpty()) db.SaveAll(rattachbins.Except(orattachbins));
                    if (!rattachuris.NullOrEmpty() && !rattachuris.Except(orattachuris).NullOrEmpty()) db.SaveAll(rattachuris.Except(orattachuris));
                    if (!rcontacts.NullOrEmpty() && !rcontacts.Except(orcontacts).NullOrEmpty()) db.SaveAll(rcontacts.Except(orcontacts));
                    if (!rcomments.NullOrEmpty() && !rcomments.Except(orcomments).NullOrEmpty()) db.SaveAll(rcomments.Except(orcomments));
                    if (!rrdates.NullOrEmpty() && !rrdates.Except(orrdates).NullOrEmpty()) db.SaveAll(rrdates.Except(orrdates));
                    if (!rexdates.NullOrEmpty() && !rexdates.Except(orexdates).NullOrEmpty()) db.SaveAll(rexdates.Except(orexdates));
                    if (!rrelateds.NullOrEmpty() && !rrelateds.Except(orrelateds).NullOrEmpty()) db.SaveAll(rrelateds.Except(orrelateds));
                    if (!rresources.NullOrEmpty() && !rresources.Except(orresources).NullOrEmpty()) db.SaveAll(rresources.Except(orresources));
                    if (!rreqstats.NullOrEmpty() && !rreqstats.Except(orreqstats).NullOrEmpty()) db.SaveAll(rreqstats.Except(orreqstats));
                    if (!raalarms.NullOrEmpty() && !raalarms.Except(oraalarms).NullOrEmpty()) db.SaveAll(raalarms.Except(oraalarms));
                    if (!rdalarms.NullOrEmpty() && !rdalarms.Except(ordalarms).NullOrEmpty()) db.SaveAll(rdalarms.Except(ordalarms));
                    if (!realarms.NullOrEmpty() && !realarms.Except(orealarms).NullOrEmpty()) db.SaveAll(realarms.Except(orealarms));

            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public void EraseAll(IEnumerable<string> keys)
        {
            try
            {
                db.Delete<VEVENT>(q => Sql.In(q.Uid, keys.ToArray()));
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public void EraseAll()
        {
            try
            {
                db.DeleteAll<VEVENT>();
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public VEVENT Hydrate(VEVENT dry)
        {
            try
            {
                var orgs = db.Select<ORGANIZER, VEVENT, REL_EVENTS_ORGANIZERS>(
                    r => r.OrganizerId,
                    r => r.Uid,
                    e => e.Id == dry.Uid);
                if (!orgs.NullOrEmpty()) dry.Organizer = orgs.FirstOrDefault();

                var rids = db.Select<RECURRENCE_ID, VEVENT, REL_EVENTS_RECURRENCE_IDS>(
                    r => r.RecurrenceId_Id,
                    r => r.Uid,
                    e => e.Id == dry.Uid);
                if (!rids.NullOrEmpty()) dry.RecurrenceId = rids.FirstOrDefault();

                var rrules = db.Select<RECUR, VEVENT, REL_EVENTS_RRULES>(
                    r => r.RecurrenceRuleId, 
                    r => r.Uid, 
                    e => e.Id == dry.Uid);
                if (!rrules.NullOrEmpty()) dry.RecurrenceRule = rrules.FirstOrDefault();

               dry.Attachments.AddRange(db.Select<ATTACH_BINARY, VEVENT, REL_EVENTS_ATTACHBINS>(
                   r => r.AttachmentId,
                   r => r.Uid,
                   e => e.Id == dry.Uid).Except(dry.Attachments.OfType<ATTACH_BINARY>(), new EqualByStringId<ATTACH_BINARY>()));

               dry.Attachments.AddRange(db.Select<ATTACH_URI, VEVENT, REL_EVENTS_ATTACHBINS>(
                   r => r.AttachmentId,
                   r => r.Uid,
                   e => e.Id == dry.Uid).Except(dry.Attachments.OfType<ATTACH_URI>()));

                dry.Attendees.AddRange(db.Select<ATTENDEE, VEVENT, REL_EVENTS_ATTENDEES>(
                    r => r.AttendeeId,
                    r => r.Uid,
                    e => e.Id == dry.Uid).Except(dry.Attendees.OfType<ATTENDEE>(), new EqualByStringId<ATTENDEE>()));

                dry.Comments.AddRange(db.Select<COMMENT, VEVENT, REL_EVENTS_COMMENTS>(
                    r => r.CommentId,
                    r => r.Uid,
                    e => e.Id == dry.Uid).Except(dry.Comments.OfType<COMMENT>()));

                dry.Contacts.AddRange(db.Select<CONTACT, VEVENT, REL_EVENTS_CONTACTS >(
                    r => r.ContactId,
                    r => r.Uid,
                    e => e.Id == dry.Uid).Except(dry.Contacts.OfType<CONTACT>(), new EqualByStringId<CONTACT>()));

                dry.RecurrenceDates.AddRange(db.Select<RDATE, VEVENT, REL_EVENTS_RDATES>(
                    r => r.RecurrenceDateId, 
                    r => r.Uid,
                    e => e.Id == dry.Uid).Except(dry.RecurrenceDates.OfType<RDATE>()));

                dry.ExceptionDates.AddRange(db.Select<EXDATE, VEVENT, REL_EVENTS_EXDATES>(
                    r => r.ExceptionDateId, 
                    r => r.Uid,
                    e => e.Id == dry.Uid).Except(dry.ExceptionDates.OfType<EXDATE>(), new EqualByStringId<EXDATE>()));

                dry.RelatedTos.AddRange(db.Select<RELATEDTO, VEVENT, REL_EVENTS_RELATEDTOS>(
                    r => r.RelatedToId, 
                    r => r.Uid,
                    e => e.Id == dry.Uid).Except(dry.RelatedTos.OfType<RELATEDTO>(), new EqualByStringId<RELATEDTO>()));

                dry.RequestStatuses.AddRange(db.Select<REQUEST_STATUS, VEVENT, REL_EVENTS_REQSTATS>(
                    r => r.ReqStatsId, 
                    r => r.Uid, 
                    e => e.Id == dry.Uid).Except(dry.RequestStatuses.OfType<REQUEST_STATUS>(), new EqualByStringId<REQUEST_STATUS>()));

                dry.Resources.AddRange(db.Select<RESOURCES, VEVENT, REL_EVENTS_RESOURCES>(
                    r => r.ResourcesId,
                    r => r.Uid,
                    e => e.Id == dry.Uid).Except(dry.Resources.OfType<RESOURCES>(), new EqualByStringId<RESOURCES>()));


                dry.Alarms.AddRange(this.AudioAlarmRepository.Find(dry.Uid.ToSingleton()));
                dry.Alarms.AddRange(this.DisplayAlarmRepository.Find(dry.Uid.ToSingleton()));
                dry.Alarms.AddRange(this.EmailAlarmRepository.Find(dry.Uid.ToSingleton()));

            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }

            return dry;

        }

        public IEnumerable<VEVENT> Hydrate(IEnumerable<VEVENT> dry)
        {
            IEnumerable<VEVENT> full = null;
            try
            {
                //1. retrieve relationships
                var uids = dry.Select(q => q.Uid).ToArray();
                var rorgs = db.Select<REL_EVENTS_ORGANIZERS>(q => Sql.In(q.Uid, uids));
                var rrids = db.Select<REL_EVENTS_RECURRENCE_IDS>(q => Sql.In(q.Uid, uids));
                var rrrules = db.Select<REL_EVENTS_RRULES>(q => Sql.In(q.Uid, uids));
                var rattends = db.Select<REL_EVENTS_ATTENDEES>(q => Sql.In(q.Uid, uids));
                var rcomments = db.Select<REL_EVENTS_COMMENTS>(q => Sql.In(q.Uid, uids));
                var rattachs = db.Select<REL_EVENTS_ATTACHBINS>(q => Sql.In(q.Uid, uids));
                var rcontacts = db.Select<REL_EVENTS_CONTACTS>(q => Sql.In(q.Uid, uids));
                var rexdates = db.Select<REL_EVENTS_EXDATES>(q => Sql.In(q.Uid, uids));
                var rrdates = db.Select<REL_EVENTS_RDATES>(q => Sql.In(q.Uid, uids));
                var rrelateds = db.Select<REL_EVENTS_RELATEDTOS>(q => Sql.In(q.Uid, uids));
                var rrstats = db.Select<REL_EVENTS_REQSTATS>(q => Sql.In(q.Uid, uids));
                var rresources = db.Select<REL_EVENTS_RESOURCES>(q => Sql.In(q.Uid, uids));

                //2. retrieve secondary entities
                var orgs = (!rorgs.Empty())? db.Select<ORGANIZER>(q => Sql.In(q.Id, rorgs.Select(r => r.OrganizerId).ToArray())): null;
                var rids = (!rrids.Empty()) ? db.Select<RECURRENCE_ID>(q => Sql.In(q.Id, rrids.Select(r => r.RecurrenceId_Id).ToArray())): null;
                var rrules = (!rrrules.Empty()) ? db.Select<RECUR>(q => Sql.In(q.Id, rrrules.Select(r => r.RecurrenceRuleId).ToArray())): null;
                var attends = (!rattends.Empty()) ? db.Select<ATTENDEE>(q => Sql.In(q.Id, rattends.Select(r => r.AttendeeId).ToArray())): null;
                var comments = (!rcomments.Empty()) ? db.Select<COMMENT>(q => Sql.In(q.Id, rcomments.Select(r => r.CommentId).ToArray())): null;
                var attachbins = (!rattachs.Empty()) ? db.Select<ATTACH_BINARY>(q => Sql.In(q.Id, rattachs.Select(r => r.AttachmentId).ToArray())): null;
                var attachuris = (!rattachs.Empty()) ? db.Select<ATTACH_URI>(q => Sql.In(q.Id, rattachs.Select(r => r.AttachmentId).ToArray())) : null;
                var contacts = (!rcontacts.Empty()) ? db.Select<CONTACT>(q => Sql.In(q.Id, rcontacts.Select(r => r.ContactId).ToArray())) : null;
                var exdates = (!rexdates.Empty()) ? db.Select<EXDATE>(q => Sql.In(q.Id, rexdates.Select(r => r.ExceptionDateId).ToArray())) : null;
                var rdates = (!rrdates.Empty()) ? db.Select<RDATE>(q => Sql.In(q.Id, rrdates.Select(r => r.RecurrenceDateId).ToArray())) : null;
                var relatedtos = (!rrelateds.Empty()) ? db.Select<RELATEDTO>(q => Sql.In(q.Id, rrelateds.Select(r => r.RelatedToId).ToArray())) : null;
                var reqstats = (!rrstats.Empty()) ? db.Select<REQUEST_STATUS>(q => Sql.In(q.Id, rrstats.Select(r => r.ReqStatsId).ToArray())) : null;
                var resources = (!rresources.Empty()) ? db.Select<RESOURCES>(q => Sql.In(q.Id, rresources.Select(r => r.ResourcesId).ToArray())) : null;

                //3. Use Linq to stitch secondary entities to primary entities
                full = dry.Select(x =>
                {
                    var xorgs = (!orgs.NullOrEmpty())?(from y in orgs join r in rorgs on y.Id equals r.OrganizerId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y): null;
                    if (!xorgs.NullOrEmpty()) x.Organizer = xorgs.FirstOrDefault();

                    var xrrids = (!rids.NullOrEmpty())? (from y in rids join r in rrids on y.Id equals r.RecurrenceId_Id join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y): null;
                    if (!rrids.NullOrEmpty()) x.RecurrenceId = xrrids.FirstOrDefault();

                    var xrrules = (!rrules.NullOrEmpty()) ?(from y in rrules join r in rrrules on y.Id equals r.RecurrenceRuleId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y): null;
                    if (!xrrules.NullOrEmpty()) x.RecurrenceRule = xrrules.FirstOrDefault();

                    var xcomments = (comments != null)?(from y in comments join r in rcomments on y.Id equals r.CommentId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y): null;
                    if(!xcomments.NullOrEmpty()) x.Comments.AddRange(xcomments.Except(x.Comments.OfType<COMMENT>(), new EqualByStringId<COMMENT>()));

                    var xattendees = (!attends.NullOrEmpty())?(from y in attends join r in rattends on y.Id equals r.AttendeeId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y): null;
                    if(!xattendees.NullOrEmpty()) x.Attendees.AddRange(xattendees.Except(x.Attendees.OfType<ATTENDEE>(), new EqualByStringId<ATTENDEE>()));

                    var xattachbins = (!attachbins.NullOrEmpty())?(from y in attachbins join r in rattachs on y.Id equals r.AttachmentId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y): null;
                    var xattachuris = (!attachuris.NullOrEmpty())?(from y in attachuris join r in rattachs on y.Id equals r.AttachmentId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y): null;

                    if(!xattachbins.NullOrEmpty()) x.Attachments.AddRange(xattachbins.Except(x.Attachments.OfType<ATTACH_BINARY>(), new EqualByStringId<ATTACH_BINARY>()));
                    if(!xattachuris.NullOrEmpty()) x.Attachments.AddRange(xattachuris.Except(x.Attachments.OfType<ATTACH_URI>(), new EqualByStringId<ATTACH_URI>()));

                    var xcontacts = (!contacts.NullOrEmpty())? (from y in contacts join r in rcontacts on y.Id equals r.ContactId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y): null;
                    if(!xcontacts.NullOrEmpty()) x.Contacts.AddRange(xcontacts.Except(x.Contacts.OfType<CONTACT>(), new EqualByStringId<CONTACT>()));

                    var xrdates = (!rdates.NullOrEmpty())?(from y in rdates join r in rrdates on y.Id equals r.RecurrenceDateId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y): null;
                    if(!xrdates.NullOrEmpty())x.RecurrenceDates.AddRange(xrdates.Except(x.RecurrenceDates.OfType<RDATE>(), new EqualByStringId<RDATE>()));

                    var xexdates = (!exdates.NullOrEmpty())?(from y in exdates join r in rexdates on y.Id equals r.ExceptionDateId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y): null;
                    if(!xexdates.NullOrEmpty())x.ExceptionDates.AddRange(xexdates.Except(x.ExceptionDates.OfType<EXDATE>(), new EqualByStringId<EXDATE>()));

                    var xrelatedtos = (!relatedtos.NullOrEmpty())?(from y in relatedtos join r in rrelateds on y.Id equals r.RelatedToId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y): null;
                    if(!xrelatedtos.NullOrEmpty())x.RelatedTos.AddRange(xrelatedtos.Except(x.RelatedTos.OfType<RELATEDTO>(), new EqualByStringId<RELATEDTO>()));

                    var xreqstats = (!reqstats.NullOrEmpty())?(from y in reqstats join r in rrstats on y.Id equals r.ReqStatsId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y): null;
                    if(!xreqstats.NullOrEmpty())x.RequestStatuses.AddRange(xreqstats.Except(x.RequestStatuses.OfType<REQUEST_STATUS>(), new EqualByStringId<REQUEST_STATUS>()));

                    var xresources = (!resources.NullOrEmpty()) ? (from y in resources join r in rresources on y.Id equals r.ResourcesId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y) : null;
                    if (!xresources.NullOrEmpty()) x.Resources.AddRange(xresources.Except(x.Resources.OfType<RESOURCES>(), new EqualByStringId<RESOURCES>()));

                    x.Alarms.AddRange(this.AudioAlarmRepository.Find(uids));
                    x.Alarms.AddRange(this.DisplayAlarmRepository.Find(uids));
                    x.Alarms.AddRange(this.EmailAlarmRepository.Find(uids));

                    return x;
                });

            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }

            return full;
        }

        public bool Contains(string fkey, string pkey)
        {
            try
            {
                return !db.Select<REL_CALENDARS_EVENTS>(q => q.ProdId == fkey && q.Uid == pkey).NullOrEmpty();
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public bool Contains(IEnumerable<string> fkeys, IEnumerable<string> pkeys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            var found = false;
            try
            {
                var rels = db.Select<REL_CALENDARS_EVENTS>(q => Sql.In(q.ProdId, fkeys.ToArray()) && Sql.In(q.Uid, pkeys.ToArray()));
                switch (mode)
                {
                    case ExpectationMode.pessimistic: found = (!rels.NullOrEmpty()) ?
                        rels.Select(x => x.Uid).Distinct().Count() == pkeys.Distinct().Count() :
                        false; break;
                    case ExpectationMode.optimistic:
                    default:
                        found = !rels.NullOrEmpty(); break;
                }
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }

            return found;
        }
    }
}
