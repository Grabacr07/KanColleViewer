using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows;

namespace Grabacr07.KanColleViewer.Views.Converters
{
	class PreviewSizeConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			try
			{
				if (values.Length != 6) throw new ArgumentException();
				String[] posString = ((String)parameter).Split(',');
				Double posX = Double.Parse(posString[0]) + (Double)values[4];
				Double posY = Double.Parse(posString[1]) + (Double)values[5];
				Double rootWidth = (Double)values[0];
				Double rootHeight = (Double)values[1];
				Double contentWidth = (Double)values[2];
				Double contentHeight = (Double)values[3];
				return new Thickness(posX, posY, rootWidth - contentWidth - posX, rootHeight - contentHeight - posY);
			}
			catch
			{
				return new Thickness(0, 0, 0, 0);
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
