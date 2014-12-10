using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace Grabacr07.Desktop.Metro.Controls
{
	public class HyperlinkEx : Hyperlink
	{
		#region Uri 依存関係プロパティ

		public Uri Uri
		{
			get { return (Uri)this.GetValue(UriProperty); }
			set { this.SetValue(UriProperty, value); }
		}
		public static readonly DependencyProperty UriProperty =
			DependencyProperty.Register("Uri", typeof(Uri), typeof(HyperlinkEx), new UIPropertyMetadata(null));

		#endregion

		protected override void OnClick()
		{
			base.OnClick();

			if (this.Uri != null)
			{
				try
				{
					Process.Start(this.Uri.ToString());
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
				}
			}
		}
	}
}
