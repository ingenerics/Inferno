using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reactive.Concurrency;

namespace Inferno
{
    /// <summary>
    /// A collection of helpers to aid working with observable properties.
    /// </summary>
    public static class RxPropertyHelperExtensions
    {
        /// <summary>
        /// Converts an Observable to an RxPropertyHelper and
        /// automatically provides the onChanged method to raise the property
        /// changed notification.
        /// </summary>
        /// <typeparam name="TObj">The object type.</typeparam>
        /// <typeparam name="TRet">The result type.</typeparam>
        /// <param name="target">
        /// The observable to convert to an RxPropertyHelper.
        /// </param>
        /// <param name="source">
        /// The ReactiveObject that has the property.
        /// </param>
        /// <param name="property">
        /// An Expression representing the property (i.e. <c>x => x.SomeProperty</c>).
        /// </param>
        /// <param name="initialValue">
        /// The initial value of the property.
        /// </param>
        /// <param name="deferSubscription">
        /// A value indicating whether the <see cref="RxPropertyHelper{T}"/>
        /// should defer the subscription to the <paramref name="target"/> source
        /// until the first call to <see cref="RxPropertyHelper{T}.Value"/>,
        /// or if it should immediately subscribe to the the <paramref name="target"/> source.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler that the notifications will be provided on - this should normally
        /// be a Dispatcher-based scheduler.
        /// </param>
        /// <returns>
        /// An initialized RxPropertyHelper; use this as the backing field
        /// for your property.
        /// </returns>
        public static RxPropertyHelper<TRet> ToProperty<TObj, TRet>(
            this IObservable<TRet> target,
            TObj source,
            Expression<Func<TObj, TRet>> property,
            TRet initialValue = default(TRet),
            bool deferSubscription = false,
            IScheduler scheduler = null)
            where TObj : class, IReactiveObject => source.ObservableToProperty(target, property, initialValue, deferSubscription, scheduler);

        /// <summary>
        /// Converts an Observable to an RxPropertyHelper and
        /// automatically provides the onChanged method to raise the property
        /// changed notification.
        /// </summary>
        /// <typeparam name="TObj">The onject type.</typeparam>
        /// <typeparam name="TRet">The result type.</typeparam>
        /// <param name="target">
        /// The observable to convert to an RxPropertyHelper.
        /// </param>
        /// <param name="source">
        /// The ReactiveObject that has the property.
        /// </param>
        /// <param name="property">
        /// An Expression representing the property (i.e. <c>x => x.SomeProperty</c>).
        /// </param>
        /// <param name="result">
        /// An out param matching the return value, provided for convenience.
        /// </param>
        /// <param name="initialValue">
        /// The initial value of the property.
        /// </param>
        /// <param name="deferSubscription">
        /// A value indicating whether the <see cref="RxPropertyHelper{T}"/>
        /// should defer the subscription to the <paramref name="target"/> source
        /// until the first call to <see cref="RxPropertyHelper{T}.Value"/>,
        /// or if it should immediately subscribe to the the <paramref name="target"/> source.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler that the notifications will be provided on - this should
        /// normally be a Dispatcher-based scheduler.
        /// </param>
        /// <returns>
        /// An initialized RxPropertyHelper; use this as the backing
        /// field for your property.
        /// </returns>
        public static RxPropertyHelper<TRet> ToProperty<TObj, TRet>(
            this IObservable<TRet> target,
            TObj source,
            Expression<Func<TObj, TRet>> property,
            out RxPropertyHelper<TRet> result,
            TRet initialValue = default(TRet),
            bool deferSubscription = false,
            IScheduler scheduler = null)
            where TObj : class, IReactiveObject
        {
            var ret = source.ObservableToProperty(target, property, initialValue, deferSubscription, scheduler);

            result = ret;
            return ret;
        }

