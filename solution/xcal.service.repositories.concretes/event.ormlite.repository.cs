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
        private int? take = null;
        private IKeyGenerator<string> keygen;

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
        public int? Take
        {
            get { return this.take; }
            set 
            {
                if (value == null) throw new ArgumentNullException("Null pages");
                this.take = value; 
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
        public EventOrmLiteRepository(IDbConnectionFactory factory, int? take)
        {
            this.DbConnectionFactory = factory;
            this.Take = take;
            this.conn = this.factory.OpenDbConnection();
        }
        public EventOrmLiteRepository(IDbConnection connection, int? take)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            this.conn = connection; 
            this.Take = take;
        }

        //cleanup
        public virtual void Dispose()
        {
            if (this.conn != null) this.conn.Dispose();
        }

        public VEVENT Find(string key)
        {
            try
            {
               return db.Select<VEVENT>(q => q.Id == key).FirstOrDefault();
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public IEnumerable<VEVENT> Find(IEnumerable<string> keys, int? skip = null)
        {
            try
            {
                return db.Select<VEVENT>(q => Sql.In(q.Uid, keys.ToArray()), skip, take);
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public IEnumerable<VEVENT> Get(int? skip = null)
        {
            IEnumerable<VEVENT> dry = null;
            try
            {
                dry = db.Select<VEVENT>(skip, take);
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
            var attendees = entity.Attendees.OfType<ATTENDEE>();
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
                        var rorg = new REL_EVENTS_ORGANIZERS 
                        { 
                            Id = this.KeyGenerator.GetNextKey(), 
                            EventId = entity.Id, 
                            OrganizerId = org.Id 
                        };
                        var ororgs = db.Select<REL_EVENTS_ORGANIZERS>(q => q.EventId == entity.Id);
                        if (!ororgs.NullOrEmpty() && !ororgs.Contains(rorg)) db.Save(rorg, transaction);
                    }
                    if (rid != null)
                    {
                        db.Save(rid, transaction);
                        var rrid = new REL_EVENTS_RECURRENCE_IDS 
                        { 
                            Id = this.KeyGenerator.GetNextKey(), 
                            EventId = entity.Id, 
                            RecurrenceId_Id = rid.Id 
                        };
                        var orrids = db.Select<REL_EVENTS_RECURRENCE_IDS>(q => q.EventId == entity.Id);
                        if (!orrids.NullOrEmpty() && !orrids.Contains(rrid)) db.Save(rrid, transaction);
                    }

                    if (rrule != null)
                    {
                        db.Save(rrule, transaction);
                        var rrrule = new REL_EVENTS_RECURS 
                        { 
                            Id = this.KeyGenerator.GetNextKey(), 
                            EventId = entity.Id, 
                            RecurrenceRuleId = rrule.Id 
                        };
                        var orrrules = db.Select<REL_EVENTS_RECURS>(q => q.EventId == entity.Id);
                        if (!orrrules.NullOrEmpty() && !orrrules.Contains(rrrule)) db.Save(rrule, transaction);
                    }

                    if (!attendees.NullOrEmpty())
                    {
                        db.SaveAll(attendees, transaction);
                        var rattends = attendees.Select(x => new REL_EVENTS_ATTENDEES 
                        { 
                            Id = this.KeyGenerator.GetNextKey(), 
                            EventId = entity.Id, 
                            AttendeeId = x.Id 
                        });
                        var orattends = db.Select<REL_EVENTS_ATTENDEES>(q => q.EventId == entity.Id);
                        db.SaveAll(!orattends.NullOrEmpty() ? rattends.Except(orattends): rattends, transaction);
                    }

                    if (!attachbins.NullOrEmpty())
                    {
                        db.SaveAll(attachbins, transaction);
                        var rattachbins = attachbins.Select(x => new REL_EVENTS_ATTACHBINS 
                        { 
                            Id = this.KeyGenerator.GetNextKey(), 
                            EventId = entity.Id, 
                            AttachmentId = x.Id 
                        });
                        var orattachbins = db.Select<REL_EVENTS_ATTACHBINS>(q => q.EventId == entity.Id);
                        db.SaveAll(!orattachbins.NullOrEmpty() ? rattachbins.Except(orattachbins): rattachbins, transaction);
                    }

                    if (!attachuris.NullOrEmpty())
                    {
                        db.SaveAll(attachuris, transaction);
                        var rattachuris = attachuris.Select(x => new REL_EVENTS_ATTACHURIS 
                        { 
                            Id = this.KeyGenerator.GetNextKey(), 
                            EventId = entity.Id, 
                            AttachmentId = x.Id 
                        });
                        var orattachuris = db.Select<REL_EVENTS_ATTACHURIS>(q => q.EventId == entity.Id );
                        db.SaveAll(!orattachuris.NullOrEmpty() ? rattachuris.Except(orattachuris) : rattachuris, transaction);
                    }

                    if (!contacts.NullOrEmpty())
                    {
                        db.SaveAll(contacts, transaction);
                        var rcontacts = contacts.Select(x => new REL_EVENTS_CONTACTS 
                        { 
                            Id = this.KeyGenerator.GetNextKey(), 
                            EventId = entity.Id, 
                            ContactId = x.Id 
                        });
                        var orcontacts = db.Select<REL_EVENTS_CONTACTS>(q => q.EventId == entity.Id);

                        db.SaveAll((!orcontacts.NullOrEmpty()) ? rcontacts.Except(orcontacts) : rcontacts, transaction);
                    }

                    if (!comments.NullOrEmpty())
                    {
                        db.SaveAll(comments, transaction);
                        var rcomments = comments.Select(x => new REL_EVENTS_COMMENTS 
                        { 
                            Id = this.KeyGenerator.GetNextKey(), 
                            EventId = entity.Id, 
                            CommentId = x.Id 
                        });
                        var orcomments = db.Select<REL_EVENTS_COMMENTS>(q => q.EventId == entity.Id);
                        db.SaveAll((!orcomments.NullOrEmpty()) ? rcomments.Except(orcomments): rcomments, transaction);
                    }

                    if (!rdates.NullOrEmpty())
                    {
                        db.SaveAll(rdates, transaction);
                        var rrdates = rdates.Select(x => new REL_EVENTS_RDATES 
                        { 
                            Id = this.KeyGenerator.GetNextKey(), 
                            EventId = entity.Id, 
                            RecurrenceDateId = x.Id 
                        });
                        var orrdates = db.Select<REL_EVENTS_RDATES>(q => q.EventId == entity.Id);
                        db.SaveAll((!orrdates.NullOrEmpty()) ? rrdates.Except(orrdates): rrdates, transaction);
                    }

                    if (!exdates.NullOrEmpty())
                    {
                        db.SaveAll(exdates, transaction);
                        var rexdates = exdates.Select(x => new REL_EVENTS_EXDATES 
                        { 
                            Id = this.KeyGenerator.GetNextKey(), 
                            EventId = entity.Id, 
                            ExceptionDateId = x.Id 
                        });
                        var orexdates = db.Select<REL_EVENTS_EXDATES>(q => q.EventId == entity.Id);
                        db.SaveAll((!orexdates.NullOrEmpty()) ? rexdates.Except(orexdates) : rexdates, transaction);
                    }

                    if (!relateds.NullOrEmpty())
                    {
                        db.SaveAll(relateds, transaction);
                        var rrelateds = relateds.Select(x => new REL_EVENTS_RELATEDTOS 
                        { 
                            Id = this.KeyGenerator.GetNextKey(), 
                            EventId = entity.Id, 
                            RelatedToId = x.Id 
                        });
                        var orrelateds = db.Select<REL_EVENTS_RELATEDTOS>(q => q.EventId == entity.Id);
                        db.SaveAll((!orrelateds.NullOrEmpty()) ? rrelateds.Except(orrelateds) : rrelateds, transaction);
                    }

                    if (!resources.NullOrEmpty())
                    {
                        db.SaveAll(resources, transaction);
                        var rresources = resources.Select(x => new REL_EVENTS_RESOURCES 
                        { 
                            Id = this.KeyGenerator.GetNextKey(), 
                            EventId = entity.Id, 
                            ResourcesId = x.Id 
                        });
                        var orresources = db.Select<REL_EVENTS_RESOURCES>(q => q.EventId == entity.Id);
                        db.SaveAll((!orresources.NullOrEmpty()) ? rresources.Except(orresources): rresources, transaction);
                    }

                    if (!reqstats.NullOrEmpty())
                    {
                        db.SaveAll(reqstats, transaction);
                        var rreqstats = reqstats.Select(x => new REL_EVENTS_REQSTATS 
                        { 
                            Id = this.KeyGenerator.GetNextKey(), 
                            EventId = entity.Id, 
                            ReqStatsId = x.Id 
                        });
                        var orreqstats = db.Select<REL_EVENTS_REQSTATS>(q => q.EventId == entity.Id);
                        db.SaveAll((!orreqstats.NullOrEmpty()) ? rreqstats.Except(orreqstats) : rreqstats, transaction);
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
                        var raalarms = aalarms.Select(x => new REL_EVENTS_AUDIO_ALARMS 
                        { 
                            Id = this.KeyGenerator.GetNextKey(), 
                            EventId = entity.Id, 
                            AlarmId = x.Id 
                        });
                        var oraalarms = db.Select<REL_EVENTS_AUDIO_ALARMS>(q => q.EventId == entity.Id);
                        db.SaveAll((!oraalarms.NullOrEmpty()) ? raalarms.Except(oraalarms): raalarms, transaction);
                    }

                    if (!dalarms.NullOrEmpty())
                    {
                        var rdalarms = dalarms.Select(x => new REL_EVENTS_DISPLAY_ALARMS 
                        { 
                            Id = this.KeyGenerator.GetNextKey(), 
                            EventId = entity.Id, 
                            AlarmId = x.Id 
                        });
                        var ordalarms = db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => q.EventId == entity.Id);
                        db.SaveAll((!ordalarms.NullOrEmpty()) ? rdalarms.Except(ordalarms) : rdalarms, transaction);
                    }

                    if (!ealarms.NullOrEmpty())
                    {
                        var realarms = ealarms.Select(x => new REL_EVENTS_EMAIL_ALARMS 
                        { 
                            Id = this.KeyGenerator.GetNextKey(), 
                            EventId = entity.Id, 
                            AlarmId = x.Id 
                        });
                        var orealarms = db.Select<REL_EVENTS_EMAIL_ALARMS>(q => q.EventId == entity.Id);
                        db.SaveAll((!orealarms.NullOrEmpty()) ? realarms.Except(orealarms) : realarms, transaction);
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

                string[] keys = null;
                try
                {
                    keys = (predicate != null)
                        ? db.SelectParam<VEVENT>(q => q.Id, predicate).ToArray()
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
                                var rorgs = keys.Select(x => new REL_EVENTS_ORGANIZERS 
                                { 
                                    Id = this.KeyGenerator.GetNextKey(), 
                                    EventId = x, 
                                    OrganizerId = org.Id 
                                });
                                var ororgs = db.Select<REL_EVENTS_ORGANIZERS>(q => Sql.In(q.EventId, keys));
                                db.SaveAll((!ororgs.NullOrEmpty())
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
                                var rrrules = keys.Select(x => new REL_EVENTS_RECURS
                                { 
                                    Id = this.KeyGenerator.GetNextKey(), 
                                    EventId = x, 
                                    RecurrenceRuleId = rrule.Id 
                                });
                                var orrrules = db.Select<REL_EVENTS_RECURS>(q => Sql.In(q.EventId, keys));
                                db.SaveAll((!orrrules.NullOrEmpty())
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
                                var rattends = keys.SelectMany(x => attends.Select(y => new REL_EVENTS_ATTENDEES 
                                { 
                                    Id = this.KeyGenerator.GetNextKey(), 
                                    EventId = x, 
                                    AttendeeId = y.Id 
                                }));
                                var orattends = db.Select<REL_EVENTS_ATTENDEES>(q => Sql.In(q.EventId, keys));
                                db.SaveAll((!orattends.NullOrEmpty())
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
                                var rattachbins = keys.SelectMany(x => attachbins.Select(y => new REL_EVENTS_ATTACHBINS 
                                { 
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x, 
                                    AttachmentId = y.Id 
                                }));
                                var orattachbins = db.Select<REL_EVENTS_ATTACHBINS>(q => Sql.In(q.EventId, keys));
                                db.SaveAll((!orattachbins.NullOrEmpty())
                                    ?rattachbins.Except(orattachbins)
                                    : rattachbins, transaction);

                            }

                            var attachuris = source.Attachments.OfType<ATTACH_BINARY>();
                            if (!attachuris.NullOrEmpty())
                            {
                                db.SaveAll(attachuris, transaction);
                                var rattachuris = keys.SelectMany(x => attachuris.Select(y => new REL_EVENTS_ATTACHURIS 
                                { 
                                    Id = this.KeyGenerator.GetNextKey(), 
                                    EventId = x, 
                                    AttachmentId = y.Id 
                                }));
                                var orattachuris = db.Select<REL_EVENTS_ATTACHURIS>(q => Sql.In(q.EventId, keys));
                                db.SaveAll((!orattachuris.NullOrEmpty() )
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
                                var rcontacts = keys.SelectMany(x => contacts.Select(y => new REL_EVENTS_CONTACTS 
                                { 
                                    Id = this.KeyGenerator.GetNextKey(), 
                                    EventId = x,
                                    ContactId = y.Id }));
                                var orcontacts = db.Select<REL_EVENTS_CONTACTS>(q => Sql.In(q.EventId, keys));
                                db.SaveAll((!orcontacts.NullOrEmpty())
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
                                var rcomments = keys.SelectMany(x => comments.Select(y => new REL_EVENTS_COMMENTS 
                                { 
                                    Id = this.KeyGenerator.GetNextKey(), 
                                    EventId = x, 
                                    CommentId = y.Id 
                                }));
                                var orcomments = db.Select<REL_EVENTS_COMMENTS>(q => Sql.In(q.EventId, keys));
                                db.SaveAll((!orcomments.NullOrEmpty())
                                    ? rcomments.Except(orcomments)
                                    : rcomments, transaction);
                            }
                        }

                        if (selection.Contains(rdatesexpr.GetMemberName()))
                        {
                            var rdates = source.Contacts.OfType<RDATE>();
                            if (!rdates.NullOrEmpty())
                            {
                                db.SaveAll(rdates, transaction);
                                var rrdates = keys.SelectMany(x => rdates.Select(y => new REL_EVENTS_RDATES 
                                { 
                                    Id = this.KeyGenerator.GetNextKey(), 
                                    EventId = x, 
                                    RecurrenceDateId = y.Id 
                                }));
                                var ordates = db.Select<REL_EVENTS_RDATES>(q => Sql.In(q.EventId, keys));
                                db.SaveAll((!ordates.NullOrEmpty())
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
                                var rexdates = keys.SelectMany(x => exdates.Select(y => new REL_EVENTS_EXDATES 
                                { 
                                    Id = this.KeyGenerator.GetNextKey(), 
                                    EventId = x, 
                                    ExceptionDateId = y.Id 
                                }));
                                var orexdates = db.Select<REL_EVENTS_EXDATES>(q => Sql.In(q.EventId, keys));
                                db.SaveAll((!exdates.NullOrEmpty())
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
                                var rrelatedtos = keys.SelectMany(x => relatedtos.Select(y => new REL_EVENTS_RELATEDTOS 
                                { 
                                    Id = this.KeyGenerator.GetNextKey(), 
                                    EventId = x, 
                                    RelatedToId = y.Id 
                                }));
                                var orrelatedtos = db.Select<REL_EVENTS_RELATEDTOS>(q => Sql.In(q.EventId, keys));
                                db.SaveAll((!relatedtos.NullOrEmpty())
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
                                var rresources = keys.SelectMany(x => resources.Select(y => new REL_EVENTS_RESOURCES 
                                { 
                                    Id = this.KeyGenerator.GetNextKey(), 
                                    EventId = x, 
                                    ResourcesId = y.Id 
                                }));
                                var orresources = db.Select<REL_EVENTS_RESOURCES>(q => Sql.In(q.EventId, keys));
                                db.SaveAll((!orresources.NullOrEmpty())
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
                                var rreqstats = keys.SelectMany(x => reqstats.Select(y => new REL_EVENTS_REQSTATS 
                                { 
                                    Id = this.KeyGenerator.GetNextKey(), 
                                    EventId = x, 
                                    ReqStatsId = y.Id 
                                }));
                                var orreqstats = db.Select<REL_EVENTS_REQSTATS>(q => Sql.In(q.EventId, keys));
                                db.SaveAll((!orreqstats.NullOrEmpty())
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
                                var raalarms = keys.SelectMany(x => aalarms.Select(y => new REL_EVENTS_AUDIO_ALARMS 
                                { 
                                    Id = this.KeyGenerator.GetNextKey(), 
                                    EventId = x,
                                    AlarmId = y.Id 
                                }));
                                var oraalarms = db.Select<REL_EVENTS_AUDIO_ALARMS>(q => Sql.In(q.EventId, keys));
                                db.SaveAll((!oraalarms.NullOrEmpty())
                                    ?raalarms.Except(oraalarms)
                                    :raalarms, transaction);
                            }

                            if (!dalarms.NullOrEmpty())
                            {
                                var rdalarms = keys.SelectMany(x => dalarms.Select(y => new REL_EVENTS_DISPLAY_ALARMS 
                                { 
                                    Id = this.KeyGenerator.GetNextKey(), 
                                    EventId = x, 
                                    AlarmId = y.Id 
                                }));
                                var ordalarms = db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => Sql.In(q.EventId, keys));
                                db.SaveAll((!ordalarms.NullOrEmpty())
                                    ?rdalarms.Except(ordalarms)
                                    :rdalarms, transaction);
                            }

                            if (!ealarms.NullOrEmpty())
                            {
                                var realarms = keys.SelectMany(x => ealarms.Select(y => new REL_EVENTS_EMAIL_ALARMS 
                                { 
                                    Id = this.KeyGenerator.GetNextKey(), 
                                    EventId = x, 
                                    AlarmId = y.Id 
                                }));
                                var orealarms = db.Select<REL_EVENTS_EMAIL_ALARMS>(q => Sql.In(q.EventId, keys));
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
                    db.UpdateOnly<VEVENT, object>(source, patchexpr, predicate);
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
            #region 1. retrieve attributes of entities

            var orgs = entities.Where(x => x.Organizer != null && x.Organizer is ORGANIZER).Select(x => x.Organizer as ORGANIZER);
            var rids = entities.Where(x => x.RecurrenceId != null && x.RecurrenceId is RECURRENCE_ID).Select(x => x.RecurrenceId as RECURRENCE_ID);
            var rrules = entities.Where(x => x.RecurrenceRule != null && x.RecurrenceRule is RECUR).Select(x => x.RecurrenceRule as RECUR);
            var attends = entities.Where(x => !x.Attendees.NullOrEmpty() && !x.Attendees.OfType<ATTENDEE>().NullOrEmpty())
                .SelectMany(x => x.Attendees.OfType<ATTENDEE>());
            var attachbins = entities.Where(x => !x.Attachments.NullOrEmpty() && !x.Attachments.OfType<ATTACH_BINARY>().NullOrEmpty())
                .SelectMany(x => x.Attachments.OfType<ATTACH_BINARY>());
            var attachuris = entities.Where(x => !x.Attachments.NullOrEmpty() && !x.Attachments.OfType<ATTACH_URI>().NullOrEmpty())
                .SelectMany(x => x.Attachments.OfType<ATTACH_URI>());
            var contacts = entities.Where(x => !x.Contacts.NullOrEmpty() && !x.Contacts.OfType<CONTACT>().NullOrEmpty())
                .SelectMany(x => x.Contacts.OfType<CONTACT>());
            var comments = entities.Where(x => !x.Comments.NullOrEmpty() && !x.Comments.OfType<COMMENT>().NullOrEmpty())
                .SelectMany(x => x.Comments.OfType<COMMENT>());
            var rdates = entities.Where(x => !x.RecurrenceDates.NullOrEmpty() && !x.RecurrenceDates.OfType<RDATE>().NullOrEmpty())
                .SelectMany(x => x.RecurrenceDates.OfType<RDATE>());
            var exdates = entities.Where(x => !x.ExceptionDates.NullOrEmpty() && !x.ExceptionDates.OfType<EXDATE>().NullOrEmpty())
                .SelectMany(x => x.ExceptionDates.OfType<EXDATE>());
            var relateds = entities.Where(x => !x.RelatedTos.NullOrEmpty() && !x.RelatedTos.OfType<RELATEDTO>().NullOrEmpty())
                .SelectMany(x => x.RelatedTos.OfType<RELATEDTO>());
            var resources = entities.Where(x => !x.Resources.NullOrEmpty() && !x.Resources.OfType<RESOURCES>().NullOrEmpty())
                .SelectMany(x => x.Resources.OfType<RESOURCES>());
            var reqstats = entities.Where(x => !x.RequestStatuses.OfType<REQUEST_STATUS>().NullOrEmpty()).SelectMany(x => x.RequestStatuses.OfType<REQUEST_STATUS>());
            var aalarms = entities.Where(x => !x.Alarms.NullOrEmpty() && !x.Alarms.OfType<AUDIO_ALARM>().NullOrEmpty())
                .SelectMany(x => x.Alarms.OfType<AUDIO_ALARM>());
            var dalarms = entities.Where(x => !x.Alarms.NullOrEmpty() && !x.Alarms.OfType<DISPLAY_ALARM>().NullOrEmpty())
                .SelectMany(x => x.Alarms.OfType<DISPLAY_ALARM>());
            var ealarms = entities.Where(x => x.Alarms.NullOrEmpty() && !x.Alarms.OfType<EMAIL_ALARM>().NullOrEmpty())
                .SelectMany(x => x.Alarms.OfType<EMAIL_ALARM>());

            #endregion

            #region 2. save non-aggregate attribbutes of entities

            using (var transaction = db.OpenTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    //save core entities
                    db.SaveAll(entities, transaction);
                    var keys = entities.Select(x => x.Id).ToArray();
                    if (!orgs.NullOrEmpty())
                    {
                        db.SaveAll(orgs, transaction);
                        var rorgs = entities.Where(x => x.Organizer != null && x.Organizer is ORGANIZER)
                            .Select(e => new REL_EVENTS_ORGANIZERS 
                            { 
                                Id = this.KeyGenerator.GetNextKey(), 
                                EventId = e.Id, 
                                OrganizerId = (e.Organizer as ORGANIZER).Id
                            });
                        var ororgs = db.Select<REL_EVENTS_ORGANIZERS>(q => Sql.In(q.EventId, keys));
                        db.SaveAll((!ororgs.NullOrEmpty()) ? rorgs.Except(ororgs) : rorgs, transaction);
                    }

                    if (!rids.NullOrEmpty())
                    {
                        db.SaveAll(rids, transaction);
                        var rrids = entities.Where(x => x.RecurrenceId != null && x.RecurrenceId is RECURRENCE_ID)
                            .Select(e => new REL_EVENTS_RECURRENCE_IDS 
                            { 
                                Id = this.KeyGenerator.GetNextKey(), 
                                EventId = e.Id, 
                                RecurrenceId_Id = (e.RecurrenceId as RECURRENCE_ID).Id 
                            });
                        var orrids = db.Select<REL_EVENTS_RECURRENCE_IDS>(q => Sql.In(q.EventId, keys));
                        db.SaveAll((!orrids.NullOrEmpty()) ? rrids.Except(orrids) : rrids, transaction);
                    }

                    if (!rrules.NullOrEmpty())
                    {
                        db.SaveAll(rrules, transaction);
                        var rrrules = entities.Where(x => x.RecurrenceRule != null && x.RecurrenceRule is RECUR)
                            .Select(e => new REL_EVENTS_RECURS 
                            { 
                                Id = this.KeyGenerator.GetNextKey(), 
                                EventId = e.Id, 
                                RecurrenceRuleId = (e.RecurrenceRule as RECUR).Id 
                            });

                        var orrrules = db.Select<REL_EVENTS_RECURS>(q => Sql.In(q.EventId, keys));
                        db.SaveAll((!orrrules.NullOrEmpty())  ? rrrules.Except(orrrules) : rrrules, transaction);
                    }

                    if (!attends.NullOrEmpty())
                    {
                        db.SaveAll(attends, transaction);
                        var rattends = entities.Where(x => !x.Attendees.OfType<ATTENDEE>().NullOrEmpty())
                            .SelectMany(e => e.Attendees.OfType<ATTENDEE>()
                                .Select(x => new REL_EVENTS_ATTENDEES 
                                { 
                                    Id = this.KeyGenerator.GetNextKey(), 
                                    EventId = e.Id, 
                                    AttendeeId = x.Id 
                                }));
                        var orattends = db.Select<REL_EVENTS_ATTENDEES>(q => Sql.In(q.EventId, keys));
                        db.SaveAll((!orattends.NullOrEmpty()) ? rattends.Except(orattends) : rattends, transaction);
                    }

                    if (!attachbins.NullOrEmpty())
                    {
                        db.SaveAll(attachbins, transaction);
                        var rattachbins = entities.Where(x => !x.Attachments.OfType<ATTACH_BINARY>().NullOrEmpty())
                            .SelectMany(e => e.Attachments.OfType<ATTACH_BINARY>()
                                .Select(x => new REL_EVENTS_ATTACHBINS 
                                { 
                                    Id = this.KeyGenerator.GetNextKey(), 
                                    EventId = e.Id, 
                                    AttachmentId = x.Id 
                                }));
                        var orattachbins = db.Select<REL_EVENTS_ATTACHBINS>(q => Sql.In(q.EventId, keys));
                        db.SaveAll((!orattachbins.NullOrEmpty()) ? rattachbins.Except(orattachbins): rattachbins, transaction);
                    }

                    if (!attachuris.NullOrEmpty())
                    {
                        db.SaveAll(attachuris, transaction);
                        var rattachuris = entities.Where(x => !x.Attachments.OfType<ATTACH_URI>().NullOrEmpty())
                            .SelectMany(e => e.Attachments.OfType<ATTACH_URI>()
                                .Select(x => new REL_EVENTS_ATTACHURIS 
                                { 
                                    Id = this.KeyGenerator.GetNextKey(), 
                                    EventId = e.Id, 
                                    AttachmentId = x.Id 
                                }));
                        var orattachuris = db.Select<REL_EVENTS_ATTACHURIS>(q => Sql.In(q.EventId, keys));
                        db.SaveAll((!orattachuris.NullOrEmpty()) ? rattachuris.Except(orattachuris): rattachuris, transaction);
                    }

                    if (!contacts.NullOrEmpty())
                    {
                        db.SaveAll(contacts, transaction);
                        var rcontacts = entities.Where(x => !x.Contacts.OfType<CONTACT>().NullOrEmpty())
                            .SelectMany(e => e.Contacts.OfType<CONTACT>().Select(x => new REL_EVENTS_CONTACTS 
                            { 
                                Id = this.KeyGenerator.GetNextKey(), 
                                EventId = e.Id, 
                                ContactId = x.Id 
                            }));
                        var orcontacts = db.Select<REL_EVENTS_CONTACTS>(q => Sql.In(q.EventId, keys));
                        db.SaveAll((!orcontacts.NullOrEmpty()) ? rcontacts.Except(orcontacts) : rcontacts, transaction);
                    }

                    if (!comments.NullOrEmpty())
                    {
                        db.SaveAll(comments, transaction);
                        var rcomments = entities.Where(x => !x.Comments.OfType<COMMENT>().NullOrEmpty())
                            .SelectMany(e => e.Contacts.OfType<COMMENT>().Select(x => new REL_EVENTS_COMMENTS 
                            { 
                                Id = this.KeyGenerator.GetNextKey(), 
                                EventId = e.Id, 
                                CommentId = x.Id 
                            }));
                        var orcomments = db.Select<REL_EVENTS_COMMENTS>(q => Sql.In(q.EventId, keys));
                        db.SaveAll((!orcomments.NullOrEmpty()) ? rcomments.Except(orcomments) : rcomments, transaction);
                    }

                    if (!rdates.NullOrEmpty())
                    {
                        db.SaveAll(rdates, transaction);
                        var rrdates = entities.Where(x => !x.RecurrenceDates.OfType<RDATE>().NullOrEmpty())
                            .SelectMany(e => e.RecurrenceDates.OfType<RDATE>().Select(x => new REL_EVENTS_RDATES 
                            { 
                                Id = this.KeyGenerator.GetNextKey(), 
                                EventId = e.Id, 
                                RecurrenceDateId = x.Id 
                            }));
                        var orrdates = db.Select<REL_EVENTS_RDATES>(q => Sql.In(q.EventId, keys));
                        db.SaveAll((!orrdates.NullOrEmpty()) ? rrdates.Except(orrdates) : rrdates, transaction);
                    }

                    if (!exdates.NullOrEmpty())
                    {
                        db.SaveAll(exdates, transaction);
                        var rexdates = entities.Where(x => !x.ExceptionDates.OfType<EXDATE>().NullOrEmpty())
                            .SelectMany(e => e.ExceptionDates.OfType<EXDATE>().Select(x => new REL_EVENTS_EXDATES 
                            { 
                                Id = this.KeyGenerator.GetNextKey(), 
                                EventId = e.Id, 
                                ExceptionDateId = x.Id 
                            }));
                        var orexdates = db.Select<REL_EVENTS_EXDATES>(q => Sql.In(q.EventId, keys));
                        db.SaveAll((!orexdates.NullOrEmpty()) ? rexdates.Except(orexdates) : rexdates, transaction);
                    }

                    if (!relateds.NullOrEmpty())
                    {
                        db.SaveAll(relateds, transaction);
                        var rrelateds = entities.Where(x => !x.RelatedTos.OfType<RELATEDTO>().NullOrEmpty())
                            .SelectMany(e => e.RelatedTos.OfType<RELATEDTO>().Select(x => new REL_EVENTS_RELATEDTOS 
                            { 
                                Id = this.KeyGenerator.GetNextKey(), 
                                EventId = e.Id, 
                                RelatedToId = x.Id 
                            }));
                        var orrelateds = db.Select<REL_EVENTS_RELATEDTOS>(q => Sql.In(q.EventId, keys));
                        db.SaveAll((!orrelateds.NullOrEmpty())? rrelateds.Except(orrelateds): rrelateds, transaction);
                    }

                    if (!resources.NullOrEmpty())
                    {
                        db.SaveAll(resources, transaction);
                        var rresources = entities.Where(x => !x.Resources.OfType<RESOURCES>().NullOrEmpty())
                            .SelectMany(e => e.Resources.OfType<RESOURCES>().Select(x => new REL_EVENTS_RESOURCES 
                        { 
                            Id = this.KeyGenerator.GetNextKey(), 
                            EventId = e.Id, 
                            ResourcesId = x.Id 
                        }));
                        var orresources = db.Select<REL_EVENTS_RESOURCES>(q => Sql.In(q.EventId, keys));
                        db.SaveAll((!orresources.NullOrEmpty())? rresources.Except(orresources): rresources, transaction);
                    }

                    if (!reqstats.NullOrEmpty())
                    {
                        db.SaveAll(reqstats, transaction);
                        var rreqstats = entities.Where(x => !x.RequestStatuses.OfType<REQUEST_STATUS>().NullOrEmpty())
                            .SelectMany(e => e.RequestStatuses.OfType<REQUEST_STATUS>().Select(x => new REL_EVENTS_REQSTATS 
                            { 
                                Id = this.KeyGenerator.GetNextKey(), 
                                EventId = e.Id, 
                                ReqStatsId = x.Id 
                            }));
                        var orreqstats = db.Select<REL_EVENTS_REQSTATS>(q => Sql.In(q.EventId, keys));
                        db.SaveAll((!orreqstats.NullOrEmpty()) ? rreqstats.Except(orreqstats): rreqstats, transaction);
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

            #region 3. save aggregate attributes of entities

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
                        var raalarms = entities.Where(x => !x.Alarms.OfType<AUDIO_ALARM>().NullOrEmpty())
                            .SelectMany(e => e.RequestStatuses.OfType<AUDIO_ALARM>().Select(x => new REL_EVENTS_AUDIO_ALARMS 
                            { 
                                Id = this.KeyGenerator.GetNextKey(), 
                                EventId = e.Id, 
                                AlarmId = x.Id 
                            }));
                        var oraalarms = db.Select<REL_EVENTS_AUDIO_ALARMS>(q => Sql.In(q.EventId, entities.Select(x => x.Id)));
                        db.SaveAll((!oraalarms.NullOrEmpty()) ? raalarms.Except(oraalarms): raalarms, transaction);
                    }

                    if (!dalarms.NullOrEmpty())
                    {
                        var rdalarms = entities.Where(x => !x.Alarms.OfType<DISPLAY_ALARM>().NullOrEmpty()).SelectMany(e => e.RequestStatuses.OfType<DISPLAY_ALARM>().Select(x => new REL_EVENTS_DISPLAY_ALARMS 
                        { 
                            Id = this.KeyGenerator.GetNextKey(), 
                            EventId = e.Id, 
                            AlarmId = x.Id 
                        }));
                        var ordalarms = db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => Sql.In(q.EventId, entities.Select(x => x.Id)) && Sql.In(q.AlarmId, dalarms.Select(x => x.Id).ToArray()));
                        db.SaveAll((!ordalarms.NullOrEmpty()) ? rdalarms.Except(ordalarms) : rdalarms, transaction);
                    }

                    if (!ealarms.NullOrEmpty())
                    {
                        var realarms = entities.Where(x => !x.Alarms.OfType<EMAIL_ALARM>().NullOrEmpty())
                            .SelectMany(e => e.RequestStatuses.OfType<EMAIL_ALARM>().Select(x => new REL_EVENTS_EMAIL_ALARMS 
                            { 
                                Id = this.KeyGenerator.GetNextKey(), 
                                EventId = e.Id, 
                                AlarmId = x.Id 
                            }));

                        var orealarms = db.Select<REL_EVENTS_EMAIL_ALARMS>(q => Sql.In(q.EventId, entities.Select(x => x.Id)));
                        db.SaveAll((!orealarms.NullOrEmpty()) ? realarms.Except(orealarms): realarms, transaction);
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
            VEVENT full = null;
            try
            {
                full = db.Select<VEVENT>(q => q.Id == dry.Id).FirstOrDefault();

                if(full != null)
                {
                    var orgs = db.Select<ORGANIZER, VEVENT, REL_EVENTS_ORGANIZERS>(
                        r => r.OrganizerId,
                        r => r.EventId,
                        e => e.Id == full.Id);
                    if (!orgs.NullOrEmpty()) full.Organizer = orgs.FirstOrDefault();

                    var rids = db.Select<RECURRENCE_ID, VEVENT, REL_EVENTS_RECURRENCE_IDS>(
                        r => r.RecurrenceId_Id,
                        r => r.EventId,
                        e => e.Id == full.Id);
                    if (!rids.NullOrEmpty()) full.RecurrenceId = rids.FirstOrDefault();

                    var rrules = db.Select<RECUR, VEVENT, REL_EVENTS_RECURS>(
                        r => r.RecurrenceRuleId,
                        r => r.EventId,
                        e => e.Id == full.Id);
                    if (!rrules.NullOrEmpty()) full.RecurrenceRule = rrules.FirstOrDefault();

                    var attachbins = db.Select<ATTACH_BINARY, VEVENT, REL_EVENTS_ATTACHBINS>(
                       r => r.AttachmentId,
                       r => r.EventId,
                       e => e.Id == full.Id);
                    if (!attachbins.NullOrEmpty()) full.Attachments.AddRangeComplement(attachbins);

                    var attachuris = db.Select<ATTACH_URI, VEVENT, REL_EVENTS_ATTACHURIS>(
                       r => r.AttachmentId,
                       r => r.EventId,
                       e => e.Id == full.Id);
                    if (!attachuris.NullOrEmpty()) full.Attachments.AddRangeComplement(attachuris);

                    var attendees = db.Select<ATTENDEE, VEVENT, REL_EVENTS_ATTENDEES>(
                         r => r.AttendeeId,
                         r => r.EventId,
                         e => e.Id == full.Id);
                    if (!attendees.NullOrEmpty()) attendees.AddRangeComplement(attendees);

                    var comments = db.Select<COMMENT, VEVENT, REL_EVENTS_COMMENTS>(
                        r => r.CommentId,
                        r => r.EventId,
                        e => e.Id == full.Id);
                    if(!comments.NullOrEmpty()) full.Comments.AddRangeComplement(comments);

                    var contacts = db.Select<CONTACT, VEVENT, REL_EVENTS_CONTACTS>(
                        r => r.ContactId,
                        r => r.EventId,
                        e => e.Id == full.Id);
                    if(!contacts.NullOrEmpty()) full.Contacts.AddRangeComplement(contacts);

                    var rdates = db.Select<RDATE, VEVENT, REL_EVENTS_RDATES>(
                        r => r.RecurrenceDateId,
                        r => r.EventId,
                        e => e.Id == full.Id);
                    if(!rdates.NullOrEmpty()) full.RecurrenceDates.AddRangeComplement(rdates);

                    var exdates = db.Select<EXDATE, VEVENT, REL_EVENTS_EXDATES>(
                        r => r.ExceptionDateId,
                        r => r.EventId,
                        e => e.Id == full.Id);
                    if(!exdates.NullOrEmpty())full.ExceptionDates.AddRangeComplement(exdates);

                   var relatedtos= db.Select<RELATEDTO, VEVENT, REL_EVENTS_RELATEDTOS>(
                        r => r.RelatedToId,
                        r => r.EventId,
                        e => e.Id == full.Id);
                    full.RelatedTos.AddRangeComplement(relatedtos);

                    var reqstats = db.Select<REQUEST_STATUS, VEVENT, REL_EVENTS_REQSTATS>(
                        r => r.ReqStatsId,
                        r => r.EventId,
                        e => e.Id == full.Id);
                    full.RequestStatuses.AddRangeComplement(reqstats);

                    var resources = db.Select<RESOURCES, VEVENT, REL_EVENTS_RESOURCES>(
                        r => r.ResourcesId,
                        r => r.EventId,
                        e => e.Id == dry.Uid);
                    if (!resources.NullOrEmpty()) full.Resources.AddRangeComplement(resources);

                    var raalarms = this.db.Select<REL_EVENTS_AUDIO_ALARMS>(q => q.Id == full.Id);
                    if (!raalarms.NullOrEmpty())
                    {
                        full.Alarms.AddRangeComplement(this.AudioAlarmRepository.Find(raalarms.Select(x => x.AlarmId).ToList()));
                    }

                    var rdalarms = this.db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => q.Id == full.Id);
                    if (!rdalarms.NullOrEmpty())
                    {
                        full.Alarms.AddRangeComplement(this.DisplayAlarmRepository.Find(rdalarms.Select(x => x.AlarmId).ToList()));
                    }

                    var realarms = this.db.Select<REL_EVENTS_EMAIL_ALARMS>(q => q.Id == full.Id);
                    if (!realarms.NullOrEmpty())
                    {
                        full.Alarms.AddRangeComplement(this.EmailAlarmRepository
                            .Hydrate(this.EmailAlarmRepository.Find(realarms.Select(x => x.AlarmId).ToList())));
                    }
                }

            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }

            return full?? dry;

        }

        public IEnumerable<VEVENT> Hydrate(IEnumerable<VEVENT> dry)
        {
            List<VEVENT> full = null;
            try
            {
                var keys = dry.Select(q => q.Id).ToArray();
                full = db.Select<VEVENT>(q => Sql.In(q.Id, keys));
                if (!full.NullOrEmpty())
                {
                    #region 1. retrieve relationships

                    var rorgs = this.db.Select<REL_EVENTS_ORGANIZERS>(q => Sql.In(q.EventId, keys));
                    var rrids = this.db.Select<REL_EVENTS_RECURRENCE_IDS>(q => Sql.In(q.EventId, keys));
                    var rrrules = this.db.Select<REL_EVENTS_RECURS>(q => Sql.In(q.EventId, keys));
                    var rattendees = this.db.Select<REL_EVENTS_ATTENDEES>(q => Sql.In(q.EventId, keys));
                    var rcomments = this.db.Select<REL_EVENTS_COMMENTS>(q => Sql.In(q.EventId, keys));
                    var rattachbins = this.db.Select<REL_EVENTS_ATTACHBINS>(q => Sql.In(q.EventId, keys));
                    var rattachuris = this.db.Select<REL_EVENTS_ATTACHURIS>(q => Sql.In(q.EventId, keys));
                    var rcontacts = this.db.Select<REL_EVENTS_CONTACTS>(q => Sql.In(q.EventId, keys));
                    var rexdates = this.db.Select<REL_EVENTS_EXDATES>(q => Sql.In(q.EventId, keys));
                    var rrdates = this.db.Select<REL_EVENTS_RDATES>(q => Sql.In(q.EventId, keys));
                    var rrelatedtos = this.db.Select<REL_EVENTS_RELATEDTOS>(q => Sql.In(q.EventId, keys));
                    var rreqstats = this.db.Select<REL_EVENTS_REQSTATS>(q => Sql.In(q.EventId, keys));
                    var rresources = this.db.Select<REL_EVENTS_RESOURCES>(q => Sql.In(q.EventId, keys));
                    var raalarms = this.db.Select<REL_EVENTS_AUDIO_ALARMS>(q => Sql.In(q.EventId, keys));
                    var rdalarms = this.db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => Sql.In(q.EventId, keys));
                    var realarms = this.db.Select<REL_EVENTS_EMAIL_ALARMS>(q => Sql.In(q.EventId, keys)); 

                    #endregion

                    #region 2. retrieve secondary entities

                    var orgs = (!rorgs.Empty()) ? db.Select<ORGANIZER>(q => Sql.In(q.Id, rorgs.Select(r => r.OrganizerId).ToArray())) : null;
                    var rids = (!rrids.Empty()) ? db.Select<RECURRENCE_ID>(q => Sql.In(q.Id, rrids.Select(r => r.RecurrenceId_Id).ToArray())) : null;
                    var rrules = (!rrrules.Empty()) ? db.Select<RECUR>(q => Sql.In(q.Id, rrrules.Select(r => r.RecurrenceRuleId).ToArray())) : null;
                    var attendees = (!rattendees.Empty()) ? db.Select<ATTENDEE>(q => Sql.In(q.Id, rattendees.Select(r => r.AttendeeId).ToArray())) : null;
                    var comments = (!rcomments.Empty()) ? db.Select<COMMENT>(q => Sql.In(q.Id, rcomments.Select(r => r.CommentId).ToArray())) : null;
                    var attachbins = (!rattachbins.Empty()) ? db.Select<ATTACH_BINARY>(q => Sql.In(q.Id, rattachbins.Select(r => r.AttachmentId).ToArray())) : null;
                    var attachuris = (!rattachuris.Empty()) ? db.Select<ATTACH_URI>(q => Sql.In(q.Id, rattachuris.Select(r => r.AttachmentId).ToArray())) : null;
                    var contacts = (!rcontacts.Empty()) ? db.Select<CONTACT>(q => Sql.In(q.Id, rcontacts.Select(r => r.ContactId).ToArray())) : null;
                    var exdates = (!rexdates.Empty()) ? db.Select<EXDATE>(q => Sql.In(q.Id, rexdates.Select(r => r.ExceptionDateId).ToArray())) : null;
                    var rdates = (!rrdates.Empty()) ? db.Select<RDATE>(q => Sql.In(q.Id, rrdates.Select(r => r.RecurrenceDateId).ToArray())) : null;
                    var relatedtos = (!rrelatedtos.Empty()) ? db.Select<RELATEDTO>(q => Sql.In(q.Id, rrelatedtos.Select(r => r.RelatedToId).ToArray())) : null;
                    var reqstats = (!rreqstats.Empty()) ? db.Select<REQUEST_STATUS>(q => Sql.In(q.Id, rreqstats.Select(r => r.ReqStatsId).ToArray())) : null;
                    var resources = (!rresources.Empty()) ? db.Select<RESOURCES>(q => Sql.In(q.Id, rresources.Select(r => r.ResourcesId).ToArray())) : null;
                    var aalarms = (!raalarms.Empty()) ? this.AudioAlarmRepository.Find(raalarms.Select(x => x.AlarmId).ToList()) : null;
                    var dalarms = (!rdalarms.Empty()) ? this.DisplayAlarmRepository.Find(rdalarms.Select(x => x.AlarmId).ToList()) : null;
                    var ealarms = (!realarms.Empty()) ? this.EmailAlarmRepository.Hydrate(this.EmailAlarmRepository.Find(realarms.Select(x => x.AlarmId).ToList())) : null;

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
                            var xrrids = from y in rids
                                         join r in rrids on y.Id equals r.RecurrenceId_Id
                                         join e in full on r.EventId equals e.Id
                                         where e.Id == x.Id
                                         select y;
                            if (!xrrids.NullOrEmpty()) x.RecurrenceId = xrrids.FirstOrDefault();
                        }

                        if (!rrules.NullOrEmpty())
                        {
                            var xrrules = from y in rrules
                                          join r in rrrules on y.Id equals r.RecurrenceRuleId
                                          join e in full on r.EventId equals e.Id
                                          where e.Id == x.Id
                                          select y;
                            if (!xrrules.NullOrEmpty()) x.RecurrenceRule = xrrules.FirstOrDefault();
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
                            var xrdates = from y in rdates
                                          join r in rrdates on y.Id equals r.RecurrenceDateId
                                          join e in full on r.EventId equals e.Id
                                          where e.Id == x.Id
                                          select y;
                            if (!xrdates.NullOrEmpty()) x.RecurrenceDates.AddRangeComplement(xrdates);
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
                            var xrelatedtos = from y in relatedtos
                                              join r in rrelatedtos on y.Id equals r.RelatedToId
                                              join e in full on r.EventId equals e.Id
                                              where e.Id == x.Id
                                              select y;
                            if (!xrelatedtos.NullOrEmpty()) x.RelatedTos.AddRangeComplement(xrelatedtos);
                        }

                        if (!reqstats.NullOrEmpty())
                        {
                            var xreqstats = from y in reqstats
                                            join r in rreqstats on y.Id equals r.ReqStatsId
                                            join e in full on r.EventId equals e.Id
                                            where e.Id == x.Id
                                            select y;
                            if (!xreqstats.NullOrEmpty()) x.RequestStatuses.AddRangeComplement(xreqstats);

                        }

                        if (!resources.NullOrEmpty())
                        {
                            var xresources = from y in resources
                                             join r in rresources on y.Id equals r.ResourcesId
                                             join e in full on r.EventId equals e.Id
                                             where e.Id == x.Id
                                             select y;
                            if (!xresources.NullOrEmpty()) x.Resources.AddRangeComplement(xresources);
                        }

                        if (!aalarms.NullOrEmpty())
                        {
                            var xraalarms = from y in aalarms
                                            join r in raalarms on y.Id equals r.AlarmId
                                            join e in full on r.EventId equals e.Id
                                            where e.Id == x.Id
                                            select y;
                            if (!xraalarms.NullOrEmpty()) x.Alarms.AddRangeComplement(xraalarms);
                        }

                        if (!dalarms.NullOrEmpty())
                        {
                            var xrdalarms = from y in dalarms
                                            join r in rdalarms on y.Id equals r.AlarmId
                                            join e in full on r.EventId equals e.Id
                                            where e.Id == x.Id
                                            select y;
                            if (!xrdalarms.NullOrEmpty()) x.Alarms.AddRangeComplement(xrdalarms);

                        }

                        if (!ealarms.NullOrEmpty())
                        {
                            var xrealarms = from y in ealarms
                                            join r in realarms on y.Id equals r.AlarmId
                                            join e in full on r.EventId equals e.Id
                                            where e.Id == x.Id
                                            select y;
                            if (!xrealarms.NullOrEmpty()) x.Alarms.AddRangeComplement(xrealarms);
                        }
                    });

                    #endregion
                }

            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }

            return full ?? dry;
        }

        public bool ContainsKey(string key)
        {
            try
            {
                return db.Count<VEVENT>(q => q.Id == key) != 0;
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public bool ContainsKeys(IEnumerable<string> keys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            try
            {
                var dkeys = keys.Distinct().ToArray();
                if (mode == ExpectationMode.pessimistic || mode == ExpectationMode.unknown)
                    return db.Count<VEVENT>(q => Sql.In(q.Id, dkeys)) == dkeys.Count();
                else return db.Count<VEVENT>(q => Sql.In(q.Id, dkeys)) != 0;
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public IEnumerable<string> GetKeys(int? skip = null)
        {
            IEnumerable<string> keys = null;
            try
            {
                keys = db.SelectParam<VEVENT>(q => q.Id, skip, take);
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
            return keys;
        }

        public IEnumerable<VEVENT> Dehydrate(IEnumerable<VEVENT> full)
        {
            var dry = full.Select(x => {  return this.Dehydrate(x); });
            return dry;
        }

        public VEVENT Dehydrate(VEVENT full)
        {
            var dry = full;
            dry.Organizer = null;
            dry.RecurrenceId = null;
            dry.RecurrenceRule = null;
            dry.Attendees.Clear();
            dry.Attachments.Clear();
            dry.Contacts.Clear();
            dry.Comments.Clear();
            dry.RecurrenceDates.Clear();
            dry.ExceptionDates.Clear();
            dry.RelatedTos.Clear();
            dry.RequestStatuses.Clear();
            dry.Resources.Clear();
            dry.Alarms.Clear();
            return dry;
        }
    }
}
