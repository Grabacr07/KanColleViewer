using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper
{
	/// <summary>
	/// プレイヤー データに依存しないマスター情報を表します。
	/// </summary>
	public class Master
	{
		/// <summary>
		/// すべての艦娘の定義を取得します。
		/// </summary>
		public MasterTable<ShipInfo> Ships { get; private set; }

		/// <summary>
		/// すべての装備アイテムの定義を取得します。
		/// </summary>
		public MasterTable<SlotItemInfo> SlotItems { get; private set; }

		/// <summary>
		/// すべての消費アイテムの定義を取得します。
		/// </summary>
		public MasterTable<UseItemInfo> UseItems { get; private set; }

		/// <summary>
		/// 艦種を取得します。
		/// </summary>
		public MasterTable<ShipType> ShipTypes { get; private set; }

		/// <summary>
		/// すべての任務を取得します。
		/// </summary>
		public MasterTable<Mission> Missions { get; private set; }


		internal Master(kcsapi_start2 start2)
		{
			this.ShipTypes = new MasterTable<ShipType>(start2.api_mst_stype.Select(x => new ShipType(x)));
			this.Ships = new MasterTable<ShipInfo>(start2.api_mst_ship.Select(x => new ShipInfo(x)));
			this.SlotItems = new MasterTable<SlotItemInfo>(start2.api_mst_slotitem.Select(x => new SlotItemInfo(x)));
			this.UseItems = new MasterTable<UseItemInfo>(start2.api_mst_useitem.Select(x => new UseItemInfo(x)));

			this.ReloadMissions();
		}


		public void ReloadMissions()
		{
			try
			{
				this.Missions = new MasterTable<Mission>(GetMissions());
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
				this.Missions = new MasterTable<Mission>();
			}
		}

		private static IEnumerable<Mission> GetMissions()
		{
			var csv = Path.Combine(
				// ReSharper disable once AssignNullToNotNullAttribute
				Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
				@"Assets\missions.csv");

			foreach (var values in File.ReadAllLines(csv, Encoding.UTF8)
				.Select(x => x.Split(','))
				.Where(x => x.Length == 3))
			{
				int id;
				if (!int.TryParse(values[0], out id)) continue;

				yield return new Mission(id, values[1], values[2]);
			}
		}
	}
}
