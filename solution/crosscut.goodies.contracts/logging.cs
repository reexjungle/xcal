using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reexmonkey.crosscut.goodies.concretes
{

    public interface ILogTable<TKey>
        where TKey: IEquatable<TKey>
    {
        TKey Key { get; set; }
        string Message { get; set; }
    }
}
