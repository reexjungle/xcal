using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using ServiceStack.OrmLite;
using reexmonkey.infrastructure.ormlite.extensions;
using reexmonkey.crosscut.essentials.concretes;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.service.repositories.contracts;

namespace reexmonkey.xcal.service.repositories.concretes
{
    /// <summary>
    /// Reüpresents a a repository of events connected to an ORMlite source
    /// </summary>
    public class EventOrmLiteRepository: IEventOrmLiteRepository
    {
        private IDbConnection conn;
        private IDbConnectionFactory factory = null;
        private int? pages = null;

        private IDbConnection db
        {
            get { return (this.conn) ?? factory.OpenDbConnection(); }
        }
        public IDbConnectionFactory DbConnectionFactory
        {
            get { return this.factory; }
            set { this.factory = value; }
        }
        public IAudioAlarmOrmLiteRepository AudioAlarmOrmLiteRepository { get; set; }
        public IDisplayAlarmOrmLiteRepository DisplayAlarmOrmLiteRepository { get; set; }
        public IEmailAlarmOrmLiteRepository EmailAlarmOrmLiteRepository { get; set; }
        public int? Pages
        {
            get { return this.pages; }
        }

        public EventOrmLiteRepository(IDbConnectionFactory factory, int? pages)
        {
            if (factory == null) throw new ArgumentNullException("Null factory");
            this.factory = factory;
            if (pages == null) throw new ArgumentNullException("Null pages");
            this.pages = pages;
            this.conn = factory.OpenDbConnection();
        }

        //cleanup
        public virtual void Dispose()
        {
            if (this.conn != null) this.conn.Dispose();
        }

