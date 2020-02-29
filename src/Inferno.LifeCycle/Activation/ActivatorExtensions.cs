using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Inferno
{
    /// <summary>
    /// A set of extension methods to help wire up ViewModel initialization and activation.
    /// </summary>
    public static class ActivatorExtensions
    {
        /// <summary>
        /// WhenInitialized allows you to register a Func to be called when a ViewModel is Initialized.
        /// </summary>
        /// <param name="item">Object that supports activation.</param>
        /// <param name="block">
        /// The method to be called when the corresponding ViewModel is initialized.
        /// It returns a list of Disposables that will be cleaned up when the ViewModel is disposed.
        /// </param>
        /// <returns>A Disposable that deactivates this registration.</returns>
        public static void WhenInitialized(this IActivate item, Func<IEnumerable<IDisposable>> block)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.Activator.AddInitializationBlock(block);
        }

        /// <summary>
        /// WhenInitialized allows you to register a Func to be called when a ViewModel is Initialized.
        /// </summary>
        /// <param name="item">Object that supports activation.</param>
        /// <param name="block">
        /// The method to be called when the corresponding ViewModel is initialized. The Action parameter (usually called 'd') allows
        /// you to register Disposables to be cleaned up when the ViewModel is disposed (i.e. "d(someObservable.Subscribe());").
        /// </param>
        /// <returns>A Disposable that deactivates this registration.</returns>
        public static void WhenInitialized(this IActivate item, Action<Action<IDisposable>> block)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.Activator.AddInitializationBlock(() =>
            {
                var ret = new List<IDisposable>();
                block(ret.Add);
                return ret;
            });
        }

        /// <summary>
        /// WhenInitialized allows you to register a Func to be called when a ViewModel is Initialized.
        /// </summary>
        /// <param name="item">Object that supports activation.</param>
        /// <param name="block">
        /// The method to be called when the corresponding ViewModel is initialized. The Action parameter (usually called 'd') allows
        /// you to collate all the disposables to be cleaned up when the ViewModel is disposed.
        /// </param>
        /// <returns>A Disposable that deactivates this registration.</returns>
        public static void WhenInitialized(this IActivate item, Action<CompositeDisposable> block)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.Activator.AddInitializationBlock(() =>
            {
                var d = new CompositeDisposable();
                block(d);
                return new[] { d };
            });
        }

        /// <summary>
        /// WhenActivated allows you to register a Func to be called when a ViewModel is Activated.
        /// </summary>
        /// <param name="item">Object that supports activation.</param>
        /// <param name="block">
        /// The method to be called when the corresponding ViewModel is activated.
        /// It returns a list of Disposables that will be cleaned up when the ViewModel is deactivated.
        /// </param>
        /// <returns>A Disposable that deactivates this registration.</returns>
        public static void WhenActivated(this IActivate item, Func<IEnumerable<IDisposable>> block)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.Activator.AddActivationBlock(block);
        }

        /// <summary>
        /// WhenActivated allows you to register a Func to be called when a ViewModel is Activated.
        /// </summary>
        /// <param name="item">Object that supports activation.</param>
        /// <param name="block">
        /// The method to be called when the corresponding ViewModel is activated. The Action parameter (usually called 'd') allows
        /// you to register Disposables to be cleaned up when the ViewModel is deactivated (i.e. "d(someObservable.Subscribe());").
        /// </param>
        /// <returns>A Disposable that deactivates this registration.</returns>
        public static void WhenActivated(this IActivate item, Action<Action<IDisposable>> block)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.Activator.AddActivationBlock(() =>
            {
                var ret = new List<IDisposable>();
                block(ret.Add);
                return ret;
            });
        }

        /// <summary>
        /// WhenActivated allows you to register a Func to be called when a ViewModel is Activated.
        /// </summary>
        /// <param name="item">Object that supports activation.</param>
        /// <param name="block">
        /// The method to be called when the corresponding ViewModel is activated. The Action parameter (usually called 'd') allows
        /// you to collate all the disposables to be cleaned up during deactivation.
        /// </param>
        /// <returns>A Disposable that deactivates this registration.</returns>
        public static void WhenActivated(this IActivate item, Action<CompositeDisposable> block)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.Activator.AddActivationBlock(() =>
            {
                var d = new CompositeDisposable();
                block(d);
                return new[] { d };
            });
        }

        /// <summary>
        /// Used to avoid race conditions during initialization.
        /// Before the WhenInitialized scope is completed, the returned observable will tick the provided initial value.
        /// After initialization the source observable will be used. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="source"></param>
        /// <param name="initialValue"></param>
        /// <returns>A disposable.</returns>
        public static IObservable<T> WhenInitializedSwitch<T>(this IActivate item, IObservable<T> source, T initialValue)
            => item.Activator.IsInitialized
                .Select(isInitialized => isInitialized ? source : Observable.Return(initialValue))
                .Switch();

        /// <summary>
        /// Used to avoid race conditions during activation.
        /// When the item is not active, or the WhenActivated scope is not yet completed, the returned observable will tick the provided fallback value.
        /// After activation the source observable will be used. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="source"></param>
        /// <param name="fallbackValue"></param>
        /// <returns>A disposable.</returns>
        public static IObservable<T> WhenActivatedSwitch<T>(this IActivate item, IObservable<T> source, T fallbackValue)
            => item.Activator.IsActive
                .Select(isActivated => isActivated ? source : Observable.Return(fallbackValue))
                .Switch();
    }
}
