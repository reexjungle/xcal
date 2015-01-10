using reexjungle.crosscut.operations.concretes;
using reexjungle.foundation.essentials.concretes;
using reexjungle.foundation.essentials.contracts;
using reexjungle.infrastructure.io.concretes;
using reexjungle.infrastructure.operations.concretes;
using reexjungle.infrastructure.operations.contracts;
using reexjungle.technical.data.concretes.extensions.ormlite;
using reexjungle.technical.data.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.repositories.concretes.relations;
using reexjungle.xcal.service.repositories.contracts;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Security;

namespace reexjungle.xcal.service.repositories.concretes.ormlite
{
    /// <summary>
    /// Reüpresents a a repository of events connected to an ORMlite source
    /// </summary>
    public class EventOrmLiteRepository : IEventOrmLiteRepository, IDisposable
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

        public EventOrmLiteRepository()
        {
        }

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

        public IEnumerable<VEVENT> Get(int? skip = null, int? take = null)
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

            var organizer = entity.Organizer;
            var recur = entity.RecurrenceRule;
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

            #endregion retrieve attributes of entity

            #region save event and its attributes

            try
            {
                db.Save(entity);

                var ororgs = db.Select<REL_EVENTS_ORGANIZERS>(q => q.EventId == entity.Id);
                var orrecurs = db.Select<REL_EVENTS_RECURS>(q => q.EventId == entity.Id);
                var orattendees = db.Select<REL_EVENTS_ATTENDEES>(q => q.EventId == entity.Id);
                var orattachbins = db.Select<REL_EVENTS_ATTACHBINS>(q => q.EventId == entity.Id);
                var orattachuris = db.Select<REL_EVENTS_ATTACHURIS>(q => q.EventId == entity.Id);
                var orcontacts = db.Select<REL_EVENTS_CONTACTS>(q => q.EventId == entity.Id);
                var orcomments = db.Select<REL_EVENTS_COMMENTS>(q => q.EventId == entity.Id);
                var orrdates = db.Select<REL_EVENTS_RDATES>(q => q.EventId == entity.Id);
                var orexdates = db.Select<REL_EVENTS_EXDATES>(q => q.EventId == entity.Id);
                var orrelateds = db.Select<REL_EVENTS_RELATEDTOS>(q => q.EventId == entity.Id);
                var orresources = db.Select<REL_EVENTS_RESOURCES>(q => q.EventId == entity.Id);
                var orreqstats = db.Select<REL_EVENTS_REQSTATS>(q => q.EventId == entity.Id);
                var oraalarms = db.Select<REL_EVENTS_AUDIO_ALARMS>(q => q.EventId == entity.Id);
                var ordalarms = db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => q.EventId == entity.Id);
                var orealarms = db.Select<REL_EVENTS_EMAIL_ALARMS>(q => q.EventId == entity.Id);

                if (organizer != null)
                {
                    db.Save(organizer);
                    var rorg = new REL_EVENTS_ORGANIZERS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        OrganizerId = organizer.Id
                    };