        public VEVENT Find(string key)
        {
            VEVENT dry = null;
            try
            {
                dry = db.Select<VEVENT>(q => q.Uid == key).FirstOrDefault();
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
            return (dry != null) ? this.Hydrate(dry) : dry;
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
            return (!dry.NullOrEmpty()) ? this.Hydrate(dry) : dry;
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

            return (!dry.NullOrEmpty()) ? this.Hydrate(dry) : dry;
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
                if (!aalarms.NullOrEmpty()) this.AudioAlarmOrmLiteRepository.SaveAll(aalarms);
                if (!dalarms.NullOrEmpty()) this.DisplayAlarmOrmLiteRepository.SaveAll(dalarms);
                if (!ealarms.NullOrEmpty()) this.EmailAlarmOrmLiteRepository.SaveAll(ealarms);

                //3. construct relations from entity details
                var rorg = (entity.Organizer != null && entity.Organizer is ORGANIZER) ?
                    (new REL_EVENTS_ORGANIZERS { Uid = entity.Id, OrganizerId = (entity.Organizer as ORGANIZER).Id }) : null;
                var rrid = (entity.RecurrenceId != null && entity.RecurrenceId is RECURRENCE_ID) ?
                    (new REL_EVENTS_RECURRENCE_IDS { Uid = entity.Id, RecurrenceId_Id = (entity.RecurrenceId as RECURRENCE_ID).Id }) : null;
                var rrrule = (entity.RecurrenceRule != null && entity.RecurrenceRule is RECUR) ?
                    (new REL_EVENTS_RRULES { Uid = entity.Id, RecurrenceRuleId = (entity.RecurrenceRule as RECUR).Id }) : null;
                var rattends = (entity.Attendees.OfType<ATTENDEE>().Count() > 0) ? entity.Attendees.OfType<ATTENDEE>()
                    .Select(x => new REL_EVENTS_ATTENDEES { Uid = entity.Id, AttendeeId = x.Id }) : null;
                var rattachbins = (entity.Attachments.OfType<ATTACH_BINARY>().Count() > 0) ?
                    (entity.Attachments.OfType<ATTACH_BINARY>().Select(x => new REL_EVENTS_ATTACHBINS { Uid = entity.Id, AttachmentId = x.Id })) : null;
                var rattachuris = (entity.Attachments.OfType<ATTACH_URI>().Count() > 0) ?
                    (entity.Attachments.OfType<ATTACH_URI>().Select(x => new REL_EVENTS_ATTACHURIS { Uid = entity.Id, AttachmentId = x.Id })) : null;
                var rcontacts = (entity.Contacts.OfType<CONTACT>().Count() > 0) ?
                    (entity.Contacts.OfType<CONTACT>().Select(x => new REL_EVENTS_CONTACTS { Uid = entity.Id, ContactId = x.Id })) : null;
                var rcomments = (entity.Contacts.OfType<COMMENT>().Count() > 0) ?
                    (entity.Contacts.OfType<COMMENT>().Select(x => new REL_EVENTS_COMMENTS { Uid = entity.Id, CommentId = x.Id })) : null;
                var rrdates = (entity.RecurrenceDates.OfType<RDATE>().Count() > 0) ?
                    (entity.RecurrenceDates.OfType<RDATE>().Select(x => new REL_EVENTS_RDATES { Uid = entity.Id, RecurrenceDateId = x.Id })) : null;
                var rexdates = (entity.ExceptionDates.OfType<EXDATE>().Count() > 0) ?
                    (entity.ExceptionDates.OfType<EXDATE>().Select(x => new REL_EVENTS_EXDATES { Uid = entity.Id, ExceptionDateId = x.Id })) : null;
                var rrelateds = (entity.RelatedTos.OfType<RELATEDTO>().Count() > 0) ?
                    (entity.RelatedTos.OfType<RELATEDTO>().Select(x => new REL_EVENTS_RELATEDTOS { Uid = entity.Id, RelatedToId = x.Id })) : null;
                var rresources = (entity.Resources.OfType<RESOURCES>().Count() > 0) ?
                    (entity.Resources.OfType<RESOURCES>().Select(x => new REL_EVENTS_RESOURCES { Uid = entity.Id, ResourcesId = x.Id })) : null;
                var rreqstats = (entity.RequestStatuses.OfType<REQUEST_STATUS>().Count() > 0) ?
                    (entity.RequestStatuses.OfType<REQUEST_STATUS>().Select(x => new REL_EVENTS_REQSTATS { Uid = entity.Id, ReqStatsId = x.Id })) : null;
                var raalarms = (entity.Alarms.OfType<AUDIO_ALARM>().Count() > 0) ?
                    (entity.Alarms.OfType<AUDIO_ALARM>().Select(x => new REL_EVENTS_AUDIO_ALARMS { Uid = entity.Id, AlarmId = x.Id })) : null;
                var rdalarms = (entity.Alarms.OfType<DISPLAY_ALARM>().Count() > 0) ?
                    (entity.Alarms.OfType<DISPLAY_ALARM>().Select(x => new REL_EVENTS_DISPLAY_ALARMS { Uid = entity.Id, AlarmId = x.Id })) : null;
                var realarms = (entity.Alarms.OfType<EMAIL_ALARM>().Count() > 0) ?
                    (entity.Alarms.OfType<EMAIL_ALARM>().Select(x => new REL_EVENTS_EMAIL_ALARMS { Uid = entity.Id, AlarmId = x.Id })) : null;
                
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
                if (!rattends.NullOrEmpty() && orattends.Intersect(rattends).NullOrEmpty()) db.SaveAll(rattends);
                if(!rattachbins.NullOrEmpty() && orattachbins.Intersect(rattachbins).NullOrEmpty()) db.SaveAll(rattachbins);
                if(!rattachuris.NullOrEmpty() && orattachuris.Intersect(rattachuris).NullOrEmpty()) db.SaveAll(rattachuris);
                if (!rcontacts.NullOrEmpty() && orcontacts.Intersect(rcontacts).NullOrEmpty()) db.SaveAll(rcontacts);
                if (!rcomments.NullOrEmpty() && orcomments.Intersect(rcomments).NullOrEmpty()) db.SaveAll(rcomments);
                if (!rrdates.NullOrEmpty() && orrdates.Intersect(rrdates).NullOrEmpty()) db.SaveAll(rrdates);
                if (!rexdates.NullOrEmpty() && orexdates.Intersect(rexdates).NullOrEmpty()) db.SaveAll(rexdates);
                if (!rrelateds.NullOrEmpty() && orrelateds.Intersect(rrelateds).NullOrEmpty()) db.SaveAll(rrelateds);
                if (!rresources.NullOrEmpty() && orresources.Intersect(rresources).NullOrEmpty()) db.SaveAll(rresources);
                if (!rreqstats.NullOrEmpty() && orreqstats.Intersect(rreqstats).NullOrEmpty()) db.SaveAll(rreqstats);
                if (!raalarms.NullOrEmpty() && oraalarms.Intersect(raalarms).NullOrEmpty()) db.SaveAll(raalarms);
                if (!rdalarms.NullOrEmpty() && ordalarms.Intersect(rdalarms).NullOrEmpty()) db.SaveAll(rdalarms);
                if (!realarms.NullOrEmpty() && orealarms.Intersect(realarms).NullOrEmpty()) db.SaveAll(realarms);
            }
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
                    if (!aalarms.NullOrEmpty()) db.SaveAll(aalarms);
                    if (!dalarms.NullOrEmpty()) db.SaveAll(dalarms);
                    if (!ealarms.NullOrEmpty()) db.SaveAll(ealarms);

                    //3. construct available relations
                    var rorgs = entities.Where(x => x.Organizer != null && x.Organizer is ORGANIZER)
                        .Select(e => new REL_EVENTS_ORGANIZERS { Uid = e.Id, OrganizerId = (e.Organizer as ORGANIZER).Id });
                    var rrids = entities.Where(x => x.RecurrenceId != null && x.RecurrenceId is ORGANIZER)
                        .Select(e => new REL_EVENTS_RECURRENCE_IDS { Uid = e.Id, RecurrenceId_Id = (e.RecurrenceId as RECURRENCE_ID).Id });
                    var rrrules = entities.Where(x => x.RecurrenceRule != null && x.RecurrenceRule is RECUR)
                        .Select(e => new REL_EVENTS_RRULES { Uid = e.Id, RecurrenceRuleId = (e.RecurrenceRule as RECUR).Id });
                    var rattends = entities.Where(x => x.Attendees.OfType<ATTENDEE>().Count() > 0)
                        .SelectMany(e => e.Attendees.OfType<ATTENDEE>().Select(x => new REL_EVENTS_ATTENDEES { Uid = e.Id, AttendeeId = x.Id }));
                    var rattachbins = entities.Where(x => x.Attachments.OfType<ATTACH_BINARY>().Count() > 0)
                        .SelectMany(e => e.Attachments.OfType<ATTACH_BINARY>().Select(x => new REL_EVENTS_ATTACHBINS { Uid = e.Id, AttachmentId = x.Id }));
                    var rattachuris = entities.Where(x => x.Attachments.OfType<ATTACH_URI>().Count() > 0)
                        .SelectMany(e => e.Attachments.OfType<ATTACH_URI>().Select(x => new REL_EVENTS_ATTACHURIS { Uid = e.Id, AttachmentId = x.Id }));
                    var rcontacts = entities.Where(x => x.Contacts.OfType<CONTACT>().Count() > 0)
                        .SelectMany(e => e.Contacts.OfType<CONTACT>().Select(x => new REL_EVENTS_CONTACTS { Uid = e.Id, ContactId = x.Id }));
                    var rcomments = entities.Where(x => x.Contacts.OfType<COMMENT>().Count() > 0)
                        .SelectMany(e => e.Contacts.OfType<COMMENT>().Select(x => new REL_EVENTS_COMMENTS { Uid = e.Id, CommentId = x.Id })); 
                    var rrdates = entities.Where(x => x.RecurrenceDates.OfType<RDATE>().Count() > 0)
                        .SelectMany(e => e.RecurrenceDates.OfType<RDATE>().Distinct(new EqualByStringId<RDATE>())
                        .Select(x => new REL_EVENTS_RDATES { Uid = e.Id, RecurrenceDateId = x.Id })); 
                    var rexdates = entities.Where(x => x.ExceptionDates.OfType<EXDATE>().Count() > 0)
                        .SelectMany(e => e.ExceptionDates.OfType<EXDATE>().Select(x => new REL_EVENTS_EXDATES { Uid = e.Id, ExceptionDateId = x.Id })); 
                    var rrelateds = entities.Where(x => x.RelatedTos.OfType<RELATEDTO>().Count() > 0)
                        .SelectMany(e => e.RelatedTos.OfType<RELATEDTO>().Select(x => new REL_EVENTS_RELATEDTOS { Uid = e.Id, RelatedToId = x.Id }));
                    var rresources = entities.Where(x => x.Resources.OfType<RESOURCES>().Count() > 0)
                        .SelectMany(e => e.Resources.OfType<RESOURCES>().Select(x => new REL_EVENTS_RESOURCES { Uid = e.Id, ResourcesId = x.Id }));
                    var rreqstats = entities.Where(x => x.RequestStatuses.OfType<REQUEST_STATUS>().Count() > 0)
                        .SelectMany(e => e.RequestStatuses.OfType<REQUEST_STATUS>().Select(x => new REL_EVENTS_REQSTATS { Uid = e.Id, ReqStatsId = x.Id }));
                    var raalarms = entities.Where(x => x.Alarms.OfType<AUDIO_ALARM>().Count() > 0)
                        .SelectMany(e => e.RequestStatuses.OfType<AUDIO_ALARM>().Select(x => new REL_EVENTS_AUDIO_ALARMS { Uid = e.Id, AlarmId = x.Id }));
                    var rdalarms = entities.Where(x => x.Alarms.OfType<DISPLAY_ALARM>().Count() > 0)
                        .SelectMany(e => e.RequestStatuses.OfType<DISPLAY_ALARM>().Select(x => new REL_EVENTS_DISPLAY_ALARMS { Uid = e.Id, AlarmId = x.Id }));
                    var realarms = entities.Where(x => x.Alarms.OfType<EMAIL_ALARM>().Count() > 0)
                        .SelectMany(e => e.RequestStatuses.OfType<EMAIL_ALARM>().Select(x => new REL_EVENTS_EMAIL_ALARMS { Uid = e.Id, AlarmId = x.Id }));

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
                    if (!rorgs.NullOrEmpty() && ororgs.Intersect(rorgs).NullOrEmpty()) db.SaveAll(rorgs);
                    if (!rrids.NullOrEmpty() && orrids.Intersect(rrids).NullOrEmpty()) db.SaveAll(rrids);
                    if (!rrrules.NullOrEmpty() && orrrules.Intersect(rrrules).NullOrEmpty()) db.SaveAll(rrrules);
                    if (!rattends.NullOrEmpty() && orattends.Intersect(rattends).NullOrEmpty()) db.SaveAll(rattends);
                    if (!rattachbins.NullOrEmpty() && orattachbins.Intersect(rattachbins).NullOrEmpty()) db.SaveAll(rattachbins);
                    if (!rattachuris.NullOrEmpty() && orattachuris.Intersect(rattachuris).NullOrEmpty()) db.SaveAll(rattachuris);
                    if (!rcontacts.NullOrEmpty() && orcontacts.Intersect(rcontacts).NullOrEmpty()) db.SaveAll(rcontacts);
                    if (!rcomments.NullOrEmpty() && orcomments.Intersect(rcomments).NullOrEmpty()) db.SaveAll(rcomments);
                    if (!rrdates.NullOrEmpty() && orrdates.Intersect(rrdates).NullOrEmpty()) db.SaveAll(rrdates);
                    if (!rexdates.NullOrEmpty() && orexdates.Intersect(rexdates).NullOrEmpty()) db.SaveAll(rexdates);
                    if (!rrelateds.NullOrEmpty() && orrelateds.Intersect(rrelateds).NullOrEmpty()) db.SaveAll(rrelateds);
                    if (!rresources.NullOrEmpty() && orresources.Intersect(rresources).NullOrEmpty()) db.SaveAll(rresources);
                    if (!rreqstats.NullOrEmpty() && orreqstats.Intersect(rreqstats).NullOrEmpty()) db.SaveAll(rreqstats);
                    if (!raalarms.NullOrEmpty() && oraalarms.Intersect(raalarms).NullOrEmpty()) db.SaveAll(raalarms);
                    if (!rdalarms.NullOrEmpty() && ordalarms.Intersect(rdalarms).NullOrEmpty()) db.SaveAll(rdalarms);
                    if (!realarms.NullOrEmpty() && orealarms.Intersect(realarms).NullOrEmpty()) db.SaveAll(realarms);

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
                   e => e.Id == dry.Uid).Except(dry.Attachments.OfType<ATTACH_BINARY>()));

