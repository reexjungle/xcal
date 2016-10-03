using xcal.domain.contracts.core.values;

namespace xcal.domain.contracts.core.properties
{
    public interface ITRIGGER
    {
        //trigrel
        IDURATION Duration { get; set; }

        RELATED Related { get; set; }

        //trigabs
        IDATE_TIME DateTime { get; set; }

        //Selector
        VALUE ValueType { get; set; }

    }
}
