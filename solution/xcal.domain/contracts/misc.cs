using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reexmonkey.xcal.domain.contracts
{
    public interface IMISC_COMPONENT: ICOMPONENT
    {
        string TokenName { get; set; }
    }
}