               dry.Attachments.AddRange(db.Select<ATTACH_URI, VEVENT, REL_EVENTS_ATTACHBINS>(
                   r => r.AttachmentId,
                   r => r.Uid,
                   e => e.Id == dry.Uid).Except(dry.Attachments.OfType<ATTACH_URI>()));

                dry.Attendees.AddRange(db.Select<ATTENDEE, VEVENT, REL_EVENTS_ATTENDEES>(
                    r => r.AttendeeId,
                    r => r.Uid,
                    e => e.Id == dry.Uid).Except(dry.Attendees.OfType<ATTENDEE>()));

                dry.Comments.AddRange(db.Select<COMMENT, VEVENT, REL_EVENTS_COMMENTS>(
                    r => r.CommentId,
                    r => r.Uid,
                    e => e.Id == dry.Uid).Except(dry.Comments.OfType<COMMENT>()));

                dry.Contacts.AddRange(db.Select<CONTACT, VEVENT, REL_EVENTS_CONTACTS >(
                    r => r.ContactId,
                    r => r.Uid,
                    e => e.Id == dry.Uid).Except(dry.Contacts.OfType<CONTACT>()));

                dry.RecurrenceDates.AddRange(db.Select<RDATE, VEVENT, REL_EVENTS_RDATES>(
                    r => r.RecurrenceDateId, 
                    r => r.Uid,
                    e => e.Id == dry.Uid).Except(dry.RecurrenceDates.OfType<RDATE>()));

