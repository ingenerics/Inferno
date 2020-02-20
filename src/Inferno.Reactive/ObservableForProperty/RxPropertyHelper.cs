using System;
using System.Diagnostics.Contracts;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace Inferno
{
    /// <summary>
    /// RxPropertyHelper is a class to help ViewModels implement
    /// "output properties", that is, a property that is backed by an
    /// Observable. The property will be read-only, but will still fire change
    /// notifications. This class can be created directly, but is more often created
    /// via the <see cref="RxPropertyHelperExtensions" /> extension methods.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    public sealed class RxPropertyHelper<T> : IHandleObservableErrors, IDisposable
    {
        private readonly Lazy<ISubject<Exception>> _thrownExceptions;
        private readonly IObservable<T> _source;
        private readonly ISubject<T> _subject;
        private T _lastValue;
        private CompositeDisposable _disposable = new CompositeDisposable();
        private int _activated;

        /// <summary>
        /// Initializes a new instance of the <see cref="RxPropertyHelper{T}"/> class.
        /// </summary>
        /// <param name="observable">
        /// The Observable to base the property on.
        /// </param>
        /// <param name="onChanged">
        /// The action to take when the property changes, typically this will call the
        /// ViewModel's RaisePropertyChanged method.
        /// </param>
        /// <param name="initialValue">
        /// The initial value of the property.
        /// </param>
        /// <param name="deferSubscription">
        /// A value indicating whether the <see cref="RxPropertyHelper{T}"/>
        /// should defer the subscription to the <paramref name="observable"/> source
        /// until the first call to <see cref="Value"/>, or if it should immediately
        /// subscribe to the the <paramref name="observable"/> source.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler that the notifications will be provided on -
        /// this should normally be a Dispatcher-based scheduler.
        /// </param>
        public RxPropertyHelper(
            IObservable<T> observable,
            Action<T> onChanged,
            T initialValue = default(T),
            bool deferSubscription = false,
            IScheduler scheduler = null)
            : this(observable, onChanged, null, initialValue, deferSubscription, scheduler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RxPropertyHelper{T}"/> class.
        /// </summary>
        /// <param name="observable">
        /// The Observable to base the property on.
        /// </param>
        /// <param name="onChanged">
        /// The action to take when the property changes, typically this will call
        /// the ViewModel's RaisePropertyChanged method.
        /// </param>
        /// <param name="onChanging">
        /// The action to take when the property changes, typically this will call
        /// the ViewModel's RaisePropertyChanging method.
        /// </param>
        /// <param name="initialValue">
        /// The initial value of the property.
        /// </param>
        /// <param name="deferSubscription">
        /// A value indicating whether the <see cref="RxPropertyHelper{T}"/>
        /// should defer the subscription to the <paramref name="observable"/> source
        /// until the first call to <see cref="Value"/>, or if it should immediately
        /// subscribe to the the <paramref name="observable"/> source.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler that the notifications will provided on - this
        /// should normally be a Dispatcher-based scheduler.
        /// </param>
        public RxPropertyHelper(
            IObservable<T> observable,
            Action<T> onChanged,
            Action<T> onChanging = null,
            T initialValue = default(T),
            bool deferSubscription = false,
            IScheduler scheduler = null)
        {
            Contract.Requires(observable != null);
            Contract.Requires(onChanged != null);

            scheduler = scheduler ?? CurrentThreadScheduler.Instance;
            onChanging = onChanging ?? (_ => { });

            _subject = new ScheduledSubject<T>(scheduler);
            _subject.Subscribe(
                x =>
                {
                    onChanging(x);
                    _lastValue = x;
                    onChanged(x);
                },
                ex => _thrownExceptions.Value.OnNext(ex))
                .DisposeWith(_disposable);

            _thrownExceptions = new Lazy<ISubject<Exception>>(() => new ScheduledSubject<Exception>(CurrentThreadScheduler.Instance, RxApp.DefaultExceptionHandler));

            _lastValue = initialValue;
            _source = observable.StartWith(initialValue).DistinctUntilChanged();
            if (!deferSubscription)
            {
                _source.Subscribe(_subject).DisposeWith(_disposable);
                _activated = 1;
            }
        }

        /// <summary>
        /// Gets the last provided value from the Observable.
        /// </summary>
        public T Value
        {
            get
            {
                if (Interlocked.CompareExchange(ref _activated, 1, 0) == 0)
                {
                    _source.Subscribe(_subject).DisposeWith(_disposable);
                }

                return _lastValue;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the RxPropertyHelper
        /// has subscribed to the source Observable.
        /// Useful for scenarios where you use deferred subscription and want to know if
        /// the RxPropertyHelper Value has been accessed yet.
        /// </summary>
        public bool IsSubscribed => _activated > 0;

        /// <summary>
        /// Gets an observable which signals whenever an exception would normally terminate the application
        /// internal state.
        /// </summary>
        public IObservable<Exception> ThrownExceptions => _thrownExceptions.Value;

        /// <summary>
        /// Constructs a "default" RxPropertyHelper object. This is useful for when you will initialize the RxPH later,
        /// but don't want bindings to access a null RxPH at startup.
        /// </summary>
        /// <param name="initialValue">
        /// The initial (and only) value of the property.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler that the notifications will be provided on - this should normally be a Dispatcher-based scheduler.
        /// </param>
        /// <returns>A default property helper.</returns>
        public static RxPropertyHelper<T> Default(T initialValue = default(T), IScheduler scheduler = null) =>
            new RxPropertyHelper<T>(Observable<T>.Never, _ => { }, initialValue, false, scheduler);

        /// <summary>
        /// Disposes this RxPropertyHelper.
        /// </summary>
        public void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}