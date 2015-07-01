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
            get { return (log != null) ? log : logfactory.GetLogger(GetType()); }
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

        public CachedCalendarService()
            : base()
        {
            TimeToLive = ResolveService<TimeSpan?>();
        }

        public object Get(FindCalendarCached request)
        {
            try
            {
                return RequestContext.ToOptimizedResultUsingCache(
                CacheClient, UrnId.Create<VCALENDAR>(request.CalendarId),
                () =>
                {
                    var result = ResolveService<CalendarService>()
                        .Get(new FindCalendar { CalendarId = request.CalendarId });
                    return result ?? VCALENDAR.Empty;
                });
            }
            catch (ArgumentNullException ex) { logger.Error(ex.ToString()); throw; }
            catch (ArgumentException ex) { logger.Error(ex.ToString()); throw; }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }

        public object Post(FindCalendarsCached request)
        {
            try
            {
                return RequestContext.ToOptimizedResultUsingCache(
                CacheClient,
                request.Page != null && request.Size != null ? string.Format("urn:calendars:{0}:{1}", request.Page, request.Size) : "urn:calendars",
                TimeToLive,
                () =>
                {
                    return ResolveService<CalendarService>()
                        .Post(new FindCalendars
                        {
                            CalendarIds = request.CalendarIds,
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

        public object Get(GetCalendarsCached request)
        {
            try
            {
                return RequestContext.ToOptimizedResultUsingCache(
                CacheClient,
                request.Page != null && request.Size != null ? string.Format("urn:calendars:{0}:{1}", request.Page, request.Size) : "urn:calendars",
                TimeToLive,
                () =>
                {
                    return ResolveService<CalendarService>()
                        .Get(new GetCalendars
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

        public object Get(GetCalendarKeysCached request)
        {
            try
            {
                return RequestContext.ToOptimizedResultUsingCache(
                CacheClient,
                request.Page != null && request.Size != null ?
                string.Format("urn:calendars:{0}:{1}", request.Page, request.Size) :
                "urn:calendars",
                TimeToLive,
                () =>
                {
                    return ResolveService<CalendarService>()
                        .Get(new GetCalendarKeys
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