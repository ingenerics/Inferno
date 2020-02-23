using System.Threading;
using System.Threading.Tasks;

namespace Inferno
{
    /// <summary>
    /// Checks if all items can be closed, starting with the last element in the collection.
    /// If this isn't the case, the CloseResult's Children collection will be empty, and no items will be closed.
    /// </summary>
    /// <typeparam name="T">The type of child element.</typeparam>
    public class AllOrNoneCloseStrategy<T> : ICloseStrategy<T>
    {
        /// <inheritdoc />
        public async Task<ICloseResult<T>> ExecuteAsync(T[] toClose, CancellationToken cancellationToken)
        {
            var closeCanOccur = true;

            var indexLast = toClose.Length - 1;

            while (closeCanOccur && indexLast >= 0)
            {
                var child = toClose[indexLast];

                if (child is IGuardClose guard)
                {
                    closeCanOccur = await guard.CanCloseAsync(cancellationToken);
                }

                indexLast--;
            } 

            return new CloseResult<T>(closeCanOccur, closeCanOccur ? toClose : new T[0] );
        }
    }
}
