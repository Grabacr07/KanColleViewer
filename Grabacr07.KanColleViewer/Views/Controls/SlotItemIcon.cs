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

namespace Grabacr07.KanColleViewer.Views.Controls
{
	/// <summary>
	/// 装備スロットの種類に基づいたアイコンを表示します。
	/// </summary>
	public class SlotItemIcon : Control
	{
		static SlotItemIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SlotItemIcon), new FrameworkPropertyMetadata(typeof(SlotItemIcon)));
		}
		
		#region Type 依存関係プロパティ

		public SlotItemIconType Type
		{
			get { return (SlotItemIconType)this.GetValue(TypeProperty); }
			set { this.SetValue(TypeProperty, value); }
		}
		public static readonly DependencyProperty TypeProperty =
			DependencyProperty.Register("Type", typeof(SlotItemIconType), typeof(SlotItemIcon), new UIPropertyMetadata(SlotItemIconType.Unknown));

		#endregion
	}
}
