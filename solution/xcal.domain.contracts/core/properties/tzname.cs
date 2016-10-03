using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xcal.domain.contracts.core.parameters;

namespace xcal.domain.contracts.core.properties
{
    public interface ITZNAME
    {
        ILANGUAGE Language { get; set; }

        string Text { get; set; }
    }
}
