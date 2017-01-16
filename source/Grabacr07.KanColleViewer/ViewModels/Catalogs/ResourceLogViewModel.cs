using System;
using System.Linq;
using System.IO;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MetroTrilithon.Mvvm;
using MetroTrilithon.Serialization;
using Grabacr07.KanColleWrapper;
using System.Threading.Tasks;
using Livet;
using Livet.EventListeners;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Views.Controls;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class ResourceLogViewModel : WindowViewModel
	{
		public ImageSource icon_fuel => BitmapToImageSource(Properties.Resources.icon_fuel);
		public ImageSource icon_ammo => BitmapToImageSource(Properties.Resources.icon_ammo);
		public ImageSource icon_steel => BitmapToImageSource(Properties.Resources.icon_steel);
		public ImageSource icon_bauxite => BitmapToImageSource(Properties.Resources.icon_bauxite);
		public ImageSource icon_repair => BitmapToImageSource(Properties.Resources.icon_repair);
		public ImageSource icon_construction => BitmapToImageSource(Properties.Resources.icon_construction);
		public ImageSource icon_development => BitmapToImageSource(Properties.Resources.icon_development);
		public ImageSource icon_improvement => BitmapToImageSource(Properties.Resources.icon_improvement);

		private string ResourceCachePath => Path.Combine("Record", "resourcelog.csv");

		private Dictionary<string, DisplayedPeriod> PeriodTable { get; set; }
		public IEnumerable<string> DisplayPeriods { get; set; }

		private Random RandomInstance { get; } = new Random();
		private double ListenerEventID { get; set; }

		#region DisplayPeriod 프로퍼티
		private string _DisplayPeriod;
		public string DisplayPeriod
		{
			get { return _DisplayPeriod; }
			set
			{
				this._DisplayPeriod = value;
				RaisePropertyChanged();
				ResetChart(GetDaysFromPeriod());
			}
		}
		#endregion

		#region 자원 프로퍼티들
		private int _CurrentFuel { get; set; }
		private int _CurrentAmmo { get; set; }
		private int _CurrentSteel { get; set; }
		private int _CurrentBauxite { get; set; }
		private int _CurrentRepairBucket { get; set; }
		private int _CurrentInstantConstruction { get; set; }
		private int _CurrentDevelopmentMaterial { get; set; }
		private int _CurrentImprovementMaterial { get; set; }

		public int CurrentFuel
		{
			get { return _CurrentFuel; }
			set
			{
				if (_CurrentFuel != value)
				{
					_CurrentFuel = value;
					RaisePropertyChanged();
				}
			}
		}
		public int CurrentAmmo
		{
			get { return _CurrentAmmo; }
			set
			{
				if (_CurrentAmmo != value)
				{
					_CurrentAmmo = value;
					RaisePropertyChanged();
				}
			}
		}
		public int CurrentSteel
		{
			get { return _CurrentSteel; }
			set
			{
				if (_CurrentSteel != value)
				{
					_CurrentSteel = value;
					RaisePropertyChanged();
				}
			}
		}
		public int CurrentBauxite
		{
			get { return _CurrentBauxite; }
			set
			{
				if (_CurrentBauxite != value)
				{
					_CurrentBauxite = value;
					RaisePropertyChanged();
				}
			}
		}
		public int CurrentRepairBucket
		{
			get { return _CurrentRepairBucket; }
			set
			{
				if (_CurrentRepairBucket != value)
				{
					_CurrentRepairBucket = value;
					RaisePropertyChanged();
				}
			}
		}
		public int CurrentInstantConstruction
		{
			get { return _CurrentInstantConstruction; }
			set
			{
				if (_CurrentInstantConstruction != value)
				{
					_CurrentInstantConstruction = value;
					RaisePropertyChanged();
				}
			}
		}
		public int CurrentDevelopmentMaterial
		{
			get { return _CurrentDevelopmentMaterial; }
			set
			{
				if (_CurrentDevelopmentMaterial != value)
				{
					_CurrentDevelopmentMaterial = value;
					RaisePropertyChanged();
				}
			}
		}
		public int CurrentImprovementMaterial
		{
			get { return _CurrentImprovementMaterial; }
			set
			{
				if (_CurrentImprovementMaterial != value)
				{
					_CurrentImprovementMaterial = value;
					RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region 자원 Visible 프로퍼티들
		private bool _FuelVisible { get; set; } = false;
		private bool _AmmoVisible { get; set; } = false;
		private bool _SteelVisible { get; set; } = false;
		private bool _BauxiteVisible { get; set; } = false;
		private bool _RepairBucketVisible { get; set; } = false;
		private bool _InstantConstructionVisible { get; set; } = false;
		private bool _DevelopmentMaterialVisible { get; set; } = false;
		private bool _ImprovementMaterialVisible { get; set; } = false;

		public bool FuelVisible
		{
			get { return _FuelVisible; }
			set
			{
				if (_FuelVisible != value)
				{
					_FuelVisible = value;
					RaisePropertyChanged();
				}
			}
		}
		public bool AmmoVisible
		{
			get { return _AmmoVisible; }
			set
			{
				if (_AmmoVisible != value)
				{
					_AmmoVisible = value;
					RaisePropertyChanged();
				}
			}
		}
		public bool SteelVisible
		{
			get { return _SteelVisible; }
			set
			{
				if (_SteelVisible != value)
				{
					_SteelVisible = value;
					RaisePropertyChanged();
				}
			}
		}
		public bool BauxiteVisible
		{
			get { return _BauxiteVisible; }
			set
			{
				if (_BauxiteVisible != value)
				{
					_BauxiteVisible = value;
					RaisePropertyChanged();
				}
			}
		}
		public bool RepairBucketVisible
		{
			get { return _RepairBucketVisible; }
			set
			{
				if (_RepairBucketVisible != value)
				{
					_RepairBucketVisible = value;
					RaisePropertyChanged();
				}
			}
		}
		public bool InstantConstructionVisible
		{
			get { return _InstantConstructionVisible; }
			set
			{
				if (_InstantConstructionVisible != value)
				{
					_InstantConstructionVisible = value;
					RaisePropertyChanged();
				}
			}
		}
		public bool DevelopmentMaterialVisible
		{
			get { return _DevelopmentMaterialVisible; }
			set
			{
				if (_DevelopmentMaterialVisible != value)
				{
					_DevelopmentMaterialVisible = value;
					RaisePropertyChanged();
				}
			}
		}
		public bool ImprovementMaterialVisible
		{
			get { return _ImprovementMaterialVisible; }
			set
			{
				if (_ImprovementMaterialVisible != value)
				{
					_ImprovementMaterialVisible = value;
					RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region ElementToDraw 프로퍼티들
		private int[] _ElementToDraw1 { get; set; }
		public int[] ElementToDraw1
		{
			get { return this._ElementToDraw1; }
			set
			{
				if (_ElementToDraw1 != value)
				{
					this._ElementToDraw1 = value;
					this.RaisePropertyChanged();
				}
			}
		}
		private int[] _ElementToDraw2 { get; set; }
		public int[] ElementToDraw2
		{
			get { return this._ElementToDraw2; }
			set
			{
				if (_ElementToDraw2 != value)
				{
					this._ElementToDraw2 = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region 시작, 끝 날짜 프로퍼티
		private DateTime _BeginDate { get; set; }
		public DateTime BeginDate
		{
			get { return this._BeginDate; }
			set
			{
				if (_BeginDate != value)
				{
					this._BeginDate = value;
					this.RaisePropertyChanged();
				}
			}
		}
		private DateTime _EndDate { get; set; }
		public DateTime EndDate
		{
			get { return this._EndDate; }
			set
			{
				if (_EndDate != value)
				{
					this._EndDate = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Y-Min/Max 프로퍼티
		private int _YMin1 { get; set; }
		private int _YMin2 { get; set; }
		private int _YMax1 { get; set; }
		private int _YMax2 { get; set; }

		public int YMin1
		{
			get { return this._YMin1; }
			set
			{
				if (_YMin1 != value)
				{
					this._YMin1 = value;
					this.RaisePropertyChanged();
				}
			}
		}
		public int YMin2
		{
			get { return this._YMin2; }
			set
			{
				if (_YMin2 != value)
				{
					this._YMin2 = value;
					this.RaisePropertyChanged();
				}
			}
		}
		public int YMax1
		{
			get { return this._YMax1; }
			set
			{
				if (_YMax1 != value)
				{
					this._YMax1 = value;
					this.RaisePropertyChanged();
				}
			}
		}
		public int YMax2
		{
			get { return this._YMax2; }
			set
			{
				if (_YMax2 != value)
				{
					this._YMax2 = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region ResourcesList 프로퍼티
		private ResourceModel[] _ResourceList { get; set; }
		public ResourceModel[] ResourceList
		{
			get { return this._ResourceList; }
			set
			{
				if (_ResourceList != value)
				{
					this._ResourceList = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		public ResourceLogViewModel()
		{
			this.Title = "자원 기록 그래프";

			Dictionary<string, DisplayedPeriod> list = new Dictionary<string, DisplayedPeriod>();
			list.Add("1일", DisplayedPeriod.OneDay);
			list.Add("3일", DisplayedPeriod.ThreeDays);
			list.Add("1주일", DisplayedPeriod.OneWeek);
			list.Add("2주일", DisplayedPeriod.TwoWeeks);
			list.Add("1개월", DisplayedPeriod.OneMonth);
			list.Add("2개월", DisplayedPeriod.TwoMonths);
			list.Add("3개월", DisplayedPeriod.ThreeMonths);
			list.Add("6개월", DisplayedPeriod.SixMonths);
			list.Add("1년", DisplayedPeriod.OneYear);
			list.Add("2년", DisplayedPeriod.TwoYears);
			list.Add("3년", DisplayedPeriod.ThreeYears);
			PeriodTable = list;

			DisplayPeriods = PeriodTable.Keys.ToList();
			DisplayPeriod = "1주일"; // 1 week default
			RaisePropertyChanged("DisplayPeriod");

			var client = KanColleClient.Current;
			CurrentFuel = 0;
			CurrentAmmo = 0;
			CurrentSteel = 0;
			CurrentBauxite = 0;
			CurrentRepairBucket = 0;
			CurrentInstantConstruction = 0;
			CurrentDevelopmentMaterial = 0;
			CurrentImprovementMaterial = 0;

			this.CompositeDisposable.Add(new PropertyChangedEventListener(client)
			{
				{ nameof(client.IsStarted), (sender, args) => {
					var materials = KanColleClient.Current.Homeport.Materials;

					this.CompositeDisposable.Add(new PropertyChangedEventListener(materials)
					{
						{ nameof(materials.Fuel),                   (s, a) => { CurrentFuel = materials.Fuel; Task.Run(() => ListenerEventWorker()); } },
						{ nameof(materials.Ammunition),             (s, a) => { CurrentAmmo = materials.Ammunition; Task.Run(() => ListenerEventWorker()); } },
						{ nameof(materials.Steel),                  (s, a) => { CurrentSteel = materials.Steel; Task.Run(() => ListenerEventWorker()); } },
						{ nameof(materials.Bauxite),                (s, a) => { CurrentBauxite = materials.Bauxite; Task.Run(() => ListenerEventWorker()); } },
						{ nameof(materials.InstantRepairMaterials), (s, a) => { CurrentRepairBucket = materials.InstantRepairMaterials; Task.Run(() => ListenerEventWorker()); } },
						{ nameof(materials.InstantBuildMaterials),  (s, a) => { CurrentInstantConstruction = materials.InstantBuildMaterials; Task.Run(() => ListenerEventWorker()); } },
						{ nameof(materials.DevelopmentMaterials),   (s, a) => { CurrentDevelopmentMaterial = materials.DevelopmentMaterials; Task.Run(() => ListenerEventWorker()); } },
						{ nameof(materials.ImprovementMaterials),   (s, a) => { CurrentImprovementMaterial = materials.ImprovementMaterials; Task.Run(() => ListenerEventWorker()); } },
					});
				}}
			});

			LoadChart();
			ResetChart(GetDaysFromPeriod());
		}
		private async void ListenerEventWorker()
		{
			var EventID = RandomInstance.Next();
			ListenerEventID = EventID;

			await Task.Delay(500);
			if (EventID != ListenerEventID) return; // Another event called

			var res = new ResourceModel
			{
				Date = DateTime.Now,

				Fuel = CurrentFuel,
				Ammo = CurrentAmmo,
				Steel = CurrentSteel,
				Bauxite = CurrentBauxite,

				RepairBucket = CurrentRepairBucket,
				DevelopmentMaterial = CurrentDevelopmentMaterial,
				InstantConstruction = CurrentInstantConstruction,
				ImprovementMaterial = CurrentImprovementMaterial
			};

			var x = ResourceList.ToList();
			x.Add(res);
			ResourceList = x.ToArray();

			ResetChart(GetDaysFromPeriod());
		}

		private int GetDaysFromPeriod()
		{
			switch (PeriodTable[DisplayPeriod])
			{
				case DisplayedPeriod.OneDay: return 1;
				case DisplayedPeriod.ThreeDays: return 3;
				case DisplayedPeriod.OneWeek: return 7;
				case DisplayedPeriod.TwoWeeks: return 7 * 2;
				case DisplayedPeriod.OneMonth: return 30;
				case DisplayedPeriod.TwoMonths: return 30 * 2;
				case DisplayedPeriod.ThreeMonths: return 30 * 3;
				case DisplayedPeriod.SixMonths: return 30 * 6;
				case DisplayedPeriod.OneYear: return 365;
				case DisplayedPeriod.TwoYears: return 365 * 2;
				case DisplayedPeriod.ThreeYears: return 365 * 3;
			}
			return 7;
		}

		private async void LoadChart()
		{
			var _ResourceList = new List<ResourceModel>();
			ResourceList = new ResourceModel[0];

			string zItemsPath = ResourceCachePath;
			if (File.Exists(zItemsPath))
			{
				await Task.Run(() =>
				{
					using (FileStream fs = File.OpenRead(zItemsPath))
					{
						while (fs.Position < fs.Length)
						{
							string[] data = CSV.Read(fs);
							if (data.Length != 9) continue;

							DateTime dt;
							if (!DateTime.TryParse(data[0], out dt)) continue;

							ResourceModel model = new ResourceModel
							{
								Date = dt,
								Fuel = int.Parse(data[1]),
								Ammo = int.Parse(data[2]),
								Steel = int.Parse(data[3]),
								Bauxite = int.Parse(data[4]),
								RepairBucket = int.Parse(data[5]),
								DevelopmentMaterial = int.Parse(data[6]),
								InstantConstruction = int.Parse(data[7]),
								ImprovementMaterial = int.Parse(data[8]),
							};
							_ResourceList.Add(model);
						}
					}
				});
			}
			ResourceList = _ResourceList.ToArray();
			ResetChart(GetDaysFromPeriod());
		}
		private void ResetChart(int days)
		{
			var flag = 0;
			if (FuelVisible) flag = 1;
			if (AmmoVisible) flag = 1;
			if (SteelVisible) flag = 1;
			if (BauxiteVisible) flag = 1;
			if (RepairBucketVisible) flag = 1;
			if (InstantConstructionVisible) flag = 1;
			if (DevelopmentMaterialVisible) flag = 1;
			if (ImprovementMaterialVisible) flag = 1;

			if (flag == 0) return;
			if (ResourceList == null) return;

			var _ElementToDraw = new List<int>();
			if (FuelVisible) _ElementToDraw.Add(1);
			if (AmmoVisible) _ElementToDraw.Add(2);
			if (SteelVisible) _ElementToDraw.Add(3);
			if (BauxiteVisible) _ElementToDraw.Add(4);
			if (RepairBucketVisible) _ElementToDraw.Add(5);
			if (InstantConstructionVisible) _ElementToDraw.Add(7);
			if (DevelopmentMaterialVisible) _ElementToDraw.Add(6);
			if (ImprovementMaterialVisible) _ElementToDraw.Add(8);

			DateTime endTime = DateTime.Now;
			DateTime beginTime = endTime.Subtract(new TimeSpan(days, 0, 0, 0));

			var Data = ResourceList.Where(x => x.Date >= beginTime && x.Date <= endTime);
			var renderer = new GraphRenderer();

			// Measure Min and Max
			int _YMin1 = int.MaxValue, _YMax1 = -1;
			int _YMin2 = int.MaxValue, _YMax2 = -1;
			foreach (var type in _ElementToDraw)
			{
				if (type >= 1 && type <= 4)
				{
					_YMin1 = Math.Min(_YMin1, Data.Select(x => renderer.ElementValue(type, x)).Min());
					_YMax1 = Math.Max(_YMax1, Data.Select(x => renderer.ElementValue(type, x)).Max());
				}
				else
				{
					_YMin2 = Math.Min(_YMin2, Data.Select(x => renderer.ElementValue(type, x)).Min());
					_YMax2 = Math.Max(_YMax2, Data.Select(x => renderer.ElementValue(type, x)).Max());
				}
			}

			_YMin1 -= (int)Math.Pow(10, Math.Floor(Math.Log10(_YMax1)));
			_YMax1 += (int)Math.Pow(10, Math.Floor(Math.Log10(_YMax1)));
			_YMin2 -= (int)Math.Pow(10, Math.Floor(Math.Log10(_YMax2)));
			_YMax2 += (int)Math.Pow(10, Math.Floor(Math.Log10(_YMax2)));

			_YMax1 = Math.Min(_YMax1, 305000);
			_YMin1 = Math.Max(0, _YMin1);
			_YMax2 = Math.Min(_YMax2, 305000);
			_YMin2 = Math.Max(0, _YMin2);

			YMin1 = _YMin1;
			YMax1 = _YMax1;
			YMin2 = _YMin2;
			YMax2 = _YMax2;

			BeginDate = beginTime;
			EndDate = endTime;

			ElementToDraw1 = _ElementToDraw.Where(x => x <= 4).ToArray();
			ElementToDraw2 = _ElementToDraw.Where(x => x > 4).ToArray();
		}

		public void Refresh()
		{
			ResetChart(GetDaysFromPeriod());
		}

		public ImageSource BitmapToImageSource(Bitmap bitmap)
		{
			MemoryStream ms = new MemoryStream();
			ImageSource image = null;

			bitmap.Save(ms, ImageFormat.Png);
			ms.Position = 0;

			BitmapImage bi = new BitmapImage();
			bi.BeginInit();
			bi.StreamSource = ms;
			bi.EndInit();
			bi.Freeze();

			Dispatcher.CurrentDispatcher.Invoke(() => image = bi);
			return image;
		}
	}

	public enum DisplayedPeriod
	{
		OneDay,
		ThreeDays,
		OneWeek,
		TwoWeeks,
		OneMonth,
		TwoMonths,
		ThreeMonths,
		SixMonths,
		OneYear,
		TwoYears,
		ThreeYears,
	}
}
