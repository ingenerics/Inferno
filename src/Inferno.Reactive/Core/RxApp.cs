using Inferno.Core;
using Inferno.Core.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;

namespace Inferno
{
    /*
     * ORIGINAL DESCRIPTION
     * ====================
     */
    /// <summary>
    /// The main registration point for common class instances throughout a ReactiveUI application.
    /// </summary>
    /// <remarks>
    /// N.B. Why we have this evil global class
    /// In a WPF or UWP application, most commands must have the Dispatcher
    /// scheduler set, because notifications will end up being run on another thread;
    /// this happens most often in a CanExecute observable. Unfortunately, in a Unit
    /// Test framework, while the MS Test Unit runner will *set* the Dispatcher (so
    /// we can't even use the lack of its presence to determine whether we're in a
    /// test runner or not), none of the items queued to it will ever be executed
    /// during the unit test.
    /// Initially, I tried to plumb the ability to set the scheduler throughout the
    /// classes, but when you start building applications on top of that, having to
    /// have *every single * class have a default Scheduler property is really
    /// irritating, with either default making life difficult.
    /// </remarks>
    /*
     * ADDENDUM
     * ========
     * Inferno takes a slightly different approach in the way the ExecutionMode is resolved:
     * - The ExecutionMode is mode is passed as an argument to the Initialize call.
     * - Inferno always expects the bootstrapper to have initialized RxApp by calling
     *   RxApp.Initialize(this, executionMode). This is where static classes and shared state
     *   for extension methods are configured. This needs to be done for all ExecutionModes. 
     * I considered moving all the configuration code to the Bootstrapper, but this way:
     *   -- RxApp functions as the gathering ground for all the reactive specific configurations.
     *   -- We still have a way to intercept, i.e. perform actions before calling RxApp.Initialize.
     *      In contrast to the approach taken by the ReactiveUI designers who trusted the initialization
     *      of components to their static constructors.
     */
    public static class RxApp
    {
        private static IDependencyResolver _dependencyResolver;
        private static ILogger _logger;

        /// <summary>
        /// The size of a small cache of items. Often used for the MemoizingMRUCache class.
        /// </summary>
        public const int SmallCacheLimit = 64;

        /// <summary>
        /// The size of a large cache of items. Often used for the MemoizingMRUCache class.
        /// </summary>
        public const int BigCacheLimit = 256;

        // N.B. The ThreadStatic dance here is for the unit test case -
        // often, each test will override MainThreadScheduler with their
        // own TestScheduler, and if this wasn't ThreadStatic, they would
        // stomp on each other, causing test cases to randomly fail,
        // then pass when you rerun them.
        [ThreadStatic]
        private static IScheduler _unitTestTaskpoolScheduler;
        private static IScheduler _taskpoolScheduler;
        [ThreadStatic]
        private static IScheduler _unitTestMainThreadScheduler;
        private static IScheduler _mainThreadScheduler;

        public static ExecutionMode ExecutionMode { get; private set; }

