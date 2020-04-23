// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

using Inferno.Core;
using Inferno.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Inferno
{
    /// <summary>
    /// A set of extension methods to help wire up View and ViewModel loading.
    /// </summary>
    public static class ViewAwareExtensions
    {
        private static ILogger _logger;
        private static MemoizingMRUCache<Type, ILoadedForViewFetcher> _loadedFetcherCache;

        internal static void Initialize(ILogger logger, IEnumerable<ILoadedForViewFetcher> loadedForViewFetchers)
        {
            _logger = logger;

            _loadedFetcherCache =
                new MemoizingMRUCache<Type, ILoadedForViewFetcher>(
                    (t, _) =>
                        loadedForViewFetchers
                            .Aggregate((count: 0, viewFetcher: default(ILoadedForViewFetcher)), (acc, x) =>
                            {
                                int score = x.GetAffinityForView(t);
                                return score > acc.count ? (score, x) : acc;
                            }).viewFetcher, RxApp.SmallCacheLimit);
        }

        /// <summary>
        /// WhenLoaded allows you to register a Func to be called when a ViewModel's View is Loaded.
        /// </summary>
        /// <param name="item">Object that is view aware.</param>
        /// <param name="block">
        /// The method to be called when the corresponding View is loaded.
        /// It returns a list of Disposables that will be cleaned up when the View is unloaded.
        /// </param>
        public static void WhenLoaded(this IViewAware item, Func<IEnumerable<IDisposable>> block)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.View.AddOnLoadedBlock(block);
        }

        /// <summary>
        /// WhenLoaded allows you to register a Func to be called when a ViewModel's View is Loaded.
        /// </summary>
        /// <param name="item">Object that is view aware.</param>
        /// <param name="block">
        /// The method to be called when the corresponding View is loaded.
        /// The Action parameter (usually called 'd') allows you to register Disposables to be cleaned up
        /// when the View is unloaded (i.e. "d(someObservable.Subscribe());").
        /// </param>
        public static void WhenLoaded(this IViewAware item, Action<Action<IDisposable>> block)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.View.AddOnLoadedBlock(() =>
            {
                var ret = new List<IDisposable>();
                block(ret.Add);
                return ret;
            });
        }

        /// <summary>
        /// WhenLoaded allows you to register a Func to be called when a ViewModel's View is Loaded.
        /// </summary>
        /// <param name="item">Object that is view aware.</param>
        /// <param name="block">
        /// The method to be called when the corresponding View is loaded.
        /// The Action parameter (usually called 'disposables') allows
        /// you to collate all the disposables to be cleaned up during unloading.
        /// </param>
        public static void WhenLoaded(this IViewAware item, Action<CompositeDisposable> block)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.View.AddOnLoadedBlock(() =>
            {
                var d = new CompositeDisposable();
                block(d);
                return new[] { d };
            });
        }

        /// <summary>
        /// WhenLoaded allows you to register a Func to be called OnViewLoaded.
        /// </summary>
        /// <param name="item">Object that supports viewEvents.</param>
        /// <param name="block">
        /// The method to be called when the corresponding View is loaded.
        /// It returns a list of Disposables that will be cleaned up when the View is unloaded.
        /// </param>
        /// <returns>A Disposable that cleans up this registration.</returns>
        public static IDisposable WhenLoaded<T>(this IViewFor<T> item, Func<IEnumerable<IDisposable>> block) where T : class
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return item.WhenLoaded(block, null);
        }

        /// <summary>
        /// WhenLoaded allows you to register a Func to be called OnViewLoaded.
        /// </summary>
        /// <param name="item">Object that supports loading.</param>
        /// <param name="block">
        /// The method to be called when the corresponding View is loaded.
        /// It returns a list of Disposables that will be cleaned up when the View is unloaded.
        /// </param>
        /// <param name="view">
        /// The IViewFor will ordinarily also host the View Model, but in the event it is not,
        /// a class implementing <see cref="IViewFor&lt;T&gt;" /> can be supplied here.
        /// </param>
        /// <returns>A Disposable that cleans up this registration.</returns>
        public static IDisposable WhenLoaded<T>(this IViewFor<T> item, Func<IEnumerable<IDisposable>> block, IViewFor<T> view) where T : class
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var loadedFetcher = _loadedFetcherCache.Get(item.GetType());
            if (loadedFetcher == null)
            {
                throw new ArgumentException($"Don't know how to detect when {item.GetType().FullName} is loaded/unloaded, you may need to implement {nameof(ILoadedForViewFetcher)}");
            }

            var viewEvents = loadedFetcher.GetLoadedForView(item);

            var vmDisposable = Disposable.Empty;
            if ((view ?? item) is IViewFor v)
            {
                vmDisposable = HandleViewModelOnViewLoaded(v, viewEvents);
            }

            var viewDisposable =
                typeof(IActivate).IsAssignableFrom(typeof(T)) ?
                    HandleViewOnActivatedAndLoaded((view ?? item), block, viewEvents) :
                    HandleViewOnLoaded(block, viewEvents);

            return new CompositeDisposable(vmDisposable, viewDisposable);
        }

        /// <summary>
        /// WhenLoaded allows you to register a Func to be called OnViewLoaded.
        /// </summary>
        /// <param name="item">Object that supports loading.</param>
        /// <param name="block">
        /// The method to be called when the corresponding View is loaded.
        /// The Action parameter (usually called 'd') allows
        /// you to register Disposables to be cleaned up when the View is
        /// unloaded (i.e. "d(someObservable.Subscribe());").
        /// </param>
        /// <returns>A Disposable that cleans up this registration.</returns>
        public static IDisposable WhenLoaded<T>(this IViewFor<T> item, Action<Action<IDisposable>> block) where T : class
        {
            return item.WhenLoaded(block, null);
        }

        /// <summary>
        /// WhenLoaded allows you to register a Func to be called OnViewLoaded.
        /// </summary>
        /// <param name="item">Object that supports loading.</param>
        /// <param name="block">
        /// The method to be called when the corresponding View is loaded.
        /// The Action parameter (usually called 'd') allows
        /// you to register Disposables to be cleaned up when the View is
        /// unloaded (i.e. "d(someObservable.Subscribe());").
        /// </param>
        /// <param name="view">
        /// The IViewFor will ordinarily also host the View Model, but in the event it is not,
        /// a class implementing <see cref="IViewFor&lt;T&gt;" /> can be supplied here.
        /// </param>
        /// <returns>A Disposable that cleans up this registration.</returns>
        public static IDisposable WhenLoaded<T>(this IViewFor<T> item, Action<Action<IDisposable>> block, IViewFor<T> view) where T : class
        {
            return item.WhenLoaded(
                () =>
                {
                    var ret = new List<IDisposable>();
                    block(ret.Add);
                    return ret;
                }, view);
        }

        /// <summary>
        /// WhenLoaded allows you to register a Func to be called OnViewLoaded.
        /// </summary>
        /// <param name="item">Object that supports loading.</param>
        /// <param name="block">
        /// The method to be called when the corresponding View is loaded.
        /// The Action parameter (usually called 'd') allows
        /// you to register Disposables to be cleaned up when the View is
        /// unloaded (i.e. "d(someObservable.Subscribe());").
        /// The Action parameter (usually called 'disposables') allows
        /// you to collate all disposables that should be cleaned up during unloading.
        /// </param>
        /// <param name="view">
        /// The IViewFor will ordinarily also host the View Model, but in the event it is not,
        /// a class implementing <see cref="IViewFor&lt;T&gt;" /> can be supplied here.
        /// </param>
        /// <returns>A Disposable that cleans up this registration.</returns>
        public static IDisposable WhenLoaded<T>(this IViewFor<T> item, Action<CompositeDisposable> block, IViewFor<T> view = null) where T : class
        {
            return item.WhenLoaded(
                () =>
                {
                    var d = new CompositeDisposable();
                    block(d);
                    return new[] { d };
                }, view);
        }

        private static IDisposable HandleViewOnLoaded(Func<IEnumerable<IDisposable>> block, IObservable<bool> viewEvents)
        {
            var viewDisposable = new SerialDisposable();

            return new CompositeDisposable(
                viewEvents.Subscribe(loaded =>
                {
                    // NB: We need to make sure to respect ordering so that the cleanup happens before we invoke block again
                    viewDisposable.Disposable = Disposable.Empty;
                    if (loaded)
                    {
                        viewDisposable.Disposable = new CompositeDisposable(block());
                    }
                }),
                viewDisposable);
        }

        private static IDisposable HandleViewOnActivatedAndLoaded(IViewFor view, Func<IEnumerable<IDisposable>> block, IObservable<bool> viewEvents)
        {
            var viewVmDisposable = new SerialDisposable();
            var viewDisposable = new SerialDisposable();
            var vmDisposable = new SerialDisposable();

            return new CompositeDisposable(
                viewEvents.Subscribe(loaded =>
                {
                    if (loaded)
                    {
                        viewVmDisposable.Disposable = view.WhenAnyValue(x => x.ViewModel)
                            .Select(x => x as IActivate)
                            .Subscribe(activatable =>
                            {
                                if (activatable != null)
                                {
                                    vmDisposable.Disposable = activatable.WhenAnyValue(x => x.IsActive).Subscribe(isActive =>
                                    {
                                        // NB: We need to make sure to respect ordering so that the cleanup happens before we invoke block again
                                        viewDisposable.Disposable = Disposable.Empty;
                                        if (isActive)
                                        {
                                            viewDisposable.Disposable = new CompositeDisposable(block());
                                        }
                                    });
                                }
                                else
                                {
                                    vmDisposable.Disposable = Disposable.Empty;
                                    viewDisposable.Disposable = Disposable.Empty;
                                }
                            });
                    }
                    else
                    {
                        viewVmDisposable.Disposable = Disposable.Empty;
                        vmDisposable.Disposable = Disposable.Empty;
                        viewDisposable.Disposable = Disposable.Empty;
                    }
                }),
                viewDisposable,
                vmDisposable,
                viewVmDisposable);
        }

        private static IDisposable HandleViewModelOnViewLoaded(IViewFor view, IObservable<bool> viewEvents)
        {
            var vmDisposable = new SerialDisposable();
            var viewVmDisposable = new SerialDisposable();

            return new CompositeDisposable(
                viewEvents.Subscribe(loaded =>
                {
                    if (loaded)
                    {
                        _logger.LogInformation($"Binding {view.ViewModel?.GetType().ToString() ?? "null"} and {view}");

                        viewVmDisposable.Disposable = view.WhenAnyValue(x => x.ViewModel)
                            .Select(x => x as IViewAware)
                            .Subscribe(x =>
                            {
                                // NB: We need to make sure to respect ordering so that the cleanup happens before we execute ViewLoaded again
                                vmDisposable.Disposable = Disposable.Empty;
                                if (x != null)
                                {
                                    vmDisposable.Disposable = x.View.ViewLoaded();
                                }
                            });
                    }
                    else
                    {
                        viewVmDisposable.Disposable = Disposable.Empty;
                        vmDisposable.Disposable = Disposable.Empty;
                    }
                }),
                vmDisposable,
                viewVmDisposable);
        }
    }
}
