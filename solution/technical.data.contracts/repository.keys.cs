using System;
using System.Collections.Generic;

namespace reexjungle.technical.data.contracts
{
    /// <summary>
    /// Specifies the interface for retrieving keys from a non-relational repository
    /// </summary>
    /// <typeparam name="TKey">The type of referencing key</typeparam>
    public interface IReadRepositoryKeys<out TKey>
        where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        /// <summary>
        /// Gets the keys from the non-relational repository
        /// </summary>
        /// <param name="skip">The number of results to skip</param>
        /// <param name="take">The number of results per page to retrieve</param>
        /// <returns></returns>
        IEnumerable<TKey> GetKeys(int? skip = null, int? take = null);
    }
}