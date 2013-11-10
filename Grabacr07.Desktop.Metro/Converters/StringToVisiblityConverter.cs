using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Grabacr07.Desktop.Metro.Converters
{
	/// <summary>
	/// 文字列が null または空文字のときに Collapsed、それ以外のときに Visible を返すコンバーターを定義します。
	/// </summary>
	public class StringToVisiblityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (value is string) && !string.IsNullOrEmpty((string)value)
				? Visibility.Visible
				: Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
