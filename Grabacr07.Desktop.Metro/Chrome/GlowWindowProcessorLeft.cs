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
	internal class GlowWindowProcessorLeft : GlowWindowProcessor
	{
		public override Orientation Orientation
		{
			get { return Orientation.Vertical; }
		}

		public override HorizontalAlignment HorizontalAlignment
		{
			get { return HorizontalAlignment.Right; }
		}

		public override VerticalAlignment VerticalAlignment
		{
			get { return VerticalAlignment.Stretch; }
		}

		public override double GetLeft(double ownerLeft, double ownerWidth)
		{
			return ownerLeft - GlowSize;
		}

		public override double GetTop(double ownerTop, double ownerHeight)
		{
			return ownerTop - GlowSize;
		}

		public override double GetWidth(double ownerLeft, double ownerWidth)
		{
			return GlowSize;
		}

		public override double GetHeight(double ownerTop, double ownerHeight)
		{
			return ownerHeight + GlowSize * 2;
		}

		public override HitTestValues GetHitTestValue(Point point, double actualWidht, double actualHeight)
		{
			var leftTop = new Rect(0, 0, actualWidht, EdgeSize);
			var leftBottom = new Rect(0, actualHeight - EdgeSize, actualWidht, EdgeSize);

			return leftTop.Contains(point)
				? HitTestValues.HTTOPLEFT
				: leftBottom.Contains(point) ? HitTestValues.HTBOTTOMLEFT : HitTestValues.HTLEFT;
		}

		public override Cursor GetCursor(Point point, double actualWidht, double actualHeight)
		{
			var leftTop = new Rect(0, 0, actualWidht, EdgeSize);
			var leftBottom = new Rect(0, actualHeight - EdgeSize, actualWidht, EdgeSize);

			return leftTop.Contains(point)
				? Cursors.SizeNWSE
				: leftBottom.Contains(point) ? Cursors.SizeNESW : Cursors.SizeWE;
		}
	}
}
