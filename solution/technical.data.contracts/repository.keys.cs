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
        /// <param name="page">The page count of the retrieved keys, when the results are paginated</param>
        /// <returns></returns>
        IEnumerable<TKey> GetKeys(int? page = null);
    }

    /// <summary>
    /// Specifies the interface for retrieving keys from a relational repository
    /// </summary>
    /// <typeparam name="TPKey">The type of referencing key</typeparam>
    /// <typeparam name="TFKey">The type of referenced key</typeparam>
    public interface IReadRepositoryKeys<out TPKey, in TFKey> : IRepository
        where TPKey : IEquatable<TPKey>, IComparable<TPKey>
        where TFKey : IEquatable<TFKey>, IComparable<TFKey>
    {
        /// <summary>
        /// Gets the referencing keys associated to the referenced key from the relational repository
        /// </summary>
        /// <param name="fkey">The referenced key</param>
        /// <param name="page">The page count of retrieved referencing keys, when the results are paginated</param>
        /// <returns>Referencing keys associated to the referenced key</returns>
        IEnumerable<TPKey> GetKeys(TFKey fkey, int? page = null);

        /// <summary>
        /// Gets the referencing keys associated to referenced keys from the relational repository
        /// </summary>
        /// <param name="fkeys">The referenced keys</param>
        /// <param name="page">The page count of retrieved referencing keys, when the results are paginated</param>
        /// <returns>Referencing keys associated to the referenced keys</returns>
        IEnumerable<TPKey> GetKeys(IEnumerable<TFKey> fkeys, int? page = null);
    }
}
