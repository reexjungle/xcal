using reexjungle.xcal.core.domain.contracts.models.values;

namespace reexjungle.xcal.core.domain.contracts.models.properties
{
    public interface ITRIGGER
    {
        VALUE ValueType { get; }
    }

    public interface ITRIGGER_RELATED : ITRIGGER
    {
        RELATED Related { get; }

        IDURATION Value { get; }
    }

    public interface ITRIGGER_ABSOLUTE : ITRIGGER
    {
        IDATE_TIME Value { get; }
    }
}
