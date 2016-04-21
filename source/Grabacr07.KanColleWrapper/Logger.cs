using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using Livet;
using Nekoxy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace Grabacr07.KanColleWrapper
{
	public class Logger : NotificationObject
	{
		private bool waitingForShip = false;
		private int dockid;
		private int[] shipmats;

		private int CurrentDeckId;
		private JObject BattleData;

		enum LogType { BuildItem, BuildShip, ShipDrop };

		internal Logger(KanColleProxy proxy)
		{
			this.shipmats = new int[5];
			try
			{
				proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_battle_midnight/battle")
					.Subscribe(x => this.BattleUpdate(x, true));

				proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_battle_midnight/sp_midnight")
					.Subscribe(x => this.BattleUpdate(x, true));

				proxy.api_req_combined_battle_airbattle
					.Subscribe(x => this.BattleUpdate(x));

				proxy.api_req_combined_battle_battle
					.Subscribe(x => this.BattleUpdate(x));

				proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_combined_battle/battle_water")
					.Subscribe(x => this.BattleUpdate(x));

				proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_combined_battle/midnight_battle")
					.Subscribe(x => this.BattleUpdate(x, true));

				proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_combined_battle/sp_midnight")
					.Subscribe(x => this.BattleUpdate(x, true));

				proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_practice/battle")
					.Subscribe(x => this.BattleUpdate(x));

				proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_practice/midnight_battle")
					.Subscribe(x => this.BattleUpdate(x, true));

				proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_sortie/airbattle")
					.Subscribe(x => this.BattleUpdate(x));

				proxy.api_req_sortie_battle
					.Subscribe(x => this.BattleUpdate(x));

				proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_sortie/ld_airbattle")
					.Subscribe(x => this.BattleUpdate(x));

				proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_combined_battle/ld_airbattle")
					.Subscribe(x => this.BattleUpdate(x));

				proxy.api_req_map_start.TryParse<map_start_next>().Subscribe(x => MapStartNext(x.Data, x.Request["api_deck_id"]));
				proxy.api_req_map_next.TryParse<map_start_next>().Subscribe(x => MapStartNext(x.Data));
				proxy.api_req_sortie_battleresult.TryParse<kcsapi_battleresult>().Subscribe(x => this.BattleResult(x.Data));
				proxy.api_req_combined_battle_battleresult.TryParse<kcsapi_battleresult>().Subscribe(x => this.BattleResult(x.Data));

				// ちょっと考えなおす
				proxy.api_req_kousyou_createitem.TryParse<kcsapi_createitem>().Subscribe(x => this.CreateItem(x.Data, x.Request));
				proxy.api_req_kousyou_createship.TryParse<kcsapi_createship>().Subscribe(x => this.CreateShip(x.Request));
				proxy.api_get_member_kdock.TryParse<kcsapi_kdock[]>().Subscribe(x => this.KDock(x.Data));

			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}
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
					Log(LogType.BuildShip, "{0},{1},{2},{3},{4},{5},{6}", DateTime.Now.ToString("yyyy/M/d H:mm"), KanColleClient.Current.Master.Ships[dock.api_created_ship_id].Name, shipmats[0], shipmats[1], shipmats[2], shipmats[3], shipmats[4]);
					waitingForShip = false;
				}
			}
		}

