using Grabacr07.KanColleWrapper;
using Livet;
using MetroRadiance.Core;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Grabacr07.KanColleViewer.Views.Controls
{
	/// <summary>
	/// PreviewBattlePopup.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class PreviewBattlePopup 
	{
		private Dpi? systemDpi;
		
		public PreviewBattlePopup()
		{
			InitializeComponent();

			this.Popup.CustomPopupPlacementCallback = this.PopupPlacementCallback;
			this.Popup.Opened += (sender, args) => this.ChangeBackground();
			this.Popup.Closed += (sender, args) => this.ChangeBackground();
		}
		


		private CustomPopupPlacement[] PopupPlacementCallback(Size popupSize, Size targetSize, Point offset)
		{
			var dpi = this.systemDpi ?? (this.systemDpi = this.GetSystemDpi()) ?? Dpi.Default;
			return new[]
			{
				new CustomPopupPlacement(new Point(offset.X * dpi.ScaleX, offset.Y* dpi.ScaleY), PopupPrimaryAxis.None),
				//new CustomPopupPlacement(new Point((App.Current.MainWindow.Width*-0.6725)*dpi.ScaleX, (offset.Y+20)*dpi.ScaleY), PopupPrimaryAxis.None),
			};
		}
		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);

			this.ChangeBackground();
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);

			this.ChangeBackground();
		}
		private void ChangeBackground()
		{
			if (this.Popup.IsOpen)
			{
				try
				{
					this.Background = this.FindResource("AccentBrushKey") as Brush;
				}
				catch (ResourceReferenceKeyNotFoundException ex)
				{
					Debug.WriteLine(ex);
				}
			}
			else if (this.IsMouseOver)
			{
				try
				{
					this.Background = this.FindResource("ActiveBackgroundBrushKey") as Brush;
				}
				catch (ResourceReferenceKeyNotFoundException ex)
				{
					Debug.WriteLine(ex);
				}
			}
			else
			{
				this.Background = Brushes.Transparent;
			}
		}
	}
}
