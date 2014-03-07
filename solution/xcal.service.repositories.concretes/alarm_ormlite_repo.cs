using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using ServiceStack.OrmLite;
using reexmonkey.technical.ormlite.extensions;
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
        public int? Pages
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
            this.Pages = pages;
            this.conn = this.factory.OpenDbConnection();
        }
        public AudioAlarmOrmLiteRepository(IDbConnection connection, int? pages)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            this.conn = connection;
            this.Pages = pages;
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
                    r => r.Uid,
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
                        r => r.Uid,
                        e => Sql.In(e.Uid, fkeys.ToArray()));
                }
                else
                {
                    dry = db.Select<AUDIO_ALARM, VEVENT, REL_EVENTS_AUDIO_ALARMS>(
                        r => r.AlarmId,
                        a => Sql.In(a.Id, pkeys.ToArray()),
                        r => r.Uid,
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
            try
            {
                //Save dry event entity i.e. without related details
                db.Save(entity);
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
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
            try
            {
                db.SaveAll(entities);
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
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
        public int? Pages
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
            this.Pages = pages;
            this.conn = this.factory.OpenDbConnection();
        }
        public DisplayAlarmOrmLiteRepository(IDbConnection connection, int? pages)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            this.conn = connection;
            this.Pages = pages;
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
                    r => r.Uid,
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
                        r => r.Uid,
                        e => Sql.In(e.Uid, fkeys.ToArray()));
                }
                else
                {
                    dry = db.Select<DISPLAY_ALARM, VEVENT, REL_EVENTS_DISPLAY_ALARMS>(
                        r => r.AlarmId,
                        a => Sql.In(a.Id, pkeys.ToArray()),
                        r => r.Uid,
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
            try
            {
                //Save dry event entity i.e. without related details
                db.Save(entity);
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        public void SaveAll(IEnumerable<DISPLAY_ALARM> entities)
        {
            try
            {
                db.SaveAll(entities);
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
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
        public int? Pages
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
            this.Pages = pages;
            this.conn = this.factory.OpenDbConnection();
        }
        public EmailAlarmOrmLiteRepository(IDbConnection connection, int? pages)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            this.conn = connection; 
            this.Pages = pages;
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
                full.Attendees.AddRange(db.Select<ATTENDEE, VALARM, RELS_EMAIL_ALARMS_ATTENDEES>(
                    r => r.AttendeeId,
                    r => r.AlarmId,
                    a => a.Id == dry.Id).Except(dry.Attachments.OfType<ATTENDEE>()));

                full.Attachments.AddRange(db.Select<ATTACH_BINARY, VALARM, RELS_EMAIL_ALARMS_ATTACHBINS>(
                    r => r.AttachmentId,
                    r => r.AlarmId,
                    a => a.Id == dry.Id).Except(dry.Attachments.OfType<ATTACH_BINARY>()));

                full.Attachments.AddRange(db.Select<ATTACH_BINARY, VALARM, RELS_EMAIL_ALARMS_ATTACHURIS>(
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
                var rattends = db.Select<RELS_EMAIL_ALARMS_ATTENDEES>(q => Sql.In(q.Id, ids));
                var rattachbins = db.Select<RELS_EMAIL_ALARMS_ATTACHBINS>(q => Sql.In(q.Id, ids));
                var rattachuris = db.Select<RELS_EMAIL_ALARMS_ATTACHURIS>(q => Sql.In(q.Id, ids));


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
                    r => r.Uid,
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
                        r => r.Uid,
                        e => Sql.In(e.Uid, fkeys.ToArray()));
                }
                else
                {
                    dry = db.Select<EMAIL_ALARM, VEVENT, REL_EVENTS_EMAIL_ALARMS>(
                        r => r.AlarmId,
                        a => Sql.In(a.Id, pkeys.ToArray()),
                        r => r.Uid,
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
            try
            {
                //Save dry event entity i.e. without related details
                db.Save(entity);

                //1. retrieve entity details
                var attends = (!entity.Attendees.OfType<ATTENDEE>().Empty()) ? entity.Attendees.OfType<ATTENDEE>() : null;
                var attachbins = (!entity.Attachments.OfType<ATTACH_BINARY>().Empty()) ? (entity.Attachments.OfType<ATTACH_BINARY>()) : null;
                var attachuris = (!entity.Attachments.OfType<ATTACH_URI>().Empty()) ? (entity.Attachments.OfType<ATTACH_URI>()) : null;

                //2. save details
                if (!attends.NullOrEmpty()) db.SaveAll(attends);
                if (!attachbins.NullOrEmpty()) db.SaveAll(attachbins);
                if (!attachuris.NullOrEmpty()) db.SaveAll(attachuris);

                //3. construct relations from entity details
                var rattends = (entity.Attendees.OfType<ATTENDEE>().Count() > 0) ? entity.Attendees.OfType<ATTENDEE>()
                    .Select(x => new RELS_EMAIL_ALARMS_ATTENDEES { AlarmId = entity.Id, AttendeeId = x.Id }) : null;
                var rattachbins = (entity.Attachments.OfType<ATTACH_BINARY>().Count() > 0) ?
                    (entity.Attachments.OfType<ATTACH_BINARY>().Select(x => new RELS_EMAIL_ALARMS_ATTACHBINS { AlarmId = entity.Id, AttachmentId = x.Id })) : null;
                var rattachuris = (entity.Attachments.OfType<ATTACH_URI>().Count() > 0) ?
                    (entity.Attachments.OfType<ATTACH_URI>().Select(x => new RELS_EMAIL_ALARMS_ATTACHURIS { AlarmId = entity.Id, AttachmentId = x.Id })) : null;

                //4. retrieve existing entity-details relations
                var orattends = (!attends.NullOrEmpty()) ?
                    db.Select<RELS_EMAIL_ALARMS_ATTENDEES>(q => q.Id == entity.Id && Sql.In(q.AttendeeId, attends.Select(x => x.Id).ToArray())) : null;
                var orattachbins = (!attachbins.NullOrEmpty()) ?
                    db.Select<RELS_EMAIL_ALARMS_ATTACHBINS>(q => q.Id == entity.Id && Sql.In(q.AttachmentId, attachbins.Select(x => x.Id).ToArray())) : null;
                var orattachuris = (!attachuris.NullOrEmpty()) ?
                    db.Select<RELS_EMAIL_ALARMS_ATTACHURIS>(q => q.Id == entity.Id && Sql.In(q.AttachmentId, attachuris.Select(x => x.Id).ToArray())) : null;

                //5. save non-existing entity-details relations
                if (!rattends.NullOrEmpty()) db.SaveAll(rattends.Except(orattends));
                if (!rattachbins.NullOrEmpty()) db.SaveAll(rattachbins.Except(orattachbins));
                if (!rattachuris.NullOrEmpty()) db.SaveAll(rattachuris.Except(orattachuris));
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
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
            try
            {
                     db.SaveAll(entities);
                    
                    //1. retrieve details of events
                    var attends = entities.Where(x => x.Attendees.OfType<ATTENDEE>().Count() > 0).SelectMany(x => x.Attendees.OfType<ATTENDEE>());
                    var attachbins = entities.Where(x => x.Attachments.OfType<ATTACH_BINARY>().Count() > 0).SelectMany(x => x.Attachments.OfType<ATTACH_BINARY>());
                    var attachuris = entities.Where(x => x.Attachments.OfType<ATTACH_URI>().Count() > 0).SelectMany(x => x.Attachments.OfType<ATTACH_URI>());

                    //2. save details of events
                    if (!attends.NullOrEmpty()) db.SaveAll(attends);
                    if (!attachbins.NullOrEmpty()) db.SaveAll(attachbins);
                    if (!attachuris.NullOrEmpty()) db.SaveAll(attachuris);

                    //3. construct available relations
                    var rattends = entities.Where(x => x.Attendees.OfType<ATTENDEE>().Count() > 0)
                        .SelectMany(e => e.Attendees.OfType<ATTENDEE>().Select(x => new RELS_EMAIL_ALARMS_ATTENDEES { Id = e.Id, AttendeeId = x.Id }));
                    var rattachbins = entities.Where(x => x.Attachments.OfType<ATTACH_BINARY>().Count() > 0)
                        .SelectMany(e => e.Attachments.OfType<ATTACH_BINARY>().Select(x => new RELS_EMAIL_ALARMS_ATTACHBINS { Id = e.Id, AttachmentId = x.Id }));
                    var rattachuris = entities.Where(x => x.Attachments.OfType<ATTACH_URI>().Count() > 0)
                        .SelectMany(e => e.Attachments.OfType<ATTACH_URI>().Select(x => new RELS_EMAIL_ALARMS_ATTACHURIS { Id = e.Id, AttachmentId = x.Id }));

                    //4. retrieve existing relations
                    var orattends = (!attends.NullOrEmpty()) ?
                        db.Select<RELS_EMAIL_ALARMS_ATTENDEES>(q => Sql.In(q.Id, entities.Select(x => x.Id)) && Sql.In(q.AttendeeId, attends.Select(x => x.Id).ToArray()))  : new List<RELS_EMAIL_ALARMS_ATTENDEES>();

                    var orattachbins = (!attachbins.NullOrEmpty()) ?
                        db.Select<RELS_EMAIL_ALARMS_ATTACHBINS>(q => Sql.In(q.Id, entities.Select(x => x.Id)) && Sql.In(q.AttachmentId, attachbins.Select(x => x.Id).ToArray())) : new List<RELS_EMAIL_ALARMS_ATTACHBINS>();

                    var orattachuris = (!attachuris.NullOrEmpty()) ?
                        db.Select<RELS_EMAIL_ALARMS_ATTACHURIS>(q => Sql.In(q.Id, entities.Select(x => x.Id)) && Sql.In(q.AttachmentId, attachuris.Select(x => x.Id))) : 
                        new List<RELS_EMAIL_ALARMS_ATTACHURIS>();


                    //5. save non-existing entity-details relations
                    if (!rattends.NullOrEmpty() && !rattends.Except(orattends).NullOrEmpty()) db.SaveAll(rattends.Except(orattends));
                    if (!rattachbins.NullOrEmpty() && !rattachbins.Except(orattachbins).NullOrEmpty()) db.SaveAll(rattachbins.Except(orattachbins));
                    if (!rattachuris.NullOrEmpty() && !rattachuris.Except(orattachuris).NullOrEmpty()) db.SaveAll(rattachuris.Except(orattachuris));
                        
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }

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

                var ids = (where != null) ? db.SelectParam<EMAIL_ALARM>(q => q.Id, where).ToArray() : db.SelectParam<EMAIL_ALARM>(q => q.Id).ToArray();

                if (selection.Contains(attendsexpr.GetMemberName()))
                {
                    var attends = (!source.Attendees.OfType<ATTENDEE>().NullOrEmpty()) ? source.Attendees.OfType<ATTENDEE>() : null;
                    if (!attends.NullOrEmpty() && !ids.NullOrEmpty())
                    {
                        db.SaveAll(attends);
                        var rattends = ids.SelectMany(x => attends.Select(y => new RELS_EMAIL_ALARMS_ATTENDEES { Id = this.KeyGenerator.GetNextKey(), AlarmId = x, AttendeeId = y.Id }));
                        var orattends = db.Select<RELS_EMAIL_ALARMS_ATTENDEES>(q => Sql.In(q.Id, ids) && Sql.In(q.AttendeeId, attends.Select(x => x.Id).ToArray()));
                        if (!rattends.NullOrEmpty() && !rattends.Except(orattends).NullOrEmpty()) db.SaveAll(rattends.Except(orattends));

                    }
                }

                if (selection.Contains(attachsexpr.GetMemberName()))
                {
                    var attachbins = (!source.Attachments.OfType<ATTACH_BINARY>().NullOrEmpty()) ? source.Attachments.OfType<ATTACH_BINARY>() : null;
                    if (!attachbins.NullOrEmpty() && !ids.NullOrEmpty())
                    {
                        db.SaveAll(attachbins);
                        var rattachbins = ids.SelectMany(x => attachbins.Select(y => new RELS_EMAIL_ALARMS_ATTACHBINS { Id = this.KeyGenerator.GetNextKey(), AlarmId = x, AttachmentId = y.Id }));
                        var orattachbins = db.Select<RELS_EMAIL_ALARMS_ATTACHBINS>(q => Sql.In(q.Id, ids) && Sql.In(q.AttachmentId, attachbins.Select(x => x.Id).ToArray()));
                        if (!rattachbins.NullOrEmpty() && !rattachbins.Except(orattachbins).NullOrEmpty()) db.SaveAll(rattachbins.Except(orattachbins));
                    }

                    var attachuris = (!source.Attachments.OfType<ATTACH_BINARY>().NullOrEmpty()) ? source.Attachments.OfType<ATTACH_URI>() : null;
                    if (!attachuris.NullOrEmpty() && !ids.NullOrEmpty())
                    {
                        db.SaveAll(attachuris);
                        var rattachuris = ids.SelectMany(x => attachuris.Select(y => new RELS_EMAIL_ALARMS_ATTACHURIS { Id = this.KeyGenerator.GetNextKey(), AlarmId = x, AttachmentId = y.Id }));
                        var orattachuris = db.Select<RELS_EMAIL_ALARMS_ATTACHURIS>(q => Sql.In(q.Id, ids) && Sql.In(q.AttachmentId, attachuris.Select(x => x.Id).ToArray()));
                        if (!rattachuris.NullOrEmpty() && !rattachuris.Except(orattachuris).NullOrEmpty()) db.SaveAll(rattachuris.Except(orattachuris));
                    }
                }
            }


            //6. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

            //7. Update matching event primitives
            if (!sprimitives.NullOrEmpty())
            {
                var patchstr = string.Format("f => new {{ {0} }}", string.Join(", ", sprimitives.Select(x => string.Format("f.{0}", x))));
                var patchexpr = patchstr.CompileToExpressionFunc<EMAIL_ALARM, object>(CodeDomLanguage.csharp, Utilities.GetReferencedAssemblyNamesFromEntryAssembly());
                db.UpdateOnly<EMAIL_ALARM, object>(source, patchexpr, where);
            }
        }
    }

}
