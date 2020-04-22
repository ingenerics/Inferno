using System;
using System.Globalization;
using System.Windows.Data;

namespace WorldCup.ViewModels.Overview
{
    public class LineItemToTrueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is LineOverviewItem;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
}
