// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

using System;
using System.Reactive.Linq;

namespace Inferno.LifeCycle.Tests
{
    public class TestLoadedForViewFetcher : ILoadedForViewFetcher
    {
        public int GetAffinityForView(Type view)
        {
            return typeof(ITestView).IsAssignableFrom(view) ? 100 : 0;
        }

        public IObservable<bool> GetLoadedForView(object view)
        {
            var tv = (ITestView)view;
            return tv.Loaded.Select(_ => true).Merge(tv.Unloaded.Select(_ => false));
        }
    }
}
