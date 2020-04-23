// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

using System;
using System.Linq;
using Xunit;

namespace Inferno.Reactive.Tests
{
    public class NewGameViewModelTests
    {
        private readonly NewGameViewModel _viewmodel;

        public NewGameViewModelTests()
        {
            RxApp.Initialize(new FakeDependencyResolverReactive());

            _viewmodel = new NewGameViewModel();
        }

        [Fact]
        public void CanAddUpToSevenPlayers()
        {
            foreach (var i in Enumerable.Range(1, 7))
            {
                _viewmodel.NewPlayerName = "Player" + i;
                _viewmodel.AddPlayer.Execute().Subscribe();
                Assert.Equal(i, _viewmodel.Players.Count);
            }
        }
    }
}
