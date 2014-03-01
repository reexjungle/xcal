using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reexmonkey.crosscut.essentials.contracts
{

    /// <summary>
    /// Specifies a contract for identifying a component
    /// </summary>
    /// <typeparam name="TId">The type of identifier</typeparam>
    public interface IContainsId<TId>
        where TId : IEquatable<TId>
    {

        /// <summary>
        /// Gets the identifier-key of the component
        /// </summary>
        TId Id { get; }
    }

    /// <summary>
    /// Specifies a contract for providing identifiers
    /// </summary>
    /// <typeparam name="TId">The type of identifier</typeparam>
    public  interface IProvidesId<TId>
        where TId: IEquatable<TId>
    {
        /// <summary>
        /// Produces the next identifier
        /// </summary>
        /// <returns>The created identifier</returns>
        TId NextId();
    }
}
