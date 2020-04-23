// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Inferno
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> @this, Action<T> action)
        {
            foreach (var v in @this)
            {
                action(v);
            }
        }

        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> @this, int count)
        {
            return @this.Take(@this.Count() - count);
        }
    }
}