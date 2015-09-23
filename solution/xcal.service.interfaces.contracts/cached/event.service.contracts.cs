using reexjungle.xcal.service.operations.concretes.cached;

namespace reexjungle.xcal.service.interfaces.contracts.cached
{
    public interface ICachedEventService
    {
        object Get(FindEventCached request);

        object Post(FindEventsCached request);

        object Get(GetEventsCached request);

        object Get(GetEventKeysCached request);
    }
}