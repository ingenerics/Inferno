// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

using System.ComponentModel;
using Inferno.Testing;

namespace Inferno.Reactive.Tests
{
    public class NonReactiveINPCObject : INotifyPropertyChanged
    {
        private TestFixture _inpcProperty;

        public event PropertyChangedEventHandler PropertyChanged;

        public TestFixture InpcProperty
        {
            get => _inpcProperty;
            set
            {
                if (_inpcProperty == value)
                {
                    return;
                }

                _inpcProperty = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InpcProperty)));
            }
        }
    }
}