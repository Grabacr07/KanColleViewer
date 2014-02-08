using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Grabacr07.KanColleViewer.Views.Behaviors
{
	public enum FlickDirection
	{
		Up,
		Down,
		Right,
		Left
	}

	public class FlickTrigger : TriggerBase<UIElement>
	{
		private StylusPointCollection downPoints;

		protected override void OnAttached()
		{
			base.OnAttached();
			this.AssociatedObject.StylusDown += OnStylusDown;
			this.AssociatedObject.StylusSystemGesture += OnStylusSystemGesture;
		}

		protected override void OnDetaching()
		{
			this.AssociatedObject.StylusDown -= OnStylusDown;
			this.AssociatedObject.StylusSystemGesture -= OnStylusSystemGesture;
			base.OnDetaching();
		}

		private void OnStylusSystemGesture(object sender, StylusSystemGestureEventArgs e)
		{
			if (e.SystemGesture != SystemGesture.Drag)
				return;

			if (this.downPoints == null)
				return;

			var hitTestResult = VisualTreeHelper.HitTest(this.AssociatedObject, downPoints[0].ToPoint());
			if (hitTestResult == null)
				return;

			var newPoints = e.GetStylusPoints(this.AssociatedObject);
			if (newPoints.Count <= 0 || this.downPoints.Count <= 0)
				return;

			if (this.AssociatedObject is Button)
				this.AssociatedObject.ReleaseStylusCapture();

			var distanceX = newPoints[0].X - this.downPoints[0].X;
			var distanceY = newPoints[0].Y - this.downPoints[0].Y;

			if (Math.Abs(distanceX) > Math.Abs(distanceY))
			{
				this.InvokeActions(distanceX < 0
					? FlickDirection.Left
					: FlickDirection.Right);
			}
			else
			{
				this.InvokeActions(distanceY < 0
					? FlickDirection.Down
					: FlickDirection.Up);
			}
		}

		private void OnStylusDown(object sender, StylusDownEventArgs e)
		{
			this.downPoints = e.GetStylusPoints(AssociatedObject);
		}
	}
}
