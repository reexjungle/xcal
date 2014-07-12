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
using reexmonkey.infrastructure.operations.contracts;
using reexmonkey.infrastructure.operations.concretes;
using reexmonkey.xcal.service.repositories.concretes.relations;

namespace reexmonkey.xcal.service.repositories.concretes.ormlite
{
    public class CalendarOrmLiteRepository: ICalendarOrmLiteRepository, IDisposable
    {
        private IDbConnection conn = null;
        private IDbConnectionFactory factory = null;
        private IKeyGenerator<string> keygen;
        private IEventRepository eventrepository;

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

        public IEventRepository EventRepository
        {
            get { return this.eventrepository; }
            set
            {
                if (value == null) throw new ArgumentNullException("EventRepository");
                this.eventrepository = value;
            }
        }

        public CalendarOrmLiteRepository() { }
        
        public CalendarOrmLiteRepository(IDbConnectionFactory factory)
        {
            this.DbConnectionFactory = factory;
            this.conn = this.factory.OpenDbConnection();
        }
        
        public CalendarOrmLiteRepository(IDbConnection connection)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            this.conn = connection; 
        }

        public VCALENDAR Hydrate(VCALENDAR dry)
        {
            VCALENDAR full = dry;
            try
            {
                var okey = db.SelectParam<VCALENDAR, string>(q => q.Id, p => p.Id == dry.Id).FirstOrDefault();
                if (!string.IsNullOrEmpty(okey))
                {
                    var revents = this.db.Select<REL_CALENDARS_EVENTS>(q => q.CalendarId == okey);
                    if (!revents.NullOrEmpty())
                    {
                        var events = this.EventRepository.FindAll(revents.Select(x => x.EventId).ToList());
                        full.Events.AddRangeComplement(this.EventRepository.HydrateAll( events));
                    }
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }

            return full ?? dry;
        }

        public IEnumerable<VCALENDAR> HydrateAll(IEnumerable<VCALENDAR> dry)
        {
            var full = dry.ToList();
            try
            {
                var keys = full.Select(q => q.Id).ToArray();
                var okeys = db.SelectParam<VCALENDAR, string>(q => q.Id, p => Sql.In(p.Id, keys));
                if (!okeys.NullOrEmpty())
                {
                    var revents = this.db.Select<REL_CALENDARS_EVENTS>(q => Sql.In(q.CalendarId, okeys));
                    if (!revents.NullOrEmpty())
                    {
                        var events = this.EventRepository.FindAll(revents.Select(x => x.EventId)).ToList();
                        full.ForEach(x =>
                        {
                            var xevents = from y in events
                                          join r in revents on y.Id equals r.EventId
                                          join c in full on r.CalendarId equals c.Id
                                          where c.Id == x.Id
                                          select y;
                            if (!xevents.NullOrEmpty()) x.Events.AddRangeComplement(xevents);
                        });
                    }
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
            return full ?? dry;
        }

        public VCALENDAR Find(string key)
        {
            try
            {
                var dry = db.Select<VCALENDAR>(q => q.Id == key).FirstOrDefault();
                return dry != null ? this.Hydrate(dry) : dry;
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public IEnumerable<VCALENDAR> FindAll(IEnumerable<string> keys, int? skip = null, int? take = null)
        {
            try
            {
                IEnumerable<VCALENDAR> dry = null;
                if(skip == null && take == null) dry = db.Select<VCALENDAR>(q => Sql.In(q.Id, keys.ToArray()));
                else dry = db.Select<VCALENDAR>(q => Sql.In(q.Id, keys.ToArray()), skip.Value, take.Value);
                return !dry.NullOrEmpty() ? this.HydrateAll(dry) : dry;
            }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
            catch (Exception) { throw; }
        }

        public IEnumerable<VCALENDAR> Get(int? skip = null, int? take = null)
        {
            try
            {
                IEnumerable<VCALENDAR> dry = null;
                if(skip == null && take == null) dry = db.Select<VCALENDAR>();
                else dry = db.Select<VCALENDAR>(skip, take);
                return !dry.NullOrEmpty() ? this.HydrateAll(dry) : dry;
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        public void Save(VCALENDAR entity)
        {
            try
            {
                db.Save<VCALENDAR>(entity);
                if (!entity.Events.NullOrEmpty())
                {
                    this.EventRepository.SaveAll(entity.Events);
                    var revents = entity.Events.Select(x => new REL_CALENDARS_EVENTS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        CalendarId = entity.Id,
                        EventId = x.Id
                    });
                    var orevents = db.Select<REL_CALENDARS_EVENTS>(q => q.CalendarId == entity.Id);
                    db.SynchronizeAll(revents, orevents);
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }

        }

        public void Patch(VCALENDAR source, Expression<Func<VCALENDAR, object>> fields, IEnumerable<string> keys = null)
        {
            #region construct anonymous fields using expression lambdas
            
            var selection = fields.GetMemberNames().ToArray();

            Expression<Func<VCALENDAR, object>> primitives = x => new
            {
                x.Version,
                x.Method,
                x.Calscale
            };

            Expression<Func<VCALENDAR, object>> relations = x => new 
            { 
                Events = x.Events, 
                Todos = x.ToDos, 
                FreeBusies = x.FreeBusies, 
                Journals = x.Journals, 
                TimeZones = x.TimeZones, 
                IanaComponents = x.IanaComponents, 
                XComponents = x.XComponents 
            };

            //4. Get list of selected relationals
            var srelation = relations.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);

            //5. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);

            #endregion

            try
            {
                var okeys = (!keys.NullOrEmpty())
                    ? db.SelectParam<VCALENDAR, string>(q => q.Id, p => Sql.In(p.Id, keys.ToArray()))
                    : db.SelectParam<VCALENDAR>(q => q.Id);
                if (!srelation.NullOrEmpty())
                {
                    Expression<Func<VCALENDAR, object>> eventsexpr = y => y.Events;
                    if (selection.Contains(eventsexpr.GetMemberName()))
                    {
                        this.EventRepository.SaveAll(source.Events.Distinct());
                        if (!source.Events.NullOrEmpty())
                        {
                            var revents = okeys.SelectMany(x => source.Events.Select(y => new REL_CALENDARS_EVENTS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                CalendarId = x,
                                EventId = y.Id
                            }));
                            var orevents = db.Select<REL_CALENDARS_EVENTS>(q => Sql.In(q.CalendarId, okeys));
                            db.SynchronizeAll(revents, orevents);
                        }
                    }
                }

                if (!sprimitives.NullOrEmpty())
                {
                    var patchstr = string.Format("f => new {{ {0} }}", string.Join(", ", sprimitives.Select(x => string.Format("f.{0}", x))));

                    var patchexpr = patchstr.CompileToExpressionFunc<VCALENDAR, object>(CodeDomLanguage.csharp, new string[] { "System.dll", "System.Core.dll", typeof(VCALENDAR).Assembly.Location, typeof(IContainsKey<string>).Assembly.Location });

                    if (!okeys.NullOrEmpty()) db.UpdateOnly<VCALENDAR, object>(source, patchexpr, q => Sql.In(q.Id, okeys));
                    else db.UpdateOnly<VCALENDAR, object>(source, patchexpr);
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
                db.Delete<VCALENDAR>(q => q.Id == key);
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        public void SaveAll(IEnumerable<VCALENDAR> entities)
        {
            try
            {
                var keys = entities.Select(x => x.Id).ToArray();
                db.SaveAll(entities.Distinct());

                var events = entities.Where(x => !x.Events.NullOrEmpty()).SelectMany(x => x.Events);
                if (!events.NullOrEmpty())
                {
                    this.EventRepository.SaveAll(events.Distinct());
                    var revents = entities.Where(x => !x.Events.NullOrEmpty()).SelectMany(c => c.Events.Select(x => new REL_CALENDARS_EVENTS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        CalendarId = c.Id,
                        EventId = x.Id
                    }));
                    var orevents = db.Select<REL_CALENDARS_EVENTS>(q => Sql.In(q.CalendarId, keys));
                    db.SynchronizeAll(revents, orevents);
                }

            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }  
        }

        public void EraseAll(IEnumerable<string> keys = null)
        {
            try
            {
                if (!keys.NullOrEmpty()) db.Delete<VCALENDAR>(q => Sql.In(q.Id, keys.ToArray()));
                else db.DeleteAll<VCALENDAR>();
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        public bool ContainsKey(string key)
        {
            try
            {
                return db.Count<VCALENDAR>(q => q.Id == key) != 0;
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        public bool ContainsKeys(IEnumerable<string> keys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            try
            {
                var dkeys = keys.Distinct().ToArray();
                if (mode == ExpectationMode.pessimistic || mode == ExpectationMode.unknown)
                    return db.Count<VCALENDAR>(q => Sql.In(q.Id, dkeys)) == dkeys.Count();
                else return db.Count<VCALENDAR>(q => Sql.In(q.Id, dkeys)) != 0;
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        public IEnumerable<VCALENDAR> DehydrateAll(IEnumerable<VCALENDAR> full)
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

        public VCALENDAR Dehydrate(VCALENDAR full)
        {
            try
            {
                var dry = full;
                if (!dry.Events.NullOrEmpty()) dry.Events.Clear();
                if (!dry.ToDos.NullOrEmpty()) dry.ToDos.Clear();
                if (!dry.Journals.NullOrEmpty()) dry.Journals.Clear();
                if (!dry.TimeZones.NullOrEmpty()) dry.TimeZones.Clear();
                if (!dry.IanaComponents.NullOrEmpty()) dry.IanaComponents.Clear();
                if (!dry.XComponents.NullOrEmpty()) dry.XComponents.Clear();
                return dry;
            }
            catch (ArgumentNullException) { throw; }
        }

        public IEnumerable<string> GetKeys(int? skip = null, int? take = null)
        {
            try
            {
                return db.SelectParam<VCALENDAR>(q => q.Id, skip, take);
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        public void Dispose()
        {
            if (this.conn != null) this.conn.Dispose();
        }
    }
}
