// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

using System;

namespace Inferno
{
    /// <summary>
    /// This interface is implemented by Inferno objects which are given IObservables as input - when the input IObservables OnError, 
    /// instead of disabling the Inferno object, we catch the IObservable and pipe it into this property.
    ///
    /// Normally this IObservable is implemented with a ScheduledSubject whose default Observer is RxApp.DefaultExceptionHandler - 
    /// this means, that if you aren't listening to ThrownExceptions and one appears, the exception will appear on the UI thread and 
    /// crash the application.
    /// </summary>
    public interface IHandleObservableErrors
    {
        /// <summary>
        /// Gets a observable which will fire whenever an exception would normally terminate the application
        /// internal state.
        /// </summary>
        IObservable<Exception> ThrownExceptions { get; }
    }
}