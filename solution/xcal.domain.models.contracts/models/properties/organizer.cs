using reexjungle.xcal.core.domain.contracts.models.parameters;
using reexjungle.xcal.core.domain.contracts.models.values;

namespace reexjungle.xcal.core.domain.contracts.models.properties
{
    public interface IORGANIZER
    {
        CAL_ADDRESS Address { get; set; }
        string CN { get; set; }
        IDIR Directory { get; set; }
        ISENT_BY SentBy { get; set; }
        ILANGUAGE Language { get; set; }
    }
}
