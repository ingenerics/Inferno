using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace Inferno.Testing
{
    public static class EnumerableTestExtensions
    {
        public static void AssertAreEqual<T>(this IEnumerable<T> lhs, IEnumerable<T> rhs)
        {
            var left = lhs.ToArray();
            var right = rhs.ToArray();

            try
            {
                Assert.Equal(left.Length, right.Length);
                for (var i = 0; i < left.Length; i++)
                {
                    Assert.Equal(left[i], right[i]);
                }
            }
            catch
            {
                Debug.WriteLine("lhs: [{0}]", string.Join(",", lhs.ToArray()));
                Debug.WriteLine("rhs: [{0}]", string.Join(",", rhs.ToArray()));
                throw;
            }
        }

        public static IEnumerable<T> DistinctUntilChanged<T>(this IEnumerable<T> @this)
        {
            var isFirst = true;
            var lastValue = default(T);

            foreach (var v in @this)
            {
                if (isFirst)
                {
                    lastValue = v;
                    isFirst = false;
                    yield return v;
                    continue;
                }

                if (!EqualityComparer<T>.Default.Equals(v, lastValue))
                {
                    yield return v;
                }

                lastValue = v;
            }
        }
    }
}
