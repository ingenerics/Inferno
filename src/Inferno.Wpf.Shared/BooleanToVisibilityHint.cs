// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Inferno
{
    /// <summary>
    /// Enum that hints at the visibility of a ui element.
    /// </summary>
    [SuppressMessage("Name", "CA1714: Flags enums should have plural names", Justification = "For legacy support")]
    [Flags]
    public enum BooleanToVisibilityHint
    {
        /// <summary>
        /// Do not modify the boolean type conversion from it's default action of using the Visibility.Collapsed.
        /// </summary>
        None = 0,

        /// <summary>
        /// Inverse the action of the boolean type conversion, when it's true collapse the visibility.
        /// </summary>
        Inverse = 1 << 1,

        /// <summary>
        /// Use the hidden version rather than the Collapsed.
        /// </summary>
        UseHidden = 1 << 2,
    }
}