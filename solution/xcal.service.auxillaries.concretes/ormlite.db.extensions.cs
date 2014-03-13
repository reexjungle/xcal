using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ServiceStack.OrmLite;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.service.repositories.concretes;


namespace reexmonkey.xcal.service.auxillaries.concretes
{
    public static class OrmLiteDbConnectionExtensions
    {
        public static void CreateTablesIfNotExist(this IDbConnection db)
        {
            //Create core tables
            db.CreateTableIfNotExists<VCALENDAR>();
            db.CreateTableIfNotExists<VEVENT>();
            db.CreateTableIfNotExists<ORGANIZER>();
            db.CreateTableIfNotExists<ATTACH_BINARY>();
            db.CreateTableIfNotExists<ATTACH_URI>();
            db.CreateTableIfNotExists<RESOURCES>();
            db.CreateTableIfNotExists<ATTENDEE>();
            db.CreateTableIfNotExists<COMMENT>();
            db.CreateTableIfNotExists<CONTACT>();
            db.CreateTableIfNotExists<REQUEST_STATUS>();
            db.CreateTableIfNotExists<RELATEDTO>();
            db.CreateTableIfNotExists<EXDATE>();
            db.CreateTableIfNotExists<RDATE>();
            db.CreateTableIfNotExists<RECURRENCE_ID>();
            db.CreateTableIfNotExists<RECUR>();
            db.CreateTableIfNotExists<AUDIO_ALARM_BINARY>();
            db.CreateTableIfNotExists<AUDIO_ALARM_URI>();
            db.CreateTableIfNotExists<DISPLAY_ALARM>();
            db.CreateTableIfNotExists<EMAIL_ALARM_BINARY>();
            db.CreateTableIfNotExists<EMAIL_ALARM_URI>();

            //create relational tables
            db.CreateTableIfNotExists<REL_CALENDARS_EVENTS>();
            db.CreateTableIfNotExists<REL_EVENTS_ORGANIZERS>();
            db.CreateTableIfNotExists<REL_EVENTS_ATTACH_BINARIES>();
            db.CreateTableIfNotExists<REL_EVENTS_ATTACH_URIS>();
            db.CreateTableIfNotExists<REL_EVENTS_RESOURCES>();
            db.CreateTableIfNotExists<REL_EVENTS_ATTENDEES>();
            db.CreateTableIfNotExists<REL_EVENTS_COMMENTS>();
            db.CreateTableIfNotExists<REL_EVENTS_CONTACTS>();
            db.CreateTableIfNotExists<REL_EVENTS_REQUEST_STATUSES>();
            db.CreateTableIfNotExists<REL_EVENTS_RELATED_TOS>();
            db.CreateTableIfNotExists<REL_EVENTS_RDATES>();
            db.CreateTableIfNotExists<REL_EVENTS_EXDATES>();
            db.CreateTableIfNotExists<REL_EVENTS_RECURRENCE_IDS>();
            db.CreateTableIfNotExists<REL_EVENTS_RECURRENCE_RULES>();
            db.CreateTableIfNotExists<REL_EVENTS_AUDIO_ALARM_BINARIES>();
            db.CreateTableIfNotExists<REL_EVENTS_AUDIO_ALARM_URIS>();
            db.CreateTableIfNotExists<REL_EVENTS_DISPLAY_ALARMS>();
            db.CreateTableIfNotExists<REL_EVENTS_EMAIL_ALARM_BINARIES>();
            db.CreateTableIfNotExists<REL_EVENTS_EMAIL_ALARM_URIS>();


        }

