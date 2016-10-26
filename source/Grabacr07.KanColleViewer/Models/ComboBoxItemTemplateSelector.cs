using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Grabacr07.KanColleViewer.Models
{
	public class ComboBoxItemTemplateSelector : DataTemplateSelector
	{
		public DataTemplate SelectedItemTemplate { get; set; }
		public DataTemplate ItemTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			bool selected = false;
			FrameworkElement fe = container as FrameworkElement;

			if (fe != null)
			{
				DependencyObject parent = fe.TemplatedParent;
				if (parent != null)
				{
					ComboBox cbo = parent as ComboBox;
					if (cbo != null) selected = true;
				}
			}

			if (selected) return SelectedItemTemplate;
			return ItemTemplate;
		}
	}
}
