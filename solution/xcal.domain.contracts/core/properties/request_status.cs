using xcal.domain.contracts.core.parameters;

namespace xcal.domain.contracts.core.properties
{
    public interface IREQUEST_STATUS
    {
        ISTATCODE Code { get; }
        string Description { get; }
        string ExceptionData { get; }
        ILANGUAGE Language { get; }

    }
}
