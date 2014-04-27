using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using ServiceStack.OrmLite;
using reexmonkey.technical.data.contracts;
using reexmonkey.technical.data.concretes.extensions.ormlite;
using reexmonkey.crosscut.operations.concretes;
using reexmonkey.foundation.essentials.contracts;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.infrastructure.io.concretes;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.service.repositories.contracts;
using reexmonkey.infrastructure.operations.concretes;
using reexmonkey.infrastructure.operations.contracts;

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
            catch (ArgumentException) { throw; }
            catch (Exception) { throw; }
        }

        public IEnumerable<VEVENT> FindAll(IEnumerable<string> keys, int? skip = null)
        {
            try
            {
                var dkeys = keys.ToArray();
                return db.Select<VEVENT>(q => Sql.In(q.Id, dkeys), skip, take);
            }
            catch (InvalidOperationException) { throw; }
            catch (ArgumentException) { throw; }
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

            return dry;
        }

        public void Save(VEVENT entity)
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
            var relateds = entity.RelatedTos;
            var resources = entity.Resources;
            var reqstats = entity.RequestStatuses;
            var aalarms = entity.Alarms.OfType<AUDIO_ALARM>();
            var dalarms = entity.Alarms.OfType<DISPLAY_ALARM>();
            var ealarms = entity.Alarms.OfType<EMAIL_ALARM>(); 

            #endregion

            #region save attributes of event

            using (var transaction = db.OpenTransaction())
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

                    if (!aalarms.NullOrEmpty())
                    {
                        this.AudioAlarmRepository.SaveAll(aalarms);
                        var raalarms = aalarms.Select(x => new REL_EVENTS_AUDIO_ALARMS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = entity.Id,
                            AlarmId = x.Id
                        });
                        var oraalarms = db.Select<REL_EVENTS_AUDIO_ALARMS>(q => q.EventId == entity.Id);
                        db.SaveAll((!oraalarms.NullOrEmpty()) ? raalarms.Except(oraalarms) : raalarms, transaction);
                    }

                    if (!dalarms.NullOrEmpty())
                    {
                        this.DisplayAlarmRepository.SaveAll(dalarms);
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
                        this.EmailAlarmRepository.SaveAll(ealarms);
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
                catch (InvalidOperationException)
                {
                    try { transaction.Rollback(); }
                    catch (InvalidOperationException) { throw; }
                    catch (Exception) { throw; }
                }
                catch (Exception)
                {
                    try { transaction.Rollback(); }
                    catch (InvalidOperationException) { throw; }
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

            using (var transaction = db.BeginTransaction())
            {
                try
                {

                    string[] keys = null;
                    keys = (predicate != null)
                        ? db.SelectParam<VEVENT>(q => q.Id, predicate).ToArray()
                        : db.SelectParam<VEVENT>(q => q.Id).ToArray();

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

                        #region save relational attributes of entities

                        if (selection.Contains(orgexpr.GetMemberName()))
                        {
                            //get events-organizers relations
                            var org = source.Organizer;
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
                            var rrule = source.RecurrenceRule;
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
                            var attends = source.Attendees;
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
                                    ? rattachbins.Except(orattachbins)
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
                                db.SaveAll((!orattachuris.NullOrEmpty())
                                    ? rattachuris.Except(orattachuris)
                                    : rattachuris, transaction);
                            }
                        }

                        if (selection.Contains(contactsexpr.GetMemberName()))
                        {
                            var contacts = source.Contacts;
                            if (!contacts.NullOrEmpty())
                            {
                                db.SaveAll(contacts, transaction);
                                var rcontacts = keys.SelectMany(x => contacts.Select(y => new REL_EVENTS_CONTACTS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    ContactId = y.Id
                                }));
                                var orcontacts = db.Select<REL_EVENTS_CONTACTS>(q => Sql.In(q.EventId, keys));
                                db.SaveAll((!orcontacts.NullOrEmpty())
                                    ? rcontacts.Except(orcontacts)
                                    : rcontacts, transaction);
                            }
                        }

                        if (selection.Contains(commentsexpr.GetMemberName()))
                        {
                            var comments = source.Comments;
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
                            var rdates = source.Contacts;
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
                                    ? rrdates.Except(ordates)
                                    : rrdates, transaction);
                            }
                        }

                        if (selection.Contains(exdatesexpr.GetMemberName()))
                        {
                            var exdates = source.Contacts;
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
                                    ? rexdates.Except(orexdates)
                                    : rexdates, transaction);
                            }
                        }

                        if (selection.Contains(relatedtosexpr.GetMemberName()))
                        {
                            var relatedtos = source.RelatedTos;
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
                                    ? rrelatedtos.Except(orrelatedtos)
                                    : rrelatedtos, transaction);
                            }
                        }

                        if (selection.Contains(resourcesexpr.GetMemberName()))
                        {
                            var resources = source.Resources;
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
                                    ? rresources.Except(orresources)
                                    : rresources, transaction);
                            }
                        }

                        if (selection.Contains(reqstatsexpr.GetMemberName()))
                        {
                            var reqstats = source.RequestStatuses;
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
                                    ? rreqstats.Except(orreqstats)
                                    : rreqstats, transaction);
                            }
                        }

                        if (selection.Contains(alarmexpr.GetMemberName()))
                        {
                            var aalarms = source.Contacts.OfType<AUDIO_ALARM>();
                            var dalarms = source.Contacts.OfType<DISPLAY_ALARM>();
                            var ealarms = source.Contacts.OfType<EMAIL_ALARM>();

                            if (!aalarms.NullOrEmpty())
                            {
                                this.AudioAlarmRepository.SaveAll(aalarms);
                                var raalarms = keys.SelectMany(x => aalarms.Select(y => new REL_EVENTS_AUDIO_ALARMS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    AlarmId = y.Id
                                }));
                                var oraalarms = db.Select<REL_EVENTS_AUDIO_ALARMS>(q => Sql.In(q.EventId, keys));
                                db.SaveAll((!oraalarms.NullOrEmpty())
                                    ? raalarms.Except(oraalarms)
                                    : raalarms, transaction);
                            }

                            if (!dalarms.NullOrEmpty())
                            {
                                this.DisplayAlarmRepository.SaveAll(dalarms);
                                var rdalarms = keys.SelectMany(x => dalarms.Select(y => new REL_EVENTS_DISPLAY_ALARMS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    AlarmId = y.Id
                                }));
                                var ordalarms = db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => Sql.In(q.EventId, keys));
                                db.SaveAll((!ordalarms.NullOrEmpty())
                                    ? rdalarms.Except(ordalarms)
                                    : rdalarms, transaction);
                            }

                            if (!ealarms.NullOrEmpty())
                            {
                                this.EmailAlarmRepository.SaveAll(ealarms);
                                var realarms = keys.SelectMany(x => ealarms.Select(y => new REL_EVENTS_EMAIL_ALARMS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    EventId = x,
                                    AlarmId = y.Id
                                }));
                                var orealarms = db.Select<REL_EVENTS_EMAIL_ALARMS>(q => Sql.In(q.EventId, keys));
                                db.SaveAll((!orealarms.NullOrEmpty() && !realarms.Except(orealarms).NullOrEmpty())
                                    ? realarms.Except(orealarms)
                                    : realarms, transaction);
                            }

                        }


                        #endregion
                    }

                    if (!sprimitives.NullOrEmpty())
                    {
                        #region update-only non-relational attributes

                        var patchstr = string.Format("f => new {{ {0} }}", string.Join(", ", sprimitives.Select(x => string.Format("f.{0}", x))));
                        var patchexpr = patchstr.CompileToExpressionFunc<VEVENT, object>(CodeDomLanguage.csharp, Utilities.GetReferencedAssemblyNamesFromEntryAssembly());
                        db.UpdateOnly<VEVENT, object>(source, patchexpr, predicate);

                        #endregion
                    }

                    transaction.Commit();
                }
                catch (NotImplementedException)
                {
                    try { transaction.Rollback(); }
                    catch (InvalidOperationException) { throw; }
                    catch (Exception) { throw; }
                }
                catch (System.Security.SecurityException)
                {
                    try { transaction.Rollback(); }
                    catch (InvalidOperationException) { throw; }
                    catch (Exception) { throw; }
                }
                catch (InvalidOperationException)
                {
                    try { transaction.Rollback(); }
                    catch (InvalidOperationException) { throw; }
                    catch (Exception) { throw; }
                }
                catch (Exception)
                {
                    try { transaction.Rollback(); }
                    catch (InvalidOperationException) { throw; }
                    catch (Exception) { throw; }
                }

            } 
        }
   
        public void Erase(string key)
        {
            try
            {
                db.Delete<VEVENT>(q => q.Id == key);
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public void SaveAll(IEnumerable<VEVENT> entities)
        {
            #region 1. retrieve attributes of entities

            var orgs = entities.Where(x => x.Organizer != null).Select(x => x.Organizer);
            var rids = entities.Where(x => x.RecurrenceId != null).Select(x => x.RecurrenceId);
            var rrules = entities.Where(x => x.RecurrenceRule != null ).Select(x => x.RecurrenceRule);
            var attendees = entities.Where(x => !x.Attendees.NullOrEmpty()).SelectMany(x => x.Attendees);
            var attachbins = entities.Where(x => !x.Attachments.NullOrEmpty() && !x.Attachments.OfType<ATTACH_BINARY>().NullOrEmpty())
                .SelectMany(x => x.Attachments.OfType<ATTACH_BINARY>());
            var attachuris = entities.Where(x => !x.Attachments.NullOrEmpty() && !x.Attachments.OfType<ATTACH_URI>().NullOrEmpty())
                .SelectMany(x => x.Attachments.OfType<ATTACH_URI>());
            var contacts = entities.Where(x => !x.Contacts.NullOrEmpty()).SelectMany(x => x.Contacts);
            var comments = entities.Where(x => !x.Comments.NullOrEmpty()).SelectMany(x => x.Comments);
            var rdates = entities.Where(x => !x.RecurrenceDates.NullOrEmpty()).SelectMany(x => x.RecurrenceDates);
            var exdates = entities.Where(x => !x.ExceptionDates.NullOrEmpty()).SelectMany(x => x.ExceptionDates);
            var relateds = entities.Where(x => !x.RelatedTos.NullOrEmpty()).SelectMany(x => x.RelatedTos);
            var resources = entities.Where(x => !x.Resources.NullOrEmpty()).SelectMany(x => x.Resources);
            var reqstats = entities.Where(x => !x.Resources.NullOrEmpty()).SelectMany(x => x.RequestStatuses);
            var aalarms = entities.Where(x => !x.Alarms.NullOrEmpty() && !x.Alarms.OfType<AUDIO_ALARM>().NullOrEmpty())
                .SelectMany(x => x.Alarms.OfType<AUDIO_ALARM>());
            var dalarms = entities.Where(x => !x.Alarms.NullOrEmpty() && !x.Alarms.OfType<DISPLAY_ALARM>().NullOrEmpty())
                .SelectMany(x => x.Alarms.OfType<DISPLAY_ALARM>());
            var ealarms = entities.Where(x => !x.Alarms.NullOrEmpty() && !x.Alarms.OfType<EMAIL_ALARM>().NullOrEmpty())
                .SelectMany(x => x.Alarms.OfType<EMAIL_ALARM>());

            #endregion

            #region 2. save attributes of entities

            using (var transaction = db.BeginTransaction())
            {
                try
                {
                    //save core entities
                    var keys = entities.Select(x => x.Id).ToArray();
                    db.SaveAll(entities, transaction);

                    if (!orgs.NullOrEmpty())
                    {
                        db.SaveAll(orgs, transaction);
                        var rorgs = entities.Where(x => x.Organizer != null)
                            .Select(e => new REL_EVENTS_ORGANIZERS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                OrganizerId = e.Organizer.Id
                            }).ToArray();
                        var ororgs = db.Select<REL_EVENTS_ORGANIZERS>(q => Sql.In(q.EventId, keys));
                        db.SaveAll((!ororgs.NullOrEmpty())
                            ? rorgs.Except(ororgs)
                            : rorgs, transaction);
                    }

                    if (!rids.NullOrEmpty())
                    {
                        db.SaveAll(rids, transaction);
                        var rrids = entities.Where(x => x.RecurrenceId != null)
                            .Select(e => new REL_EVENTS_RECURRENCE_IDS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                RecurrenceId_Id = e.RecurrenceId.Id
                            });
                        var orrids = db.Select<REL_EVENTS_RECURRENCE_IDS>(q => Sql.In(q.EventId, keys));
                        db.SaveAll((!orrids.NullOrEmpty()) ? rrids.Except(orrids) : rrids, transaction);
                    }

                    if (!rrules.NullOrEmpty())
                    {
                        db.SaveAll(rrules, transaction);
                        var rrrules = entities.Where(x => x.RecurrenceRule != null)
                            .Select(e => new REL_EVENTS_RECURS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                RecurrenceRuleId = (e.RecurrenceRule as RECUR).Id
                            });

                        var orrrules = db.Select<REL_EVENTS_RECURS>(q => Sql.In(q.EventId, keys));
                        db.SaveAll((!orrrules.NullOrEmpty()) ? rrrules.Except(orrrules) : rrrules, transaction);
                    }

                    if (!attendees.NullOrEmpty())
                    {
                        db.SaveAll(attendees, transaction);
                        var rattends = entities.Where(x => !x.Attendees.NullOrEmpty())
                            .SelectMany(e => e.Attendees
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
                        db.SaveAll((!orattachbins.NullOrEmpty()) ? rattachbins.Except(orattachbins) : rattachbins, transaction);
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
                        db.SaveAll((!orattachuris.NullOrEmpty()) ? rattachuris.Except(orattachuris) : rattachuris, transaction);
                    }

                    if (!contacts.NullOrEmpty())
                    {
                        db.SaveAll(contacts, transaction);
                        var rcontacts = entities.Where(x => !x.Contacts.NullOrEmpty())
                            .SelectMany(e => e.Contacts.Select(x => new REL_EVENTS_CONTACTS
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
                        var rcomments = entities.Where(x => !x.Comments.NullOrEmpty())
                            .SelectMany(e => e.Contacts.Select(x => new REL_EVENTS_COMMENTS
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
                        var rrdates = entities.Where(x => !x.RecurrenceDates.NullOrEmpty())
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
                        var rexdates = entities.Where(x => !x.ExceptionDates.NullOrEmpty())
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
                        var rrelateds = entities.Where(x => !x.RelatedTos.NullOrEmpty())
                            .SelectMany(e => e.RelatedTos.Select(x => new REL_EVENTS_RELATEDTOS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                RelatedToId = x.Id
                            }));
                        var orrelateds = db.Select<REL_EVENTS_RELATEDTOS>(q => Sql.In(q.EventId, keys));
                        db.SaveAll((!orrelateds.NullOrEmpty()) ? rrelateds.Except(orrelateds) : rrelateds, transaction);
                    }

                    if (!resources.NullOrEmpty())
                    {
                        db.SaveAll(resources, transaction);
                        var rresources = entities.Where(x => !x.Resources.NullOrEmpty())
                            .SelectMany(e => e.Resources.Select(x => new REL_EVENTS_RESOURCES
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = e.Id,
                            ResourcesId = x.Id
                        }));
                        var orresources = db.Select<REL_EVENTS_RESOURCES>(q => Sql.In(q.EventId, keys));
                        db.SaveAll((!orresources.NullOrEmpty()) ? rresources.Except(orresources) : rresources, transaction);
                    }

                    if (!reqstats.NullOrEmpty())
                    {
                        db.SaveAll(reqstats, transaction);
                        var rreqstats = entities.Where(x => !x.RequestStatuses.NullOrEmpty())
                            .SelectMany(e => e.RequestStatuses.Select(x => new REL_EVENTS_REQSTATS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                ReqStatsId = x.Id
                            }));
                        var orreqstats = db.Select<REL_EVENTS_REQSTATS>(q => Sql.In(q.EventId, keys));
                        db.SaveAll((!orreqstats.NullOrEmpty()) ? rreqstats.Except(orreqstats) : rreqstats, transaction);
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
                        var oraalarms = db.Select<REL_EVENTS_AUDIO_ALARMS>(q => Sql.In(q.EventId, entities.Select(x => x.Id)));
                        db.SaveAll((!oraalarms.NullOrEmpty()) ? raalarms.Except(oraalarms) : raalarms, transaction);
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
                        var ordalarms = db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => Sql.In(q.EventId, entities.Select(x => x.Id)) && Sql.In(q.AlarmId, dalarms.Select(x => x.Id).ToArray()));
                        db.SaveAll((!ordalarms.NullOrEmpty()) ? rdalarms.Except(ordalarms) : rdalarms, transaction);
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

                        var orealarms = db.Select<REL_EVENTS_EMAIL_ALARMS>(q => Sql.In(q.EventId, entities.Select(x => x.Id)));
                        db.SaveAll((!orealarms.NullOrEmpty()) ? realarms.Except(orealarms) : realarms, transaction);
                    }

                    transaction.Commit();
                }
                catch (NullReferenceException)
                {
                    try { transaction.Rollback(); }
                    catch (InvalidOperationException) { throw; }
                }
                catch (InvalidOperationException)
                {
                    try { transaction.Rollback(); }
                    catch (InvalidOperationException) { throw; }
                }
                catch (Exception)
                {
                    try { transaction.Rollback(); }
                    catch (Exception) { throw; }
                }
            } 

            #endregion
        }

        public void EraseAll(IEnumerable<string> keys = null)
        {
            try
            {
                if(!keys.NullOrEmpty()) db.Delete<VEVENT>(q => Sql.In(q.Id, keys.ToArray()));
                else db.DeleteAll<VEVENT>();
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public VEVENT Hydrate(VEVENT dry)
        {
            VEVENT full = dry;
            try
            {
                var okey = db.SelectParam<VEVENT, string>(q => q.Id, p => p.Id == dry.Id).FirstOrDefault();
                if(!string.IsNullOrEmpty(okey))
                {
                    var orgs = db.Select<ORGANIZER, VEVENT, REL_EVENTS_ORGANIZERS>(
                        r => r.OrganizerId,
                        r => r.EventId,
                        e => e.Id == okey);
                    if (!orgs.NullOrEmpty()) full.Organizer = orgs.FirstOrDefault();

                    var rids = db.Select<RECURRENCE_ID, VEVENT, REL_EVENTS_RECURRENCE_IDS>(
                        r => r.RecurrenceId_Id,
                        r => r.EventId,
                        e => e.Id == okey);
                    if (!rids.NullOrEmpty()) full.RecurrenceId = rids.FirstOrDefault();

                    var rrules = db.Select<RECUR, VEVENT, REL_EVENTS_RECURS>(
                        r => r.RecurrenceRuleId,
                        r => r.EventId,
                        e => e.Id == okey);
                    if (!rrules.NullOrEmpty()) full.RecurrenceRule = rrules.FirstOrDefault();

                    var attachbins = db.Select<ATTACH_BINARY, VEVENT, REL_EVENTS_ATTACHBINS>(
                       r => r.AttachmentId,
                       r => r.EventId,
                       e => e.Id == okey);
                    if (!attachbins.NullOrEmpty()) full.Attachments.AddRangeComplement(attachbins);

                    var attachuris = db.Select<ATTACH_URI, VEVENT, REL_EVENTS_ATTACHURIS>(
                       r => r.AttachmentId,
                       r => r.EventId,
                       e => e.Id == okey);
                    if (!attachuris.NullOrEmpty()) full.Attachments.AddRangeComplement(attachuris);

                    var attendees = db.Select<ATTENDEE, VEVENT, REL_EVENTS_ATTENDEES>(
                         r => r.AttendeeId,
                         r => r.EventId,
                         e => e.Id == okey);
                    if (!attendees.NullOrEmpty()) attendees.AddRangeComplement(attendees);

                    var comments = db.Select<COMMENT, VEVENT, REL_EVENTS_COMMENTS>(
                        r => r.CommentId,
                        r => r.EventId,
                        e => e.Id == okey);
                    if(!comments.NullOrEmpty()) full.Comments.AddRangeComplement(comments);

                    var contacts = db.Select<CONTACT, VEVENT, REL_EVENTS_CONTACTS>(
                        r => r.ContactId,
                        r => r.EventId,
                        e => e.Id == okey);
                    if(!contacts.NullOrEmpty()) full.Contacts.AddRangeComplement(contacts);

                    var rdates = db.Select<RDATE, VEVENT, REL_EVENTS_RDATES>(
                        r => r.RecurrenceDateId,
                        r => r.EventId,
                        e => e.Id == okey);
                    if(!rdates.NullOrEmpty()) full.RecurrenceDates.AddRangeComplement(rdates);

                    var exdates = db.Select<EXDATE, VEVENT, REL_EVENTS_EXDATES>(
                        r => r.ExceptionDateId,
                        r => r.EventId,
                        e => e.Id == okey);
                    if(!exdates.NullOrEmpty())full.ExceptionDates.AddRangeComplement(exdates);

                   var relatedtos= db.Select<RELATEDTO, VEVENT, REL_EVENTS_RELATEDTOS>(
                        r => r.RelatedToId,
                        r => r.EventId,
                        e => e.Id == okey);
                    full.RelatedTos.AddRangeComplement(relatedtos);

                    var reqstats = db.Select<REQUEST_STATUS, VEVENT, REL_EVENTS_REQSTATS>(
                        r => r.ReqStatsId,
                        r => r.EventId,
                        e => e.Id == okey);
                    full.RequestStatuses.AddRangeComplement(reqstats);

                    var resources = db.Select<RESOURCES, VEVENT, REL_EVENTS_RESOURCES>(
                        r => r.ResourcesId,
                        r => r.EventId,
                        e => e.Id == okey);
                    if (!resources.NullOrEmpty()) full.Resources.AddRangeComplement(resources);

                    var raalarms = this.db.Select<REL_EVENTS_AUDIO_ALARMS>(q => q.Id == okey);
                    if (!raalarms.NullOrEmpty())
                    {
                        full.Alarms.AddRangeComplement(this.AudioAlarmRepository.FindAll(raalarms.Select(x => x.AlarmId).ToList()));
                    }

                    var rdalarms = this.db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => q.Id == okey);
                    if (!rdalarms.NullOrEmpty())
                    {
                        full.Alarms.AddRangeComplement(this.DisplayAlarmRepository.FindAll(rdalarms.Select(x => x.AlarmId).ToList()));
                    }

                    var realarms = this.db.Select<REL_EVENTS_EMAIL_ALARMS>(q => q.Id == okey);
                    if (!realarms.NullOrEmpty())
                    {
                        full.Alarms.AddRangeComplement(this.EmailAlarmRepository
                            .Hydrate(this.EmailAlarmRepository.FindAll(realarms.Select(x => x.AlarmId).ToList())));
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
                full = dry.ToList();
                var keys = full.Select(q => q.Id).ToArray();  
                var okeys = db.SelectParam<VEVENT, string>(q => q.Id, p => Sql.In(p.Id, keys));
                if (!okeys.NullOrEmpty())
                {
                    #region 1. retrieve relationships

                    var rorgs = this.db.Select<REL_EVENTS_ORGANIZERS>(q => Sql.In(q.EventId, okeys));
                    var rrids = this.db.Select<REL_EVENTS_RECURRENCE_IDS>(q => Sql.In(q.EventId, okeys));
                    var rrrules = this.db.Select<REL_EVENTS_RECURS>(q => Sql.In(q.EventId, okeys));
                    var rattendees = this.db.Select<REL_EVENTS_ATTENDEES>(q => Sql.In(q.EventId, okeys));
                    var rcomments = this.db.Select<REL_EVENTS_COMMENTS>(q => Sql.In(q.EventId, okeys));
                    var rattachbins = this.db.Select<REL_EVENTS_ATTACHBINS>(q => Sql.In(q.EventId, okeys));
                    var rattachuris = this.db.Select<REL_EVENTS_ATTACHURIS>(q => Sql.In(q.EventId, okeys));
                    var rcontacts = this.db.Select<REL_EVENTS_CONTACTS>(q => Sql.In(q.EventId, okeys));
                    var rexdates = this.db.Select<REL_EVENTS_EXDATES>(q => Sql.In(q.EventId, okeys));
                    var rrdates = this.db.Select<REL_EVENTS_RDATES>(q => Sql.In(q.EventId, okeys));
                    var rrelatedtos = this.db.Select<REL_EVENTS_RELATEDTOS>(q => Sql.In(q.EventId, okeys));
                    var rreqstats = this.db.Select<REL_EVENTS_REQSTATS>(q => Sql.In(q.EventId, okeys));
                    var rresources = this.db.Select<REL_EVENTS_RESOURCES>(q => Sql.In(q.EventId, okeys));
                    var raalarms = this.db.Select<REL_EVENTS_AUDIO_ALARMS>(q => Sql.In(q.EventId, okeys));
                    var rdalarms = this.db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => Sql.In(q.EventId, okeys));
                    var realarms = this.db.Select<REL_EVENTS_EMAIL_ALARMS>(q => Sql.In(q.EventId, okeys)); 

                    #endregion

                    #region 2. retrieve secondary entities

                    var orgs = (!rorgs.Empty()) ? db.Select<ORGANIZER>(q => Sql.In(q.Id, rorgs.Select(r => r.OrganizerId).ToList())) : null;
                    var rids = (!rrids.Empty()) ? db.Select<RECURRENCE_ID>(q => Sql.In(q.Id, rrids.Select(r => r.RecurrenceId_Id).ToList())) : null;
                    var rrules = (!rrrules.Empty()) ? db.Select<RECUR>(q => Sql.In(q.Id, rrrules.Select(r => r.RecurrenceRuleId).ToList())) : null;
                    var attendees = (!rattendees.Empty()) ? db.Select<ATTENDEE>(q => Sql.In(q.Id, rattendees.Select(r => r.AttendeeId).ToList())) : null;
                    var comments = (!rcomments.Empty()) ? db.Select<COMMENT>(q => Sql.In(q.Id, rcomments.Select(r => r.CommentId).ToList())) : null;
                    var attachbins = (!rattachbins.Empty()) ? db.Select<ATTACH_BINARY>(q => Sql.In(q.Id, rattachbins.Select(r => r.AttachmentId).ToList())) : null;
                    var attachuris = (!rattachuris.Empty()) ? db.Select<ATTACH_URI>(q => Sql.In(q.Id, rattachuris.Select(r => r.AttachmentId).ToList())) : null;
                    var contacts = (!rcontacts.Empty()) ? db.Select<CONTACT>(q => Sql.In(q.Id, rcontacts.Select(r => r.ContactId).ToList())) : null;
                    var exdates = (!rexdates.Empty()) ? db.Select<EXDATE>(q => Sql.In(q.Id, rexdates.Select(r => r.ExceptionDateId).ToList())) : null;
                    var rdates = (!rrdates.Empty()) ? db.Select<RDATE>(q => Sql.In(q.Id, rrdates.Select(r => r.RecurrenceDateId).ToList())) : null;
                    var relatedtos = (!rrelatedtos.Empty()) ? db.Select<RELATEDTO>(q => Sql.In(q.Id, rrelatedtos.Select(r => r.RelatedToId).ToList())) : null;
                    var reqstats = (!rreqstats.Empty()) ? db.Select<REQUEST_STATUS>(q => Sql.In(q.Id, rreqstats.Select(r => r.ReqStatsId).ToList())) : null;
                    var resources = (!rresources.Empty()) ? db.Select<RESOURCES>(q => Sql.In(q.Id, rresources.Select(r => r.ResourcesId).ToList())) : null;
                    var aalarms = (!raalarms.Empty()) ? this.AudioAlarmRepository.FindAll(raalarms.Select(x => x.AlarmId).ToList()) : null;
                    var dalarms = (!rdalarms.Empty()) ? this.DisplayAlarmRepository.FindAll(rdalarms.Select(x => x.AlarmId).ToList()) : null;
                    var ealarms = (!realarms.Empty()) ? this.EmailAlarmRepository.FindAll(realarms.Select(x => x.AlarmId).ToList()) : null;

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
            catch (ApplicationException) { throw; }
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
            catch (ApplicationException) { throw; }
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
            catch (ApplicationException) { throw; }
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
            if(!dry.Attendees.NullOrEmpty()) dry.Attendees.Clear();
            if (!dry.Attachments.NullOrEmpty()) dry.Attachments.Clear();
            if (!dry.Contacts.NullOrEmpty()) dry.Contacts.Clear();
            if (!dry.Comments.NullOrEmpty()) dry.Comments.Clear();
            if (!dry.RecurrenceDates.NullOrEmpty()) dry.RecurrenceDates.Clear();
            if (!dry.ExceptionDates.NullOrEmpty()) dry.ExceptionDates.Clear();
            if (!dry.RelatedTos.NullOrEmpty()) dry.RelatedTos.Clear();
            if (!dry.RequestStatuses.NullOrEmpty()) dry.RequestStatuses.Clear();
            if (!dry.Resources.NullOrEmpty()) dry.Resources.Clear();
            if (!dry.Alarms.NullOrEmpty()) dry.Alarms.Clear();
            return dry;
        }
    }
}
