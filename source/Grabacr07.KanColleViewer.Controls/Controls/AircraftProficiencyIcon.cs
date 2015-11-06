using System.Windows;
using System.Windows.Controls;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.Controls
{
	public class AircraftProficiencyIcon : Control
	{
		static AircraftProficiencyIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
				typeof(AircraftProficiencyIcon),
				new FrameworkPropertyMetadata(typeof(AircraftProficiencyIcon)));
		}

		public int Level
		{
			get { return (int)this.GetValue(LevelProperty); }
			set { this.SetValue(LevelProperty, value); }
		}

		public static readonly DependencyProperty LevelProperty =
			DependencyProperty.Register(nameof(Level), typeof(int), typeof(AircraftProficiencyIcon), new UIPropertyMetadata(0));
	}
}