using Inferno.Core;
using System;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Inferno
{
    /// <summary>
    /// Internal implementation details which performs Binding ICommand's to controls.
    /// </summary>
    internal static class CommandBinderImplementationExtensions
    {
        public static IReactiveBinding<TView, TViewModel, TProp> BindCommand<TView, TViewModel, TProp, TControl>(
                this ICommandBinderImplementation @this,
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> propertyName,
                Expression<Func<TView, TControl>> controlName,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand
        {
            return @this.BindCommand(viewModel, view, propertyName, controlName, Observable<object>.Empty, toEvent);
        }

        public static IReactiveBinding<TView, TViewModel, TProp> BindCommand<TView, TViewModel, TProp, TControl, TParam>(
                this ICommandBinderImplementation @this,
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> propertyName,
                Expression<Func<TView, TControl>> controlName,
                Expression<Func<TViewModel, TParam>> withParameter,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand
        {
            if (withParameter == null)
            {
                throw new ArgumentNullException(nameof(withParameter));
            }

            var paramExpression = Reflection.Rewrite(withParameter.Body);
            var param = Reflection.ViewModelWhenAnyValue(viewModel, view, paramExpression);

            return @this.BindCommand(viewModel, view, propertyName, controlName, param, toEvent);
        }
    }
}
