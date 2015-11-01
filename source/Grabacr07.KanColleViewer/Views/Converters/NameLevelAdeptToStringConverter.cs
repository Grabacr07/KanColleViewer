using System;
using System.Globalization;
using System.Windows.Data;
using Grabacr07.KanColleViewer.Properties;

namespace Grabacr07.KanColleViewer.Views.Converters
{
	public class NameLevelAdeptToStringConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (!((values[0] is string) && (values[1] is int) && (values[2] is int))) return "";

			var name = (string)values[0];
			var level = (int)values[1];
			var adept = (int)values[2];
			var leveltext = level >= 10 ? " ★max" : level >= 1 ? (" ★+" + level) : "";
			var adepttext = adept >= 1 ? " " + string.Format(Resources.Ship_Text_Adept_Template, adept) : "";
			return name + leveltext + adepttext;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}