using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Grabacr07.KanColleViewer.Views.Controls
{
	/// <summary>
	/// PreviewPopUpButton.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class PreviewPopUpButton
	{
		public PreviewPopUpButton()
		{
			InitializeComponent();
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
			if (this.IsMouseOver)
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
