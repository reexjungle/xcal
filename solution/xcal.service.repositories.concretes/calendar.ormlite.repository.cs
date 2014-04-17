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

namespace reexmonkey.xcal.service.repositories.concretes
{
    public class CalendarOrmLiteRepository: ICalendarOrmLiteRepository
    {
        private IDbConnection conn = null;
        private IDbConnectionFactory factory = null;
        private int? take = null;
        private IKeyGenerator<string> keygen;
        private IEventRepository eventrepository;

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
        
        public CalendarOrmLiteRepository(IDbConnectionFactory factory, int? take)
        {
            this.DbConnectionFactory = factory;
            this.Take = take;
            this.conn = this.factory.OpenDbConnection();
        }
        
        public CalendarOrmLiteRepository(IDbConnection connection, int? take)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            this.conn = connection; 
            this.Take = take;
        }

        public VCALENDAR Hydrate(VCALENDAR dry)
        {
            VCALENDAR full = null;
            try
            {
                full = db.Select<VCALENDAR>(q => q.Id == dry.Id).FirstOrDefault();
                if (full != null)
                {
                    var revents = this.db.Select<REL_CALENDARS_EVENTS>(q => q.CalendarId == full.Id);
                    if (!revents.NullOrEmpty())
                    {
                        var events = this.EventRepository.Find(revents.Select(x => x.EventId).ToList());
                        full.Components.AddRangeComplement(this.EventRepository.Hydrate(events));
                    }
                }

            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }

            return full ?? dry;
        }

        public IEnumerable<VCALENDAR> Hydrate(IEnumerable<VCALENDAR> dry)
        {
            IEnumerable<VCALENDAR> full = null;
            var keys = dry.Select(x => x.Id).Distinct().ToList();
            full = db.Select<VCALENDAR>(q => Sql.In(q.Id, keys));
            if (!full.NullOrEmpty())
            {
                var revents = this.db.Select<REL_CALENDARS_EVENTS>(q => Sql.In(q.CalendarId, keys));
                if (!revents.NullOrEmpty())
                {
                    var events = this.EventRepository.Hydrate(this.EventRepository.Find(revents.Select(x => x.EventId))).ToList();
                    full.Select(x =>
                    {
                        var xevents = from y in events
                                      join r in revents on y.Id equals r.EventId
                                      join c in full on r.CalendarId equals c.Id
                                      where c.Id == x.Id
                                      select y;
                        if (!xevents.NullOrEmpty()) x.Components.AddRangeComplement(xevents);
                        return x;
                    });
                }
            }
            return full ?? dry;
        }

