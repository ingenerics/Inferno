// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Inferno.Reactive.Tests
{
    public class AwaiterTest
    {
        public AwaiterTest()
        {
            RxApp.Initialize(new FakeDependencyResolverReactive());
        }

        [Fact]
        public void AwaiterSmokeTest()
        {
            var fixture = AwaitAnObservable();
            fixture.Wait();

            Assert.Equal(42, fixture.Result);
        }

        private async Task<int> AwaitAnObservable()
        {
            var o = Observable.Start(
                () =>
                {
                    Thread.Sleep(1000);
                    return 42;
                },
                RxApp.TaskpoolScheduler);

            var ret = await o;
            return ret;
        }
    }
}
