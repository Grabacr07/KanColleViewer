using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Internal;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using Livet;

namespace Grabacr07.KanColleWrapper
{
	public class Logger : NotificationObject
	{
		public bool EnableLogging { get; set; }

		private bool waitingForShip;
		private int dockid;
		private readonly int[] shipmats;
        private DateTime lastMatLog;

		private enum LogType
		{
			BuildItem,
			BuildShip,
			ShipDrop,
            MaterialLog
		};

		internal Logger(KanColleProxy proxy)
		{
			this.shipmats = new int[5];
            this.lastMatLog = new DateTime(1970, 1, 1);
            this.EnableLogging = proxy.EnableLogging;
			proxy.api_req_kousyou_createitem.TryParse<kcsapi_createitem>().Subscribe(x => this.CreateItem(x.Data, x.Request));
			proxy.api_req_kousyou_createship.TryParse<kcsapi_createship>().Subscribe(x => this.CreateShip(x.Request));
			proxy.api_get_member_kdock.TryParse<kcsapi_kdock[]>().Subscribe(x => this.KDock(x.Data));
			proxy.api_req_sortie_battleresult.TryParse<kcsapi_battleresult>().Subscribe(x => this.BattleResult(x.Data));
		}

        public void UpdateMaterial()
        {
            if (DateTime.UtcNow.Subtract(this.lastMatLog).TotalSeconds > 300.0)
            {
                this.lastMatLog = DateTime.UtcNow;
                this.Log(Logger.LogType.MaterialLog, "{0},{1},{2},{3},{4},{5},{6},{7}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), KanColleClient.Current.Homeport.Materials.Fuel, KanColleClient.Current.Homeport.Materials.Ammunition, KanColleClient.Current.Homeport.Materials.Steel,
                    KanColleClient.Current.Homeport.Materials.Bauxite, KanColleClient.Current.Homeport.Materials.InstantRepairMaterials, KanColleClient.Current.Homeport.Materials.InstantBuildMaterials, KanColleClient.Current.Homeport.Materials.DevelopmentMaterials);
            }
        }
        
        private void CreateItem(kcsapi_createitem item, NameValueCollection req)
		{
            this.Log(LogType.BuildItem, "{0},{1},{2},{3},{4},{5},{6},{7}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), item.api_create_flag == 1 ? KanColleClient.Current.Master.SlotItems[item.api_slot_item.api_slotitem_id].Name : "",
                req["api_item1"], req["api_item2"], req["api_item3"], req["api_item4"], KanColleClient.Current.Homeport.Organization.Fleets[1].Ships[0].Info.ShipType.Name, KanColleClient.Current.Homeport.Organization.Fleets[1].Ships[0].Info.Name);
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
                this.Log(LogType.BuildShip, "{0},{1},{2},{3},{4},{5},{6}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), KanColleClient.Current.Master.Ships[dock.api_created_ship_id].Name, this.shipmats[0], this.shipmats[1], this.shipmats[2], this.shipmats[3], this.shipmats[4]);
				this.waitingForShip = false;
			}
		}

		private void BattleResult(kcsapi_battleresult br)
		{
            this.Log(LogType.ShipDrop, "{0},{1},{2},{3},{4}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), br.api_quest_name, br.api_win_rank, br.api_enemy_info.api_deck_name, (br.api_get_ship == null) ? "" : br.api_get_ship.api_ship_name);
		}

		private void Log(LogType type, string format, params object[] args)
		{
			if (!this.EnableLogging) return;

			var mainFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

			switch (type)
			{
				case LogType.BuildItem:
					if (!File.Exists(mainFolder + "\\ItemBuildLog.csv"))
					{
						using (var w = File.AppendText(mainFolder + "\\ItemBuildLog.csv"))
						{
                            w.WriteLine("Time,Result,Fuel,Ammo,Steel,Bauxite,Secretary Type,Secretary Name", args);
						}
					}
					using (var w = File.AppendText(mainFolder + "\\ItemBuildLog.csv"))
					{
						w.WriteLine(format, args);
					}
					break;

				case LogType.BuildShip:
					if (!File.Exists(mainFolder + "\\ShipBuildLog.csv"))
					{
						using (var w = File.AppendText(mainFolder + "\\ShipBuildLog.csv"))
						{
                            w.WriteLine("Time,Result,Fuel,Ammo,Steel,Bauxite,# of Build Materials", args);
						}
					}
					using (var w = File.AppendText(mainFolder + "\\ShipBuildLog.csv"))
					{
						w.WriteLine(format, args);
					}
					break;

				case LogType.ShipDrop:
                    if (!File.Exists(mainFolder + "\\BattleLog.csv"))
					{
                        using (var w = File.AppendText(mainFolder + "\\BattleLog.csv"))
						{
                            w.WriteLine("Time,Operation,Rank,Enemy Fleet,Result", args);
						}
					}
                    using (var w = File.AppendText(mainFolder + "\\BattleLog.csv"))
					{
						w.WriteLine(format, args);
					}
					break;

                case LogType.MaterialLog:
                    if (!File.Exists(mainFolder + "\\MaterialLog.csv"))
                    {
                        using (var w=File.AppendText(mainFolder+"\\MaterialLog.csv"))
                        {
                            w.WriteLine("Time,Fuel,Ammo,Steel,Bauxite,Repair Materials,Build Materials,Development Materials");
                        }
                    }
                    using (var w = File.AppendText(mainFolder + "\\MaterialLog.csv"))
                    {
                        w.WriteLine(format, args);
                    }
                    break;
			}
		}
	}
}
