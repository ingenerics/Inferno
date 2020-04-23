// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

using System;
using System.Collections.Generic;

namespace Inferno
{
    internal sealed class ChainedComparer<T> : IComparer<T>
    {
        private readonly IComparer<T> _parent;
        private readonly Comparison<T> _inner;

        public ChainedComparer(IComparer<T> parent, Comparison<T> comparison)
        {
            _parent = parent;
            _inner = comparison ?? throw new ArgumentNullException(nameof(comparison));
        }

        /// <inheritdoc />
        public int Compare(T x, T y)
        {
            var parentResult = _parent?.Compare(x, y) ?? 0;

            return parentResult != 0 ? parentResult : _inner(x, y);
        }
    }
}
