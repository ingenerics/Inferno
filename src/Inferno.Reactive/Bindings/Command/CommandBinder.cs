using Inferno.Core;
using System;
using System.Linq.Expressions;
using System.Windows.Input;

namespace Inferno
{
    /// <summary>
    /// Various helpers to bind View controls and ViewModel commands together.
    /// </summary>
    public static class CommandBinder
    {
        private static ICommandBinderImplementation _binderImplementation;

        internal static void Initialize(ICommandBinderImplementation commandBinderImplementation)
        {
            _binderImplementation = commandBinderImplementation ?? throw new ArgumentNullException(nameof(commandBinderImplementation));
        }

        /// <summary>
        /// Bind a command from the ViewModel to an explicitly specified control
        /// on the View.
        /// </summary>
        /// <typeparam name="TView">The view type.</typeparam>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <typeparam name="TProp">The property type.</typeparam>
        /// <typeparam name="TControl">The control type.</typeparam>
        /// <typeparam name="TParam">The parameter type.</typeparam>
        /// <returns>A class representing the binding. Dispose it to disconnect
        /// the binding.</returns>
        /// <param name="view">The View.</param>
        /// <param name="viewModel">The View model.</param>
        /// <param name="propertyName">The ViewModel command to bind.</param>
        /// <param name="controlName">The name of the control on the view.</param>
        /// <param name="withParameter">The ViewModel property to pass as the
        /// param of the ICommand.</param>
        /// <param name="toEvent">If specified, bind to the specific event
        /// instead of the default.
        /// NOTE: If this parameter is used inside WhenActivated, it's
        /// important to dispose the binding when the view is deactivated.</param>
        public static IReactiveBinding<TView, TViewModel, TProp> BindCommand<TView, TViewModel, TProp, TControl, TParam>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> propertyName,
                Expression<Func<TView, TControl>> controlName,
                IObservable<TParam> withParameter,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand
        {
            return _binderImplementation.BindCommand(viewModel, view, propertyName, controlName, withParameter, toEvent);
        }

        /// <summary>
        /// Bind a command from the ViewModel to an explicitly specified control
        /// on the View.
        /// </summary>
        /// <typeparam name="TView">The view type.</typeparam>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <typeparam name="TProp">The property type.</typeparam>
        /// <typeparam name="TControl">The control type.</typeparam>
        /// <returns>A class representing the binding. Dispose it to disconnect
        /// the binding.</returns>
        /// <param name="view">The View.</param>
        /// <param name="viewModel">The View model.</param>
        /// <param name="propertyName">The ViewModel command to bind.</param>
        /// <param name="controlName">The name of the control on the view.</param>
        /// <param name="toEvent">If specified, bind to the specific event
        /// instead of the default.
        /// NOTE: If this parameter is used inside WhenActivated, it's
        /// important to dispose the binding when the view is deactivated.</param>
        public static IReactiveBinding<TView, TViewModel, TProp> BindCommand<TView, TViewModel, TProp, TControl>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> propertyName,
                Expression<Func<TView, TControl>> controlName,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand
        {
            return _binderImplementation.BindCommand(viewModel, view, propertyName, controlName, toEvent);
        }

        /// <summary>
        /// Bind a command from the ViewModel to an explicitly specified control
        /// on the View.
        /// </summary>
        /// <typeparam name="TView">The view type.</typeparam>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <typeparam name="TProp">The property type.</typeparam>
        /// <typeparam name="TControl">The control type.</typeparam>
        /// <typeparam name="TParam">The parameter type.</typeparam>
        /// <returns>A class representing the binding. Dispose it to disconnect
        /// the binding.</returns>
        /// <param name="view">The View.</param>
        /// <param name="viewModel">The View model.</param>
        /// <param name="propertyName">The ViewModel command to bind.</param>
        /// <param name="controlName">The name of the control on the view.</param>
        /// <param name="withParameter">The ViewModel property to pass as the
        /// param of the ICommand.</param>
        /// <param name="toEvent">If specified, bind to the specific event
        /// instead of the default.
        /// NOTE: If this parameter is used inside WhenActivated, it's
        /// important to dispose the binding when the view is deactivated.</param>
        public static IReactiveBinding<TView, TViewModel, TProp> BindCommand<TView, TViewModel, TProp, TControl, TParam>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> propertyName,
                Expression<Func<TView, TControl>> controlName,
                Expression<Func<TViewModel, TParam>> withParameter,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand
        {
            return _binderImplementation.BindCommand(viewModel, view, propertyName, controlName, withParameter, toEvent);
        }
    }
}
