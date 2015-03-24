using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reexjungle.technical.data.contracts
{
    [Flags]
    public enum IgnoreRule
    {
        deleted = 0x2,
        modified = 0x4,
        none = 0x8,
    }

    public interface IDataMigrator
    {
        void Configure<TSource, TTarget>(IgnoreRule rule)
            where TSource : class
            where TTarget : class;

        void Migrate();
    }
}