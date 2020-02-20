﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;

namespace Inferno
{
    /// <summary>
    /// This class is the final fallback for WhenAny, and will simply immediately
    /// return the value of the type at the time it was created. It will also
    /// warn the user that this is probably not what they want to do.
    /// </summary>
    public class POCOObservableForProperty : ICreatesObservableForProperty
    {
        private static readonly IDictionary<(Type, string), bool> hasWarned = new ConcurrentDictionary<(Type, string), bool>();

        /// <inheritdoc/>
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            return 1;
        }

        /// <inheritdoc/>
        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, Expression expression, string propertyName, bool beforeChanged = false, bool suppressWarnings = false)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            var type = sender.GetType();
            if (!hasWarned.ContainsKey((type, propertyName)) && !suppressWarnings)
            {
                Debug.WriteLine($"The class {type.FullName} property {propertyName} is a POCO type and won't send change notifications, WhenAny will only return a single value!");
                hasWarned[(type, propertyName)] = true;
            }

            return Observable.Return(new ObservedChange<object, object>(sender, expression), RxApp.MainThreadScheduler)
                .Concat(Observable<IObservedChange<object, object>>.Never);
        }
    }
}