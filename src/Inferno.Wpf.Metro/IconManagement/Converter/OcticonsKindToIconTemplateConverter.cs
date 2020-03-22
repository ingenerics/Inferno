using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Inferno
{
    public class OcticonsKindToIconTemplateConverter : IBindingTypeConverter
    {
        internal static PackIcon PackIcon = new PackIcon { Width = 20, Height = 20 };

        public int GetAffinityForObjects(Type fromType, Type toType) => fromType == typeof(PackIconOcticonsKind) ? 10 : 0;

        public bool TryConvert(object value, Type toType, object conversionHint, out object result)
        {
            result = null;

            if (value is PackIconOcticonsKind octiconsKind && octiconsKind != PackIconOcticonsKind.None)
            {
                var icon = PackIcon.GetPackIcon<PackIconOcticonsControl, PackIconOcticonsKind>(octiconsKind);
                result = ToDataTemplate(icon);
            }

            return true;
        }

        private static DataTemplate ToDataTemplate(Control icon)
        {
            var template = new DataTemplate();

            var content = new FrameworkElementFactory(typeof(ContentControl));
            content.SetBinding(ContentControl.ContentProperty, new Binding() { Source = icon });

            template.VisualTree = content;
            template.Seal();
            return template;
        }
    }
}