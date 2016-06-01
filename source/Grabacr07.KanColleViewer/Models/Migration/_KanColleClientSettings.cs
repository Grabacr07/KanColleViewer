using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Grabacr07.KanColleWrapper;

namespace Grabacr07.KanColleViewer.Models.Migration
{
	/// <summary>互換性のために残されています。</summary>
	[Obsolete]
	[Serializable]
	[XmlRoot("KanColleClientSettings")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	// ReSharper disable once InconsistentNaming
	public class _KanColleClientSettings : IKanColleClientSettings
	{
		public int NotificationShorteningTime { get; set; }
		public int ReSortieCondition { get; set; }
		public string ViewRangeCalcType { get; set; }
		public bool IsViewRangeCalcIncludeFirstFleet { get; set; }
		public bool IsViewRangeCalcIncludeSecondFleet { get; set; }
		public bool CheckFlagshipIsRepairShip { get; set; }

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { }
			remove { }
		}
	}
}
