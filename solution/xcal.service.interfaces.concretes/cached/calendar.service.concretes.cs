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
    public class CachedCalendarService : Service, ICachedCalendarService
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

        public CachedCalendarService()
            : base()
        {
            this.TimeToLive = this.ResolveService<TimeSpan?>();
        }

        public object Get(FindCalendarCached request)
        {
            try
            {
                return base.RequestContext.ToOptimizedResultUsingCache(
                this.CacheClient, UrnId.Create<VCALENDAR>(request.CalendarId),
                () =>
                {
                    var result = this.ResolveService<CalendarService>()
                        .Get(new FindCalendar { CalendarId = request.CalendarId });
                    return result ?? VCALENDAR.Empty;
                });
            }
            catch (ArgumentNullException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ArgumentException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }

        public object Post(FindCalendarsCached request)
        {
            try
            {
                return base.RequestContext.ToOptimizedResultUsingCache(
                this.CacheClient,
                request.Page != null && request.Size != null ? string.Format("urn:calendars:{0}:{1}", request.Page, request.Size) : "urn:calendars",
                this.TimeToLive,
                () =>
                {
                    return this.ResolveService<CalendarService>()
                        .Post(new FindCalendars
                        {
                            CalendarIds = request.CalendarIds,
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

        public object Get(GetCalendarsCached request)
        {
            try
            {
                return base.RequestContext.ToOptimizedResultUsingCache(
                this.CacheClient,
                request.Page != null && request.Size != null ? string.Format("urn:calendars:{0}:{1}", request.Page, request.Size) : "urn:calendars",
                this.TimeToLive,
                () =>
                {
                    return this.ResolveService<CalendarService>()
                        .Get(new GetCalendars
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

        public object Get(GetCalendarKeysCached request)
        {
            try
            {
                return base.RequestContext.ToOptimizedResultUsingCache(
                this.CacheClient,
                request.Page != null && request.Size != null ?
                string.Format("urn:calendars:{0}:{1}", request.Page, request.Size) :
                "urn:calendars",
                this.TimeToLive,
                () =>
                {
                    return this.ResolveService<CalendarService>()
                        .Get(new GetCalendarKeys
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