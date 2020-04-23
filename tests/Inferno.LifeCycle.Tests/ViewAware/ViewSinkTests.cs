// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

using System;
using System.Reactive.Concurrency;
using DynamicData;
using Xunit;

namespace Inferno.LifeCycle.Tests
{
    public class ScreenActivatorTests
    {
        [Fact]
        public void LoadingTicksOnViewLoadedObservable()
        {
            var viewSink = new ViewSink();
            viewSink.OnViewLoaded.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var loaded).Subscribe();

            viewSink.ViewLoaded();

            Assert.Equal(1, loaded.Count);
        }

        [Fact]
        public void UnloadingIgnoringRefCountTicksOnViewUnloadedObservable()
        {
            var viewSink = new ViewSink();
            viewSink.OnViewUnloaded.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var unloaded).Subscribe();

            viewSink.ViewUnloaded(true);

            Assert.Equal(1, unloaded.Count);
        }

        [Fact]
        public void IgnoringCountDoesntTickOnViewUnloadedObservable()
        {
            var viewSink = new ViewSink();
            viewSink.OnViewUnloaded.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var unloaded).Subscribe();

            viewSink.ViewUnloaded(false);

            Assert.Equal(0, unloaded.Count);
        }

        [Fact]
        public void UnloadingFollowingLoadingTicksOnViewUnloadedObservable()
        {
            var viewSink = new ViewSink();
            viewSink.OnViewUnloaded.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var unloaded).Subscribe();

            viewSink.ViewLoaded();
            viewSink.ViewUnloaded(false);

            Assert.Equal(1, unloaded.Count);
        }

        [Fact]
        public void DisposingAfterLoadingTicksOnViewUnloadedObservable()
        {
            var viewSink = new ViewSink();
            viewSink.OnViewLoaded.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var loaded).Subscribe();
            viewSink.OnViewUnloaded.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var unloaded).Subscribe();

            using (viewSink.ViewLoaded())
            {
                Assert.Equal(1, loaded.Count);
                Assert.Equal(0, unloaded.Count);
            }

            Assert.Equal(1, loaded.Count);
            Assert.Equal(1, unloaded.Count);
        }
    }
}
