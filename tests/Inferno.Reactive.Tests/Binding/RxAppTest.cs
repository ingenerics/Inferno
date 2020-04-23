// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

using System.Diagnostics;
using System.Reactive.Concurrency;
using Xunit;

namespace Inferno.Reactive.Tests
{
    public class RxAppTest
    {
        public RxAppTest()
        {
            RxApp.Initialize(new FakeDependencyResolverReactive());
        }

        [Fact]
        public void SchedulerShouldBeCurrentThreadInTestRunner()
        {
            Debug.WriteLine(RxApp.MainThreadScheduler.GetType().FullName);
            Assert.Equal(CurrentThreadScheduler.Instance, RxApp.MainThreadScheduler);
        }
    }
}
