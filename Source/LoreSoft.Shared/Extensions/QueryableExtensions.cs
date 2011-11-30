using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoreSoft.Shared.Extensions
{
    public static class QueryableExtensions
    {
        public static Task<List<T>> ToListAsync<T>(this IQueryable<T> source)
        {
            var task = Task.Factory.StartNew(() => source.ToList());
            return task;
        }
    }
}