using Inferno.Core;
using Inferno.Core.Logging;
using System;
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

        /// <summary>
        /// Creates an instance of the screen.
        /// </summary>
        public Screen()
        {
            _displayName = GetType().FullName;

            View = new ViewSink();
            Activator = new Activator();
        }

        private static ILogger _logger;
        public static ILogger Logger
        {
            get => _logger;
            set => _logger = value ?? throw new NullReferenceException($"{nameof(Screen)}.{nameof(Logger)}");
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
        /// Virtualized in order to help with document oriented view models.
        /// </summary>
        public virtual bool IsInitialized
        {
            get => _isInitialized;
            set => this.RaiseAndSetIfChanged(ref _isInitialized, value);
        }

        /// <summary>
        /// Indicates whether or not this instance is currently active.
        /// Virtualized in order to help with document oriented view models.
        /// </summary>
        public virtual bool IsActive
        {
            get => _isActive;
            set => this.RaiseAndSetIfChanged(ref _isActive, value);
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
                await OnInitializeAsync(cancellationToken);
                IsInitialized = initialized = true;
            }

            Logger.LogInformation(this, $"Activating { this }");
            await OnActivateAsync(cancellationToken); // Recursively called on children (when using conductors)

            if (initialized)
            {
                // Execute WhenInitialized blocks
                Activator.Initialize();
            }

            // Execute WhenActivated blocks
            Activator.Activate();

            IsActive = true;
        }

        async Task IActivate.DeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            if (IsActive || IsInitialized && close)
            {
                Logger.LogInformation(this, $"Deactivating { this }");
                await OnDeactivateAsync(close, cancellationToken);

                // Dispose WhenActivated subscriptions
                Activator.Deactivate(close);

                IsActive = false;

                if (close)
                {
                    View.Dispose();
                    Activator.Dispose();
                    Logger.LogInformation(this, $"Closed { this }");
                }
            }
        }

        #endregion IActivate

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
        /// Tries to close this instance by asking its Parent to initiate shutdown or by asking its corresponding view to close.
        /// </summary>
        public virtual async Task TryCloseAsync()
        {
            if (Parent is IConductor conductor)
            {
                await conductor.CloseItemAsync(this, CancellationToken.None);
            }

            //var closeAction = PlatformProvider.Current.GetViewCloseAction(this, Views.Values, dialogResult);

            //await Execute.OnUIThreadAsync(async () => await closeAction(CancellationToken.None));
        }

        #endregion Customizable LifeCycle

        public override string ToString() => DisplayName ?? base.ToString();
    }
}