                    db.MergeAll(rorg.ToSingleton(), ororgs);
                }
                else db.RemoveAll(ororgs);

                if (recur != null)
                {
                    db.Save(recur);
                    var rrecur = new REL_EVENTS_RECURS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        RecurId = recur.Id
                    };

                    db.MergeAll(rrecur.ToSingleton(), orrecurs);
                }
                else db.RemoveAll(ororgs);

                if (!attendees.NullOrEmpty())
                {
                    db.SaveAll(attendees.Distinct());
                    var rattendees = attendees.Select(x => new REL_EVENTS_ATTENDEES
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        AttendeeId = x.Id
                    });

                    db.MergeAll(rattendees, orattendees);
                }
                else db.RemoveAll(orattendees);

                if (!attachbins.NullOrEmpty())
                {
                    db.SaveAll(attachbins.Distinct());
                    var rattachbins = attachbins.Select(x => new REL_EVENTS_ATTACHBINS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        AttachmentId = x.Id
                    });
                    db.MergeAll(rattachbins, orattachbins);
                }
                else db.RemoveAll(orattachbins);

                if (!attachuris.NullOrEmpty())
                {
                    db.SaveAll(attachuris.Distinct());
                    var rattachuris = attachuris.Select(x => new REL_EVENTS_ATTACHURIS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        AttachmentId = x.Id
                    });
                    db.MergeAll(rattachuris, orattachuris);
                }
                else db.RemoveAll(orattachuris);

                if (!contacts.NullOrEmpty())
                {
                    db.SaveAll(contacts.Distinct());
                    var rcontacts = contacts.Select(x => new REL_EVENTS_CONTACTS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        ContactId = x.Id
                    });
                    db.MergeAll(rcontacts, orcontacts);
                }
                else db.RemoveAll(orcontacts);

                if (!comments.NullOrEmpty())
                {
                    db.SaveAll(comments.Distinct());
                    var rcomments = comments.Select(x => new REL_EVENTS_COMMENTS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        CommentId = x.Id
                    });
                    db.MergeAll(rcomments, orcomments);
                }
                else db.RemoveAll(orcomments);

                if (!rdates.NullOrEmpty())
                {
                    db.SaveAll(rdates.Distinct());
                    var rrdates = rdates.Select(x => new REL_EVENTS_RDATES
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        RecurrenceDateId = x.Id
                    });
                    db.MergeAll(rrdates, orrdates);
                }
                else db.RemoveAll(orrdates);

                if (!exdates.NullOrEmpty())
                {
                    db.SaveAll(exdates.Distinct());
                    var rexdates = exdates.Select(x => new REL_EVENTS_EXDATES
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        ExceptionDateId = x.Id
                    });
                    db.MergeAll(rexdates, orexdates);
                }
                else db.RemoveAll(orexdates);

                if (!relateds.NullOrEmpty())
                {
                    db.SaveAll(relateds.Distinct());
                    var rrelateds = relateds.Select(x => new REL_EVENTS_RELATEDTOS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        RelatedToId = x.Id
                    });
                    db.MergeAll(rrelateds, orrelateds);
                }
                else db.RemoveAll(orrelateds);

                if (!resources.NullOrEmpty())
                {
                    db.SaveAll(resources.Distinct());
                    var rresources = resources.Select(x => new REL_EVENTS_RESOURCES
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        ResourcesId = x.Id
                    });
                    db.MergeAll(rresources, orresources);
                }
                else db.RemoveAll(orresources);

                if (!reqstats.NullOrEmpty())
                {
                    db.SaveAll(reqstats.Distinct());
                    var rreqstats = reqstats.Select(x => new REL_EVENTS_REQSTATS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        ReqStatsId = x.Id
                    });
                    db.MergeAll(rreqstats, orreqstats);
                }
                else db.RemoveAll(orreqstats);

                if (!aalarms.NullOrEmpty())
                {
                    this.AudioAlarmRepository.SaveAll(aalarms.Distinct());
                    var raalarms = aalarms.Select(x => new REL_EVENTS_AUDIO_ALARMS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        AlarmId = x.Id
                    });
                    db.MergeAll(raalarms, oraalarms);
                }
                else db.RemoveAll(orealarms);

                if (!dalarms.NullOrEmpty())
                {
                    this.DisplayAlarmRepository.SaveAll(dalarms.Distinct());
                    var rdalarms = dalarms.Select(x => new REL_EVENTS_DISPLAY_ALARMS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        AlarmId = x.Id
                    });
                    db.MergeAll(rdalarms, ordalarms);
                }
                else db.RemoveAll(ordalarms);

                if (!ealarms.NullOrEmpty())
                {
                    this.EmailAlarmRepository.SaveAll(ealarms.Distinct());
                    var realarms = ealarms.Select(x => new REL_EVENTS_EMAIL_ALARMS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = entity.Id,
                        AlarmId = x.Id
                    });
                    db.MergeAll(realarms, orealarms);
                }
                else db.RemoveAll(orealarms);
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }

            #endregion save event and its attributes
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
                x.Categories,
                x.Duration,
                x.RecurrenceRule
            };

            Expression<Func<VEVENT, object>> relations = x => new
            {
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
                x.Organizer,
                x.RecurrenceId,

            };

            //4. Get list of selected relationals
            var srelation = relations.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);

            //5. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);

            #endregion construct anonymous fields using expression lambdas

            try
            {
                var okeys = (keys != null)
                    ? db.SelectParam<VEVENT, string>(q => q.Id, p => Sql.In(p.Id, keys.ToArray())).ToArray()
                    : db.SelectParam<VEVENT>(q => q.Id).ToArray();

                if (!srelation.NullOrEmpty())
                {
                    Expression<Func<VEVENT, object>> orgexpr = y => y.Organizer;
                    Expression<Func<VEVENT, object>> recurexpr = y => y.RecurrenceRule;
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
                        var ororgs = db.Select<REL_EVENTS_ORGANIZERS>(q => Sql.In(q.EventId, okeys));
                        if (source.Organizer != null)
                        {
                            db.Save(source.Organizer);
                            var rorgs = okeys.Select(x => new REL_EVENTS_ORGANIZERS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                OrganizerId = source.Organizer.Id
                            });
                            db.MergeAll(rorgs, ororgs);
                        }
                        else db.RemoveAll(ororgs);
                    }

                    if (selection.Contains(recurexpr.GetMemberName()))
                    {
                        var orrecurs = db.Select<REL_EVENTS_RECURS>(q => Sql.In(q.EventId, okeys));
                        if (source.RecurrenceRule != null)
                        {
                            db.Save(source.RecurrenceRule);
                            var rrecurs = okeys.Select(x => new REL_EVENTS_RECURS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                RecurId = source.RecurrenceRule.Id
                            });
                            db.MergeAll(rrecurs, orrecurs);
                        }
                        else db.RemoveAll(orrecurs);
                    }

                    if (selection.Contains(attendsexpr.GetMemberName()))
                    {
                        var orattendees = db.Select<REL_EVENTS_ATTENDEES>(q => Sql.In(q.EventId, okeys));
                        if (!source.Attendees.NullOrEmpty())
                        {
                            db.SaveAll(source.Attendees.Distinct());
                            var rattendees = okeys.SelectMany(x => source.Attendees.Select(y => new REL_EVENTS_ATTENDEES
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                AttendeeId = y.Id
                            }));
                            db.MergeAll(rattendees, orattendees);
                        }
                        else db.RemoveAll(orattendees);
                    }

                    if (selection.Contains(attachbinsexpr.GetMemberName()))
                    {
                        var orattachbins = db.Select<REL_EVENTS_ATTACHBINS>(q => Sql.In(q.EventId, okeys));
                        if (!source.AttachmentUris.NullOrEmpty())
                        {
                            db.SaveAll(source.AttachmentUris.Distinct());
                            var rattachbins = okeys.SelectMany(x => source.AttachmentUris.Select(y => new REL_EVENTS_ATTACHBINS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                AttachmentId = y.Id
                            }));

                            db.MergeAll(rattachbins, orattachbins);
                        }
                        else db.RemoveAll(orattachbins);
                    }
                    if (selection.Contains(attachurisexpr.GetMemberName()))
                    {
                        var orattachuris = db.Select<REL_EVENTS_ATTACHURIS>(q => Sql.In(q.EventId, okeys));
                        if (!source.AttachmentUris.NullOrEmpty())
                        {
                            db.SaveAll(source.AttachmentUris.Distinct());
                            var rattachuris = okeys.SelectMany(x => source.AttachmentUris.Select(y => new REL_EVENTS_ATTACHURIS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                AttachmentId = y.Id
                            }));
                            db.MergeAll(rattachuris, orattachuris);
                        }
                        else db.RemoveAll(orattachuris);
                    }

                    if (selection.Contains(contactsexpr.GetMemberName()))
                    {
                        var orcontacts = db.Select<REL_EVENTS_CONTACTS>(q => Sql.In(q.EventId, okeys));
                        if (!source.Contacts.NullOrEmpty())
                        {
                            db.SaveAll(source.Contacts.Distinct());
                            var rcontacts = okeys.SelectMany(x => source.Contacts.Select(y => new REL_EVENTS_CONTACTS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                ContactId = y.Id
                            }));
                            db.MergeAll(rcontacts, orcontacts);
                        }
                        else db.RemoveAll(orcontacts);
                    }

                    if (selection.Contains(commentsexpr.GetMemberName()))
                    {
                        var orcomments = db.Select<REL_EVENTS_COMMENTS>(q => Sql.In(q.EventId, okeys));
                        if (!source.Comments.NullOrEmpty())
                        {
                            db.SaveAll(source.Comments.Distinct());
                            var rcomments = okeys.SelectMany(x => source.Comments.Select(y => new REL_EVENTS_COMMENTS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                CommentId = y.Id
                            }));
                            db.MergeAll(rcomments, orcomments);
                        }
                        else db.RemoveAll(orcomments);
                    }

                    if (selection.Contains(rdatesexpr.GetMemberName()))
                    {
                        var orrdates = db.Select<REL_EVENTS_RDATES>(q => Sql.In(q.EventId, okeys));
                        if (!source.RecurrenceDates.NullOrEmpty())
                        {
                            db.SaveAll(source.RecurrenceDates.Distinct());
                            var rrdates = okeys.SelectMany(x => source.RecurrenceDates.Select(y => new REL_EVENTS_RDATES
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                RecurrenceDateId = y.Id
                            }));
                            db.MergeAll(rrdates, orrdates);
                        }
                        else db.RemoveAll(orrdates);
                    }

                    if (selection.Contains(exdatesexpr.GetMemberName()))
                    {
                        var orexdates = db.Select<REL_EVENTS_EXDATES>(q => Sql.In(q.EventId, okeys));
                        if (!source.ExceptionDates.NullOrEmpty())
                        {
                            db.SaveAll(source.ExceptionDates.Distinct());
                            var rexdates = okeys.SelectMany(x => source.ExceptionDates.Select(y => new REL_EVENTS_EXDATES
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                ExceptionDateId = y.Id
                            }));
                            db.MergeAll(rexdates, orexdates);
                        }
                        else db.RemoveAll(orexdates);
                    }

                    if (selection.Contains(relatedtosexpr.GetMemberName()))
                    {
                        var orrelatedtos = db.Select<REL_EVENTS_RELATEDTOS>(q => Sql.In(q.EventId, okeys));
                        if (!source.RelatedTos.NullOrEmpty())
                        {
                            db.SaveAll(source.RelatedTos.Distinct());
                            var rrelatedtos = okeys.SelectMany(x => source.RelatedTos.Select(y => new REL_EVENTS_RELATEDTOS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                RelatedToId = y.Id
                            }));

                            db.MergeAll(rrelatedtos, orrelatedtos);
                        }
                        else db.RemoveAll(orrelatedtos);
                    }

                    if (selection.Contains(resourcesexpr.GetMemberName()))
                    {
                        var orresources = db.Select<REL_EVENTS_RESOURCES>(q => Sql.In(q.EventId, okeys));
                        if (!source.Resources.NullOrEmpty())
                        {
                            db.SaveAll(source.Resources.Distinct());
                            var rresources = okeys.SelectMany(x => source.Resources.Select(y => new REL_EVENTS_RESOURCES
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                ResourcesId = y.Id
                            }));
                            db.MergeAll(rresources, orresources);
                        }
                        else db.RemoveAll(orresources);
                    }

                    if (selection.Contains(reqstatsexpr.GetMemberName()))
                    {
                        var orreqstats = db.Select<REL_EVENTS_REQSTATS>(q => Sql.In(q.EventId, okeys));
                        if (!source.RequestStatuses.NullOrEmpty())
                        {
                            db.SaveAll(source.RequestStatuses.Distinct());
                            var rreqstats = okeys.SelectMany(x => source.RequestStatuses.Select(y => new REL_EVENTS_REQSTATS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                ReqStatsId = y.Id
                            }));
                            db.MergeAll(rreqstats, orreqstats);
                        }
                        else db.RemoveAll(orreqstats);
                    }

                    if (selection.Contains(aalarmexpr.GetMemberName()))
                    {
                        var oraalarms = db.Select<REL_EVENTS_AUDIO_ALARMS>(q => Sql.In(q.EventId, okeys));
                        if (!source.AudioAlarms.NullOrEmpty())
                        {
                            this.AudioAlarmRepository.SaveAll(source.AudioAlarms.Distinct());
                            var raalarms = okeys.SelectMany(x => source.AudioAlarms.Select(y => new REL_EVENTS_AUDIO_ALARMS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                AlarmId = y.Id
                            }));
                            db.MergeAll(raalarms, oraalarms);
                        }
                        else db.RemoveAll(oraalarms);
                    }

                    if (selection.Contains(dalarmexpr.GetMemberName()))
                    {
                        var ordalarms = db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => Sql.In(q.EventId, okeys));
                        if (!source.DisplayAlarms.NullOrEmpty())
                        {
                            this.DisplayAlarmRepository.SaveAll(source.DisplayAlarms.Distinct());
                            var rdalarms = okeys.SelectMany(x => source.DisplayAlarms.Select(y => new REL_EVENTS_DISPLAY_ALARMS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                AlarmId = y.Id
                            }));
                            db.MergeAll(rdalarms, ordalarms);
                        }
                        else db.RemoveAll(ordalarms);
                    }

                    if (selection.Contains(ealarmexpr.GetMemberName()))
                    {
                        var orealarms = db.Select<REL_EVENTS_EMAIL_ALARMS>(q => Sql.In(q.EventId, okeys));
                        if (!source.EmailAlarms.NullOrEmpty())
                        {
                            this.EmailAlarmRepository.SaveAll(source.EmailAlarms.Distinct());
                            var realarms = okeys.SelectMany(x => source.EmailAlarms.Select(y => new REL_EVENTS_EMAIL_ALARMS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x,
                                AlarmId = y.Id
                            }));

                            db.MergeAll(realarms, orealarms);
                        }
                        else db.RemoveAll(orealarms);
                    }

                    #endregion save relational attributes of entities
                }

                if (!sprimitives.NullOrEmpty())
                {
                    var patchstr = string.Format("f => new {{ {0} }}", string.Join(", ", sprimitives.Select(x => string.Format("f.{0}", x))));

                    //throw new FormatException(patchstr);

                    var patchexpr = patchstr.CompileToExpressionFunc<VEVENT, object>(CodeDomLanguage.csharp, new string[] { "System.dll", "System.Core.dll", typeof(VEVENT).Assembly.Location, typeof(IContainsKey<string>).Assembly.Location });

                    if (!okeys.NullOrEmpty()) db.UpdateOnly<VEVENT, object>(source, patchexpr, q => Sql.In(q.Id, okeys.ToArray()));
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
            var recurs = entities.Where(x => x.RecurrenceRule != null).Select(x => x.RecurrenceRule);
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
            var ealarms = entities.Where(x => !x.EmailAlarms.NullOrEmpty()).SelectMany(x => x.EmailAlarms);

            #endregion 1. retrieve attributes of entities

            #region 2. save attributes of entities

            try
            {
                var keys = entities.Select(x => x.Id).ToArray();
                db.SaveAll(entities);

                var ororgs = db.Select<REL_EVENTS_ORGANIZERS>(q => Sql.In(q.EventId, keys));
                var orrecurs = db.Select<REL_EVENTS_RECURS>(q => Sql.In(q.EventId, keys));
                var orattendees = db.Select<REL_EVENTS_ATTENDEES>(q => Sql.In(q.EventId, keys));
                var orattachbins = db.Select<REL_EVENTS_ATTACHBINS>(q => Sql.In(q.EventId, keys));
                var orattachuris = db.Select<REL_EVENTS_ATTACHURIS>(q => Sql.In(q.EventId, keys));
                var orcontacts = db.Select<REL_EVENTS_CONTACTS>(q => Sql.In(q.EventId, keys));
                var orcomments = db.Select<REL_EVENTS_COMMENTS>(q => Sql.In(q.EventId, keys));
                var orrdates = db.Select<REL_EVENTS_RDATES>(q => Sql.In(q.EventId, keys));
                var orexdates = db.Select<REL_EVENTS_EXDATES>(q => Sql.In(q.EventId, keys));
                var orrelateds = db.Select<REL_EVENTS_RELATEDTOS>(q => Sql.In(q.EventId, keys));
                var orresources = db.Select<REL_EVENTS_RESOURCES>(q => Sql.In(q.EventId, keys));
                var orreqstats = db.Select<REL_EVENTS_REQSTATS>(q => Sql.In(q.EventId, keys));
                var oraalarms = db.Select<REL_EVENTS_AUDIO_ALARMS>(q => Sql.In(q.EventId, entities.Select(x => x.Id)));
                var ordalarms = db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => Sql.In(q.EventId, entities.Select(x => x.Id)));
                var orealarms = db.Select<REL_EVENTS_EMAIL_ALARMS>(q => Sql.In(q.EventId, entities.Select(x => x.Id)));

                if (!orgs.NullOrEmpty())
                {
                    db.SaveAll(orgs.Distinct());
                    var rorgs = entities.Where(x => x.Organizer != null)
                            .Select(x => new REL_EVENTS_ORGANIZERS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x.Id,
                                OrganizerId = x.Organizer.Id
                            });

                    db.MergeAll(rorgs, ororgs);
                }
                else db.RemoveAll(ororgs);

                if (!recurs.NullOrEmpty())
                {
                    db.SaveAll(recurs.Distinct());
                    var rrecurs = entities.Where(x => x.RecurrenceRule != null)
                            .Select(x => new REL_EVENTS_RECURS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                EventId = x.Id,
                                RecurId = x.RecurrenceRule.Id
                            });

                    db.MergeAll(rrecurs, orrecurs);
                }
                else db.RemoveAll(orrecurs);

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

                    db.MergeAll(rattendees, orattendees);
                }
                else db.RemoveAll(orattendees);

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

                    db.MergeAll(rattachbins, orattachbins);
                }
                else db.RemoveAll(orattachbins);

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
                    db.MergeAll(rattachuris, orattachuris);
                }
                else db.RemoveAll(orattachuris);

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
                    db.MergeAll(rcontacts, orcontacts);
                }
                else db.RemoveAll(orcontacts);

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
                    db.MergeAll(rcomments, orcomments);
                }
                else db.RemoveAll(orcomments);

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
                    db.MergeAll(rrdates, orrdates);
                }
                else db.RemoveAll(orrdates);

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
                    db.MergeAll(rexdates, rexdates);
                }
                else db.RemoveAll(orexdates);

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
                    db.MergeAll(rrelateds, orrelateds);
                }
                else db.RemoveAll(orrelateds);

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
                    db.MergeAll(rresources, orresources);
                }
                else db.RemoveAll(orresources);

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
                    db.MergeAll(rreqstats, orreqstats);
                }
                else db.RemoveAll(orreqstats);

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
                    db.MergeAll(raalarms, oraalarms);
                }
                else db.RemoveAll(oraalarms);

                if (!dalarms.NullOrEmpty())
                {
                    this.DisplayAlarmRepository.SaveAll(dalarms.Distinct());
                    var rdalarms = entities.Where(x => !x.AudioAlarms.NullOrEmpty()).SelectMany(e => e.DisplayAlarms.Select(x => new REL_EVENTS_DISPLAY_ALARMS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        EventId = e.Id,
                        AlarmId = x.Id
                    }));

                    db.MergeAll(rdalarms, ordalarms);
                }
                else db.RemoveAll(ordalarms);

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

                    db.MergeAll(realarms, orealarms);
                }
                else db.RemoveAll(orealarms);
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }

            #endregion 2. save attributes of entities
        }

        public void EraseAll(IEnumerable<string> keys = null)
        {
            try
            {
                if (!keys.NullOrEmpty()) db.Delete<VEVENT>(q => Sql.In(q.Id, keys.ToArray()));
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
                var organizers = db.Select<ORGANIZER, VEVENT, REL_EVENTS_ORGANIZERS>(
                    r => r.OrganizerId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!organizers.NullOrEmpty()) full.Organizer = organizers.First();

                var recurs = db.Select<RECUR, VEVENT, REL_EVENTS_RECURS>(
                    r => r.RecurId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!recurs.NullOrEmpty()) full.RecurrenceRule = recurs.First();

                var attachbins = db.Select<ATTACH_BINARY, VEVENT, REL_EVENTS_ATTACHBINS>(
                   r => r.AttachmentId,
                   r => r.EventId,
                   e => e.Id == dry.Id);
                if (!attachbins.NullOrEmpty()) full.AttachmentBinaries.MergeRange(attachbins);

                var attachuris = db.Select<ATTACH_URI, VEVENT, REL_EVENTS_ATTACHURIS>(
                   r => r.AttachmentId,
                   r => r.EventId,
                   e => e.Id == dry.Id);
                if (!attachuris.NullOrEmpty()) full.AttachmentUris.MergeRange(attachuris);

                var attendees = db.Select<ATTENDEE, VEVENT, REL_EVENTS_ATTENDEES>(
                     r => r.AttendeeId,
                     r => r.EventId,
                     e => e.Id == dry.Id);
                if (!attendees.NullOrEmpty()) full.Attendees.MergeRange(attendees);

                var comments = db.Select<COMMENT, VEVENT, REL_EVENTS_COMMENTS>(
                    r => r.CommentId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!comments.NullOrEmpty()) full.Comments.MergeRange(comments);

                var contacts = db.Select<CONTACT, VEVENT, REL_EVENTS_CONTACTS>(
                    r => r.ContactId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!contacts.NullOrEmpty()) full.Contacts.MergeRange(contacts);

                var rdates = db.Select<RDATE, VEVENT, REL_EVENTS_RDATES>(
                    r => r.RecurrenceDateId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!rdates.NullOrEmpty()) full.RecurrenceDates.MergeRange(rdates);

                var exdates = db.Select<EXDATE, VEVENT, REL_EVENTS_EXDATES>(
                    r => r.ExceptionDateId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!exdates.NullOrEmpty()) full.ExceptionDates.MergeRange(exdates);

                var relatedtos = db.Select<RELATEDTO, VEVENT, REL_EVENTS_RELATEDTOS>(
                     r => r.RelatedToId,
                     r => r.EventId,
                     e => e.Id == dry.Id);
                full.RelatedTos.MergeRange(relatedtos);

                var reqstats = db.Select<REQUEST_STATUS, VEVENT, REL_EVENTS_REQSTATS>(
                    r => r.ReqStatsId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                full.RequestStatuses.MergeRange(reqstats);

                var resources = db.Select<RESOURCES, VEVENT, REL_EVENTS_RESOURCES>(
                    r => r.ResourcesId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!resources.NullOrEmpty()) full.Resources.MergeRange(resources);

                var raalarms = this.db.Select<REL_EVENTS_AUDIO_ALARMS>(q => q.EventId == dry.Id);
                if (!raalarms.NullOrEmpty())
                {
                    full.AudioAlarms.MergeRange(this.AudioAlarmRepository.FindAll(raalarms.Select(x => x.AlarmId).ToList()));
                }

                var rdalarms = this.db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => q.EventId == dry.Id);
                if (!rdalarms.NullOrEmpty())
                {
                    full.DisplayAlarms.MergeRange(this.DisplayAlarmRepository.FindAll(rdalarms.Select(x => x.AlarmId).ToList()));
                }

                var realarms = this.db.Select<REL_EVENTS_EMAIL_ALARMS>(q => q.EventId == dry.Id);
                if (!realarms.NullOrEmpty())
                {
                    full.EmailAlarms.MergeRange(this.EmailAlarmRepository.FindAll(realarms.Select(x => x.AlarmId).ToList()));
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }

            return full ?? dry;
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
                    var rrecurs = this.db.Select<REL_EVENTS_RECURS>(q => Sql.In(q.EventId, okeys));
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

                    #endregion 1. retrieve relationships

                    #region 2. retrieve secondary entitiesB

                    var orgs =  (!rorgs.SafeEmpty()) ? db.Select<ORGANIZER>(q => Sql.In(q.Id, rorgs.Select(r => r.OrganizerId).ToList())) : null;
                    var recurs =  (!rrecurs.SafeEmpty()) ? db.Select<RECUR>(q => Sql.In(q.Id, rrecurs.Select(r => r.RecurId).ToList())) : null;
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

                    #endregion 2. retrieve secondary entitiesB

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
                            if (!xorgs.NullOrEmpty()) x.Organizer = xorgs.First();
                        }

                        if (!recurs.NullOrEmpty())
                        {
                            var xrecurs = from y in recurs
                                        join r in rorgs on y.Id equals r.OrganizerId
                                        join e in full on r.EventId equals e.Id
                                        where e.Id == x.Id
                                        select y;
                            if (!xrecurs.NullOrEmpty()) x.RecurrenceRule = xrecurs.First();
                        }

                        if (!comments.NullOrEmpty())
                        {
                            var xcomments = from y in comments
                                            join r in rcomments on y.Id equals r.CommentId
                                            join e in full on r.EventId equals e.Id
                                            where e.Id == x.Id
                                            select y;
                            if (!xcomments.NullOrEmpty()) x.Comments.MergeRange(xcomments);
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
                            if (!xattachbins.NullOrEmpty()) x.AttachmentBinaries.MergeRange(xattachbins);
                        }

                        if (!attachuris.NullOrEmpty())
                        {
                            var xattachuris = from y in attachuris
                                              join r in rattachuris on y.Id equals r.AttachmentId
                                              join e in full on r.EventId equals e.Id
                                              where e.Id == x.Id
                                              select y;
                            if (!xattachuris.NullOrEmpty()) x.AttachmentUris.MergeRange(xattachuris);
                        }

                        if (!contacts.NullOrEmpty())
                        {
                            var xcontacts = from y in contacts
                                            join r in rcontacts on y.Id equals r.ContactId
                                            join e in full on r.EventId equals e.Id
                                            where e.Id == x.Id
                                            select y;

                            if (!xcontacts.NullOrEmpty()) x.Contacts.MergeRange(xcontacts);
                        }

                        if (!rdates.NullOrEmpty())
                        {
                            var xrdates = from y in rdates
                                          join r in rrdates on y.Id equals r.RecurrenceDateId
                                          join e in full on r.EventId equals e.Id
                                          where e.Id == x.Id
                                          select y;
                            if (!xrdates.NullOrEmpty()) x.RecurrenceDates.MergeRange(xrdates);
                        }

                        if (!exdates.NullOrEmpty())
                        {
                            var xexdates = from y in exdates
                                           join r in rexdates on y.Id equals r.ExceptionDateId
                                           join e in full on r.EventId equals e.Id
                                           where e.Id == x.Id
                                           select y;
                            if (!xexdates.NullOrEmpty()) x.ExceptionDates.MergeRange(xexdates);
                        }

                        if (!relatedtos.NullOrEmpty())
                        {
                            var xrelatedtos = from y in relatedtos
                                              join r in rrelatedtos on y.Id equals r.RelatedToId
                                              join e in full on r.EventId equals e.Id
                                              where e.Id == x.Id
                                              select y;
                            if (!xrelatedtos.NullOrEmpty()) x.RelatedTos.MergeRange(xrelatedtos);
                        }

                        if (!reqstats.NullOrEmpty())
                        {
                            var xreqstats = from y in reqstats
                                            join r in rreqstats on y.Id equals r.ReqStatsId
                                            join e in full on r.EventId equals e.Id
                                            where e.Id == x.Id
                                            select y;
                            if (!xreqstats.NullOrEmpty()) x.RequestStatuses.MergeRange(xreqstats);
                        }

                        if (!resources.NullOrEmpty())
                        {
                            var xresources = from y in resources
                                             join r in rresources on y.Id equals r.ResourcesId
                                             join e in full on r.EventId equals e.Id
                                             where e.Id == x.Id
                                             select y;
                            if (!xresources.NullOrEmpty()) x.Resources.MergeRange(xresources);
                        }

                        if (!aalarms.NullOrEmpty())
                        {
                            var xraalarms = from y in aalarms
                                            join r in raalarms on y.Id equals r.AlarmId
                                            join e in full on r.EventId equals e.Id
                                            where e.Id == x.Id
                                            select y;
                            if (!xraalarms.NullOrEmpty()) x.AudioAlarms.MergeRange(xraalarms);
                        }

                        if (!dalarms.NullOrEmpty())
                        {
                            var xrdalarms = from y in dalarms
                                            join r in rdalarms on y.Id equals r.AlarmId
                                            join e in full on r.EventId equals e.Id
                                            where e.Id == x.Id
                                            select y;
                            if (!xrdalarms.NullOrEmpty()) x.DisplayAlarms.MergeRange(xrdalarms);
                        }

                        if (!ealarms.NullOrEmpty())
                        {
                            var xrealarms = from y in ealarms
                                            join r in realarms on y.Id equals r.AlarmId
                                            join e in full on r.EventId equals e.Id
                                            where e.Id == x.Id
                                            select y;
                            if (!xrealarms.NullOrEmpty()) x.EmailAlarms.MergeRange(xrealarms);
                        }
                    });

                    #endregion 3. Use Linq to stitch secondary entities to primary entities
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
                if (dry.Organizer != null) dry.Organizer = null;
                if (dry.RecurrenceRule != null) dry.RecurrenceRule = null;
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