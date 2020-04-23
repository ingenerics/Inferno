// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

using System;
using System.Windows.Input;

namespace Inferno
{
    /// <summary>
    /// Internal relay command used for Command binding.
    /// </summary>
    internal class RelayCommand : ICommand
    {
        private readonly Func<object, bool> _canExecute;
        private readonly Action<object> _execute;
        private bool? _prevCanExecute;

        public RelayCommand(Func<object, bool> canExecute = null, Action<object> execute = null)
        {
            _canExecute = canExecute ?? (_ => true);
            _execute = execute ?? (_ => { });
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            var ce = _canExecute(parameter);
            if (CanExecuteChanged != null && (!_prevCanExecute.HasValue || ce != _prevCanExecute))
            {
                CanExecuteChanged(this, EventArgs.Empty);
                _prevCanExecute = ce;
            }

            return ce;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
