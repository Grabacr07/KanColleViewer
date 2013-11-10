using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Grabacr07.Desktop.Metro.Win32;

namespace Grabacr07.Desktop.Metro.Chrome
{
	internal class GlowWindowProcessorTop : GlowWindowProcessor
	{
		public override Orientation Orientation
		{
			get { return Orientation.Horizontal; }
		}

		public override HorizontalAlignment HorizontalAlignment
		{
			get { return HorizontalAlignment.Stretch; }
		}

		public override VerticalAlignment VerticalAlignment
		{
			get { return VerticalAlignment.Bottom; }
		}

		public override double GetLeft(double ownerLeft, double ownerWidth)
		{
			return ownerLeft;
		}

		public override double GetTop(double ownerTop, double ownerHeight)
		{
			return ownerTop - GlowSize;
		}

		public override double GetWidth(double ownerLeft, double ownerWidth)
		{
			return ownerWidth;
		}

		public override double GetHeight(double ownerTop, double ownerHeight)
		{
			return GlowSize;
		}

		public override HitTestValues GetHitTestValue(Point point, double actualWidht, double actualHeight)
		{
			var topLeft = new Rect(0, 0, EdgeSize - GlowSize, actualHeight);
			var topRight = new Rect(actualWidht - EdgeSize + GlowSize, 0, EdgeSize - GlowSize, actualHeight);

			return topLeft.Contains(point)
				? HitTestValues.HTTOPLEFT
				: topRight.Contains(point) ? HitTestValues.HTTOPRIGHT : HitTestValues.HTTOP;
		}

		public override Cursor GetCursor(Point point, double actualWidht, double actualHeight)
		{
			var topLeft = new Rect(0, 0, EdgeSize - GlowSize, actualHeight);
			var topRight = new Rect(actualWidht - EdgeSize + GlowSize, 0, EdgeSize - GlowSize, actualHeight);

			return topLeft.Contains(point)
				? Cursors.SizeNWSE
				: topRight.Contains(point) ? Cursors.SizeNESW : Cursors.SizeNS;
		}
	}
}
