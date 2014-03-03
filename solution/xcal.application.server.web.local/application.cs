using System;
using Funq;
using ServiceStack.CacheAccess;
using ServiceStack.OrmLite;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.Redis;
using ServiceStack.Logging;
using reexmonkey.xcal.service.interfaces.live;
using reexmonkey.crosscut.goodies.concretes;
using reexmonkey.xcal.service.repositories.contracts;
using reexmonkey.xcal.service.repositories.concretes;


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
                    { "Access-Control-Allow-Methods", "GET, POST, PUT, PATCH, DELETE, RESET, OPTIONS" },
                    { "Access-Control-Allow-Headers", "Content-Type" },
                },
                DebugMode = true, //Do not show StackTraces in service responses during development
            });

            #endregion

            #region configure routes

            #endregion

            #region inject plugins
            
            #endregion

            #region inject database provider
            
            if(Properties.Settings.Default.db_provider_type == DataProviderType.rdbms)
            {
                #region inject orm repositories

                container.Register<ICalendarRepository>(x => new CalendarOrmLiteRepository
                {
                    DbConnectionFactory = x.Resolve<IDbConnectionFactory>(),
                    EventRepository = x.Resolve<IEventRepository>(),
                    Pages = Properties.Settings.Default.calendars_page_count
                });

                container.Register<IEventRepository>(x => new EventOrmLiteRepository
                {
                    DbConnectionFactory = x.Resolve<IDbConnectionFactory>(),
                    AudioAlarmOrmLiteRepository = x.Resolve<IAudioAlarmOrmLiteRepository>(),
                    DisplayAlarmOrmLiteRepository = x.Resolve<IDisplayAlarmOrmLiteRepository>(),
                    EmailAlarmOrmLiteRepository = x.Resolve<IEmailAlarmOrmLiteRepository>(),
                    Pages = Properties.Settings.Default.events_page_count
                });

                container.Register<IAudioAlarmOrmLiteRepository>(x => new AudioAlarmOrmLiteRepository
                {
                    DbConnectionFactory = x.Resolve<IDbConnectionFactory>(),
                    Pages = Properties.Settings.Default.alarms_page_count
                });

                container.Register<IDisplayAlarmOrmLiteRepository>(x => new DisplayAlarmOrmLiteRepository
                {
                    DbConnectionFactory = x.Resolve<IDbConnectionFactory>(),
                    Pages = Properties.Settings.Default.alarms_page_count
                });

                container.Register<IEmailAlarmOrmLiteRepository>(x => new EmailAlarmOrmLiteRepository
                {
                    DbConnectionFactory = x.Resolve<IDbConnectionFactory>(),
                    Pages = Properties.Settings.Default.alarms_page_count
                });

                #endregion

                #region inject cached providers

                //register cache client to redis server running on linux. 
                //NOTE: Redis Server must already be installed on the remote machine and must be running
                container.Register<IRedisClientsManager>(x => new PooledRedisClientManager(Properties.Settings.Default.redis_server));
                container.Register<ICacheClient>(x => (ICacheClient)x.Resolve<IRedisClientsManager>().GetCacheClient());

                #endregion

                #region inject db provider


                #endregion
            }
            else if(Properties.Settings.Default.db_provider_type == DataProviderType.nosql)
            {
                #region inject redis repositories

                #endregion

                #region inject redis provider

                //register cache client to redis server running on linux. 
                //NOTE: Redis Server must already be installed on the remote machine and must be running
                container.Register<IRedisClientsManager>(x => new PooledRedisClientManager(Properties.Settings.Default.redis_server));
                container.Register<IRedisClient>(x => x.Resolve<IRedisClientsManager>().GetClient());


                #endregion
            }

            #endregion

            #region inject loggers



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