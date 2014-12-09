using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Security;
using ServiceStack.OrmLite;
using reexjungle.technical.data.contracts;
using reexjungle.technical.data.concretes.extensions.ormlite;
using reexjungle.crosscut.operations.concretes;
using reexjungle.foundation.essentials.contracts;
using reexjungle.foundation.essentials.concretes;
using reexjungle.infrastructure.io.concretes;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.repositories.contracts;
using reexjungle.infrastructure.operations.concretes;
using reexjungle.infrastructure.operations.contracts;
using reexjungle.xcal.service.repositories.concretes.relations;


namespace reexjungle.xcal.service.repositories.concretes.ormlite
{
    /// <summary>
    /// Reüpresents a a repository of events connected to an ORMlite source
    /// </summary>
    public class EventOrmLiteRepository: IEventOrmLiteRepository, IDisposable
    {
        private IDbConnection conn = null;
        private IDbConnectionFactory factory = null;
        private IKeyGenerator<string> keygen;
        private IAudioAlarmRepository aalarmrepository = null;
        private IDisplayAlarmRepository dalarmrepository = null;
        private IEmailAlarmRepository ealarmrepository = null;
        private IDbConnection db
        {
            get { return (this.conn) ?? (this.conn = factory.OpenDbConnection()); }
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

        public IKeyGenerator<string> KeyGenerator
        {
            get { return this.keygen; }
            set 
            {
                if (value == null) throw new ArgumentNullException("KeyGenerator");
                this.keygen = value;
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

        public EventOrmLiteRepository() { }

        public EventOrmLiteRepository(IDbConnectionFactory factory)
        {
            this.DbConnectionFactory = factory;
        }

        public EventOrmLiteRepository(IDbConnection connection, int? take)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            this.conn = connection; 
        }

        public void Dispose()
        {
            if (this.conn != null) this.conn.Dispose();
        }

        public VEVENT Find(string key)
        {
            try
            {
               var dry = db.Select<VEVENT>(q => q.Id == key).FirstOrDefault();
               return dry != null ? this.Hydrate(dry) : dry;
            }

            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
            catch (Exception) { throw; }
        }

        public IEnumerable<VEVENT> FindAll(IEnumerable<string> keys, int? skip = null, int? take = null)
        {
            try
            {
                var dry = db.Select<VEVENT>(q => Sql.In(q.Id, keys.ToArray()), skip, take);
                return !dry.NullOrEmpty() ? this.HydrateAll(dry) : dry;
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        public IEnumerable<VEVENT> Get(int? skip = null, int? take  = null)
        {
            try
            {
                var dry = db.Select<VEVENT>(skip, take);
                return !dry.NullOrEmpty() ? this.HydrateAll(dry) : dry;
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        public void Save(VEVENT entity)
        {

            #region retrieve attributes of entity

            var org = entity.Organizer;
            var rid = entity.RecurrenceId;
            var rrule = entity.RecurrenceRule;
            var attendees = entity.Attendees;
            var attachbins = entity.AttachmentBinaries;
            var attachuris = entity.AttachmentUris;
            var contacts = entity.Contacts;
            var comments = entity.Comments;
            var rdates = entity.RecurrenceDates;
            var exdates = entity.ExceptionDates;
            var relateds = entity.RelatedTos;
            var resources = entity.Resources;
            var reqstats = entity.RequestStatuses;
            var aalarms = entity.AudioAlarms;
            var dalarms = entity.DisplayAlarms;
            var ealarms = entity.EmailAlarms; 

            #endregion

            #region save event and its attributes
            try
            {
                db.Save(entity);
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
                    db.SynchronizeAll(rorg.ToSingleton(), ororgs);
                }
                else
                {
                    var ororgs = db.Select<REL_EVENTS_ORGANIZERS>(q => q.EventId == entity.Id);
                    if (!ororgs.NullOrEmpty()) db.DeleteById<ORGANIZER>(ororgs.Select(x => x.OrganizerId));

                }

                if (rid != null)
                {
                    db.Save(rid);
                    var rrid = new REL_EVENTS_RECURRENCE_IDS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        RecurrenceId_Id = rid.Id
                    };
                    var orrids = db.Select<REL_EVENTS_RECURRENCE_IDS>(q => q.EventId == entity.Id);
                    db.SynchronizeAll(rrid.ToSingleton(), orrids);
                }
                else
                {
                    var orrids = db.Select<REL_EVENTS_RECURRENCE_IDS>(q => q.EventId == entity.Id);
                    if (!orrids.NullOrEmpty()) db.DeleteById<RECURRENCE_ID>(orrids.Select(x => x.RecurrenceId_Id));
                }

                if (rrule != null)
                {
                    db.Save(rrule);
                    var rrrule = new REL_EVENTS_RECURS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        RecurrenceRuleId = rrule.Id
                    };
                    var orrrules = db.Select<REL_EVENTS_RECURS>(q => q.EventId == entity.Id);
                    db.SynchronizeAll(rrrule.ToSingleton(), orrrules);
                }
                else
                {
                    var orrrules = db.Select<REL_EVENTS_RECURS>(q => q.EventId == entity.Id);
                    if (!orrrules.NullOrEmpty()) db.DeleteById<RECUR>(orrrules.Select(x => x.RecurrenceRuleId));
                }

                if (!attendees.NullOrEmpty())
                {
                    db.SaveAll(attendees.Distinct());
                    var rattendees = attendees.Select(x => new REL_EVENTS_ATTENDEES
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        AttendeeId = x.Id
                    });
                    var orattendees = db.Select<REL_EVENTS_ATTENDEES>(q => q.EventId == entity.Id);
                    db.SynchronizeAll(rattendees, orattendees);
                }
                else if(attendees.SafeEmpty())
                {
                    var orattendees = db.Select<REL_EVENTS_ATTENDEES>(q => q.EventId == entity.Id);
                    if(!orattendees.NullOrEmpty()) db.DeleteByIds<ATTENDEE>(orattendees.Select(x => x.AttendeeId));
                }

                if (!attachbins.NullOrEmpty())
                {
                    db.SaveAll(attachbins.Distinct());
                    var rattachbins = attachbins.Select(x => new REL_EVENTS_ATTACHBINS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        AttachmentId = x.Id
                    });
                    var orattachbins = db.Select<REL_EVENTS_ATTACHBINS>(q => q.EventId == entity.Id);
                    db.SynchronizeAll(rattachbins, orattachbins);
                }
                else if (attachbins.SafeEmpty())
                {
                    var orattachbins = db.Select<REL_EVENTS_ATTACHBINS>(q => q.EventId == entity.Id);
                    if (!orattachbins.NullOrEmpty()) db.DeleteByIds<ATTACH_BINARY>(orattachbins.Select(x => x.AttachmentId));
                } 

                if (!attachuris.NullOrEmpty())
                {
                    db.SaveAll(attachuris.Distinct());
                    var rattachuris = attachuris.Select(x => new REL_EVENTS_ATTACHURIS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        AttachmentId = x.Id
                    });
                    var orattachuris = db.Select<REL_EVENTS_ATTACHURIS>(q => q.EventId == entity.Id);
                    db.SynchronizeAll(rattachuris, orattachuris);
                }
                else if (attachuris.SafeEmpty())
                {
                    var orattachuris = db.Select<REL_EVENTS_ATTACHURIS>(q => q.EventId == entity.Id);
                    if (!orattachuris.NullOrEmpty()) db.DeleteByIds<ATTACH_URI>(orattachuris.Select(x => x.AttachmentId));
                }                

                if (!contacts.NullOrEmpty())
                {
                    db.SaveAll(contacts.Distinct());
                    var rcontacts = contacts.Select(x => new REL_EVENTS_CONTACTS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        ContactId = x.Id
                    });
                    var orcontacts = db.Select<REL_EVENTS_CONTACTS>(q => q.EventId == entity.Id);
                    db.SynchronizeAll(rcontacts, orcontacts);
                }
                else if(contacts.SafeEmpty())
                {
                    var orcontacts = db.Select<REL_EVENTS_CONTACTS>(q => q.EventId == entity.Id);
                    if (!orcontacts.NullOrEmpty()) db.DeleteByIds<CONTACT>(orcontacts.Select(x => x.ContactId));
                }

                if (!comments.NullOrEmpty())
                {
                    db.SaveAll(comments.Distinct());
                    var rcomments = comments.Select(x => new REL_EVENTS_COMMENTS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        CommentId = x.Id
                    });
                    var orcomments = db.Select<REL_EVENTS_COMMENTS>(q => q.EventId == entity.Id);
                    db.SynchronizeAll(rcomments, orcomments);
                }
                else if (comments.SafeEmpty())
                {
                    var orcomments = db.Select<REL_EVENTS_COMMENTS>(q => q.EventId == entity.Id);
                    if (!orcomments.NullOrEmpty()) db.DeleteByIds<COMMENT>(orcomments.Select(x => x.CommentId));
                }

