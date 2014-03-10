using System;
using Funq;
using ServiceStack.CacheAccess;
using ServiceStack.OrmLite;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.Redis;
using ServiceStack.Logging;
using ServiceStack.Logging.NLogger;
using ServiceStack.ServiceInterface.Validation;
using ServiceStack.Plugins.MsgPack;
using ServiceStack.ServiceInterface.Cors;
using reexmonkey.xcal.service.validators.concretes;
using reexmonkey.xcal.service.interfaces.contracts.live;
using reexmonkey.xcal.service.interfaces.concretes.live;
using reexmonkey.crosscut.essentials.contracts;
using reexmonkey.crosscut.essentials.concretes;
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
                DebugMode = false, //Show StackTraces in service responses during development
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
            container.RegisterValidators(typeof(EventValidator).Assembly);

            #endregion

            #region configure loggers

            

            #endregion

            #region inject loggers

            container.Register<ILogFactory>(new NLogFactory());

            #endregion

            #region inject key generators

            container.Register<IGuidKeyGenerator>(new GuidKeyGenerator());
            container.Register<IFPIKeyGenerator>(x => new FPIKeyGenerator<string>(new GuidKeyGenerator())
            {
                Owner = Properties.Settings.Default.fpiOwner,
                LanguageId = Properties.Settings.Default.fpiLanguageId,
                Description = Properties.Settings.Default.fpiDescription,
                Authority = Properties.Settings.Default.fpiAuthority
            });

            #endregion

            #region inject core repositories and datastore

            if (Properties.Settings.Default.db_provider_type == DataProviderType.rdbms)
            {
                #region inject ormlite repositories

                container.Register<ICalendarRepository>(x => new CalendarOrmLiteRepository
                {
                    KeyGenerator = x.Resolve<IFPIKeyGenerator>(),
                    DbConnectionFactory = x.Resolve<IDbConnectionFactory>(),
                    EventRepository = x.Resolve<IEventRepository>(),
                    Pages = Properties.Settings.Default.calendars_page_count
                });

                container.Register<IEventRepository>(x => new EventOrmLiteRepository
                {
                    KeyGenerator = x.Resolve<IGuidKeyGenerator>(),
                    DbConnectionFactory = x.Resolve<IDbConnectionFactory>(),
                    AudioAlarmRepository = x.Resolve<IAudioAlarmOrmLiteRepository>(),
                    DisplayAlarmRepository = x.Resolve<IDisplayAlarmOrmLiteRepository>(),
                    EmailAlarmRepository = x.Resolve<IEmailAlarmOrmLiteRepository>(),
                    Pages = Properties.Settings.Default.events_page_count
                });

                container.Register<IAudioAlarmOrmLiteRepository>(x => new AudioAlarmOrmLiteRepository
                {
                    KeyGenerator = x.Resolve<IGuidKeyGenerator>(),
                    DbConnectionFactory = x.Resolve<IDbConnectionFactory>(),
                    Pages = Properties.Settings.Default.alarms_page_count
                });

                container.Register<IDisplayAlarmOrmLiteRepository>(x => new DisplayAlarmOrmLiteRepository
                {
                    KeyGenerator = x.Resolve<IGuidKeyGenerator>(),
                    DbConnectionFactory = x.Resolve<IDbConnectionFactory>(),
                    Pages = Properties.Settings.Default.alarms_page_count
                });

                container.Register<IEmailAlarmOrmLiteRepository>(x => new EmailAlarmOrmLiteRepository
                {
                    KeyGenerator = x.Resolve<IGuidKeyGenerator>(),
                    DbConnectionFactory = x.Resolve<IDbConnectionFactory>(),
                    Pages = Properties.Settings.Default.alarms_page_count
                });

                #endregion

                #region inject cached providers

                //register cache client to redis server running on linux. 
                //NOTE: Redis Server must already be installed on the remote machine and must be running
                container.Register<IRedisClientsManager>(x => new PooledRedisClientManager(Properties.Settings.Default.redis_server));
                var cclient = container.Resolve<IRedisClientsManager>().GetCacheClient();
                if (cclient != null) container.Register<ICacheClient>(x => cclient);

                #endregion

                #region inject rdbms provider


                #endregion
            }
            else if (Properties.Settings.Default.db_provider_type == DataProviderType.nosql)
            {
                #region inject redis repositories

                container.Register<ICalendarRepository>(x => new CalendarRedisRepository
                {
                    KeyGenerator = x.Resolve<IFPIKeyGenerator>(),
                    RedisClientsManager = container.Resolve<IRedisClientsManager>(),
                    EventRepository = x.Resolve<IEventRepository>(),
                    Pages = Properties.Settings.Default.calendars_page_count
                });

                container.Register<IEventRepository>(x => new EventRedisRepository
                {
                    KeyGenerator = x.Resolve<IGuidKeyGenerator>(),
                    RedisClientsManager = container.Resolve<IRedisClientsManager>(),
                    AudioAlarmRepository = x.Resolve<IAudioAlarmRedisRepository>(),
                    DisplayAlarmRepository = x.Resolve<IDisplayAlarmRedisRepository>(),
                    EmailAlarmRepository = x.Resolve<IEmailAlarmRedisRepository>(),
                    Pages = Properties.Settings.Default.events_page_count
                });

                container.Register<IAudioAlarmRedisRepository>(x => new AudioAlarmRedisRepository
                {
                    KeyGenerator = x.Resolve<IGuidKeyGenerator>(),
                    RedisClientsManager = container.Resolve<IRedisClientsManager>(),
                    Pages = Properties.Settings.Default.alarms_page_count
                });

                container.Register<IDisplayAlarmRedisRepository>(x => new DisplayAlarmRedisRepository
                {
                    KeyGenerator = x.Resolve<IGuidKeyGenerator>(),
                    RedisClientsManager = container.Resolve<IRedisClientsManager>(),
                    Pages = Properties.Settings.Default.alarms_page_count
                });

                container.Register<IEmailAlarmRedisRepository>(x => new EmailAlarmRedisRepository
                {
                    KeyGenerator = x.Resolve<IGuidKeyGenerator>(),
                    RedisClientsManager = container.Resolve<IRedisClientsManager>(),
                    Pages = Properties.Settings.Default.alarms_page_count
                });

                #endregion

                #region inject redis provider

                //register cache client to redis server running on linux. 
                //NOTE: Redis Server must already be installed on the remote machine and must be running
                container.Register<IRedisClientsManager>(x => new PooledRedisClientManager(Properties.Settings.Default.redis_server));


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