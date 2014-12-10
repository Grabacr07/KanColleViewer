using Grabacr07.KanColleWrapper.Models;
using System.Windows;
using System.Windows.Controls;

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