                if (!rdates.NullOrEmpty())
                {
                    db.SaveAll(rdates.Distinct());
                    var rrdates = rdates.Select(x => new REL_EVENTS_RDATES
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        RecurrenceDateId = x.Id
                    });
                    var orrdates = db.Select<REL_EVENTS_RDATES>(q => q.EventId == entity.Id);
                    db.SynchronizeAll(rrdates, orrdates);
                }
                else if (rdates.SafeEmpty())
                {
                    var orrdates = db.Select<REL_EVENTS_RDATES>(q => q.EventId == entity.Id);
                    if (!orrdates.NullOrEmpty()) db.DeleteByIds<RDATE>(orrdates.Select(x => x.RecurrenceDateId));
                }

                if (!exdates.NullOrEmpty())
                {
                    db.SaveAll(exdates.Distinct());
                    var rexdates = exdates.Select(x => new REL_EVENTS_EXDATES
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        ExceptionDateId = x.Id
                    });
                    var orexdates = db.Select<REL_EVENTS_EXDATES>(q => q.EventId == entity.Id);
                    db.SynchronizeAll(rexdates, orexdates);
                }
                else if (exdates.SafeEmpty())
                {
                    var orexdates = db.Select<REL_EVENTS_EXDATES>(q => q.EventId == entity.Id);
                    if (!orexdates.NullOrEmpty()) db.DeleteByIds<EXDATE>(orexdates.Select(x => x.ExceptionDateId));
                }

                if (!relateds.NullOrEmpty())
                {
                    db.SaveAll(relateds.Distinct());
                    var rrelateds = relateds.Select(x => new REL_EVENTS_RELATEDTOS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        RelatedToId = x.Id
                    });
                    var orrelateds = db.Select<REL_EVENTS_RELATEDTOS>(q => q.EventId == entity.Id);
                    db.SynchronizeAll(rrelateds, orrelateds);
                }
                else if (relateds.SafeEmpty())
                {
                    var orrelateds = db.Select<REL_EVENTS_RELATEDTOS>(q => q.EventId == entity.Id);
                    if (!orrelateds.NullOrEmpty()) db.DeleteByIds<RELATEDTO>(orrelateds.Select(x => x.RelatedToId));
                }

                if (!resources.NullOrEmpty())
                {
                    db.SaveAll(resources.Distinct());
                    var rresources = resources.Select(x => new REL_EVENTS_RESOURCES
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        ResourcesId = x.Id
                    });
                    var orresources = db.Select<REL_EVENTS_RESOURCES>(q => q.EventId == entity.Id);
                    db.SynchronizeAll(rresources, orresources);
                }
                else if (resources.SafeEmpty())
                {
                    var orresources = db.Select<REL_EVENTS_RESOURCES>(q => q.EventId == entity.Id);
                    if (!orresources.NullOrEmpty()) db.DeleteByIds<RESOURCES>(orresources.Select(x => x.ResourcesId));
                }

                if (!reqstats.NullOrEmpty())
                {
                    db.SaveAll(reqstats.Distinct());
                    var rreqstats = reqstats.Select(x => new REL_EVENTS_REQSTATS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        ReqStatsId = x.Id
                    });
                    var orreqstats = db.Select<REL_EVENTS_REQSTATS>(q => q.EventId == entity.Id);
                    db.SynchronizeAll(rreqstats, orreqstats);
                }
                else if (reqstats.SafeEmpty())
                {
                    var oreqstats = db.Select<REL_EVENTS_REQSTATS>(q => q.EventId == entity.Id);
                    if (!oreqstats.NullOrEmpty()) db.DeleteByIds<REQUEST_STATUS>(oreqstats.Select(x => x.ReqStatsId));
                }

                if (!aalarms.NullOrEmpty())
                {
                    this.AudioAlarmRepository.SaveAll(aalarms.Distinct());
                    var raalarms = aalarms.Select(x => new REL_EVENTS_AUDIO_ALARMS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        AlarmId = x.Id
                    });
                    var oraalarms = db.Select<REL_EVENTS_AUDIO_ALARMS>(q => q.EventId == entity.Id);
                    db.SynchronizeAll(raalarms, oraalarms);
                }
                else if (aalarms.SafeEmpty())
                {
                    var oraalarms = db.Select<REL_EVENTS_AUDIO_ALARMS>(q => q.EventId == entity.Id);
                    if (!oraalarms.NullOrEmpty()) db.DeleteByIds<AUDIO_ALARM>(oraalarms.Select(x => x.AlarmId));
                }

                if (!dalarms.NullOrEmpty())
                {
                    this.DisplayAlarmRepository.SaveAll(dalarms.Distinct());
                    var rdalarms = dalarms.Select(x => new REL_EVENTS_DISPLAY_ALARMS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        AlarmId = x.Id
                    });
                    var ordalarms = db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => q.EventId == entity.Id);
                    db.SynchronizeAll(rdalarms, ordalarms);
                }
                else if (dalarms.SafeEmpty())
                {
                    var ordalarms = db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => q.EventId == entity.Id);
                    if (!ordalarms.NullOrEmpty()) db.DeleteByIds<DISPLAY_ALARM>(ordalarms.Select(x => x.AlarmId));
                }

                if (!ealarms.NullOrEmpty())
                {
                    this.EmailAlarmRepository.SaveAll(ealarms.Distinct());
                    var realarms = ealarms.Select(x => new REL_EVENTS_EMAIL_ALARMS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        AlarmId = x.Id
                    });
                    var orealarms = db.Select<REL_EVENTS_EMAIL_ALARMS>(q => q.EventId == entity.Id);
                    db.SynchronizeAll(realarms, orealarms);
                }
                else if (ealarms.SafeEmpty())
                {
                    var orealarms = db.Select<REL_EVENTS_EMAIL_ALARMS>(q => q.EventId == entity.Id);
                    if (!orealarms.NullOrEmpty()) db.DeleteByIds<EMAIL_ALARM>(orealarms.Select(x => x.AlarmId));
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }

            #endregion

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
                x.Classification,
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
                x.EmailAlarms
            };

            //4. Get list of selected relationals
            var srelation = relations.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);

            //5. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);

            #endregion
            
            try
            {
                var okeys = (keys != null)
                    ? db.SelectParam<VEVENT, string>(q => q.Id, p => Sql.In(p.Id, keys.ToArray())).ToArray()
                    : db.SelectParam<VEVENT>(q => q.Id).ToArray();

                if (!srelation.NullOrEmpty())
                {
                    Expression<Func<VEVENT, object>> orgexpr = y => y.Organizer;
                    Expression<Func<VEVENT, object>> ridexpr = y => y.RecurrenceId;
                    Expression<Func<VEVENT, object>> rruleexpr = y => y.RecurrenceRule;
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

                    #region save relational attributes of entities

                    if (selection.Contains(orgexpr.GetMemberName()))
                    {
                        //get events-organizers relations
                        var org = source.Organizer;
                        if (org != null)
                        {
                            db.Save(org);
                            var rorgs = okeys.Select(x => new REL_EVENTS_ORGANIZERS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                OrganizerId = org.Id
                            });
                            var ororgs = db.Select<REL_EVENTS_ORGANIZERS>(q => Sql.In(q.EventId, okeys));
                            db.SynchronizeAll(rorgs, ororgs);

                        }
                    }
                    if (selection.Contains(ridexpr.GetMemberName()))
                    {
                        //get events-organizers relations
                        var rid = source.RecurrenceId;
                        if (rid != null)
                        {
                            db.Save(rid);
                            var rrids = okeys.Select(x => new REL_EVENTS_RECURRENCE_IDS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                RecurrenceId_Id = rid.Id
                            });
                            var orrids = db.Select<REL_EVENTS_RECURRENCE_IDS>(q => Sql.In(q.EventId, okeys));
                            db.SynchronizeAll(rrids, orrids);
                        }
                    }
                    if (selection.Contains(rruleexpr.GetMemberName()))
                    {
                        var rrule = source.RecurrenceRule;
                        if (rrule != null)
                        {
                            db.Save(rrule);
                            var rrrules = okeys.Select(x => new REL_EVENTS_RECURS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                RecurrenceRuleId = rrule.Id
                            });
                            var orrrules = db.Select<REL_EVENTS_RECURS>(q => Sql.In(q.EventId, okeys));
                            db.SynchronizeAll(rrrules, orrrules);
                        }
                    }

                    if (selection.Contains(attendsexpr.GetMemberName()))
                    {
                        var attendees = source.Attendees;
                        if (!attendees.NullOrEmpty())
                        {
                            db.SaveAll(attendees.Distinct());
                            var rattendees = okeys.SelectMany(x => attendees.Select(y => new REL_EVENTS_ATTENDEES
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                AttendeeId = y.Id
                            }));
                            var orattendees = db.Select<REL_EVENTS_ATTENDEES>(q => Sql.In(q.EventId, okeys));
                            db.SynchronizeAll(rattendees, orattendees);
                        }
                    }

                    if (selection.Contains(attachbinsexpr.GetMemberName()))
                    {
                        var attachbins = source.AttachmentUris;
                        if (!attachbins.NullOrEmpty())
                        {
                            db.SaveAll(attachbins.Distinct());
                            var rattachbins = okeys.SelectMany(x => attachbins.Select(y => new REL_EVENTS_ATTACHBINS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                AttachmentId = y.Id
                            }));
                            var orattachbins = db.Select<REL_EVENTS_ATTACHBINS>(q => Sql.In(q.EventId, okeys));
                            db.SynchronizeAll(rattachbins, orattachbins);
                        }
                    }
                    if (selection.Contains(attachurisexpr.GetMemberName()))
                    {
                        var attachuris = source.AttachmentUris;
                        if (!attachuris.NullOrEmpty())
                        {
                            db.SaveAll(attachuris.Distinct());
                            var rattachuris = okeys.SelectMany(x => attachuris.Select(y => new REL_EVENTS_ATTACHURIS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                AttachmentId = y.Id
                            }));
                            var orattachuris = db.Select<REL_EVENTS_ATTACHURIS>(q => Sql.In(q.EventId, okeys));
                            db.SynchronizeAll(rattachuris, orattachuris);
                        }
                    }

                    if (selection.Contains(contactsexpr.GetMemberName()))
                    {
                        var contacts = source.Contacts;
                        if (!contacts.NullOrEmpty())
                        {
                            db.SaveAll(contacts.Distinct());
                            var rcontacts = okeys.SelectMany(x => contacts.Select(y => new REL_EVENTS_CONTACTS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                ContactId = y.Id
                            }));
                            var orcontacts = db.Select<REL_EVENTS_CONTACTS>(q => Sql.In(q.EventId, okeys));
                            db.SynchronizeAll(rcontacts, orcontacts);
                        }
                    }

                    if (selection.Contains(commentsexpr.GetMemberName()))
                    {
                        var comments = source.Comments;
                        if (!comments.NullOrEmpty())
                        {
                            db.SaveAll(comments.Distinct());
                            var rcomments = okeys.SelectMany(x => comments.Select(y => new REL_EVENTS_COMMENTS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                CommentId = y.Id
                            }));
                            var orcomments = db.Select<REL_EVENTS_COMMENTS>(q => Sql.In(q.EventId, okeys));
                            db.SynchronizeAll(rcomments, orcomments);
                        }
                    }

                    if (selection.Contains(rdatesexpr.GetMemberName()))
                    {
                        var rdates = source.Contacts;
                        if (!rdates.NullOrEmpty())
                        {
                            db.SaveAll(rdates.Distinct());
                            var rrdates = okeys.SelectMany(x => rdates.Select(y => new REL_EVENTS_RDATES
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                RecurrenceDateId = y.Id
                            }));
                            var orrdates = db.Select<REL_EVENTS_RDATES>(q => Sql.In(q.EventId, okeys));
                            db.SynchronizeAll(rrdates, orrdates);
                        }
                    }

                    if (selection.Contains(exdatesexpr.GetMemberName()))
                    {
                        var exdates = source.Contacts;
                        if (!exdates.NullOrEmpty())
                        {
                            db.SaveAll(exdates.Distinct());
                            var rexdates = okeys.SelectMany(x => exdates.Select(y => new REL_EVENTS_EXDATES
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                ExceptionDateId = y.Id
                            }));
                            var orexdates = db.Select<REL_EVENTS_EXDATES>(q => Sql.In(q.EventId, okeys));
                            db.SynchronizeAll(rexdates, orexdates);
                        }
                    }

                    if (selection.Contains(relatedtosexpr.GetMemberName()))
                    {
                        var relatedtos = source.RelatedTos;
                        if (!relatedtos.NullOrEmpty())
                        {
                            db.SaveAll(relatedtos.Distinct());
                            var rrelatedtos = okeys.SelectMany(x => relatedtos.Select(y => new REL_EVENTS_RELATEDTOS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                RelatedToId = y.Id
                            }));
                            var orrelatedtos = db.Select<REL_EVENTS_RELATEDTOS>(q => Sql.In(q.EventId, okeys));
                            db.SynchronizeAll(rrelatedtos, orrelatedtos);
                        }
                    }

                    if (selection.Contains(resourcesexpr.GetMemberName()))
                    {
                        var resources = source.Resources;
                        if (!resources.NullOrEmpty())
                        {
                            db.SaveAll(resources.Distinct());
                            var rresources = okeys.SelectMany(x => resources.Select(y => new REL_EVENTS_RESOURCES
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                ResourcesId = y.Id
                            }));
                            var orresources = db.Select<REL_EVENTS_RESOURCES>(q => Sql.In(q.EventId, okeys));
                            db.SynchronizeAll(rresources, orresources);
                        }
                    }

                    if (selection.Contains(reqstatsexpr.GetMemberName()))
                    {
                        var reqstats = source.RequestStatuses;
                        if (!reqstats.NullOrEmpty())
                        {
                            db.SaveAll(reqstats.Distinct());
                            var rreqstats = okeys.SelectMany(x => reqstats.Select(y => new REL_EVENTS_REQSTATS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                ReqStatsId = y.Id
                            }));
                            var orreqstats = db.Select<REL_EVENTS_REQSTATS>(q => Sql.In(q.EventId, okeys));
                            db.SynchronizeAll(rreqstats, orreqstats);
                        }
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
                            var oraalarms = db.Select<REL_EVENTS_AUDIO_ALARMS>(q => Sql.In(q.EventId, okeys));
                            db.SynchronizeAll(raalarms, oraalarms);
                        }
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
                            var ordalarms = db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => Sql.In(q.EventId, okeys));
                            db.SynchronizeAll(rdalarms, ordalarms);
                        }

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
                            var orealarms = db.Select<REL_EVENTS_EMAIL_ALARMS>(q => Sql.In(q.EventId, okeys));
                            db.SynchronizeAll(realarms, orealarms);
                        }
                    }

                    #endregion
                }

                if (!sprimitives.NullOrEmpty())
                {
                    var patchstr = string.Format("f => new {{ {0} }}", string.Join(", ", sprimitives.Select(x => string.Format("f.{0}", x))));

                    //throw new FormatException(patchstr);

                    var patchexpr = patchstr.CompileToExpressionFunc<VEVENT, object>(CodeDomLanguage.csharp, new string[] { "System.dll", "System.Core.dll", typeof(VEVENT).Assembly.Location, typeof(IContainsKey<string>).Assembly.Location });

                    if (!keys.NullOrEmpty()) db.UpdateOnly<VEVENT, object>(source, patchexpr, q => Sql.In(q.Id, keys.ToArray()));
                    else db.UpdateOnly<VEVENT, object>(source, patchexpr);
                }
            }

            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }
   
        public void Erase(string key)
        {
            try
            {
                db.Delete<VEVENT>(q => q.Id == key);
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        public void SaveAll(IEnumerable<VEVENT> entities)
        {
            #region 1. retrieve attributes of entities

            var orgs = entities.Where(x => x.Organizer != null).Select(x => x.Organizer);
            var rids = entities.Where(x => x.RecurrenceId != null).Select(x => x.RecurrenceId);
            var rrules = entities.Where(x => x.RecurrenceRule != null ).Select(x => x.RecurrenceRule);
            var attendees = entities.Where(x => !x.Attendees.NullOrEmpty()).SelectMany(x => x.Attendees);
            var attachbins = entities.Where(x => !x.AttachmentBinaries.NullOrEmpty()).SelectMany(x => x.AttachmentBinaries);
            var attachuris = entities.Where(x => !x.AttachmentUris.NullOrEmpty()).SelectMany(x => x.AttachmentUris);
            var contacts = entities.Where(x => !x.Contacts.NullOrEmpty()).SelectMany(x => x.Contacts);
            var comments = entities.Where(x => !x.Comments.NullOrEmpty()).SelectMany(x => x.Comments);
            var rdates = entities.Where(x => !x.RecurrenceDates.NullOrEmpty()).SelectMany(x => x.RecurrenceDates);
            var exdates = entities.Where(x => !x.ExceptionDates.NullOrEmpty()).SelectMany(x => x.ExceptionDates);
            var relateds = entities.Where(x => !x.RelatedTos.NullOrEmpty()).SelectMany(x => x.RelatedTos);
            var resources = entities.Where(x => !x.Resources.NullOrEmpty()).SelectMany(x => x.Resources);
            var reqstats = entities.Where(x => !x.Resources.NullOrEmpty()).SelectMany(x => x.RequestStatuses);
            var aalarms = entities.Where(x => !x.AudioAlarms.NullOrEmpty()).SelectMany(x => x.AudioAlarms);
            var dalarms = entities.Where(x => !x.DisplayAlarms.NullOrEmpty()).SelectMany(x => x.DisplayAlarms);
            var ealarms = entities.Where(x => !x.EmailAlarms.NullOrEmpty()) .SelectMany(x => x.EmailAlarms);

            #endregion
                   
            #region 2. save attributes of entities

            try
            {
                var keys = entities.Select(x => x.Id).ToArray();
                db.SaveAll(entities);

                if (!orgs.NullOrEmpty())
                {
                    db.SaveAll(orgs.Distinct());
                    var rorgs = entities.Where(x => x.Organizer != null)
                        .Select(e => new REL_EVENTS_ORGANIZERS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = e.Id,
                            OrganizerId = e.Organizer.Id
                        }).ToArray();
                    var ororgs = db.Select<REL_EVENTS_ORGANIZERS>(q => Sql.In(q.EventId, keys));
                    db.SynchronizeAll(rorgs, ororgs);
                }

                if (!rids.NullOrEmpty())
                {
                    db.SaveAll(rids.Distinct());
                    var rrids = entities.Where(x => x.RecurrenceId != null)
                        .Select(e => new REL_EVENTS_RECURRENCE_IDS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = e.Id,
                            RecurrenceId_Id = e.RecurrenceId.Id
                        });
                    var orrids = db.Select<REL_EVENTS_RECURRENCE_IDS>(q => Sql.In(q.EventId, keys));
                    db.SynchronizeAll(rrids, orrids);
                }

                if (!rrules.NullOrEmpty())
                {
                    db.SaveAll(rrules.Distinct());
                    var rrrules = entities.Where(x => x.RecurrenceRule != null)
                        .Select(e => new REL_EVENTS_RECURS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = e.Id,
                            RecurrenceRuleId = e.RecurrenceRule.Id
                        });

                    var orrrules = db.Select<REL_EVENTS_RECURS>(q => Sql.In(q.EventId, keys));
                    db.SynchronizeAll(rrrules, orrrules);
                }

                if (!attendees.NullOrEmpty())
                {
                    db.SaveAll(attendees.Distinct());
                    var rattendees = entities.Where(x => !x.Attendees.NullOrEmpty())
                        .SelectMany(e => e.Attendees
                            .Select(x => new REL_EVENTS_ATTENDEES
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                AttendeeId = x.Id
                            }));
                    var orattendees = db.Select<REL_EVENTS_ATTENDEES>(q => Sql.In(q.EventId, keys));
                    db.SynchronizeAll(rattendees, orattendees);
                }

                if (!attachbins.NullOrEmpty())
                {
                    db.SaveAll(attachbins.Distinct());
                    var rattachbins = entities.Where(x => !x.AttachmentBinaries.OfType<ATTACH_BINARY>().NullOrEmpty())
                        .SelectMany(e => e.AttachmentBinaries.OfType<ATTACH_BINARY>()
                            .Select(x => new REL_EVENTS_ATTACHBINS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                AttachmentId = x.Id
                            }));
                    var orattachbins = db.Select<REL_EVENTS_ATTACHBINS>(q => Sql.In(q.EventId, keys));
                    db.SynchronizeAll(rattachbins, orattachbins);
                }

                if (!attachuris.NullOrEmpty())
                {
                    db.SaveAll(attachuris.Distinct());
                    var rattachuris = entities.Where(x => !x.AttachmentBinaries.OfType<ATTACH_URI>().NullOrEmpty())
                        .SelectMany(e => e.AttachmentBinaries.OfType<ATTACH_URI>()
                            .Select(x => new REL_EVENTS_ATTACHURIS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = e.Id,
                                AttachmentId = x.Id
                            }));
                    var orattachuris = db.Select<REL_EVENTS_ATTACHURIS>(q => Sql.In(q.EventId, keys));
                    db.SynchronizeAll(rattachuris, orattachuris);

                }

                if (!contacts.NullOrEmpty())
                {
                    db.SaveAll(contacts.Distinct());
                    var rcontacts = entities.Where(x => !x.Contacts.NullOrEmpty())
                        .SelectMany(e => e.Contacts.Select(x => new REL_EVENTS_CONTACTS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = e.Id,
                            ContactId = x.Id
                        }));
                    var orcontacts = db.Select<REL_EVENTS_CONTACTS>(q => Sql.In(q.EventId, keys));
                    db.SynchronizeAll(rcontacts, orcontacts);
                }

                if (!comments.NullOrEmpty())
                {
                    db.SaveAll(comments.Distinct());
                    var rcomments = entities.Where(x => !x.Comments.NullOrEmpty())
                        .SelectMany(e => e.Contacts.Select(x => new REL_EVENTS_COMMENTS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = e.Id,
                            CommentId = x.Id
                        }));
                    var orcomments = db.Select<REL_EVENTS_COMMENTS>(q => Sql.In(q.EventId, keys));
                    db.SynchronizeAll(rcomments, orcomments);
                }

                if (!rdates.NullOrEmpty())
                {
                    db.SaveAll(rdates.Distinct());
                    var rrdates = entities.Where(x => !x.RecurrenceDates.NullOrEmpty())
                        .SelectMany(e => e.RecurrenceDates.OfType<RDATE>().Select(x => new REL_EVENTS_RDATES
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = e.Id,
                            RecurrenceDateId = x.Id
                        }));
                    var orrdates = db.Select<REL_EVENTS_RDATES>(q => Sql.In(q.EventId, keys));
                    db.SynchronizeAll(rrdates, orrdates);
                }

                if (!exdates.NullOrEmpty())
                {
                    db.SaveAll(exdates.Distinct());
                    var rexdates = entities.Where(x => !x.ExceptionDates.NullOrEmpty())
                        .SelectMany(e => e.ExceptionDates.OfType<EXDATE>().Select(x => new REL_EVENTS_EXDATES
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = e.Id,
                            ExceptionDateId = x.Id
                        }));
                    var orexdates = db.Select<REL_EVENTS_EXDATES>(q => Sql.In(q.EventId, keys));
                    db.SynchronizeAll(rexdates, rexdates);
                }

                if (!relateds.NullOrEmpty())
                {
                    db.SaveAll(relateds.Distinct());
                    var rrelateds = entities.Where(x => !x.RelatedTos.NullOrEmpty())
                        .SelectMany(e => e.RelatedTos.Select(x => new REL_EVENTS_RELATEDTOS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = e.Id,
                            RelatedToId = x.Id
                        }));
                    var orrelateds = db.Select<REL_EVENTS_RELATEDTOS>(q => Sql.In(q.EventId, keys));
                    db.SynchronizeAll(rrelateds, orrelateds);

                }

                if (!resources.NullOrEmpty())
                {
                    db.SaveAll(resources.Distinct());
                    var rresources = entities.Where(x => !x.Resources.NullOrEmpty())
                        .SelectMany(e => e.Resources.Select(x => new REL_EVENTS_RESOURCES
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = e.Id,
                            ResourcesId = x.Id
                        }));
                    var orresources = db.Select<REL_EVENTS_RESOURCES>(q => Sql.In(q.EventId, keys));
                    db.SynchronizeAll(rresources, orresources);

                }

                if (!reqstats.NullOrEmpty())
                {
                    db.SaveAll(reqstats.Distinct());
                    var rreqstats = entities.Where(x => !x.RequestStatuses.NullOrEmpty())
                        .SelectMany(e => e.RequestStatuses.Select(x => new REL_EVENTS_REQSTATS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = e.Id,
                            ReqStatsId = x.Id
                        }));
                    var orreqstats = db.Select<REL_EVENTS_REQSTATS>(q => Sql.In(q.EventId, keys));
                    db.SynchronizeAll(rreqstats, orreqstats);
                }

                if (!aalarms.NullOrEmpty())
                {
                    this.AudioAlarmRepository.SaveAll(aalarms.Distinct());
                    var raalarms = entities.Where(x => !x.AudioAlarms.NullOrEmpty())
                        .SelectMany(e => e.AudioAlarms.Select(x => new REL_EVENTS_AUDIO_ALARMS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = e.Id,
                            AlarmId = x.Id
                        }));
                    var oraalarms = db.Select<REL_EVENTS_AUDIO_ALARMS>(q => Sql.In(q.EventId, entities.Select(x => x.Id)));
                    db.SynchronizeAll(raalarms, oraalarms);
                }

                if (!dalarms.NullOrEmpty())
                {
                    this.DisplayAlarmRepository.SaveAll(dalarms.Distinct());
                    var rdalarms = entities.Where(x => !x.AudioAlarms.NullOrEmpty()).SelectMany(e => e.DisplayAlarms.Select(x => new REL_EVENTS_DISPLAY_ALARMS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = e.Id,
                        AlarmId = x.Id
                    }));
                    var ordalarms = db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => Sql.In(q.EventId, entities.Select(x => x.Id)) && Sql.In(q.AlarmId, dalarms.Select(x => x.Id).ToArray()));
                    db.SynchronizeAll(rdalarms, ordalarms);

                }

                if (!ealarms.NullOrEmpty())
                {
                    this.EmailAlarmRepository.SaveAll(ealarms.Distinct());
                    var realarms = entities.Where(x => !x.EmailAlarms.NullOrEmpty())
                        .SelectMany(e => e.EmailAlarms.Select(x => new REL_EVENTS_EMAIL_ALARMS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            EventId = e.Id,
                            AlarmId = x.Id
                        }));

                    var orealarms = db.Select<REL_EVENTS_EMAIL_ALARMS>(q => Sql.In(q.EventId, entities.Select(x => x.Id)));
                    db.SynchronizeAll(realarms, orealarms);

                }
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }

            #endregion
        }

        public void EraseAll(IEnumerable<string> keys = null)
        {
            try
            {
                if(!keys.NullOrEmpty()) db.Delete<VEVENT>(q => Sql.In(q.Id, keys.ToArray()));
                else db.DeleteAll<VEVENT>();
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        public VEVENT Hydrate(VEVENT dry)
        {
            var full = dry;
            try
            {
                var orgs = db.Select<ORGANIZER, VEVENT, REL_EVENTS_ORGANIZERS>(
                    r => r.OrganizerId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!orgs.NullOrEmpty()) full.Organizer = orgs.FirstOrDefault();

                var rids = db.Select<RECURRENCE_ID, VEVENT, REL_EVENTS_RECURRENCE_IDS>(
                    r => r.RecurrenceId_Id,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!rids.NullOrEmpty()) full.RecurrenceId = rids.FirstOrDefault();

                var rrules = db.Select<RECUR, VEVENT, REL_EVENTS_RECURS>(
                    r => r.RecurrenceRuleId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!rrules.NullOrEmpty()) full.RecurrenceRule = rrules.FirstOrDefault();

                var attachbins = db.Select<ATTACH_BINARY, VEVENT, REL_EVENTS_ATTACHBINS>(
                   r => r.AttachmentId,
                   r => r.EventId,
                   e => e.Id == dry.Id);
                if (!attachbins.NullOrEmpty()) full.AttachmentBinaries.AddRangeComplement(attachbins);

                var attachuris = db.Select<ATTACH_URI, VEVENT, REL_EVENTS_ATTACHURIS>(
                   r => r.AttachmentId,
                   r => r.EventId,
                   e => e.Id == dry.Id);
                if (!attachuris.NullOrEmpty()) full.AttachmentUris.AddRangeComplement(attachuris);

                var attendees = db.Select<ATTENDEE, VEVENT, REL_EVENTS_ATTENDEES>(
                     r => r.AttendeeId,
                     r => r.EventId,
                     e => e.Id == dry.Id);
                if (!attendees.NullOrEmpty()) full.Attendees.AddRangeComplement(attendees);

                var comments = db.Select<COMMENT, VEVENT, REL_EVENTS_COMMENTS>(
                    r => r.CommentId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!comments.NullOrEmpty()) full.Comments.AddRangeComplement(comments);

                var contacts = db.Select<CONTACT, VEVENT, REL_EVENTS_CONTACTS>(
                    r => r.ContactId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!contacts.NullOrEmpty()) full.Contacts.AddRangeComplement(contacts);

                var rdates = db.Select<RDATE, VEVENT, REL_EVENTS_RDATES>(
                    r => r.RecurrenceDateId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!rdates.NullOrEmpty()) full.RecurrenceDates.AddRangeComplement(rdates);

                var exdates = db.Select<EXDATE, VEVENT, REL_EVENTS_EXDATES>(
                    r => r.ExceptionDateId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!exdates.NullOrEmpty()) full.ExceptionDates.AddRangeComplement(exdates);

                var relatedtos = db.Select<RELATEDTO, VEVENT, REL_EVENTS_RELATEDTOS>(
                     r => r.RelatedToId,
                     r => r.EventId,
                     e => e.Id == dry.Id);
                full.RelatedTos.AddRangeComplement(relatedtos);

                var reqstats = db.Select<REQUEST_STATUS, VEVENT, REL_EVENTS_REQSTATS>(
                    r => r.ReqStatsId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                full.RequestStatuses.AddRangeComplement(reqstats);

                var resources = db.Select<RESOURCES, VEVENT, REL_EVENTS_RESOURCES>(
                    r => r.ResourcesId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!resources.NullOrEmpty()) full.Resources.AddRangeComplement(resources);

                var raalarms = this.db.Select<REL_EVENTS_AUDIO_ALARMS>(q => q.EventId == dry.Id);
                if (!raalarms.NullOrEmpty())
                {
                    full.AudioAlarms.AddRangeComplement(this.AudioAlarmRepository.FindAll(raalarms.Select(x => x.AlarmId).ToList()));
                }

                var rdalarms = this.db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => q.EventId == dry.Id);
                if (!rdalarms.NullOrEmpty())
                {
                    full.DisplayAlarms.AddRangeComplement(this.DisplayAlarmRepository.FindAll(rdalarms.Select(x => x.AlarmId).ToList()));
                }

                var realarms = this.db.Select<REL_EVENTS_EMAIL_ALARMS>(q => q.EventId == dry.Id);
                if (!realarms.NullOrEmpty())
                {
                    full.EmailAlarms.AddRangeComplement(this.EmailAlarmRepository.FindAll(realarms.Select(x => x.AlarmId).ToList()));
                }

            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }

            return full?? dry;

        }

        public IEnumerable<VEVENT> HydrateAll(IEnumerable<VEVENT> dry)
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

                    #region 2. retrieve secondary entitiesB

                    var orgs = (!rorgs.SafeEmpty()) ? db.Select<ORGANIZER>(q => Sql.In(q.Id, rorgs.Select(r => r.OrganizerId).ToList())) : null;
                    var rids = (!rrids.SafeEmpty()) ? db.Select<RECURRENCE_ID>(q => Sql.In(q.Id, rrids.Select(r => r.RecurrenceId_Id).ToList())) : null;
                    var rrules = (!rrrules.SafeEmpty()) ? db.Select<RECUR>(q => Sql.In(q.Id, rrrules.Select(r => r.RecurrenceRuleId).ToList())) : null;
                    var attendees = (!rattendees.SafeEmpty()) ? db.Select<ATTENDEE>(q => Sql.In(q.Id, rattendees.Select(r => r.AttendeeId).ToList())) : null;
                    var comments = (!rcomments.SafeEmpty()) ? db.Select<COMMENT>(q => Sql.In(q.Id, rcomments.Select(r => r.CommentId).ToList())) : null;
                    var attachbins = (!rattachbins.SafeEmpty()) ? db.Select<ATTACH_BINARY>(q => Sql.In(q.Id, rattachbins.Select(r => r.AttachmentId).ToList())) : null;
                    var attachuris = (!rattachuris.SafeEmpty()) ? db.Select<ATTACH_URI>(q => Sql.In(q.Id, rattachuris.Select(r => r.AttachmentId).ToList())) : null;
                    var contacts = (!rcontacts.SafeEmpty()) ? db.Select<CONTACT>(q => Sql.In(q.Id, rcontacts.Select(r => r.ContactId).ToList())) : null;
                    var exdates = (!rexdates.SafeEmpty()) ? db.Select<EXDATE>(q => Sql.In(q.Id, rexdates.Select(r => r.ExceptionDateId).ToList())) : null;
                    var rdates = (!rrdates.SafeEmpty()) ? db.Select<RDATE>(q => Sql.In(q.Id, rrdates.Select(r => r.RecurrenceDateId).ToList())) : null;
                    var relatedtos = (!rrelatedtos.SafeEmpty()) ? db.Select<RELATEDTO>(q => Sql.In(q.Id, rrelatedtos.Select(r => r.RelatedToId).ToList())) : null;
                    var reqstats = (!rreqstats.SafeEmpty()) ? db.Select<REQUEST_STATUS>(q => Sql.In(q.Id, rreqstats.Select(r => r.ReqStatsId).ToList())) : null;
                    var resources = (!rresources.SafeEmpty()) ? db.Select<RESOURCES>(q => Sql.In(q.Id, rresources.Select(r => r.ResourcesId).ToList())) : null;
                    var aalarms = (!raalarms.SafeEmpty()) ? this.AudioAlarmRepository.FindAll(raalarms.Select(x => x.AlarmId).ToList()) : null;
                    var dalarms = (!rdalarms.SafeEmpty()) ? this.DisplayAlarmRepository.FindAll(rdalarms.Select(x => x.AlarmId).ToList()) : null;
                    var ealarms = (!realarms.SafeEmpty()) ? this.EmailAlarmRepository.FindAll(realarms.Select(x => x.AlarmId).ToList()) : null;

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
                            if (!xattachbins.NullOrEmpty()) x.AttachmentBinaries.AddRangeComplement(xattachbins);

                        }

                        if (!attachuris.NullOrEmpty())
                        {
                            var xattachuris = from y in attachuris
                                              join r in rattachuris on y.Id equals r.AttachmentId
                                              join e in full on r.EventId equals e.Id
                                              where e.Id == x.Id
                                              select y;
                            if (!xattachuris.NullOrEmpty()) x.AttachmentUris.AddRangeComplement(xattachuris);
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
                            if (!xraalarms.NullOrEmpty()) x.AudioAlarms.AddRangeComplement(xraalarms);
                        }

                        if (!dalarms.NullOrEmpty())
                        {
                            var xrdalarms = from y in dalarms
                                            join r in rdalarms on y.Id equals r.AlarmId
                                            join e in full on r.EventId equals e.Id
                                            where e.Id == x.Id
                                            select y;
                            if (!xrdalarms.NullOrEmpty()) x.DisplayAlarms.AddRangeComplement(xrdalarms);

                        }

                        if (!ealarms.NullOrEmpty())
                        {
                            var xrealarms = from y in ealarms
                                            join r in realarms on y.Id equals r.AlarmId
                                            join e in full on r.EventId equals e.Id
                                            where e.Id == x.Id
                                            select y;
                            if (!xrealarms.NullOrEmpty()) x.EmailAlarms.AddRangeComplement(xrealarms);
                        }
                    });

                    #endregion
                }

            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }

            return full ?? dry;
        }

        public bool ContainsKey(string key)
        {
            try
            {
                return db.Count<VEVENT>(q => q.Id == key) != 0;
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        public bool ContainsKeys(IEnumerable<string> keys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            try
            {
                var dkeys = keys.Distinct();
                if (mode == ExpectationMode.pessimistic || mode == ExpectationMode.unknown)
                    return db.Count<VEVENT>(q => Sql.In(q.Id, dkeys)) == dkeys.Count();
                else return db.Count<VEVENT>(q => Sql.In(q.Id, dkeys)) != 0;
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        public IEnumerable<string> GetKeys(int? skip = null, int? take = null)
        {
            try
            {
               return db.SelectParam<VEVENT>(q => q.Id, skip, take);
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
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
            try
            {
                var dry = full;
                dry.Organizer = null;
                dry.RecurrenceId = null;
                dry.RecurrenceRule = null;
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
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }
    }
}
