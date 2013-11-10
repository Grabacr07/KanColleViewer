using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace Grabacr07.Desktop.Metro.Behaviors
{
	/// <summary>
	/// ウィンドウが非アクティブ化されたときに Opacity 値を操作するビヘイビア。
	/// </summary>
	public class InactiveOpacityBehavior : Behavior<FrameworkElement>
	{
		#region OpacityWhenInactive 依存関係プロパティ

		/// <summary>
		/// <see cref="IsActive"/> プロパティ値が false になったときに、アタッチ先要素の
		/// <see cref="FrameworkElement.Opacity"/> に適用する透過度を取得または設定します。
		/// </summary>
		public double OpacityWhenInactive
		{
			get { return (double)this.GetValue(OpacityWhenInactiveProperty); }
			set { this.SetValue(OpacityWhenInactiveProperty, value); }
		}

		/// <summary>
		/// <see cref="OpacityWhenInactive"/> 依存関係プロパティを識別します。
		/// </summary>
		public static readonly DependencyProperty OpacityWhenInactiveProperty =
			DependencyProperty.Register("OpacityWhenInactive", typeof(double), typeof(InactiveOpacityBehavior), new UIPropertyMetadata(0.7));

		#endregion

		#region IsActive 依存関係プロパティ

		/// <summary>
		/// アタッチ先要素がアクティブかどうかを示す値を取得または設定します。
		/// </summary>
		public bool IsActive
		{
			get { return (bool)this.GetValue(IsActiveProperty); }
			set { this.SetValue(IsActiveProperty, value); }
		}

		/// <summary>
		/// <see cref="InactiveOpacityBehavior.IsActive"/> 依存関係プロパティを識別します。
		/// </summary>
		public static readonly DependencyProperty IsActiveProperty =
			DependencyProperty.Register("IsActive", typeof(bool), typeof(InactiveOpacityBehavior), new UIPropertyMetadata(false, IsActivePropertyChangedCallback));

		private static void IsActivePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var source = (InactiveOpacityBehavior)d;
			var isActive = (bool)e.NewValue;

			source.AssociatedObject.Opacity = isActive ? 1.0 : source.OpacityWhenInactive;
		}

		#endregion
	}
}
