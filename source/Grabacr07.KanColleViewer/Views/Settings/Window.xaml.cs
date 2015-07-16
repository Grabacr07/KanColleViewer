using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Grabacr07.KanColleViewer.Views.Settings
{
	partial class Window
	{
		public Window()
		{
			this.InitializeComponent();
		}

		#region IsDockMatched 添付プロパティ

		public static readonly DependencyProperty IsDockMatchedProperty = DependencyProperty.RegisterAttached(
			"IsDockMatched", typeof(bool), typeof(Window), new PropertyMetadata(default(bool)));

		public static void SetIsDockMatched(DependencyObject element, bool value)
		{
			element.SetValue(IsDockMatchedProperty, value);
		}

		public static bool GetIsDockMatched(DependencyObject element)
		{
			return (bool)element.GetValue(IsDockMatchedProperty);
		}

		#endregion
	}
}
