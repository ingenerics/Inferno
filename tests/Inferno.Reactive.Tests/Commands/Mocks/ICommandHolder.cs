﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

using System.Windows.Input;

namespace Inferno.Reactive.Tests
{
    public class ICommandHolder : ReactiveObject
    {
        private ICommand _theCommand;

        public ICommand TheCommand
        {
            get => _theCommand;
            set => this.RaiseAndSetIfChanged(ref _theCommand, value);
        }
    }
}