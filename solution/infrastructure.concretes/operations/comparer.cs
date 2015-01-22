using reexjungle.foundation.essentials.contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reexjungle.infrastructure.concretes.operations
{
    public abstract class EqualByTId<TPrimary, TId> : IEqualityComparer<TPrimary>
        where TPrimary : IContainsKey<TId>
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
        where TPrimary : IContainsKey<string>
    {
        public override bool Equals(TPrimary x, TPrimary y)
        {
            return x.Id.Equals(y.Id, StringComparison.OrdinalIgnoreCase);
        }
    }

    public class EqualByIntId<TPrimary> : EqualByTId<TPrimary, int>
        where TPrimary : IContainsKey<int>
    { }

    public class EqualByLongId<TPrimary> : EqualByTId<TPrimary, long>
        where TPrimary : IContainsKey<long>
    { }

    public class ITupleEqualityComparer<T1, T2> : IEqualityComparer<Tuple<T1, T2>>
    {
        private T1 arg1 = default(T1);
        private T2 arg2 = default(T2);

        public ITupleEqualityComparer(T1 arg1)
        {
            this.arg1 = arg1;
        }

        public ITupleEqualityComparer(T2 arg2)
        {
            this.arg2 = arg2;
        }

        public bool Equals(Tuple<T1, T2> x, Tuple<T1, T2> y)
        {
            if (this.arg1.Equals(default(T1)) && arg2.Equals(default(T2))) return false;
            else if (!this.arg1.Equals(default(T1)) && arg2.Equals(default(T2))) return x.Item1.Equals(arg1);
            else if (this.arg1.Equals(default(T1)) && !arg2.Equals(default(T2))) return x.Item2.Equals(arg2);
            else return x.Item1.Equals(arg1) && x.Item2.Equals(arg2);
        }

        public int GetHashCode(Tuple<T1, T2> obj)
        {
            if (obj == null || GetType() != obj.GetType()) return 0;
            return obj.GetHashCode();
        }
    }
}