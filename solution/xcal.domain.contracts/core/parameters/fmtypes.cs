using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xcal.domain.contracts.core.parameters
{
    public interface IFMTTYPE
    {
        string TypeName { get; set; }

        string SubTypeName { get; set; }
    }
}
