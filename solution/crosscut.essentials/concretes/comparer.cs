using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using reexmonkey.crosscut.essentials.contracts;

namespace reexmonkey.crosscut.essentials.concretes
{
    public abstract class EqualByTId<TPrimary, TId> : IEqualityComparer<TPrimary>
        where TPrimary : IContainsId<TId>
        where TId : IEquatable<TId>
    {

        public virtual bool Equals(TPrimary x, TPrimary y)
        {
            return x.Id.Equals(y.Id);
        }

        public virtual int GetHashCode(TPrimary obj)
        {
            if (obj == null) return 0;
            var k = (TPrimary)obj;
            return (k != null) ? k.Id.GetHashCode() : 0;
        }
    }

    public class EqualByStringId<TPrimary> : EqualByTId<TPrimary, string>
        where TPrimary : IContainsId<string>
    {
        public override bool Equals(TPrimary x, TPrimary y)
        {
            return x.Id.Equals(y.Id, StringComparison.OrdinalIgnoreCase);
        }
    }

    public class EqualByIntId<TPrimary> : EqualByTId<TPrimary, int>
        where TPrimary : IContainsId<int>
    { }

    public class EqualByLongId<TPrimary> : EqualByTId<TPrimary, long>
        where TPrimary : IContainsId<long>
    { }
}
