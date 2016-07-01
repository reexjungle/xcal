using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.repositories.concretes.relations;
using reexjungle.xcal.service.repositories.contracts;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.foundation.contracts;
using reexjungle.xmisc.infrastructure.concretes.io;
using reexjungle.xmisc.infrastructure.contracts;
using reexjungle.xmisc.technical.data.concretes.orm;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using reexjungle.xcal.infrastructure.serialization;

namespace reexjungle.xcal.service.repositories.concretes.ormlite
{
    /// <summary>
    /// Represents a repository of events hosted on an ORMlite datastore
    /// </summary>
    public class EventOrmRepository : IEventRepository, IOrmRepository
    {
        private readonly IDbConnectionFactory factory;
        private readonly IKeyGenerator<Guid> keygenerator;
        private readonly IAudioAlarmRepository aalarmrepository;
        private readonly IDisplayAlarmRepository dalarmrepository;
        private readonly IEmailAlarmRepository ealarmrepository;

        /// <summary>
        /// Gets the connection factory of this repository.
        /// </summary>
        public IDbConnectionFactory DbConnectionFactory => factory;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="keygenerator"></param>
        /// <param name="aalarmrepository"></param>
        /// <param name="dalarmrepository"></param>
        /// <param name="ealarmrepository"></param>
        /// <param name="factory"></param>
        public EventOrmRepository(
            IKeyGenerator<Guid> keygenerator,
            IAudioAlarmRepository aalarmrepository,
            IDisplayAlarmRepository dalarmrepository,
            IEmailAlarmRepository ealarmrepository,
            IDbConnectionFactory factory)
        {
            if (keygenerator == null) throw new ArgumentNullException(nameof(keygenerator));
            if (aalarmrepository == null) throw new ArgumentNullException(nameof(aalarmrepository));
            if (dalarmrepository == null) throw new ArgumentNullException(nameof(dalarmrepository));
            if (ealarmrepository == null) throw new ArgumentNullException(nameof(ealarmrepository));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            this.keygenerator = keygenerator;
            this.aalarmrepository = aalarmrepository;
            this.dalarmrepository = dalarmrepository;
            this.ealarmrepository = ealarmrepository;
            this.factory = factory;
        }

        public VEVENT Find(Guid key)
        {
            using (var db = factory.OpenDbConnection())
            {
                var dry = db.Select<VEVENT>(q => q.Id == key).FirstOrDefault();
                return dry != null ? Hydrate(dry) : null; 
            }
        }

        public IEnumerable<VEVENT> FindAll(IEnumerable<Guid> keys, int? skip = null, int? take = null)
        {
            using (var db = factory.OpenDbConnection())
            {
                var dry = db.Select<VEVENT>(q => Sql.In(q.Id, keys.ToArray()), skip, take);
                return !dry.NullOrEmpty() ? HydrateAll(dry) : dry; 
            }
        }

        public IEnumerable<VEVENT> Get(int? skip = null, int? take = null)
        {
            using (var db = factory.OpenDbConnection())
            {
                var dry = db.Select<VEVENT>(skip, take);
                return !dry.NullOrEmpty() ? HydrateAll(dry) : dry; 
            }
        }

        public void Save(VEVENT entity)
        {
            #region retrieve attributes of entity

            var organizer = entity.Organizer;
            var recur = entity.RecurrenceRule;
            var attendees = entity.Attendees;
            var attachbins = entity.Attachments.OfType<ATTACH_BINARY>().ToList();
            var attachuris = entity.Attachments.OfType<ATTACH_URI>().ToList();
            var contacts = entity.Contacts;
            var comments = entity.Comments;
            var rdates = entity.RecurrenceDates;
            var exdates = entity.ExceptionDates;
            var relateds = entity.RelatedTos;
            var resources = entity.Resources;
            var reqstats = entity.RequestStatuses;
            var aalarms = entity.Alarms.OfType<AUDIO_ALARM>().ToList();
            var dalarms = entity.Alarms.OfType<DISPLAY_ALARM>().ToList();
            var ealarms = entity.Alarms.OfType<EMAIL_ALARM>().ToList();

            #endregion retrieve attributes of entity

            #region save event and its attributes

            using (var db = factory.OpenDbConnection())
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
                        Id = keygenerator.GetNext(),
                        EventId = entity.Id,
                        OrganizerId = organizer.Id
                    };

                    db.MergeAll<REL_EVENTS_ORGANIZERS, Guid>(rorg.ToSingleton(), ororgs);
                }
                else db.RemoveAll<REL_EVENTS_ORGANIZERS, Guid>(ororgs);

                if (recur != null)
                {
                    db.Save(recur);
                    var rrecur = new REL_EVENTS_RECURS
                    {
                        Id = keygenerator.GetNext(),
                        EventId = entity.Id,
                        RecurId = recur.Id
                    };

                    db.MergeAll<REL_EVENTS_RECURS, Guid>(rrecur.ToSingleton(), orrecurs);
                }
                else db.RemoveAll<REL_EVENTS_RECURS, Guid>(orrecurs);

                if (!attendees.NullOrEmpty())
                {
                    db.SaveAll(attendees.Distinct());
                    var rattendees = attendees.Select(x => new REL_EVENTS_ATTENDEES
                    {
                        Id = keygenerator.GetNext(),
                        EventId = entity.Id,
                        AttendeeId = x.Id
                    });

                    db.MergeAll<REL_EVENTS_ATTENDEES, Guid>(rattendees, orattendees);
                }
                else db.RemoveAll<REL_EVENTS_ATTENDEES, Guid>(orattendees);

