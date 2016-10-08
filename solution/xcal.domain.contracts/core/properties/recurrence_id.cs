using xcal.domain.contracts.core.parameters;
using xcal.domain.contracts.core.values;

namespace xcal.domain.contracts.core.properties
{
    public interface IRECURRENCE_ID
    {
        IDATE_TIME Value { get; }
        RANGE Range { get; }
        ITZID TimeZoneId { get; }
    }
}
