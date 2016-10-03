using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xcal.domain.contracts.core.properties
{
    public interface IRELATEDTO
    {
        string Reference { get; set; }
        RELTYPE RelationshipType { get; set; }
    }

}
