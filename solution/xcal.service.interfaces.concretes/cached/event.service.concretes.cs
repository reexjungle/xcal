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

namespace reexjungle.xcal.service.interfaces.concretes.cached
{
    public class CachedEventService : Service, ICachedEventService
    {
        public ICacheClient CacheClient { get; set; }

        private ILogFactory logfactory;

        private ILog log;

        private ILog logger
        {
            get { return log ?? (log = logfactory.GetLogger(GetType())); }
        }

        public ILogFactory LogFactory
        {
            get { return logfactory; }
            set
            {
                if (value == null) throw new ArgumentNullException("Logger");
                logfactory = value;
                log = logfactory.GetLogger(GetType());
            }
        }

        public TimeSpan? TimeToLive { get; set; }

        public CachedEventService()
        {
            TimeToLive = ResolveService<TimeSpan?>();
        }

        public object Get(FindEventCached request)
        {
            try
            {
                return RequestContext.ToOptimizedResultUsingCache(
                CacheClient, UrnId.Create<VEVENT>(request.EventId),
                () =>
                {
                    var result = ResolveService<EventService>()
                        .Get(new FindEvent { EventId = request.EventId });
                    return result ?? VEVENT.Empty;
                });
            }
            catch (ArgumentNullException ex) { logger.Error(ex.ToString()); throw; }
            catch (ArgumentException ex) { logger.Error(ex.ToString()); throw; }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }

        public object Post(FindEventsCached request)
        {
            try
            {
                return RequestContext.ToOptimizedResultUsingCache(
                CacheClient,
                request.Page != null && request.Size != null ? string.Format("urn:events:{0}:{1}", request.Page, request.Size) : "urn:events",
                TimeToLive,
                () =>
                {
                    return ResolveService<EventService>()
                        .Post(new FindEvents
                        {
                            EventIds = request.EventIds,
                            Page = request.Page,
                            Size = request.Size
                        });
                });
            }
            catch (ArgumentNullException ex) { logger.Error(ex.ToString()); throw; }
            catch (ArgumentException ex) { logger.Error(ex.ToString()); throw; }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }

        public object Get(GetEventsCached request)
        {
            try
            {
                return RequestContext.ToOptimizedResultUsingCache(
                CacheClient,
                request.Page != null && request.Size != null ? string.Format("urn:events:{0}:{1}", request.Page, request.Size) : "urn:events",
                TimeToLive,
                () =>
                {
                    return ResolveService<EventService>()
                        .Get(new GetEvents
                        {
                            Page = request.Page,
                            Size = request.Size
                        });
                });
            }
            catch (ArgumentNullException ex) { logger.Error(ex.ToString()); throw; }
            catch (ArgumentException ex) { logger.Error(ex.ToString()); throw; }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }

        public object Get(GetEventKeysCached request)
        {
            try
            {
                return RequestContext.ToOptimizedResultUsingCache(
                CacheClient,
                request.Page != null && request.Size != null ? string.Format("urn:events:{0}:{1}", request.Page, request.Size) : "urn:events",
                TimeToLive,
                () =>
                {
                    return ResolveService<EventService>()
                        .Get(new GetEventKeys
                        {
                            Page = request.Page,
                            Size = request.Size
                        });
                });
            }
            catch (ArgumentNullException ex) { logger.Error(ex.ToString()); throw; }
            catch (ArgumentException ex) { logger.Error(ex.ToString()); throw; }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }
    }
}