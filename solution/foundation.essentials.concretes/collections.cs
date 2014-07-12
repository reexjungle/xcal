using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace reexmonkey.foundation.essentials.concretes
{
    public static class GenericCollectionExtensions
    {
        public static bool Empty<TValue>(this IEnumerable<TValue> source)
        {
            if (source == null) throw new ArgumentNullException("source", "Enumerable type should not be null!");
            return (source.Count() == 0);
        }

        public static bool NullOrEmpty<TValue>(this IEnumerable<TValue> source)
        {
            return (!(source != null && source.Count() != 0));
        }

        public static bool AreDuplicatesOf<TValue>(this IEnumerable<TValue> first, IEnumerable<TValue> second, IEqualityComparer<TValue> comparer = null)
        {
            return (comparer == null)
                ? first.Intersect(second).Count() == first.Count()
                :first.Intersect(second, comparer).Count() == first.Count();
        }

        public static bool AreUnique<TValue>(this IEnumerable<TValue> source, IEqualityComparer<TValue> comparer = null)
        {
            return (comparer == null) ? source.Distinct().Count() == source.Count(): source.Distinct(comparer).Count() == source.Count();
        }

        public static bool AreDisjoint<TValue>(this IEnumerable<TValue> first, IEnumerable<TValue> second, IEqualityComparer<TValue> comparer = null)
        {
            return (comparer == null) ? first.Intersect(second).Count() == 0 : first.Intersect(second, comparer).Count() == 0;
        }

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
            try
            {
                var incoming = (comparer != null)
                    ?collection.ToArray().Except(list, comparer)
                    : collection.ToArray().Except(list);

                if (!incoming.NullOrEmpty()) list.AddRange(incoming);
            }
            catch (ArgumentNullException) { throw; }
        }

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
