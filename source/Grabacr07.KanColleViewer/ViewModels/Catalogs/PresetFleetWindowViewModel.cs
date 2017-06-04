using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using MetroTrilithon.Mvvm;
using Livet;
using Livet.Messaging;
using Livet.EventListeners;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Grabacr07.KanColleViewer.Views.Catalogs;
using Grabacr07.KanColleViewer.ViewModels.Contents.Fleets;
using System.Runtime.Serialization.Json;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class PresetFleetWindowViewModel : WindowViewModel
	{
		#region Fleets 변경 통지 프로퍼티
		private IReadOnlyCollection<PresetFleetData> _Fleets { get; set; }
		public IReadOnlyCollection<PresetFleetData> Fleets
		{
			get { return this._Fleets; }
			set
			{
				if (this._Fleets != value)
				{
					this._Fleets = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region SelectedFleet 변경 통지 프로퍼티
		private PresetFleetData _SelectedFleet { get; set; }
		public PresetFleetData SelectedFleet
		{
			get { return this._SelectedFleet; }
			set
			{
				if (this._SelectedFleet != value)
				{
					this._SelectedFleet = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		private string RecordPath => Path.Combine(
			Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location),
			"Record",
			"fleet_preset.json"
		);

		public PresetFleetWindowViewModel()
		{
			this.Title = "함대 프리셋";

			this.LoadFleets();
			this.SelectedFleet = this.Fleets.FirstOrDefault();
		}

		public void LoadFleets()
		{
			var list = new List<PresetFleetData>();

			if (File.Exists(RecordPath))
			{
				var lines = File.ReadAllText(RecordPath)
					.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

				foreach (var line in lines)
				{
					var item = new PresetFleetData();
					item.Deserialize(line);
					list.Add(item);
				}
			}

			this.Fleets = list.ToArray();
			this.SelectedFleet = this.Fleets.FirstOrDefault();
		}
		public void SaveFleets()
		{
			var datas = this.Fleets.Select(x => x.Serialize());

			File.WriteAllText(
				RecordPath,
				string.Join(Environment.NewLine, datas)
			);
		}

		public void ShowPresetAddWindow()
		{
			var catalog = new PresetFleetAddWindowViewModel(this);
			WindowService.Current.MainWindow.Transition(catalog, typeof(PresetFleetAddWindow));
		}
		public void ShowPresetDeleteWindow()
		{
			var fleet = this.SelectedFleet;
			if (fleet == null) return;

			var catalog = new PresetFleetDeleteWindowViewModel(this, fleet);
			WindowService.Current.MainWindow.Transition(catalog, typeof(PresetFleetDeleteWindow));
		}

		public void AddFleet(PresetFleetData fleet)
		{
			this.Fleets = this.Fleets
				.Concat(new PresetFleetData[] { fleet })
				.ToArray();

			if (this.SelectedFleet == null)
				this.SelectedFleet = this.Fleets.FirstOrDefault();

			SaveFleets();
		}
		public void DeleteFleet(PresetFleetData fleet)
		{
			this.Fleets = this.Fleets
				.Where(x => x != fleet)
				.ToArray();

			if (this.SelectedFleet == fleet)
				this.SelectedFleet = this.Fleets.FirstOrDefault();

			SaveFleets();
		}
	}

	#region Model Wrapper
	public class PresetFleetData : ViewModel
	{
		public PresetFleetModel Source { get; private set; }

		public PresetFleetData()
		{
			this.Source = new PresetFleetModel
			{
				Name = "Untitled Fleet",
				Ships = new PresetShipModel[0]
			};
		}
		public PresetFleetData(string Name) : this()
		{
			this.Source.Name = Name;
		}

		public string Serialize()
			=> this.Source?.Serialize();

		public void Deserialize(string Data)
			=> this.Source = PresetFleetModel.Deserialize(Data);
	}
	public class PresetShipData : NotificationObject
	{
		private ShipInfo ship =>
				KanColleClient.Current.Master.Ships
					.SingleOrDefault(x => x.Value.Id == this.Source.Id).Value
					?? null;

		public PresetShipModel Source { get; private set; }

		public string Name => ship?.Name ?? "？？？";
		public string TypeName => ship?.ShipType?.Name ?? "？？？";

		public PresetShipData()
		{
			this.Source = new PresetShipModel
			{
				Id = -1,
				Slots = new PresetSlotModel[0],
				ExSlot = null
			};
		}
		public PresetShipData(Ship ship)
		{
			this.Source = new PresetShipModel {
				Id = ship.Info.Id,

				Slots = ship.Slots
					.Select(x => new PresetSlotModel
					{
						Id = x.Item.Info.Id,
						Level = x.Item.Level,
						Proficiency = x.Item.Proficiency
					})
					.ToArray(),

				ExSlot = ship.ExSlot != null
					? new PresetSlotModel
					{
						Id = ship.ExSlot.Item.Info.Id,
						Level = ship.ExSlot.Item.Level,
						Proficiency = ship.ExSlot.Item.Proficiency
					}
					: null,

				HP = ship.HP.Maximum,
				Armor = ship.Armer.Current,
				Evasion = ship.RawData.api_kaihi[0],
				Carries = ship.Info.RawData.api_maxeq.Sum(),
				Speed = (int)ship.Speed,
				Range = ship.RawData.api_leng,

				Firepower = ship.Firepower.Current,
				Torpedo = ship.Torpedo.Current,
				AA = ship.AA.Current,
				ASW = ship.ASW.Current,
				LOS = ship.ViewRange,
				Luck = ship.Luck.Current
			};
		}

		public string Serialize()
			=> this.Source?.Serialize();

		public void Deserialize(string Data)
			=> this.Source = PresetShipModel.Deserialize(Data);
	}
	public class PresetSlotData : NotificationObject
	{
		private SlotItemInfo item =>
			KanColleClient.Current.Master.SlotItems
				.SingleOrDefault(x => x.Value.Id == this.Source?.Id).Value
				?? null;

		public PresetSlotModel Source { get; private set; }

		public string Name => item?.Name ?? "？？？";

		public PresetSlotData()
		{
			this.Source = new PresetSlotModel
			{
				Id = 0,
				Level = 0,
				Proficiency = 0
			};
		}
		public PresetSlotData(SlotItem item)
		{
			this.Source = new PresetSlotModel
			{
				Id = item.Info.Id,
				Level = item.Level,
				Proficiency = item.Proficiency
			};
		}

		public string Serialize()
			=> this.Source?.Serialize();

		public void Deserialize(string Data)
			=> this.Source = PresetSlotModel.Deserialize(Data);
	}
	#endregion

	#region Models
	public class PresetFleetModel
	{
		public string Name { get; set; }
		public PresetShipModel[] Ships { get; set; }

		public string Serialize()
		{
			return string.Format(
				"{{\"Name\":\"{0}\",\"Ships\":[{1}]}}",
				this.Name.Replace("\"", "\\\""),
				string.Join(",", this.Ships.Select(x => x.Serialize()))
			);
		}
		public static PresetFleetModel Deserialize(string Data)
			=> PresetUtil.ParseJson<PresetFleetModel>(Data);
	}
	public class PresetShipModel
	{
		public int Id { get; set; }
		public int Level { get; set; }

		public int HP { get; set; }
		public int Armor { get; set; }
		public int Evasion { get; set; }
		public int Carries { get; set; }
		public int Speed { get; set; }
		public int Range { get; set; }

		public int Firepower { get; set; }
		public int Torpedo { get; set; }
		public int AA { get; set; }
		public int ASW { get; set; }
		public int LOS { get; set; }
		public int Luck { get; set; }

		public PresetSlotModel[] Slots;
		public PresetSlotModel ExSlot;

		public string Serialize()
		{
			return string.Format(
				"{{\"Id\":{0},\"Level\":{1},\"HP\":{2},\"Armor\":{3},\"Evasion\":{4},\"Carries\":{5},\"Speed\":{6},\"Range\":{7},\"Firepower\":{8},"
				+ "\"Torpedo\":{9},\"AA\":{10},\"ASW\":{11},\"LOS\":{12},\"Luck\":{13},\"Slots\":[{14}],\"ExSlot\":{15}}}",

				this.Id,
				this.Level,

				this.HP,
				this.Armor,
				this.Evasion,
				this.Carries,
				this.Speed,
				this.Range,

				this.Firepower,
				this.Torpedo,
				this.AA,
				this.ASW,
				this.LOS,
				this.Luck,

				string.Join(",", this.Slots.Select(x => x.Serialize())),
				this.ExSlot.Serialize()
			);
		}
		public static PresetShipModel Deserialize(string Data)
			=> PresetUtil.ParseJson<PresetShipModel>(Data);
	}
	public class PresetSlotModel
	{
		public int Id { get; set; }
		public int Level { get; set; }
		public int Proficiency { get; set; }

		public string Serialize()
		{
			return string.Format(
				"{{\"Id\":{0},\"Level\":{1},\"Proficiency\":{2}}}",
				this.Id,
				this.Level,
				this.Proficiency
			);
		}
		public static PresetSlotModel Deserialize(string Data)
			=> PresetUtil.ParseJson<PresetSlotModel>(Data);
	}
	#endregion

	internal class PresetUtil
	{
		public static T ParseJson<T>(string Json) where T : class
		{
			var bytes = Encoding.UTF8.GetBytes(Json);
			var serializer = new DataContractJsonSerializer(typeof(T));
			using (var stream = new MemoryStream(bytes))
			{
				var rawResult = serializer.ReadObject(stream) as T;
				return rawResult;
			}
		}
	}
}
