using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inferno
{
    /// <summary>
    /// Starts with checking the last item and works its way forward.
    /// Doesn't check further once an item is found that can't close
    /// and returns only the closable items (if any) found so far.
    /// </summary>
    /// <typeparam name="T">The type of child element.</typeparam>
    public class OneByOneCloseStrategy<T> : ICloseStrategy<T>
    {
        /// <inheritdoc />
        public async Task<ICloseResult<T>> ExecuteAsync(T[] toClose, CancellationToken cancellationToken)
        {
            var closeable = new List<T>();
            var closeCanOccur = true;

            if (toClose.Length == 0) return new CloseResult<T>(closeCanOccur, closeable);

            var indexLast = toClose.Length - 1;

            do
            {
                var child = toClose[indexLast];

                if (child is IGuardClose guard)
                {
                    closeCanOccur = await guard.CanCloseAsync(cancellationToken);

                    if (closeCanOccur)
                    {
                        closeable.Add(child);
                    }
                }
                else
                {
                    closeable.Add(child);
                }

            } while (closeCanOccur && indexLast-- > 0);

            return new CloseResult<T>(closeCanOccur, closeable);
        }
    }
}
