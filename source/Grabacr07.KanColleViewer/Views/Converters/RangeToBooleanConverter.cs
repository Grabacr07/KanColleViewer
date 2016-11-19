using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Grabacr07.KanColleViewer.Views.Converters
{
	public class RangeToBooleanConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return false;
			if (value.GetType() != typeof(int)) return false;
			if (parameter == null) return false;

			var v = (int)value;
			var p = parameter as string;

			var range = p.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
			if (range.Length != 2) return false;

			int min, max;
			if (!int.TryParse(range[0], out min)) return false;
			if (!int.TryParse(range[1], out max)) return false;
			return min <= v && v <= max;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
