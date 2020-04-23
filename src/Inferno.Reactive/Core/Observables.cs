// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Inferno
{
    /// <summary>
    /// Provides commonly required, statically-allocated, pre-canned observables.
    /// </summary>
    public static class Observables
    {
        /// <summary>
        /// An observable that ticks a single, Boolean value of <c>true</c>.
        /// </summary>
        public static readonly IObservable<bool> True = Observable.Return(true);

        /// <summary>
        /// An observable that ticks a single, Boolean value of <c>false</c>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This observable is equivalent to <c>Observable&lt;bool&gt;.Default</c>, but is provided for convenience.
        /// </para>
        /// </remarks>
        public static readonly IObservable<bool> False = Observable.Return(false);

        /// <summary>
        /// An observable that ticks <c>Unit.Default</c> as a single value.</summary>
        /// <remarks>
        /// <para>
        /// This observable is equivalent to <c>Observable&lt;Unit&gt;.Default</c>, but is provided for convenience.
        /// </para>
        /// </remarks>
        public static readonly IObservable<Unit> Unit = Observable<Unit>.Default;
    }
}