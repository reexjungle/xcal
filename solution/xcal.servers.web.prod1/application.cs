using Funq;
using MySql.Data.MySqlClient;
using NLog;
using reexjungle.xcal.application.server.web.prod1.Properties;
using reexjungle.xcal.crosscut.concretes.operations;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.interfaces.concretes.live;
using reexjungle.xcal.service.plugins.formats.concretes;
using reexjungle.xcal.service.repositories.concretes.ormlite;
using reexjungle.xcal.service.repositories.concretes.relations;
using reexjungle.xcal.service.repositories.contracts;
using reexjungle.xcal.service.validators.concretes;
using reexjungle.xmisc.infrastructure.concretes.operations;
using reexjungle.xmisc.infrastructure.contracts;
using reexjungle.xmisc.technical.data.concretes.orm;
using ServiceStack.Logging;
using ServiceStack.Logging.Elmah;
using ServiceStack.Logging.NLogger;
using ServiceStack.OrmLite;
using ServiceStack.Plugins.MsgPack;
using ServiceStack.Redis;
using ServiceStack.ServiceInterface.Cors;
using ServiceStack.ServiceInterface.Validation;
using ServiceStack.WebHost.Endpoints;
using System;
using System.Data;
using System.Diagnostics;

namespace reexjungle.xcal.application.server.web.prod1
{
    public class ApplicationHost : AppHostBase
    {
        public override void Configure(Container container)
        {
            #region configure headers

            //Enable global CORS features on  Response headers
            SetConfig(new EndpointHostConfig
            {
                GlobalResponseHeaders =
                {
                    //{ "Access-Control-Allow-Origin", "*" },
                    { "Access-Control-Allow-Methods", "GET, POST, PUT, PATCH, ANY, DELETE, RESET, OPTIONS" },
                    { "Access-Control-Allow-Headers", "Content-Type" },
                },
                DebugMode = false, //Show StackTraces in service responses during development
                ReturnsInnerException = false
            });

            #endregion configure headers

            #region configure request and response filters

            //this.PreRequestFilters.Add((req, res) =>
            //    {
            //    });

            #endregion configure request and response filters

            #region configure plugins

            Plugins.Add(new ValidationFeature());
            Plugins.Add(new MsgPackFormat());
            Plugins.Add(new iCalendarFormat());
            Plugins.Add(new CorsFeature());

            #endregion configure plugins

            #region inject plugins

            //register all validators defined in the assembly of EventValidator
            container.RegisterValidators(typeof(EventValidator).Assembly);

            #endregion inject plugins

            #region inject loggers

            container.Register<ILogFactory>(new ElmahLogFactory(new NLogFactory()));

            #endregion inject loggers

            #region inject key generators

            container.Register<IKeyGenerator<Guid>>(new SequentialGuidKeyGenerator());

            #endregion inject key generators

            #region inject rdbms provider

            container.Register(MySqlDialect.Provider);
            container.Register<IDbConnectionFactory>(new OrmLiteConnectionFactory(Settings.Default.mysql_server, container.Resolve<IOrmLiteDialectProvider>()));

            #endregion inject rdbms provider

            #region Create databases and corresponding tables

            var dbfactory = container.Resolve<IDbConnectionFactory>();

            #region create logger databases and tables

            try
            {
                dbfactory.Run(x =>
                {
                    //create NLog database and table
                    x.CreateSchemaIfNotExists(Settings.Default.nlog_db_name, Settings.Default.overwrite_db);
                    x.ChangeDatabase(Settings.Default.nlog_db_name);
                    x.ConnectionString = string.Format("{0};Database={1};", Settings.Default.mysql_server, Settings.Default.nlog_db_name);
                    x.CreateTableIfNotExists<NlogTable>();

                    //create elmah database, table and stored procedures
                    x.CreateSchemaIfNotExists(Settings.Default.elmah_db_name, Settings.Default.overwrite_db);
                    x.ChangeDatabase(Settings.Default.elmah_db_name);
                    x.ConnectionString = string.Format("{0};Database={1};", Settings.Default.mysql_server, Settings.Default.elmah_db_name);

                    //execute initialization script on first run
                    if (!x.TableExists(Settings.Default.elmah_error_table))
                    {
                        //execute creation of stored procedures
                        x.ExecuteSql(Resources.elmah_mysql_CreateLogTable);
                        x.ExecuteSql(Resources.elmah_mysql_GetErrorXml);
                        x.ExecuteSql(Resources.elmah_mysql_GetErrorsXml);
                        x.ExecuteSql(Resources.elmah_mysql_LogError);

                        //call "create table" stored procedure
                        x.Exec(cmd =>
                        {
                            cmd.CommandText = "elmah_CreateLogTable";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.ExecuteNonQuery();
                        });
                    }

                    x.Dispose();
                });
            }
            catch (NLogConfigurationException ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            catch (NLogRuntimeException ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            #endregion create logger databases and tables

            #endregion Create databases and corresponding tables

            #region inject core repositories and create primary data sources on first run

            #region create main database and tables

            try
            {
                dbfactory.Run(x =>
                {
                    x.CreateSchemaIfNotExists(Settings.Default.main_db_name);
                    x.ChangeDatabase(Settings.Default.main_db_name);
                    x.ConnectionString = string.Format("{0};Database={1};", Settings.Default.mysql_server, Settings.Default.main_db_name);

                    //core tables
                    x.CreateTableIfNotExists(typeof(VCALENDAR), typeof(VEVENT), typeof(VTODO), typeof(VFREEBUSY), typeof(FREEBUSY_INFO), typeof(VJOURNAL), typeof(VTIMEZONE), typeof(STANDARD), typeof(DAYLIGHT), typeof(IANA_PROPERTY), typeof(IANA_COMPONENT), typeof(X_PROPERTY), typeof(X_COMPONENT), typeof(AUDIO_ALARM), typeof(DISPLAY_ALARM), typeof(EMAIL_ALARM), typeof(ATTENDEE), typeof(ORGANIZER), typeof(COMMENT), typeof(RELATEDTO), typeof(ATTACH_BINARY), typeof(ATTACH_URI), typeof(CONTACT), typeof(RDATE), typeof(EXDATE), typeof(REQUEST_STATUS), typeof(RESOURCES), typeof(TZNAME));

                    //Create 3NF relational tables
                    x.CreateTableIfNotExists(typeof(REL_CALENDARS_EVENTS), typeof(REL_CALENDARS_TODOS), typeof(REL_CALENDARS_FREEBUSIES), typeof(REL_CALENDARS_JOURNALS), typeof(REL_CALENDARS_TIMEZONES), typeof(REL_CALENDARS_IANACS), typeof(REL_CALENDARS_XCS), typeof(REL_EVENTS_ATTACHBINS), typeof(REL_EVENTS_ATTACHURIS), typeof(REL_EVENTS_ATTENDEES), typeof(REL_EVENTS_ORGANIZERS), typeof(REL_EVENTS_RECURS), typeof(REL_EVENTS_AUDIO_ALARMS), typeof(REL_EVENTS_COMMENTS), typeof(REL_EVENTS_CONTACTS), typeof(REL_EVENTS_DISPLAY_ALARMS), typeof(REL_EVENTS_EMAIL_ALARMS), typeof(REL_EVENTS_EXDATES), typeof(REL_EVENTS_RDATES), typeof(REL_EVENTS_RELATEDTOS), typeof(REL_EVENTS_REQSTATS), typeof(REL_EVENTS_RESOURCES), typeof(REL_TODOS_ATTACHBINS), typeof(REL_TODOS_ATTACHURIS), typeof(REL_TODOS_ATTENDEES), typeof(REL_TODOS_AUDIO_ALARMS), typeof(REL_TODOS_COMMENTS), typeof(REL_TODOS_CONTACTS), typeof(REL_TODOS_DISPLAY_ALARMS), typeof(REL_TODOS_EMAIL_ALARMS), typeof(REL_TODOS_EXDATES), typeof(REL_TODOS_RDATES), typeof(REL_TODOS_RELATEDTOS), typeof(REL_TODOS_REQSTATS), typeof(REL_TODOS_RESOURCES), typeof(REL_FREEBUSIES_ATTACHBINS), typeof(REL_FREEBUSIES_ATTACHURIS), typeof(REL_FREEBUSIES_ATTENDEES), typeof(REL_FREEBUSIES_COMMENTS), typeof(REL_FREEBUSIES_REQSTATS), typeof(REL_FREEBUSIES_INFOS), typeof(REL_JOURNALS_ATTACHBINS), typeof(REL_JOURNALS_ATTACHURIS), typeof(REL_JOURNALS_ATTENDEES), typeof(REL_JOURNALS_COMMENTS), typeof(REL_JOURNALS_CONTACTS), typeof(REL_JOURNALS_EXDATES), typeof(REL_AALARMS_ATTACHBINS), typeof(REL_AALARMS_ATTACHURIS), typeof(REL_EALARMS_ATTACHBINS), typeof(REL_EALARMS_ATTACHURIS), typeof(REL_EALARMS_ATTENDEES), typeof(REL_TIMEZONES_STANDARDS), typeof(REL_TIMEZONES_DAYLIGHT), typeof(REL_STANDARDS_RECURS), typeof(REL_STANDARDS_COMMENTS), typeof(REL_STANDARDS_RDATES), typeof(REL_STANDARDS_TZNAMES), typeof(REL_DAYLIGHT_RECURS), typeof(REL_DAYLIGHTS_COMMENTS), typeof(REL_DAYLIGHTS_RDATES), typeof(REL_DAYLIGHTS_TZNAMES));
                });
            }
            catch (MySqlException ex)
            {
                container.Resolve<ILogFactory>().GetLogger(GetType()).Error(ex.StackTrace, ex);
            }
            catch (InvalidOperationException ex)
            {
                container.Resolve<ILogFactory>().GetLogger(GetType()).Error(ex.ToString(), ex);
            }
            catch (Exception ex)
            {
                container.Resolve<ILogFactory>().GetLogger(GetType()).Error(ex.ToString(), ex);
            }

            #endregion create main database and tables

            #region inject ormlite repositories

            container.Register<IAudioAlarmRepository>(x => new AudioAlarmOrmRepository(
                    x.Resolve<IKeyGenerator<Guid>>(),
                    x.Resolve<IDbConnectionFactory>()));

            container.Register<IDisplayAlarmRepository>(x => new DisplayAlarmOrmRepository(
                x.Resolve<IDbConnectionFactory>()));

            container.Register<IEmailAlarmRepository>(x => new EmailAlarmOrmRepository(
                x.Resolve<IKeyGenerator<Guid>>(),
                x.Resolve<IDbConnectionFactory>()));

            container.Register<IEventRepository>(x => new EventOrmRepository(
                    x.Resolve<IKeyGenerator<Guid>>(),
                    x.Resolve<IAudioAlarmRepository>(),
                    x.Resolve<IDisplayAlarmRepository>(),
                    x.Resolve<IEmailAlarmRepository>(),
                    x.Resolve<IDbConnectionFactory>()));

            container.Register<ICalendarRepository>(x => new CalendarOrmRepository(
                    x.Resolve<IKeyGenerator<Guid>>(),
                    x.Resolve<IEventRepository>(),
                    x.Resolve<IDbConnectionFactory>()));

            container.Register<IAdminRepository>(x => new AdminOrmRepository(
                x.Resolve<IDbConnectionFactory>()));

            #endregion inject ormlite repositories

            #region inject cached providers

            //register cache client to redis server running on linux.
            //NOTE: Redis Server must already be installed on the remote machine and must be running
            container.Register<IRedisClientsManager>(x => new BasicRedisClientManager(Settings.Default.redis_server));
            var cachedclient = container.Resolve<IRedisClientsManager>().GetCacheClient();
            if (cachedclient != null) container.Register(x => cachedclient);

            #endregion inject cached providers

            #endregion inject core repositories and create primary data sources on first run
        }

        public ApplicationHost()
            : base(Settings.Default.service_name, typeof(EventWebService).Assembly)
        {
            #region set up mono compliant settings

            if (Environment.GetEnvironmentVariable("MONO_STRICT_MS_COMPLIANT") != "yes")
                Environment.SetEnvironmentVariable("MONO_STRICT_MS_COMPLIANT", "yes");

            #endregion set up mono compliant settings
        }
    }
}