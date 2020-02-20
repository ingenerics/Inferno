using System;

namespace Inferno
{
    /// <summary>
    /// Calls ToString on types. In WPF, ComponentTypeConverter should win
    /// instead of this, since It's Better™.
    /// </summary>
    public class StringConverter : IBindingTypeConverter
    {
        /// <inheritdoc/>
        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            return toType == typeof(string) ? 2 : 0;
        }

        /// <inheritdoc/>
        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            // XXX: All Of The Localization
            result = from?.ToString();
            return true;
        }
    }
}