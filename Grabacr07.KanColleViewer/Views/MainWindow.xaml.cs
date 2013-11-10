using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.Win32;
using mshtml;
using SHDocVw;
using IServiceProvider = Grabacr07.KanColleViewer.Win32.IServiceProvider;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Grabacr07.KanColleViewer.Views
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var doc = this.WebBrowser.Document as HTMLDocument;
			if (doc != null)
			{
				var target = doc.getElementById("game_frame").document as HTMLDocument;
				if (target != null)
				{
					var frames = target.frames;

					for (var i = 0; i < frames.length; i++)
					{
						var item = frames.item(i);
						var provider = item as IServiceProvider;
						if (provider != null)
						{
							object o;
							var w = provider.QueryService(typeof (IWebBrowserApp).GUID, typeof (IWebBrowser2).GUID, out o);

							var b = o as IWebBrowser2;
							if (b != null)
							{
								var doc2 = b.Document as HTMLDocument;
								if (doc2 != null)
								{
									var swf = doc2.getElementById("externalswf") as HTMLEmbed;
									if (swf != null)
									{
										var viewObject = swf as IViewObject;
										if (viewObject != null)
										{
											var width = int.Parse(swf.width);
											var height = int.Parse(swf.height);

											var image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
											var rect1 = new RECT { left = 0, top = 0, width = width, height = height };
											var rect2 = new RECT { left = 0, top = 0, width = width, height = height };
											var tdevice = new DVTARGETDEVICE { tdSize = 0, };

											using (var g = Graphics.FromImage(image))
											{
												var hdc = g.GetHdc();
												var result = viewObject.Draw(1, 0, IntPtr.Zero, tdevice, IntPtr.Zero, hdc, rect1, null, IntPtr.Zero, IntPtr.Zero);

												g.ReleaseHdc(hdc);
											}

											image.Save("test.png", ImageFormat.Png);
										}
									}
								}
							}
						}
					}
				}


				//if (frame != null)
				//{
				//	var targe = frame.getElementById("externalswf");
				//}

				//var flash = doc.getElementById("externalswf");
			}
		}
	}
}
