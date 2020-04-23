// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

using System;
using System.Windows;
using System.Windows.Input;
using Xunit;

namespace Inferno.Reactive.Tests
{
    public class WpfCommandBindingImplementationTests
    {
        public WpfCommandBindingImplementationTests()
        {
            RxApp.Initialize(new FakeDependencyResolverReactive());
        }

        [StaFact]
        public void CommandBindToExplicitEventWireup()
        {
            var vm = new CommandBindingViewModel();
            var view = new CommandBindingView { ViewModel = vm };

            var invokeCount = 0;
            vm.Command2.Subscribe(_ => invokeCount++);

            var disp = view.BindCommand(vm, x => x.Command2, x => x.Command2, "MouseUp");

            view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });

            disp.Dispose();

            view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
            Assert.Equal(1, invokeCount);
        }
    }
}