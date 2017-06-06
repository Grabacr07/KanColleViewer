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
		public class RemodelItemListItem
		{
			/// <summary>
			/// 장비명
			/// </summary>
			public string ItemName { get; set; }

			/// <summary>
			/// 개수 가능 요일 전체
			/// </summary>
			public WeekDayFlag TotalWeekday { get; set; }

			/// <summary>
			/// 아이콘 종류
			/// </summary>
			public SlotItemIconType? IconType { get; set; }

			/// <summary>
			/// 소모 나사량
			/// </summary>
			public string UseScrew { get; set; }

			/// <summary>
			/// 툴팁 내용
			/// </summary>
			public string ToolTipString { get; set; }

			/// <summary>
			/// 개수 함선들
			/// </summary>
			public List<ShipInfo> Ships { get; set; }

			/// <summary>
			/// 장비 변환 가능 여부
			/// </summary>
			public bool Upgradable { get; set; }

			/// <summary>
			/// 장비변환 후 장비 아이콘
			/// </summary>
			public SlotItemIconType? UpgradeIconType { get; set; }

			/// <summary>
			/// 장비변환 함선들
			/// </summary>
			public List<ShipInfo> UpgradeShips { get; set; }
		}

		public class ShipInfo
		{
			/// <summary>
			/// 개수 가능 요일
			/// </summary>
			public WeekDayFlag Weekday { get; set; }

			/// <summary>
			/// 함선 이름
			/// </summary>
			public string ShipName { get; set; }

			/// <summary>
			/// 장비 변환 후 장비
			/// </summary>
			public string Upgrade { get; set; }

			/// <summary>
			/// 장비 변환 후 장비 아이콘
			/// </summary>
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
				if (this._IsLoading != value)
				{
					this._IsLoading = value;
					this.RaisePropertyChanged();
				}
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
				if (this._OnlyOwnSlotItems != value)
				{
					this._OnlyOwnSlotItems = value;
					this.RaisePropertyChanged();
					this.Update();
				}
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
				if (this._OnlyRemodeledSlotItems != value)
				{
					this._OnlyRemodeledSlotItems = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}
		#endregion

		#region OnlyOwnShips 변경통지 프로퍼티
		private bool _OnlyOwnShips;
		public bool OnlyOwnShips
		{
			get { return this._OnlyOwnShips; }
			set
			{
				if (this._OnlyOwnShips != value)
				{
					this._OnlyOwnShips = value;
					this.RaisePropertyChanged();
					this.Update();
				}
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

		#region RemodelItemList 変更通知プロパティ
		private List<RemodelItemListItem> _RemodelItemList;
		public List<RemodelItemListItem> RemodelItemList
		{
			get { return _RemodelItemList; }
			set
			{
				if (_RemodelItemList != value)
				{
					_RemodelItemList = value;
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

					var homeport = KanColleClient.Current.Homeport;

					this.RemodelXML = XDocument.Load(Path.Combine(MainFolder, "Translations", "RemodelSlots.xml"));
					IEnumerable<XElement> RemodelList = GetRemodelList();

					var Weekday = "AllWeekdays";
					//RemodelList에서 오늘 개수공창 목록에 들어갈것들을 선별한다.
					RemodelList = RemodelList.Where(f => WeekDaySetter(Convert.ToInt32(f.Element(Weekday).Value)).HasFlag(today));

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

					// 소지 장비로 필터
					if (this.OnlyOwnSlotItems)
					{
						RemodelList = RemodelList.Where(x =>
						{
							var name = x.Element("SlotItemName").Value;
							var itemid = KanColleClient.Current.Master.SlotItems
								.Where(y => y.Value.RawData.api_name == name)
								.FirstOrDefault()
								.Value?.Id ?? 0;

							return homeport.Itemyard.SlotItems.Any(y => y.Value.Info.Id == itemid);
						});
					}

					// 개수된 장비로 필터
					if (this.OnlyRemodeledSlotItems)
					{
						RemodelList = RemodelList.Where(x =>
						{
							var name = x.Element("SlotItemName").Value;
							var itemid = KanColleClient.Current.Master.SlotItems
								.Where(y => y.Value.RawData.api_name == name)
								.FirstOrDefault()
								.Value?.Id ?? 0;

							return homeport.Itemyard.SlotItems.Any(y => y.Value.Info.Id == itemid && y.Value.Level > 0);
						});
					}

					// 보유중인 함선으로 필터
					if (this.OnlyOwnShips)
					{
						string[] assume =
						{
							"改二", "改三",
							"改二甲","改二乙","改二丙","改二丁",
							"改二戊","改二己","改二庚","改二辛",
							"改二壬","改二癸",
							"zwei","drei","due",
							"改"
						};

						RemodelList = RemodelList.Where(x =>
						{
							List<string> ships = new List<string>();

							for (int i = 0; ; i++)
							{
								string element = string.Format("ShipName{0}", i + 1);
								string element2 = string.Format("WeekDays{0}", i + 1);
								string name = x.Element(element)?.Value ?? null;
								string week = x.Element(element2)?.Value ?? null;
								if (name == null || week == null) break;

								var today = WeekDayTable[SelectedDay];
								if (week.Contains(today.ToString()))
									ships.Add(name);
							}
							return homeport.Organization.Ships.Any(y =>
							{
								var name = y.Value.Info.JPName;
								if (ships.Contains(name)) return true;

								foreach (var z in assume)
									name = name.Replace(z, string.Empty).Trim();
								return ships.Contains(name);
							});
						});
					}


					// 개수 가능 장비 목록을 작성
					var list = MakeDefaultList(RemodelList).ToList();
					var _RemodelItemList = SortList(list);

					Application.Current.Dispatcher.Invoke(() =>
					{
						this.RemodelItemList = _RemodelItemList;
						this.IsLoading = false;
					});
				}).Start();
			}
		}

		/// <summary>
		/// 장비 목록 결과를 정렬
		/// </summary>
		/// <param name="myList"></param>
		/// <returns></returns>
		private List<RemodelItemListItem> SortList(List<RemodelItemListItem> myList)
		{
			myList?.Sort((x, y) =>
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
			return ShipList
				.GroupBy(x => x.Upgrade)
				.Select(x =>
				{
					var y = x.First();
					return new ShipInfo
					{
						ShipName = string.Join(", ", x.Select(z => z.ShipName)),
						Upgrade = y.Upgrade,
						UpgradeIconType = y.UpgradeIconType,
						Weekday = y.Weekday
					};
				})
				.Distinct(x => x.Upgrade)
				.ToList();
		}

		/// <summary>
		/// 개수 가능 함선 목록 작성
		/// </summary>
		private List<ShipInfo> MakeShipList(XElement Context)
		{
			List<ShipInfo> ShipList = new List<ShipInfo>();

			for (int ShipCount = 1; ; ShipCount++)
			{
				// 이름작성
				string ShipElement;

				ShipInfo Ship = new ShipInfo();

				// 함선명
				ShipElement = string.Format("ShipName{0}", ShipCount);
				Ship.ShipName = Context.Element(ShipElement)?.Value ?? null;
				if (Ship.ShipName != null)
				{
					Ship.ShipName = Ship.ShipName.StartsWith("※")
						? Ship.ShipName
						: KanColleClient.Current.Translations.GetTranslation(Ship.ShipName, TranslationType.Ships, false);
				}
				else break;

				// 업그레이드 부분
				ShipElement = string.Format("Upgrade{0}", ShipCount);
				Ship.Upgrade = Context.Element(ShipElement)?.Value ?? null;
				if (Ship.Upgrade != null)
				{
					Ship.Upgrade = KanColleClient.Current.Translations.GetTranslation(Ship.Upgrade, TranslationType.Equipment, false);

					//슬롯 아이템 Master목록에서 해당되는 아이콘을 찾아 삽입
					Ship.UpgradeIconType = KanColleClient.Current.Master.SlotItems
						.Where(x => x.Value.Name == Ship.Upgrade)
						.Select(x => x.Value.IconType)
						.FirstOrDefault();
				}

				// 개수 가능일
				ShipElement = string.Format("WeekDays{0}", ShipCount);
				var weekDays = Context.Element(ShipElement)?.Value ?? null;
				if (weekDays != null)
				{
					int weekday = Convert.ToInt32(weekDays);
					Ship.Weekday = WeekDaySetter(weekday);
				}

				if (Ship.Weekday == WeekDayFlag.None && Ship.Upgrade == null && Ship.ShipName == null)
					break;

				if (Ship.Weekday.HasFlag(today))
					ShipList.Add(Ship);

				/* Upgrade 항목은 있지만 ShipName 은 없는 경우. 존재하지 않을테니 주석으로
				// * 사용한다면 상단 함선명의 else continue; 를 주석으로
				else if (IsImprovementList && Ship.Upgrade != null)
				{
					Ship.ShipName = "없음";
					ShipList.Add(Ship);
				}
				// */
			} // while구문 종료

			return ShipList;
		}

		/// <summary>
		/// 개수 가능 장비 목록 작성
		/// </summary>
		private List<RemodelItemListItem> MakeDefaultList(IEnumerable<XElement> List, int Position = -1)
		{
			//var PosElement = "Position";
			List<RemodelItemListItem> ItemList = new List<RemodelItemListItem>();

			//리스트를 Position에 따라 필터링
			//var tempList = List.Where(f => f.Element(PosElement).Value.Equals(Position.ToString())).ToList();
			var tempList = List;

			//XML리스트를 List<RemodelItemList>의 형태로 재작성
			foreach (var item in tempList)
			{
				//초기화
				RemodelItemListItem ItemContent = new RemodelItemListItem
				{
					ItemName = item.Element("SlotItemName").Value
				};

				//우선 장비명을 번역
				ItemContent.ItemName =
					ItemContent.ItemName.StartsWith("※") // 얘는 번역 X
					? ItemContent.ItemName
					: KanColleClient.Current.Translations.GetTranslation(ItemContent.ItemName, TranslationType.Equipment, false);

				//슬롯 아이템 Master목록에서 해당되는 아이콘을 찾아 삽입
				ItemContent.IconType = KanColleClient.Current.Master.SlotItems
					.Where(x => x.Value.Name == ItemContent.ItemName)
					.Select(x => x.Value.IconType)
					.FirstOrDefault();

				// 개수시 필요한 나사 개수 목록을 작성
				List<string> screwCombine = new List<string>();
				if (item.Element("StartScrew") != null) screwCombine.Add(item.Element("StartScrew").Value);
				if (item.Element("MidScrew") != null) screwCombine.Add(item.Element("MidScrew").Value);
				if (item.Element("LastScrew") != null) screwCombine.Add(item.Element("LastScrew").Value);
				ItemContent.UseScrew = string.Join("/", screwCombine);

				// 툴팁을 선택
				if (item.Element("ToolTip") != null)
					ItemContent.ToolTipString = item.Element("ToolTip").Value;
				else
					ItemContent.ToolTipString = "특이사항 없음";

				// 개수시 필요한 칸무스 작성
				ItemContent.Ships = MakeShipList(item);

				// 장비변환이 가능한지 여부 판단
				ItemContent.Upgradable = ItemContent.Ships.Any(x => x.Upgrade != null);

				// 장비변환 가능한 경우 칸무스 목록 작성
				if (ItemContent.Upgradable)
					ItemContent.UpgradeShips = TrimShipList(ItemContent.Ships);

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
}
