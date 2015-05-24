using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace BattleInfoPlugin.Views.Converters
{
    public class AdditionalNameColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var name = value as string;
            if (string.IsNullOrEmpty(name)) return new SolidColorBrush(Colors.Transparent);
            if (name.ToLower().Trim() == "elite") return new SolidColorBrush(Color.FromRgb(238, 69, 76));
            if (name.ToLower().Trim() == "flagship") return new SolidColorBrush(Color.FromRgb(221, 182, 42));
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
