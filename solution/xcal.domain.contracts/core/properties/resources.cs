using System.Collections.Generic;
using xcal.domain.contracts.core.parameters;

namespace xcal.domain.contracts.core.properties
{
    public interface IRESOURCES
    {
        IALTREP AlternativeText { get; }

        ILANGUAGE Language { get; }

        List<string> Values { get; }
    }
}