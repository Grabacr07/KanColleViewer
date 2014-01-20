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
using Grabacr07.Desktop.Metro.Controls;

namespace Grabacr07.KanColleViewer.Views.Controls
{
	/// <summary>
	/// ミュートの切り替え機能を提供する <see cref="Button"/> を表します。
	/// </summary>
	public class MuteButton : CallMethodButton
	{
		static MuteButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MuteButton), new FrameworkPropertyMetadata(typeof(MuteButton)));
		}

		#region IsMute 依存関係プロパティ

		public bool IsMute
		{
			get { return (bool)this.GetValue(IsMuteProperty); }
			set { this.SetValue(IsMuteProperty, value); }
		}
		public static readonly DependencyProperty IsMuteProperty =
			DependencyProperty.Register("IsMute", typeof(bool), typeof(MuteButton), new UIPropertyMetadata(false));

		#endregion

	}
}
