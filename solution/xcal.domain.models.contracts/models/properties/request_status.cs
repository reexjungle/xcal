using reexjungle.xcal.core.domain.contracts.models.parameters;

namespace reexjungle.xcal.core.domain.contracts.models.properties
{
    public interface IREQUEST_STATUS
    {
        ISTATCODE Code { get; }
        string Description { get; }
        string ExceptionData { get; }
        ILANGUAGE Language { get; }

    }
}
