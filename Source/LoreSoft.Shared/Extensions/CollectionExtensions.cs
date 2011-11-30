using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LoreSoft.Shared.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Removes the all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> of the items.</typeparam>
        /// <param name="collection">The collection to remove items from.</param>
        /// <param name="filter">The delegate that defines the conditions of the elements to remove.</param>
        /// <returns>The number of items removed.</returns>
        public static int RemoveAll<T>(this ICollection<T> collection, Func<T, bool> filter)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (filter == null)
                throw new ArgumentNullException("filter");

            var removed = collection.Where(filter).ToArray();
            removed.ForEach(e => collection.Remove(e));
            return removed.Length;
        }

        /// <summary>
        /// Resizes the specified list by adding or removing items.
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> of the items.</typeparam>
        /// <param name="list">The list to be resized.</param>
        /// <param name="newSize">The new size of the list.</param>
        /// <param name="newItem">The new item function.</param>
        public static void Resize<T>(this IList<T> list, int newSize, Func<T> newItem)
        {
            int count = list.Count;
            int diff = newSize - count;

            // sync collection
            if (diff > 0)
                for (int i = 0; i < diff; i++)
                    list.Add(newItem());
            else if (diff < 0)
                for (int i = count - 1; i >= count + diff; i--)
                    list.RemoveAt(i);
        }
    }
}
