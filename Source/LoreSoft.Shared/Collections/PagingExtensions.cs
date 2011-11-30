using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoreSoft.Shared.Collections
{
    /// <summary>
    /// Paging extension methods.
    /// </summary>
    public static class PagingExtensions
    {
        #region IQueryable<T> extensions

        /// <summary>
        /// Converts the source to a <see cref="T:LoreSoft.Shared.Collections.PagedList`1"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list.</typeparam>
        /// <param name="source">The <see cref="T:System.Linq.IQueryable`1"/> source.</param>
        /// <param name="pageIndex">The zero based index of the page.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <returns>A new instance of <see cref="T:LoreSoft.Shared.Collections.PagedList`1"/>.</returns>
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize)
        {
            return new PagedList<T>(source, pageIndex, pageSize);
        }

        /// <summary>
        /// Converts the source to a <see cref="T:LoreSoft.Shared.Collections.PagedList`1"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list.</typeparam>
        /// <param name="source">The <see cref="T:System.Linq.IQueryable`1"/> source.</param>
        /// <param name="pageIndex">The zero based index of the page.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="totalCount">The total count.</param>
        /// <returns>A new instance of <see cref="T:LoreSoft.Shared.Collections.PagedList`1"/>.</returns>
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize, int totalCount)
        {
            return new PagedList<T>(source, pageIndex, pageSize, totalCount);
        }

        #endregion

        #region IEnumerable<T> extensions

        /// <summary>
        /// Converts the source to a <see cref="T:LoreSoft.Shared.Collections.PagedList`1"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list.</typeparam>
        /// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1"/> source.</param>
        /// <param name="pageIndex">The zero based index of the page.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <returns>A new instance of <see cref="T:LoreSoft.Shared.Collections.PagedList`1"/>.</returns>
        public static PagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
        {
            return new PagedList<T>(source, pageIndex, pageSize);
        }

        /// <summary>
        /// Converts the source to a <see cref="T:LoreSoft.Shared.Collections.PagedList`1"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list.</typeparam>
        /// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1"/> source.</param>
        /// <param name="pageIndex">The zero based index of the page.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="totalCount">The total count.</param>
        /// <returns>A new instance of <see cref="T:LoreSoft.Shared.Collections.PagedList`1"/>.</returns>
        public static PagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize, int totalCount)
        {
            return new PagedList<T>(source, pageIndex, pageSize, totalCount);
        }

        #endregion
    }
}
