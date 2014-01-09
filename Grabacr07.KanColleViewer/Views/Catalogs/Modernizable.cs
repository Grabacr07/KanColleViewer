using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.Views.Catalogs
{
	/// <summary>
	/// <see cref="ModernizableStatus"/> の表示をサポートします。
	/// </summary>
	public class Modernizable : Control
	{
		static Modernizable()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Modernizable), new FrameworkPropertyMetadata(typeof(Modernizable)));
		}


		#region Status 依存関係プロパティ

		public ModernizableStatus Status
		{
			get { return (ModernizableStatus)this.GetValue(StatusProperty); }
			set { this.SetValue(StatusProperty, value); }
		}
		public static readonly DependencyProperty StatusProperty =
			DependencyProperty.Register("Status", typeof(ModernizableStatus), typeof(Modernizable), new UIPropertyMetadata(ModernizableStatus.Dummy, StatusPropertyChangedCallback));

		private static void StatusPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var source = (Modernizable)d;
			var status = (ModernizableStatus)e.NewValue;

			source.IsMax = status.IsMax;
		}

		#endregion

		#region IsMax 依存関係プロパティ

		public bool IsMax
		{
			get { return (bool)this.GetValue(IsMaxProperty); }
			private set { this.SetValue(IsMaxProperty, value); }
		}
		public static readonly DependencyProperty IsMaxProperty =
			DependencyProperty.Register("IsMax", typeof(bool), typeof(Modernizable), new UIPropertyMetadata(false));

		#endregion


	}
}
