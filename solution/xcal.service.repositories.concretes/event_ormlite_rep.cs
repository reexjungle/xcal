using System;
using System.Data;
using System.Linq;
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
        }

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
            VEVENT sparse = null;
            try
            {
                sparse = db.Select<VEVENT>(q => q.Uid == key).FirstOrDefault();
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
            return (sparse != null) ? this.Rehydrate(sparse) : sparse;
        }

        public IEnumerable<VEVENT> Find(IEnumerable<string> keys, int? page = null)
        {
            IEnumerable<VEVENT> sparse = null;
            try
            {
                sparse = db.Select<VEVENT>(q => Sql.In(q.Uid, keys.ToArray()), page, pages);
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
            return (!sparse.NullOrEmpty()) ? this.Rehydrate(sparse) : sparse;
        }

        public IEnumerable<VEVENT> Get(int? page = null)
        {
            IEnumerable<VEVENT> sparse = null;
            try
            {
                sparse = db.Select<VEVENT>(page, pages);
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
            return (!sparse.NullOrEmpty()) ? this.Rehydrate(sparse) : sparse;
        }

        public void Save(VEVENT entity)
        {
            throw new NotImplementedException();
        }

        public void Patch(VEVENT entity)
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

        public void PatchAll(IEnumerable<VEVENT> entities)
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

        public VEVENT Rehydrate(VEVENT sparse)
        {
            try
            {
                sparse.Organizer = db.Select<ORGANIZER, VEVENT, REL_EVENTS_ORGANIZERS>(
                    r => r.OrganizerId,
                    r => r.Uid,
                    e => e.Id == sparse.Uid).FirstOrDefault();

                sparse.RecurrenceId = db.Select<RECURRENCE_ID, VEVENT, REL_EVENTS_RECURRENCE_IDS>(
                    r => r.RecurrenceId_Id,
                    r => r.Uid,
                    e => e.Id == sparse.Uid).FirstOrDefault();

                sparse.RecurrenceRule = db.Select<RECUR, VEVENT, REL_EVENTS_RRULES>(
                    r => r.RecurrenceRuleId, 
                    r => r.Uid, 
                    e => e.Id == sparse.Uid).FirstOrDefault();

                sparse.Attachments.Clear();
                sparse.Attachments.AddRange(
                    db.Select<ATTACH_BINARY, VEVENT, REL_EVENTS_ATTACHMENTS>(
                    r => r.AttachmentId,
                    r => r.Uid,
                    e => e.Id == sparse.Uid));

                sparse.Attachments.AddRange(
                    db.Select<ATTACH_URI, VEVENT, REL_EVENTS_ATTACHMENTS>(
                    r => r.AttachmentId,
                    r => r.Uid,
                    e => e.Id == sparse.Uid));

                sparse.Attendees.Clear();
                sparse.Attendees.AddRange(
                    db.Select<ATTENDEE, VEVENT, REL_EVENTS_ATTENDEES>(
                    r => r.AttendeeId,
                    r => r.Uid,
                    e => e.Id == sparse.Uid));

                sparse.Comments.Clear();
                sparse.Comments.AddRange(db.Select<COMMENT, VEVENT, REL_EVENTS_COMMENTS>(
                    r => r.CommentId, 
                    r => r.Uid, 
                    e => e.Id == sparse.Uid));

                sparse.Contacts.Clear();
                sparse.Contacts.AddRange(db.Select<CONTACT, VEVENT, REL_EVENTS_CONTACTS >(
                    r => r.ContactId,
                    r => r.Uid,
                    e => e.Id == sparse.Uid));

                sparse.RecurrenceDates.Clear();
                sparse.RecurrenceDates.AddRange(db.Select<RDATE, VEVENT, REL_EVENTS_RDATES>(
                    r => r.RecurrenceDateId, 
                    r => r.Uid, 
                    e => e.Id == sparse.Uid));

                sparse.ExceptionDates.Clear();
                sparse.ExceptionDates.AddRange(db.Select<EXDATE, VEVENT, REL_EVENTS_EXDATES>(
                    r => r.ExceptionDateId, 
                    r => r.Uid, 
                    e => e.Id == sparse.Uid));

                sparse.RelatedTos.Clear();
                sparse.RelatedTos.AddRange(db.Select<RELATEDTO, VEVENT, REL_EVENTS_RELATEDTOS>(
                    r => r.RelatedToId, 
                    r => r.Uid, 
                    e => e.Id == sparse.Uid));

                sparse.RequestStatuses.Clear();
                sparse.RequestStatuses.AddRange(db.Select<REQUEST_STATUS, VEVENT, REL_EVENTS_REQSTATS>(
                    r => r.ReqStatId, 
                    r => r.Uid, 
                    e => e.Id == sparse.Uid));

                sparse.Alarms.Clear();
                sparse.Alarms.AddRange(db.Select<AUDIO_ALARM, VEVENT, REL_EVENTS_ALARMS>(
                    r => r.AlarmId,
                    r => r.Uid,
                    e => e.Id == sparse.Uid));

                sparse.Alarms.AddRange(db.Select<DISPLAY_ALARM, VEVENT, REL_EVENTS_ALARMS>(
                    r => r.AlarmId,
                    r => r.Uid,
                    e => e.Id == sparse.Uid));

                sparse.Alarms.AddRange(db.Select<EMAIL_ALARM, VEVENT, REL_EVENTS_ALARMS>(
                    r => r.AlarmId,
                    r => r.Uid,
                    e => e.Id == sparse.Uid));

            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }

            return sparse;

        }

        public IEnumerable<VEVENT> Rehydrate(IEnumerable<VEVENT> dry)
        {
            IEnumerable<VEVENT> full = null;
            try
            {
                //get relationships
                var uids = dry.Select(q => q.Uid).ToArray();
                var rorgs = db.Select<REL_EVENTS_ORGANIZERS>(q => Sql.In(q.Uid, uids));
                var rrids = db.Select<REL_EVENTS_RECURRENCE_IDS>(q => Sql.In(q.Uid, uids));
                var rrrules = db.Select<REL_EVENTS_RRULES>(q => Sql.In(q.Uid, uids));
                var rattends = db.Select<REL_EVENTS_ATTENDEES>(q => Sql.In(q.Uid, uids));
                var rcomments = db.Select<REL_EVENTS_COMMENTS>(q => Sql.In(q.Uid, uids));
                var rattachs = db.Select<REL_EVENTS_ATTACHMENTS>(q => Sql.In(q.Uid, uids));
                var rcontacts = db.Select<REL_EVENTS_CONTACTS>(q => Sql.In(q.Uid, uids));
                var rexdates = db.Select<REL_EVENTS_EXDATES>(q => Sql.In(q.Uid, uids));
                var rrdates = db.Select<REL_EVENTS_RDATES>(q => Sql.In(q.Uid, uids));
                var rrelateds = db.Select<REL_EVENTS_RELATEDTOS>(q => Sql.In(q.Uid, uids));
                var rrstats = db.Select<REL_EVENTS_REQSTATS>(q => Sql.In(q.Uid, uids));
                var ralarms = db.Select<REL_EVENTS_ALARMS>(q => Sql.In(q.Uid, uids));

                //get properties
                var organizers = (!rorgs.Empty()) ? db.Select<ORGANIZER>(q => Sql.In(q.Id, rorgs.Select(r => r.OrganizerId).ToArray())) : db.Select<ORGANIZER>();
                var rids = (!rrids.Empty()) ? db.Select<RECURRENCE_ID>(q => Sql.In(q.Id, rrids.Select(r => r.RecurrenceId_Id).ToArray())) : db.Select<RECURRENCE_ID>();
                var rrules = (!rrrules.Empty()) ? db.Select<RECUR>(q => Sql.In(q.Id, rrrules.Select(r => r.RecurrenceRuleId).ToArray())) : db.Select<RECUR>();
                var attendees = (!rattends.Empty()) ? db.Select<ATTENDEE>(q => Sql.In(q.Id, rattends.Select(r => r.AttendeeId).ToArray())) : db.Select<ATTENDEE>();
                var comments = (!rcomments.Empty()) ? db.Select<COMMENT>(q => Sql.In(q.Id, rcomments.Select(r => r.CommentId).ToArray())) : db.Select<COMMENT>();
                var attachbinaries = (!rattachs.Empty()) ? db.Select<ATTACH_BINARY>(q => Sql.In(q.Id, rattachs.Select(r => r.AttachmentId).ToArray())) : db.Select<ATTACH_BINARY>();
                var attachuris = (!rattachs.Empty()) ? db.Select<ATTACH_URI>(q => Sql.In(q.Id, rattachs.Select(r => r.AttachmentId).ToArray())) : db.Select<ATTACH_URI>();
                var contacts = (!rcontacts.Empty()) ? db.Select<CONTACT>(q => Sql.In(q.Id, rcontacts.Select(r => r.ContactId).ToArray())) : db.Select<CONTACT>();
                var exdates = (!rexdates.Empty()) ? db.Select<EXDATE>(q => Sql.In(q.Id, rexdates.Select(r => r.ExceptionDateId).ToArray())) : db.Select<EXDATE>();
                var rdates = (!rrdates.Empty()) ? db.Select<RDATE>(q => Sql.In(q.Id, rrdates.Select(r => r.RecurrenceDateId).ToArray())) : db.Select<RDATE>();
                var relatedtos = (!rrelateds.Empty()) ? db.Select<RELATEDTO>(q => Sql.In(q.Id, rrelateds.Select(r => r.RelatedToId).ToArray())) : db.Select<RELATEDTO>();
                var reqstatuses = (!rrstats.Empty()) ? db.Select<REQUEST_STATUS>(q => Sql.In(q.Id, rrstats.Select(r => r.ReqStatId).ToArray())) : db.Select<REQUEST_STATUS>();
                var raalarms = (!ralarms.Empty()) ? db.Select<AUDIO_ALARM>(q => Sql.In(q.Id, ralarms.Select(r => r.AlarmId))) : db.Select<AUDIO_ALARM>();

                //Use Linq to stitch all
                full = dry.Select(x =>
                {
                    var xorgs = (from y in organizers join r in rorgs on y.Id equals r.OrganizerId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y);
                    if (!xorgs.NullOrEmpty()) x.Organizer = xorgs.FirstOrDefault();

                    var xrrids = (from y in rids join r in rrids on y.Id equals r.RecurrenceId_Id join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y);
                    if (!rrids.NullOrEmpty()) x.RecurrenceId = xrrids.FirstOrDefault();

                    var xrrules = (from y in rrules join r in rrrules on y.Id equals r.RecurrenceRuleId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y);
                    if (!xrrules.NullOrEmpty()) x.RecurrenceRule = xrrules.FirstOrDefault();

                    var xcomments = from y in comments join r in rcomments on y.Id equals r.CommentId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y;
                    x.Comments.AddRange(xcomments.Except(x.Comments.OfType<COMMENT>()));

                    var xattendees = from y in attendees join r in rattends on y.Id equals r.AttendeeId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y;
                    x.Attendees.AddRange(xattendees.Except(x.Attendees.OfType<ATTENDEE>()));

                    var xattachbins = from y in attachbinaries join r in rattachs on y.Id equals r.AttachmentId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y;
                    var xattachuris = from y in attachuris join r in rattachs on y.Id equals r.AttachmentId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y;
                    x.Attachments.AddRange(xattachbins.Except(x.Attachments.OfType<ATTACH_BINARY>()));
                    x.Attachments.AddRange(xattachuris.Except(x.Attachments.OfType<ATTACH_URI>()));

                    var xcontacts = from y in contacts join r in rcontacts on y.Id equals r.ContactId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y;
                    x.Contacts.AddRange(xcontacts.Except(x.Contacts.OfType<CONTACT>()));

                    var xrdates = from y in rdates join r in rrdates on y.Id equals r.RecurrenceDateId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y;
                    x.RecurrenceDates.AddRange(xrdates.Except(x.RecurrenceDates.OfType<RDATE>()));

                    var xexdates = from y in exdates join r in rexdates on y.Id equals r.ExceptionDateId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y;
                    x.ExceptionDates.AddRange(xexdates.Except(x.ExceptionDates.OfType<EXDATE>()));

                    var xrelatedtos = from y in relatedtos join r in rrelateds on y.Id equals r.RelatedToId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y;
                    x.RelatedTos.AddRange(xrelatedtos.Except(x.RelatedTos.OfType<RELATEDTO>()));

                    var xreqtats = from y in reqstatuses join r in rrstats on y.Id equals r.ReqStatId join e in dry on r.Uid equals e.Uid where e.Uid == x.Uid select y;
                    x.RequestStatuses.AddRange(xreqtats.Except(x.RequestStatuses.OfType<REQUEST_STATUS>()));

                    return x;
                });
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }

            return full;
        }

        public VEVENT Dehydrate(VEVENT full)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VEVENT> Dehydrate(IEnumerable<VEVENT> dry)
        {
            throw new NotImplementedException();
        }
    }
}
