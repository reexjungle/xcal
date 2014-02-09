using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reexmonkey.crosscut.essentials.contracts
{
    /// <summary>
    /// Specifies a contract for identifying a component
    /// </summary>
    public interface IContainsId<TId>
        where TId : IEquatable<TId>
    {
        /// <summary>
        /// The identifier-key of the component
        /// </summary>
        TId Id { get; }
    }
}