        public static void DropTables(this IDbConnection db)
        {
            //drop email alarm - properties relations
            db.DropTable<RELS_EMAIL_ALARMS_ATTACHBINS>();
            db.DropTable<RELS_EMAIL_ALARMS_ATTACHURIS>();
            db.DropTable<RELS_EMAIL_ALARMS_ATTENDEES>();

            //drop component-properties relations
            //drop event-properties relations
            db.DropTable<REL_EVENTS_RECURRENCE_IDS>();
            db.DropTable<REL_EVENTS_ORGANIZERS>();
            db.DropTable<REL_EVENTS_ATTACHBINS>();
            db.DropTable<REL_EVENTS_ATTACHURIS>();
            db.DropTable<REL_EVENTS_RESOURCES>();
            db.DropTable<REL_EVENTS_ATTENDEES>();
            db.DropTable<REL_EVENTS_COMMENTS>();
            db.DropTable<REL_EVENTS_CONTACTS>();
            db.DropTable<REL_EVENTS_REQSTATS>();
            db.DropTable<REL_EVENTS_RELATEDTOS>();
            db.DropTable<REL_EVENTS_RDATES>();
            db.DropTable<REL_EVENTS_EXDATES>();
            db.DropTable<REL_EVENTS_RRULES>();
            db.DropTable<REL_EVENTS_AUDIO_ALARMS>();
            db.DropTable<REL_EVENTS_DISPLAY_ALARMS>();
            db.DropTable<REL_EVENTS_EMAIL_ALARMS>();

            //drop calendar-components relations
            db.DropTable<REL_CALENDARS_EVENTS>();

            //drop core event properties
            db.DropTable<ATTACH_BINARY>();
            db.DropTable<ATTACH_URI>();
            db.DropTable<RESOURCES>();
            db.DropTable<ATTENDEE>();
            db.DropTable<COMMENT>();
            db.DropTable<CONTACT>();
            db.DropTable<REQUEST_STATUS>();
            db.DropTable<RELATEDTO>();
            db.DropTable<EXDATE>();
            db.DropTable<RDATE>();
            db.DropTable<RECUR>();
            db.DropTable<AUDIO_ALARM>();
            db.DropTable<DISPLAY_ALARM>();
            db.DropTable<EMAIL_ALARM>();
            db.DropTable<ORGANIZER>();
            db.DropTable<RECURRENCE_ID>();

            //drop components
            db.DropTable<VEVENT>();

            //drop calendar objects
            db.DropTable<VCALENDAR>();
        }

        public static void DropAndCreateTables(this IDbConnection db)
        {
            //drop relational tables
            db.DropAndCreateTable<REL_EVENTS_ORGANIZERS>();
            db.DropAndCreateTable<REL_EVENTS_ATTACHBINS>();
            db.DropAndCreateTable<REL_EVENTS_ATTACHURIS>();
            db.DropAndCreateTable<REL_EVENTS_RESOURCES>();
            db.DropAndCreateTable<REL_EVENTS_ATTENDEES>();
            db.DropAndCreateTable<REL_EVENTS_COMMENTS>();
            db.DropAndCreateTable<REL_EVENTS_CONTACTS>();
            db.DropAndCreateTable<REL_EVENTS_REQSTATS>();
            db.DropAndCreateTable<REL_EVENTS_RELATEDTOS>();
            db.DropAndCreateTable<REL_EVENTS_RDATES>();
            db.DropAndCreateTable<REL_EVENTS_EXDATES>();
            db.DropAndCreateTable<REL_EVENTS_RECURRENCE_IDS>();
            db.DropAndCreateTable<REL_EVENTS_RRULES>();
            db.DropAndCreateTable<REL_EVENTS_AUDIO_ALARMS>();
            db.DropAndCreateTable<REL_EVENTS_DISPLAY_ALARMS>();
            db.DropAndCreateTable<REL_EVENTS_EMAIL_ALARMS>();
            db.DropAndCreateTable<RELS_EMAIL_ALARMS_ATTACHBINS>();

            db.DropAndCreateTable<REL_CALENDARS_EVENTS>();


            //drop and create core tables
            db.DropAndCreateTable<VEVENT>();
            db.DropAndCreateTable<VCALENDAR>();
            db.DropAndCreateTable<ORGANIZER>();
            db.DropAndCreateTable<ATTACH_BINARY>();
            db.DropAndCreateTable<ATTACH_URI>();
            db.DropAndCreateTable<RESOURCES>();
            db.DropAndCreateTable<RESOURCES>();
            db.DropAndCreateTable<ATTENDEE>();
            db.DropAndCreateTable<COMMENT>();
            db.DropAndCreateTable<CONTACT>();
            db.DropAndCreateTable<REQUEST_STATUS>();
            db.DropAndCreateTable<RELATEDTO>();
            db.DropAndCreateTable<REQUEST_STATUS>();
            db.DropAndCreateTable<EXDATE>();
            db.DropAndCreateTable<RDATE>();
            db.DropAndCreateTable<RECURRENCE_ID>();
            db.DropAndCreateTable<RECUR>();
            db.DropAndCreateTable<AUDIO_ALARM_BINARY>();
            db.DropAndCreateTable<AUDIO_ALARM_URI>();
            db.DropAndCreateTable<DISPLAY_ALARM>();
            db.DropAndCreateTable<EMAIL_ALARM_BINARY>();
            db.DropAndCreateTable<EMAIL_ALARM_URI>();
        }

