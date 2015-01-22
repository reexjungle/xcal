using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reexjungle.infrastructure.contracts
{
    public interface IPaginated<T>
        where T : struct
    {
        T? Page { get; set; }

        T? Size { get; set; }
    }
}