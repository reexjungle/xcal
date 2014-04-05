using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reexmonkey.technical.data.contracts
{
    /// <summary>
    /// Specifies the interface for retrieving keys from a non-relational repository
    /// </summary>
    /// <typeparam name="TKey">The type of referencing key</typeparam>
    public interface IReadRepositoryKeys<out TKey> : IRepository
        where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        /// <summary>
        /// Gets the keys from the non-relational repository
        /// </summary>
        /// <param name="skip">The page count of the retrieved keys, when the results are paginated</param>
        /// <returns></returns>
        IEnumerable<TKey> GetKeys(int? skip = null);
    }

 
}
