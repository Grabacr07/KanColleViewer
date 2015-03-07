using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using System;
using System.Collections.Generic;
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

		#region FirstList 変更通知プロパティ

		private List<RemodelShipList> _FirstList;

		public List<RemodelShipList> FirstList
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

		private List<RemodelShipList> _SecondList;

		public List<RemodelShipList> SecondList
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

		private List<RemodelShipList> _ThirdList;

		public List<RemodelShipList> ThirdList
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

				IEnumerable<XElement> First = RemodelList.Where(f => f.Element(Position).Value.Equals("1")).ToList();
				IEnumerable<XElement> Second = RemodelList.Where(f => f.Element(Position).Value.Equals("2")).ToList();
				IEnumerable<XElement> Third = RemodelList.Where(f => f.Element(Position).Value.Equals("3")).ToList();

				this.FirstList = new List<RemodelShipList>(MakeUpList(First, today));
				this.SecondList = new List<RemodelShipList>(MakeUpList(Second, today));
				this.ThirdList = new List<RemodelShipList>(MakeUpList(Third, today));
			}
		}
		private List<RemodelShipList> MakeUpList(IEnumerable<XElement> remodellist, WeekDayFlag today)
		{
			List<RemodelShipList> templist = new List<RemodelShipList>();

			foreach (var item in remodellist)
			{
				var temp = new RemodelShipList();
				if (item.Element("SlotItemName") != null)
				{
					temp.ItemName = KanColleClient.Current.Translations.GetTranslation(item.Element("SlotItemName").Value, TranslationType.Equipment);
					foreach (var slotitem in KanColleClient.Current.Master.SlotItems)
					{
						if (slotitem.Value.Name == temp.ItemName)
							temp.IconType = slotitem.Value.IconType;
					}
				}
				if (item.Element("ShipName1") != null)
				{
					if (WeekDaySetter(Convert.ToInt32(item.Element("WeekDays1").Value)).HasFlag(today))
					{
						temp.ShipName1 = KanColleClient.Current.Translations.GetTranslation(item.Element("ShipName1").Value, TranslationType.Ships);
					}
				}
				if (item.Element("ShipName2") != null)
				{
					if (WeekDaySetter(Convert.ToInt32(item.Element("WeekDays2").Value)).HasFlag(today))
					{
						temp.ShipName2 = KanColleClient.Current.Translations.GetTranslation(item.Element("ShipName2").Value, TranslationType.Ships);
					}
				}
				if (item.Element("AllWeekdays") != null) temp.TotalWeekday = WeekDaySetter(Convert.ToInt32(item.Element("AllWeekdays").Value));
				if (item.Element("ToolTip") != null)
					temp.ToolTipString = item.Element("ToolTip").Value;
				else temp.ToolTipString = "특이사항 없음";
                if (temp.TotalWeekday.HasFlag(today))
					templist.Add(temp);
			}

			return templist;
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
	public class RemodelShipList
	{
		public string ItemName { get; set; }
		public string ShipName1 { get; set; }
		public string ShipName2 { get; set; }
		public WeekDayFlag TotalWeekday { get; set; }
		public SlotItemIconType? IconType { get; set; }
		public string ToolTipString { get; set; }
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
		All = Monday | Tuesday | Wednesday | Thursday | Friday | Saturday | Sunday,
	}
	#endregion
}