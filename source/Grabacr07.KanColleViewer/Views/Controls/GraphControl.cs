using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Threading;
using ImageSource = System.Windows.Media.ImageSource;
using BitmapImage = System.Windows.Media.Imaging.BitmapImage;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.Views.Controls
{
	internal class GraphRenderer
	{
		private const int LeftMargin = 44;
		private const int BottomMargin = 20;

		public int ElementValue(int ElementType, ResourceModel Element)
		{
			switch (ElementType)
			{
				case 1: return Element.Fuel;
				case 2: return Element.Ammo;
				case 3: return Element.Steel;
				case 4: return Element.Bauxite;
				case 5: return Element.RepairBucket;
				case 6: return Element.DevelopmentMaterial;
				case 7: return Element.InstantConstruction;
				case 8: return Element.ImprovementMaterial;
			}
			return 0;
		}
		private Color[] colorTable => new Color[]
		{
			Color.Black,
			Color.FromArgb(0x60, 0xC0, 0x60),
			Color.FromArgb(0x90, 0x80, 0x60),
			Color.FromArgb(0xDC, 0xDC, 0xDC),
			Color.FromArgb(0xD0, 0xAC, 0x54),
			Color.FromArgb(0xA0, 0xD0, 0xA0),
			Color.FromArgb(0x44, 0xA0, 0x98),
			Color.FromArgb(0xD8, 0xC0, 0x60),
			Color.FromArgb(0xCC, 0xCC, 0xCC),
		};
		private Font TextFont => new Font("Segoe UI", 11.0f, GraphicsUnit.Pixel);

		private int step { get; set; }

		private int[] RenderHorizontalInfo(Graphics g, System.Drawing.Size sz, int Min, int Max)
		{
			List<int> yList = new List<int>();

			for (int i = Min - (Min % step); i <= Max; i += step)
			{
				var y = sz.Height - BottomMargin - 1 - (int)((float)(i - Min) / (Max - Min) * (sz.Height - BottomMargin - 2));
				yList.Add(y);

				g.DrawLine(
					new Pen(Color.FromArgb(0xC0, System.Drawing.SystemColors.ActiveBorder), 1.0f),
					new System.Drawing.Point(LeftMargin, y),
					new System.Drawing.Point(LeftMargin - 4, y)
				);

				StringFormat format = StringFormat.GenericDefault;
				format.Alignment = StringAlignment.Far;
				g.DrawString(
					i.ToString(),
					this.TextFont,
					Brushes.White,
					new System.Drawing.Point(LeftMargin - 4, (y - 6 < 0 ? 0 : y - 6)),
					format
				);
			}
			return yList.ToArray();
		}
		private void RenderHorizontalLine(Graphics g, System.Drawing.Size sz, int[] yList)
		{
			foreach (var y in yList)
			{
				g.DrawLine(
					new Pen(Color.FromArgb(0x40, System.Drawing.SystemColors.ActiveBorder), 1.28f),
					new System.Drawing.Point(LeftMargin, y),
					new System.Drawing.Point(sz.Width, y)
				);
			}
		}

		private int[] RenderVerticalInfo(Graphics g, System.Drawing.Size sz, DateTime begin, DateTime end)
		{
			List<int> xList = new List<int>();
			List<string> dtList = new List<string>();
			SizeF tsz = g.MeasureString("00/00", this.TextFont);
			int endMins = (int)(end.Subtract(DateTime.MinValue)).TotalMinutes;
			int bgnMins = (int)(begin.Subtract(DateTime.MinValue)).TotalMinutes;
			int xStep = (int)tsz.Width + 4; //  (sz.Width - 64 - 2) / ;

			for (int i = sz.Width - xStep / 2; i > LeftMargin; i -= xStep)
			{
				xList.Add(i);

				var dt = DateTime.MinValue;
				dt += new TimeSpan(0, (i - LeftMargin) * (endMins - bgnMins) / (sz.Width - LeftMargin) + bgnMins, 0);

				string str = dt.ToString("MM/dd");
				if (dtList.Contains(str)) continue;

				dtList.Add(str);

				g.DrawLine(
					new Pen(Color.FromArgb(0xC0, System.Drawing.SystemColors.ActiveBorder), 1.0f),
					new System.Drawing.Point(i, sz.Height - BottomMargin),
					new System.Drawing.Point(i, sz.Height - BottomMargin + 4)
				);

				StringFormat format = StringFormat.GenericDefault;
				format.Alignment = StringAlignment.Center;
				g.DrawString(
					str,
					this.TextFont,
					Brushes.White,
					new System.Drawing.Point(i, sz.Height - BottomMargin + 4),
					format
				);
			}
			return xList.ToArray();
		}
		private void RenderVerticalLine(Graphics g, System.Drawing.Size sz, int[] xList)
		{
			foreach (var x in xList)
			{
				g.DrawLine(
					new Pen(Color.FromArgb(0x40, System.Drawing.SystemColors.ActiveBorder), 1.28f),
					new System.Drawing.Point(x, 1),
					new System.Drawing.Point(x, sz.Height - BottomMargin - 2)
				);
			}
		}

		public void Render(Graphics g, System.Drawing.Size sz, IEnumerable<ResourceModel> Data, int[] ElementToDraw, DateTime beginTime, DateTime endTime, int YMin, int YMax)
		{
			var renderData = Data.OrderBy(x => x.Date);

			int length = (int)(endTime - beginTime).TotalMinutes;

			step = (YMax - YMin) / 2;
			step = (int)Math.Pow(10, (int)Math.Log10(step));
			if (step == 0) return;

			while ((YMax - YMin) / step < 4) step /= 2;
			while ((YMax - YMin) / step > 6) step *= 2;


			// Graph-box
			g.DrawRectangle(
				new Pen(Color.FromArgb(0xC0, System.Drawing.SystemColors.ActiveBorder)),
				new Rectangle(LeftMargin, 0, sz.Width - LeftMargin - 1, sz.Height - BottomMargin)
			);

			var yList = RenderHorizontalInfo(g, sz, YMin, YMax);
			var xList = RenderVerticalInfo(g, sz, beginTime, endTime);

			g.SetClip(new Rectangle(LeftMargin + 1, 1, sz.Width - LeftMargin - 2, sz.Height - BottomMargin));
			RenderHorizontalLine(g, sz, yList);
			RenderVerticalLine(g, sz, xList);

			foreach (var type in ElementToDraw)
			{
				var color = colorTable[type];

				GraphicsPath path = new GraphicsPath(FillMode.Winding);
				System.Drawing.Point pt;

				pt = new System.Drawing.Point(
					LeftMargin - 2 + 1 + (int)(((renderData.First().Date - beginTime).TotalMinutes / length) * (sz.Width - LeftMargin)),
					sz.Height - BottomMargin + 4 - 1
				);
				foreach (var item in renderData)
				{
					int value = ElementValue(type, item);

					System.Drawing.Point pt2 = new System.Drawing.Point(
						LeftMargin - 2 + 1 + (int)(((item.Date - beginTime).TotalMinutes / length) * (sz.Width - LeftMargin)),
						sz.Height - BottomMargin - 2 - (int)((float)(value - YMin) / (YMax - YMin) * (sz.Height - BottomMargin - 2))
					);

					path.AddLine(pt, pt2);
					pt = pt2;
				}
				path.AddLine(
					new System.Drawing.Point(
						pt.X,
						sz.Height - BottomMargin + 4 - 1
					),
					new System.Drawing.Point(
						pt.X,
						sz.Height - BottomMargin + 4 - 1
					)
				);

				g.FillPath(new SolidBrush(Color.FromArgb(51, color)), path);
				g.DrawPath(new Pen(color, 2.15f), path);
			}
			g.ResetClip();
		}
	}

	public class GraphControl : Grid
	{
		System.Windows.Controls.Image image;

		static GraphControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
				typeof(GraphControl),
				new FrameworkPropertyMetadata(typeof(GraphControl))
			);
		}
		public GraphControl()
		{
			this.SizeChanged += (s, e) =>
			{
				this.image.Source = null;
				new Thread(() =>
				{
					Thread.Sleep(100);
					this.Dispatcher.Invoke(() => Redraw());
				}).Start();
			};
			image = new System.Windows.Controls.Image();
			image.Stretch = System.Windows.Media.Stretch.Fill;
			this.Children.Add(image);
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			var Properties = new string[]
			{
				nameof(ElementToDraw),
				nameof(Data),
				nameof(BeginDate),
				nameof(EndDate),
				nameof(YMin),
				nameof(YMax)
			};
			if (Properties.Any(x => x == e.Property.Name))
				Redraw();
		}

		public int[] ElementToDraw
		{
			get { return (int[])this.GetValue(ElementToDrawProperty); }
			set
			{
				this.SetValue(ElementToDrawProperty, value);
				this.Redraw();
			}
		}
		public static readonly DependencyProperty ElementToDrawProperty =
			DependencyProperty.Register(nameof(ElementToDraw), typeof(int[]), typeof(GraphControl), new UIPropertyMetadata(new int[0]));

		public ResourceModel[] Data
		{
			get { return (ResourceModel[])this.GetValue(DataProperty); }
			set
			{
				this.SetValue(DataProperty, value);
				this.Redraw();
			}
		}
		public static readonly DependencyProperty DataProperty =
			DependencyProperty.Register(nameof(Data), typeof(ResourceModel[]), typeof(GraphControl), new UIPropertyMetadata(new ResourceModel[0]));

		public DateTime BeginDate
		{
			get { return (DateTime)this.GetValue(BeginDateProperty); }
			set
			{
				this.SetValue(BeginDateProperty, value);
				this.Redraw();
			}
		}
		public static readonly DependencyProperty BeginDateProperty =
			DependencyProperty.Register(nameof(BeginDate), typeof(DateTime), typeof(GraphControl), new UIPropertyMetadata(DateTime.MinValue));

		public DateTime EndDate
		{
			get { return (DateTime)this.GetValue(EndDateProperty); }
			set
			{
				this.SetValue(EndDateProperty, value);
				this.Redraw();
			}
		}
		public static readonly DependencyProperty EndDateProperty =
			DependencyProperty.Register(nameof(EndDate), typeof(DateTime), typeof(GraphControl), new UIPropertyMetadata(DateTime.MinValue));

		public int YMin
		{
			get { return (int)this.GetValue(YMinProperty); }
			set
			{
				this.SetValue(YMinProperty, value);
				this.Redraw();
			}
		}
		public static readonly DependencyProperty YMinProperty =
			DependencyProperty.Register(nameof(YMin), typeof(int), typeof(GraphControl), new UIPropertyMetadata(0));

		public int YMax
		{
			get { return (int)this.GetValue(YMaxProperty); }
			set
			{
				this.SetValue(YMaxProperty, value);
				this.Redraw();
			}
		}
		public static readonly DependencyProperty YMaxProperty =
			DependencyProperty.Register(nameof(YMax), typeof(int), typeof(GraphControl), new UIPropertyMetadata(0));

		private void Redraw()
		{
			System.Drawing.Size sz = new System.Drawing.Size(
				(int)this.ActualWidth,
				(int)this.ActualHeight
			);

			if (sz.Width <= 0 || sz.Height <= 0) return;
			if (ElementToDraw == null || ElementToDraw.Length == 0) return;
			if (Data == null || Data.Length == 0) return;

			var buffer = new Bitmap(
				sz.Width,
				sz.Height,
				PixelFormat.Format32bppArgb
			);
			using (Graphics g = Graphics.FromImage(buffer))
			{
				g.SmoothingMode = SmoothingMode.HighQuality;
				g.InterpolationMode = InterpolationMode.HighQualityBilinear;
				g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;

				var renderer = new GraphRenderer();
				renderer.Render(g, sz, Data, ElementToDraw, BeginDate, EndDate, YMin, YMax);
			}

			this.image.Source = BitmapToImageSource(buffer);
		}

		private static ImageSource BitmapToImageSource(Bitmap bitmap)
		{
			MemoryStream ms = new MemoryStream();
			ImageSource image = null;

			bitmap.Save(ms, ImageFormat.Png);
			ms.Position = 0;

			BitmapImage bi = new BitmapImage();
			bi.BeginInit();
			bi.StreamSource = ms;
			bi.EndInit();
			bi.Freeze();

			Dispatcher.CurrentDispatcher.Invoke(() => image = bi);
			return image;
		}
	}
}
