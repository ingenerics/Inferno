using System;

namespace Inferno
{
    /// <summary>
    /// This interface is implemented by Inferno objects which are given IObservables as input - when the input IObservables OnError, 
    /// instead of disabling the Inferno object, we catch the IObservable and pipe it into this property.
    ///
    /// Normally this IObservable is implemented with a ScheduledSubject whose default Observer is RxApp.DefaultExceptionHandler - 
    /// this means, that if you aren't listening to ThrownExceptions and one appears, the exception will appear on the UI thread and 
    /// crash the application.
    /// </summary>
    public interface IHandleObservableErrors
    {
        /// <summary>
        /// Gets a observable which will fire whenever an exception would normally terminate ReactiveUI
        /// internal state.
        /// </summary>
        IObservable<Exception> ThrownExceptions { get; }
    }
}