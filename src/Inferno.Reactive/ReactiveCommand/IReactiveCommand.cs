using System;

namespace Inferno
{
    /// <summary>
    /// Encapsulates a user action behind a reactive interface.
    /// This is for interop inside for the command binding.
    /// Not meant for external use due to the fact it doesn't implement ICommand
    /// to force the user to favor the Reactive style command execution.
    /// </summary>
    public interface IReactiveCommand : IDisposable, IHandleObservableErrors
    {
        /// <summary>
        /// Gets an observable whose value indicates whether the command is currently executing.
        /// </summary>
        /// <remarks>
        /// This observable can be particularly useful for updating UI, such as showing an activity indicator whilst a command
        /// is executing.
        /// </remarks>
        IObservable<bool> IsExecuting { get; }

        /// <summary>
        /// Gets an observable whose value indicates whether the command can currently execute.
        /// </summary>
        /// <remarks>
        /// The value provided by this observable is governed both by any <c>canExecute</c> observable provided during
        /// command creation, as well as the current execution status of the command. A command that is currently executing
        /// will always yield <c>false</c> from this observable, even if the <c>canExecute</c> pipeline is currently <c>true</c>.
        /// </remarks>
        IObservable<bool> CanExecute { get; }
    }
}