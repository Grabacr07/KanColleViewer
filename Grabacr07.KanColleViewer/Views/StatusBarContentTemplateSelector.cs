using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Grabacr07.KanColleViewer.Views
{
	public class StatusBarContentTemplateSelector : DataTemplateSelector
	{



		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			return base.SelectTemplate(item, container);
		}
	}
}
