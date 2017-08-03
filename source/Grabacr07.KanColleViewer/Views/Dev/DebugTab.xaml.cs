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
using System.Windows.Threading;
using System.Threading;
using System.IO;

namespace Grabacr07.KanColleViewer.Views.Dev
{
	/// <summary>
	/// DebugTab.xaml の相互作用ロジック
	/// </summary>
	public partial class DebugTab : UserControl
	{
		public DebugTab()
		{
			InitializeComponent();
		}

		public void ExportSlotitemIcons()
		{
			new Thread(() =>
			{
				var list = Enum.GetValues(typeof(KanColleWrapper.Models.SlotItemIconType));
				foreach (var item in list)
				{
					SlotItem.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
					{
						SlotItem.Type = (KanColleWrapper.Models.SlotItemIconType)item;
					}));

					Thread.Sleep(50);

					SlotItem.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
					{
						ExportCanvas(SlotItem, item.ToString() + ".png");
					}));
				}
			}).Start();
		}

		private void ExportCanvas(Control elem, string filename)
		{
			string path = System.IO.Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
				"icons",
				filename
			);
			var size = elem.RenderSize;
			int width = (int)elem.Width;
			int height = (int)elem.Height;

			RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
				(int)size.Width,
				(int)size.Height,
				96, 96,
				PixelFormats.Pbgra32
			);

			elem.Measure(size); //Important
			elem.Arrange(new Rect(size)); //Important

			DrawingVisual visual = new DrawingVisual();
			using (DrawingContext context = visual.RenderOpen())
			{
				VisualBrush brush = new VisualBrush(elem);
				brush.Stretch = Stretch.Uniform;
				context.DrawRectangle(
					brush,
					null,
					new Rect(
						new Point(),
						new Size(elem.Width, elem.Height)
					)
				);
			}
			renderBitmap.Render(visual);

			using (FileStream fs = new FileStream(path, FileMode.Create))
			{
				BitmapEncoder encoder = new PngBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
				encoder.Save(fs);
			}
		}

		private void ExportSlotitemIcons(object sender, RoutedEventArgs e)
		{
			ExportSlotitemIcons();
		}
	}
}
