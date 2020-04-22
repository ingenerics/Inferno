using Inferno.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Inferno
{
    /// <summary>
    /// A base implementation of <see cref = "IScreen" />.
    /// </summary>
    public class Screen : ReactiveObject, IScreen, IViewAware, IChild
    {
        private string _displayName;
        private object _parent;
        private bool _isInitialized;
        private bool _isActive;
        private bool _isClosed;

        /// <summary>
        /// Creates an instance of the screen.
        /// </summary>
        public Screen()
        {
            _displayName = GetType().FullName;

            View = new ViewSink();
            Activator = new Activator();
        }

        /// <summary>
        /// Gets or Sets the Display Name
        /// </summary>
        public virtual string DisplayName
        {
            get => _displayName;
            set => this.RaiseAndSetIfChanged(ref _displayName, value);
        }

        /// <summary>
        /// Gets or Sets the Parent <see cref = "IConductor" />
        /// </summary>
        public virtual object Parent
        {
            get => _parent;
            set => this.RaiseAndSetIfChanged(ref _parent, value);
        }

        #region IViewAware

        /// <summary>
        /// A sink where the view can post messages.
        /// </summary>
        public ViewSink View { get; }

        #endregion IViewAware

        #region IActivate

        /// <summary>
        /// Indicates whether or not this instance has been initialized.
        /// </summary>
        public bool IsInitialized
        {
            get => _isInitialized;
            private set => this.RaiseAndSetIfChanged(ref _isInitialized, value);
        }

        /// <summary>
        /// Indicates whether or not this instance is currently active.
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            private set => this.RaiseAndSetIfChanged(ref _isActive, value);
        }

        /// <summary>
        /// Indicates if this instance is closed (ie its lifecycle is terminated) and
        /// the plumbing resources have been disposed of.
        /// </summary>
        public bool IsClosed
        {
            get => _isClosed;
            private set => this.RaiseAndSetIfChanged(ref _isClosed, value);
        }

        /// <summary>
        /// Encapsulates an item's initialization and (de)activation hooks.
        /// </summary>
        public Activator Activator { get; }

        async Task IActivate.ActivateAsync(CancellationToken cancellationToken)
        {
            if (IsActive)
                return;

            var initialized = false;

            if (!IsInitialized)
            {
                this.LogInformation(this, $"Initializing { this }");
                await OnInitializeAsync(cancellationToken);
                IsInitialized = initialized = true;
            }

            this.LogInformation(this, $"Activating { this }");
            await OnActivateAsync(cancellationToken); // Recursively called on children (when using conductors)

            IsActive = true;

            if (initialized)
            {
                // Execute WhenInitialized blocks
                Activator.Initialize();
            }

            // Execute WhenActivated blocks
            Activator.Activate();
        }

        async Task IActivate.DeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            if (IsActive || IsInitialized && close)
            {
                this.LogInformation(this, $"Deactivating { this }");
                await OnDeactivateAsync(close, cancellationToken);

                // Dispose WhenActivated subscriptions
                Activator.Deactivate(close);

                IsActive = false;

                if (close)
                {
                    View.Dispose();
                    Activator.Dispose();
                    IsClosed = true;
                    this.LogInformation(this, $"Closed { this }");
                }
            }
        }

        #region Customizable LifeCycle

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected virtual Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected virtual Task OnActivateAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Called when deactivating.
        /// </summary>
        /// <param name = "close">Indicates whether this instance will be closed.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Called to check whether or not this instance can close.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation and holds the value of the close check..</returns>
        public virtual Task<bool> CanCloseAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Tries to close this instance by asking its Parent to initiate shutdown.
        /// </summary>
        public virtual async Task TryCloseAsync()
        {
            if (Parent is IConductor conductor)
            {
                await conductor.CloseItemAsync(this, CancellationToken.None);
            }
        }

        #endregion Customizable LifeCycle

        #endregion IActivate

        public override string ToString() => DisplayName ?? base.ToString();
    }
}
