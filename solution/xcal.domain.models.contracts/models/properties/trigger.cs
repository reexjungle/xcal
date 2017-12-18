using reexjungle.xcal.core.domain.contracts.models.values;

namespace reexjungle.xcal.core.domain.contracts.models.properties
{
    public interface ITRIGGER
    {
        VALUETYPE ValuetypeType { get; }
    }

    public interface ITRIGGER_RELATED : ITRIGGER
    {
        RELATED Related { get; }

        DURATION Value { get; }
    }

    public interface ITRIGGER_ABSOLUTE : ITRIGGER
    {
        DATE_TIME Value { get; }
    }
}
