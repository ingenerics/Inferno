using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Threading;

namespace Inferno
{
    [DataContract]
    public class ReactiveObject : IReactiveNotifyPropertyChanged<IReactiveObject>, IHandleObservableErrors, IReactiveObject
    {
        private readonly Lazy<IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>>> _changing;
        private readonly Lazy<IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>>> _changed;
        private readonly Lazy<IObservable<Exception>> _thrownExceptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveObject"/> class.
        /// </summary>
        public ReactiveObject()
        {
            _changing = new Lazy<IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>>>(() => ((IReactiveObject)this).GetChangingObservable(), LazyThreadSafetyMode.PublicationOnly);
            _changed = new Lazy<IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>>>(() => ((IReactiveObject)this).GetChangedObservable(), LazyThreadSafetyMode.PublicationOnly);
            _thrownExceptions = new Lazy<IObservable<Exception>>(this.GetThrownExceptionsObservable, LazyThreadSafetyMode.PublicationOnly);
        }

        public event PropertyChangingEventHandler PropertyChanging;

        public event PropertyChangedEventHandler PropertyChanged;

        public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changing => _changing.Value;

        public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changed => _changed.Value;

        public IObservable<Exception> ThrownExceptions => _thrownExceptions.Value;

        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);

        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

        public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

        /// <summary>
        /// Determines if change notifications are enabled or not.
        /// </summary>
        /// <returns>A value indicating whether change notifications are enabled.</returns>
        public bool AreChangeNotificationsEnabled() => IReactiveObjectExtensions.AreChangeNotificationsEnabled(this);

        /// <summary>
        /// Delays notifications until the return IDisposable is disposed.
        /// </summary>
        /// <returns>A disposable which when disposed will send delayed notifications.</returns>
        public IDisposable DelayChangeNotifications() => IReactiveObjectExtensions.DelayChangeNotifications(this);
    }
}
