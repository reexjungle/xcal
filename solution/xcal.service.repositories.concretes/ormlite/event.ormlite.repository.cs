using reexjungle.technical.data.concretes.extensions.ormlite;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.repositories.concretes.relations;
using reexjungle.xcal.service.repositories.contracts;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.foundation.contracts;
using reexjungle.xmisc.infrastructure.concretes.io;
using reexjungle.xmisc.infrastructure.contracts;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace reexjungle.xcal.service.repositories.concretes.ormlite
{
    /// <summary>
    /// Represents a repository of events hosted on an ORMlite datastore
    /// </summary>
    public class EventOrmLiteRepository : IEventRepository, IOrmLiteRepository, IDisposable
    {
        private IDbConnection conn;
        private readonly IDbConnectionFactory factory;
        private readonly IKeyGenerator<Guid> keygenerator;
        private readonly IAudioAlarmRepository aalarmrepository;
        private readonly IDisplayAlarmRepository dalarmrepository;
        private readonly IEmailAlarmRepository ealarmrepository;

        private IDbConnection db
        {
            get { return conn ?? (conn = factory.OpenDbConnection()); }
        }

        /// <summary>
        /// Gets the connection factory of this repository.
        /// </summary>
        public IDbConnectionFactory DbConnectionFactory
        {
            get { return factory; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="keygenerator"></param>
        /// <param name="aalarmrepository"></param>
        /// <param name="dalarmrepository"></param>
        /// <param name="ealarmrepository"></param>
        /// <param name="factory"></param>
        public EventOrmLiteRepository(
            IKeyGenerator<Guid> keygenerator,
            IAudioAlarmRepository aalarmrepository,
            IDisplayAlarmRepository dalarmrepository,
            IEmailAlarmRepository ealarmrepository,
            IDbConnectionFactory factory)
        {
            if (keygenerator == null) throw new ArgumentNullException("keygenerator");
            if (aalarmrepository == null) throw new ArgumentNullException("aalarmrepository");
            if (dalarmrepository == null) throw new ArgumentNullException("dalarmrepository");
            if (ealarmrepository == null) throw new ArgumentNullException("ealarmrepository");
            if (factory == null) throw new ArgumentNullException("factory");

            this.keygenerator = keygenerator;
            this.aalarmrepository = aalarmrepository;
            this.dalarmrepository = dalarmrepository;
            this.ealarmrepository = ealarmrepository;
            this.factory = factory;
        }

        public void Dispose()
        {
            if (conn != null) conn.Dispose();
        }

        public VEVENT Find(Guid key)
        {
            var dry = db.Select<VEVENT>(q => q.Id == key).FirstOrDefault();
            return dry != null ? Hydrate(dry) : null;
        }

        public IEnumerable<VEVENT> FindAll(IEnumerable<Guid> keys, int? skip = null, int? take = null)
        {
            var dry = db.Select<VEVENT>(q => Sql.In(q.Id, keys.ToArray()), skip, take);
            return !dry.NullOrEmpty() ? HydrateAll(dry) : dry;
        }

        public IEnumerable<VEVENT> Get(int? skip = null, int? take = null)
        {
            var dry = db.Select<VEVENT>(skip, take);
            return !dry.NullOrEmpty() ? HydrateAll(dry) : dry;
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

                db.MergeAll(rorg.ToSingleton(), ororgs);
            }
            else db.RemoveAll(ororgs);

            if (recur != null)
            {
                db.Save(recur);
                var rrecur = new REL_EVENTS_RECURS
                {
                    Id = keygenerator.GetNext(),
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
                    Id = keygenerator.GetNext(),
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
                    Id = keygenerator.GetNext(),
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

            #endregion save event and its attributes
        }

        public void Patch(VEVENT source, Expression<Func<VEVENT, object>> fields, IEnumerable<Guid> keys = null)
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
            var selectionlist = selection as IList<string> ?? selection.ToList();
            var srelation = relations.GetMemberNames().Intersect(selectionlist, StringComparer.OrdinalIgnoreCase);

            //5. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selectionlist, StringComparer.OrdinalIgnoreCase);

            #endregion construct anonymous fields using expression lambdas

            var okeys = (keys != null)
                ? db.SelectParam<VEVENT, Guid>(q => q.Id, p => Sql.In(p.Id, keys.ToArray())).ToArray()
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

                if (selectionlist.Contains(orgexpr.GetMemberName()))
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
                        db.MergeAll(rorgs, ororgs);
                    }
                    else db.RemoveAll(ororgs);
                }

                if (selectionlist.Contains(recurexpr.GetMemberName()))
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
                        db.MergeAll(rrecurs, orrecurs);
                    }
                    else db.RemoveAll(orrecurs);
                }

                if (selectionlist.Contains(attendsexpr.GetMemberName()))
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
                        db.MergeAll(rattendees, orattendees);
                    }
                    else db.RemoveAll(orattendees);
                }

                if (selectionlist.Contains(attachbinsexpr.GetMemberName()))
                {
                    var orattachbins = db.Select<REL_EVENTS_ATTACHBINS>(q => Sql.In(q.EventId, okeys));
                    if (!source.AttachmentUris.NullOrEmpty())
                    {
                        db.SaveAll(source.AttachmentUris.Distinct());
                        var rattachbins = okeys.SelectMany(x => source.AttachmentUris.Select(y => new REL_EVENTS_ATTACHBINS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = x,
                            AttachmentId = y.Id
                        }));

                        db.MergeAll(rattachbins, orattachbins);
                    }
                    else db.RemoveAll(orattachbins);
                }
                if (selectionlist.Contains(attachurisexpr.GetMemberName()))
                {
                    var orattachuris = db.Select<REL_EVENTS_ATTACHURIS>(q => Sql.In(q.EventId, okeys));
                    if (!source.AttachmentUris.NullOrEmpty())
                    {
                        db.SaveAll(source.AttachmentUris.Distinct());
                        var rattachuris = okeys.SelectMany(x => source.AttachmentUris.Select(y => new REL_EVENTS_ATTACHURIS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = x,
                            AttachmentId = y.Id
                        }));
                        db.MergeAll(rattachuris, orattachuris);
                    }
                    else db.RemoveAll(orattachuris);
                }

                if (selectionlist.Contains(contactsexpr.GetMemberName()))
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
                        db.MergeAll(rcontacts, orcontacts);
                    }
                    else db.RemoveAll(orcontacts);
                }

                if (selectionlist.Contains(commentsexpr.GetMemberName()))
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
                        db.MergeAll(rcomments, orcomments);
                    }
                    else db.RemoveAll(orcomments);
                }

                if (selectionlist.Contains(rdatesexpr.GetMemberName()))
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
                        db.MergeAll(rrdates, orrdates);
                    }
                    else db.RemoveAll(orrdates);
                }

                if (selectionlist.Contains(exdatesexpr.GetMemberName()))
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
                        db.MergeAll(rexdates, orexdates);
                    }
                    else db.RemoveAll(orexdates);
                }

                if (selectionlist.Contains(relatedtosexpr.GetMemberName()))
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

                        db.MergeAll(rrelatedtos, orrelatedtos);
                    }
                    else db.RemoveAll(orrelatedtos);
                }

                if (selectionlist.Contains(resourcesexpr.GetMemberName()))
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
                        db.MergeAll(rresources, orresources);
                    }
                    else db.RemoveAll(orresources);
                }

                if (selectionlist.Contains(reqstatsexpr.GetMemberName()))
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
                        db.MergeAll(rreqstats, orreqstats);
                    }
                    else db.RemoveAll(orreqstats);
                }

                if (selectionlist.Contains(aalarmexpr.GetMemberName()))
                {
                    var oraalarms = db.Select<REL_EVENTS_AUDIO_ALARMS>(q => Sql.In(q.EventId, okeys));
                    if (!source.AudioAlarms.NullOrEmpty())
                    {
                        aalarmrepository.SaveAll(source.AudioAlarms.Distinct());
                        var raalarms = okeys.SelectMany(x => source.AudioAlarms.Select(y => new REL_EVENTS_AUDIO_ALARMS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = x,
                            AlarmId = y.Id
                        }));
                        db.MergeAll(raalarms, oraalarms);
                    }
                    else db.RemoveAll(oraalarms);
                }

                if (selectionlist.Contains(dalarmexpr.GetMemberName()))
                {
                    var ordalarms = db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => Sql.In(q.EventId, okeys));
                    if (!source.DisplayAlarms.NullOrEmpty())
                    {
                        dalarmrepository.SaveAll(source.DisplayAlarms.Distinct());
                        var rdalarms = okeys.SelectMany(x => source.DisplayAlarms.Select(y => new REL_EVENTS_DISPLAY_ALARMS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = x,
                            AlarmId = y.Id
                        }));
                        db.MergeAll(rdalarms, ordalarms);
                    }
                    else db.RemoveAll(ordalarms);
                }

                if (selectionlist.Contains(ealarmexpr.GetMemberName()))
                {
                    var orealarms = db.Select<REL_EVENTS_EMAIL_ALARMS>(q => Sql.In(q.EventId, okeys));
                    if (!source.EmailAlarms.NullOrEmpty())
                    {
                        ealarmrepository.SaveAll(source.EmailAlarms.Distinct());
                        var realarms = okeys.SelectMany(x => source.EmailAlarms.Select(y => new REL_EVENTS_EMAIL_ALARMS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = x,
                            AlarmId = y.Id
                        }));

                        db.MergeAll(realarms, orealarms);
                    }
                    else db.RemoveAll(orealarms);
                }

                #endregion save relational attributes of entities
            }

            var sprimitivelist = sprimitives as IList<string> ?? sprimitives.ToList();
            if (!sprimitivelist.NullOrEmpty())
            {
                var patchstr = string.Format("f => new {{ {0} }}", string.Join(", ", sprimitivelist.Select(x => string.Format("f.{0}", x))));

                var patchexpr = patchstr.CompileToExpressionFunc<VEVENT, object>(
                    CodeDomLanguage.csharp,
                    "System.dll", "System.Core.dll",
                    typeof(VEVENT).Assembly.Location,
                    typeof(IContainsKey<Guid>).Assembly.Location);

                if (!okeys.NullOrEmpty()) db.UpdateOnly(source, patchexpr, q => Sql.In(q.Id, okeys.ToArray()));
                else db.UpdateOnly(source, patchexpr);
            }
        }

        public void Erase(Guid key)
        {
            db.Delete<VEVENT>(q => q.Id == key);
        }

        public void SaveAll(IEnumerable<VEVENT> entities)
        {
            #region 1. retrieve attributes of entities

            var lentities = entities as IList<VEVENT> ?? entities.ToList();
            var organizers = lentities.Where(x => x.Organizer != null).Select(x => x.Organizer).ToList();
            var recurs = lentities.Where(x => x.RecurrenceRule != null).Select(x => x.RecurrenceRule).ToList();
            var attendees = lentities.Where(x => !x.Attendees.NullOrEmpty()).SelectMany(x => x.Attendees).ToList();
            var attachbins = lentities.Where(x => !x.AttachmentBinaries.NullOrEmpty()).SelectMany(x => x.AttachmentBinaries).ToList();
            var attachuris = lentities.Where(x => !x.AttachmentUris.NullOrEmpty()).SelectMany(x => x.AttachmentUris).ToList();
            var contacts = lentities.Where(x => !x.Contacts.NullOrEmpty()).SelectMany(x => x.Contacts).ToList();
            var comments = lentities.Where(x => !x.Comments.NullOrEmpty()).SelectMany(x => x.Comments).ToList();
            var rdates = lentities.Where(x => !x.RecurrenceDates.NullOrEmpty()).SelectMany(x => x.RecurrenceDates).ToList();
            var exdates = lentities.Where(x => !x.ExceptionDates.NullOrEmpty()).SelectMany(x => x.ExceptionDates).ToList();
            var relateds = lentities.Where(x => !x.RelatedTos.NullOrEmpty()).SelectMany(x => x.RelatedTos).ToList();
            var resources = lentities.Where(x => !x.Resources.NullOrEmpty()).SelectMany(x => x.Resources).ToList();
            var reqstats = lentities.Where(x => !x.Resources.NullOrEmpty()).SelectMany(x => x.RequestStatuses).ToList();
            var aalarms = lentities.Where(x => !x.AudioAlarms.NullOrEmpty()).SelectMany(x => x.AudioAlarms).ToList();
            var dalarms = lentities.Where(x => !x.DisplayAlarms.NullOrEmpty()).SelectMany(x => x.DisplayAlarms).ToList();
            var ealarms = lentities.Where(x => !x.EmailAlarms.NullOrEmpty()).SelectMany(x => x.EmailAlarms).ToList();

            #endregion 1. retrieve attributes of entities

            #region 2. save attributes of entities

            var keys = lentities.Select(x => x.Id).ToArray();
            db.SaveAll(lentities);

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
            var oraalarms = db.Select<REL_EVENTS_AUDIO_ALARMS>(q => Sql.In(q.EventId, lentities.Select(x => x.Id)));
            var ordalarms = db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => Sql.In(q.EventId, lentities.Select(x => x.Id)));
            var orealarms = db.Select<REL_EVENTS_EMAIL_ALARMS>(q => Sql.In(q.EventId, lentities.Select(x => x.Id)));

            if (!organizers.NullOrEmpty())
            {
                db.SaveAll(organizers.Distinct());
                var rorgs = lentities.Where(x => x.Organizer != null)
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
                var rrecurs = lentities.Where(x => x.RecurrenceRule != null)
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
                var rattendees = lentities.Where(x => !x.Attendees.NullOrEmpty())
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

            if (!attachbins.NullOrEmpty())
            {
                db.SaveAll(attachbins.Distinct());

                var rattachbins = lentities.Where(x => !x.AttachmentBinaries.NullOrEmpty())
                    .SelectMany(e => e.AttachmentBinaries
                        .Select(x => new REL_EVENTS_ATTACHBINS
                        {
                            Id = keygenerator.GetNext(),
                            EventId = e.Id,
                            AttachmentId = x.Id
                        })).ToList();

                db.MergeAll(rattachbins, orattachbins);
            }
            else db.RemoveAll(orattachbins);

            if (!attachuris.NullOrEmpty())
            {
                db.SaveAll(attachuris.Distinct());
                var rattachuris = lentities.Where(x => !x.AttachmentBinaries.OfType<ATTACH_URI>().NullOrEmpty())
                    .SelectMany(e => e.AttachmentBinaries.OfType<ATTACH_URI>()
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
                var rcontacts = lentities.Where(x => !x.Contacts.NullOrEmpty())
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
                var rcomments = lentities.Where(x => !x.Comments.NullOrEmpty())
                    .SelectMany(e => e.Contacts.Select(x => new REL_EVENTS_COMMENTS
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
                var rrdates = lentities.Where(x => !x.RecurrenceDates.NullOrEmpty())
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
                var rexdates = lentities.Where(x => !x.ExceptionDates.NullOrEmpty())
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
                var rrelateds = lentities.Where(x => !x.RelatedTos.NullOrEmpty())
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
                var rresources = lentities.Where(x => !x.Resources.NullOrEmpty())
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
                var rreqstats = lentities.Where(x => !x.RequestStatuses.NullOrEmpty())
                    .SelectMany(e => e.RequestStatuses.Select(x => new REL_EVENTS_REQSTATS
                    {
                        Id = keygenerator.GetNext(),
                        EventId = e.Id,
                        ReqStatsId = x.Id
                    })).ToList();
                db.MergeAll(rreqstats, orreqstats);
            }
            else db.RemoveAll(orreqstats);

            if (!aalarms.NullOrEmpty())
            {
                aalarmrepository.SaveAll(aalarms.Distinct());
                var raalarms = lentities.Where(x => !x.AudioAlarms.NullOrEmpty())
                    .SelectMany(e => e.AudioAlarms.Select(x => new REL_EVENTS_AUDIO_ALARMS
                    {
                        Id = keygenerator.GetNext(),
                        EventId = e.Id,
                        AlarmId = x.Id
                    })).ToList();
                db.MergeAll(raalarms, oraalarms);
            }
            else db.RemoveAll(oraalarms);

            if (!dalarms.NullOrEmpty())
            {
                dalarmrepository.SaveAll(dalarms.Distinct());
                var rdalarms = lentities.Where(x => !x.AudioAlarms.NullOrEmpty()).SelectMany(e => e.DisplayAlarms.Select(x => new REL_EVENTS_DISPLAY_ALARMS
                {
                    Id = keygenerator.GetNext(),
                    EventId = e.Id,
                    AlarmId = x.Id
                })).ToList();

                db.MergeAll(rdalarms, ordalarms);
            }
            else db.RemoveAll(ordalarms);

            if (!ealarms.NullOrEmpty())
            {
                ealarmrepository.SaveAll(ealarms.Distinct());
                var realarms = lentities.Where(x => !x.EmailAlarms.NullOrEmpty())
                    .SelectMany(e => e.EmailAlarms.Select(x => new REL_EVENTS_EMAIL_ALARMS
                    {
                        Id = keygenerator.GetNext(),
                        EventId = e.Id,
                        AlarmId = x.Id
                    })).ToList();

                db.MergeAll(realarms, orealarms);
            }
            else db.RemoveAll(orealarms);

            #endregion 2. save attributes of entities
        }

        public void EraseAll(IEnumerable<Guid> keys = null)
        {
            if (!keys.NullOrEmpty()) db.Delete<VEVENT>(q => Sql.In(q.Id, keys.ToArray()));
            else db.DeleteAll<VEVENT>();
        }

        public VEVENT Hydrate(VEVENT dry)
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
            if (!attachbins.NullOrEmpty()) dry.AttachmentBinaries.MergeRange(attachbins);

            var attachuris = db.Select<ATTACH_URI, VEVENT, REL_EVENTS_ATTACHURIS>(
                r => r.AttachmentId,
                r => r.EventId,
                e => e.Id == dry.Id);
            if (!attachuris.NullOrEmpty()) dry.AttachmentUris.MergeRange(attachuris);

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
                dry.AudioAlarms.MergeRange(aalarmrepository.FindAll(raalarms.Select(x => x.AlarmId).ToList()));
            }

            var rdalarms = db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => q.EventId == dry.Id);
            if (!rdalarms.NullOrEmpty())
            {
                dry.DisplayAlarms.MergeRange(dalarmrepository.FindAll(rdalarms.Select(x => x.AlarmId).ToList()));
            }

            var realarms = db.Select<REL_EVENTS_EMAIL_ALARMS>(q => q.EventId == dry.Id);
            if (!realarms.NullOrEmpty())
            {
                dry.EmailAlarms.MergeRange(ealarmrepository.FindAll(realarms.Select(x => x.AlarmId).ToList()));
            }

            return dry;
        }

        public IEnumerable<VEVENT> HydrateAll(IEnumerable<VEVENT> dry)
        {
            var events = dry.ToList();
            var keys = events.Select(q => q.Id).ToArray();
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

                var orgs = (!rorgs.SafeEmpty()) ? db.Select<ORGANIZER>(q => Sql.In(q.Id, rorgs.Select(r => r.OrganizerId).ToList())) : null;
                var recurs = (!rrecurs.SafeEmpty()) ? db.Select<RECUR>(q => Sql.In(q.Id, rrecurs.Select(r => r.RecurId).ToList())) : null;
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
                var aalarms = (!raalarms.SafeEmpty()) ? aalarmrepository.FindAll(raalarms.Select(x => x.AlarmId).ToList()) : null;
                var dalarms = (!rdalarms.SafeEmpty()) ? dalarmrepository.FindAll(rdalarms.Select(x => x.AlarmId).ToList()) : null;
                var ealarms = (!realarms.SafeEmpty()) ? ealarmrepository.FindAll(realarms.Select(x => x.AlarmId).ToList()) : null;

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
                        if (!xattachbins.NullOrEmpty()) x.AttachmentBinaries.MergeRange(xattachbins);
                    }

                    if (!attachuris.NullOrEmpty())
                    {
                        var xattachuris = from y in attachuris
                                          join r in rattachuris on y.Id equals r.AttachmentId
                                          join e in events on r.EventId equals e.Id
                                          where e.Id == x.Id
                                          select y;
                        if (!xattachuris.NullOrEmpty()) x.AttachmentUris.MergeRange(xattachuris);
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
                        if (!xraalarms.NullOrEmpty()) x.AudioAlarms.MergeRange(xraalarms);
                    }

                    if (!dalarms.NullOrEmpty())
                    {
                        var xrdalarms = from y in dalarms
                                        join r in rdalarms on y.Id equals r.AlarmId
                                        join e in events on r.EventId equals e.Id
                                        where e.Id == x.Id
                                        select y;
                        if (!xrdalarms.NullOrEmpty()) x.DisplayAlarms.MergeRange(xrdalarms);
                    }

                    if (!ealarms.NullOrEmpty())
                    {
                        var xrealarms = from y in ealarms
                                        join r in realarms on y.Id equals r.AlarmId
                                        join e in events on r.EventId equals e.Id
                                        where e.Id == x.Id
                                        select y;
                        if (!xrealarms.NullOrEmpty()) x.EmailAlarms.MergeRange(xrealarms);
                    }
                });

                #endregion 3. Use Linq to stitch secondary entities to primary entities
            }

            return events;
        }

        public bool ContainsKey(Guid key)
        {
            return db.Count<VEVENT>(q => q.Id == key) != 0;
        }

        public bool ContainsKeys(IEnumerable<Guid> keys, ExpectationMode mode = ExpectationMode.Optimistic)
        {
            try
            {
                var dkeys = keys.Distinct();
                if (mode == ExpectationMode.Pessimistic || mode == ExpectationMode.Unknown)
                    return db.Count<VEVENT>(q => Sql.In(q.Id, dkeys)) == dkeys.Count();
                else return db.Count<VEVENT>(q => Sql.In(q.Id, dkeys)) != 0;
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        public IEnumerable<Guid> GetKeys(int? skip = null, int? take = null)
        {
            return db.SelectParam<VEVENT>(q => q.Id, skip, take);
        }

        public IEnumerable<VEVENT> DehydrateAll(IEnumerable<VEVENT> full)
        {
            try
            {
                var pquery = full.AsParallel();
                pquery.ForAll(x => Dehydrate(x));
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