        public VCALENDAR Find(string key)
        {
            try
            {
                return db.Select<VCALENDAR>(q => q.Id == key).FirstOrDefault();
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public IEnumerable<VCALENDAR> Find(IEnumerable<string> keys, int? skip = null)
        {
            try
            {
                return db.Select<VCALENDAR>(q => Sql.In(q.Id, keys.ToArray()), skip.Value, take.Value);
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public IEnumerable<VCALENDAR> Get(int? skip = null)
        {
            IEnumerable<VCALENDAR> dry = null;
            try
            {
                dry = db.Select<VCALENDAR>(skip, take);
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }

            return dry;
        }

        public void Save(VCALENDAR entity)
        {

            using (var transaction = db.BeginTransaction())
            {
                try
                {
                    db.Save<VCALENDAR>(entity, transaction);
                    var events = entity.Components.OfType<VEVENT>();
                    if (!events.NullOrEmpty())
                    {
                        this.EventRepository.SaveAll(events);
                        var revents = events.Select(x => new REL_CALENDARS_EVENTS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            CalendarId = entity.Id,
                            EventId = x.Id
                        });
                        var orevents = db.Select<REL_CALENDARS_EVENTS>(q => q.CalendarId == entity.Id);
                        db.SaveAll(!orevents.NullOrEmpty()
                            ? revents.Except(orevents) 
                            : revents, transaction);
                    }

                    transaction.Commit();
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

        }

        public void Patch(VCALENDAR source, Expression<Func<VCALENDAR, object>> fields, Expression<Func<VCALENDAR, bool>> predicate = null)
        {
            #region construct anonymous fields using expression lambdas
            
            var selection = fields.GetMemberNames();

            Expression<Func<VCALENDAR, object>> primitives = x => new
            {
                x.Version,
                x.Method,
                x.Calscale
            };

            Expression<Func<VCALENDAR, object>> relations = x => new {  x.Components };

            //4. Get list of selected relationals
            var srelation = relations.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

            //5. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

            #endregion

            using (var transaction = db.BeginTransaction())
            {

                try
                {
                    var keys = (predicate != null)
                        ? db.SelectParam<VCALENDAR>(q => q.Id, predicate).ToArray()
                        : db.SelectParam<VCALENDAR>(q => q.Id).ToArray();

                    if (!srelation.NullOrEmpty())
                    {
                        Expression<Func<VCALENDAR, object>> componentsexpr = y => y.Components;

                        #region save relational aggregate attributes of entities

                        if (selection.Contains(componentsexpr.GetMemberName()))
                        {
                            var events = source.Components.OfType<VEVENT>();
                            if (!events.NullOrEmpty()) this.EventRepository.SaveAll(events);


                            if (!events.NullOrEmpty())
                            {
                                var revents = keys.SelectMany(x => events.Select(y => new REL_CALENDARS_EVENTS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    CalendarId = x,
                                    EventId = y.Id
                                }));
                                var orevents = db.Select<REL_CALENDARS_EVENTS>(q => Sql.In(q.EventId, keys));
                                db.SaveAll((!orevents.NullOrEmpty()) ? revents.Except(orevents) : revents, transaction);
                            }


                        }

                        #endregion

                    }

                    if (!sprimitives.NullOrEmpty())
                    {
                        #region update-only non-relational attributes

                        var patchstr = string.Format("f => new {{ {0} }}", string.Join(", ", sprimitives.Select(x => string.Format("f.{0}", x))));
                        var patchexpr = patchstr.CompileToExpressionFunc<VCALENDAR, object>(CodeDomLanguage.csharp, Utilities.GetReferencedAssemblyNamesFromEntryAssembly());
                        db.UpdateOnly<VCALENDAR, object>(source, patchexpr, predicate);

                        #endregion

                    }
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
                db.Delete<VCALENDAR>(q => q.Id == key);
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public void SaveAll(IEnumerable<VCALENDAR> entities)
        {

            var events = entities.Where(x => !x.Components.NullOrEmpty() && !x.Components.OfType<VEVENT>().NullOrEmpty())
                .SelectMany(x => x.Components.OfType<VEVENT>());

            using (var transaction = db.OpenTransaction())
            {
                try
                {
                    if (!events.NullOrEmpty())
                    {
                        var keys = entities.Select(x => x.Id).ToArray();
                        this.EventRepository.SaveAll(events);
                        var revents = entities.Where(x => !x.Components.OfType<VEVENT>().NullOrEmpty())
                            .SelectMany(c => c.Components.OfType<VEVENT>().Select(x => new REL_CALENDARS_EVENTS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                CalendarId = c.Id,
                                EventId = x.Id
                            }));
                        var orevents = db.Select<REL_CALENDARS_EVENTS>(q => Sql.In(q.EventId, keys));
                        db.SaveAll((!orevents.NullOrEmpty()) 
                            ? revents.Except(orevents) 
                            : revents, transaction);
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
        }

        public void EraseAll(IEnumerable<string> keys = null)
        {
            try
            {
                if (!keys.NullOrEmpty()) db.Delete<VCALENDAR>(q => Sql.In(q.Id, keys.ToArray()));
                else db.DeleteAll<VCALENDAR>();
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public bool ContainsKey(string key)
        {
            try
            {
                return db.Count<VCALENDAR>(q => q.Id == key) != 0;
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
                    return db.Count<VCALENDAR>(q => Sql.In(q.Id, dkeys)) == dkeys.Count();
                else return db.Count<VCALENDAR>(q => Sql.In(q.Id, dkeys)) != 0;
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public IEnumerable<VCALENDAR> Dehydrate(IEnumerable<VCALENDAR> full)
        {
            var dry = full.Select(x => { return this.Dehydrate(x); });
            return dry;
        }

        public VCALENDAR Dehydrate(VCALENDAR full)
        {
            var dry = full;
            if (!dry.Components.NullOrEmpty()) dry.Components.Clear();
            return dry;
        }

        public IEnumerable<string> GetKeys(int? skip = null)
        {
            IEnumerable<string> keys = null;
            try
            {
                keys = db.SelectParam<VCALENDAR>(q => q.Id, skip, take);
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
            return keys;
        }
    }
}
