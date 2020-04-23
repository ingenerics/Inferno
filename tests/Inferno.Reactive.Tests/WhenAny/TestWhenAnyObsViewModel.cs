// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

using DynamicData;
using DynamicData.Binding;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Inferno.Reactive.Tests
{
    public class TestWhenAnyObsViewModel : ReactiveObject
    {
        private IObservable<IChangeSet<int>> _changes;

        private ObservableCollectionExtended<int> _myListOfInts;

        public TestWhenAnyObsViewModel()
        {
            Command1 = ReactiveCommand.CreateFromObservable<int, int>(Observable.Return, outputScheduler: ImmediateScheduler.Instance);
            Command2 = ReactiveCommand.CreateFromObservable<int, int>(Observable.Return, outputScheduler: ImmediateScheduler.Instance);
            Command3 = ReactiveCommand.CreateFromObservable<string, string>(Observable.Return, outputScheduler: ImmediateScheduler.Instance);
        }

        public IObservable<IChangeSet<int>> Changes
        {
            get => _changes;
            set => this.RaiseAndSetIfChanged(ref _changes, value);
        }

        public ReactiveCommand<int, int> Command1 { get; set; }

        public ReactiveCommand<int, int> Command2 { get; set; }

        public ReactiveCommand<string, string> Command3 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Test usage")]
        public ObservableCollectionExtended<int> MyListOfInts
        {
            get => _myListOfInts;
            set
            {
                this.RaiseAndSetIfChanged(ref _myListOfInts, value);
                Changes = MyListOfInts?.ToObservableChangeSet();
            }
        }
    }
}
