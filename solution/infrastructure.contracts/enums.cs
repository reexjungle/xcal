using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reexmonkey.infrastructure.operations.contracts
{
    public enum ExpectationMode
    {
        optimistic,
        pessimistic,
        unknown
    }

    public enum Authority
    {
        ISO = 0x1,
        NonStandard = 0x2,
        None = 0x4
    }
}
