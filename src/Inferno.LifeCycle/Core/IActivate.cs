using System.Threading;
using System.Threading.Tasks;

namespace Inferno
{
    /// <summary>
    /// Denotes an instance which requires activation.
    /// </summary>
    public interface IActivate
    {
        /// <summary>
        /// Indicates whether or not this instance has been initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Indicates whether or not this instance is currently active.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Encapsulates an item's initialization and (de)activation hooks.
        /// </summary>
        Activator Activator { get; }

        /// <summary>
        /// Activates this instance.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ActivateAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Deactivates this instance.
        /// </summary>
        /// <param name="close">Indicates whether or not this instance is being closed.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeactivateAsync(bool close, CancellationToken cancellationToken);
    }
}
