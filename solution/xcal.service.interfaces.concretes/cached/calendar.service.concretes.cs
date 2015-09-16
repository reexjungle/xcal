using System;
using System.Collections.Generic;
using System.Linq;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.interfaces.concretes.live;
using reexjungle.xcal.service.interfaces.contracts.cached;
using reexjungle.xcal.service.operations.concretes.cached;
using reexjungle.xcal.service.operations.concretes.live;
using reexjungle.xmisc.infrastructure.contracts;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.Logging;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace reexjungle.xcal.service.interfaces.concretes.cached
{
    public class CachedCalendarService : Service, ICachedCalendarService
    {
        private readonly ICacheKeyBuilder<Guid> keyBuilder; 
        private readonly ICacheClient client;
        private readonly ILogFactory factory;
        private readonly TimeSpan? ttl;

        private ILog log;
        private ILog logger
        {
            get { return log ?? (log = factory.GetLogger(GetType())) ; }
        }

        public CachedCalendarService(TimeSpan? ttl, ICacheClient client, ICacheKeyBuilder<Guid> keyBuilder, ILogFactory factory)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (keyBuilder == null) throw new ArgumentNullException("keyBuilder");
            if (factory == null) throw new ArgumentNullException("factory");

            this.client = client;
            this.keyBuilder = keyBuilder;
            this.factory = factory;
            this.keyBuilder = keyBuilder;
            this.ttl = ttl;
        }

        public object Get(FindCalendarCached request)
        {
            try
            {
                return RequestContext.ToOptimizedResultUsingCache(
                    client,
                    keyBuilder.Build(request, x => x.CalendarId).ToString(),
                    ttl,
                    () =>
                    {
                        var result = ResolveService<CalendarWebService>()
                            .Get(new FindCalendar {CalendarId = request.CalendarId});
                        return result ?? VCALENDAR.Empty;
                    });
            }
            catch (ArgumentNullException ex)
            {
                logger.Error(ex);
                throw;
            }
            catch (ArgumentException ex)
            {
                logger.Error(ex);
                throw;
            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex);
                throw;
            }
            catch (ApplicationException ex)
            {
                logger.Error(ex);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex); 
                throw;
            }
        }

        public object Post(FindCalendarsCached request)
        {
            try
            {
                return RequestContext.ToOptimizedResultUsingCache(
                    client,
                    keyBuilder.Build(request, x => x.CalendarIds).ToString(),
                    ttl,
                    () => ResolveService<CalendarWebService>()
                        .Post(new FindCalendars
                        {
                            CalendarIds = request.CalendarIds,
                            Page = request.Page,
                            Size = request.Size
                        }));

            }
            catch (ArgumentNullException ex)
            {
                logger.Error(ex);
                throw;
            }
            catch (ArgumentException ex)
            {
                logger.Error(ex);
                throw;
            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex);
                throw;
            }
            catch (ApplicationException ex)
            {
                logger.Error(ex);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex); throw;
            }
        }

        public object Get(GetCalendarsCached request)
        {
            try
            {
                return RequestContext.ToOptimizedResultUsingCache(
                    client,
                    keyBuilder.NullKey.ToString(),
                    ttl,
                    () => ResolveService<CalendarWebService>()
                        .Get(new GetCalendars
                        {
                            Page = request.Page,
                            Size = request.Size
                        }));
               
            }
            catch (ArgumentNullException ex)
            {
                logger.Error(ex);
                throw;
            }
            catch (ArgumentException ex)
            {
                logger.Error(ex);
                throw;
            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex);
                throw;
            }
            catch (ApplicationException ex)
            {
                logger.Error(ex);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex); 
                throw;
            }
        }

        public object Get(GetCalendarKeysCached request)
        {
            try
            {
                return RequestContext.ToOptimizedResultUsingCache(
                    client,
                    keyBuilder.NullKey.ToString(),
                    ttl,
                    () => ResolveService<CalendarWebService>()
                        .Get(new GetCalendarKeys
                        {
                            Page = request.Page,
                            Size = request.Size
                        }));
            }
            catch (ArgumentNullException ex)
            {
                logger.Error(ex);
                throw;
            }
            catch (ArgumentException ex)
            {
                logger.Error(ex);
                throw;
            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex);
                throw;
            }
            catch (ApplicationException ex)
            {
                logger.Error(ex);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex); 
                throw;
            }
        }
    }
}