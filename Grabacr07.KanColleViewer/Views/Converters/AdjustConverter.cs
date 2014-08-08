using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.Views.Converters
{
    public class AdjustConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var number = Double.Parse(value.ToString());
            var plus = Double.Parse(parameter.ToString());
            return number + plus;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var number = Double.Parse(value.ToString());
            var plus = Double.Parse(parameter.ToString());
            return number - plus;
        }
    }
}
