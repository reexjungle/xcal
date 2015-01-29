using Funq;
using MySql.Data.MySqlClient;
using reexjungle.crosscut.operations.concretes;
using reexjungle.foundation.essentials.concretes;
using reexjungle.foundation.essentials.contracts;
using reexjungle.infrastructure.concretes.operations;
using reexjungle.infrastructure.contracts;
using reexjungle.technical.data.concretes.extensions.ormlite.mysql;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.interfaces.concretes.live;
using reexjungle.xcal.service.plugins.formats.concretes;
using reexjungle.xcal.service.repositories.concretes.ormlite;
using reexjungle.xcal.service.repositories.concretes.redis;
using reexjungle.xcal.service.repositories.concretes.relations;
using reexjungle.xcal.service.repositories.contracts;
using reexjungle.xcal.service.validators.concretes;
using ServiceStack.CacheAccess;
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

namespace reexjungle.xcal.application.server.web.dev2
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
                    //{ "Access-Control-Allow-Origin", "*" },
                    { "Access-Control-Allow-Methods", "GET, POST, PUT, PATCH, ANY, DELETE, RESET, OPTIONS" },
                    { "Access-Control-Allow-Headers", "Content-Type" },
                },
                DebugMode = true, //Show StackTraces in service responses during development
                ReturnsInnerException = true
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

            container.Register<IGuidKeyGenerator>(new GuidKeyGenerator());

            #endregion inject key generators

            #region inject rdbms provider

            container.Register<IOrmLiteDialectProvider>(MySqlDialect.Provider);
            container.Register<IDbConnectionFactory>(new OrmLiteConnectionFactory(Properties.Settings.Default.mysql_server, container.Resolve<IOrmLiteDialectProvider>()));

            #endregion inject rdbms provider

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
                    x.ConnectionString = string.Format("{0};Database={1};", Properties.Settings.Default.mysql_server, Properties.Settings.Default.nlog_db_name);
                    x.CreateTableIfNotExists<NlogTable>();

                    //create elmah database, table and stored procedures
                    x.CreateSchemaIfNotExists(Properties.Settings.Default.elmah_db_name, Properties.Settings.Default.overwrite_db);
                    x.ChangeDatabase(Properties.Settings.Default.elmah_db_name);
                    x.ConnectionString = string.Format("{0};Database={1};", Properties.Settings.Default.mysql_server, Properties.Settings.Default.elmah_db_name);

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

                    x.Dispose();
                });
            }
            catch (NLog.NLogConfigurationException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
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

            #endregion create logger databases and tables

            #endregion Create databases and corresponding tables

            #region inject core repositories and create primary data sources on first run

            #region inject redis repositories

            container.Register<ICalendarRepository>(x => new CalendarRedisRepository
            {
                KeyGenerator = x.Resolve<IGuidKeyGenerator>(),
                RedisClientsManager = container.Resolve<IRedisClientsManager>(),
                EventRepository = x.Resolve<IEventRepository>(),
            });

            container.Register<IEventRepository>(x => new EventRedisRepository
            {
                KeyGenerator = x.Resolve<IGuidKeyGenerator>(),
                RedisClientsManager = container.Resolve<IRedisClientsManager>(),
                AudioAlarmRepository = x.Resolve<IAudioAlarmRepository>(),
                DisplayAlarmRepository = x.Resolve<IDisplayAlarmRepository>(),
                EmailAlarmRepository = x.Resolve<IEmailAlarmRepository>(),
            });

            container.Register<IAudioAlarmRepository>(x => new AudioAlarmRedisRepository
            {
                KeyGenerator = x.Resolve<IGuidKeyGenerator>(),
                RedisClientsManager = container.Resolve<IRedisClientsManager>(),
            });

            container.Register<IDisplayAlarmRepository>(x => new DisplayAlarmRedisRepository
            {
                KeyGenerator = x.Resolve<IGuidKeyGenerator>(),
                RedisClientsManager = container.Resolve<IRedisClientsManager>(),
            });

            container.Register<IEmailAlarmRepository>(x => new EmailAlarmRedisRepository
            {
                KeyGenerator = x.Resolve<IGuidKeyGenerator>(),
                RedisClientsManager = container.Resolve<IRedisClientsManager>(),
            });

            container.Register<IAdminRepository>(x => new AdminRedisRepository
            {
                RedisClientsManager = container.Resolve<IRedisClientsManager>(),
            });

            #endregion inject redis repositories

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

            #endregion inject redis provider

            #endregion inject core repositories and create primary data sources on first run
        }

        public ApplicationHost()
            : base(Properties.Settings.Default.service_name, typeof(EventService).Assembly)
        {
            #region set up mono compliant settings

            if (Environment.GetEnvironmentVariable("MONO_STRICT_MS_COMPLIANT") != "yes")
                Environment.SetEnvironmentVariable("MONO_STRICT_MS_COMPLIANT", "yes");

            #endregion set up mono compliant settings
        }
    }
}