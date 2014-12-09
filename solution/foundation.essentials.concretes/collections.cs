using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace reexjungle.foundation.essentials.concretes
{

    /// <summary>
    /// Extension class for generic collections
    /// </summary>
    public static class GenericCollectionExtensions
    {

        /// <summary>
        /// Checks (without null-safety-checks) if a generic collection is empty 
        /// </summary>
        /// <typeparam name="TValue">The generic type of the value.</typeparam>
        /// <param name="source">The geneic collection</param>
        /// <returns>True if it is empty, otherwise false</returns>
        public static bool Empty<TValue>(this IEnumerable<TValue> source)
        {
            return (source.Count() == 0);
        }



        /// <summary>
        /// Checks (with null-safety-checks) if a generic collection is empty 
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static bool SafeEmpty<TValue>(this IEnumerable<TValue> source)
        {
            return (source != null) ? source.Count() == 0 : false;
        }


        /// <summary>
        /// Checks if a generic collection is null or empty
        /// </summary>
        /// <typeparam name="TValue">The generic type of the value.</typeparam>
        /// <param name="source">The geneic collection</param>
        /// <returns>True if it is empty, otherwise false</returns>
        public static bool NullOrEmpty<TValue>(this IEnumerable<TValue> source)
        {
            return (!(source != null && source.Count() != 0));
        }


        /// <summary>
        /// Checks if two generic collections are duplicates of each other
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="first">The collection to be compared</param>
        /// <param name="second">The other generic collection collection</param>
        /// <param name="comparer">The equality comparer used for the comparison. If null, the default equality comparer for the collection member is used</param>
        /// <returns>True if the two collections are duplicates of each other, otherwise false</returns>
        public static bool AreDuplicatesOf<TValue>(this IEnumerable<TValue> first, IEnumerable<TValue> second, IEqualityComparer<TValue> comparer = null)
        {
            return (comparer == null)
                ? first.Intersect(second).Count() == first.Count()
                :first.Intersect(second, comparer).Count() == first.Count();
        }


        /// <summary>
        /// Checks if members of a generic collection are unique to each other.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The generic collection</param>
        /// <param name="comparer">The equality comparer used for the comparison. If null, the default equality comparer for the collection member is used</param>
        /// <returns></returns>
        public static bool AreUnique<TValue>(this IEnumerable<TValue> source, IEqualityComparer<TValue> comparer = null)
        {
            return (comparer == null) ? source.Distinct().Count() == source.Count(): source.Distinct(comparer).Count() == source.Count();
        }


        /// <summary>
        /// Checks if two generic collections are disjoint.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="first">The collection to be compared</param>
        /// <param name="second">The other generic collection collection</param>
        /// <param name="comparer">The equality comparer used for the comparison. If null, the default equality comparer for the collection member is used</param>
        /// <returns>True if the two collections are disjoint, otherwise false</returns>
        public static bool AreDisjoint<TValue>(this IEnumerable<TValue> first, IEnumerable<TValue> second, IEqualityComparer<TValue> comparer = null)
        {
            return (comparer == null) ? first.Intersect(second).Count() == 0 : first.Intersect(second, comparer).Count() == 0;
        }


        /// <summary>
        /// Checks if two generic collections intersects each other.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="first">The collection to be compared</param>
        /// <param name="second">The other generic collection collection</param>
        /// <param name="comparer">The equality comparer used for the comparison. If null, the default equality comparer for the collection member is used</param>
        /// <returns>True if the two collections intersect, otherwise false</returns>
        public static bool Intersects<TValue>(this IEnumerable<TValue> first, IEnumerable<TValue> second, IEqualityComparer<TValue> comparer = null)
        {
            return (comparer == null) ? first.Intersect(second).Count() > 0 : first.Intersect(second, comparer).Count() > 0;
        }



        /// <summary>
        /// Gets the duplicates between two generic collections
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="first">The collection to be compared</param>
        /// <param name="second">The other generic collection collection</param>
        /// <param name="comparer">The equality comparer used for the comparison. If null, the default equality comparer for the collection member is used</param>
        /// <returns></returns>
        public static IEnumerable<TValue> Duplicates<TValue>(this IEnumerable<TValue> first, IEnumerable<TValue> second, IEqualityComparer<TValue> comparer = null)
        {
            return (comparer == null)? second.Intersect(first): second.Intersect(first, comparer);
        }

        public static IEnumerable<TValue> NonDuplicates<TValue>(this IEnumerable<TValue> first, IEnumerable<TValue> second, IEqualityComparer<TValue> comparer = null)
        {
            return (comparer == null)? 
                (second.Union(first)).Except(second.Intersect(first)):
                (second.Union(first, comparer)).Except(second.Intersect(first, comparer), comparer);
        }

        public static void AddRange<TValue>(this List<TValue> list, IEnumerable<TValue> collection, Expression<Func<TValue, bool>> predicate)
        {
            list.AddRange(collection.Where(predicate.Compile()));
        }

        public static void AddRangeComplement<TValue>(this List<TValue> list, IEnumerable<TValue> collection, IEqualityComparer<TValue> comparer = null)
        {
            var incoming = (comparer != null)
                ? collection.ToArray().Except(list.Distinct(), comparer)
                : collection.ToArray().Except(list.Distinct());

            if (!incoming.NullOrEmpty()) list.AddRange(incoming);
        }


        /// <summary>
        /// Adds a value to the specified list when the precondition is satisfied.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="value">The value.</param>
        /// <param name="precondition">if set to <c>true</c> [precondition].</param>
        public static void Add<TValue>(this IList<TValue> list, TValue value, bool precondition)
        {
            if (precondition) list.Add(value);
        }

        public static void Add<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value, bool precondition)
        {
            if (precondition) dictionary.Add(key, value);
        }

        public static void Add<TKey, TValue>(this IDictionary<TKey, IList<TValue>> dictionary, TKey key, TValue value, bool precondition)
            where TValue : IEquatable<TValue>
        {
            if (!precondition) return;
            if (!dictionary.ContainsKey(key)) dictionary.Add(key, new List<TValue> { value });
            else dictionary[key].Add(value);
        }

        public static void Replace<TSource>(this List<TSource> list, IEnumerable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            list.RemoveAll(predicate.Compile().Invoke);
            list.AddRange(source, predicate);
        }

        public static TValue[] ToSingleton<TValue>(this TValue value)
        {
            return new TValue[] { value };
        }




    }
}