/*
//KC3 리플레이 데이터 예시
{
	"diff": 0,
	"world": 1,
	"mapnum": 1,
	"fleetnum": 1,
	"combined": 0,
	"fleet1": [
		{
			"mst_id": 399,
			"level": 138,
			"kyouka": [ 9, 48, 0, 14, 6 ],
			"morale": 67,
			"equip": [ 15, 15, 0, 0, 0 ]
		}
	],
	"support1": 0,
	"support2": 0,
	"time": 1460983675,
	"hq": "8040045",
	"id": 3,
	"battles": [
		{
			"node": 1,
			"data": { },
			"yasen": { },
			"rating": "SS",
			"drop": 44,
			"baseEXP": 30,
			"hqEXP": 10
		}
	]
}
*/
		private void MapStartNext(map_start_next startnext, string api_deck_id = null)
		{
			if (api_deck_id != null)
				CurrentDeckId = int.Parse(api_deck_id);

			#region KC3 리플레이 JSON 작성
			var organization = KanColleClient.Current.Homeport.Organization;
			BattleData = new JObject(
				new JProperty("diff", 0),
				new JProperty("world", startnext.api_maparea_id),
				new JProperty("mapnum", startnext.api_mapinfo_no),
				new JProperty("fleetnum", CurrentDeckId),
				new JProperty("combined", Convert.ToInt32(organization.Combined && CurrentDeckId == 1)),
				new JProperty("support1", GetSupportingFleet(false)),
				new JProperty("support2", GetSupportingFleet(true)),
				new JProperty("time", (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds),
				new JProperty("hq", KanColleClient.Current.Homeport.Admiral.Experience.ToString()));

			JProperty[] fleetdata = new JProperty[organization.Fleets.Count];

			int count = 0;
			foreach (var fleet in organization.Fleets)
			{
				fleetdata[count] = new JProperty($"fleet{count + 1}", 
					new JArray(fleet.Value.Ships.Select(ship =>
						new JObject(
							new JProperty("mst_id", ship.Info.Id),
							new JProperty("level", ship.Level),
							new JProperty("kyouka",
								new JArray(ship.Firepower.Upgraded, ship.Torpedo.Upgraded, ship.AA.Upgraded, ship.Armer.Upgraded, ship.Luck.Upgraded)),
							new JProperty("morale", ship.Condition),
							new JProperty("equip",
								new JArray(ship.Slots.Select(y => y.Item.Info.Id).ToArray()))))));

				count++;
			}

			foreach(var fleet in fleetdata)
			{
				BattleData.Add(fleet);
			}

			JProperty battles = new JProperty("battles", 
				new JArray(
					new JObject(
						new JProperty("node", startnext.api_no),
						new JProperty("data", new JObject()),
						new JProperty("yasen", new JObject()))));

			BattleData.Add(battles);
			#endregion
		}

		//코드 출처: https://github.com/KC3Kai/KC3Kai/
		/// <summary>
		/// 지원 함대를 가져옵니다.
		/// </summary>
		/// <param name="bossSupport">true: 함대결전지원임무, false: 전방지원함대</param>
		/// <returns>0: 없음, 1~4: 1~4함대</returns>
		private int GetSupportingFleet(bool bossSupport)
		{
			foreach(var fleet in KanColleClient.Current.Homeport.Organization.Fleets)
				if (fleet.Value.Expedition.IsInExecution)
					if (IsSupportExpedition(fleet.Value.Expedition.Mission.Id, bossSupport))
						return fleet.Value.Id+1;

			return 0;
		}

		//코드 출처: https://github.com/KC3Kai/KC3Kai/
		/// <summary>
		/// 해당 원정 번호가 함대 지원 원정인지를 구합니다.
		/// </summary>
		/// <param name="expedNum">원정 번호</param>
		/// <param name="bossSupport">true: 함대결전지원임무, false: 전방지원함대</param>
		/// <returns></returns>
		private bool IsSupportExpedition(int expedNum, bool bossSupport)
		{
			int w, n;
			bool e = (expedNum > 100);
			if (e) expedNum -= 100;
			w = ((expedNum - 1) / 8) + 1;
			n = (expedNum - 1) % 8;
			return (w == 5 || e) && (Convert.ToBoolean(n) == bossSupport);
		}

		private void BattleUpdate(Session session, bool yasen=false)
		{
			string battletype = "data";
			if (yasen)
				battletype = "yasen";

			BattleData["battles"][0][battletype] = JObject.Parse(session.Response.BodyAsString.Replace("svdata=", ""))["api_data"];
		}

		private void BattleResult(kcsapi_battleresult br)
		{
			string ShipName = "";
			string MapType = "";
			if (br.api_get_ship != null)
			{
				ShipName = KanColleClient.Current.Translations.GetTranslation(br.api_get_ship.api_ship_name, TranslationType.Ships, false, br);
			}
			MapType = KanColleClient.Current.Translations.GetTranslation(br.api_quest_name, TranslationType.OperationMaps, false, br);

			string currentTime = DateTime.Now.ToString("yyyy\\/MM\\/dd HH\\:mm\\:ss", System.Globalization.CultureInfo.InvariantCulture);

			#region JSON파일 저장
			JObject lastBattle = (JObject)BattleData["battles"][0];

			lastBattle.Add(new JProperty("rank", br.api_win_rank));
			lastBattle.Add(new JProperty("drop", br.api_get_ship?.api_ship_id));
			lastBattle.Add(new JProperty("baseEXP", br.api_get_base_exp));
			lastBattle.Add(new JProperty("hqEXP", br.api_get_exp));

			string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

			JObject json;
			if (File.Exists(Path.Combine(MainFolder, "replaydata.json")))
				json = JObject.Parse(File.ReadAllText(Path.Combine(MainFolder, "replaydata.json")));
			else
				json = new JObject();
			
			json.Add(new JProperty(currentTime, BattleData));

			using (StreamWriter file = File.CreateText(Path.Combine(MainFolder, "replaydata.json")))
			{
				using (JsonTextWriter writer = new JsonTextWriter(file))
				{
					json.WriteTo(writer);
					writer.Close();
				}
				file.Close();
			}
			#endregion

			#region CSV파일 저장
			//날짜,해역이름,해역,적 함대,랭크,드랍
			Log(LogType.ShipDrop, "{0},{1},{2},{3},{4},{5}",
				currentTime,
				MapType,
				$"{BattleData.SelectToken("world").ToString()}-{(int)BattleData.SelectToken("mapnum")}-{(int)BattleData.SelectToken("battles[0].node")}",
				KanColleClient.Current.Translations.GetTranslation(br.api_enemy_info.api_deck_name, TranslationType.OperationSortie, false, br, -1),
				br.api_win_rank, 
				ShipName);
			#endregion
		}

		private void Log(LogType Type, string format, params object[] args)
		{
			try
			{
				byte[] utf8Bom = { 0xEF, 0xBB, 0xBF };
				string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

				if (Type == LogType.BuildItem)
				{
					if (!System.IO.File.Exists(MainFolder + "\\ItemBuildLog.csv"))
					{
						var csvPath = Path.Combine(MainFolder, "ItemBuildLog.csv");
						using (var fileStream = new FileStream(csvPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
						using (var writer = new BinaryWriter(fileStream))
						{
							writer.Write(utf8Bom);
						}
						using (StreamWriter w = File.AppendText(MainFolder + "\\ItemBuildLog.csv"))
						{
							w.WriteLine("날짜,결과,비서함,연료,탄,강재,보크사이트", args);
						}
					}
					using (StreamWriter w = File.AppendText(MainFolder + "\\ItemBuildLog.csv"))
					{
						w.WriteLine(format, args);
					}
					//bin 작성 시작

				}
				else if (Type == LogType.BuildShip)
				{
					if (!System.IO.File.Exists(MainFolder + "\\ShipBuildLog.csv"))
					{
						var csvPath = Path.Combine(MainFolder, "ShipBuildLog.csv");
						using (var fileStream = new FileStream(csvPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
						using (var writer = new BinaryWriter(fileStream))
						{
							writer.Write(utf8Bom);
						}
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
					if (!System.IO.File.Exists(MainFolder + "\\DropLog2.csv"))
					{
						var csvPath = Path.Combine(MainFolder, "DropLog2.csv");
						using (var fileStream = new FileStream(csvPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
						using (var writer = new BinaryWriter(fileStream))
						{
							writer.Write(utf8Bom);
						}
						using (StreamWriter w = File.AppendText(MainFolder + "\\DropLog2.csv"))
						{
							w.WriteLine("ID,날짜,해역이름,해역,적 함대,랭크,드랍", args);
						}
					}

					using (StreamWriter w = File.AppendText(MainFolder + "\\DropLog2.csv"))
					{
						w.WriteLine(format, args);
					}
				}
				LogToBin(Type, format, args);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		private void LogToBin(LogType Type, string format, params object[] args)
		{
			string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
			if (!Directory.Exists(Path.Combine(MainFolder, "Bin")))
				Directory.CreateDirectory(Path.Combine(MainFolder, "Bin"));
			#region BuildItem
			if (Type == LogType.BuildItem)
			{
				var binPath = Path.Combine(MainFolder, "Bin", "ItemBuild.bin");
				var item = new ItemStringLists();

				item.Date = args[0].ToString();
				if (args[1].ToString() == "NA") item.Results = string.Empty;
				else item.Results = args[1].ToString();
				item.Assistant = args[2].ToString();
				item.Fuel = Convert.ToInt32(args[3].ToString());
				item.Bullet = Convert.ToInt32(args[4].ToString());
				item.Steel = Convert.ToInt32(args[5].ToString());
				item.bauxite = Convert.ToInt32(args[6].ToString());

				using (var fileStream = new FileStream(binPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
				using (var writer = new BinaryWriter(fileStream))
				{
					writer.Seek(0, SeekOrigin.End);
					writer.Write(item.Date);
					if (item.Results == "NA") writer.Write(string.Empty);
					else writer.Write(item.Results);
					writer.Write(item.Assistant);
					writer.Write(item.Fuel);
					writer.Write(item.Bullet);
					writer.Write(item.Steel);
					writer.Write(item.bauxite);

					fileStream.Dispose();
					fileStream.Close();
					writer.Dispose();
					writer.Close();
				}
			}
			#endregion

			#region BuildShip
			else if (Type == LogType.BuildShip)
			{
				var binPath = Path.Combine(MainFolder, "Bin", "ShipBuild.bin");
				var item = new BuildStirngLists();

				item.Date = args[0].ToString();
				item.Results = args[1].ToString();
				item.Fuel = Convert.ToInt32(args[2].ToString());
				item.Bullet = Convert.ToInt32(args[3].ToString());
				item.Steel = Convert.ToInt32(args[4].ToString());
				item.bauxite = Convert.ToInt32(args[5].ToString());
				item.UseItems = Convert.ToInt32(args[6].ToString());

				using (var fileStream = new FileStream(binPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
				using (var writer = new BinaryWriter(fileStream))
				{
					writer.Seek(0, SeekOrigin.End);
					writer.Write(item.Date);
					writer.Write(item.Results);
					writer.Write(item.Fuel);
					writer.Write(item.Bullet);
					writer.Write(item.Steel);
					writer.Write(item.bauxite);
					writer.Write(item.UseItems);

					fileStream.Dispose();
					fileStream.Close();
					writer.Dispose();
					writer.Close();
				}
			}
			#endregion

			#region ShipDrop
			else if (Type == LogType.ShipDrop)
			{
				// 날짜,해역이름,해역,적 함대,랭크,드랍
				var binPath = Path.Combine(MainFolder, "Bin", "Drop2.bin");
				var item = new DropStringLists();
				
				item.Date = args[0].ToString();
				item.SeaArea = args[1].ToString();
				item.MapInfo = args[2].ToString();
				item.EnemyFleet = args[3].ToString();
				item.Rank = args[4].ToString();
				item.Drop = args[5].ToString();

				using (var fileStream = new FileStream(binPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
				using (var writer = new BinaryWriter(fileStream))
				{
					writer.Seek(0, SeekOrigin.End);
					writer.Write(item.Date);
					writer.Write(item.SeaArea);
					writer.Write(item.MapInfo);
					writer.Write(item.EnemyFleet);
					writer.Write(item.Rank);
					writer.Write(item.Drop);

					fileStream.Dispose();
					fileStream.Close();
					writer.Dispose();
					writer.Close();
				}
			}
			#endregion
		}
	}
}
