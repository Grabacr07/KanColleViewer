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
			try
			{
				var v = (int)value;
				var p = parameter as string;

				var range = p?.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
				if (range?.Length != 2) return false;

				var min = int.Parse(range[0]);
				var max = int.Parse(range[1]);
				return min <= v && v <= max;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.Write(ex);
			}

			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
