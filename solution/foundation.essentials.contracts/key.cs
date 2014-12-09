using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reexjungle.foundation.essentials.contracts
{

    /// <summary>
    /// Specifies a contract for identifying a component
    /// </summary>
    /// <typeparam name="TKey">The type of identifier</typeparam>
    public interface IContainsKey<TKey>
        where TKey : IEquatable<TKey>
    {

        /// <summary>
        /// Gets the identifier-key of the component
        /// </summary>
        TKey Id { get; }
    }

}
