// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

using Inferno.Testing;

namespace Inferno.Reactive.Tests
{
    public class HostTestFixture : ReactiveObject
    {
        private TestFixture _child;

        private NonObservableTestFixture _pocoChild;

        private int _someOtherParam;

        public TestFixture Child
        {
            get => _child;
            set => this.RaiseAndSetIfChanged(ref _child, value);
        }

        public NonObservableTestFixture PocoChild
        {
            get => _pocoChild;
            set => this.RaiseAndSetIfChanged(ref _pocoChild, value);
        }

        public int SomeOtherParam
        {
            get => _someOtherParam;
            set => this.RaiseAndSetIfChanged(ref _someOtherParam, value);
        }
    }
}
