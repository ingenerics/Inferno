using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Inferno
{
    public class DialogTypeToIconTemplateConverter : IBindingTypeConverter
    {
        internal static PackIcon PackIcon = new PackIcon { Width = 20, Height = 20 };

        public int GetAffinityForObjects(Type fromType, Type toType) => fromType == typeof(DialogType) ? 10 : 0;

        public bool TryConvert(object value, Type toType, object conversionHint, out object result)
        {
            result = null;

            if (value is DialogType dialogType && dialogType != DialogType.None)
            {
                var icon = IconForType(dialogType);
                result = ToDataTemplate(icon);
            }

            return true;
        }

        private static Control IconForType(DialogType dialogType)
        {
            PackIconOcticonsKind octiconsIcon;

            switch (dialogType)
            {
                case DialogType.Error:
                    octiconsIcon = PackIconOcticonsKind.CircleSlash;
                    break;
                case DialogType.Information:
                    octiconsIcon = PackIconOcticonsKind.Info;
                    break;
                case DialogType.Question:
                    octiconsIcon = PackIconOcticonsKind.Question;
                    break;
                case DialogType.Warning:
                    octiconsIcon = PackIconOcticonsKind.Alert;
                    break;
                case DialogType.Settings:
                    octiconsIcon = PackIconOcticonsKind.Gear;
                    break;
                case DialogType.None: // TryConvert method will handle None
                    octiconsIcon = PackIconOcticonsKind.None;
                    break;
                default:
                    throw new NotImplementedException($"{nameof(IconForType)} {nameof(DialogType)}.{dialogType}");
            }

            return PackIcon.GetPackIcon<PackIconOcticonsControl, PackIconOcticonsKind>(octiconsIcon);
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