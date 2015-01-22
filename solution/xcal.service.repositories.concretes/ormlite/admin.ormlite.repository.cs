using reexjungle.infrastructure.contracts;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.repositories.concretes.relations;
using reexjungle.xcal.service.repositories.contracts;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace reexjungle.xcal.service.repositories.concretes.ormlite
{
    public class AdminOrmLiteRepository : IAdminOrmLiteRepository
    {
        private IDbConnection conn;
        private IDbConnectionFactory factory = null;

        private IDbConnection db
        {
            get { return (this.conn) ?? (conn = factory.OpenDbConnection()); }
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

        public AdminOrmLiteRepository()
        {
        }

        public AdminOrmLiteRepository(IDbConnectionFactory factory)
        {
            this.DbConnectionFactory = factory;
        }

        public AdminOrmLiteRepository(IDbConnection connection)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            this.conn = connection;
        }

        public void Dispose()
        {
            if (this.conn != null) this.conn.Dispose();
        }

        private void RecreateTables(IDbConnectionFactory factory)
        {
            factory.Run(x =>
            {
                using (var transaction = x.BeginTransaction())
                {
                    try
                    {
                        //core tables
                        x.DropAndCreateTables(typeof(VCALENDAR), typeof(VEVENT), typeof(VTODO), typeof(VFREEBUSY), typeof(FREEBUSY_INFO), typeof(VJOURNAL), typeof(VTIMEZONE), typeof(STANDARD), typeof(DAYLIGHT), typeof(IANA_PROPERTY), typeof(IANA_COMPONENT), typeof(X_PROPERTY), typeof(XCOMPONENT), typeof(AUDIO_ALARM), typeof(DISPLAY_ALARM), typeof(EMAIL_ALARM), typeof(ORGANIZER), typeof(ATTENDEE), typeof(COMMENT), typeof(RELATEDTO), typeof(ATTACH_BINARY), typeof(ATTACH_URI), typeof(CONTACT), typeof(RDATE), typeof(EXDATE), typeof(RECUR), typeof(RECURRENCE_ID), typeof(REQUEST_STATUS), typeof(RESOURCES), typeof(TZNAME));

                        //3NF relational tables
                        x.DropAndCreateTables(typeof(REL_CALENDARS_EVENTS), typeof(REL_CALENDARS_TODOS), typeof(REL_CALENDARS_FREEBUSIES), typeof(REL_CALENDARS_JOURNALS), typeof(REL_CALENDARS_TIMEZONES), typeof(REL_CALENDARS_IANACS), typeof(REL_CALENDARS_XCS), typeof(REL_EVENTS_ATTACHBINS), typeof(REL_EVENTS_ATTACHURIS), typeof(REL_EVENTS_ATTENDEES), typeof(REL_EVENTS_AUDIO_ALARMS), typeof(REL_EVENTS_COMMENTS), typeof(REL_EVENTS_CONTACTS), typeof(REL_EVENTS_DISPLAY_ALARMS), typeof(REL_EVENTS_EMAIL_ALARMS), typeof(REL_EVENTS_EXDATES), typeof(REL_EVENTS_RDATES), typeof(REL_EVENTS_RELATEDTOS), typeof(REL_EVENTS_REQSTATS), typeof(REL_EVENTS_RESOURCES), typeof(REL_TODOS_ATTACHBINS), typeof(REL_TODOS_ATTACHURIS), typeof(REL_TODOS_ATTENDEES), typeof(REL_TODOS_AUDIO_ALARMS), typeof(REL_TODOS_COMMENTS), typeof(REL_TODOS_CONTACTS), typeof(REL_TODOS_DISPLAY_ALARMS), typeof(REL_TODOS_EMAIL_ALARMS), typeof(REL_TODOS_EXDATES), typeof(REL_TODOS_RDATES), typeof(REL_TODOS_RELATEDTOS), typeof(REL_TODOS_REQSTATS), typeof(REL_TODOS_RESOURCES), typeof(REL_FREEBUSIES_ATTACHBINS), typeof(REL_FREEBUSIES_ATTACHURIS), typeof(REL_FREEBUSIES_ATTENDEES), typeof(REL_FREEBUSIES_COMMENTS), typeof(REL_FREEBUSIES_REQSTATS), typeof(REL_FREEBUSIES_INFOS), typeof(REL_JOURNALS_ATTACHBINS), typeof(REL_JOURNALS_ATTACHURIS), typeof(REL_JOURNALS_ATTENDEES), typeof(REL_JOURNALS_COMMENTS), typeof(REL_JOURNALS_CONTACTS), typeof(REL_JOURNALS_EXDATES), typeof(REL_JOURNALS_RDATES), typeof(REL_JOURNALS_RELATEDTOS), typeof(REL_JOURNALS_REQSTATS), typeof(REL_JOURNALS_RESOURCES), typeof(REL_EALARMS_ATTACHBINS), typeof(REL_EALARMS_ATTACHURIS), typeof(REL_EALARMS_ATTENDEES), typeof(REL_TIMEZONES_STANDARDS), typeof(REL_TIMEZONES_DAYLIGHT), typeof(REL_STANDARDS_COMMENTS), typeof(REL_STANDARDS_RDATES), typeof(REL_STANDARDS_TZNAMES), typeof(REL_DAYLIGHTS_COMMENTS), typeof(REL_DAYLIGHTS_RDATES), typeof(REL_DAYLIGHTS_TZNAMES));

                        transaction.Commit();
                    }
                    catch (ApplicationException)
                    {
                        try { transaction.Rollback(); }
                        catch (Exception) { }
                        throw;
                    }
                    catch (InvalidOperationException)
                    {
                        try { transaction.Rollback(); }
                        catch (Exception) { }
                        throw;
                    }
                }
            });
        }

        private void DeleteAllRowsFromTables(IDbConnectionFactory factory)
        {
            factory.Run(x =>
            {
                using (var transaction = x.BeginTransaction())
                {
                    try
                    {
                        //core tables
                        x.DeleteAll(typeof(VCALENDAR));
                        x.DeleteAll(typeof(VEVENT));
                        x.DeleteAll(typeof(VTODO));
                        x.DeleteAll(typeof(VFREEBUSY));
                        x.DeleteAll(typeof(FREEBUSY_INFO));
                        x.DeleteAll(typeof(VJOURNAL));
                        x.DeleteAll(typeof(VTIMEZONE));
                        x.DeleteAll(typeof(STANDARD));
                        x.DeleteAll(typeof(DAYLIGHT));
                        x.DeleteAll(typeof(IANA_PROPERTY));
                        x.DeleteAll(typeof(IANA_COMPONENT));
                        x.DeleteAll(typeof(X_PROPERTY));
                        x.DeleteAll(typeof(XCOMPONENT));
                        x.DeleteAll(typeof(AUDIO_ALARM));
                        x.DeleteAll(typeof(DISPLAY_ALARM));
                        x.DeleteAll(typeof(EMAIL_ALARM));
                        x.DeleteAll(typeof(ATTENDEE));
                        x.DeleteAll(typeof(COMMENT));
                        x.DeleteAll(typeof(RELATEDTO));
                        x.DeleteAll(typeof(ATTACH_BINARY));
                        x.DeleteAll(typeof(ATTACH_URI));
                        x.DeleteAll(typeof(CONTACT));
                        x.DeleteAll(typeof(RDATE));
                        x.DeleteAll(typeof(EXDATE));
                        x.DeleteAll(typeof(REQUEST_STATUS));
                        x.DeleteAll(typeof(RESOURCES));
                        x.DeleteAll(typeof(TZNAME));

                        transaction.Commit();
                    }
                    catch (ApplicationException)
                    {
                        try { transaction.Rollback(); }
                        catch (Exception) { }
                        throw;
                    }
                    catch (InvalidOperationException)
                    {
                        try { transaction.Rollback(); }
                        catch (Exception) { }
                        throw;
                    }
                }
            });
        }

        public void Flush(FlushMode mode = FlushMode.soft)
        {
            try
            {
                if (mode == FlushMode.hard) this.RecreateTables(this.DbConnectionFactory);
                else this.DeleteAllRowsFromTables(this.DbConnectionFactory);
            }
            catch (ApplicationException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }
    }
}