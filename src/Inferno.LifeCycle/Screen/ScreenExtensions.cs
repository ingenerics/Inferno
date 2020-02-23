using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inferno
{
    /// <summary>
    /// Hosts extension methods for <see cref="IScreen"/> classes.
    /// </summary>
    public static class ScreenExtensions
    {
        /// <summary>
        /// Activates the item if it implements <see cref="IActivate"/>, otherwise does nothing.
        /// </summary>
        /// <param name="potentialActivatable">The potential activatable.</param>
        public static Task TryActivateAsync(object potentialActivatable)
        {
            return potentialActivatable is IActivate activatable ? activatable.ActivateAsync(CancellationToken.None) : Task.FromResult(true);
        }

        /// <summary>
        /// Activates the item if it implements <see cref="IActivate"/>, otherwise does nothing.
        /// </summary>
        /// <param name="potentialActivatable">The potential activatable.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task TryActivateAsync(object potentialActivatable, CancellationToken cancellationToken)
        {
            return potentialActivatable is IActivate activatable ? activatable.ActivateAsync(cancellationToken) : Task.FromResult(true);
        }

        /// <summary>
        /// Deactivates the item if it implements <see cref="IActivate"/>, otherwise does nothing.
        /// </summary>
        /// <param name="potentialActivatable">The potential activatable.</param>
        /// <param name="close">Indicates whether or not to close the item after deactivating it.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task TryDeactivateAsync(object potentialActivatable, bool close, CancellationToken cancellationToken)
        {
            return potentialActivatable is IActivate activatable ? activatable.DeactivateAsync(close, cancellationToken) : Task.FromResult(true);
        }

        /// <summary>
        /// Closes the specified item.
        /// </summary>
        /// <param name="conductor">The conductor.</param>
        /// <param name="item">The item to close.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task CloseItemAsync(this IConductor conductor, object item, CancellationToken cancellationToken)
            => conductor.DeactivateItemAsync(item, true, cancellationToken);

        /// <summary>
        /// Closes the specified item.
        /// </summary>
        /// <param name="conductor">The conductor.</param>
        /// <param name="item">The item to close.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task CloseItemAsync<T>(this ConductorBase<T> conductor, T item, CancellationToken cancellationToken) where T : class
            => conductor.DeactivateItemAsync(item, true, cancellationToken);

        ///<summary>
        /// Activates a child whenever the specified parent is activated
        /// and deactivates the child whenever the specified parent is deactivated.
        ///</summary>
        ///<param name="child">The child to (de)activate.</param>
        ///<param name="parent">The parent whose activation triggers the child's activation.</param>
        public static void ActivateWith(this IActivate child, IActivate parent)
        {
            if (parent == null) return;

            // Note on the disposal, in a hierarchical composition we'll always want to close the child(ren) first.
            // Eg in a multilevel tree the leaf nodes will be closed first, working our way back to the root.

            child.Activator.AddDisposables(
                parent.Activator
                    .Activated
                    .Select(_ => child.ActivateAsync(CancellationToken.None))
                    .Subscribe(),
                parent.Activator
                    .Deactivated
                    .Select(wasClosed => child.DeactivateAsync(wasClosed, CancellationToken.None))
                    .Subscribe()
                );
        }

        ///<summary>
        /// Activates and Deactivates a child whenever the specified parent is Activated or Deactivated.
        ///</summary>
        ///<param name="child">The child to activate/deactivate.</param>
        ///<param name="parent">The parent whose activation/deactivation triggers the child's activation/deactivation.</param>
        public static void ConductWith<TChild, TParent>(this TChild child, TParent parent)
            where TChild : IActivate
            where TParent : IActivate
        {
            child.ActivateWith(parent);
        }
    }
}