                if (!attachbins.NullOrEmpty())
                {
                    db.SaveAll(attachbins.Distinct());
                    var rattachbins = attachbins.Select(x => new REL_EVENTS_ATTACHBINS
                    {
                        Id = keygenerator.GetNext(),
                        EventId = entity.Id,
                        AttachmentId = x.Id
                    });
                    db.MergeAll<REL_EVENTS_ATTACHBINS, Guid>(rattachbins, orattachbins);
                }
                else db.RemoveAll<REL_EVENTS_ATTACHBINS, Guid>(orattachbins);

                if (!attachuris.NullOrEmpty())
                {
                    db.SaveAll(attachuris.Distinct());
                    var rattachuris = attachuris.Select(x => new REL_EVENTS_ATTACHURIS
                    {
                        Id = keygenerator.GetNext(),
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
                        Id = keygenerator.GetNext(),
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
                        Id = keygenerator.GetNext(),
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
                        Id = keygenerator.GetNext(),
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
                        Id = keygenerator.GetNext(),
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
                        Id = keygenerator.GetNext(),
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
                        Id = keygenerator.GetNext(),
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
                        Id = keygenerator.GetNext(),
                        EventId = entity.Id,
                        ReqStatsId = x.Id
                    });
                    db.MergeAll(rreqstats, orreqstats);
                }
                else db.RemoveAll(orreqstats);

                if (!aalarms.NullOrEmpty())
                {
                    aalarmrepository.SaveAll(aalarms.Distinct());
                    var raalarms = aalarms.Select(x => new REL_EVENTS_AUDIO_ALARMS
                    {
                        Id = keygenerator.GetNext(),
                        EventId = entity.Id,
                        AlarmId = x.Id
                    });
                    db.MergeAll(raalarms, oraalarms);
                }
                else db.RemoveAll(orealarms);

                if (!dalarms.NullOrEmpty())
                {
                    dalarmrepository.SaveAll(dalarms.Distinct());
                    var rdalarms = dalarms.Select(x => new REL_EVENTS_DISPLAY_ALARMS
                    {
                        Id = keygenerator.GetNext(),
                        EventId = entity.Id,
                        AlarmId = x.Id
                    });
                    db.MergeAll(rdalarms, ordalarms);
                }
                else db.RemoveAll(ordalarms);

