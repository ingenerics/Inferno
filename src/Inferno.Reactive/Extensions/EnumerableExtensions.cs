using System;
using System.Collections.Generic;
using System.Linq;

namespace Inferno
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> @this, Action<T> action)
        {
            foreach (var v in @this)
            {
                action(v);
            }
        }

        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> @this, int count)
        {
            return @this.Take(@this.Count() - count);
        }
    }
}