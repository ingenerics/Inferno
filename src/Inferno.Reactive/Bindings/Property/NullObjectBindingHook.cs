using System;

namespace Inferno
{
    /// <summary>
    /// Provides a dummy implementation of IPropertyBindingHook
    /// </summary>
    public class NullObjectBindingHook : IPropertyBindingHook
    {
        /// <inheritdoc/>
        public bool ExecuteHook(object source, object target, Func<IObservedChange<object, object>[]> getCurrentViewModelProperties, Func<IObservedChange<object, object>[]> getCurrentViewProperties, BindingDirection direction)
        {
            return true;
        }
    }
}
