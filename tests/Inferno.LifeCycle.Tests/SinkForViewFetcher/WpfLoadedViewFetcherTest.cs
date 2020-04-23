// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

using DynamicData;
using Inferno.Testing;
using System;
using System.Reactive.Concurrency;
using System.Windows;
using FactAttribute = Xunit.WpfFactAttribute;

namespace Inferno.LifeCycle.Tests
{
    public class WpfLoadedViewFetcherTest
    {
        [Fact]
        public void FrameworkElementIsActivatedAndDeactivated()
        {
            var uc = new WpfTestUserControl();
            var viewEvents = new LoadedForViewFetcher();

            var obs = viewEvents.GetLoadedForView(uc);
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

        [Fact]
        public void IsHitTestVisibleDoesNotTriggerViewSink()
        {
            var uc = new WpfTestUserControl();
            uc.IsHitTestVisible = false;
            var viewEvents = new LoadedForViewFetcher();

            var obs = viewEvents.GetLoadedForView(uc);
            obs.ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var loaded).Subscribe();

            var loadedEvent = new RoutedEventArgs();
            loadedEvent.RoutedEvent = FrameworkElement.LoadedEvent;

            uc.RaiseEvent(loadedEvent);

            // OnViewLoaded has happened.
            new[] { true }.AssertAreEqual(loaded);

            uc.IsHitTestVisible = true;

            // IsHitTestVisible true, we don't want this to influence our view lifecycle.
            new[] { true }.AssertAreEqual(loaded);

            var unloaded = new RoutedEventArgs();
            unloaded.RoutedEvent = FrameworkElement.UnloadedEvent;

            uc.RaiseEvent(unloaded);

            // We had both a loaded/hit test visible change/unloaded happen.
            new[] { true, false }.AssertAreEqual(loaded);
        }
    }
}
