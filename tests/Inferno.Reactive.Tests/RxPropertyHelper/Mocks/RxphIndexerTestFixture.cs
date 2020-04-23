// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

namespace Inferno.Reactive.Tests
{
    internal class RxphIndexerTestFixture : ReactiveObject
    {
        private string _text;

        public RxphIndexerTestFixture()
        {
            var temp = this.WhenAnyValue(f => f.Text)
                           .ToProperty(this, f => f["Whatever"])
                           .Value;
        }

        public string Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }

        public string this[string propertyName] => string.Empty;
    }
}
