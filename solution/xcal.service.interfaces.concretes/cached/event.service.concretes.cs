using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.interfaces.concretes.live;
using reexjungle.xcal.service.interfaces.contracts.cached;
using reexjungle.xcal.service.operations.concretes.cached;
using reexjungle.xcal.service.operations.concretes.live;
using ServiceStack.CacheAccess;
using ServiceStack.Common;
using ServiceStack.Logging;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using System;
using reexjungle.xmisc.infrastructure.contracts;

namespace reexjungle.xcal.service.interfaces.concretes.cached
{
    public class CachedEventService : Service, ICachedEventService
    {
        private readonly ICacheClient client;
        private readonly ICacheKeyBuilder<Guid> keyBuilder; 
        private readonly ILogFactory factory;
        private readonly TimeSpan? ttl;

        private ILog log;
        private ILog logger
        {
            get { return log ?? (log = factory.GetLogger(GetType())); }
        }


        public CachedEventService(TimeSpan? ttl, ICacheClient client, ICacheKeyBuilder<Guid> keyBuilder, ILogFactory factory)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (keyBuilder == null) throw new ArgumentNullException("keyBuilder");
            if (factory == null) throw new ArgumentNullException("factory");

            this.ttl = ttl;
            this.client = client;
            this.factory = factory;
            this.keyBuilder = keyBuilder;
        }

        public object Get(FindEventCached request)
        {
            try
            {
                return RequestContext.ToOptimizedResultUsingCache(
                    client,
                    keyBuilder.Build(request, x => x.EventId).ToString(),
                    () =>
                    {
                        var result = ResolveService<EventWebService>()
                            .Get(new FindEvent {EventId = request.EventId});
                        return result ?? VEVENT.Empty;
                    });
            }
            catch (ArgumentNullException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (ArgumentException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (ApplicationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString()); throw;
            }
        }

        public object Post(FindEventsCached request)
        {
            try
            {
                return RequestContext.ToOptimizedResultUsingCache(
                    client,
                    keyBuilder.Build(request, x => x.EventIds).ToString(),
                    ttl,
                    () => ResolveService<EventWebService>()
                        .Post(new FindEvents
                        {
                            EventIds = request.EventIds,
                            Page = request.Page,
                            Size = request.Size
                        }));
            }
            catch (ArgumentNullException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (ArgumentException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (ApplicationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString()); throw;
            }
        }

        public object Get(GetEventsCached request)
        {
            try
            {
               var results = RequestContext.ToOptimizedResultUsingCache(
                    client,
                    keyBuilder.Build(request, x => x.Page, x => x.Size).ToString(),
                    ttl,
                    () => ResolveService<EventWebService>()
                        .Get(new GetEvents
                        {
                            Page = request.Page,
                            Size = request.Size
                        }));

                return results;
            }
            catch (ArgumentNullException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (ArgumentException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (ApplicationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString()); throw;
            }
        }

        public object Get(GetEventKeysCached request)
        {
            try
            {
                return RequestContext.ToOptimizedResultUsingCache(
                    client,
                    keyBuilder.Build(request, x => x.Page.Value, x => x.Size.Value).ToString(),
                    ttl, 
                    () => ResolveService<EventWebService>().Get(new GetEventKeys
                    {
                        Page = request.Page,
                        Size = request.Size
                    }));
            }
            catch (ArgumentNullException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (ArgumentException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (ApplicationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString()); 
                throw;
            }
        }
    }
}