using Inferno.Core;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace Inferno.LifeCycle.Tests
{
    public sealed class LoadingView : ReactiveObject, IViewFor<ViewAwareViewModel>, IDisposable
    {
        private ViewAwareViewModel _viewModel;

        public LoadingView()
        {
            this.WhenLoaded(d =>
            {
                IsLoadedCount++;
                d(Disposable.Create(() => IsLoadedCount--));
            });
        }

        public Subject<Unit> Loaded { get; } = new Subject<Unit>();

        public Subject<Unit> Unloaded { get; } = new Subject<Unit>();

        public ViewAwareViewModel ViewModel
        {
            get => _viewModel;
            set => this.RaiseAndSetIfChanged(ref _viewModel, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (ViewAwareViewModel)value;
        }

        public int IsLoadedCount { get; set; }

        public void Dispose()
        {
            Loaded?.Dispose();
            Unloaded?.Dispose();
        }
    }
}
