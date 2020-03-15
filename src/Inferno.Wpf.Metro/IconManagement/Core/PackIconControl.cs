using System;
using System.Collections.Generic;
using System.Windows;

namespace Inferno
{
    /// <summary>
    /// Class PackIconControl which is the base class for any PackIcon control.
    /// </summary>
    /// <typeparam name="TKind">The type of the enum kind.</typeparam>
    /// <seealso cref="PackIconBase{TKind}" />
    public abstract class PackIconControl<TKind> : PackIconBase<TKind>
    {
        static PackIconControl()
        {
            OpacityProperty.OverrideMetadata(typeof(PackIconControl<TKind>), new UIPropertyMetadata(1d));
            VisibilityProperty.OverrideMetadata(typeof(PackIconControl<TKind>), new UIPropertyMetadata(Visibility.Visible));
        }

        protected PackIconControl(Func<IDictionary<TKind, string>> dataIndexFactory) : base(dataIndexFactory)
        {
        }
    }
}