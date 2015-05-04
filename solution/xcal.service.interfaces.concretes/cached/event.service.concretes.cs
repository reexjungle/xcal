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

        private ILog log = null;

        private ILog logger
        {
            get { return (log != null) ? this.log : this.logfactory.GetLogger(this.GetType()); }
        }

        public ILogFactory LogFactory
        {
            get { return this.logfactory; }
            set
            {
                if (value == null) throw new ArgumentNullException("Logger");
                this.logfactory = value;
                this.log = logfactory.GetLogger(this.GetType());
            }
        }

        public TimeSpan? TimeToLive { get; set; }

        public CachedEventService()
        {
            this.TimeToLive = this.ResolveService<TimeSpan?>();
        }

        public object Get(FindEventCached request)
        {
            try
            {
                return base.RequestContext.ToOptimizedResultUsingCache(
                this.CacheClient, UrnId.Create<VEVENT>(request.EventId),
                () =>
                {
                    var result = this.ResolveService<EventService>()
                        .Get(new FindEvent { EventId = request.EventId });
                    return result ?? VEVENT.Empty;
                });
            }
            catch (ArgumentNullException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ArgumentException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public object Post(FindEventsCached request)
        {
            try
            {
                return base.RequestContext.ToOptimizedResultUsingCache(
                this.CacheClient,
                request.Page != null && request.Size != null ? string.Format("urn:events:{0}:{1}", request.Page, request.Size) : "urn:events",
                this.TimeToLive,
                () =>
                {
                    return this.ResolveService<EventService>()
                        .Post(new FindEvents
                        {
                            EventIds = request.EventIds,
                            Page = request.Page,
                            Size = request.Size
                        });
                });
            }
            catch (ArgumentNullException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ArgumentException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public object Get(GetEventsCached request)
        {
            try
            {
                return base.RequestContext.ToOptimizedResultUsingCache(
                this.CacheClient,
                request.Page != null && request.Size != null ? string.Format("urn:events:{0}:{1}", request.Page, request.Size) : "urn:events",
                this.TimeToLive,
                () =>
                {
                    return this.ResolveService<EventService>()
                        .Get(new GetEvents
                        {
                            Page = request.Page,
                            Size = request.Size
                        });
                });
            }
            catch (ArgumentNullException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ArgumentException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public object Get(GetEventKeysCached request)
        {
            try
            {
                return base.RequestContext.ToOptimizedResultUsingCache(
                this.CacheClient,
                request.Page != null && request.Size != null ? string.Format("urn:events:{0}:{1}", request.Page, request.Size) : "urn:events",
                this.TimeToLive,
                () =>
                {
                    return this.ResolveService<EventService>()
                        .Get(new GetEventKeys
                        {
                            Page = request.Page,
                            Size = request.Size
                        });
                });
            }
            catch (ArgumentNullException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ArgumentException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }
    }
}