        /// <summary>
        /// Initializes static members of the <see cref="RxApp"/> class.
        /// </summary>
        /// <exception cref="UnhandledErrorException">Default exception when we have an unhandled exception.</exception>
        public static void Initialize(IDependencyResolver dependencyResolver, IScheduler mainThreadScheduler = null)
        {
            _dependencyResolver = dependencyResolver;
            _taskpoolScheduler = TaskPoolScheduler.Default;
            _logger = _dependencyResolver.GetInstance<ILogger>();

            ExecutionMode = mainThreadScheduler != null ? ExecutionMode.Dispatcher : ExecutionMode.UnitTest;

            switch (ExecutionMode)
            {
                case ExecutionMode.Dispatcher:
                    _logger.LogInformation("Initializing to normal mode");
                    _mainThreadScheduler = mainThreadScheduler;
                    SuppressViewCommandBindingMessage = true;
                    break;

                case ExecutionMode.UnitTest:
                    _logger.LogInformation("Detected Unit Test Runner, setting MainThreadScheduler to CurrentThread");
                    // For unit tests we need to set MainThreadScheduler to CurrentThread
                    _mainThreadScheduler = CurrentThreadScheduler.Instance;
                    break;

                default:
                    throw new NotImplementedException($"{nameof(ExecutionMode)} {ExecutionMode}");
            }

            DefaultExceptionHandler = Observer.Create<Exception>(ex =>
            {
                // NB: If you're seeing this, it means that an
                // RxPropertyHelper or the CanExecute of a
                // ReactiveCommand ended in an OnError. Instead of silently
                // breaking, Inferno will halt here if a debugger is attached.
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                MainThreadScheduler.Schedule(() => throw new UnhandledErrorException(
                    "An object implementing IHandleObservableErrors (often a ReactiveCommand or RxPropertyHelper) has errored, thereby breaking its observable pipeline. To prevent this, ensure the pipeline does not error, or Subscribe to the ThrownExceptions property of the object in question to handle the erroneous case.",
                    ex));
            });

            #region Configuration of static classes and shared state

            ReactiveLoggerExtensions.Initialize(_logger);
            IReactiveObjectExtensions.Initialize(_logger);

            var observableForPropertyFactories = _dependencyResolver.GetAllInstances<ICreatesObservableForProperty>();
            ReactiveNotifyPropertyChangedExtensions.Initialize(observableForPropertyFactories);

            var commandBinderImplementation = _dependencyResolver.GetInstance<ICommandBinderImplementation>();
            CommandBinder.Initialize(commandBinderImplementation);

            var commandBindingFactories = _dependencyResolver.GetAllInstances<ICreatesCommandBinding>();
            CreatesCommandBinding.Initialize(commandBindingFactories);

            var bindingTypeConverters = _dependencyResolver.GetAllInstances(typeof(IBindingTypeConverter)).OfType<IBindingTypeConverter>();
            var setMethodBindingConverters = _dependencyResolver.GetAllInstances(typeof(ISetMethodBindingConverter)).OfType<ISetMethodBindingConverter>();
            var propertyBindingHooks = _dependencyResolver.GetAllInstances(typeof(IPropertyBindingHook)).OfType<IPropertyBindingHook>();
            PropertyBinderImplementation.Initialize(_logger, bindingTypeConverters, setMethodBindingConverters, propertyBindingHooks);

            #endregion Configuration of static classes and shared state
        }

        /// <summary>
        /// Gets or sets a scheduler used to schedule work items that
        /// should be run "on the UI thread". In Dispatcher mode, this will be
        /// DispatcherScheduler, and in UnitTest mode this will be Immediate,
        /// to simplify writing common unit tests.
        /// </summary>
        public static IScheduler MainThreadScheduler
        {
            get => _unitTestMainThreadScheduler ?? _mainThreadScheduler;
            set
            {
                // N.B. The ThreadStatic dance here is for the unit test case -
                // often, each test will override MainThreadScheduler with their
                // own TestScheduler, and if this wasn't ThreadStatic, they would
                // stomp on each other, causing test cases to randomly fail,
                // then pass when you rerun them.
                if (ExecutionMode == ExecutionMode.UnitTest)
                {
                    _unitTestMainThreadScheduler = value;
                }
                else
                {
                    _mainThreadScheduler = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the a the scheduler used to schedule work items to
        /// run in a background thread. In both modes, this will run on the TPL
        /// Task Pool.
        /// </summary>
        public static IScheduler TaskpoolScheduler
        {
            get => _unitTestTaskpoolScheduler ?? _taskpoolScheduler;
            set
            {
                if (ExecutionMode == ExecutionMode.UnitTest)
                {
                    _unitTestTaskpoolScheduler = value;
                }
                else
                {
                    _taskpoolScheduler = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether log messages should be suppressed for command bindings in the view.
        /// </summary>
        public static bool SuppressViewCommandBindingMessage { get; set; }

        /// <summary>
        /// Gets or sets the Observer which signalled whenever an object that has a
        /// ThrownExceptions property doesn't Subscribe to that Observable. Use
        /// Observer.Create to set up what will happen - the default is to crash
        /// the application with an error message.
        /// </summary>
        public static IObserver<Exception> DefaultExceptionHandler { get; set; }
    }
}
