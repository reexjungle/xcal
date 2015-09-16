using reexjungle.xcal.service.operations.concretes.cached;

namespace reexjungle.xcal.service.interfaces.contracts.cached
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICachedCalendarService
    {
        /// <summary>
        /// Finds a calendar from 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        object Get(FindCalendarCached request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        object Post(FindCalendarsCached request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        object Get(GetCalendarsCached request);
    }
}