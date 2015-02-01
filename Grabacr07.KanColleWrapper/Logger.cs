using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using Livet;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;

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
					Log(LogType.BuildShip, "{0},{1},{2},{3},{4},{5},{6}", DateTime.Now.ToString("yyyy/M/d H:mm"), KanColleClient.Current.Master.Ships[dock.api_created_ship_id].Name, shipmats[0], shipmats[1], shipmats[2], shipmats[3], shipmats[4]);
					waitingForShip = false;
				}
			}
		}

		private void BattleResult(kcsapi_battleresult br)
		{
			string ShipName = "";
			string ShipType = "";
			if (br.api_get_ship != null)
			{
				ShipName = KanColleClient.Current.Translations.GetTranslation(br.api_get_ship.api_ship_name, TranslationType.Ships, br);
			}
			ShipType = KanColleClient.Current.Translations.GetTranslation(br.api_quest_name, TranslationType.OperationMaps, br);

			Log(LogType.ShipDrop, "{0},{1},{2},{3},{4}", DateTime.Now.ToString("yyyy/M/d H:mm"), ShipName, ShipType,
				KanColleClient.Current.Translations.GetTranslation(br.api_enemy_info.api_deck_name, TranslationType.OperationSortie, br, -1),
				br.api_win_rank);
		}

		private void Log(LogType Type, string format, params object[] args)
		{
			if (!EnableLogging)
				return;
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
				if (!System.IO.File.Exists(MainFolder + "\\DropLog.csv"))
				{
					var csvPath = Path.Combine(MainFolder, "DropLog.csv");
					using (var fileStream = new FileStream(csvPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
					using (var writer = new BinaryWriter(fileStream))
					{
						writer.Write(utf8Bom);
					}
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
			LogToBin(Type, format, args);
		}

		private void LogToBin(LogType Type, string format, params object[] args)
		{
			string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

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
				//날짜,드랍,해역,적 함대,랭크
				var binPath = Path.Combine(MainFolder, "Bin", "Drop.bin");
				var item = new DropStringLists();

				item.Date = args[0].ToString();
				item.Drop = args[1].ToString();
				item.SeaArea = args[2].ToString();
				item.EnemyFleet = args[3].ToString();
				item.Rank = args[4].ToString();

				using (var fileStream = new FileStream(binPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
				using (var writer = new BinaryWriter(fileStream))
				{
					writer.Seek(0, SeekOrigin.End);
					writer.Write(item.Date);
					writer.Write(item.Drop);
					writer.Write(item.SeaArea);
					writer.Write(item.EnemyFleet);
					writer.Write(item.Rank);

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
