using System;
using System.Data;
using Funq;
using MySql.Data.MySqlClient;
using ServiceStack.CacheAccess;
using ServiceStack.OrmLite;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.Redis;
using ServiceStack.Logging;
using ServiceStack.Logging.NLogger;
using ServiceStack.Logging.Elmah;
using ServiceStack.ServiceInterface.Validation;
using ServiceStack.Plugins.MsgPack;
using ServiceStack.ServiceInterface.Cors;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.service.validators.concretes;
using reexmonkey.xcal.service.interfaces.concretes.live;
using reexmonkey.foundation.essentials.contracts;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.crosscut.operations.concretes;
using reexmonkey.xcal.service.repositories.contracts;
using reexmonkey.xcal.service.repositories.concretes;
using reexmonkey.technical.data.concretes.extensions.ormlite.mysql;
using reexmonkey.infrastructure.operations.concretes;
using reexmonkey.infrastructure.operations.contracts;


namespace reexmonkey.xcal.application.server.web.local
{
    public class ApplicationHost : AppHostBase
    {
        public override void Configure(Container container)
        {
            #region configure headers

            //Enable global CORS features on  Response headers
            base.SetConfig(new EndpointHostConfig
            {
                GlobalResponseHeaders =
                {
                    { "Access-Control-Allow-Origin", "*" },
                    { "Access-Control-Allow-Methods", "GET, POST, PUT, PATCH, ANY, DELETE, RESET, OPTIONS" },
                    { "Access-Control-Allow-Headers", "Content-Type" },
                },
                DebugMode = true, //Show StackTraces in service responses during development
                ReturnsInnerException = false
            });

            #endregion

            #region configure routes

            //TODO: Configure routes centrally

            #endregion

            #region configure plugins

            Plugins.Add(new ValidationFeature());
            Plugins.Add(new MsgPackFormat());
            Plugins.Add(new CorsFeature());

            #endregion

            #region inject plugins

            //register all validators defined in the assembly of EventValidator
            //container.RegisterValidators(typeof(EventValidator).Assembly);

            #endregion

            #region inject loggers

            container.Register<ILogFactory>(new ElmahLogFactory(new NLogFactory()));

            #endregion

            #region inject key generators

            container.Register<IGuidKeyGenerator>(new GuidKeyGenerator());

            #endregion

            #region inject rdbms provider

            container.Register<IOrmLiteDialectProvider>(MySqlDialect.Provider);
            container.Register<IDbConnectionFactory>(new OrmLiteConnectionFactory(
                Properties.Settings.Default.mysql_server,
                container.Resolve<IOrmLiteDialectProvider>()));

            #endregion

            #region Create databases and corresponding tables

            var dbfactory = container.Resolve<IDbConnectionFactory>();

            #region create logger databases and tables

            try
            {
                dbfactory.Run(x =>
                {
                    //create NLog database and table
                    x.CreateSchemaIfNotExists(Properties.Settings.Default.nlog_db_name, Properties.Settings.Default.overwrite_db);
                    x.ChangeDatabase(Properties.Settings.Default.nlog_db_name);
                    x.ConnectionString = string.Format("{0}; Database={1}", Properties.Settings.Default.mysql_server, Properties.Settings.Default.nlog_db_name);
                    x.CreateTableIfNotExists<NlogTable>();

                    //create elmah database, table and stored procedures
                    x.CreateSchemaIfNotExists(Properties.Settings.Default.elmah_db_name, Properties.Settings.Default.overwrite_db);
                    x.ChangeDatabase(Properties.Settings.Default.elmah_db_name);
                    x.ConnectionString = string.Format("{0}; Database={1}", Properties.Settings.Default.mysql_server, Properties.Settings.Default.elmah_db_name);

                    //execute initialization script on first run
                    if (!x.TableExists(Properties.Settings.Default.elmah_error_table))
                    {
                        //execute creation of stored procedures
                        x.ExecuteSql(Properties.Resources.elmah_mysql_CreateLogTable);
                        x.ExecuteSql(Properties.Resources.elmah_mysql_GetErrorXml);
                        x.ExecuteSql(Properties.Resources.elmah_mysql_GetErrorsXml);
                        x.ExecuteSql(Properties.Resources.elmah_mysql_LogError);

                        //call "create table" stored procedure
                        x.Exec(cmd =>
                        {
                            cmd.CommandText = "elmah_CreateLogTable";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.ExecuteNonQuery();
                        });
                    }
                });

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            catch (NLog.NLogRuntimeException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            catch (InvalidOperationException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            #endregion

           
            #endregion

            #region inject core repositories and create primary data sources on first run


            if (Properties.Settings.Default.primary_storage == StorageType.rdbms)
            {

                #region  create main database and tables

                try
                {
                    dbfactory.Run(x =>
                    {
                        x.CreateSchemaIfNotExists(Properties.Settings.Default.main_db_name, false);
                        x.ChangeDatabase(Properties.Settings.Default.main_db_name);
                        x.ConnectionString = string.Format("{0}; Database={1}", Properties.Settings.Default.mysql_server, Properties.Settings.Default.main_db_name);

                        //Create core tables 
                        x.CreateTableIfNotExists(typeof(VCALENDAR), typeof(VEVENT), typeof(AUDIO_ALARM), typeof(DISPLAY_ALARM), typeof(EMAIL_ALARM), typeof(ORGANIZER), typeof(ATTENDEE), typeof(TEXT), typeof(RELATEDTO), typeof(ATTACH_BINARY), typeof(ATTACH_URI), typeof(TEXT), typeof(RDATE), typeof(EXDATE), typeof(RECUR), typeof(RECURRENCE_ID), typeof(REQUEST_STATUS), typeof(RESOURCES));

                        //Create 3NF relational tables
                        x.CreateTableIfNotExists(typeof(REL_CALENDARS_EVENTS), typeof(REL_EVENTS_ATTACHBINS), typeof(REL_EVENTS_ATTACHURIS), typeof(REL_EVENTS_ATTENDEES), typeof(REL_EVENTS_AUDIO_ALARMS), typeof(REL_EVENTS_COMMENTS), typeof(REL_EVENTS_CONTACTS), typeof(REL_EVENTS_DISPLAY_ALARMS), typeof(REL_EVENTS_EMAIL_ALARMS), typeof(REL_EVENTS_EXDATES), typeof(REL_EVENTS_ORGANIZERS), typeof(REL_EVENTS_RDATES), typeof(REL_EVENTS_RECURRENCE_IDS), typeof(REL_EVENTS_RELATEDTOS), typeof(REL_EVENTS_REQSTATS), typeof(REL_EVENTS_RESOURCES), typeof(REL_EVENTS_RECURS), typeof(RELS_EALARMS_ATTACHBINS), typeof(RELS_EALARMS_ATTACHURIS), typeof(RELS_EALARMS_ATTENDEES));
                    });

                }
                catch (MySqlException ex)
                {
                    container.Resolve<ILogFactory>().GetLogger(this.GetType()).Error(ex.ToString());
                }
                catch (InvalidOperationException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    container.Resolve<ILogFactory>().GetLogger(this.GetType()).Error(ex.ToString(), ex);
                }

                #endregion

                #region inject ormlite repositories

                container.Register<ICalendarRepository>(x => new CalendarOrmLiteRepository
                {
                    KeyGenerator = x.Resolve<IGuidKeyGenerator>(),
                    DbConnectionFactory = x.Resolve<IDbConnectionFactory>(),
                    EventRepository = x.Resolve<IEventRepository>(),
                    Take = Properties.Settings.Default.calendars_take
                });

                var ar = new AudioAlarmOrmLiteRepository()
                {
                    KeyGenerator = container.Resolve<IGuidKeyGenerator>(),
                    DbConnectionFactory = container.Resolve<IDbConnectionFactory>(),
                    Take = Properties.Settings.Default.alarms_take
                };
                ar.Take = 2;

                container.Register<IEventRepository>(x => new EventOrmLiteRepository
                {
                    KeyGenerator = x.Resolve<IGuidKeyGenerator>(),
                    DbConnectionFactory = x.Resolve<IDbConnectionFactory>(),
                    AudioAlarmRepository = x.Resolve<IAudioAlarmRepository>(),
                    DisplayAlarmRepository = x.Resolve<IDisplayAlarmRepository>(),
                    EmailAlarmRepository = x.Resolve<IEmailAlarmRepository>(),
                    Take = Properties.Settings.Default.events_take
                });

                container.Register<IAudioAlarmRepository>(x => new AudioAlarmOrmLiteRepository
                {
                    KeyGenerator = x.Resolve<IGuidKeyGenerator>(),
                    DbConnectionFactory = x.Resolve<IDbConnectionFactory>(),
                    Take = Properties.Settings.Default.alarms_take
                });

                container.Register<IDisplayAlarmRepository>(x => new DisplayAlarmOrmLiteRepository
                {
                    KeyGenerator = x.Resolve<IGuidKeyGenerator>(),
                    DbConnectionFactory = x.Resolve<IDbConnectionFactory>(),
                    Take = Properties.Settings.Default.alarms_take
                });

                container.Register<IEmailAlarmRepository>(x => new EmailAlarmOrmLiteRepository
                {
                    KeyGenerator = x.Resolve<IGuidKeyGenerator>(),
                    DbConnectionFactory = x.Resolve<IDbConnectionFactory>(),
                    Take = Properties.Settings.Default.alarms_take
                });

                #endregion

                #region inject cached providers

                //register cache client to redis server running on linux. 
                //NOTE: Redis Server must already be installed on the remote machine and must be running
                container.Register<IRedisClientsManager>(x => new BasicRedisClientManager(Properties.Settings.Default.redis_server));
                var cachedclient = container.Resolve<IRedisClientsManager>().GetCacheClient();
                if (cachedclient != null) container.Register<ICacheClient>(x => cachedclient);

                #endregion

            }
            else if (Properties.Settings.Default.primary_storage == StorageType.nosql)
            {
                #region inject redis repositories

                container.Register<ICalendarRepository>(x => new CalendarRedisRepository
                {
                    KeyGenerator = x.Resolve<IGuidKeyGenerator>(),
                    RedisClientsManager = container.Resolve<IRedisClientsManager>(),
                    EventRepository = x.Resolve<IEventRepository>(),
                    Take = Properties.Settings.Default.calendars_take
                });

                container.Register<IEventRepository>(x => new EventRedisRepository
                {
                    KeyGenerator = x.Resolve<IGuidKeyGenerator>(),
                    RedisClientsManager = container.Resolve<IRedisClientsManager>(),
                    AudioAlarmRepository = x.Resolve<IAudioAlarmRepository>(),
                    DisplayAlarmRepository = x.Resolve<IDisplayAlarmRepository>(),
                    EmailAlarmRepository = x.Resolve<IEmailAlarmRepository>(),
                    Take = Properties.Settings.Default.events_take
                });

                container.Register<IAudioAlarmRepository>(x => new AudioAlarmRedisRepository
                {
                    KeyGenerator = x.Resolve<IGuidKeyGenerator>(),
                    RedisClientsManager = container.Resolve<IRedisClientsManager>(),
                    Take = Properties.Settings.Default.alarms_take
                });

                container.Register<IDisplayAlarmRepository>(x => new DisplayAlarmRedisRepository
                {
                    KeyGenerator = x.Resolve<IGuidKeyGenerator>(),
                    RedisClientsManager = container.Resolve<IRedisClientsManager>(),
                    Take = Properties.Settings.Default.alarms_take
                });

                container.Register<IEmailAlarmRepository>(x => new EmailAlarmRedisRepository
                {
                    KeyGenerator = x.Resolve<IGuidKeyGenerator>(),
                    RedisClientsManager = container.Resolve<IRedisClientsManager>(),
                    Take = Properties.Settings.Default.alarms_take
                });

                #endregion

                #region inject redis provider

                //register cache client to redis server running on linux. 
                //NOTE: Redis Server must already be installed on the local machine and must be running
                container.Register<IRedisClientsManager>(x => new BasicRedisClientManager(Properties.Settings.Default.redis_server));

                try
                {
                    var redis = container.Resolve<IRedisClientsManager>().GetClient();
                    redis.FlushDb();
                }
                catch (RedisResponseException ex)
                {
                    container.Resolve<ILogFactory>().GetLogger(this.GetType()).Error(ex.ToString(), ex);
                }
                catch (RedisException ex)
                {
                    container.Resolve<ILogFactory>().GetLogger(this.GetType()).Error(ex.ToString(), ex);
                }
                #endregion
            }

            #endregion
        }

        public ApplicationHost() : base(Properties.Settings.Default.service_name, typeof(EventService).Assembly)
        {
            #region set up mono compliant settings

            if (Environment.GetEnvironmentVariable("MONO_STRICT_MS_COMPLIANT") != "yes")
                Environment.SetEnvironmentVariable("MONO_STRICT_MS_COMPLIANT", "yes");

            #endregion

        }

    }
}