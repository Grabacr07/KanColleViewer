using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using Livet;
using System;
using System.Collections.Specialized;
using System.IO;

namespace Grabacr07.KanColleWrapper
{
	public class Logger : NotificationObject
	{
		public bool EnableLogging { get; set; }

		private bool waitingForShip = false;
		private int dockid;
		private int[] shipmats;

		enum LogType { BuildItem, BuildShip, ShipDrop };

		internal Logger(KanColleProxy proxy)
		{
			this.shipmats = new int[5];

			// ちょっと考えなおす
			proxy.api_req_kousyou_createitem.TryParse<kcsapi_createitem>().Subscribe(x => this.CreateItem(x.Data, x.Request));
			proxy.api_req_kousyou_createship.TryParse<kcsapi_createship>().Subscribe(x => this.CreateShip(x.Request));
			proxy.api_get_member_kdock.TryParse<kcsapi_kdock[]>().Subscribe(x => this.KDock(x.Data));
			proxy.api_req_sortie_battleresult.TryParse<kcsapi_battleresult>().Subscribe(x => this.BattleResult(x.Data));
			proxy.api_req_combined_battle_battleresult.TryParse<kcsapi_battleresult>().Subscribe(x => this.BattleResult(x.Data));
		}

		private void CreateItem(kcsapi_createitem source, NameValueCollection req)
		{
			this.Log(LogType.BuildItem, "{0},{1},{2},{3},{4},{5},{6}", DateTime.Now.ToString("yyyy/M/d H:mm"),
				source.api_create_flag == 1 ? KanColleClient.Current.Master.SlotItems[source.api_slot_item.api_slotitem_id].Name : "NA",
				KanColleClient.Current.Homeport.Organization.Fleets[1].Ships[0].Info.ShipType.Name,
				req["api_item1"], req["api_item2"], req["api_item3"], req["api_item4"]);

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
			foreach (var dock in docks)
			{
				if (waitingForShip && dock.api_id == dockid)
				{
					Log(LogType.BuildShip, "{0},{1},{2},{3},{4},{5},{6}", DateTime.Now.ToString("yyyy/M/d H:mm"),KanColleClient.Current.Master.Ships[dock.api_created_ship_id].Name, shipmats[0], shipmats[1], shipmats[2], shipmats[3], shipmats[4]);
					waitingForShip = false;
				}
			}
		}

		private void BattleResult(kcsapi_battleresult br)
		{
			string ShipName="";
			string ShipType="";
			if (br.api_get_ship != null)
			{
			ShipName = KanColleClient.Current.Translations.GetTranslation(br.api_get_ship.api_ship_name, TranslationType.Ships, br);
			ShipType = KanColleClient.Current.Translations.GetTranslation(br.api_quest_name, TranslationType.OperationMaps, br);
			}

			Log(LogType.ShipDrop, "{0},{1},{2},{3},{4}",DateTime.Now.ToString("yyyy/M/d H:mm"),ShipName,ShipType,
				KanColleClient.Current.Translations.GetTranslation(br.api_enemy_info.api_deck_name, TranslationType.OperationSortie, br,-1),
				br.api_win_rank);
		}

		private void Log(LogType Type, string format, params object[] args)
		{
			if (!EnableLogging)
				return;

			string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

			if (Type == LogType.BuildItem)
			{
				if (!System.IO.File.Exists(MainFolder + "\\ItemBuildLog.csv"))
				{
					using (StreamWriter w = File.AppendText(MainFolder + "\\ItemBuildLog.csv"))
					{
						w.WriteLine("날짜,결과,비서함,연료,탄,강재,보크사이트", args);
					}
				}

				using (StreamWriter w = File.AppendText(MainFolder + "\\ItemBuildLog.csv"))
				{
					w.WriteLine(format, args);
				}
			}
			else if (Type == LogType.BuildShip)
			{
				if (!System.IO.File.Exists(MainFolder + "\\ShipBuildLog.csv"))
				{
					using (StreamWriter w = File.AppendText(MainFolder + "\\ShipBuildLog.csv"))
					{
						w.WriteLine("날짜,결과,연료,탄,강재,보크사이트,개발자재", args);
					}
				}

				using (StreamWriter w = File.AppendText(MainFolder + "\\ShipBuildLog.csv"))
				{
					w.WriteLine(format, args);
				}
			}
			else if (Type == LogType.ShipDrop)
			{
				if (!System.IO.File.Exists(MainFolder + "\\DropLog.csv"))
				{
					using (StreamWriter w = File.AppendText(MainFolder + "\\DropLog.csv"))
					{
						w.WriteLine("날짜,드랍,해역,적 함대,랭크", args);
					}
				}

				using (StreamWriter w = File.AppendText(MainFolder + "\\DropLog.csv"))
				{
					w.WriteLine(format, args);
				}
			}
		}
	}
}
