using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class RemodelListWindowViewModel : WindowViewModel
	{
		public IEnumerable<string> WeekDayList { get; private set; }
		public static Dictionary<string, int> WeekDayTable = new Dictionary<string, int>
		{
			{"일요일", 1}, {"월요일", 2}, {"화요일", 3}, {"수요일", 4}, {"목요일", 5},
			{"금요일", 6}, {"토요일", 7}
		};

		private XDocument RemodelXML;
		string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
		List<RemodelItemList> tempimp = new List<RemodelItemList>();
		List<RemodelItemList> tempUse = new List<RemodelItemList>();

		#region FirstList 変更通知プロパティ

		private List<RemodelItemList> _FirstList;

		public List<RemodelItemList> FirstList
		{
			get { return _FirstList; }
			set
			{
				if (_FirstList != value)
				{
					_FirstList = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region SecondList 変更通知プロパティ

		private List<RemodelItemList> _SecondList;

		public List<RemodelItemList> SecondList
		{
			get { return _SecondList; }
			set
			{
				if (_SecondList != value)
				{
					_SecondList = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ThirdList 変更通知プロパティ

		private List<RemodelItemList> _ThirdList;

		public List<RemodelItemList> ThirdList
		{
			get { return _ThirdList; }
			set
			{
				if (_ThirdList != value)
				{
					_ThirdList = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Improvement 変更通知プロパティ

		private List<RemodelItemList> _Improvement;

		public List<RemodelItemList> Improvement
		{
			get { return _Improvement; }
			set
			{
				if (_Improvement != value)
				{
					_Improvement = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region UseItemList 変更通知プロパティ

		private List<RemodelItemList> _UseItemList;

		public List<RemodelItemList> UseItemList
		{
			get { return _UseItemList; }
			set
			{
				if (_UseItemList != value)
				{
					_UseItemList = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region WeekDayView 変更通知プロパティ

		private int _WeekDayView;

		public int WeekDayView
		{
			get { return _WeekDayView; }
			set
			{
				if (_WeekDayView != value)
				{
					_WeekDayView = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region SelectedDay 変更通知プロパティ

		private string _SelectedDay;

		public string SelectedDay
		{
			get { return this._SelectedDay; }
			set
			{
				if (_SelectedDay != value)
				{
					this._SelectedDay = value;
					this.RaisePropertyChanged();
					this.Update(false);
				}
			}
		}

		#endregion
		public RemodelListWindowViewModel()
		{
			this.Title = "개수공창 리스트";
			this.WeekDayList = WeekDayTable.Keys.ToList();

			Update();
		}
		private void Update(bool IsStart = true)
		{
			if (File.Exists(Path.Combine(MainFolder, "Translations", "RemodelSlots.xml")))
			{
				if (IsStart) WeekDayView = (int)DateTime.Today.DayOfWeek + 1;
				else WeekDayView = WeekDayTable[SelectedDay];
				WeekDayFlag today = new WeekDayFlag();
				switch (WeekDayView)
				{
					case 1:
						today |= WeekDayFlag.Sunday;
						if (IsStart) SelectedDay = "일요일";
						break;
					case 2:
						today |= WeekDayFlag.Monday;
						if (IsStart) SelectedDay = "월요일";
						break;
					case 3:
						today |= WeekDayFlag.Tuesday;
						if (IsStart) SelectedDay = "화요일";
						break;
					case 4:
						today |= WeekDayFlag.Wednesday;
						if (IsStart) SelectedDay = "수요일";
						break;
					case 5:
						today |= WeekDayFlag.Thursday;
						if (IsStart) SelectedDay = "목요일";
						break;
					case 6:
						today |= WeekDayFlag.Friday;
						if (IsStart) SelectedDay = "금요일";
						break;
					case 7:
						today |= WeekDayFlag.Saturday;
						if (IsStart) SelectedDay = "토요일";
						break;
				}
				this.RemodelXML = XDocument.Load(Path.Combine(MainFolder, "Translations", "RemodelSlots.xml"));
				IEnumerable<XElement> RemodelList = GetRemodelList();
				var Position = "Position";
				var Weekday = "AllWeekdays";
				//RemodelList에서 오늘 개수공창 목록에 들어갈것들을 선별한다.
				RemodelList = RemodelList.Where(f => WeekDaySetter(Convert.ToInt32(f.Element(Weekday).Value)).HasFlag(today));
				//선별된 목록을 Position값을 이용해 상/중/하 그룹으로 나눈다
				IEnumerable<XElement> First = RemodelList.Where(f => f.Element(Position).Value.Equals("1")).ToList();
				IEnumerable<XElement> Second = RemodelList.Where(f => f.Element(Position).Value.Equals("2")).ToList();
				IEnumerable<XElement> Third = RemodelList.Where(f => f.Element(Position).Value.Equals("3")).ToList();

			}
		}
		private void MakeDefaultList()
		{

		}
		private WeekDayFlag WeekDaySetter(int weekint)
		{
			if (weekint.ToString().Count() == 7)
			{
				return WeekDayFlag.All;
			}

			int var = weekint;
			int len = 1;
			int div;

			while (true)
			{
				div = var / 10;
				if (div == 0) break;
				len++;
				var = div;
			}
			WeekDayFlag weekday = new WeekDayFlag();
			for (int i = 0; i < len; i++)
			{
				var temp = weekint.ToString();
				int inttemp = Convert.ToInt32(temp[i].ToString());
				switch (inttemp)
				{
					case 1:
						weekday |= WeekDayFlag.Sunday;
						break;
					case 2:
						weekday |= WeekDayFlag.Monday;
						break;
					case 3:
						weekday |= WeekDayFlag.Tuesday;
						break;
					case 4:
						weekday |= WeekDayFlag.Wednesday;
						break;
					case 5:
						weekday |= WeekDayFlag.Thursday;
						break;
					case 6:
						weekday |= WeekDayFlag.Friday;
						break;
					case 7:
						weekday |= WeekDayFlag.Saturday;
						break;
				}
			}
			return weekday;
		}
		private IEnumerable<XElement> GetRemodelList()
		{
			if (RemodelXML != null)
			{
				if (KanColleClient.Current.Updater.RemodelUpdate)
				{
					this.RemodelXML = XDocument.Load(Path.Combine(MainFolder, "Translations", "RemodelSlots.xml"));
					KanColleClient.Current.Updater.RemodelUpdate = false;
				}
				return RemodelXML.Descendants("RemodelSlot");
			}
			return null;
		}
	}
	#region 목록
	public class RemodelItemList
	{
		public string ItemName { get; set; }
		public WeekDayFlag TotalWeekday { get; set; }
		public SlotItemIconType? IconType { get; set; }
		public string ToolTipString { get; set; }
		public string UseEquip { get; set; }
		public string ShipName { get; set; }
		//public string Upgrade { get; set; }
		public SlotItemIconType? UpgradeIconType { get; set; }
		public List<ShipInfo> Ships { get; set; }
	}
	public class ShipInfo
	{
		public WeekDayFlag Weekday { get; set; }
		public string Upgrade { get; set; }
		public string ShipName { get; set; }
		public SlotItemIconType? UpgradeIconType { get; set; }
	}

	[Flags]
	public enum WeekDayFlag
	{
		None = 0,
		Sunday = 1,
		Monday = 1 << 1,
		Tuesday = 1 << 2,
		Wednesday = 1 << 3,
		Thursday = 1 << 4,
		Friday = 1 << 5,
		Saturday = 1 << 6,
		NotNeedShip = 1 << 7,
		All = Monday | Tuesday | Wednesday | Thursday | Friday | Saturday | Sunday,
	}
	#endregion
}