                dry.ExceptionDates.AddRange(db.Select<EXDATE, VEVENT, REL_EVENTS_EXDATES>(
                    r => r.ExceptionDateId, 
                    r => r.Uid,
                    e => e.Id == dry.Uid).Except(dry.ExceptionDates.OfType<EXDATE>()));

                dry.RelatedTos.AddRange(db.Select<RELATEDTO, VEVENT, REL_EVENTS_RELATEDTOS>(
                    r => r.RelatedToId, 
                    r => r.Uid,
                    e => e.Id == dry.Uid).Except(dry.RelatedTos.OfType<RELATEDTO>()));

                dry.RequestStatuses.AddRange(db.Select<REQUEST_STATUS, VEVENT, REL_EVENTS_REQSTATS>(
                    r => r.ReqStatsId, 
                    r => r.Uid, 
                    e => e.Id == dry.Uid).Except(dry.RequestStatuses.OfType<REQUEST_STATUS>()));

                dry.Resources.AddRange(db.Select<RESOURCES, VEVENT, REL_EVENTS_RESOURCES>(
                    r => r.ResourcesId,
                    r => r.Uid,
                    e => e.Id == dry.Uid).Except(dry.Resources.OfType<RESOURCES>()));


                dry.Alarms.AddRange(db.Select<AUDIO_ALARM, VEVENT, REL_EVENTS_AUDIO_ALARMS>(
                    r => r.AlarmId,
                    r => r.Uid,
                    e => e.Id == dry.Uid).Except(dry.Alarms.OfType<AUDIO_ALARM>()));

