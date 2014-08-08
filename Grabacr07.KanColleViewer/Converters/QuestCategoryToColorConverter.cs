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
	public class QuestCategoryToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is QuestCategory)
			{
				var category = (QuestCategory)value;
				switch (category)
				{
					case QuestCategory.Composition:
						return Color.FromRgb(42, 125, 70);
					case QuestCategory.Sortie:
						return Color.FromRgb(181, 59, 54);
					case QuestCategory.Expeditions:
						return Color.FromRgb(59, 160, 157);
					case QuestCategory.Practice:
						return Color.FromRgb(141, 198, 96);
					case QuestCategory.Supply:
						return Color.FromRgb(178, 147, 47);
					case QuestCategory.Building:
						return Color.FromRgb(100, 68, 59);
					case QuestCategory.Remodelling:
						return Color.FromRgb(169, 135, 186);
				}
			}

			return Color.FromRgb(128, 128, 128);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
