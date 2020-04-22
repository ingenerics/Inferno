using System;
using System.Globalization;
using System.Windows.Data;

namespace WorldCup.ViewModels.Overview
{
    public class DivideActualSizeByConverter : IValueConverter
    {
        // Keep the parent's desired size as is
        private const double Correction = 10.0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dividend = (double) value;
            var divisor = (double) parameter;

            var correctedQuotient = (dividend / divisor) - Correction;

            return Math.Max(0, correctedQuotient);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var correctedQuotient = (double)value;
            var divisor = (double)parameter;

            return (correctedQuotient * divisor) + Correction;
        }
    }
}
