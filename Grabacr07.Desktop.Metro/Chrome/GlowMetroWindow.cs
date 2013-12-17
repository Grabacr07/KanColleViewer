using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Grabacr07.Desktop.Metro.Chrome
{
	/// <summary>
	/// Visual Studio 2012 風のウィンドウを表します。
	/// </summary>
	public class GlowMetroWindow : MetroWindow
	{
		static GlowMetroWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GlowMetroWindow), new FrameworkPropertyMetadata(typeof(GlowMetroWindow)));
		}

		private readonly GlowWindow left;
		private readonly GlowWindow right;
		private readonly GlowWindow top;
		private readonly GlowWindow bottom;

		/// <summary>
		/// <see cref="GlowMetroWindow"/> クラスの新しいインスタンスを初期化します。
		/// </summary>
		public GlowMetroWindow()
		{
			this.left = new GlowWindow(this, new GlowWindowProcessorLeft());
			this.right = new GlowWindow(this, new GlowWindowProcessorRight());
			this.top = new GlowWindow(this, new GlowWindowProcessorTop());
			this.bottom = new GlowWindow(this, new GlowWindowProcessorBottom());

			this.SizeChanged += (sender, args) => this.UpdateGlowWindows();
		}


		protected override void OnContentRendered(EventArgs e)
		{
			base.OnContentRendered(e);

			this.left.Show();
			this.right.Show();
			this.top.Show();
			this.bottom.Show();
			this.UpdateGlowWindows();
		}

		protected override void OnStateChanged(EventArgs e)
		{
			base.OnStateChanged(e);
			this.UpdateGlowWindows();
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			this.UpdateGlowWindows();
		}

		protected override void OnDeactivated(EventArgs e)
		{
			base.OnDeactivated(e);
			this.UpdateGlowWindows();
		}

		protected override void OnLocationChanged(EventArgs e)
		{
			base.OnLocationChanged(e);
			this.UpdateGlowWindows();
		}

		protected override void OnClosed(EventArgs e)
		{
			this.left.Close();
			this.right.Close();
			this.top.Close();
			this.bottom.Close();

			base.OnClosed(e);
		}

		private void UpdateGlowWindows()
		{
			this.left.Update();
			this.right.Update();
			this.top.Update();
			this.bottom.Update();
		}
	}
}
