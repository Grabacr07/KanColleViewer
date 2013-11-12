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
	public enum NavigationDirection
	{
		GoBack,
		GoForward,
		Reload,
	}


	/// <summary>
	/// 
	/// </summary>
	public class NavigationButton : Button
	{
		static NavigationButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationButton), new FrameworkPropertyMetadata(typeof(NavigationButton)));
		}

		#region Direction 依存関係プロパティ

		public NavigationDirection Direction
		{
			get { return (NavigationDirection)this.GetValue(DirectionProperty); }
			set { this.SetValue(DirectionProperty, value); }
		}
		public static readonly DependencyProperty DirectionProperty =
			DependencyProperty.Register("Direction", typeof(NavigationDirection), typeof(NavigationButton), new UIPropertyMetadata(NavigationDirection.GoBack));

		#endregion
	}
}
