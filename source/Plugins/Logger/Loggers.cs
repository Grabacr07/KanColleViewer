using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Collections.Specialized;
using System.Reflection;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models.Raw;
using Livet;
using Logger.Models;
using Logger.Properties;
using MetroTrilithon.Mvvm;

namespace Logger
{
	public abstract class LoggerBase : NotificationObject
	{
		private static LoggerSettings Settings => KanColleLogger.Settings;

		#region LoggerName 変更通知プロパティ

		private string _LoggerName;

		public string LoggerName 
		{ 
			get { return this._LoggerName; }
			set	
			{
				if (this._LoggerName != value)
				{
					this._LoggerName = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Filename 変更通知プロパティ

		private string _Filename;

		public string Filename
		{
			get { return this._Filename; }
			set
			{
				if (this._Filename != value)
				{
					this._Filename = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public string Title
		{
			get
			{
				PropertyInfo pInfo = typeof(Resources).GetProperty("Logger_" + this.GetType().Name);

				if ((pInfo == null) || (pInfo.PropertyType != typeof(string))) return this.GetType().Name;

				return (string)pInfo.GetValue(typeof(Resources), null);
			}
		}

		private string _DefaultFormat;

		public string DefaultFormat
		{
			get { return this._DefaultFormat; }
			set
			{
				if (this._DefaultFormat != value)
				{
					this._DefaultFormat = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#region Format 変更通知プロパティ

		public string Format
		{
			get { return Settings.LoggersFormats[LoggerName].Value; }
			set
			{
				if (Settings.LoggersFormats[LoggerName].Value != value)
				{
					Settings.LoggersFormats[LoggerName].Value = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged(nameof(LogSample));
				}
			}
		}

		#endregion

		#region Enabled 変更通知プロパティ

		public bool Enabled
		{
			get { return Settings.LoggersEnabled[this.LoggerName].Value; }
			set
			{
				if (Settings.LoggersEnabled[this.LoggerName].Value != value)
				{
					Settings.LoggersEnabled[this.LoggerName].Value = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public abstract string LogSample { get; }

		public abstract string FormatToolTip { get; }

		public abstract List<string> ColumnHeaders { get; }

		public void Initialize()
		{
			Settings.DateTimeUseJapanese.Subscribe(x => this.RaisePropertyChanged(nameof(LogSample)));
			Settings.DateTimeFormat.Subscribe(x => this.RaisePropertyChanged(nameof(LogSample)));
			ResourceService.Current.Subscribe(x => this.RaisePropertyChanged(nameof(Title)));
		}

		public void Log(params object[] args)
		{
			if (!this.Enabled) return;

			if (!Directory.Exists(Settings.Location))
			{
				try
				{
					Directory.CreateDirectory(Settings.Location);
				}
				catch { }
			}

			try
			{
				if (!File.Exists(Path.Combine(Settings.Location, this.Filename)))
				{
					// Create and add the CSV header
					using (var w = File.CreateText(Path.Combine(Settings.Location, this.Filename)))
					{
						w.WriteLine(string.Format(Format, (new List<object> { "UTC Time", Settings.DateTimeUseJapanese ? "JST Time" : "Local Time" }).Concat(ColumnHeaders).ToArray()));
					}
				}
				using (var w = File.AppendText(Path.Combine(Settings.Location, this.Filename)))
				{
					w.WriteLine(FormatForLog(args));
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		public string Timestamp(bool useIso = true)
		{
			if (useIso) return DateTime.UtcNow.ToString("o");

			try
			{
				return Settings.DateTimeUseJapanese
					? TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time")).ToString(Settings.DateTimeFormat.Value)
					: DateTime.Now.ToString(Settings.DateTimeFormat.Value);
			}
			catch
			{
				return "<Date/time string format error>";
			}
		}

		public string FormatForLog(params object[] args)
		{
			try
			{
				return string.Format(Format, (new List<object> { Timestamp(), Timestamp(false) }).Concat(args).ToArray());
			}
			catch
			{
				return "<Format string error>";
			}
		}
	}

	public class ItemLog : LoggerBase
	{
		public override string LogSample => FormatForLog("Yamato Wave Motion Gun", "CVB", "6000", "9001", "9001", "10", "Yamato Kai4");

		public override string FormatToolTip => Resources.Settings_Tooltip_Development;
		public override List<string> ColumnHeaders { get; } = new List<string> { "Item", "Secretary Type", "Fuel", "Ammo", "Steel", "Bauxite", "Secretary Name" };

		public ItemLog(KanColleProxy proxy)
		{
			proxy.api_req_kousyou_createitem.TryParse<kcsapi_createitem>().Subscribe(x => this.CreateItem(x.Data, x.Request));
			this.Filename = "DevelopmentLog.csv";
			this.LoggerName = "Development";
			this.DefaultFormat = "{0},{2},{3},{4},{5},{6},{7}";
		}

		private void CreateItem(kcsapi_createitem item, NameValueCollection req)
		{
			Log(item.api_create_flag == 1 ? KanColleClient.Current.Master.SlotItems[item.api_slot_item.api_slotitem_id].Name : "FAILED",
				KanColleClient.Current.Homeport.Organization.Fleets[1].Ships[0].Info.ShipType.Name,
				req["api_item1"], 
				req["api_item2"], 
				req["api_item3"], 
				req["api_item4"],
				KanColleClient.Current.Homeport.Organization.Fleets[1].Ships[0].Info.Name);
		}
	}

	public class ConstructionLog : LoggerBase
	{
		public override string LogSample => FormatForLog("Yamato Kai4", "9001", "9001", "9001", "7000", "100", "Iona", "SS");

		public override string FormatToolTip => Resources.Settings_Tooltip_Construction;
		public override List<string> ColumnHeaders { get; } = new List<string> { "Ship Name", "Fuel", "Ammo", "Steel", "Bauxite", "Development Materials", "Secretary Name", "Secretary Type"};

		private readonly int[] shipmats;
		private bool waitingForShip;
		private int dockid;

		public ConstructionLog(KanColleProxy proxy)
		{
			this.shipmats = new int[5];
			proxy.api_req_kousyou_createship.TryParse<kcsapi_createship>().Subscribe(x => this.CreateShip(x.Request));
			proxy.api_get_member_kdock.TryParse<kcsapi_kdock[]>().Subscribe(x => this.KDock(x.Data));
			this.Filename = "ConstructionLog.csv";
			this.LoggerName = "Construction";
			this.DefaultFormat = "{0},{2},{3},{4},{5},{6},{7}";
		}

		private void CreateShip(NameValueCollection req)
		{
			this.waitingForShip = true;
			this.dockid = Int32.Parse(req["api_kdock_id"]);
			this.shipmats[0] = Int32.Parse(req["api_item1"]);
			this.shipmats[1] = Int32.Parse(req["api_item2"]);
			this.shipmats[2] = Int32.Parse(req["api_item3"]);
			this.shipmats[3] = Int32.Parse(req["api_item4"]);
			this.shipmats[4] = Int32.Parse(req["api_item5"]);
		}

		private void KDock(kcsapi_kdock[] docks)
		{
			foreach (var dock in docks.Where(dock => this.waitingForShip && dock.api_id == this.dockid))
			{
				this.Log(KanColleClient.Current.Master.Ships[dock.api_created_ship_id].Name, 
						 this.shipmats[0], 
						 this.shipmats[1], 
						 this.shipmats[2], 
						 this.shipmats[3], 
						 this.shipmats[4],
						 KanColleClient.Current.Homeport.Organization.Fleets[1].Ships[0].Info.Name,
						 KanColleClient.Current.Homeport.Organization.Fleets[1].Ships[0].Info.ShipType.Name);
				this.waitingForShip = false;
			}
		}
	}

	public class BattleLog : LoggerBase
	{
		public override string LogSample => FormatForLog("Takao", "The Fortress Port of Yokosuka", "Haruna and Kirishima", "B");
		public override string FormatToolTip => Resources.Settings_Tooltip_BattleResult;
		public override List<string> ColumnHeaders { get; } = new List<string> { "Ship Name", "Map Name", "Node Name", "Victory Rank" };

		public BattleLog(KanColleProxy proxy)
		{
			proxy.api_req_sortie_battleresult.TryParse<kcsapi_battleresult>().Subscribe(x => this.BattleResult(x.Data));
			proxy.api_req_combined_battle_battleresult.TryParse<kcsapi_combined_battle_battleresult>().Subscribe(x => this.BattleResult(x.Data));
			this.Filename = "BattleLog.csv";
			this.LoggerName = "BattleResults";
			this.DefaultFormat = "{0},{2},{3},{4},{5}";
		}

		private void BattleResult(kcsapi_battleresult br)
		{
			try
			{
				if (br.api_get_ship == null)
				return;

				Log(br.api_get_ship.api_ship_name,
					br.api_quest_name,
					br.api_enemy_info.api_deck_name,
					br.api_win_rank);
			}
			catch { }
		}

		private void BattleResult(kcsapi_combined_battle_battleresult br)
		{
			try
			{
				if (br.api_get_ship == null)
					return;

				Log(br.api_get_ship.api_ship_name,
					br.api_quest_name,
					br.api_enemy_info.api_deck_name,
					br.api_win_rank);
			}
			catch { }
		}
	}

	public class MaterialsLog : LoggerBase
	{
		public override string LogSample => FormatForLog("24500", "20247", "36102", "14322", "1405", "790", "1109", "3");
		public override string FormatToolTip => Resources.Settings_Tooltip_Materials;
		public override List<string> ColumnHeaders { get; } = new List<string> { "Fuel", "Ammo", "Steel", "Bauxite", "Development Kits", "Instant Repair", "Instant Construction", "Improvement Materials" };

		private int[] materials = new int[8];

		public MaterialsLog(KanColleProxy proxy)
		{
			proxy.api_port.TryParse<kcsapi_port>().Subscribe(x => this.MaterialsHistory(x.Data.api_material));
			proxy.api_get_member_material.TryParse<kcsapi_material[]>().Subscribe(x => this.MaterialsHistory(x.Data));
			proxy.api_req_hokyu_charge.TryParse<kcsapi_charge>().Subscribe(x => this.MaterialsHistory(x.Data.api_material));
			proxy.api_req_kousyou_destroyship.TryParse<kcsapi_destroyship>().Subscribe(x => this.MaterialsHistory(x.Data.api_material));
			this.Filename = "MaterialsExpenditureLog.csv";
			this.LoggerName = "Materials";
			this.DefaultFormat = "{0},{2},{3},{4},{5},{6},{7},{8},{9}";

			for (int i = 0; i < 8; i++)
				materials[i] = 0;
		}

		private void MaterialsHistory(kcsapi_material[] source)
		{
			const int sourcelength = 8;

			if (source == null || source.Length != sourcelength)
				return;

			bool process = false;

			for (int i = 0; i < sourcelength; i++)
				if (source[i].api_value != materials[i])
				{
					materials[i] = source[i].api_value;
					process = true;
				}

			if (!process) return;

			Log(materials[(int)MaterialsType.Fuel],
				materials[(int)MaterialsType.Ammo],
				materials[(int)MaterialsType.Steel],
				materials[(int)MaterialsType.Bauxite],
				materials[(int)MaterialsType.DevelopmentKits],
				materials[(int)MaterialsType.InstantRepair],
				materials[(int)MaterialsType.InstantBuild],
				materials[(int)MaterialsType.ImprovementMaterials]);
		}

		private void MaterialsHistory(int[] source)
		{
			const int sourcelength = 4;

			if (source == null || source.Length != sourcelength)
				return;

			bool process = false;

			for (int i = 0; i < sourcelength; i++)
				if (source[i] != materials[i])
				{
					materials[i] = source[i];
					process = true;
				}

			if (materials[(int)MaterialsType.DevelopmentKits] != KanColleClient.Current.Homeport.Materials.DevelopmentMaterials)
			{
				materials[(int)MaterialsType.DevelopmentKits] = KanColleClient.Current.Homeport.Materials.DevelopmentMaterials;
				process = true;
			}

			if (materials[(int)MaterialsType.InstantRepair] != KanColleClient.Current.Homeport.Materials.InstantRepairMaterials)
			{
				materials[(int)MaterialsType.InstantRepair] = KanColleClient.Current.Homeport.Materials.InstantRepairMaterials;
				process = true;
			}

			if (materials[(int)MaterialsType.InstantBuild] != KanColleClient.Current.Homeport.Materials.InstantBuildMaterials)
			{
				materials[(int)MaterialsType.InstantBuild] = KanColleClient.Current.Homeport.Materials.InstantBuildMaterials;
				process = true;
			}

			if (materials[(int)MaterialsType.ImprovementMaterials] != KanColleClient.Current.Homeport.Materials.ImprovementMaterials)
			{
				materials[(int)MaterialsType.ImprovementMaterials] = KanColleClient.Current.Homeport.Materials.ImprovementMaterials;
				process = true;
			}

			if (!process) return;

			Log(materials[(int)MaterialsType.Fuel],
				materials[(int)MaterialsType.Ammo],
				materials[(int)MaterialsType.Steel],
				materials[(int)MaterialsType.Bauxite],
				materials[(int)MaterialsType.DevelopmentKits],
				materials[(int)MaterialsType.InstantRepair],
				materials[(int)MaterialsType.InstantBuild],
				materials[(int)MaterialsType.ImprovementMaterials]);
		}

		enum MaterialsType
		{
			Fuel = 0,
			Ammo = 1,
			Steel = 2,
			Bauxite = 3,
			InstantBuild = 4,
			InstantRepair = 5,
			DevelopmentKits = 6,
			ImprovementMaterials = 7,
		}
	}
}
