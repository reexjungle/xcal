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
        private int? capacity = null;

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
        public int? Capacity
        {
            get { return this.capacity; }
            set 
            {
                if (value == null) throw new ArgumentNullException("Null pages");
                this.capacity = value; 
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
            this.Capacity = pages;
            this.conn = this.factory.OpenDbConnection();
        }
        public EventOrmLiteRepository(IDbConnection connection, int? pages)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            this.conn = connection; 
            this.Capacity = pages;
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
                dry = db.Select<VEVENT>(q => Sql.In(q.Uid, keys.ToArray()), page, capacity);
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
                    Conjunctor.AND, JoinMode.INNER, true, page, capacity); 
                }
                else
                {
                    dry = db.Select<VEVENT, VCALENDAR, REL_CALENDARS_EVENTS>(
                    r => r.Uid,
                    r => r.ProdId,
                    c => Sql.In(c.ProdId, fkeys.ToArray()),
                    JoinMode.INNER, true, page, capacity); 
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
                dry = db.Select<VEVENT>(page, capacity);
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }

            return (!dry.NullOrEmpty()) ? this.Hydrate(dry) : null;
        }

        public void Save(VEVENT entity)
        {

            #region retrieve attributes of entity

            var org = entity.Organizer as ORGANIZER;
            var rid = entity.RecurrenceId as RECURRENCE_ID;
            var rrule = entity.RecurrenceRule as RECUR;
            var attends = entity.Attendees.OfType<ATTENDEE>();
            var attachbins = entity.Attachments.OfType<ATTACH_BINARY>();
            var attachuris = entity.Attachments.OfType<ATTACH_URI>();
            var contacts = entity.Contacts.OfType<CONTACT>();
            var comments = entity.Comments.OfType<COMMENT>();
            var rdates = entity.RecurrenceDates.OfType<RDATE>();
            var exdates = entity.ExceptionDates.OfType<EXDATE>();
            var relateds = entity.RelatedTos.OfType<RELATEDTO>();
            var resources = entity.Resources.OfType<RESOURCES>();
            var reqstats = entity.RequestStatuses.OfType<REQUEST_STATUS>();
            var aalarms = entity.Alarms.OfType<AUDIO_ALARM>();
            var dalarms = entity.Alarms.OfType<DISPLAY_ALARM>();
            var ealarms = entity.Alarms.OfType<EMAIL_ALARM>(); 
            #endregion

            #region save non-aggregate attribbutes of entity

            using (var transaction = db.OpenTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    db.Save(entity, transaction);
                    if (org != null)
                    {
                        db.Save(org);
                        var rorg = new REL_EVENTS_ORGANIZERS { Id = this.KeyGenerator.GetNextKey(), EventId = entity.Id, OrganizerId = org.Id };
                        var ororgs = db.Select<REL_EVENTS_ORGANIZERS>(q => q.EventId == entity.Id && q.OrganizerId == org.Id);
                        if (!ororgs.NullOrEmpty() && !ororgs.Contains(rorg)) db.Save(rorg);
                    }
                    if (rid != null)
                    {
                        db.Save(rid, transaction);
                        var rrid = new REL_EVENTS_RECURRENCE_IDS { Id = this.KeyGenerator.GetNextKey(), EventId = entity.Id, RecurrenceId_Id = rid.Id };
                        var orrids = db.Select<REL_EVENTS_RECURRENCE_IDS>(q => q.EventId == entity.Id && q.RecurrenceId_Id == rid.Id);
                        if (rrid != null && !orrids.Contains(rrid)) db.Save(rrid);
                    }

                    if (rrule != null)
                    {
                        db.Save(rrule, transaction);
                        var rrrule = new REL_EVENTS_RRULES { Id = this.KeyGenerator.GetNextKey(), EventId = entity.Id, RecurrenceRuleId = rrule.Id };
                        var orrrules = db.Select<REL_EVENTS_RRULES>(q => q.EventId == entity.Id && q.RecurrenceRuleId == rrule.Id);
                        if (rrrule != null && !orrrules.Contains(rrrule)) db.Save(rrule);
                    }

                    if (!attends.NullOrEmpty())
                    {
                        db.SaveAll(attends, transaction);
                        var rattends = attends.Select(x => new REL_EVENTS_ATTENDEES { Id = this.KeyGenerator.GetNextKey(), EventId = entity.Id, AttendeeId = x.Id });
                        var orattends = db.Select<REL_EVENTS_ATTENDEES>(q => q.EventId == entity.Id && Sql.In(q.AttendeeId, attends.Select(x => x.Id).ToArray()));
                        db.SaveAll((!orattends.NullOrEmpty() && !rattends.Except(orattends).NullOrEmpty())
                        ? rattends.Except(orattends)
                        : rattends, transaction);
                    }

                    if (!attachbins.NullOrEmpty())
                    {
                        db.SaveAll(attachbins, transaction);
                        var rattachbins = attachbins.Select(x => new REL_EVENTS_ATTACHBINS { Id = this.KeyGenerator.GetNextKey(), EventId = entity.Id, AttachmentId = x.Id });
                        var orattachbins = db.Select<REL_EVENTS_ATTACHBINS>(q => q.EventId == entity.Id && Sql.In(q.AttachmentId, attachbins.Select(x => x.Id).ToArray()));
                        db.SaveAll((!orattachbins.NullOrEmpty() && !rattachbins.Except(orattachbins).NullOrEmpty())
                        ? rattachbins.Except(orattachbins)
                        : rattachbins, transaction);
                    }

                    if (!attachuris.NullOrEmpty())
                    {
                        db.SaveAll(attachuris, transaction);
                        var rattachuris = attachuris.Select(x => new REL_EVENTS_ATTACHURIS { Id = this.KeyGenerator.GetNextKey(), EventId = entity.Id, AttachmentId = x.Id });

                        var orattachuris = (!attachuris.NullOrEmpty()) ?
                            db.Select<REL_EVENTS_ATTACHURIS>(q => q.EventId == entity.Id && Sql.In(q.AttachmentId, attachuris.Select(x => x.Id).ToArray())) : null;

                        db.SaveAll((!orattachuris.NullOrEmpty() && !rattachuris.Except(orattachuris).NullOrEmpty())
                         ? rattachuris.Except(orattachuris)
                         : rattachuris, transaction);
                    }

                    if (!contacts.NullOrEmpty())
                    {
                        db.SaveAll(contacts, transaction);
                        var rcontacts = contacts.Select(x => new REL_EVENTS_CONTACTS { Id = this.KeyGenerator.GetNextKey(), EventId = entity.Id, ContactId = x.Id });
                        var orcontacts = db.Select<REL_EVENTS_CONTACTS>(q => q.EventId == entity.Id && Sql.In(q.ContactId, contacts.Select(x => x.Id).ToArray()));

                        db.SaveAll((!orcontacts.NullOrEmpty() && !rcontacts.Except(orcontacts).NullOrEmpty())
                        ? rcontacts.Except(orcontacts)
                        : rcontacts, transaction);
                    }

                    if (!comments.NullOrEmpty())
                    {
                        db.SaveAll(comments, transaction);
                        var rcomments = comments.Select(x => new REL_EVENTS_COMMENTS { Id = this.KeyGenerator.GetNextKey(), EventId = entity.Id, CommentId = x.Id });
                        var orcomments = db.Select<REL_EVENTS_COMMENTS>(q => q.EventId == entity.Id && Sql.In(q.CommentId, comments.Select(x => x.Id).ToArray()));
                        db.SaveAll((!orcomments.NullOrEmpty() && !rcomments.Except(orcomments).NullOrEmpty())
                        ? rcomments.Except(orcomments)
                        : rcomments, transaction);
                    }

                    if (!rdates.NullOrEmpty())
                    {
                        db.SaveAll(rdates, transaction);
                        var rrdates = rdates.Select(x => new REL_EVENTS_RDATES { Id = this.KeyGenerator.GetNextKey(), EventId = entity.Id, RecurrenceDateId = x.Id });
                        var orrdates = db.Select<REL_EVENTS_RDATES>(q => q.EventId == entity.Id && Sql.In(q.RecurrenceDateId, rdates.Select(x => x.Id).ToArray()));
                        db.SaveAll((!orrdates.NullOrEmpty() && !rrdates.Except(orrdates).NullOrEmpty())
                            ? rrdates.Except(orrdates) :
                            rrdates, transaction);
                    }

                    if (!exdates.NullOrEmpty())
                    {
                        db.SaveAll(exdates, transaction);
                        var rexdates = exdates.Select(x => new REL_EVENTS_EXDATES { Id = this.KeyGenerator.GetNextKey(), EventId = entity.Id, ExceptionDateId = x.Id });
                        var orexdates = db.Select<REL_EVENTS_EXDATES>(q => q.EventId == entity.Id && Sql.In(q.ExceptionDateId, exdates.Select(x => x.Id).ToArray()));
                        db.SaveAll((!orexdates.NullOrEmpty() && !rexdates.Except(orexdates).NullOrEmpty())
                        ? rexdates.Except(orexdates)
                        : rexdates, transaction);
                    }

                    if (!relateds.NullOrEmpty())
                    {
                        db.SaveAll(relateds, transaction);
                        var rrelateds = relateds.Select(x => new REL_EVENTS_RELATEDTOS { Id = this.KeyGenerator.GetNextKey(), EventId = entity.Id, RelatedToId = x.Id });
                        var orrelateds = db.Select<REL_EVENTS_RELATEDTOS>(q => q.EventId == entity.Id && Sql.In(q.RelatedToId, relateds.Select(x => x.Id).ToArray()));
                        db.SaveAll((!orrelateds.NullOrEmpty() && !rrelateds.Except(orrelateds).NullOrEmpty())
                        ? rrelateds.Except(orrelateds)
                        : rrelateds, transaction);
                    }

                    if (!resources.NullOrEmpty())
                    {
                        db.SaveAll(resources, transaction);
                        var rresources = resources.Select(x => new REL_EVENTS_RESOURCES { Id = this.KeyGenerator.GetNextKey(), EventId = entity.Id, ResourcesId = x.Id });
                        var orresources = db.Select<REL_EVENTS_RESOURCES>(q => q.EventId == entity.Id && Sql.In(q.ResourcesId, resources.Select(x => x.Id).ToArray()));
                        db.SaveAll((!orresources.NullOrEmpty() && !rresources.Except(orresources).NullOrEmpty())
                        ? rresources.Except(orresources)
                        : rresources, transaction);

                    }
                    if (!reqstats.NullOrEmpty())
                    {
                        db.SaveAll(reqstats, transaction);
                        var rreqstats = reqstats.Select(x => new REL_EVENTS_REQSTATS { Id = this.KeyGenerator.GetNextKey(), EventId = entity.Id, ReqStatsId = x.Id });
                        var orreqstats = db.Select<REL_EVENTS_REQSTATS>(q => q.EventId == entity.Id && Sql.In(q.ReqStatsId, reqstats.Select(x => x.Id)));

                        db.SaveAll((!orreqstats.NullOrEmpty() && !rreqstats.Except(orreqstats).NullOrEmpty())
                        ? rreqstats.Except(orreqstats)
                        : rreqstats, transaction);
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    try { transaction.Rollback(); }
                    catch (Exception) { throw; }
                }
            } 
            #endregion

            #region save aggregate attributes of entity
            
            try
            {
                if (!aalarms.NullOrEmpty()) this.AudioAlarmRepository.SaveAll(aalarms);
                if (!dalarms.NullOrEmpty()) this.DisplayAlarmRepository.SaveAll(dalarms);
                if (!ealarms.NullOrEmpty()) this.EmailAlarmRepository.SaveAll(ealarms);
            }
            catch (Exception) { throw; }

            using (var transaction = db.OpenTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    if (!aalarms.NullOrEmpty())
                    {
                        var raalarms = aalarms.Select(x => new REL_EVENTS_AUDIO_ALARMS { Id = this.KeyGenerator.GetNextKey(), EventId = entity.Id, AlarmId = x.Id });
                        var oraalarms = db.Select<REL_EVENTS_AUDIO_ALARMS>(q => q.EventId == entity.Id && Sql.In(q.EventId, aalarms.Select(x => x.Id).ToArray()));
                        db.SaveAll((!oraalarms.NullOrEmpty() && !raalarms.Except(oraalarms).NullOrEmpty())
                        ? raalarms.Except(oraalarms)
                        : raalarms, transaction);
                    }

                    if (!dalarms.NullOrEmpty())
                    {
                        var rdalarms = dalarms.Select(x => new REL_EVENTS_DISPLAY_ALARMS { Id = this.KeyGenerator.GetNextKey(), EventId = entity.Id, AlarmId = x.Id });
                        var ordalarms = db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => q.EventId == entity.Id && Sql.In(q.EventId, dalarms.Select(x => x.Id).ToArray()));
                        db.SaveAll((!ordalarms.NullOrEmpty() && !rdalarms.Except(ordalarms).NullOrEmpty())
                        ? rdalarms.Except(ordalarms)
                        : rdalarms, transaction);
                    }

                    if (!ealarms.NullOrEmpty())
                    {
                        var realarms = ealarms.Select(x => new REL_EVENTS_EMAIL_ALARMS { Id = this.KeyGenerator.GetNextKey(), EventId = entity.Id, AlarmId = x.Id });
                        var orealarms = db.Select<REL_EVENTS_EMAIL_ALARMS>(q => q.EventId == entity.Id && Sql.In(q.EventId, ealarms.Select(x => x.Id).ToArray()));
                        db.SaveAll((!orealarms.NullOrEmpty() && !realarms.Except(orealarms).NullOrEmpty())
                        ? realarms.Except(orealarms)
                        : realarms, transaction);
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    try { transaction.Rollback(); }
                    catch (Exception) { throw; }
                }
            } 
            #endregion

        }

        public void Patch(VEVENT source, Expression<Func<VEVENT, object>> fields, Expression<Func<VEVENT, bool>> where = null)
        {
            #region construct anonymous fields using expression lambdas
            
            var selection = fields.GetMemberNames();

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

                string[] eventids = null;
                try
                {
                    eventids = (where != null)
                ? db.SelectParam<VEVENT>(q => q.Id, where).ToArray()
                : db.SelectParam<VEVENT>(q => q.Id).ToArray();
                }
                catch (Exception)
                {
                    
                    throw;
                }


                #region save relational non-aggregate attributes of entities

                using (var transaction = db.OpenTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        if (selection.Contains(orgexpr.GetMemberName()))
                        {
                            //get events-organizers relations
                            var org = source.Organizer as ORGANIZER;
                            if (org != null)
                            {
                                db.Save(org);
                                var rorgs = eventids.Select(x => new REL_EVENTS_ORGANIZERS { Id = this.KeyGenerator.GetNextKey(), EventId = x, OrganizerId = org.Id });
                                var ororgs = db.Select<REL_EVENTS_ORGANIZERS>(q => Sql.In(q.EventId, eventids) && q.OrganizerId == (source.Organizer as ORGANIZER).Id);
                                db.SaveAll((!ororgs.NullOrEmpty() && !rorgs.Except(ororgs).NullOrEmpty())
                               ? rorgs.Except(ororgs)
                               : rorgs, transaction);
                            }
                        }
                        if (selection.Contains(rruleexpr.GetMemberName()))
                        {
                            var rrule = source.RecurrenceRule as RECUR;
                            if (rrule != null)
                            {
                                db.Save(rrule);
                                var rrrules = eventids.Select(x => new REL_EVENTS_RRULES { Id = this.KeyGenerator.GetNextKey(), EventId = x, RecurrenceRuleId = rrule.Id });
                                var orrrules = db.Select<REL_EVENTS_RRULES>(q => Sql.In(q.EventId, eventids) && q.RecurrenceRuleId == (source.RecurrenceRule as RECUR).Id);
                                db.SaveAll((!orrrules.NullOrEmpty() && !rrrules.Except(orrrules).NullOrEmpty())
                                ? rrrules.Except(orrrules)
                                : rrrules, transaction);
                            }
                        }

                        if (selection.Contains(attendsexpr.GetMemberName()))
                        {
                            var attends = source.Attendees.OfType<ATTENDEE>();
                            if (!attends.NullOrEmpty())
                            {
                                db.SaveAll(attends, transaction);
                                var rattends = eventids.SelectMany(x => attends.Select(y => new REL_EVENTS_ATTENDEES { Id = this.KeyGenerator.GetNextKey(), EventId = x, AttendeeId = y.Id }));
                                var orattends = db.Select<REL_EVENTS_ATTENDEES>(q => Sql.In(q.EventId, eventids) && Sql.In(q.AttendeeId, attends.Select(x => x.Id).ToArray()));
                                db.SaveAll((!orattends.NullOrEmpty() && !rattends.Except(orattends).NullOrEmpty())
                                    ? rattends.Except(orattends)
                                    : rattends, transaction);
                            }
                        }

                        if (selection.Contains(attachsexpr.GetMemberName()))
                        {
                            var attachbins = source.Attachments.OfType<ATTACH_BINARY>();
                            if (!attachbins.NullOrEmpty())
                            {
                                db.SaveAll(attachbins, transaction);
                                var rattachbins = eventids.SelectMany(x => attachbins.Select(y => new REL_EVENTS_ATTACHBINS { Id = this.KeyGenerator.GetNextKey(), EventId = x, AttachmentId = y.Id }));
                                var orattachbins = db.Select<REL_EVENTS_ATTACHBINS>(q => Sql.In(q.EventId, eventids) && Sql.In(q.AttachmentId, attachbins.Select(x => x.Id).ToArray()));
                                db.SaveAll((!orattachbins.NullOrEmpty() && !rattachbins.Except(orattachbins).NullOrEmpty())
                                ?rattachbins.Except(orattachbins)
                                : rattachbins, transaction);

                            }

                            var attachuris = source.Attachments.OfType<ATTACH_BINARY>();
                            if (!attachuris.NullOrEmpty())
                            {
                                db.SaveAll(attachuris, transaction);
                                var rattachuris = eventids.SelectMany(x => attachuris.Select(y => new REL_EVENTS_ATTACHURIS { Id = this.KeyGenerator.GetNextKey(), EventId = x, AttachmentId = y.Id }));
                                var orattachuris = db.Select<REL_EVENTS_ATTACHURIS>(q => Sql.In(q.EventId, eventids) && Sql.In(q.AttachmentId, attachuris.Select(x => x.Id).ToArray()));
                                db.SaveAll((!orattachuris.NullOrEmpty() && !rattachuris.Except(orattachuris).NullOrEmpty())
                                ?rattachuris.Except(orattachuris)
                                :rattachuris, transaction);
                            }
                        }

                        if (selection.Contains(contactsexpr.GetMemberName()))
                        {
                            var contacts = source.Contacts.OfType<CONTACT>();
                            if (!contacts.NullOrEmpty())
                            {
                                db.SaveAll(contacts, transaction);
                                var rcontacts = eventids.SelectMany(x => contacts.Select(y => new REL_EVENTS_CONTACTS { Id = this.KeyGenerator.GetNextKey(), EventId = x, ContactId = y.Id }));
                                var orcontacts = db.Select<REL_EVENTS_CONTACTS>(q => Sql.In(q.EventId, eventids) && Sql.In(q.ContactId, contacts.Select(x => x.Id).ToArray()));
                                db.SaveAll((!orcontacts.NullOrEmpty() && !rcontacts.Except(orcontacts).NullOrEmpty())
                                ?rcontacts.Except(orcontacts)
                                :rcontacts, transaction);
                            }
                        }

                        if (selection.Contains(commentsexpr.GetMemberName()))
                        {
                            var comments = source.Contacts.OfType<COMMENT>();
                            if (!comments.NullOrEmpty())
                            {
                                db.SaveAll(comments, transaction);
                                var rcomments = eventids.SelectMany(x => comments.Select(y => new REL_EVENTS_COMMENTS { Id = this.KeyGenerator.GetNextKey(), EventId = x, CommentId = y.Id }));
                                var orcomments = db.Select<REL_EVENTS_COMMENTS>(q => Sql.In(q.EventId, eventids) && Sql.In(q.CommentId, comments.Select(x => x.Id).ToArray()));
                                db.SaveAll((!orcomments.NullOrEmpty() && !rcomments.Except(orcomments).NullOrEmpty())
                                ?rcomments.Except(orcomments)
                                :rcomments, transaction);
                            }
                        }

                        if (selection.Contains(rdatesexpr.GetMemberName()))
                        {
                            var rdates = source.Contacts.OfType<RDATE>();
                            if (!rdates.NullOrEmpty())
                            {
                                db.SaveAll(rdates, transaction);
                                var rrdates = eventids.SelectMany(x => rdates.Select(y => new REL_EVENTS_RDATES { Id = this.KeyGenerator.GetNextKey(), EventId = x, RecurrenceDateId = y.Id }));
                                var ordates = db.Select<REL_EVENTS_RDATES>(q => Sql.In(q.EventId, eventids) && Sql.In(q.RecurrenceDateId, rdates.Select(x => x.Id).ToArray()));
                                db.SaveAll((!ordates.NullOrEmpty() && !rrdates.Except(ordates).NullOrEmpty())
                                    ?rrdates.Except(ordates)
                                    :rrdates, transaction);
                            }
                        }

                        if (selection.Contains(exdatesexpr.GetMemberName()))
                        {
                            var exdates = source.Contacts.OfType<EXDATE>();
                            if (!exdates.NullOrEmpty())
                            {
                                db.SaveAll(exdates, transaction);
                                var rexdates = eventids.SelectMany(x => exdates.Select(y => new REL_EVENTS_EXDATES { Id = this.KeyGenerator.GetNextKey(), EventId = x, ExceptionDateId = y.Id }));
                                var orexdates = db.Select<REL_EVENTS_EXDATES>(q => Sql.In(q.EventId, eventids) && Sql.In(q.ExceptionDateId, exdates.Select(x => x.Id).ToArray()));
                                db.SaveAll((!exdates.NullOrEmpty() && !rexdates.Except(orexdates).NullOrEmpty())
                                ?rexdates.Except(orexdates)
                                :rexdates, transaction);
                            }
                        }

                        if (selection.Contains(relatedtosexpr.GetMemberName()))
                        {
                            var relatedtos = source.Contacts.OfType<RELATEDTO>();
                            if (!relatedtos.NullOrEmpty())
                            {
                                db.SaveAll(relatedtos, transaction);
                                var rrelatedtos = eventids.SelectMany(x => relatedtos.Select(y => new REL_EVENTS_RELATEDTOS { Id = this.KeyGenerator.GetNextKey(), EventId = x, RelatedToId = y.Id }));
                                var orrelatedtos = db.Select<REL_EVENTS_RELATEDTOS>(q => Sql.In(q.EventId, eventids) && Sql.In(q.RelatedToId, relatedtos.Select(x => x.Id).ToArray()));
                                db.SaveAll((!relatedtos.NullOrEmpty() && !rrelatedtos.Except(orrelatedtos).NullOrEmpty())
                                ?rrelatedtos.Except(orrelatedtos)
                                :rrelatedtos, transaction);
                            }
                        }

                        if (selection.Contains(resourcesexpr.GetMemberName()))
                        {
                            var resources = source.Contacts.OfType<RESOURCES>();
                            if (!resources.NullOrEmpty())
                            {
                                db.SaveAll(resources, transaction);
                                var rresources = eventids.SelectMany(x => resources.Select(y => new REL_EVENTS_RESOURCES { Id = this.KeyGenerator.GetNextKey(), EventId = x, ResourcesId = y.Id }));
                                var orresources = db.Select<REL_EVENTS_RESOURCES>(q => Sql.In(q.EventId, eventids) && Sql.In(q.ResourcesId, resources.Select(x => x.Id).ToArray()));
                                db.SaveAll((!orresources.NullOrEmpty() && !rresources.Except(orresources).NullOrEmpty())
                                ?rresources.Except(orresources)
                                :rresources, transaction);
                            }
                        }

                        if (selection.Contains(reqstatsexpr.GetMemberName()))
                        {
                            var reqstats = source.Contacts.OfType<REQUEST_STATUS>();
                            if (!reqstats.NullOrEmpty())
                            {
                                db.SaveAll(reqstats, transaction);
                                var rreqstats = eventids.SelectMany(x => reqstats.Select(y => new REL_EVENTS_REQSTATS { Id = this.KeyGenerator.GetNextKey(), EventId = x, ReqStatsId = y.Id }));
                                var orreqstats = db.Select<REL_EVENTS_REQSTATS>(q => Sql.In(q.EventId, eventids) && Sql.In(q.ReqStatsId, reqstats.Select(x => x.Id).ToArray()));
                                db.SaveAll((!orreqstats.NullOrEmpty() && !rreqstats.Except(orreqstats).NullOrEmpty())
                                ?rreqstats.Except(orreqstats)
                                :rreqstats, transaction);
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        try {  transaction.Rollback(); }
                        catch (Exception) {  throw; }
                    }
                }

                #endregion

                #region save relational aggregate attributes of entities

                if (selection.Contains(alarmexpr.GetMemberName()))
                {
                    var aalarms = source.Contacts.OfType<AUDIO_ALARM>();
                    var dalarms = source.Contacts.OfType<DISPLAY_ALARM>(); 
                    var ealarms = source.Contacts.OfType<EMAIL_ALARM>();

                    if (!aalarms.NullOrEmpty()) AudioAlarmRepository.SaveAll(aalarms);
                    if (!dalarms.NullOrEmpty()) DisplayAlarmRepository.SaveAll(dalarms);
                    if (!ealarms.NullOrEmpty()) EmailAlarmRepository.SaveAll(ealarms);

                    using (var transaction = db.OpenTransaction())
                    {
                        try
                        {
                            if (!aalarms.NullOrEmpty())
                            {
                                var raalarms = eventids.SelectMany(x => aalarms.Select(y => new REL_EVENTS_AUDIO_ALARMS { Id = this.KeyGenerator.GetNextKey(), EventId = x, AlarmId = y.Id }));
                                var oraalarms = db.Select<REL_EVENTS_AUDIO_ALARMS>(q => Sql.In(q.EventId, eventids) && Sql.In(q.AlarmId, aalarms.Select(x => x.Id).ToArray()));
                                db.SaveAll((!oraalarms.NullOrEmpty() && !oraalarms.Except(oraalarms).NullOrEmpty())
                                ?raalarms.Except(oraalarms)
                                :raalarms, transaction);
                            }

                            if (!dalarms.NullOrEmpty())
                            {
                                var rdalarms = eventids.SelectMany(x => dalarms.Select(y => new REL_EVENTS_DISPLAY_ALARMS { Id = this.KeyGenerator.GetNextKey(), EventId = x, AlarmId = y.Id }));
                                var ordalarms = db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => Sql.In(q.EventId, eventids) && Sql.In(q.AlarmId, dalarms.Select(x => x.Id).ToArray()));
                                db.SaveAll((!ordalarms.NullOrEmpty() && !rdalarms.Except(ordalarms).NullOrEmpty())
                                ?rdalarms.Except(ordalarms)
                                :rdalarms, transaction);
                            }

                            if (!ealarms.NullOrEmpty())
                            {

                                var realarms = eventids.SelectMany(x => ealarms.Select(y => new REL_EVENTS_EMAIL_ALARMS { Id = this.KeyGenerator.GetNextKey(), EventId = x, AlarmId = y.Id }));
                                var orealarms = db.Select<REL_EVENTS_EMAIL_ALARMS>(q => Sql.In(q.EventId, eventids) && Sql.In(q.AlarmId, dalarms.Select(x => x.Id).ToArray()));
                                db.SaveAll((!orealarms.NullOrEmpty() && !realarms.Except(orealarms).NullOrEmpty())
                                ?realarms.Except(orealarms)
                                :realarms, transaction);
                            }

                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            try { transaction.Rollback(); }
                            catch (Exception) { throw; }
                        }
                    }

                }
                #endregion

            } 

            #endregion

            #region update-only non-relational attributes

            try
            {
                //7. Update matching event primitives
                if (!sprimitives.NullOrEmpty())
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
            #endregion

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
            #region retrieve attributes of entities

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

            #endregion

            #region save non-aggregate attribbutes of entities

            using (var transaction = db.OpenTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    //save core entities
                    db.SaveAll(entities, transaction);
                    if (!orgs.NullOrEmpty())
                    {
                        db.SaveAll(orgs, transaction);
                        var rorgs = entities.Where(x => x.Organizer != null && x.Organizer is ORGANIZER).Select(e => new REL_EVENTS_ORGANIZERS { Id = this.KeyGenerator.GetNextKey(), EventId = e.Id, OrganizerId = (e.Organizer as ORGANIZER).Id });

                        var ororgs = db.Select<REL_EVENTS_ORGANIZERS>(q => Sql.In(q.EventId, entities.Select(x => x.Id)) && Sql.In(q.OrganizerId, orgs.Select(x => x.Id).ToArray()));

                        db.SaveAll((!ororgs.NullOrEmpty() && rorgs.Except(ororgs).NullOrEmpty())
                           ? rorgs.Except(ororgs)
                           : rorgs, transaction);
                    }

                    if (!rids.NullOrEmpty())
                    {
                        db.SaveAll(rids, transaction);
                        var rrids = entities.Where(x => x.RecurrenceId != null && x.RecurrenceId is RECURRENCE_ID).Select(e => new REL_EVENTS_RECURRENCE_IDS { Id = this.KeyGenerator.GetNextKey(), EventId = e.Id, RecurrenceId_Id = (e.RecurrenceId as RECURRENCE_ID).Id });

                        var orrids = db.Select<REL_EVENTS_RECURRENCE_IDS>(q => Sql.In(q.EventId, entities.Select(x => x.Id)) && Sql.In(q.RecurrenceId_Id, rids.Select(x => x.Id).ToArray()));

                        db.SaveAll((!orrids.NullOrEmpty() && rrids.Except(orrids).NullOrEmpty())
                            ? rrids.Except(orrids)
                            : rrids, transaction);
                    }

                    if (!rrules.NullOrEmpty())
                    {
                        db.SaveAll(rrules, transaction);
                        var rrrules = entities.Where(x => x.RecurrenceRule != null && x.RecurrenceRule is RECUR).Select(e => new REL_EVENTS_RRULES { Id = this.KeyGenerator.GetNextKey(), EventId = e.Id, RecurrenceRuleId = (e.RecurrenceRule as RECUR).Id });

                        var orrrules = db.Select<REL_EVENTS_RRULES>(q => Sql.In(q.EventId, entities.Select(x => x.Id)) && Sql.In(q.RecurrenceRuleId, rrules.Select(x => x.Id).ToArray()));

                        db.SaveAll((!orrrules.NullOrEmpty() && rrrules.Except(orrrules).NullOrEmpty())
                            ? rrrules.Except(orrrules) :
                            rrrules, transaction);
                    }

                    if (!attends.NullOrEmpty())
                    {
                        db.SaveAll(attends, transaction);
                        var rattends = entities.Where(x => !x.Attendees.OfType<ATTENDEE>().NullOrEmpty())
                            .SelectMany(e => e.Attendees.OfType<ATTENDEE>().Select(x => new REL_EVENTS_ATTENDEES { Id = this.KeyGenerator.GetNextKey(), EventId = e.Id, AttendeeId = x.Id }));
                        var orattends = db.Select<REL_EVENTS_ATTENDEES>(q => Sql.In(q.EventId, entities.Select(x => x.Id)) && Sql.In(q.AttendeeId, attends.Select(x => x.Id).ToArray()));
                        db.SaveAll((!orattends.NullOrEmpty() && !rattends.Except(orattends).NullOrEmpty())
                            ? rattends.Except(orattends) :
                            rattends, transaction);
                    }

                    if (!attachbins.NullOrEmpty())
                    {
                        db.SaveAll(attachbins, transaction);
                        var rattachbins = entities.Where(x => !x.Attachments.OfType<ATTACH_BINARY>().NullOrEmpty())
                            .SelectMany(e => e.Attachments.OfType<ATTACH_BINARY>().Select(x => new REL_EVENTS_ATTACHBINS { Id = this.KeyGenerator.GetNextKey(), EventId = e.Id, AttachmentId = x.Id }));

                        var orattachbins = db.Select<REL_EVENTS_ATTACHBINS>(q => Sql.In(q.EventId, entities.Select(x => x.Id)) && Sql.In(q.AttachmentId, attachbins.Select(x => x.Id).ToArray()));


                        db.SaveAll((!orattachbins.NullOrEmpty() && !rattachbins.Except(orattachbins).NullOrEmpty())
                            ? rattachbins.Except(orattachbins)
                            : rattachbins, transaction);
                    }

                    if (!attachuris.NullOrEmpty())
                    {
                        db.SaveAll(attachuris, transaction);
                        var rattachuris = entities.Where(x => !x.Attachments.OfType<ATTACH_URI>().NullOrEmpty())
                            .SelectMany(e => e.Attachments.OfType<ATTACH_URI>().Select(x => new REL_EVENTS_ATTACHURIS { Id = this.KeyGenerator.GetNextKey(), EventId = e.Id, AttachmentId = x.Id }));

                        var orattachuris = db.Select<REL_EVENTS_ATTACHURIS>(q => Sql.In(q.EventId, entities.Select(x => x.Id)) && Sql.In(q.AttachmentId, attachbins.Select(x => x.Id).ToArray()));

                        db.SaveAll((!orattachuris.NullOrEmpty() && !rattachuris.Except(orattachuris).NullOrEmpty())
                            ? rattachuris.Except(orattachuris)
                            : rattachuris, transaction);
                    }

                    if (!contacts.NullOrEmpty())
                    {
                        db.SaveAll(contacts, transaction);

                        var rcontacts = entities.Where(x => !x.Contacts.OfType<CONTACT>().NullOrEmpty())
                            .SelectMany(e => e.Contacts.OfType<CONTACT>().Select(x => new REL_EVENTS_CONTACTS { Id = this.KeyGenerator.GetNextKey(), EventId = e.Id, ContactId = x.Id }));

                        var orcontacts = db.Select<REL_EVENTS_CONTACTS>(q => Sql.In(q.EventId, entities.Select(x => x.Id)) && Sql.In(q.ContactId, contacts.Select(x => x.Id).ToArray()));

                        db.SaveAll((!orcontacts.NullOrEmpty() && !rcontacts.Except(orcontacts).NullOrEmpty())
                            ? rcontacts.Except(orcontacts)
                            : rcontacts, transaction);
                    }

                    if (!comments.NullOrEmpty())
                    {
                        db.SaveAll(comments, transaction);
                        var rcomments = entities.Where(x => !x.Contacts.OfType<COMMENT>().NullOrEmpty())
                            .SelectMany(e => e.Contacts.OfType<COMMENT>().Select(x => new REL_EVENTS_COMMENTS { Id = this.KeyGenerator.GetNextKey(), EventId = e.Id, CommentId = x.Id }));

                        var orcomments = db.Select<REL_EVENTS_COMMENTS>(q => Sql.In(q.EventId, entities.Select(x => x.Id)) && Sql.In(q.CommentId, comments.Select(x => x.Id).ToArray()));

                        db.SaveAll((!orcomments.NullOrEmpty() && !rcomments.Except(orcomments).NullOrEmpty())
                            ? rcomments.Except(orcomments) :
                            rcomments, transaction);
                    }

                    if (!rdates.NullOrEmpty())
                    {
                        db.SaveAll(rdates, transaction);
                        var rrdates = entities.Where(x => !x.RecurrenceDates.OfType<RDATE>().NullOrEmpty()).SelectMany(e => e.RecurrenceDates.OfType<RDATE>().Distinct(new EqualByStringId<RDATE>())
                            .Select(x => new REL_EVENTS_RDATES { Id = this.KeyGenerator.GetNextKey(), EventId = e.Id, RecurrenceDateId = x.Id }));

                        var orrdates = db.Select<REL_EVENTS_RDATES>(q => Sql.In(q.EventId, entities.Select(x => x.Id)) && Sql.In(q.RecurrenceDateId, rdates.Select(x => x.Id).ToArray()));

                        db.SaveAll((!orrdates.NullOrEmpty() && !rrdates.Except(orrdates).NullOrEmpty())
                            ? rrdates.Except(orrdates)
                            : rrdates, transaction);
                    }

                    if (!exdates.NullOrEmpty())
                    {
                        db.SaveAll(exdates, transaction);
                        var rexdates = entities.Where(x => !x.ExceptionDates.OfType<EXDATE>().NullOrEmpty())
                            .SelectMany(e => e.ExceptionDates.OfType<EXDATE>().Select(x => new REL_EVENTS_EXDATES { Id = this.KeyGenerator.GetNextKey(), EventId = e.Id, ExceptionDateId = x.Id }));

                        var orexdates = db.Select<REL_EVENTS_EXDATES>(q => Sql.In(q.EventId, entities.Select(x => x.Id)) && Sql.In(q.ExceptionDateId, exdates.Select(x => x.Id).ToArray()));

                        db.SaveAll((!rexdates.NullOrEmpty() && !rexdates.Except(orexdates).NullOrEmpty())
                            ? rexdates.Except(orexdates)
                            : rexdates, transaction);
                    }

                    if (!relateds.NullOrEmpty())
                    {
                        db.SaveAll(relateds, transaction);
                        var rrelateds = entities.Where(x => !x.RelatedTos.OfType<RELATEDTO>().NullOrEmpty())
                            .SelectMany(e => e.RelatedTos.OfType<RELATEDTO>().Select(x => new REL_EVENTS_RELATEDTOS { Id = this.KeyGenerator.GetNextKey(), EventId = e.Id, RelatedToId = x.Id }));

                        var orrelateds = db.Select<REL_EVENTS_RELATEDTOS>(q => Sql.In(q.EventId, entities.Select(x => x.Id)) && Sql.In(q.RelatedToId, relateds.Select(x => x.Id).ToArray()));

                        db.SaveAll((!orrelateds.NullOrEmpty() && !rrelateds.Except(orrelateds).NullOrEmpty())
                            ? rrelateds.Except(orrelateds)
                            : rrelateds, transaction);
                    }

                    if (!resources.NullOrEmpty())
                    {
                        db.SaveAll(resources, transaction);
                        var rresources = entities.Where(x => !x.Resources.OfType<RESOURCES>().NullOrEmpty()).SelectMany(e => e.Resources.OfType<RESOURCES>().Select(x => new REL_EVENTS_RESOURCES { Id = this.KeyGenerator.GetNextKey(), EventId = e.Id, ResourcesId = x.Id }));

                        var orresources = db.Select<REL_EVENTS_RESOURCES>(q => Sql.In(q.EventId, entities.Select(x => x.Id)) && Sql.In(q.ResourcesId, resources.Select(x => x.Id).ToArray()));

                        db.SaveAll((!orresources.NullOrEmpty() && !rresources.Except(orresources).NullOrEmpty())
                        ? rresources.Except(orresources)
                        : rresources, transaction);
                    }

                    if (!reqstats.NullOrEmpty())
                    {
                        db.SaveAll(reqstats, transaction);
                        var rreqstats = entities.Where(x => !x.RequestStatuses.OfType<REQUEST_STATUS>().NullOrEmpty())
                            .SelectMany(e => e.RequestStatuses.OfType<REQUEST_STATUS>().Select(x => new REL_EVENTS_REQSTATS { Id = this.KeyGenerator.GetNextKey(), EventId = e.Id, ReqStatsId = x.Id }));

                        var orreqstats = db.Select<REL_EVENTS_REQSTATS>(q => Sql.In(q.EventId, entities.Select(x => x.Id)) && Sql.In(q.ReqStatsId, reqstats.Select(x => x.Id).ToArray()));

                        db.SaveAll((!orreqstats.NullOrEmpty() && !rreqstats.Except(orreqstats).NullOrEmpty())
                            ? rreqstats.Except(orreqstats)
                            : rreqstats, transaction);

                    }

                    transaction.Commit();

                }
                catch (Exception)
                {
                    try {  transaction.Rollback(); }
                    catch (Exception) { throw; }
                }
            } 

            #endregion

            #region save aggregate attributes of entities

            try
            {
                if (!aalarms.NullOrEmpty()) this.AudioAlarmRepository.SaveAll(aalarms);
                if (!dalarms.NullOrEmpty()) this.DisplayAlarmRepository.SaveAll(dalarms);
                if (!ealarms.NullOrEmpty()) this.EmailAlarmRepository.SaveAll(ealarms);
            }
            catch (Exception) { throw; }

            using (var transaction = db.OpenTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    if (!aalarms.NullOrEmpty())
                    {
                        var raalarms = entities.Where(x => !x.Alarms.OfType<AUDIO_ALARM>().NullOrEmpty()).SelectMany(e => e.RequestStatuses.OfType<AUDIO_ALARM>().Select(x => new REL_EVENTS_AUDIO_ALARMS { Id = this.KeyGenerator.GetNextKey(), EventId = e.Id, AlarmId = x.Id }));

                        var oraalarms = db.Select<REL_EVENTS_AUDIO_ALARMS>(q => Sql.In(q.EventId, entities.Select(x => x.Id)) && Sql.In(q.AlarmId, aalarms.Select(x => x.Id).ToArray()));

                        db.SaveAll((!oraalarms.NullOrEmpty() && !raalarms.Except(oraalarms).NullOrEmpty())
                        ? raalarms.Except(oraalarms)
                        : raalarms, transaction);
                    }

                    if (!dalarms.NullOrEmpty())
                    {
                        var rdalarms = entities.Where(x => !x.Alarms.OfType<DISPLAY_ALARM>().NullOrEmpty()).SelectMany(e => e.RequestStatuses.OfType<DISPLAY_ALARM>().Select(x => new REL_EVENTS_DISPLAY_ALARMS { Id = this.KeyGenerator.GetNextKey(), EventId = e.Id, AlarmId = x.Id }));

                        var ordalarms = db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => Sql.In(q.EventId, entities.Select(x => x.Id)) && Sql.In(q.AlarmId, dalarms.Select(x => x.Id).ToArray()));

                        db.SaveAll((!ordalarms.NullOrEmpty() && !rdalarms.Except(ordalarms).NullOrEmpty())
                        ? rdalarms.Except(ordalarms)
                        : rdalarms, transaction);
                    }

                    if (!ealarms.NullOrEmpty())
                    {
                        var realarms = entities.Where(x => !x.Alarms.OfType<EMAIL_ALARM>().NullOrEmpty()).SelectMany(e => e.RequestStatuses.OfType<EMAIL_ALARM>().Select(x => new REL_EVENTS_EMAIL_ALARMS { Id = this.KeyGenerator.GetNextKey(), EventId = e.Id, AlarmId = x.Id }));

                        var orealarms = db.Select<REL_EVENTS_EMAIL_ALARMS>(q => Sql.In(q.EventId, entities.Select(x => x.Id)) && Sql.In(q.AlarmId, ealarms.Select(x => x.Id).ToArray()));

                        db.SaveAll((!orealarms.NullOrEmpty() && !realarms.Except(orealarms).NullOrEmpty())
                            ? realarms.Except(orealarms)
                            : realarms, transaction);
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    try { transaction.Rollback(); }
                    catch (Exception) { throw; }
                }
            }

            #endregion
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
                    r => r.EventId,
                    e => e.Id == dry.Uid);
                if (!orgs.NullOrEmpty()) dry.Organizer = orgs.FirstOrDefault();

                var rids = db.Select<RECURRENCE_ID, VEVENT, REL_EVENTS_RECURRENCE_IDS>(
                    r => r.RecurrenceId_Id,
                    r => r.EventId,
                    e => e.Id == dry.Uid);
                if (!rids.NullOrEmpty()) dry.RecurrenceId = rids.FirstOrDefault();

                var rrules = db.Select<RECUR, VEVENT, REL_EVENTS_RRULES>(
                    r => r.RecurrenceRuleId, 
                    r => r.EventId, 
                    e => e.Id == dry.Uid);
                if (!rrules.NullOrEmpty()) dry.RecurrenceRule = rrules.FirstOrDefault();

               dry.Attachments.AddRangeComplement(db.Select<ATTACH_BINARY, VEVENT, REL_EVENTS_ATTACHBINS>(
                   r => r.AttachmentId,
                   r => r.EventId,
                   e => e.Id == dry.Uid).Except(dry.Attachments.OfType<ATTACH_BINARY>(),new EqualByStringId<ATTACH_BINARY>()));

               dry.Attachments.AddRange(db.Select<ATTACH_URI, VEVENT, REL_EVENTS_ATTACHBINS>(
                   r => r.AttachmentId,
                   r => r.EventId,
                   e => e.Id == dry.Uid).Except(dry.Attachments.OfType<ATTACH_URI>()));

                dry.Attendees.AddRange(db.Select<ATTENDEE, VEVENT, REL_EVENTS_ATTENDEES>(
                    r => r.AttendeeId,
                    r => r.EventId,
                    e => e.Id == dry.Uid).Except(dry.Attendees.OfType<ATTENDEE>(), new EqualByStringId<ATTENDEE>()));

                dry.Comments.AddRange(db.Select<COMMENT, VEVENT, REL_EVENTS_COMMENTS>(
                    r => r.CommentId,
                    r => r.EventId,
                    e => e.Id == dry.Uid).Except(dry.Comments.OfType<COMMENT>()));

                dry.Contacts.AddRange(db.Select<CONTACT, VEVENT, REL_EVENTS_CONTACTS >(
                    r => r.ContactId,
                    r => r.EventId,
                    e => e.Id == dry.Uid).Except(dry.Contacts.OfType<CONTACT>(), new EqualByStringId<CONTACT>()));

                dry.RecurrenceDates.AddRange(db.Select<RDATE, VEVENT, REL_EVENTS_RDATES>(
                    r => r.RecurrenceDateId, 
                    r => r.EventId,
                    e => e.Id == dry.Uid).Except(dry.RecurrenceDates.OfType<RDATE>()));

                dry.ExceptionDates.AddRange(db.Select<EXDATE, VEVENT, REL_EVENTS_EXDATES>(
                    r => r.ExceptionDateId, 
                    r => r.EventId,
                    e => e.Id == dry.Uid).Except(dry.ExceptionDates.OfType<EXDATE>(), new EqualByStringId<EXDATE>()));

                dry.RelatedTos.AddRange(db.Select<RELATEDTO, VEVENT, REL_EVENTS_RELATEDTOS>(
                    r => r.RelatedToId, 
                    r => r.EventId,
                    e => e.Id == dry.Uid).Except(dry.RelatedTos.OfType<RELATEDTO>(), new EqualByStringId<RELATEDTO>()));

                dry.RequestStatuses.AddRange(db.Select<REQUEST_STATUS, VEVENT, REL_EVENTS_REQSTATS>(
                    r => r.ReqStatsId, 
                    r => r.EventId, 
                    e => e.Id == dry.Uid).Except(dry.RequestStatuses.OfType<REQUEST_STATUS>(), new EqualByStringId<REQUEST_STATUS>()));

                dry.Resources.AddRange(db.Select<RESOURCES, VEVENT, REL_EVENTS_RESOURCES>(
                    r => r.ResourcesId,
                    r => r.EventId,
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
                var rorgs = db.Select<REL_EVENTS_ORGANIZERS>(q => Sql.In(q.EventId, uids));
                var rrids = db.Select<REL_EVENTS_RECURRENCE_IDS>(q => Sql.In(q.EventId, uids));
                var rrrules = db.Select<REL_EVENTS_RRULES>(q => Sql.In(q.EventId, uids));
                var rattends = db.Select<REL_EVENTS_ATTENDEES>(q => Sql.In(q.EventId, uids));
                var rcomments = db.Select<REL_EVENTS_COMMENTS>(q => Sql.In(q.EventId, uids));
                var rattachs = db.Select<REL_EVENTS_ATTACHBINS>(q => Sql.In(q.EventId, uids));
                var rcontacts = db.Select<REL_EVENTS_CONTACTS>(q => Sql.In(q.EventId, uids));
                var rexdates = db.Select<REL_EVENTS_EXDATES>(q => Sql.In(q.EventId, uids));
                var rrdates = db.Select<REL_EVENTS_RDATES>(q => Sql.In(q.EventId, uids));
                var rrelateds = db.Select<REL_EVENTS_RELATEDTOS>(q => Sql.In(q.EventId, uids));
                var rrstats = db.Select<REL_EVENTS_REQSTATS>(q => Sql.In(q.EventId, uids));
                var rresources = db.Select<REL_EVENTS_RESOURCES>(q => Sql.In(q.EventId, uids));

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
                    var xorgs = (!orgs.NullOrEmpty())?(from y in orgs join r in rorgs on y.Id equals r.OrganizerId join e in dry on r.EventId equals e.Uid where e.Uid == x.Uid select y): null;
                    if (!xorgs.NullOrEmpty()) x.Organizer = xorgs.FirstOrDefault();

                    var xrrids = (!rids.NullOrEmpty())? (from y in rids join r in rrids on y.Id equals r.RecurrenceId_Id join e in dry on r.EventId equals e.Uid where e.Uid == x.Uid select y): null;
                    if (!rrids.NullOrEmpty()) x.RecurrenceId = xrrids.FirstOrDefault();

                    var xrrules = (!rrules.NullOrEmpty()) ?(from y in rrules join r in rrrules on y.Id equals r.RecurrenceRuleId join e in dry on r.EventId equals e.Uid where e.Uid == x.Uid select y): null;
                    if (!xrrules.NullOrEmpty()) x.RecurrenceRule = xrrules.FirstOrDefault();

                    var xcomments = (comments != null)?(from y in comments join r in rcomments on y.Id equals r.CommentId join e in dry on r.EventId equals e.Uid where e.Uid == x.Uid select y): null;
                    if(!xcomments.NullOrEmpty()) x.Comments.AddRange(xcomments.Except(x.Comments.OfType<COMMENT>(), new EqualByStringId<COMMENT>()));

                    var xattendees = (!attends.NullOrEmpty())?(from y in attends join r in rattends on y.Id equals r.AttendeeId join e in dry on r.EventId equals e.Uid where e.Uid == x.Uid select y): null;
                    if(!xattendees.NullOrEmpty()) x.Attendees.AddRange(xattendees.Except(x.Attendees.OfType<ATTENDEE>(), new EqualByStringId<ATTENDEE>()));

                    var xattachbins = (!attachbins.NullOrEmpty())?(from y in attachbins join r in rattachs on y.Id equals r.AttachmentId join e in dry on r.EventId equals e.Uid where e.Uid == x.Uid select y): null;
                    var xattachuris = (!attachuris.NullOrEmpty())?(from y in attachuris join r in rattachs on y.Id equals r.AttachmentId join e in dry on r.EventId equals e.Uid where e.Uid == x.Uid select y): null;

                    if(!xattachbins.NullOrEmpty()) x.Attachments.AddRange(xattachbins.Except(x.Attachments.OfType<ATTACH_BINARY>(), new EqualByStringId<ATTACH_BINARY>()));
                    if(!xattachuris.NullOrEmpty()) x.Attachments.AddRange(xattachuris.Except(x.Attachments.OfType<ATTACH_URI>(), new EqualByStringId<ATTACH_URI>()));

                    var xcontacts = (!contacts.NullOrEmpty())? (from y in contacts join r in rcontacts on y.Id equals r.ContactId join e in dry on r.EventId equals e.Uid where e.Uid == x.Uid select y): null;
                    if(!xcontacts.NullOrEmpty()) x.Contacts.AddRange(xcontacts.Except(x.Contacts.OfType<CONTACT>(), new EqualByStringId<CONTACT>()));

                    var xrdates = (!rdates.NullOrEmpty())?(from y in rdates join r in rrdates on y.Id equals r.RecurrenceDateId join e in dry on r.EventId equals e.Uid where e.Uid == x.Uid select y): null;
                    if(!xrdates.NullOrEmpty())x.RecurrenceDates.AddRange(xrdates.Except(x.RecurrenceDates.OfType<RDATE>(), new EqualByStringId<RDATE>()));

                    var xexdates = (!exdates.NullOrEmpty())?(from y in exdates join r in rexdates on y.Id equals r.ExceptionDateId join e in dry on r.EventId equals e.Uid where e.Uid == x.Uid select y): null;
                    if(!xexdates.NullOrEmpty())x.ExceptionDates.AddRange(xexdates.Except(x.ExceptionDates.OfType<EXDATE>(), new EqualByStringId<EXDATE>()));

                    var xrelatedtos = (!relatedtos.NullOrEmpty())?(from y in relatedtos join r in rrelateds on y.Id equals r.RelatedToId join e in dry on r.EventId equals e.Uid where e.Uid == x.Uid select y): null;
                    if(!xrelatedtos.NullOrEmpty())x.RelatedTos.AddRange(xrelatedtos.Except(x.RelatedTos.OfType<RELATEDTO>(), new EqualByStringId<RELATEDTO>()));

                    var xreqstats = (!reqstats.NullOrEmpty())?(from y in reqstats join r in rrstats on y.Id equals r.ReqStatsId join e in dry on r.EventId equals e.Uid where e.Uid == x.Uid select y): null;
                    if(!xreqstats.NullOrEmpty())x.RequestStatuses.AddRange(xreqstats.Except(x.RequestStatuses.OfType<REQUEST_STATUS>(), new EqualByStringId<REQUEST_STATUS>()));

                    var xresources = (!resources.NullOrEmpty()) ? (from y in resources join r in rresources on y.Id equals r.ResourcesId join e in dry on r.EventId equals e.Uid where e.Uid == x.Uid select y) : null;
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

        public IEnumerable<string> GetKeys(string fkey, int? page = null)
        {
            IEnumerable<string> keys = null;
            try
            {
                var events = db.Select<VEVENT, VCALENDAR, REL_CALENDARS_EVENTS>(
                    r => r.Uid,
                    r => r.ProdId,
                    c => c.ProdId == fkey);
                keys = (!events.NullOrEmpty()) ? events.Select(x => x.Uid) : null;
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
            return keys;
        }

        public IEnumerable<string> GetKeys(IEnumerable<string> fkeys, int? page = null)
        {
            IEnumerable<string> keys = null;
            try
            {
                var events = db.Select<VEVENT, VCALENDAR, REL_CALENDARS_EVENTS>(
                    r => r.Uid,
                    r => r.ProdId,
                    c => Sql.In(c.ProdId, fkeys.ToArray()));
                keys = (!events.NullOrEmpty()) ? events.Select(x => x.Uid) : null;
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
            return keys;
        }
    }
}
