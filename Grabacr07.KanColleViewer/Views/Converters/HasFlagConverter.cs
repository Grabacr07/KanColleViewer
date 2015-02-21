using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Grabacr07.KanColleViewer.Views.Converters
{
	public class HasFlagConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				var enumValue = value as Enum;
				if (enumValue != null)
				{
					var p = parameter as string;
					if (p != null)
					{
						return p.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
							.Select(x => x.Trim())
							.Select(x => Enum.Parse(enumValue.GetType(), x) as Enum)
							.All(x => enumValue.HasFlag(x));
					}
				}
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
