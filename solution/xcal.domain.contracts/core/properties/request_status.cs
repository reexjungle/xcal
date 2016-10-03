using xcal.domain.contracts.core.parameters;

namespace xcal.domain.contracts.core.properties
{
    public interface IREQUEST_STATUS
    {
        ISTATCODE Code { get; set; }
        string Description { get; set; }
        string ExceptionData { get; set; }
        ILANGUAGE Language { get; set; }

    }
}
