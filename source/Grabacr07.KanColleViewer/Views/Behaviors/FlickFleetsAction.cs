using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Interactivity;
using Grabacr07.KanColleViewer.ViewModels.Contents.Fleets;

namespace Grabacr07.KanColleViewer.Views.Behaviors
{
	public enum FlickOrientation
	{
		Horizontal,
		Vertical
	}

	public class FlickFleetsAction : TargetedTriggerAction<FleetsViewModel>
	{
		public FlickOrientation Orientation { get; set; }

	    public bool IsCyclic { get; set; }

		public FlickFleetsAction()
		{
			this.Orientation = FlickOrientation.Horizontal;
		}

		protected override void Invoke(object parameter)
		{
			if (!(parameter is FlickDirection)) return;

			switch ((FlickDirection)parameter)
			{
				case FlickDirection.Up:
					if (this.Orientation == FlickOrientation.Vertical) this.Next();
					break;
				case FlickDirection.Down:
					if (this.Orientation == FlickOrientation.Vertical) this.Previous();
					break;
				case FlickDirection.Left:
					if (this.Orientation == FlickOrientation.Horizontal) this.Next();
					break;
				case FlickDirection.Right:
					if (this.Orientation == FlickOrientation.Horizontal) this.Previous();
					break;
			}
		}

		private void Next()
		{
			if (this.Target == null) return;

			var selectedIndex = Array.IndexOf(this.Target.Fleets, this.Target.SelectedFleet);
			if (selectedIndex < this.Target.Fleets.Length - 1)
			{
				this.Target.SelectedFleet = this.Target.Fleets[selectedIndex + 1];
			}
			else if (this.IsCyclic)
			{
				this.Target.SelectedFleet = this.Target.Fleets.First();
			}
		}

		private void Previous()
		{
			if (this.Target == null) return;

			var selectedIndex = Array.IndexOf(this.Target.Fleets, this.Target.SelectedFleet);
			if (selectedIndex > 0)
			{
				this.Target.SelectedFleet = this.Target.Fleets[selectedIndex - 1];
			}
			else if (this.IsCyclic)
			{
				this.Target.SelectedFleet = this.Target.Fleets.Last();
			}
		}
	}
}
