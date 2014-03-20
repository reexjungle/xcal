using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reexmonkey.crosscut.essentials.contracts
{
    public enum Authority
    {
        ISO = 0x1,
        NonStandard = 0x2,
        None = 0x4
    }

    public enum ExpectationMode
    {
        optimistic,
        pessimistic,
        unknown
    }
}
