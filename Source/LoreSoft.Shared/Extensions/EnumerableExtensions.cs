using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LoreSoft.Shared.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool Contains<TSource>(this IEnumerable<TSource> enumerable, Func<TSource, bool> function)
        {
            var a = enumerable.FirstOrDefault(function);
            var b = default(TSource);
            return !Equals(a, b);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.DistinctBy(keySelector, EqualityComparer<TKey>.Default);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (keySelector == null)
                throw new ArgumentNullException("keySelector");
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            return DistinctByImpl(source, keySelector, comparer);
        }

        private static IEnumerable<TSource> DistinctByImpl<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            var knownKeys = new HashSet<TKey>(comparer);
            foreach (var element in source)
                if (knownKeys.Add(keySelector(element)))
                    yield return element;
        }

        public static void AddRange<TSource>(this ICollection<TSource> list, IEnumerable<TSource> range)
        {
            foreach (var r in range)
                list.Add(r);
        }

        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            foreach (var item in source)
                action(item);
        }

        public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> items)
        {
            return items == null || !items.Any();
        }

        public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> selector)
        {
            int index = 0;
            foreach (var item in source)
            {
                if (selector(item))
                    return index;

                index++;
            }

            // not found
            return -1;
        }

        private static readonly Random _random = new Random();

        public static TSource Random<TSource>(this IEnumerable<TSource> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            int count = items.Count();
            if (count == 0)
                return default(TSource);

            int index = _random.Next(0, count);
            return items.ElementAt(index);
        }

        public static TSource Random<TSource>(this IEnumerable<TSource> items, Func<TSource, int> weightSelector)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            // if no weight delegate, use standard random
            if (weightSelector == null)
                return items.Random();

            // total weight
            int total = items.Sum(weightSelector);
            if (total == 0)
                return default(TSource);

            // random index between 0 and total weight
            int index = _random.Next(0, total);
            int lastWeight = 0;

            // each item is given a range based on weight
            foreach (var item in items)
            {
                int weight = weightSelector(item);

                int start = lastWeight;
                int end = lastWeight + weight;

                // if index is between last weight and this weight, return
                if (index.Between(start, end))
                    return item;

                lastWeight = end;
            }

            // shouldn't reach here
            return items.FirstOrDefault();
        }

        /// <summary>
        /// Converts an IEnumerable of values to a delimited string.
        /// </summary>
        /// <typeparam name="T">
        /// The type of objects to delimit.
        /// </typeparam>
        /// <param name="values">
        /// The IEnumerable string values to convert.
        /// </param>
        /// <param name="delimiter">
        /// The delimiter.
        /// </param>
        /// <returns>
        /// A delimited string of the values.
        /// </returns>
        public static string ToDelimitedString<T>(this IEnumerable<T> values, string delimiter)
        {
            var sb = new StringBuilder();
            foreach (var i in values)
            {
                if (sb.Length > 0)
                    sb.Append(delimiter);
                sb.Append(i.ToString());
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts an IEnumerable of values to a delimited string.
        /// </summary>
        /// <param name="values">The IEnumerable string values to convert.</param>
        /// <returns>A delimited string of the values.</returns>
        public static string ToDelimitedString(this IEnumerable<string> values)
        {
            return ToDelimitedString(values, ",");
        }

        /// <summary>
        /// Converts an IEnumerable of values to a delimited string.
        /// </summary>
        /// <param name="values">The IEnumerable string values to convert.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns>A delimited string of the values.</returns>
        public static string ToDelimitedString(this IEnumerable<string> values, string delimiter)
        {
            var sb = new StringBuilder();
            foreach (var i in values)
            {
                if (sb.Length > 0)
                    sb.Append(delimiter);
                sb.Append(i);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Creates a <see cref="T:System.Collections.Generic.HashSet`1"/> from an <see cref="T:System.Collections.Generic.IEnumerable`1"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1"/> to create a <see cref="T:System.Collections.Generic.HashSet`1"/> from.</param>
        /// <returns>A <see cref="T:System.Collections.Generic.HashSet`1"/> that contains elements from the input sequence.</returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        /// <summary>
        /// Creates a <see cref="T:System.Collections.Generic.HashSet`1"/> from an <see cref="T:System.Collections.Generic.IEnumerable`1"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1"/> to create a <see cref="T:System.Collections.Generic.HashSet`1"/> from.</param>
        /// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> to compare elements.</param>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.HashSet`1"/> that contains elements from the input sequence.
        /// </returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer)
        {
            return new HashSet<T>(source, comparer);
        }

        public static IEnumerable<IndexedItem<T>> ToIndexed<T>(this IEnumerable<T> items)
        {
            int index = 0;
            return items.Select(item => new IndexedItem<T>(index++, item));
        }

        public static IEnumerable<Node<T>> ToTree<T>(this IEnumerable<T> collection, Func<T, T> getParent)
          where T : class
        {
            var top = new Node<T>();

            var dic = new Dictionary<T, Node<T>>();

            Func<T, Node<T>> createNode = null;

            createNode = item => dic.GetOrAdd(item, k =>
            {
                var itemNode = new Node<T>(item);
                T parent = getParent(item);
                var parentNode = parent != null ? createNode(parent) : top;
                parentNode.Children.Add(itemNode);
                return itemNode;
            });

            foreach (var item in collection)
                createNode(item);

            return top.Children;
        }

        /// <summary>
        /// Creates a <see cref="T:System.Collections.ObjectModel.ObservableCollection`1"/> from an <see cref="T:System.Collections.Generic.IEnumerable`1"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1"/> to create a <see cref="T:System.Collections.ObjectModel.ObservableCollection`1"/> from.</param>
        /// <returns>A <see cref="T:System.Collections.ObjectModel.ObservableCollection`1"/> that contains elements from the input sequence.</returns>
        public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> source)
        {
            return new ObservableCollection<T>(source);
        }
    }

    public class IndexedItem<T>
    {
        public IndexedItem(int index, T item)
        {
            Index = index;
            Item = item;
        }

        public int Index { get; private set; }
        public T Item { get; private set; }
    }

    public class Node<T>
    {
        public T Value { get; set; }
        public List<Node<T>> Children { get; set; }

        public Node(T value)
        {
            Value = value;
            Children = new List<Node<T>>();
        }

        public Node()
        {
            Children = new List<Node<T>>();
        }
    }
}
