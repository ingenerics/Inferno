using Inferno.Core;
using System;
using System.Reactive;
using System.Reactive.Subjects;

namespace Inferno.LifeCycle.Tests
{
    public abstract class TestViewBase<T> : ReactiveObject, IViewFor<T>, ITestView, IDisposable where T : class
    {
        private T _viewModel;

        public Subject<Unit> Loaded { get; } = new Subject<Unit>();

        public Subject<Unit> Unloaded { get; } = new Subject<Unit>();

        public T ViewModel
        {
            get => _viewModel;
            set => this.RaiseAndSetIfChanged(ref _viewModel, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (T)value;
        }

        public void Dispose()
        {
            Loaded?.Dispose();
            Unloaded?.Dispose();
        }
    }

    /// <summary>
    /// Used by GetAffinityForView in TestLoadedForViewFetcher
    /// </summary>
    public interface ITestView
    {
        Subject<Unit> Loaded { get; }
        Subject<Unit> Unloaded { get; } 
    }
}
