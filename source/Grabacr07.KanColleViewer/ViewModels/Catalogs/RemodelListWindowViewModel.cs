using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using MetroTrilithon.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Grabacr07.KanColleViewer.ViewModels.Contents;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class RemodelListWindowViewModel : WindowViewModel
	{
		public IEnumerable<string> WeekDayList { get; private set; }
		public static Dictionary<string, int> WeekDayTable = new Dictionary<string, int>
		{
			{"일요일", 1}, {"월요일", 2}, {"화요일", 3}, {"수요일", 4}, {"목요일", 5},
			{"금요일", 6}, {"토요일", 7}
		};
		private WeekDayFlag today { get; set; }

		private XDocument RemodelXML;
		string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

		#region IsLoading 변경통지 프로퍼티

		private bool _IsLoading;
		public bool IsLoading
		{
			get { return this._IsLoading; }
			set
			{
				this._IsLoading = value;
				this.RaisePropertyChanged();
			}
		}

		#endregion

		#region OnlyOwnSlotItems 변경통지 프로퍼티

		private bool _OnlyOwnSlotItems;
		public bool OnlyOwnSlotItems
		{
			get { return this._OnlyOwnSlotItems; }
			set
			{
				this._OnlyOwnSlotItems = value;
				this.RaisePropertyChanged();
				this.Update();
			}
		}

		#endregion

		#region OnlyRemodeledSlotItems 변경통지 프로퍼티

		private bool _OnlyRemodeledSlotItems;
		public bool OnlyRemodeledSlotItems
		{
			get { return this._OnlyRemodeledSlotItems; }
			set
			{
				this._OnlyRemodeledSlotItems = value;
				this.RaisePropertyChanged();
				this.Update();
			}
		}

		#endregion

		#region RemodelFilters 프로퍼티

		public ICollection<RemodelFilterViewModel> RemodelFilters { get; }

		#endregion

		#region RemodelFilterSelected 변경통지 프로퍼티

		private RemodelFilterViewModel _RemodelFilterSelected;
		public RemodelFilterViewModel RemodelFilterSelected
		{
			get { return this._RemodelFilterSelected; }
			set
			{
				if (this._RemodelFilterSelected != value)
				{
					this._RemodelFilterSelected = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

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
					this.Update();
				}
			}
		}

		#endregion

		private bool Initialized;

		public RemodelListWindowViewModel()
		{
			Initialized = false;

			this.Title = "개수공창 리스트";
			this.WeekDayList = WeekDayTable.Keys.ToList();
			this.SelectedDay = this.WeekDayList.FirstOrDefault();


			var list = new List<RemodelFilterViewModel>();

			var icons = Enum.GetNames(typeof(SlotItemIconType));
			list.Add(new RemodelFilterViewModel("All", null));

			foreach (var item in icons)
				list.Add(new RemodelFilterViewModel(item, (SlotItemIconType)Enum.Parse(typeof(SlotItemIconType), item)));

			this.RemodelFilters = list;
			this.RemodelFilterSelected = this.RemodelFilters.FirstOrDefault();

			Initialized = true;
			Update(true);
		}
		private void Update(bool IsStart = false)
		{
			if (File.Exists(Path.Combine(MainFolder, "Translations", "RemodelSlots.xml")))
			{
				if (!this.Initialized || this.IsLoading) return;
				this.IsLoading = true;

				new System.Threading.Thread(() =>
				{
					if (IsStart) WeekDayView = (int)DateTime.Today.DayOfWeek + 1;
					else WeekDayView = WeekDayTable[SelectedDay];

					today = (WeekDayFlag)(1 << (WeekDayView - 1));
					if (IsStart)
					{
						switch (WeekDayView)
						{
							case 1:
								SelectedDay = "일요일";
								break;
							case 2:
								SelectedDay = "월요일";
								break;
							case 3:
								SelectedDay = "화요일";
								break;
							case 4:
								SelectedDay = "수요일";
								break;
							case 5:
								SelectedDay = "목요일";
								break;
							case 6:
								SelectedDay = "금요일";
								break;
							case 7:
								SelectedDay = "토요일";
								break;
						}
					}
					this.RemodelXML = XDocument.Load(Path.Combine(MainFolder, "Translations", "RemodelSlots.xml"));
					IEnumerable<XElement> RemodelList = GetRemodelList();

					var Weekday = "AllWeekdays";
					//RemodelList에서 오늘 개수공창 목록에 들어갈것들을 선별한다.
					RemodelList = RemodelList.Where(f => WeekDaySetter(Convert.ToInt32(f.Element(Weekday).Value)).HasFlag(today));

					// 소지 장비로 필터
					if (this.OnlyOwnSlotItems)
					{
						RemodelList = RemodelList.Where(x =>
						{
							var name = x.Element("SlotItemName").Value;
							var itemid = KanColleClient.Current.Master.SlotItems
								.Where(y => y.Value.RawData.api_name == name)
								.FirstOrDefault()
								.Value.Id;

							return KanColleClient.Current.Homeport.Itemyard.SlotItems.Any(y => y.Value.Info.Id == itemid);
						});
					}

					// 개수된 장비로 필터
					if(this.OnlyRemodeledSlotItems)
					{
						RemodelList = RemodelList.Where(x =>
						{
							var name = x.Element("SlotItemName").Value;
							var itemid = KanColleClient.Current.Master.SlotItems
								.Where(y => y.Value.RawData.api_name == name)
								.FirstOrDefault()
								.Value.Id;

							return KanColleClient.Current.Homeport.Itemyard.SlotItems.Any(y => y.Value.Info.Id == itemid && y.Value.Level > 0);
						});
					}

					// 아이콘으로 필터
					if (this.RemodelFilterSelected.Display.HasValue)
					{
						RemodelList = RemodelList.Where(x =>
						{
							var name = x.Element("SlotItemName").Value;
							var icon = KanColleClient.Current.Master.SlotItems
								.Where(y => y.Value.RawData.api_name == name)
								.Select(y => y.Value.IconType)
								.FirstOrDefault();

							// Not registered icon yet (Unknown icon)
							if (Enum.GetName(typeof(SlotItemIconType), icon) == null)
								icon = SlotItemIconType.Unknown;

							return icon == this.RemodelFilterSelected.Display.Value;
						});
					}


					//상중하 리스트를 작성->상중하의 구분을 제거
					var list = MakeDefaultList(RemodelList).ToList();
					var _FirstList = SortList(list);

					//소모아이템 리스트를 작성
					var use = MakeUseItemList(RemodelList);
					var _UseItemList = SortList(use.ToList());

					//개조 목록을 작성
					var im = MakeUpgradeList(RemodelList);
					var _Improvement = SortList(im.ToList());

					Grabacr07.KanColleViewer.Application.Current.Dispatcher.Invoke(() =>
					{
						this.FirstList = _FirstList;
						this.UseItemList = _UseItemList;
						this.Improvement = _Improvement;
						this.IsLoading = false;
					});
				}).Start();
			}
		}
		private List<RemodelItemList> SortList(List<RemodelItemList> myList)
		{
			myList.Sort(delegate (RemodelItemList x, RemodelItemList y)
			{
				SlotItemIconType? a = x.IconType, b = y.IconType;

				if (Enum.GetName(typeof(SlotItemIconType), a) == null) a = SlotItemIconType.Unknown;
				if (Enum.GetName(typeof(SlotItemIconType), b) == null) b = SlotItemIconType.Unknown;

				return ((int)a.Value).CompareTo((int)b.Value);
			});
			return myList;
		}
		private List<ShipInfo> TrimShipList(List<ShipInfo> ShipList)
		{
			for (int i = 0; i < ShipList.Count; i++)
			{
				for (int j = i + 1; j < ShipList.Count; j++)
				{
					if (ShipList[i].Upgrade == ShipList[j].Upgrade)
					{
						ShipList[i].IsSameUpgradeExist = true;
						ShipList[j].IsSameUpgradeExist = true;
					}
				}
			}
			var RemoveSameShip = ShipList.Where(x => !x.IsSameUpgradeExist).ToList();
			var SameShipList = ShipList.Where(x => x.IsSameUpgradeExist).ToList();

			if (SameShipList.Count > 1)
			{
				ShipInfo mergedShip = new ShipInfo();
				string MergedName = SameShipList[0].ShipName;

				for (int i = 1; i < SameShipList.Count; i++)
				{
					MergedName = MergedName + ", " + SameShipList[i].ShipName;
				}

				mergedShip = SameShipList[0];
				mergedShip.ShipName = MergedName;
				RemoveSameShip.Add(mergedShip);
			}


			return RemoveSameShip;
		}
		private List<ShipInfo> MakeShipList(XElement Context, bool IsImprovementList = false)
		{
			List<ShipInfo> ShipList = new List<ShipInfo>();

			bool Checker = true;
			int ShipCount = 1;

			while (Checker)
			{
				//이름작성
				string ShipElement = "ShipName";
				var temp = ShipElement + ShipCount.ToString();

				ShipInfo Ship = new ShipInfo();
				Ship.IsSameUpgradeExist = false;
				if (Context.Element(temp) != null)
				{
					Ship.ShipName = Context.Element(temp).Value;
					Ship.ShipName = KanColleClient.Current.Translations.GetTranslation(Ship.ShipName, TranslationType.Ships, false);
				}
				else Ship.ShipName = string.Empty;
				//업그레이드 부분
				ShipElement = "Upgrade";
				temp = ShipElement + ShipCount.ToString();

				if (Context.Element(temp) != null)
				{
					Ship.Upgrade = Context.Element(temp).Value;
					Ship.Upgrade = KanColleClient.Current.Translations.GetTranslation(Ship.Upgrade, TranslationType.Equipment, false);

					//슬롯 아이템 Master목록에서 해당되는 아이콘을 찾아 삽입
					foreach (var slotitem in KanColleClient.Current.Master.SlotItems)
					{
						if (slotitem.Value.Name == Ship.Upgrade)
							Ship.UpgradeIconType = slotitem.Value.IconType;
					}
				}
				else
				{
					Ship.Upgrade = null;
					Ship.UpgradeIconType = null;
				}
				ShipElement = "WeekDays";
				temp = ShipElement + ShipCount.ToString();
				if (Context.Element(temp) != null)
				{
					int weekday = Convert.ToInt32(Context.Element(temp).Value);
					Ship.Weekday = WeekDaySetter(weekday);
				}
				else Ship.Weekday = WeekDayFlag.None;

				if (Ship.Weekday == WeekDayFlag.None && Ship.Upgrade == null && Ship.ShipName == string.Empty)
				{
					Checker = false;
				}
				else
				{
					if (Ship.ShipName != string.Empty)
					{
						if (Ship.Weekday.HasFlag(today))
						{
							if (IsImprovementList)
							{
								if (Ship.Upgrade != null)
								{
									ShipList.Add(Ship);
									ShipCount++;
								}
								else ShipCount++;
							}
							else
							{
								ShipList.Add(Ship);
								ShipCount++;
							}
						}
						else
						{
							ShipCount++;
						}
					}
					else
					{
						if (IsImprovementList)
						{
							if (Ship.Upgrade != null)
							{
								Ship.ShipName = "없음";
								ShipList.Add(Ship);
								ShipCount++;
							}
							else ShipCount++;
						}
						else ShipCount++;
					}
				}
			}//while구문 종료

			return ShipList;
		}
		private List<RemodelItemList> MakeDefaultList(IEnumerable<XElement> List, int Position=-1)
		{
			//var PosElement = "Position";

			List<RemodelItemList> ItemList = new List<RemodelItemList>();


			//리스트를 Position에 따라 필터링
			//var tempList = List.Where(f => f.Element(PosElement).Value.Equals(Position.ToString())).ToList();
			var tempList = List;
			//XML리스트를 List<RemodelItemList>의 형태로 재작성
			foreach (var item in tempList)
			{
				//초기화
				RemodelItemList ItemContent = new RemodelItemList
				{
					ItemName = item.Element("SlotItemName").Value
				};
				//우선 장비명을 번역
				ItemContent.ItemName = KanColleClient.Current.Translations.GetTranslation(ItemContent.ItemName, TranslationType.Equipment, false);
				//슬롯 아이템 Master목록에서 해당되는 아이콘을 찾아 삽입
				foreach (var slotitem in KanColleClient.Current.Master.SlotItems)
				{
					if (slotitem.Value.Name == ItemContent.ItemName)
						ItemContent.IconType = slotitem.Value.IconType;
				}
				if (item.Element("ToolTip") != null)
					ItemContent.ToolTipString = item.Element("ToolTip").Value;
				else
				{
					ItemContent.ToolTipString = "특이사항 없음";
				}
				ItemContent.Ships = new List<ShipInfo>(MakeShipList(item));


				ItemList.Add(ItemContent);
			}

			return ItemList;
		}
		private List<RemodelItemList> MakeUpgradeList(IEnumerable<XElement> List)
		{
			var tempList = List;
			List<RemodelItemList> ItemList = new List<RemodelItemList>();
			foreach (var item in tempList)
			{
				//초기화
				RemodelItemList ItemContent = new RemodelItemList
				{
					ItemName = item.Element("SlotItemName").Value
				};
				//우선 장비명을 번역
				ItemContent.ItemName = KanColleClient.Current.Translations.GetTranslation(ItemContent.ItemName, TranslationType.Equipment, false);
				//슬롯 아이템 Master목록에서 해당되는 아이콘을 찾아 삽입
				foreach (var slotitem in KanColleClient.Current.Master.SlotItems)
				{
					if (slotitem.Value.Name == ItemContent.ItemName)
						ItemContent.IconType = slotitem.Value.IconType;
				}


				ItemContent.Ships = new List<ShipInfo>(TrimShipList(MakeShipList(item, true)));

				if (ItemContent.Ships.Count > 0) ItemList.Add(ItemContent);
			}
			return ItemList;
		}
		private List<RemodelItemList> MakeUseItemList(IEnumerable<XElement> List)
		{
			var tempList = List;

			List<RemodelItemList> ItemList = new List<RemodelItemList>();

			foreach (var item in tempList)
			{
				//초기화
				RemodelItemList ItemContent = new RemodelItemList
				{
					ItemName = item.Element("SlotItemName").Value
				};
				//우선 장비명을 번역
				ItemContent.ItemName = KanColleClient.Current.Translations.GetTranslation(ItemContent.ItemName, TranslationType.Equipment, false);
				//슬롯 아이템 Master목록에서 해당되는 아이콘을 찾아 삽입
				foreach (var slotitem in KanColleClient.Current.Master.SlotItems)
				{
					if (slotitem.Value.Name == ItemContent.ItemName)
						ItemContent.IconType = slotitem.Value.IconType;
				}
				// 개수시 필요한 나사 개수 목록을 작성
				StringBuilder screwCombine = new StringBuilder();
				if (item.Element("StartScrew") != null)
				{
					screwCombine.Append(item.Element("StartScrew").Value);
				}
				if (item.Element("MidScrew") != null)
				{
					screwCombine.Append("/");
					screwCombine.Append(item.Element("MidScrew").Value);
				}
				if (item.Element("LastScrew") != null)
				{
					screwCombine.Append("/");
					screwCombine.Append(item.Element("LastScrew").Value);
				}
				if (screwCombine.Length > 0)
					ItemContent.UseScrew = screwCombine.ToString();

				ItemList.Add(ItemContent);
			}
			return ItemList;
		}
		private WeekDayFlag WeekDaySetter(int weekint)
		{
			var temp = weekint.ToString();
			int len = temp.Length;

			if (len == 7) return WeekDayFlag.All;

			WeekDayFlag weekday = new WeekDayFlag();
			for (int i = 0; i < len; i++)
			{
				int inttemp = int.Parse(temp[i].ToString());
				weekday |= (WeekDayFlag)(1 << (inttemp - 1));
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
		public string UseScrew { get; set; }
		public SlotItemIconType? UpgradeIconType { get; set; }
		public List<ShipInfo> Ships { get; set; }
	}
	public class ShipInfo
	{
		public WeekDayFlag Weekday { get; set; }
		public string Upgrade { get; set; }
		public string ShipName { get; set; }
		public SlotItemIconType? UpgradeIconType { get; set; }
		public bool IsSameUpgradeExist { get; set; }
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

	public class RemodelFilterViewModel : Livet.ViewModel
	{
		public string Key { get; }
		public SlotItemIconType? Display { get; }

		public RemodelFilterViewModel(string Key, SlotItemIconType? Display)
		{
			this.Key = Key;
			this.Display = Display;
		}
	}
	#endregion
}