using reexjungle.xcal.core.domain.contracts.models.parameters;
using System.Collections.Generic;

namespace reexjungle.xcal.core.domain.contracts.models.properties
{
    public interface IRESOURCES
    {
        IALTREP AlternativeText { get; }

        ILANGUAGE Language { get; }

        List<string> Values { get; }
    }
}