                dry.Alarms.AddRange(db.Select<DISPLAY_ALARM, VEVENT, REL_EVENTS_AUDIO_ALARMS>(
                    r => r.AlarmId,
                    r => r.Uid,
                    e => e.Id == dry.Uid).Except(dry.Alarms.OfType<DISPLAY_ALARM>()));

                dry.Alarms.AddRange(db.Select<EMAIL_ALARM, VEVENT, REL_EVENTS_AUDIO_ALARMS>(
                    r => r.AlarmId,
                    r => r.Uid,
                    e => e.Id == dry.Uid).Except(dry.Alarms.OfType<EMAIL_ALARM>()));

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
                var ralarms = db.Select<REL_EVENTS_AUDIO_ALARMS>(q => Sql.In(q.Uid, uids));

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
                var aalarms = (!ralarms.Empty()) ? db.Select<AUDIO_ALARM>(q => Sql.In(q.Id, ralarms.Select(r => r.AlarmId))) : null;
                var dalarms = (!ralarms.Empty()) ? db.Select<DISPLAY_ALARM>(q => Sql.In(q.Id, ralarms.Select(r => r.AlarmId))) : null;
                var ealarms = (!ralarms.Empty()) ? db.Select<EMAIL_ALARM>(q => Sql.In(q.Id, ralarms.Select(r => r.AlarmId))) : null;

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

