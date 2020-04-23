// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

using Xunit;

namespace Inferno.LifeCycle.Tests
{
    public class ViewAwareViewModelTests
    {
        [Fact]
        public void LoadingGetRefCounted()
        {
            var fixture = new ViewAwareViewModel();
            Assert.Equal(0, fixture.IsLoadedCount);

            fixture.View.ViewLoaded();
            Assert.Equal(1, fixture.IsLoadedCount);

            fixture.View.ViewLoaded();
            Assert.Equal(1, fixture.IsLoadedCount);

            fixture.View.ViewUnloaded();
            Assert.Equal(1, fixture.IsLoadedCount);

            // Refcount drops to zero
            fixture.View.ViewUnloaded();
            Assert.Equal(0, fixture.IsLoadedCount);
        }

        [Fact]
        public void DerivedLoadingsDontGetStomped()
        {
            var fixture = new DerivedViewAwareViewModel();
            Assert.Equal(0, fixture.IsLoadedCount);
            Assert.Equal(0, fixture.IsLoadedCountAlso);

            fixture.View.ViewLoaded();
            Assert.Equal(1, fixture.IsLoadedCount);
            Assert.Equal(1, fixture.IsLoadedCountAlso);

            fixture.View.ViewLoaded();
            Assert.Equal(1, fixture.IsLoadedCount);
            Assert.Equal(1, fixture.IsLoadedCountAlso);

            fixture.View.ViewUnloaded();
            Assert.Equal(1, fixture.IsLoadedCount);
            Assert.Equal(1, fixture.IsLoadedCountAlso);

            fixture.View.ViewUnloaded();
            Assert.Equal(0, fixture.IsLoadedCount);
            Assert.Equal(0, fixture.IsLoadedCountAlso);
        }
    }
}
