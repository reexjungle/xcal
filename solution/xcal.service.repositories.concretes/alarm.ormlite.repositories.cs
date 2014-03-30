using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using ServiceStack.OrmLite;
using reexmonkey.technical.data.contracts;
using reexmonkey.technical.data.concretes.extensions.ormlite;
using reexmonkey.crosscut.essentials.contracts;
using reexmonkey.crosscut.essentials.concretes;
using reexmonkey.crosscut.goodies.concretes;
using reexmonkey.crosscut.io.concretes;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.service.repositories.contracts;

namespace reexmonkey.xcal.service.repositories.concretes
{
    public class AudioAlarmOrmLiteRepository : IAudioAlarmOrmLiteRepository
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
            set
            {
                if (value == null) throw new ArgumentNullException("Null factory");
                this.factory = value;
            }
        }
        public int? Capacity
        {
            get { return this.pages; }
            set
            {
                if (value == null) throw new ArgumentNullException("Null pages");
                this.pages = value;
            }
        }
        public IKeyGenerator<string> KeyGenerator { get; set; }


        public AudioAlarmOrmLiteRepository() { }
        public AudioAlarmOrmLiteRepository(IDbConnectionFactory factory, int? pages)
        {
            this.DbConnectionFactory = factory;
            this.Capacity = pages;
            this.conn = this.factory.OpenDbConnection();
        }
        public AudioAlarmOrmLiteRepository(IDbConnection connection, int? pages)
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

        public AUDIO_ALARM Find(string fkey, string pkey)
        {
            AUDIO_ALARM dry = null;
            try
            {
                dry = db.Select<AUDIO_ALARM, VEVENT, REL_EVENTS_AUDIO_ALARMS>(
                    r => r.AlarmId,
                    a => a.Id == pkey,
                    r => r.EventId,
                    e => e.Uid == fkey).FirstOrDefault();
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
            return dry;
        }

        public IEnumerable<AUDIO_ALARM> Find(IEnumerable<string> fkeys, IEnumerable<string> pkeys = null, int? page = null)
        {
            IEnumerable<AUDIO_ALARM> dry = null;

            try
            {
                if (pkeys == null)
                {
                    dry = db.Select<AUDIO_ALARM, VEVENT, REL_EVENTS_AUDIO_ALARMS>(
                        r => r.AlarmId,
                        r => r.EventId,
                        e => Sql.In(e.Uid, fkeys.ToArray()));
                }
                else
                {
                    dry = db.Select<AUDIO_ALARM, VEVENT, REL_EVENTS_AUDIO_ALARMS>(
                        r => r.AlarmId,
                        a => Sql.In(a.Id, pkeys.ToArray()),
                        r => r.EventId,
                        e => Sql.In(e.Uid, fkeys.ToArray()));
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
            return dry;
        }

        public IEnumerable<AUDIO_ALARM> Get(int? page = null)
        {
            IEnumerable<AUDIO_ALARM> dry = null;
            try
            {
                dry = db.Select<AUDIO_ALARM>(page, pages);
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }

            return dry;
        }

        public void Save(AUDIO_ALARM entity)
        {
            using (var transaction = db.OpenTransaction())
            {
                try
                {
                    //Save dry event entity i.e. without related details
                    db.Save(entity, transaction);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    try { transaction.Rollback();}
                    catch (Exception) {throw;}
                } 
            }
        }

        public void Erase(string key)
        {
            try
            {
                db.Delete<AUDIO_ALARM>(q => q.Id.ToUpper() == key.ToUpper());
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public void SaveAll(IEnumerable<AUDIO_ALARM> entities)
        {
            using (var transaction = db.OpenTransaction())
            {
                try
                {
                    db.SaveAll(entities, transaction);
                    transaction.Commit();
                }
                catch (Exception) 
                {
                    try { transaction.Rollback();}
                    catch (Exception) {throw;}
                } 
            }
        }

        public void Patch(AUDIO_ALARM source, Expression<Func<AUDIO_ALARM, object>> fields, Expression<Func<AUDIO_ALARM, bool>> where = null)
        {
            //1. Get fields slected for patching
            var selection = fields.GetMemberNames();

            //2.Get list of all non-related event details (primitives)
            Expression<Func<AUDIO_ALARM, object>> primitives = x => new
            {
                x.Id,
                x.Action,
                x.Trigger,
                x.Duration,
                x.Attachment
            };

            //3. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

            //4. Update matching event primitives
            if (!sprimitives.NullOrEmpty())
            {
                var patchstr = string.Format("f => new {{ {0} }}", string.Join(", ", sprimitives.Select(x => string.Format("f.{0}", x))));
                var patchexpr = patchstr.CompileToExpressionFunc<AUDIO_ALARM, object>(CodeDomLanguage.csharp, Utilities.GetReferencedAssemblyNamesFromEntryAssembly());
                db.UpdateOnly<AUDIO_ALARM, object>(source, patchexpr, where);
            }
        }

        public void EraseAll(IEnumerable<string> keys = null)
        {
            try
            {
                if (!keys.NullOrEmpty()) db.Delete<AUDIO_ALARM>(q => Sql.In(q.Id, keys.ToArray()));
                else db.DeleteAll<AUDIO_ALARM>();
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public bool Contains(string fkey, string pkey)
        {
            try
            {
                return !db.Select<REL_EVENTS_AUDIO_ALARMS>(q => q.EventId == fkey && q.AlarmId == pkey).NullOrEmpty();
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public bool Contains(IEnumerable<string> fkeys, IEnumerable<string> pkeys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            var found = false;
            try
            {
                var rels = db.Select<REL_EVENTS_AUDIO_ALARMS>(q => Sql.In(q.EventId, fkeys.ToArray()) && Sql.In(q.AlarmId, pkeys.ToArray()));
                switch (mode)
                {
                    case ExpectationMode.pessimistic: found = (!rels.NullOrEmpty()) ?
                        rels.Select(x => x.AlarmId).Distinct().Count() == pkeys.Distinct().Count() :
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
    }

    public class DisplayAlarmOrmLiteRepository : IDisplayAlarmOrmLiteRepository
    {

        private IDbConnection conn;
        private IDbConnectionFactory factory = null;
        private int? pages = null;
        public IKeyGenerator<string> KeyGenerator { get; set; }

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
            get { return this.pages; }
            set
            {
                if (value == null) throw new ArgumentNullException("Null pages");
                this.pages = value;
            }
        }

        public DisplayAlarmOrmLiteRepository() { }
        public DisplayAlarmOrmLiteRepository(IDbConnectionFactory factory, int? pages)
        {
            this.DbConnectionFactory = factory;
            this.Capacity = pages;
            this.conn = this.factory.OpenDbConnection();
        }
        public DisplayAlarmOrmLiteRepository(IDbConnection connection, int? pages)
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

        public DISPLAY_ALARM Find(string fkey, string pkey)
        {
            DISPLAY_ALARM dry = null;
            try
            {
                dry = db.Select<DISPLAY_ALARM, VEVENT, REL_EVENTS_DISPLAY_ALARMS>(
                    r => r.AlarmId,
                    a => a.Id == pkey,
                    r => r.EventId,
                    e => e.Uid == fkey).FirstOrDefault();
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
            return dry;
        }

        public IEnumerable<DISPLAY_ALARM> Find(IEnumerable<string> fkeys, IEnumerable<string> pkeys = null, int? page = null)
        {
            IEnumerable<DISPLAY_ALARM> dry = null;

            try
            {
                if (pkeys == null)
                {
                    dry = db.Select<DISPLAY_ALARM, VEVENT, REL_EVENTS_DISPLAY_ALARMS>(
                        r => r.AlarmId,
                        r => r.EventId,
                        e => Sql.In(e.Uid, fkeys.ToArray()));
                }
                else
                {
                    dry = db.Select<DISPLAY_ALARM, VEVENT, REL_EVENTS_DISPLAY_ALARMS>(
                        r => r.AlarmId,
                        a => Sql.In(a.Id, pkeys.ToArray()),
                        r => r.EventId,
                        e => Sql.In(e.Uid, fkeys.ToArray()));
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
            return dry;
        }

        public IEnumerable<DISPLAY_ALARM> Get(int? page = null)
        {
            IEnumerable<DISPLAY_ALARM> dry = null;
            try
            {
                dry = db.Select<DISPLAY_ALARM>(page, pages);
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }

            return dry;
        }

        public void Save(DISPLAY_ALARM entity)
        {
            using (var transaction = db.OpenTransaction())
            {
                try
                {
                    //Save dry event entity i.e. without related details
                    db.Save(entity, transaction);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    try { transaction.Rollback(); }
                    catch (Exception) { throw; }
                }
            }
        }

        public void SaveAll(IEnumerable<DISPLAY_ALARM> entities)
        {
            using (var transaction = db.OpenTransaction())
            {
                try
                {
                    db.SaveAll(entities, transaction);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    try { transaction.Rollback(); }
                    catch (Exception) { throw; }
                }
            }
        }

        public void Patch(DISPLAY_ALARM source, Expression<Func<DISPLAY_ALARM, object>> fields, Expression<Func<DISPLAY_ALARM, bool>> where = null)
        {
            //1. Get fields slected for patching
            var selection = fields.GetMemberNames();

            //2.Get list of all non-related event details (primitives)
            Expression<Func<DISPLAY_ALARM, object>> primitives = x => new
            {
                x.Id,
                x.Action,
                x.Trigger,
                x.Duration,
                x.Description
            };

            //3. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

            //4. Update matching event primitives
            if (!sprimitives.NullOrEmpty())
            {
                var patchstr = string.Format("f => new {{ {0} }}", string.Join(", ", sprimitives.Select(x => string.Format("f.{0}", x))));
                var patchexpr = patchstr.CompileToExpressionFunc<DISPLAY_ALARM, object>(CodeDomLanguage.csharp, Utilities.GetReferencedAssemblyNamesFromEntryAssembly());
                db.UpdateOnly<DISPLAY_ALARM, object>(source, patchexpr, where);
            }
        }

        public void Erase(string key)
        {
            try
            {
                db.Delete<DISPLAY_ALARM>(q => q.Id.ToUpper() == key.ToUpper());
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public void EraseAll(IEnumerable<string> keys = null)
        {
            try
            {
                if (!keys.NullOrEmpty()) db.Delete<DISPLAY_ALARM>(q => Sql.In(q.Id, keys.ToArray()));
                else db.DeleteAll<DISPLAY_ALARM>();
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public bool Contains(string fkey, string pkey)
        {
            try
            {
                return !db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => q.EventId == fkey && q.AlarmId == pkey).NullOrEmpty();
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public bool Contains(IEnumerable<string> fkeys, IEnumerable<string> pkeys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            var found = false;
            try
            {
                var rels = db.Select<REL_EVENTS_DISPLAY_ALARMS>(q => Sql.In(q.EventId, fkeys.ToArray()) && Sql.In(q.AlarmId, pkeys.ToArray()));
                switch (mode)
                {
                    case ExpectationMode.pessimistic: found = (!rels.NullOrEmpty()) ?
                        rels.Select(x => x.AlarmId).Distinct().Count() == pkeys.Distinct().Count() :
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

    }

    public class EmailAlarmOrmLiteRepository: IEmailAlarmOrmLiteRepository
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
            set 
            {
                if (value == null) throw new ArgumentNullException("Null factory");
                this.factory = value; 
            }
        }
        public int? Capacity
        {
            get { return this.pages; }
            set 
            {
                if (value == null) throw new ArgumentNullException("Null pages");
                this.pages = value; 
            }
        }
        public IKeyGenerator<string> KeyGenerator { get; set; }

        public EmailAlarmOrmLiteRepository() { }
        public EmailAlarmOrmLiteRepository(IDbConnectionFactory factory, int? pages)
        {
            this.DbConnectionFactory = factory;
            this.Capacity = pages;
            this.conn = this.factory.OpenDbConnection();
        }
        public EmailAlarmOrmLiteRepository(IDbConnection connection, int? pages)
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

        public EMAIL_ALARM Hydrate(EMAIL_ALARM dry)
        {
            var full = dry;
            try
            {
                full.Attendees.AddRange(db.Select<ATTENDEE, EMAIL_ALARM, RELS_EALARMS_ATTENDEES>(
                    r => r.AttendeeId,
                    r => r.AlarmId,
                    a => a.Id == dry.Id).Except(dry.Attachments.OfType<ATTENDEE>()));

                full.Attachments.AddRange(db.Select<ATTACH_BINARY, EMAIL_ALARM, RELS_EALARMS_ATTACHBINS>(
                    r => r.AttachmentId,
                    r => r.AlarmId,
                    a => a.Id == dry.Id).Except(dry.Attachments.OfType<ATTACH_BINARY>()));

                full.Attachments.AddRange(db.Select<ATTACH_BINARY, EMAIL_ALARM, RELS_EALARMS_ATTACHURIS>(
                    r => r.AttachmentId,
                    r => r.AlarmId,
                    a => a.Id == dry.Id).Except(dry.Attachments.OfType<ATTACH_BINARY>()));
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }

            return full;
        }

        public IEnumerable<EMAIL_ALARM> Hydrate(IEnumerable<EMAIL_ALARM> dry)
        {
            IEnumerable<EMAIL_ALARM> full = null;
            try
            {
                //1. retrieve relationships
                var ids = dry.Select(q => q.Id).ToArray();
                var rattends = db.Select<RELS_EALARMS_ATTENDEES>(q => Sql.In(q.Id, ids));
                var rattachbins = db.Select<RELS_EALARMS_ATTACHBINS>(q => Sql.In(q.Id, ids));
                var rattachuris = db.Select<RELS_EALARMS_ATTACHURIS>(q => Sql.In(q.Id, ids));


                //2. retrieve secondary entities
                var attends = (!rattends.Empty()) ? db.Select<ATTENDEE>(q => Sql.In(q.Id, rattends.Select(r => r.AttendeeId).ToArray())) : null;
                var attachbins = (!rattachbins.Empty()) ? db.Select<ATTACH_BINARY>(q => Sql.In(q.Id, rattachbins.Select(r => r.AttachmentId).ToArray())) : null;
                var attachuris = (!rattachuris.Empty()) ? db.Select<ATTACH_URI>(q => Sql.In(q.Id, rattachuris.Select(r => r.AttachmentId).ToArray())) : null;

                //3. Use Linq to stitch secondary entities to primary entities
                full = dry.Select(x =>
                {
                    var xattachbins = (!attachbins.NullOrEmpty()) ? (from y in attachbins join r in rattachbins on y.Id equals r.AttachmentId join a in dry on r.AlarmId equals a.Id where a.Id == x.Id select y) : null;
                    var xattachuris = (!attachuris.NullOrEmpty()) ? (from y in attachuris join r in rattachuris on y.Id equals r.AttachmentId join a in dry on r.AlarmId equals a.Id where a.Id == x.Id select y) : null;

                    if (!xattachbins.NullOrEmpty()) x.Attachments.AddRange(xattachbins.Except(x.Attachments.OfType<ATTACH_BINARY>()));
                    if (!xattachuris.NullOrEmpty()) x.Attachments.AddRange(xattachuris.Except(x.Attachments.OfType<ATTACH_URI>()));

                    var xattends = (!attends.NullOrEmpty()) ? (from y in attends join r in rattends on y.Id equals r.AlarmId join a in dry on r.Id equals a.Id where a.Id == x.Id select y) : null;
                    if (!xattends.NullOrEmpty()) x.Attendees.AddRange(xattends.Except(x.Attendees.OfType<ATTENDEE>()));

                    return x;
                });
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }

            return full;
        }

        public EMAIL_ALARM Find(string fkey, string pkey )
        {
            EMAIL_ALARM dry = null;
            try
            {
                dry = db.Select<EMAIL_ALARM, VEVENT, REL_EVENTS_EMAIL_ALARMS>(
                    r => r.AlarmId,
                    a => a.Id == pkey,
                    r => r.EventId,
                    e => e.Uid == fkey).FirstOrDefault();
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
            return (dry != null) ? this.Hydrate(dry) : null;
        }

        public IEnumerable<EMAIL_ALARM> Find(IEnumerable<string> fkeys, IEnumerable<string> pkeys = null, int? page = null)
        {
            IEnumerable<EMAIL_ALARM> dry = null;

            try
            {
                if (pkeys == null)
                {
                    dry = db.Select<EMAIL_ALARM, VEVENT, REL_EVENTS_EMAIL_ALARMS>(
                        r => r.AlarmId,
                        r => r.EventId,
                        e => Sql.In(e.Uid, fkeys.ToArray()));
                }
                else
                {
                    dry = db.Select<EMAIL_ALARM, VEVENT, REL_EVENTS_EMAIL_ALARMS>(
                        r => r.AlarmId,
                        a => Sql.In(a.Id, pkeys.ToArray()),
                        r => r.EventId,
                        e => Sql.In(e.Uid, fkeys.ToArray()));
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
            return (dry != null) ? this.Hydrate(dry) : null;
        }

        public IEnumerable<EMAIL_ALARM> Get(int? page = null)
        {
            IEnumerable<EMAIL_ALARM> dry = null;
            try
            {
                dry = db.Select<EMAIL_ALARM>(page, pages);
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }

            return (!dry.NullOrEmpty()) ? this.Hydrate(dry) : null;
        }

        public void Save(EMAIL_ALARM entity)
        {
            using (var transaction = db.OpenTransaction())
            {
                try
                {
                    //Save dry event entity i.e. without related details
                    db.Save(entity, transaction);

                    //1. retrieve entity details
                    var attends = entity.Attendees.OfType<ATTENDEE>();
                    var attachbins = entity.Attachments.OfType<ATTACH_BINARY>();
                    var attachuris = entity.Attachments.OfType<ATTACH_URI>();

                    //2. save details
                    if (!attends.NullOrEmpty())
                    {
                        db.SaveAll(attends, transaction);
                        var rattends = attends.Select(x => new RELS_EALARMS_ATTENDEES { AlarmId = entity.Id, AttendeeId = x.Id });
                        var orattends = db.Select<RELS_EALARMS_ATTENDEES>(q => q.Id == entity.Id && Sql.In(q.AttendeeId, attends.Select(x => x.Id).ToArray()));
                        db.SaveAll(!orattends.NullOrEmpty() ? rattends.Except(orattends) : rattends, transaction);
                    }

                    if (!attachbins.NullOrEmpty())
                    {
                        db.SaveAll(attachbins, transaction);
                        var rattachbins = attachbins.Select(x => new RELS_EALARMS_ATTACHBINS { AlarmId = entity.Id, AttachmentId = x.Id });
                        var orattachbins = db.Select<RELS_EALARMS_ATTACHBINS>(q => q.Id == entity.Id && Sql.In(q.AttachmentId, attachbins.Select(x => x.Id).ToArray()));
                        db.SaveAll(!orattachbins.NullOrEmpty() ? rattachbins.Except(orattachbins) : rattachbins, transaction);
                    }

                    if (!attachuris.NullOrEmpty())
                    {
                        db.SaveAll(attachuris, transaction);
                        var rattachuris = attachuris.Select(x => new RELS_EALARMS_ATTACHURIS { AlarmId = entity.Id, AttachmentId = x.Id });
                        var orattachuris = db.Select<RELS_EALARMS_ATTACHURIS>(q => q.Id == entity.Id && Sql.In(q.AttachmentId, attachuris.Select(x => x.Id).ToArray()));
                        var rdiffs = rattachuris.Except(orattachuris);
                        db.SaveAll(!orattachuris.NullOrEmpty() ? rattachuris.Except(orattachuris) : rattachuris, transaction);
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

        public void Erase(string key)
        {
            try
            {
                db.Delete<EMAIL_ALARM>(q => q.Id.ToUpper() == key.ToUpper());
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public void SaveAll(IEnumerable<EMAIL_ALARM> entities)
        {
            using (var transaction = db.OpenTransaction())
            {
                try
                {
                    db.SaveAll(entities, transaction);

                    //1. retrieve details of events
                    var attends = entities.Where(x => x.Attendees.OfType<ATTENDEE>().Count() > 0).SelectMany(x => x.Attendees.OfType<ATTENDEE>());
                    var attachbins = entities.Where(x => x.Attachments.OfType<ATTACH_BINARY>().Count() > 0).SelectMany(x => x.Attachments.OfType<ATTACH_BINARY>());
                    var attachuris = entities.Where(x => x.Attachments.OfType<ATTACH_URI>().Count() > 0).SelectMany(x => x.Attachments.OfType<ATTACH_URI>());

                    //2. save details of events
                    if (!attends.NullOrEmpty())
                    {
                        db.SaveAll(attends, transaction);
                        var rattends = entities.Where(x => !x.Attendees.OfType<ATTENDEE>().NullOrEmpty())
                            .SelectMany(e => e.Attendees.OfType<ATTENDEE>().Select(x => new RELS_EALARMS_ATTENDEES { Id = e.Id, AttendeeId = x.Id }));
                        var orattends = db.Select<RELS_EALARMS_ATTENDEES>(q => Sql.In(q.Id, entities.Select(x => x.Id)) && Sql.In(q.AttendeeId, attends.Select(x => x.Id).ToArray()));
                        db.SaveAll(!orattends.NullOrEmpty() ? rattends.Except(orattends) : rattends, transaction);
                    }

                    if (!attachbins.NullOrEmpty())
                    {
                        db.SaveAll(attachbins, transaction);
                        var rattachbins = entities.Where(x => !x.Attachments.OfType<ATTACH_BINARY>().NullOrEmpty())
                            .SelectMany(e => e.Attachments.OfType<ATTACH_BINARY>().Select(x => new RELS_EALARMS_ATTACHBINS { Id = e.Id, AttachmentId = x.Id }));
                        var orattachbins = db.Select<RELS_EALARMS_ATTACHBINS>(q => Sql.In(q.Id, entities.Select(x => x.Id)) && Sql.In(q.AttachmentId, attachbins.Select(x => x.Id).ToArray()));
                        db.SaveAll(!orattachbins.NullOrEmpty() ? rattachbins.Except(orattachbins) : rattachbins, transaction);
                    }

                    if (!attachuris.NullOrEmpty())
                    {
                        db.SaveAll(attachuris, transaction);
                        var rattachuris = entities.Where(x => !x.Attachments.OfType<ATTACH_URI>().NullOrEmpty())
                            .SelectMany(e => e.Attachments.OfType<ATTACH_URI>().Select(x => new RELS_EALARMS_ATTACHURIS { Id = e.Id, AttachmentId = x.Id }));
                        var orattachuris = db.Select<RELS_EALARMS_ATTACHURIS>(q => Sql.In(q.Id, entities.Select(x => x.Id)) && Sql.In(q.AttachmentId, attachuris.Select(x => x.Id)));
                        db.SaveAll(!orattachuris.NullOrEmpty() ? rattachuris.Except(orattachuris) : rattachuris, transaction);

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

        public void EraseAll(IEnumerable<string> keys)
        {
            try
            {
                db.Delete<EMAIL_ALARM>(q => Sql.In(q.Id, keys.ToArray()));
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public void EraseAll()
        {
            try
            {
                db.DeleteAll<EMAIL_ALARM>();
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public void Patch(EMAIL_ALARM source, Expression<Func<EMAIL_ALARM, object>> fields, Expression<Func<EMAIL_ALARM, bool>> where = null)
        {
            //1. Get fields slected for patching
            var selection = fields.GetMemberNames();

            //2.Get list of all non-related event details (primitives)
            Expression<Func<EMAIL_ALARM, object>> primitives = x => new
            {
                x.Id,
                x.Action,
                x.Trigger,
                x.Duration,
                x.Description,
                x.Summary
            };


            //3.Get list of all related event details (relation)
            Expression<Func<EMAIL_ALARM, object>> relations = x => new
            {
                x.Attendees,
                x.Attachments
            };

            //4. Get list of selected relations
            var srelation = relations.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

            //5. Patch relations
            if (!srelation.NullOrEmpty())
            {
                Expression<Func<EMAIL_ALARM, object>> attendsexpr = y => y.Attendees;
                Expression<Func<EMAIL_ALARM, object>> attachsexpr = y => y.Attachments;

                var eventids = (where != null) 
                    ? db.SelectParam<EMAIL_ALARM>(q => q.Id, where).ToArray() 
                    : db.SelectParam<EMAIL_ALARM>(q => q.Id).ToArray();

                using (var transaction = db.OpenTransaction())
                {
                    try
                    {
                        bool skip = eventids.NullOrEmpty();
                        if (selection.Contains(attendsexpr.GetMemberName()))
                        {
                            var attends = source.Attendees.OfType<ATTENDEE>();
                            if (!attends.NullOrEmpty() && !skip)
                            {
                                db.SaveAll(attends, transaction);
                                var rattends = eventids.SelectMany(x => attends.Select(y => new RELS_EALARMS_ATTENDEES { Id = this.KeyGenerator.GetNextKey(), AlarmId = x, AttendeeId = y.Id }));
                                var orattends = db.Select<RELS_EALARMS_ATTENDEES>(q => Sql.In(q.Id, eventids) && Sql.In(q.AttendeeId, attends.Select(x => x.Id).ToArray()));
                                db.SaveAll(!rattends.NullOrEmpty() ? rattends.Except(orattends) : rattends, transaction);

                            }
                        }

                        if (selection.Contains(attachsexpr.GetMemberName()))
                        {
                            var attachbins = source.Attachments.OfType<ATTACH_BINARY>();
                            if (!attachbins.NullOrEmpty() && !skip)
                            {
                                db.SaveAll(attachbins, transaction);
                                var rattachbins = eventids.SelectMany(x => attachbins.Select(y => new RELS_EALARMS_ATTACHBINS { Id = this.KeyGenerator.GetNextKey(), AlarmId = x, AttachmentId = y.Id }));
                                var orattachbins = db.Select<RELS_EALARMS_ATTACHBINS>(q => Sql.In(q.Id, eventids) && Sql.In(q.AttachmentId, attachbins.Select(x => x.Id).ToArray()));
                                db.SaveAll(!rattachbins.NullOrEmpty() ? rattachbins.Except(orattachbins) : rattachbins, transaction);
                            }

                            var attachuris = source.Attachments.OfType<ATTACH_URI>();
                            if (!attachuris.NullOrEmpty() && !skip)
                            {
                                db.SaveAll(attachuris, transaction);
                                var rattachuris = eventids.SelectMany(x => attachuris.Select(y => new RELS_EALARMS_ATTACHURIS { Id = this.KeyGenerator.GetNextKey(), AlarmId = x, AttachmentId = y.Id }));
                                var orattachuris = db.Select<RELS_EALARMS_ATTACHURIS>(q => Sql.In(q.Id, eventids) && Sql.In(q.AttachmentId, attachuris.Select(x => x.Id).ToArray()));
                                db.SaveAll(!rattachuris.NullOrEmpty() ? rattachuris.Except(orattachuris) : rattachuris, transaction);
                            }
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


            //6. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

            //7. Update matching event primitives
            if (!sprimitives.NullOrEmpty())
            {
                try
                {
                    var patchstr = string.Format("f => new {{ {0} }}", string.Join(", ", sprimitives.Select(x => string.Format("f.{0}", x))));
                    var patchexpr = patchstr.CompileToExpressionFunc<EMAIL_ALARM, object>(CodeDomLanguage.csharp, Utilities.GetReferencedAssemblyNamesFromEntryAssembly());
                    db.UpdateOnly<EMAIL_ALARM, object>(source, patchexpr, where);
                }
                catch (NotImplementedException) { throw; }
                catch (System.Security.SecurityException) { throw; }
                catch (InvalidOperationException) { throw; }
                catch (Exception) { throw; }
            }
        }

        public bool Contains(string fkey, string pkey)
        {
            try
            {
                return !db.Select<REL_EVENTS_EMAIL_ALARMS>(q => q.EventId == fkey && q.AlarmId == pkey).NullOrEmpty();
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public bool Contains(IEnumerable<string> fkeys, IEnumerable<string> pkeys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            var found = false;
            try
            {
                var rels = db.Select<REL_EVENTS_EMAIL_ALARMS>(q => Sql.In(q.EventId, fkeys.ToArray()) && Sql.In(q.AlarmId, pkeys.ToArray()));
                switch (mode)
                {
                    case ExpectationMode.pessimistic: found = (!rels.NullOrEmpty()) ?
                        rels.Select(x => x.AlarmId).Distinct().Count() == pkeys.Distinct().Count() :
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

    }

}