                    if(!xattachbins.NullOrEmpty()) x.Attachments.AddRange(xattachbins.Except(x.Attachments.OfType<ATTACH_BINARY>()));
                    if(!xattachuris.NullOrEmpty()) x.Attachments.AddRange(xattachuris.Except(x.Attachments.OfType<ATTACH_URI>()));

                    var xcontacts = (!contacts.NullOrEmpty())? (from y in contacts join r in rcontacts on y.Id equals r.ContactId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y): null;
                    if(!xcontacts.NullOrEmpty()) x.Contacts.AddRange(xcontacts.Except(x.Contacts.OfType<CONTACT>()));

                    var xrdates = (!rdates.NullOrEmpty())?(from y in rdates join r in rrdates on y.Id equals r.RecurrenceDateId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y): null;
                    if(!xrdates.NullOrEmpty())x.RecurrenceDates.AddRange(xrdates.Except(x.RecurrenceDates.OfType<RDATE>()));

                    var xexdates = (!exdates.NullOrEmpty())?(from y in exdates join r in rexdates on y.Id equals r.ExceptionDateId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y): null;
                    if(!xexdates.NullOrEmpty())x.ExceptionDates.AddRange(xexdates.Except(x.ExceptionDates.OfType<EXDATE>()));

                    var xrelatedtos = (!relatedtos.NullOrEmpty())?(from y in relatedtos join r in rrelateds on y.Id equals r.RelatedToId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y): null;
                    if(!xrelatedtos.NullOrEmpty())x.RelatedTos.AddRange(xrelatedtos.Except(x.RelatedTos.OfType<RELATEDTO>()));

                    var xreqstats = (!reqstats.NullOrEmpty())?(from y in reqstats join r in rrstats on y.Id equals r.ReqStatsId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y): null;
                    if(!xreqstats.NullOrEmpty())x.RequestStatuses.AddRange(xreqstats.Except(x.RequestStatuses.OfType<REQUEST_STATUS>()));

                    var xresources = (!resources.NullOrEmpty()) ? (from y in resources join r in rresources on y.Id equals r.ResourcesId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y) : null;
                    if (!xresources.NullOrEmpty()) x.Resources.AddRange(xresources.Except(x.Resources.OfType<RESOURCES>()));

                    var xaalarms = (!aalarms.NullOrEmpty())?(from y in aalarms join r in ralarms on y.Id equals r.AlarmId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y): null;
                    if (!xaalarms.NullOrEmpty()) x.Alarms.AddRange(xaalarms.Except(x.Alarms.OfType<AUDIO_ALARM>()));

                    var xdalarms = (!dalarms.NullOrEmpty()) ? (from y in dalarms join r in ralarms on y.Id equals r.AlarmId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y) : null;
                    if (!xdalarms.NullOrEmpty()) x.Alarms.AddRange(xdalarms.Except(x.Alarms.OfType<DISPLAY_ALARM>()));

                    var xealarms = (!ealarms.NullOrEmpty()) ? (from y in ealarms join r in ralarms on y.Id equals r.AlarmId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y) : null;
                    if (!xealarms.NullOrEmpty()) x.Alarms.AddRange(xealarms.Except(x.Alarms.OfType<EMAIL_ALARM>()));

                    return x;
                });
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }

            return full;
        }

    }
}
