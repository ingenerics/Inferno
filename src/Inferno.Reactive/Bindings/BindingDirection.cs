// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

namespace Inferno
{
    /// <summary>
    /// The type of binding that the ReactiveBinding represents.
    /// </summary>
    public enum BindingDirection
    {
        /// <summary>The binding is updated only one way from the ViewModel.</summary>
        OneWay,

        /// <summary>The binding is updated from both the View and the ViewModel.</summary>
        TwoWay,

        /// <summary>The binding is updated asynchronously one way from the ViewModel.</summary>
        AsyncOneWay,
    }
}
