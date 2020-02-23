using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading;

namespace Inferno
{
    /// <summary>
    /// Views will internally call this class when they come on screen. Once you instantiate this class, use the
    /// WhenLoaded method to register what to do when loaded.
    ///
    /// Views are Loaded when they *enter* the Visual Tree, and are Unloaded when they *leave* the Visual Tree.
    /// It is critical to understand this when it comes to UI virtualization.
    ///
    /// NOTE: You **must** call WhenLoaded in the corresponding View when using WhenViewLoaded in the ViewModel.
    /// </summary>
    public sealed class ViewSink : IDisposable
    {
        private readonly List<Func<IEnumerable<IDisposable>>> _blocks;
        private readonly Subject<Unit> _onViewLoaded;
        private readonly Subject<Unit> _onViewUnloaded;

        private IDisposable _onLoadedHandle = Disposable.Empty;
        // There may be multiple views bound to the viewmodel referencing this sink.
        private int _refCount; 

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewSink"/> class.
        /// </summary>
        public ViewSink()
        {
            _blocks = new List<Func<IEnumerable<IDisposable>>>();
            _onViewLoaded = new Subject<Unit>();
            _onViewUnloaded = new Subject<Unit>();
        }

        /// <summary>
        /// Gets a observable which will tick every time the view is loaded.
        /// </summary>
        public IObservable<Unit> OnViewLoaded => _onViewLoaded;

        /// <summary>
        /// Gets a observable which will tick every time the view is unloaded.
        /// </summary>
        public IObservable<Unit> OnViewUnloaded => _onViewUnloaded;

        /// <summary>
        /// This method is called by the framework when the corresponding View is loaded.
        /// Call this method in unit tests to simulate a ViewModel's View being loaded.
        /// </summary>
        /// <returns>A Disposable that calls ViewUnloaded when disposed.</returns>
        public IDisposable ViewLoaded()
        {
            if (Interlocked.Increment(ref _refCount) == 1)
            {
                var disp = new CompositeDisposable(_blocks.SelectMany(x => x()));
                Interlocked.Exchange(ref _onLoadedHandle, disp).Dispose();
                _onViewLoaded.OnNext(Unit.Default);
            }

            return Disposable.Create(() => ViewUnloaded());
        }

        /// <summary>
        /// This method is called by the framework when the corresponding View is unloaded.
        /// </summary>
        /// <param name="ignoreRefCount">
        /// Force the VM to be unloaded, even if the View is loaded by more than one ViewModel.
        /// </param>
        public void ViewUnloaded(bool ignoreRefCount = false)
        {
            if (Interlocked.Decrement(ref _refCount) == 0 || ignoreRefCount)
            {
                Interlocked.Exchange(ref _onLoadedHandle, Disposable.Empty).Dispose();
                _onViewUnloaded.OnNext(Unit.Default);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _onLoadedHandle?.Dispose();
            _onViewLoaded?.Dispose();
            _onViewUnloaded?.Dispose();
        }

        /// <summary>
        /// Adds OnLoaded blocks to the list of registered blocks.
        /// These will called when the view is loaded, then disposed when it's unloaded.
        /// </summary>
        /// <param name="block">The block to add.</param>
        internal void AddOnLoadedBlock(Func<IEnumerable<IDisposable>> block)
        {
            _blocks.Add(block);
        }
    }
}
