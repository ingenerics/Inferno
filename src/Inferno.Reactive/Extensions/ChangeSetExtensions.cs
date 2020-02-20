﻿using System;
using System.Reactive.Linq;
using DynamicData;

namespace Inferno
{
    /// <summary>
    /// Mixin associated with the DynamicData IChangeSet class.
    /// </summary>
    public static class ChangeSetMixin
    {
        /// <summary>
        /// Is the change set associated with a count change.
        /// </summary>
        /// <param name="changeSet">The change list to evaluate.</param>
        /// <returns>If the change set is caused by the count being changed.</returns>
        public static bool HasCountChanged(this IChangeSet changeSet)
        {
            if (changeSet == null)
            {
                throw new ArgumentNullException(nameof(changeSet));
            }

            return changeSet.Adds > 0 || changeSet.Removes > 0;
        }

        /// <summary>
        /// Is the change set associated with a count change.
        /// </summary>
        /// <param name="changeSet">The change list to evaluate.</param>
        /// <returns>An observable of changes that only have count changes.</returns>
        public static IObservable<IChangeSet> CountChanged(this IObservable<IChangeSet> changeSet)
        {
            return changeSet.Where(HasCountChanged);
        }

        /// <summary>
        /// Is the change set associated with a count change.
        /// </summary>
        /// <typeparam name="T">The change set type.</typeparam>
        /// <param name="changeSet">The change list to evaluate.</param>
        /// <returns>An observable of changes that only have count changes.</returns>
        public static IObservable<IChangeSet<T>> CountChanged<T>(this IObservable<IChangeSet<T>> changeSet)
        {
            return changeSet.Where(x => x.HasCountChanged());
        }
    }
}
