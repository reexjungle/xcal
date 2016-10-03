using System.Collections.Generic;
using xcal.domain.contracts.core.parameters;

namespace xcal.domain.contracts.core.properties
{
    public interface IRESOURCES
    {
        IALTREP AlternativeText { get; set; }

        ILANGUAGE Language { get; set; }

        List<string> Values { get; set; }
    }
}