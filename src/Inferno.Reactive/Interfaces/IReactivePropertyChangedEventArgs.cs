﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

namespace Inferno
{
    /// <summary>
    /// IReactivePropertyChangedEventArgs is a generic interface that is used to wrap the NotifyPropertyChangedEventArgs and 
    /// gives information about changed properties. This includes the sender of the notification.
    /// Note that it is used for both Changing (i.e.'before change') and Changed Observables.
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    public interface IReactivePropertyChangedEventArgs<out TSender>
    {
        /// <summary>
        /// Gets the name of the property that has changed on Sender.
        /// </summary>
        string PropertyName { get; }

        /// <summary>
        /// Gets the object that has raised the change.
        /// </summary>
        TSender Sender { get; }
    }
}