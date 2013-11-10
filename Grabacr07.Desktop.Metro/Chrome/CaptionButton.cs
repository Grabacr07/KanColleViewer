using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Grabacr07.Desktop.Metro.Chrome
{
	/// <summary>
	/// ウィンドウのキャプション部分で使用するために最適化された <see cref="Button"/> コントロールを表します。
	/// </summary>
	public class CaptionButton : Button
	{
		static CaptionButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CaptionButton), new FrameworkPropertyMetadata(typeof(CaptionButton)));
		}

		private Window owner;

		#region WindowAction 依存関係プロパティ

		/// <summary>
		/// ボタンに割り当てるウィンドウ操作を取得または設定します。
		/// </summary>
		public WindowAction WindowAction
		{
			get { return (WindowAction)this.GetValue(WindowActionProperty); }
			set { this.SetValue(WindowActionProperty, value); }
		}

		/// <summary>
		/// <see cref="P:Grabacr07.Desktop.Metro.Chrome.CaptionButton.WindowAction"/> 依存関係プロパティを識別します。
		/// </summary>
		public static readonly DependencyProperty WindowActionProperty =
			DependencyProperty.Register("WindowAction", typeof(WindowAction), typeof(CaptionButton), new UIPropertyMetadata(WindowAction.None));

		#endregion

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			this.owner = Window.GetWindow(this);
			if (this.owner != null)
			{
				this.owner.StateChanged += (sender, args) => this.ChangeVisibility();
				this.ChangeVisibility();
			}
		}

		protected override void OnClick()
		{
			this.WindowAction.Invoke(this);
			base.OnClick();
		}

		private void ChangeVisibility()
		{
			switch (this.WindowAction)
			{
				case WindowAction.Maximize:
					this.Visibility = this.owner.WindowState != WindowState.Maximized ? Visibility.Visible : Visibility.Collapsed;
					break;
				case WindowAction.Minimize:
					this.Visibility = this.owner.WindowState != WindowState.Minimized ? Visibility.Visible : Visibility.Collapsed;
					break;
				case WindowAction.Normalize:
					this.Visibility = this.owner.WindowState != WindowState.Normal ? Visibility.Visible : Visibility.Collapsed;
					break;
			}
		}
	}
}
