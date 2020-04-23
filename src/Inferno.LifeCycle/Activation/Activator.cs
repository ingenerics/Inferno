// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace Inferno
{
    /// <summary>
    /// Conductors will internally call this class when they need to manage the lifecycle of their children.
    /// Once you instantiate this class, use the WhenInitialized and WhenActivated methods to register blocks.
    /// </summary>
    public sealed class Activator : IDisposable
    {
        private readonly List<Func<IEnumerable<IDisposable>>> _initializationBlocks;
        private readonly List<Func<IEnumerable<IDisposable>>> _activationBlocks;
        private readonly Subject<Unit> _initialized;
        private readonly Subject<Unit> _activated;
        private readonly Subject<bool> _deactivated;
        private readonly CompositeDisposable _compositeDisposable;

        private IDisposable _activationHandle;
        private bool _isInitialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="Activator"/> class.
        /// </summary>
        public Activator()
        {
            _initializationBlocks = new List<Func<IEnumerable<IDisposable>>>();
            _activationBlocks = new List<Func<IEnumerable<IDisposable>>>();
            _initialized = new Subject<Unit>();
            _activated = new Subject<Unit>();
            _deactivated = new Subject<bool>();
            _activationHandle = Disposable.Empty;
            _compositeDisposable = new CompositeDisposable();
        }

        /// <summary>
        /// Gets a observable which will tick every time the item is activated.
        /// </summary>
        /// <value>The activated.</value>
        public IObservable<Unit> Activated => _activated;

        /// <summary>
        /// Gets a observable which will tick every time the item is deactivated.
        /// The observable will tick true when the item was closed after deactivation, and false otherwise.
        /// </summary>
        /// <value>True if the deactivated item was also closed.</value>
        public IObservable<bool> Deactivated => _deactivated;

        /// <summary>
        /// Gets a observable which will tick true when the item has been initialized and false otherwise.
        /// </summary>
        public IObservable<bool> HasInitialized =>
            _initialized
                .Select(_ => true)
                .StartWith(false)
                .Replay(1)
                .RefCount();

        /// <summary>
        /// Gets a observable which will tick true when the item is activated and false otherwise.
        /// </summary>
        public IObservable<bool> HasActivated =>
            Observable.Merge(
                    _activated.Select(_ => true),
                    _deactivated.Select(_ => false))
                .StartWith(false)
                .Replay(1)
                .RefCount();

        /// <summary>
        /// This method is called by the framework when the corresponding ViewModel is initialized.
        /// Call this method in unit tests to simulate a ViewModel being initialized.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) throw new InvalidOperationException($"{nameof(Activator)}.{nameof(Initialize)} can only be called once.");

            AddDisposable(new CompositeDisposable(_initializationBlocks.SelectMany(x => x())));
            _initialized.OnNext(Unit.Default);

            _isInitialized = true;
        }

        /// <summary>
        /// This method is called by the framework when the corresponding ViewModel is activated.
        /// Call this method in unit tests to simulate a ViewModel being activated.
        /// </summary>
        /// <returns>A Disposable that calls Deactivate when disposed.</returns>
        public IDisposable Activate()
        {
            if (!_isInitialized) throw new InvalidOperationException($"{nameof(Activator)}.{nameof(Initialize)} needs to be called before calling {nameof(Activator)}.{nameof(Activate)}.");

            var disp = new CompositeDisposable(_activationBlocks.SelectMany(x => x()));
            Interlocked.Exchange(ref _activationHandle, disp).Dispose();
            _activated.OnNext(Unit.Default);

            return Disposable.Create(() => Deactivate(false));
        }

        /// <summary>
        /// This method is called by the framework when the corresponding ViewModel is deactivated.
        /// Call this method in unit tests to simulate a ViewModel being deactivated.
        /// </summary>
        /// <param name="close">
        /// Indicates the item is being closed after deactivation.
        /// </param>
        public void Deactivate(bool close)
        {
            Interlocked.Exchange(ref _activationHandle, Disposable.Empty).Dispose();
            _deactivated.OnNext(close);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _compositeDisposable.Dispose();
            _activationHandle?.Dispose();
            _initialized.Dispose();
            _activated?.Dispose();
            _deactivated?.Dispose();
        }

        /// <summary>
        /// Adds action blocks to the list of registered blocks. These will be executed on initialization, then disposed when the activator is disposed of.
        /// </summary>
        /// <param name="block">The block to add.</param>
        internal void AddInitializationBlock(Func<IEnumerable<IDisposable>> block)
        {
            _initializationBlocks.Add(block);
        }

        /// <summary>
        /// Adds action blocks to the list of registered blocks. These will be executed on activation, then disposed on deactivation.
        /// </summary>
        /// <param name="block">The block to add.</param>
        internal void AddActivationBlock(Func<IEnumerable<IDisposable>> block)
        {
            _activationBlocks.Add(block);
        }

        /// <summary>
        /// Adds an IDisposable, which will be disposed of when the activator is disposed of.
        /// </summary>
        /// <param name="disposable">The disposable to add.</param>
        public void AddDisposable(IDisposable disposable)
        {
            _compositeDisposable.Add(disposable);
        }

        /// <summary>
        /// Adds IDisposables, which will be disposed of when the activator is disposed of.
        /// </summary>
        /// <param name="disposables">The disposables to add.</param>
        public void AddDisposables(params IDisposable[] disposables)
        {
            foreach (var disposable in disposables)
            {
                _compositeDisposable.Add(disposable);
            }
        }
    }
}
