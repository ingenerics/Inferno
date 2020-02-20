using System;
using System.Windows;

namespace Inferno
{
    /// <summary>
    /// This type convert converts between Boolean and XAML Visibility - the
    /// conversionHint is a BooleanToVisibilityHint.
    /// </summary>
    public class BooleanToVisibilityTypeConverter : IBindingTypeConverter
    {
        /// <inheritdoc/>
        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            if (fromType == typeof(bool) && toType == typeof(Visibility))
            {
                return 10;
            }

            if (fromType == typeof(Visibility) && toType == typeof(bool))
            {
                return 10;
            }

            return 0;
        }

        /// <inheritdoc/>
        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            var hint = conversionHint is BooleanToVisibilityHint ?
                (BooleanToVisibilityHint)conversionHint :
                BooleanToVisibilityHint.None;

            if (toType == typeof(Visibility))
            {
                var fromAsBool = hint.HasFlag(BooleanToVisibilityHint.Inverse) ? !(bool)@from : (bool)from;
                var notVisible = hint.HasFlag(BooleanToVisibilityHint.UseHidden) ? Visibility.Hidden : Visibility.Collapsed;
                result = fromAsBool ? Visibility.Visible : notVisible;
                return true;
            }

            var fromAsVis = (Visibility)from;
            result = fromAsVis == Visibility.Visible ^ !hint.HasFlag(BooleanToVisibilityHint.Inverse);

            return true;
        }
    }
}
