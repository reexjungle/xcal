using reexjungle.xcal.core.domain.contracts.models.parameters;
using reexjungle.xcal.core.domain.contracts.models.values;

namespace reexjungle.xcal.core.domain.contracts.models.properties
{
    public interface IRECURRENCE_ID
    {
        DATE_TIME Value { get; }
        RANGE Range { get; }
        TZID TimeZoneId { get; }
    }
}