        public static void ReinitializeTables(this IDbConnection db)
        {
            //clear core tables
            db.DeleteAll<VCALENDAR>();
            db.DeleteAll<VEVENT>();
            db.DeleteAll<ORGANIZER>();
            db.DeleteAll<ATTACH_BINARY>();
            db.DeleteAll<ATTACH_URI>();
            db.DeleteAll<RESOURCES>();
            db.DeleteAll<ATTENDEE>();
            db.DeleteAll<COMMENT>();
            db.DeleteAll<CONTACT>();
            db.DeleteAll<REQUEST_STATUS>();
            db.DeleteAll<RELATEDTO>();
            db.DeleteAll<REQUEST_STATUS>();
            db.DeleteAll<EXDATE>();
            db.DeleteAll<RDATE>();
            db.DeleteAll<RECUR>();
            db.DeleteAll<AUDIO_ALARM_BINARY>();
            db.DeleteAll<AUDIO_ALARM_URI>();
            db.DeleteAll<DISPLAY_ALARM>();
            db.DeleteAll<EMAIL_ALARM_BINARY>();
            db.DeleteAll<EMAIL_ALARM_URI>();

            //clear relational tables
            db.DeleteAll<REL_CALENDARS_EVENTS>();
            db.DeleteAll<REL_EVENTS_RECURRENCE_IDS>();
            db.DeleteAll<REL_EVENTS_ORGANIZERS>();
            db.DeleteAll<REL_EVENTS_ATTACH_BINARIES>();
            db.DeleteAll<REL_EVENTS_ATTACH_URIS>();
            db.DeleteAll<REL_EVENTS_RESOURCES>();
            db.DeleteAll<REL_EVENTS_ATTENDEES>();
            db.DeleteAll<REL_EVENTS_COMMENTS>();
            db.DeleteAll<REL_EVENTS_CONTACTS>();
            db.DeleteAll<REL_EVENTS_REQUEST_STATUSES>();
            db.DeleteAll<REL_EVENTS_RELATED_TOS>();
            db.DeleteAll<REL_EVENTS_RDATES>();
            db.DeleteAll<REL_EVENTS_EXDATES>();
            db.DeleteAll<REL_EVENTS_RECURRENCE_RULES>();
            db.DeleteAll<REL_EVENTS_AUDIO_ALARM_BINARIES>();
            db.DeleteAll<REL_EVENTS_AUDIO_ALARM_URIS>();
            db.DeleteAll<REL_EVENTS_DISPLAY_ALARMS>();
            db.DeleteAll<REL_EVENTS_EMAIL_ALARM_BINARIES>();
            db.DeleteAll<REL_EVENTS_EMAIL_ALARM_URIS>();

        }
    }
}
