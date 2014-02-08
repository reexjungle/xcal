using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace reexmonkey.crosscut.essentials.concretes
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

        public static bool AreDuplicatesOf<TValue>(this IEnumerable<TValue> source, IEnumerable<TValue> other, Expression<Func<IEnumerable<TValue>, IEnumerable<TValue>, bool>> comparer)
        {
            return comparer.Compile()(source, other);
        }

        public static bool AreDuplicatesOf<TValue>(this IEnumerable<TValue> source, IEnumerable<TValue> other, Func<IEnumerable<TValue>, IEnumerable<TValue>, bool> compare)
        {
            return compare(source, other);
        }

        public static bool AreDuplicatesOf<TValue>(this IEnumerable<TValue> first, IEnumerable<TValue> second)
        {
            return first.Intersect(second).Count() == 0;
        }

        public static bool AreDuplicatesOf<TValue>(this IEnumerable<TValue> first, IEnumerable<TValue> second, IEqualityComparer<TValue> comparer)
        {
            return first.Intersect(second, comparer).Count() == 0;
        }

        public static bool AreUnique<TValue>(this IEnumerable<TValue> first, IEnumerable<TValue> second)
        {
            return first.Intersect(second).Count() == 0;
        }

        public static bool AreUnique<TValue>(this IEnumerable<TValue> first, IEnumerable<TValue> second, IEqualityComparer<TValue> comparer)
        {
            return first.Intersect(second, comparer).Count() == 0;
        }

        public static bool AreOfType<TValue>(this IEnumerable<object> values)
        {
            var equals = values.Select(x => x.GetType() == typeof(TValue)).ToArray();
            return !equals.Contains(false);
        }

        public static IEnumerable<TValue> Duplicates<TValue>(this IEnumerable<TValue> first, IEnumerable<TValue> second)
        {
            return second.Intersect(first);
        }

        public static IEnumerable<TValue> Duplicates<TValue>(this IEnumerable<TValue> first, IEnumerable<TValue> second, IEqualityComparer<TValue> comparer)
        {
            return second.Intersect(first, comparer).ToList();
        }

        public static IEnumerable<TValue> NonDuplicates<TValue>(this IEnumerable<TValue> first, IEnumerable<TValue> second)
        {
            return (second.Union(first)).Except(second.Intersect(first));
        }

        public static IEnumerable<TValue> NonDuplicates<TValue>(this IEnumerable<TValue> first, IEnumerable<TValue> second, IEqualityComparer<TValue> comparer)
        {
            return (second.Union(first, comparer)).Except(second.Intersect(first, comparer), comparer);
        }

        public static void AddRange<TValue>(this List<TValue> list, IEnumerable<TValue> value, Expression<Func<TValue, bool>> predicate)
        {
            list.AddRange(value.Where(predicate.Compile()));
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
