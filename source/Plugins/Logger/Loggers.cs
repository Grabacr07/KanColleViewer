using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Microsoft.Win32;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using Grabacr07.KanColleWrapper.Models.Translations;
using Livet;

namespace Logger
{
	public abstract class LoggerBase : NotificationObject
	{
		private readonly string _KeyPath = @"SOFTWARE\Smooth and Flat\KanColleViewer\Logger\";

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

		#region Text 変更通知プロパティ

		private string _Text;

		public string Text
		{
			get { return this._Text; }
			set
			{
				if (this._Text != value)
				{
					this._Text = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Enabled 変更通知プロパティ

		private bool _Enabled;

		public bool Enabled
		{
			get { return this._Enabled; }
			set
			{
				if (this._Enabled != value)
				{
					this._Enabled = value;
					this.SaveSettings();
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public void Log(string format, params object[] args)
		{
			if (!this.Enabled) return;

			try
			{
				string mainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
				using (var w = File.AppendText(mainFolder + @"\" + this.Filename))
				{
					w.WriteLine(format, args);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		public void LoadSettings()
		{
			RegistryKey LoggerPreferences = Registry.CurrentUser.OpenSubKey(_KeyPath + LoggerName, true);

			if (LoggerPreferences != null)
			{
				Enabled = LoggerPreferences.GetValue("Enabled", "False").ToString().Equals("True");
			}
			else
			{
				Enabled = false;
			}
		}

		public void SaveSettings()
		{
			RegistryKey LoggerPreferences = Registry.CurrentUser.CreateSubKey(_KeyPath + LoggerName);

			LoggerPreferences.SetValue("Enabled", Enabled.ToString());
		}

		public string TimestampISO()
		{
			return DateTime.UtcNow.ToString("o");
		}
	}

	public class ItemLog : LoggerBase
	{
		public ItemLog(KanColleProxy proxy)
		{
			proxy.api_req_kousyou_createitem.TryParse<kcsapi_createitem>().Subscribe(x => this.CreateItem(x.Data, x.Request));
			this.Text = "Development";
			this.Filename = "DevelopmentLog.csv";
			this.LoggerName = "Development";
			this.LoadSettings();
		}

		private void CreateItem(kcsapi_createitem item, NameValueCollection req)
		{
			Log("{0},{1},{2},{3},{4},{5},{6}",
				this.TimestampISO(),
				item.api_create_flag == 1 ? KanColleClient.Current.Master.SlotItems[item.api_slot_item.api_slotitem_id].Name : "Penguin",
				KanColleClient.Current.Homeport.Organization.Fleets[1].Ships[0].Info.ShipType.Name,
				req["api_item1"], req["api_item2"], req["api_item3"], req["api_item4"]);
		}
	}

	public class ConstructionLog : LoggerBase
	{
		private readonly int[] shipmats;
		private bool waitingForShip;
		private int dockid;

		public ConstructionLog(KanColleProxy proxy)
		{
			this.shipmats = new int[5];
			proxy.api_req_kousyou_createship.TryParse<kcsapi_createship>().Subscribe(x => this.CreateShip(x.Request));
			proxy.api_get_member_kdock.TryParse<kcsapi_kdock[]>().Subscribe(x => this.KDock(x.Data));
			this.Text = "Construction";
			this.Filename = "ConstructionLog.csv";
			this.LoggerName = "Construction";
			this.LoadSettings();
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
				this.Log("{0},{1},{2},{3},{4},{5},{6}", this.TimestampISO(), KanColleClient.Current.Master.Ships[dock.api_created_ship_id].Name, this.shipmats[0], this.shipmats[1], this.shipmats[2], this.shipmats[3], this.shipmats[4]);
				this.waitingForShip = false;
			}
		}
	}

	public class BattleLog : LoggerBase
	{
		public BattleLog(KanColleProxy proxy)
		{
			proxy.api_req_sortie_battleresult.TryParse<kcsapi_battleresult>().Subscribe(x => this.BattleResult(x.Data));
			this.Text = "Ships obtained as drops";
			this.Filename = "BattleLog.csv";
			this.LoggerName = "BattleResults";
			this.LoadSettings();
		}

		private void BattleResult(kcsapi_battleresult br)
		{
			if (br.api_get_ship == null)
				return;

			Log("{0},{1},{2},{3},{4}", this.TimestampISO(),
				br.api_get_ship.api_ship_name,
				br.api_quest_name,
				br.api_enemy_info.api_deck_name,
				br.api_win_rank);
		}
	}

	public class MaterialsLog : LoggerBase
	{
		public MaterialsLog(KanColleProxy proxy)
		{
			proxy.api_get_member_material.TryParse<kcsapi_material[]>().Subscribe(x => this.MaterialsHistory(x.Data));
			proxy.api_req_hokyu_charge.TryParse<kcsapi_charge>().Subscribe(x => this.MaterialsHistory(x.Data.api_material));
			proxy.api_req_kousyou_destroyship.TryParse<kcsapi_destroyship>().Subscribe(x => this.MaterialsHistory(x.Data.api_material));
			this.Text = "Materials";
			this.Filename = "MaterialsExpenditureLog.csv";
			this.LoggerName = "Materials";
			this.LoadSettings();
		}

		private void MaterialsHistory(kcsapi_material[] source)
		{
			if (source == null || source.Length != 7)
				return;

			Log("{0},{1},{2},{3},{4},{5},{6},{7}",
				this.TimestampISO(),
				source[0].api_value, source[1].api_value, source[2].api_value, source[3].api_value, source[6].api_value, source[5].api_value, source[4].api_value);
		}

		private void MaterialsHistory(int[] source)
		{
			if (source == null || source.Length != 4)
				return;

			Log("{0},{1},{2},{3},{4},{5},{6},{7}",
				this.TimestampISO(),
				source[0], source[1], source[2], source[3],
				KanColleClient.Current.Homeport.Materials.DevelopmentMaterials,
				KanColleClient.Current.Homeport.Materials.InstantRepairMaterials,
				KanColleClient.Current.Homeport.Materials.InstantBuildMaterials);
		}
	}
}