                if (!ealarms.NullOrEmpty())
                {
                    ealarmrepository.SaveAll(ealarms.Distinct());
                    var realarms = ealarms.Select(x => new REL_EVENTS_EMAIL_ALARMS
                    {
                        Id = keygenerator.GetNext(),
                        EventId = entity.Id,
                        AlarmId = x.Id
                    });
                    db.MergeAll(realarms, orealarms);
                }
                else db.RemoveAll(orealarms); 
            }

            #endregion save event and its attributes
        }

        public void Patch(VEVENT source, IEnumerable<string> fields, IEnumerable<Guid> keys = null)
        {
            #region construct anonymous fields using expression lambdas

            Expression<Func<VEVENT, object>> primitives = x => new
            {
                x.Uid,
                x.Start,
                x.Created,
                x.Description,
                x.Classification,
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
                x.Categories,
                x.Duration,
                x.RecurrenceRule
            };

            Expression<Func<VEVENT, object>> relations = x => new
            {
                x.Attendees,
                x.Attachments,
                x.Contacts,
                x.Comments,
                x.RecurrenceDates,
                x.ExceptionDates,
                x.RelatedTos,
                x.Resources,
                x.RequestStatuses,
                x.Alarms,
                x.Organizer,
                x.RecurrenceId,
            };

            //4. Get list of selected relationals
            var selection = fields as IList<string> ?? fields.ToList();
            var srelation = relations.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);

            //5. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);

            #endregion construct anonymous fields using expression lambdas

            using (var db = factory.OpenDbConnection())
            {
                var okeys = (keys != null)
                    ? db.SelectParam<VEVENT, Guid>(q => q.Id, p => Sql.In(p.Id, keys.ToArray())).ToArray()
                    : db.SelectParam<VEVENT, Guid>(q => q.Id).ToArray();

                if (!srelation.NullOrEmpty())
                {
                    Expression<Func<VEVENT, object>> orgexpr = y => y.Organizer;
                    Expression<Func<VEVENT, object>> recurexpr = y => y.RecurrenceRule;
                    Expression<Func<VEVENT, object>> attendsexpr = y => y.Attendees;
                    Expression<Func<VEVENT, object>> attachsexpr = y => y.Attachments;
                    Expression<Func<VEVENT, object>> contactsexpr = y => y.Contacts;
                    Expression<Func<VEVENT, object>> commentsexpr = y => y.Comments;
                    Expression<Func<VEVENT, object>> rdatesexpr = y => y.RecurrenceDates;
                    Expression<Func<VEVENT, object>> exdatesexpr = y => y.ExceptionDates;
                    Expression<Func<VEVENT, object>> relatedtosexpr = y => y.RelatedTos;
                    Expression<Func<VEVENT, object>> resourcesexpr = y => y.Resources;
                    Expression<Func<VEVENT, object>> reqstatsexpr = y => y.RequestStatuses;
                    Expression<Func<VEVENT, object>> alarmsexpr = y => y.Alarms;


                    #region save relational attributes of entities

                    if (selection.Contains(orgexpr.GetMemberName()))
                    {
                        var ororgs = db.Select<REL_EVENTS_ORGANIZERS>(q => Sql.In(q.EventId, okeys));
                        if (source.Organizer != null)
                        {
                            db.Save(source.Organizer);
                            var rorgs = okeys.Select(x => new REL_EVENTS_ORGANIZERS
                            {
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                OrganizerId = source.Organizer.Id
                            });
                            db.RemoveAll(ororgs);
                            db.SaveAll(rorgs);
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
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                RecurId = source.RecurrenceRule.Id
                            });
                            db.RemoveAll(orrecurs);
                            db.SaveAll(rrecurs);
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
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                AttendeeId = y.Id
                            }));
                            db.RemoveAll(orattendees);
                            db.SaveAll(rattendees);
                        }
                        db.RemoveAll(orattendees);
                    }

                    if (selection.Contains(attachsexpr.GetMemberName()))
                    {
                        var attachbins = source.Attachments.OfType<ATTACH_BINARY>();
                        var attachuris = source.Attachments.OfType<ATTACH_URI>();

                        var orattachbins = db.Select<REL_EVENTS_ATTACHBINS>(q => Sql.In(q.EventId, okeys));
                        if (attachbins.Any())
                        {
                            db.SaveAll(attachbins);
                            var rattachbins = okeys.SelectMany(x => attachbins.Select(y => new REL_EVENTS_ATTACHBINS
                            {
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                AttachmentId = y.Id
                            }));

                            db.RemoveAll(orattachbins);
                            db.SaveAll(rattachbins);
                        }
                        else db.RemoveAll(orattachbins);

                        var orattachuris = db.Select<REL_EVENTS_ATTACHURIS>(q => Sql.In(q.EventId, okeys));
                        if (attachuris.Any())
                        {
                            db.SaveAll(attachuris);
                            var rattachuris = okeys.SelectMany(x => attachuris.Select(y => new REL_EVENTS_ATTACHURIS
                            {
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                AttachmentId = y.Id
                            }));
                            db.RemoveAll(orattachuris);
                            db.SaveAll(rattachuris);
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
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                ContactId = y.Id
                            }));
                            db.RemoveAll(orcontacts);
                            db.SaveAll(rcontacts);
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
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                CommentId = y.Id
                            }));
                            db.RemoveAll(orcomments);
                            db.SaveAll(rcomments);
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
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                RecurrenceDateId = y.Id
                            }));
                            db.RemoveAll(orrdates);
                            db.SaveAll(rrdates);
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
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                ExceptionDateId = y.Id
                            }));
                            db.RemoveAll(orexdates);
                            db.SaveAll(rexdates);
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
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                RelatedToId = y.Id
                            }));

                            db.RemoveAll(orrelatedtos);
                            db.SaveAll(rrelatedtos);
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
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                ResourcesId = y.Id
                            }));
                            db.RemoveAll(orresources);
                            db.SaveAll(rresources);
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
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                ReqStatsId = y.Id
                            }));
                            db.RemoveAll(orreqstats);
                            db.SaveAll(rreqstats);
                        }
                        else db.RemoveAll(orreqstats);
                    }

                    if (selection.Contains(alarmsexpr.GetMemberName()))
                    {
                        var aalarms = source.Alarms.OfType<AUDIO_ALARM>();
                        var ealarms = source.Alarms.OfType<EMAIL_ALARM>();
                        var dalarms = source.Alarms.OfType<DISPLAY_ALARM>();

                        var oraalarms = db.Select<REL_EVENTS_AUDIO_ALARMS>(q => Sql.In(q.EventId, okeys));
                        if (aalarms.Any())
                        {
                            aalarmrepository.SaveAll(aalarms);
                            var raalarms = okeys.SelectMany(x => source.Alarms.Select(y => new REL_EVENTS_AUDIO_ALARMS
                            {
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                AlarmId = y.Id
                            }));
                            db.RemoveAll(oraalarms);
                            db.SaveAll(raalarms);
                        }
                        else db.RemoveAll(oraalarms);

                        var ordalarms = db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => Sql.In(q.EventId, okeys));
                        if (dalarms.Any())
                        {
                            dalarmrepository.SaveAll(dalarms);
                            var rdalarms = okeys.SelectMany(x => dalarms.Select(y => new REL_EVENTS_DISPLAY_ALARMS
                            {
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                AlarmId = y.Id
                            }));
                            db.RemoveAll(ordalarms);
                            db.SaveAll(rdalarms);
                        }
                        else db.RemoveAll(ordalarms);

                        var orealarms = db.Select<REL_EVENTS_EMAIL_ALARMS>(q => Sql.In(q.EventId, okeys));
                        if (ealarms.Any())
                        {
                            ealarmrepository.SaveAll(ealarms);
                            var realarms = okeys.SelectMany(x => ealarms.Select(y => new REL_EVENTS_EMAIL_ALARMS
                            {
                                Id = keygenerator.GetNext(),
                                EventId = x,
                                AlarmId = y.Id
                            }));

                            db.RemoveAll(orealarms);
                            db.SaveAll(realarms);
                        }
                        else db.RemoveAll(orealarms);
                    }

                    #endregion save relational attributes of entities
                }

                var sprimitivelist = sprimitives as IList<string> ?? sprimitives.ToList();
                if (!sprimitivelist.NullOrEmpty())
                {
                    var patchstr = $"f => new {{ {string.Join(", ", sprimitivelist.Select(x => $"f.{x}"))} }}";

                    var patchexpr = patchstr.CompileToExpressionFunc<VEVENT, object>(
                        CodeDomLanguage.csharp,
                        "System.dll", "System.Core.dll",
                        typeof(CalendarWriter).Assembly.Location,
                        typeof(VEVENT).Assembly.Location,
                        typeof(IContainsKey<Guid>).Assembly.Location);

                    if (!okeys.NullOrEmpty()) db.UpdateOnly(source, patchexpr, q => Sql.In(q.Id, okeys.ToArray()));
                    else db.UpdateOnly(source, patchexpr);
                } 
            }
        }

        public void Erase(Guid key)
        {
            using (var db = factory.OpenDbConnection())
            {
                db.Delete<VEVENT>(q => q.Id == key); 
            }
        }

        public void SaveAll(IEnumerable<VEVENT> entities)
        {
            #region 1. retrieve attributes of entities

            var organizers = entities.Select(x => x.Organizer).ToList();
            var recurs = entities.Select(x => x.RecurrenceRule).ToList();
            var attendees = entities.SelectMany(x => x.Attendees).ToList();
            var attachments = entities.SelectMany(x => x.Attachments).ToList();
            var attachbins = attachments.OfType<ATTACH_BINARY>().ToList();
            var attachuris = attachments.OfType<ATTACH_URI>().ToList();
            var contacts = entities.SelectMany(x => x.Contacts).ToList();
            var comments = entities.SelectMany(x => x.Comments).ToList();
            var rdates = entities.SelectMany(x => x.RecurrenceDates).ToList();
            var exdates = entities.SelectMany(x => x.ExceptionDates).ToList();
            var relateds = entities.SelectMany(x => x.RelatedTos).ToList();
            var resources = entities.SelectMany(x => x.Resources).ToList();
            var reqstats = entities.SelectMany(x => x.RequestStatuses).ToList();
            var alarms = entities.SelectMany(x => x.Alarms).ToList();
            var aalarms = alarms.OfType<AUDIO_ALARM>().ToList();
            var dalarms = alarms.OfType<DISPLAY_ALARM>().ToList();
            var ealarms = alarms.OfType<EMAIL_ALARM>().ToList();

            #endregion 1. retrieve attributes of entities

            #region 2. save attributes of entities

            using (var db = factory.OpenDbConnection())
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

                if (!organizers.NullOrEmpty())
                {
                    db.SaveAll(organizers.Distinct());
                    var rorgs = entities.Where(x => x.Organizer != null)
                        .Select(x => new REL_EVENTS_ORGANIZERS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = x.Id,
                            OrganizerId = x.Organizer.Id
                        }).ToList();

                    db.MergeAll(rorgs, ororgs);
                }
                else db.RemoveAll(ororgs);

                if (!recurs.NullOrEmpty())
                {
                    db.SaveAll(recurs.Distinct());
                    var rrecurs = entities.Where(x => x.RecurrenceRule != null)
                        .Select(x => new REL_EVENTS_RECURS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = x.Id,
                            RecurId = x.RecurrenceRule.Id
                        }).ToList();

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
                                Id = keygenerator.GetNext(),
                                EventId = e.Id,
                                AttendeeId = x.Id
                            })).ToList();

                    db.MergeAll(rattendees, orattendees);
                }
                else db.RemoveAll(orattendees);

                if (attachbins.Any())
                {
                    db.SaveAll(attachbins.Distinct());

                    var rattachbins = entities.SelectMany(e => attachbins
                            .Select(x => new REL_EVENTS_ATTACHBINS
                            {
                                Id = keygenerator.GetNext(),
                                EventId = e.Id,
                                AttachmentId = x.Id
                            })).ToList();

                    db.MergeAll(rattachbins, orattachbins);
                }
                else db.RemoveAll(orattachbins);

                if (attachuris.Any())
                {
                    db.SaveAll(attachuris.Distinct());
                    var rattachuris = entities.SelectMany(e => attachuris
                            .Select(x => new REL_EVENTS_ATTACHURIS
                            {
                                Id = keygenerator.GetNext(),
                                EventId = e.Id,
                                AttachmentId = x.Id
                            })).ToList();
                    db.MergeAll(rattachuris, orattachuris);
                }
                else db.RemoveAll(orattachuris);

                if (!contacts.NullOrEmpty())
                {
                    db.SaveAll(contacts.Distinct());
                    var rcontacts = entities.Where(x => !x.Contacts.NullOrEmpty())
                        .SelectMany(e => e.Contacts.Select(x => new REL_EVENTS_CONTACTS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = e.Id,
                            ContactId = x.Id
                        })).ToList();
                    db.MergeAll(rcontacts, orcontacts);
                }
                else db.RemoveAll(orcontacts);

                if (!comments.NullOrEmpty())
                {
                    db.SaveAll(comments.Distinct());
                    var rcomments = entities.Where(x => !x.Comments.NullOrEmpty())
                        .SelectMany(e => e.Comments.Select(x => new REL_EVENTS_COMMENTS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = e.Id,
                            CommentId = x.Id
                        })).ToList();
                    db.MergeAll(rcomments, orcomments);
                }
                else db.RemoveAll(orcomments);

                if (!rdates.NullOrEmpty())
                {
                    db.SaveAll(rdates.Distinct());
                    var rrdates = entities.Where(x => !x.RecurrenceDates.NullOrEmpty())
                        .SelectMany(e => e.RecurrenceDates.Select(x => new REL_EVENTS_RDATES
                        {
                            Id = keygenerator.GetNext(),
                            EventId = e.Id,
                            RecurrenceDateId = x.Id
                        })).ToList();
                    db.MergeAll(rrdates, orrdates);
                }
                else db.RemoveAll(orrdates);

                if (!exdates.NullOrEmpty())
                {
                    db.SaveAll(exdates.Distinct());
                    var rexdates = entities.Where(x => !x.ExceptionDates.NullOrEmpty())
                        .SelectMany(e => e.ExceptionDates.Select(x => new REL_EVENTS_EXDATES
                        {
                            Id = keygenerator.GetNext(),
                            EventId = e.Id,
                            ExceptionDateId = x.Id
                        })).ToList();
                    db.MergeAll(rexdates, orexdates);
                }
                else db.RemoveAll(orexdates);

                if (!relateds.NullOrEmpty())
                {
                    db.SaveAll(relateds.Distinct());
                    var rrelateds = entities.Where(x => !x.RelatedTos.NullOrEmpty())
                        .SelectMany(e => e.RelatedTos.Select(x => new REL_EVENTS_RELATEDTOS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = e.Id,
                            RelatedToId = x.Id
                        })).ToList();
                    db.MergeAll(rrelateds, orrelateds);
                }
                else db.RemoveAll(orrelateds);

                if (!resources.NullOrEmpty())
                {
                    db.SaveAll(resources.Distinct());
                    var rresources = entities.Where(x => !x.Resources.NullOrEmpty())
                        .SelectMany(e => e.Resources.Select(x => new REL_EVENTS_RESOURCES
                        {
                            Id = keygenerator.GetNext(),
                            EventId = e.Id,
                            ResourcesId = x.Id
                        })).ToList();
                    db.MergeAll(rresources, orresources);
                }
                else db.RemoveAll(orresources);

                if (!reqstats.NullOrEmpty())
                {
                    db.SaveAll(reqstats.Distinct());
                    var rreqstats = entities.Where(x => !x.RequestStatuses.NullOrEmpty())
                        .SelectMany(e => e.RequestStatuses.Select(x => new REL_EVENTS_REQSTATS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = e.Id,
                            ReqStatsId = x.Id
                        })).ToList();
                    db.MergeAll(rreqstats, orreqstats);
                }
                else db.RemoveAll(orreqstats);

                if (aalarms.Any())
                {
                    aalarmrepository.SaveAll(aalarms.Distinct());
                    var raalarms = entities.SelectMany(e => aalarms.Select(x => new REL_EVENTS_AUDIO_ALARMS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = e.Id,
                            AlarmId = x.Id
                        })).ToList();
                    db.MergeAll(raalarms, oraalarms);
                }
                else db.RemoveAll(oraalarms);

                if (dalarms.Any())
                {
                    dalarmrepository.SaveAll(dalarms.Distinct());
                    var rdalarms = entities.SelectMany(e => dalarms.Select(x => new REL_EVENTS_DISPLAY_ALARMS
                    {
                        Id = keygenerator.GetNext(),
                        EventId = e.Id,
                        AlarmId = x.Id
                    })).ToList();

                    db.MergeAll(rdalarms, ordalarms);
                }
                else db.RemoveAll(ordalarms);

                if (ealarms.Any())
                {
                    ealarmrepository.SaveAll(ealarms.Distinct());
                    var realarms = entities.SelectMany(e => ealarms.Select(x => new REL_EVENTS_EMAIL_ALARMS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = e.Id,
                            AlarmId = x.Id
                        })).ToList();

                    db.MergeAll(realarms, orealarms);
                }
                else db.RemoveAll(orealarms); 
            }

            #endregion 2. save attributes of entities
        }

        public void EraseAll(IEnumerable<Guid> keys = null)
        {
            using (var db = factory.OpenDbConnection())
            {
                if (!keys.NullOrEmpty()) db.Delete<VEVENT>(q => Sql.In(q.Id, keys.ToArray()));
                else db.DeleteAll<VEVENT>(); 
            }
        }

        public VEVENT Hydrate(VEVENT dry)
        {
            using (var db = factory.OpenDbConnection())
            {
                var organizers = db.Select<ORGANIZER, VEVENT, REL_EVENTS_ORGANIZERS>(
                    r => r.OrganizerId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!organizers.NullOrEmpty()) dry.Organizer = organizers.First();

                var recurs = db.Select<RECUR, VEVENT, REL_EVENTS_RECURS>(
                    r => r.RecurId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!recurs.NullOrEmpty()) dry.RecurrenceRule = recurs.First();

                var attachbins = db.Select<ATTACH_BINARY, VEVENT, REL_EVENTS_ATTACHBINS>(
                    r => r.AttachmentId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!attachbins.NullOrEmpty()) dry.Attachments.MergeRange(attachbins);

                var attachuris = db.Select<ATTACH_URI, VEVENT, REL_EVENTS_ATTACHURIS>(
                    r => r.AttachmentId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!attachuris.NullOrEmpty()) dry.Attachments.MergeRange(attachuris);

                var attendees = db.Select<ATTENDEE, VEVENT, REL_EVENTS_ATTENDEES>(
                    r => r.AttendeeId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!attendees.NullOrEmpty()) dry.Attendees.MergeRange(attendees);

                var comments = db.Select<COMMENT, VEVENT, REL_EVENTS_COMMENTS>(
                    r => r.CommentId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!comments.NullOrEmpty()) dry.Comments.MergeRange(comments);

                var contacts = db.Select<CONTACT, VEVENT, REL_EVENTS_CONTACTS>(
                    r => r.ContactId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!contacts.NullOrEmpty()) dry.Contacts.MergeRange(contacts);

                var rdates = db.Select<RDATE, VEVENT, REL_EVENTS_RDATES>(
                    r => r.RecurrenceDateId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!rdates.NullOrEmpty()) dry.RecurrenceDates.MergeRange(rdates);

                var exdates = db.Select<EXDATE, VEVENT, REL_EVENTS_EXDATES>(
                    r => r.ExceptionDateId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!exdates.NullOrEmpty()) dry.ExceptionDates.MergeRange(exdates);

                var relatedtos = db.Select<RELATEDTO, VEVENT, REL_EVENTS_RELATEDTOS>(
                    r => r.RelatedToId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                dry.RelatedTos.MergeRange(relatedtos);

                var reqstats = db.Select<REQUEST_STATUS, VEVENT, REL_EVENTS_REQSTATS>(
                    r => r.ReqStatsId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                dry.RequestStatuses.MergeRange(reqstats);

                var resources = db.Select<RESOURCES, VEVENT, REL_EVENTS_RESOURCES>(
                    r => r.ResourcesId,
                    r => r.EventId,
                    e => e.Id == dry.Id);
                if (!resources.NullOrEmpty()) dry.Resources.MergeRange(resources);

                var raalarms = db.Select<REL_EVENTS_AUDIO_ALARMS>(q => q.EventId == dry.Id);
                if (!raalarms.NullOrEmpty())
                {
                    dry.Alarms.MergeRange(aalarmrepository.FindAll(raalarms.Select(x => x.AlarmId).ToList()));
                }

                var rdalarms = db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => q.EventId == dry.Id);
                if (!rdalarms.NullOrEmpty())
                {
                    dry.Alarms.MergeRange(dalarmrepository.FindAll(rdalarms.Select(x => x.AlarmId).ToList()));
                }

                var realarms = db.Select<REL_EVENTS_EMAIL_ALARMS>(q => q.EventId == dry.Id);
                if (!realarms.NullOrEmpty())
                {
                    dry.Alarms.MergeRange(ealarmrepository.FindAll(realarms.Select(x => x.AlarmId).ToList()));
                } 
            }

            return dry;
        }

        public IEnumerable<VEVENT> HydrateAll(IEnumerable<VEVENT> dry)
        {
            var events = dry.ToList();
            var keys = events.Select(q => q.Id);
            using (var db = factory.OpenDbConnection())
            {
                var okeys = db.SelectParam<VEVENT>(q => q.Id, p => Sql.In(p.Id, keys));
                if (!okeys.NullOrEmpty())
                {
                    #region 1. retrieve relationships

                    var rorgs = db.Select<REL_EVENTS_ORGANIZERS>(q => Sql.In(q.EventId, okeys));
                    var rrecurs = db.Select<REL_EVENTS_RECURS>(q => Sql.In(q.EventId, okeys));
                    var rattendees = db.Select<REL_EVENTS_ATTENDEES>(q => Sql.In(q.EventId, okeys));
                    var rcomments = db.Select<REL_EVENTS_COMMENTS>(q => Sql.In(q.EventId, okeys));
                    var rattachbins = db.Select<REL_EVENTS_ATTACHBINS>(q => Sql.In(q.EventId, okeys));
                    var rattachuris = db.Select<REL_EVENTS_ATTACHURIS>(q => Sql.In(q.EventId, okeys));
                    var rcontacts = db.Select<REL_EVENTS_CONTACTS>(q => Sql.In(q.EventId, okeys));
                    var rexdates = db.Select<REL_EVENTS_EXDATES>(q => Sql.In(q.EventId, okeys));
                    var rrdates = db.Select<REL_EVENTS_RDATES>(q => Sql.In(q.EventId, okeys));
                    var rrelatedtos = db.Select<REL_EVENTS_RELATEDTOS>(q => Sql.In(q.EventId, okeys));
                    var rreqstats = db.Select<REL_EVENTS_REQSTATS>(q => Sql.In(q.EventId, okeys));
                    var rresources = db.Select<REL_EVENTS_RESOURCES>(q => Sql.In(q.EventId, okeys));
                    var raalarms = db.Select<REL_EVENTS_AUDIO_ALARMS>(q => Sql.In(q.EventId, okeys));
                    var rdalarms = db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => Sql.In(q.EventId, okeys));
                    var realarms = db.Select<REL_EVENTS_EMAIL_ALARMS>(q => Sql.In(q.EventId, okeys));

                    #endregion 1. retrieve relationships

                    #region 2. retrieve secondary entitiesB

                    var orgs = (!rorgs.NullOrEmpty()) ? db.Select<ORGANIZER>(q => Sql.In(q.Id, rorgs.Select(r => r.OrganizerId))) : null;
                    var recurs = (!rrecurs.NullOrEmpty()) ? db.Select<RECUR>(q => Sql.In(q.Id, rrecurs.Select(r => r.RecurId))) : null;
                    var attendees = (!rattendees.NullOrEmpty()) ? db.Select<ATTENDEE>(q => Sql.In(q.Id, rattendees.Select(r => r.AttendeeId))) : null;
                    var comments = (!rcomments.NullOrEmpty()) ? db.Select<COMMENT>(q => Sql.In(q.Id, rcomments.Select(r => r.CommentId))) : null;
                    var attachbins = (!rattachbins.NullOrEmpty()) ? db.Select<ATTACH_BINARY>(q => Sql.In(q.Id, rattachbins.Select(r => r.AttachmentId))) : null;
                    var attachuris = (!rattachuris.NullOrEmpty()) ? db.Select<ATTACH_URI>(q => Sql.In(q.Id, rattachuris.Select(r => r.AttachmentId))) : null;
                    var contacts = (!rcontacts.NullOrEmpty()) ? db.Select<CONTACT>(q => Sql.In(q.Id, rcontacts.Select(r => r.ContactId))) : null;
                    var exdates = (!rexdates.NullOrEmpty()) ? db.Select<EXDATE>(q => Sql.In(q.Id, rexdates.Select(r => r.ExceptionDateId))) : null;
                    var rdates = (!rrdates.NullOrEmpty()) ? db.Select<RDATE>(q => Sql.In(q.Id, rrdates.Select(r => r.RecurrenceDateId))) : null;
                    var relatedtos = (!rrelatedtos.NullOrEmpty()) ? db.Select<RELATEDTO>(q => Sql.In(q.Id, rrelatedtos.Select(r => r.RelatedToId))) : null;
                    var reqstats = (!rreqstats.NullOrEmpty()) ? db.Select<REQUEST_STATUS>(q => Sql.In(q.Id, rreqstats.Select(r => r.ReqStatsId))) : null;
                    var resources = (!rresources.NullOrEmpty()) ? db.Select<RESOURCES>(q => Sql.In(q.Id, rresources.Select(r => r.ResourcesId))) : null;
                    var aalarms = (!raalarms.NullOrEmpty()) ? aalarmrepository.FindAll(raalarms.Select(x => x.AlarmId)) : null;
                    var dalarms = (!rdalarms.NullOrEmpty()) ? dalarmrepository.FindAll(rdalarms.Select(x => x.AlarmId)) : null;
                    var ealarms = (!realarms.NullOrEmpty()) ? ealarmrepository.FindAll(realarms.Select(x => x.AlarmId)) : null;

                    #endregion 2. retrieve secondary entitiesB

                    #region 3. Use Linq to stitch secondary entities to primary entities

                    events.ForEach(x =>
                    {
                        if (!orgs.NullOrEmpty())
                        {
                            var xorgs = from y in orgs
                                        join r in rorgs on y.Id equals r.OrganizerId
                                        join e in events on r.EventId equals e.Id
                                        where e.Id == x.Id
                                        select y;
                            if (!xorgs.NullOrEmpty()) x.Organizer = xorgs.First();
                        }

                        if (!recurs.NullOrEmpty())
                        {
                            var xrecurs = from y in recurs
                                          join r in rorgs on y.Id equals r.OrganizerId
                                          join e in events on r.EventId equals e.Id
                                          where e.Id == x.Id
                                          select y;
                            if (!xrecurs.NullOrEmpty()) x.RecurrenceRule = xrecurs.First();
                        }

                        if (!comments.NullOrEmpty())
                        {
                            var xcomments = from y in comments
                                            join r in rcomments on y.Id equals r.CommentId
                                            join e in events on r.EventId equals e.Id
                                            where e.Id == x.Id
                                            select y;
                            if (!xcomments.NullOrEmpty()) x.Comments.MergeRange(xcomments);
                        }

                        if (!attendees.NullOrEmpty())
                        {
                            var xattendees = from y in attendees
                                             join r in rattendees on y.Id equals r.AttendeeId
                                             join e in events on r.EventId equals e.Id
                                             where e.Id == x.Id
                                             select y;
                            if (!xattendees.NullOrEmpty()) x.Attendees.MergeRange(xattendees);
                        }

                        if (!attachbins.NullOrEmpty())
                        {
                            var xattachbins = from y in attachbins
                                              join r in rattachbins on y.Id equals r.AttachmentId
                                              join e in events on r.EventId equals e.Id
                                              where e.Id == x.Id
                                              select y;
                            if (!xattachbins.NullOrEmpty()) x.Attachments.MergeRange(xattachbins);
                        }

                        if (!attachuris.NullOrEmpty())
                        {
                            var xattachuris = from y in attachuris
                                              join r in rattachuris on y.Id equals r.AttachmentId
                                              join e in events on r.EventId equals e.Id
                                              where e.Id == x.Id
                                              select y;
                            if (!xattachuris.NullOrEmpty()) x.Attachments.MergeRange(xattachuris);
                        }

                        if (!contacts.NullOrEmpty())
                        {
                            var xcontacts = from y in contacts
                                            join r in rcontacts on y.Id equals r.ContactId
                                            join e in events on r.EventId equals e.Id
                                            where e.Id == x.Id
                                            select y;

                            if (!xcontacts.NullOrEmpty()) x.Contacts.MergeRange(xcontacts);
                        }

                        if (!rdates.NullOrEmpty())
                        {
                            var xrdates = from y in rdates
                                          join r in rrdates on y.Id equals r.RecurrenceDateId
                                          join e in events on r.EventId equals e.Id
                                          where e.Id == x.Id
                                          select y;
                            if (!xrdates.NullOrEmpty()) x.RecurrenceDates.MergeRange(xrdates);
                        }

                        if (!exdates.NullOrEmpty())
                        {
                            var xexdates = from y in exdates
                                           join r in rexdates on y.Id equals r.ExceptionDateId
                                           join e in events on r.EventId equals e.Id
                                           where e.Id == x.Id
                                           select y;
                            if (!xexdates.NullOrEmpty()) x.ExceptionDates.MergeRange(xexdates);
                        }

                        if (!relatedtos.NullOrEmpty())
                        {
                            var xrelatedtos = from y in relatedtos
                                              join r in rrelatedtos on y.Id equals r.RelatedToId
                                              join e in events on r.EventId equals e.Id
                                              where e.Id == x.Id
                                              select y;
                            if (!xrelatedtos.NullOrEmpty()) x.RelatedTos.MergeRange(xrelatedtos);
                        }

                        if (!reqstats.NullOrEmpty())
                        {
                            var xreqstats = from y in reqstats
                                            join r in rreqstats on y.Id equals r.ReqStatsId
                                            join e in events on r.EventId equals e.Id
                                            where e.Id == x.Id
                                            select y;
                            if (!xreqstats.NullOrEmpty()) x.RequestStatuses.MergeRange(xreqstats);
                        }

                        if (!resources.NullOrEmpty())
                        {
                            var xresources = from y in resources
                                             join r in rresources on y.Id equals r.ResourcesId
                                             join e in events on r.EventId equals e.Id
                                             where e.Id == x.Id
                                             select y;
                            if (!xresources.NullOrEmpty()) x.Resources.MergeRange(xresources);
                        }

                        if (!aalarms.NullOrEmpty())
                        {
                            var xraalarms = from y in aalarms
                                            join r in raalarms on y.Id equals r.AlarmId
                                            join e in events on r.EventId equals e.Id
                                            where e.Id == x.Id
                                            select y;
                            if (!xraalarms.NullOrEmpty()) x.Alarms.MergeRange(xraalarms);
                        }

                        if (!dalarms.NullOrEmpty())
                        {
                            var xrdalarms = from y in dalarms
                                            join r in rdalarms on y.Id equals r.AlarmId
                                            join e in events on r.EventId equals e.Id
                                            where e.Id == x.Id
                                            select y;
                            if (!xrdalarms.NullOrEmpty()) x.Alarms.MergeRange(xrdalarms);
                        }

                        if (!ealarms.NullOrEmpty())
                        {
                            var xrealarms = from y in ealarms
                                            join r in realarms on y.Id equals r.AlarmId
                                            join e in events on r.EventId equals e.Id
                                            where e.Id == x.Id
                                            select y;
                            if (!xrealarms.NullOrEmpty()) x.Alarms.MergeRange(xrealarms);
                        }
                    });

                    #endregion 3. Use Linq to stitch secondary entities to primary entities
                } 
            }

            return events;
        }

        public bool ContainsKey(Guid key)
        {
            using (var db = factory.OpenDbConnection())
            {
                return db.Count<VEVENT>(q => q.Id == key) != 0; 
            }
        }

        public bool ContainsKeys(IEnumerable<Guid> keys, ExpectationMode mode = ExpectationMode.Optimistic)
        {
            var dkeys = keys.Distinct();
            using (var db = factory.OpenDbConnection())
            {

                return mode == ExpectationMode.Pessimistic || mode == ExpectationMode.Unknown
                    ? db.Count<VEVENT>(q => Sql.In(q.Id, dkeys)) == dkeys.Count()
                    : db.Count<VEVENT>(q => Sql.In(q.Id, dkeys)) != 0; 
            }
        }

        public IEnumerable<Guid> GetKeys(int? skip = null, int? take = null)
        {
            using (var db = factory.OpenDbConnection())
            {
                return db.SelectParam<VEVENT, Guid>(q => q.Id, skip, take); 
            }
        }

        public IEnumerable<VEVENT> DehydrateAll(IEnumerable<VEVENT> full)
        {
            var events = full as IList<VEVENT> ?? full.ToList();
            foreach (var vevent in events)
            {
                Dehydrate(vevent);
            }

            return events;
        }

        public VEVENT Dehydrate(VEVENT @event)
        {
            if (@event.Organizer != null) @event.Organizer = null;
            if (@event.RecurrenceRule != null) @event.RecurrenceRule = null;
            if (@event.Attendees.Any()) @event.Attendees.Clear();
            if (@event.Attachments.Any()) @event.Attachments.Clear();
            if (@event.Contacts.Any()) @event.Contacts.Clear();
            if (@event.Comments.Any()) @event.Comments.Clear();
            if (@event.RecurrenceDates.Any()) @event.RecurrenceDates.Clear();
            if (@event.ExceptionDates.Any()) @event.ExceptionDates.Clear();
            if (@event.RelatedTos.Any()) @event.RelatedTos.Clear();
            if (@event.RequestStatuses.Any()) @event.RequestStatuses.Clear();
            if (@event.Resources.Any()) @event.Resources.Clear();
            if (@event.Alarms.Any()) @event.Alarms.Clear();
            return @event;
        }
    }
}