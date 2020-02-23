using DynamicData;
using Inferno.Testing;
using System;
using System.Reactive.Concurrency;
using System.Windows;
using Xunit;

namespace Inferno.LifeCycle.Tests
{
    public class WpfSinkForActivatedViewFetcherTest
    {
        [StaFact]
        public void FrameworkElementIsActivatedAndDeactivated()
        {
            var uc = new WpfTestUserControl();
            var viewEvents = new SinkForLoadedViewFetcher();

            var obs = viewEvents.GetSinkForView(uc);
            obs.ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var loaded).Subscribe();

            var loadedEvent = new RoutedEventArgs();
            loadedEvent.RoutedEvent = FrameworkElement.LoadedEvent;

            uc.RaiseEvent(loadedEvent);

            new[] { true }.AssertAreEqual(loaded);

            var unloaded = new RoutedEventArgs();
            unloaded.RoutedEvent = FrameworkElement.UnloadedEvent;

            uc.RaiseEvent(unloaded);

            new[] { true, false }.AssertAreEqual(loaded);
        }

        [StaFact]
        public void IsHitTestVisibleDoesNotAddToSink()
        {
            var uc = new WpfTestUserControl();
            uc.IsHitTestVisible = false;
            var viewEvents = new SinkForLoadedViewFetcher();

            var obs = viewEvents.GetSinkForView(uc);
            obs.ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var loaded).Subscribe();

            var loadedEvent = new RoutedEventArgs();
            loadedEvent.RoutedEvent = FrameworkElement.LoadedEvent;

            uc.RaiseEvent(loadedEvent);

            // OnViewLoaded has happened.
            new[] { true }.AssertAreEqual(loaded);

            uc.IsHitTestVisible = true;

            // IsHitTestVisible true, we don't want this to influence our event sink.
            new[] { true }.AssertAreEqual(loaded);

            var unloaded = new RoutedEventArgs();
            unloaded.RoutedEvent = FrameworkElement.UnloadedEvent;

            uc.RaiseEvent(unloaded);

            // We had both a loaded/hit test visible change/unloaded happen.
            new[] { true, false }.AssertAreEqual(loaded);
        }
    }
}
