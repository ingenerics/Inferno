// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the ReactiveUI project root for full license information.

// This file may have been changed since it was pulled from the ReactiveUI repository.

using System;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;

namespace Inferno
{
    /*
     * From MSDN https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/object-lifetime-events:
     * The Loaded event is raised as a coordinated effort throughout the entire element tree (specifically, the logical tree).
     * When all elements in the tree are in a state where they are considered loaded, the Loaded event is first raised on the root element.
     * The Loaded event is then raised successively on each child element.
     */

    /// <summary>
    /// LoadedForViewFetcher is how Inferno determines which lifecycle state the FrameworkElement is in,
    /// if it's loaded or unloaded (ie when an element is added to, resp. removed from the visual tree).
    /// </summary>
    public class LoadedForViewFetcher : ILoadedForViewFetcher
    {
        /// <inheritdoc/>
        public int GetAffinityForView(Type view)
        {
            return typeof(FrameworkElement).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo()) ? 10 : 0;
        }

        /// <inheritdoc/>
        public IObservable<bool> GetLoadedForView(object view)
        {
            var fe = view as FrameworkElement;

            if (fe == null)
            {
                return Observable<bool>.Empty;
            }

            var viewLoaded = Observable.FromEvent<RoutedEventHandler, bool>(
                eventHandler =>
                {
                    void Handler(object sender, RoutedEventArgs e) => eventHandler(true);
                    return Handler;
                },
                x => fe.Loaded += x,
                x => fe.Loaded -= x);

            var viewUnloaded = Observable.FromEvent<RoutedEventHandler, bool>(
                eventHandler =>
                {
                    void Handler(object sender, RoutedEventArgs e) => eventHandler(false);
                    return Handler;
                },
                x => fe.Unloaded += x,
                x => fe.Unloaded -= x);

            return viewLoaded
                .Merge(viewUnloaded)
                .DistinctUntilChanged();
        }
    }
}
