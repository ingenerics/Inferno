// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

using System;
using System.Windows.Input;

namespace Inferno.Reactive.Tests
{
    public class FakeCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public object CanExecuteParameter { get; private set; }

        public object ExecuteParameter { get; private set; }

        public bool CanExecute(object parameter)
        {
            CanExecuteParameter = parameter;
            return true;
        }

        public void Execute(object parameter)
        {
            ExecuteParameter = parameter;
        }

        protected virtual void NotifyCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged?.Invoke(this, e);
        }
    }
}
