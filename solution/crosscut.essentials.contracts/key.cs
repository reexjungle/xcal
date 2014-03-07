using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reexmonkey.crosscut.essentials.contracts
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

    /// <summary>
    /// Specifies a contract for providing identifiers
    /// </summary>
    /// <typeparam name="TId">The type of identifier</typeparam>
    public  interface IKeyGenerator<TId>
        where TId: IEquatable<TId>
    {
        /// <summary>
        /// Produces the next identifier
        /// </summary>
        /// <returns>The created identifier</returns>
        TId GetNextKey();
    }
}
