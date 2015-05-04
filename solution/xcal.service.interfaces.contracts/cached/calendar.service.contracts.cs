using reexjungle.xcal.service.operations.concretes.cached;

namespace reexjungle.xcal.service.interfaces.contracts.cached
{
    public interface ICachedCalendarService
    {
        object Get(FindCalendarCached request);

        object Post(FindCalendarsCached request);

        object Get(GetCalendarsCached request);
    }
}