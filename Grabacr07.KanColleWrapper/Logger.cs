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

		private enum LogType
		{
			BuildItem,
			BuildShip,
			ShipDrop
		};

		internal Logger(KanColleProxy proxy)
		{
			this.shipmats = new int[5];

			// ちょっと考えなおす
			//proxy.api_req_kousyou_createitem.TryParse<kcsapi_createitem>().Subscribe(x => this.CreateItem(x.Data, x.Request));
			//proxy.api_req_kousyou_createship.TryParse<kcsapi_createship>().Subscribe(x => this.CreateShip(x.Request));
			//proxy.api_get_member_kdock.TryParse<kcsapi_kdock[]>().Subscribe(x => this.KDock(x.Data));
			//proxy.api_req_sortie_battleresult.TryParse<kcsapi_battleresult>().Subscribe(x => this.BattleResult(x.Data));
		}

		private void CreateItem(kcsapi_createitem item, NameValueCollection req)
		{
			this.Log(LogType.BuildItem, "{0},{1},{2},{3},{4},{5},{6}", item.api_create_flag == 1 ? KanColleClient.Current.Master.SlotItems[item.api_slotitem_id].Name : "NA",
				KanColleClient.Current.Homeport.Organization.Fleets[1].Ships[0].Info.ShipType.Name,
				req["api_item1"], req["api_item2"], req["api_item3"], req["api_item4"], DateTime.Now.ToString("M/d/yyyy H:mm"));
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
				this.Log(LogType.BuildShip, "{0},{1},{2},{3},{4},{5},{6}", KanColleClient.Current.Master.Ships[dock.api_created_ship_id].Name, this.shipmats[0], this.shipmats[1], this.shipmats[2], this.shipmats[3], this.shipmats[4], DateTime.Now.ToString("M/d/yyyy H:mm"));
				this.waitingForShip = false;
			}
		}

		private void BattleResult(kcsapi_battleresult br)
		{
			if (br.api_get_ship == null)
				return;

			this.Log(LogType.ShipDrop, "{0},{1},{2},{3},{4}", br.api_get_ship.api_ship_name, br.api_quest_name, br.api_enemy_info.api_deck_name, br.api_win_rank, DateTime.Now.ToString("M/d/yyyy H:mm"));
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
							w.WriteLine("Result,Secretary,Fuel,Ammo,Steel,Bauxite,Date", args);
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
							w.WriteLine("Result,Fuel,Ammo,Steel,Bauxite,# of Build Materials,Date", args);
						}
					}
					using (var w = File.AppendText(mainFolder + "\\ShipBuildLog.csv"))
					{
						w.WriteLine(format, args);
					}
					break;

				case LogType.ShipDrop:
					if (!File.Exists(mainFolder + "\\DropLog.csv"))
					{
						using (var w = File.AppendText(mainFolder + "\\DropLog.csv"))
						{
							w.WriteLine("Result,Operation,Enemy Fleet,Rank,Date", args);
						}
					}
					using (var w = File.AppendText(mainFolder + "\\DropLog.csv"))
					{
						w.WriteLine(format, args);
					}
					break;
			}
		}
	}
}
