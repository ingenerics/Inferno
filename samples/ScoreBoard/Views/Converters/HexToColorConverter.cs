using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ScoreBoard.Views.Converters
{
    public class HexToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => new BrushConverter().ConvertFrom((string)value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
}
