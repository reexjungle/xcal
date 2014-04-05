using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using reexmonkey.crosscut.essentials.contracts;

namespace reexmonkey.technical.data.contracts
{
    /// <summary>
    /// Specifies the interface for repositories
    /// </summary>
    public interface IRepository { }


    /// <summary>
    /// Specifies the interface for read-only operations on a repository
    /// </summary>
    /// <typeparam name="TEntity">Type of entity to read from repository</typeparam>
    /// <typeparam name="TKey">Type of unique identifier for for retrieving entities</typeparam>
    public interface IReadRepository<out TEntity, in TKey> : IRepository
        where TKey: IEquatable<TKey>, IComparable<TKey>
    {
        /// <summary>
        /// Finds an entity in the repository based on a unique identifier
        /// </summary>
        /// <param name="key">Type of unique identifier for retrieval of entity from repository </param>
        /// <returns>The found entity from the repository</returns>
        TEntity Find(TKey key);

        /// <summary>
        /// Finds entities in the repository based on unique identifiers
        /// </summary>
        /// <param name="keys">Unique identifiers for retrieving the entities</param>
        /// <param name="skip">The page number of the retrieved entities when the results are paged</param>
        /// <returns>Found entities from the repository </returns>
        IEnumerable<TEntity> Find(IEnumerable<TKey> keys, int? skip = null);

        /// <summary>
        /// Gets all entities from the repository
        /// </summary>
        /// <param name="skip">The page count of the retrieved entities</param>
        /// <returns></returns>
        IEnumerable<TEntity> Get(int? skip = null);

        /// <summary>
        /// Checks if the repository contains an entity
        /// </summary>
        /// <param name="key">The unique identifier of the entity</param>
        /// <returns>True if the entity is found in the repository, otherwise false</returns>
        bool ContainsKey(TKey key);

        /// <summary>
        /// Checks if the repository contains entities
        /// </summary>
        /// <param name="keys">The unique identifiers of the entities</param>
        /// <param name="mode">How the search is performed. 
        /// Optimistic if at least one entity found, 
        /// Pessimistic if all entities are found</param>
        /// <returns>True if the entities are found, otherwise false</returns>
        bool ContainsKeys(IEnumerable<TKey> keys, ExpectationMode mode = ExpectationMode.optimistic);
    }

    /// <summary>
    /// Specifies the interface for write-only operationd on a repository
    /// </summary>
    /// <typeparam name="TEntity">Type of entity to write to repository</typeparam>
    /// <typeparam name="TKey">Type of unique identifier for writing entities</typeparam>
    public interface IWriteRepository<TEntity, TKey> : IRepository
       where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        /// <summary>
        /// Gets the provider of identifiers
        /// </summary>
        IKeyGenerator<TKey> KeyGenerator { get; set; }

        /// <summary>
        /// Inserts a new entity or updates an existing one in the repository
        /// </summary>
        /// <param name="entity">The entity to save</param>
        void Save(TEntity entity);

        /// <summary>
        /// Patches fields of an entity in the repository
        /// </summary>
        /// <param name="source">The source containing patch details</param>
        /// <param name="fields">Specfies which fields are used for the patching. The fields are specified in an anonymous variable</param>
        /// <param name="predicate">Filters the entities to patch. No filter implies all entities are patched</param>
        void Patch(TEntity source, Expression<Func<TEntity, object>> fields, Expression<Func<TEntity, bool>> predicate = null);

        /// <summary>
        /// Erases an entity from the repository based on a unique identifier
        /// </summary>
        /// <param name="key">The unique identifier of the entity</param>
        void Erase(TKey key);

        /// <summary>
        /// Inserts new entities or updates existing ones in the repository 
        /// </summary>
        /// <param name="entities">The entities to save</param>
        void SaveAll(IEnumerable<TEntity> entities);

        /// <summary>
        /// Erases entities from the repository based on unique identifiers
        /// </summary>
        /// <param name="keys">The unique identifier of the entity</param>
        void EraseAll(IEnumerable<TKey> keys = null);

    }

}
