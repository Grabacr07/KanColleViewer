using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Grabacr07.KanColleWrapper.Models;
using Settings2 = Grabacr07.KanColleViewer.Models.Settings;

namespace Grabacr07.KanColleViewer.Views.Converters
{
	public class AdjustConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var number = Double.Parse(value.ToString());
			var plus = Double.Parse(parameter.ToString());
			if (Settings2.Current.OrientationMode == Models.OrientationType.Vertical) plus = plus - 54.00;
			return number + plus;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var number = Double.Parse(value.ToString());
			var plus = Double.Parse(parameter.ToString());
			if (Settings2.Current.OrientationMode == Models.OrientationType.Vertical) plus = plus - 54.00;
			return number - plus;
		}
	}
}
