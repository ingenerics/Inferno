﻿using Inferno.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Inferno
{
    /// <summary>
    /// Denotes an instance which conducts other objects by managing an ActiveItem and maintaining a strict lifecycle.
    /// </summary>
    /// <remarks>Conducted instances can opt in to the lifecycle by implementing any of the following <see cref="IActivate"/>, <see cref="IGuardClose"/>.</remarks>
    public interface IConductor : IReactiveObject, IParent
    {
        /// <summary>
        /// Activates the specified item.
        /// </summary>
        /// <param name="item">The item to activate.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        Task ActivateItemAsync(object item, CancellationToken cancellationToken);

        /// <summary>
        /// Deactivates the specified item.
        /// </summary>
        /// <param name="item">The item to close.</param>
        /// <param name="close">Indicates whether or not to close the item after deactivating it.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeactivateItemAsync(object item, bool close, CancellationToken cancellationToken);
    }

    /// <summary>
    /// An <see cref="IConductor"/> that also implements <see cref="IHaveActiveItem"/>.
    /// </summary>
    public interface IConductActiveItem : IConductor, IHaveActiveItem
    {
    }
}
