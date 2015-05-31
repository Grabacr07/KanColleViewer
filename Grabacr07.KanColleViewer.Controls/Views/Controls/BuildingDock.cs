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
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.Views.Controls
{
	/// <summary>
	/// 建造ドックを表示します。
	/// </summary>
	public class BuildingDock : Control
	{
		static BuildingDock()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BuildingDock), new FrameworkPropertyMetadata(typeof(BuildingDock)));
		}

		#region Number 依存関係プロパティ

		public int Number
		{
			get { return (int)this.GetValue(NumberProperty); }
			set { this.SetValue(NumberProperty, value); }
		}
		public static readonly DependencyProperty NumberProperty =
			DependencyProperty.Register("Number", typeof(int), typeof(BuildingDock), new UIPropertyMetadata(0));

		#endregion

		#region State 依存関係プロパティ

		public BuildingDockState State
		{
			get { return (BuildingDockState)this.GetValue(StateProperty); }
			set { this.SetValue(StateProperty, value); }
		}
		public static readonly DependencyProperty StateProperty =
			DependencyProperty.Register("State", typeof(BuildingDockState), typeof(BuildingDock), new UIPropertyMetadata(BuildingDockState.Locked));

		#endregion

		#region ShipName 依存関係プロパティ

		public string ShipName
		{
			get { return (string)this.GetValue(ShipNameProperty); }
			set { this.SetValue(ShipNameProperty, value); }
		}
		public static readonly DependencyProperty ShipNameProperty =
			DependencyProperty.Register("ShipName", typeof(string), typeof(BuildingDock), new UIPropertyMetadata(""));

		#endregion

		#region CompleteTime 依存関係プロパティ

		public string CompleteTime
		{
			get { return (string)this.GetValue(CompleteTimeProperty); }
			set { this.SetValue(CompleteTimeProperty, value); }
		}
		public static readonly DependencyProperty CompleteTimeProperty =
			DependencyProperty.Register("CompleteTime", typeof(string), typeof(BuildingDock), new UIPropertyMetadata(""));

		#endregion

		#region RemainingTime 依存関係プロパティ

		public string RemainingTime
		{
			get { return (string)this.GetValue(RemainingTimeProperty); }
			set { this.SetValue(RemainingTimeProperty, value); }
		}
		public static readonly DependencyProperty RemainingTimeProperty =
			DependencyProperty.Register("RemainingTime", typeof(string), typeof(BuildingDock), new UIPropertyMetadata(""));

		#endregion

		#region IsDisplayShipName 依存関係プロパティ

		public bool IsDisplayShipName
		{
			get { return (bool)this.GetValue(IsDisplayShipNameProperty); }
			set { this.SetValue(IsDisplayShipNameProperty, value); }
		}
		public static readonly DependencyProperty IsDisplayShipNameProperty =
			DependencyProperty.Register("IsDisplayShipName", typeof(bool), typeof(BuildingDock), new UIPropertyMetadata(false));

		#endregion

	}
}
