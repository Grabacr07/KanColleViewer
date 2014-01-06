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

namespace Grabacr07.KanColleViewer.Views.Controls
{
	public class AppIcon : Control
	{
		static AppIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(AppIcon), new FrameworkPropertyMetadata(typeof(AppIcon)));
		}


		#region AnchorVisibility 依存関係プロパティ

		public Visibility AnchorVisibility
		{
			get { return (Visibility)this.GetValue(AnchorVisibilityProperty); }
			set { this.SetValue(AnchorVisibilityProperty, value); }
		}
		public static readonly DependencyProperty AnchorVisibilityProperty =
			DependencyProperty.Register("AnchorVisibility", typeof(Visibility), typeof(AppIcon), new UIPropertyMetadata(Visibility.Visible));

		#endregion

		#region BandVisibility 依存関係プロパティ

		public Visibility BandVisibility
		{
			get { return (Visibility)this.GetValue(BandVisibilityProperty); }
			set { this.SetValue(BandVisibilityProperty, value); }
		}
		public static readonly DependencyProperty BandVisibilityProperty =
			DependencyProperty.Register("BandVisibility", typeof(Visibility), typeof(AppIcon), new UIPropertyMetadata(Visibility.Visible));

		#endregion
	}
}