        /// <summary>
        /// Converts an Observable to an RxPropertyHelper and
        /// automatically provides the onChanged method to raise the property
        /// changed notification.
        /// </summary>
        /// <typeparam name="TObj">The object type.</typeparam>
        /// <typeparam name="TRet">The result type.</typeparam>
        /// <param name="target">
        /// The observable to convert to an RxPropertyHelper.
        /// </param>
        /// <param name="source">
        /// The ReactiveObject that has the property.
        /// </param>
        /// <param name="property">
        /// The name of the property that has changed. Recommended for use with nameof() or a FODY.
        /// or a fody.
        /// </param>
        /// <param name="initialValue">
        /// The initial value of the property.
        /// </param>
        /// <param name="deferSubscription">
        /// A value indicating whether the <see cref="RxPropertyHelper{T}"/>
        /// should defer the subscription to the <paramref name="target"/> source
        /// until the first call to <see cref="RxPropertyHelper{T}.Value"/>,
        /// or if it should immediately subscribe to the the <paramref name="target"/> source.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler that the notifications will be provided on - this should normally
        /// be a Dispatcher-based scheduler.
        /// </param>
        /// <returns>
        /// An initialized RxPropertyHelper; use this as the backing field
        /// for your property.
        /// </returns>
        public static RxPropertyHelper<TRet> ToProperty<TObj, TRet>(
            this IObservable<TRet> target,
            TObj source,
            string property,
            TRet initialValue = default(TRet),
            bool deferSubscription = false,
            IScheduler scheduler = null)
            where TObj : class, IReactiveObject => source.ObservableToProperty(target, property, initialValue, deferSubscription, scheduler);

        /// <summary>
        /// Converts an Observable to an RxPropertyHelper and
        /// automatically provides the onChanged method to raise the property
        /// changed notification.
        /// </summary>
        /// <typeparam name="TObj">The object type.</typeparam>
        /// <typeparam name="TRet">The result type.</typeparam>
        /// <param name="target">
        /// The observable to convert to an RxPropertyHelper.
        /// </param>
        /// <param name="source">
        /// The ReactiveObject that has the property.
        /// </param>
        /// <param name="property">
        /// The name of the property that has changed. Recommended for use with nameof() or a FODY.
        /// </param>
        /// <param name="result">
        /// An out param matching the return value, provided for convenience.
        /// </param>
        /// <param name="initialValue">
        /// The initial value of the property.
        /// </param>
        /// <param name="deferSubscription">
        /// A value indicating whether the <see cref="RxPropertyHelper{T}"/>
        /// should defer the subscription to the <paramref name="target"/> source
        /// until the first call to <see cref="RxPropertyHelper{T}.Value"/>,
        /// or if it should immediately subscribe to the the <paramref name="target"/> source.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler that the notifications will be provided on - this should
        /// normally be a Dispatcher-based scheduler.
        /// </param>
        /// <returns>
        /// An initialized RxPropertyHelper; use this as the backing
        /// field for your property.
        /// </returns>
        public static RxPropertyHelper<TRet> ToProperty<TObj, TRet>(
            this IObservable<TRet> target,
            TObj source,
            string property,
            out RxPropertyHelper<TRet> result,
            TRet initialValue = default(TRet),
            bool deferSubscription = false,
            IScheduler scheduler = null)
            where TObj : class, IReactiveObject
        {
            result = source.ObservableToProperty(
                 target,
                 property,
                 initialValue,
                 deferSubscription,
                 scheduler);

            return result;
        }

        private static RxPropertyHelper<TRet> ObservableToProperty<TObj, TRet>(
            this TObj target,
            IObservable<TRet> observable,
            Expression<Func<TObj, TRet>> property,
            TRet initialValue = default(TRet),
            bool deferSubscription = false,
            IScheduler scheduler = null)
            where TObj : class, IReactiveObject
        {
            Contract.Requires(target != null);
            Contract.Requires(observable != null);
            Contract.Requires(property != null);

            Expression expression = Reflection.Rewrite(property.Body);

            if (expression.GetParent().NodeType != ExpressionType.Parameter)
            {
                throw new ArgumentException("Property expression must be of the form 'x => x.SomeProperty'");
            }

            var name = expression.GetMemberInfo().Name;
            if (expression is IndexExpression)
            {
                name += "[]";
            }

            return new RxPropertyHelper<TRet>(
                observable,
                _ => target.RaisingPropertyChanged(name),
                _ => target.RaisingPropertyChanging(name),
                initialValue,
                deferSubscription,
                scheduler);
        }

        private static RxPropertyHelper<TRet> ObservableToProperty<TObj, TRet>(
            this TObj target,
            IObservable<TRet> observable,
            string property,
            TRet initialValue = default(TRet),
            bool deferSubscription = false,
            IScheduler scheduler = null)
            where TObj : class, IReactiveObject
        {
            Contract.Requires(target != null);
            Contract.Requires(observable != null);
            Contract.Requires(property != null);

            return new RxPropertyHelper<TRet>(
                observable,
                _ => target.RaisingPropertyChanged(property),
                _ => target.RaisingPropertyChanging(property),
                initialValue,
                deferSubscription,
                scheduler);
        }
    }
}
