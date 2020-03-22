using System;

namespace Inferno
{
    public class DialogTypeToIconTemplateConverter : IBindingTypeConverter
    {
        internal static OcticonsKindToIconTemplateConverter OcticonsKindToIconTemplateConverter = new OcticonsKindToIconTemplateConverter();

        public int GetAffinityForObjects(Type fromType, Type toType) => fromType == typeof(DialogType) ? 10 : 0;

        public bool TryConvert(object value, Type toType, object conversionHint, out object result)
        {
            result = null;

            if (value is DialogType dialogType && dialogType != DialogType.None)
            {
                var octiconsKind = OcticonsKindForType(dialogType);
                return OcticonsKindToIconTemplateConverter.TryConvert(octiconsKind, toType, conversionHint, out result);
            }

            return true;
        }

        private static PackIconOcticonsKind OcticonsKindForType(DialogType dialogType)
        {
            PackIconOcticonsKind octiconsKind;

            switch (dialogType)
            {
                case DialogType.Error:
                    octiconsKind = PackIconOcticonsKind.CircleSlash;
                    break;
                case DialogType.Information:
                    octiconsKind = PackIconOcticonsKind.Info;
                    break;
                case DialogType.Question:
                    octiconsKind = PackIconOcticonsKind.Question;
                    break;
                case DialogType.Warning:
                    octiconsKind = PackIconOcticonsKind.Alert;
                    break;
                case DialogType.Settings:
                    octiconsKind = PackIconOcticonsKind.Gear;
                    break;
                case DialogType.None: // TryConvert method will handle None
                    octiconsKind = PackIconOcticonsKind.None;
                    break;
                default:
                    throw new NotImplementedException($"{nameof(OcticonsKindForType)} {nameof(DialogType)}.{dialogType}");
            }

            return octiconsKind;
        }
    }
}