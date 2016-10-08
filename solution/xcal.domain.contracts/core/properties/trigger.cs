using xcal.domain.contracts.core.values;

namespace xcal.domain.contracts.core.properties
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
