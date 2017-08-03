using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grabacr07.KanColleWrapper.Models.Raw;
using Grabacr07.KanColleViewer.QuestTracker.Models.Model;

namespace Grabacr07.KanColleViewer.QuestTracker.Models.Extensions
{
	internal static class ModelExtension
	{
		private static readonly FleetDamages defaultValue = new FleetDamages();

		#region 지원함대
		public static FleetDamages GetEnemyDamages(this kcsapi_data_support_info support)
			=> support?.api_support_airatack?.api_stage3?.api_edam?.GetDamages()
				?? support?.api_support_hourai?.api_damage?.GetDamages()
				?? defaultValue;

		public static FleetDamages GetEachFirstEnemyDamages(this kcsapi_data_support_info support)
			=> support?.api_support_airatack?.api_stage3?.api_edam?.GetEachDamages()
			   ?? support?.api_support_hourai?.api_damage?.GetEachDamages()
			   ?? defaultValue;

		public static FleetDamages GetEachSecondEnemyDamages(this kcsapi_data_support_info support)
			=> support?.api_support_airatack?.api_stage3?.api_edam?.GetEachDamages(true)
			   ?? support?.api_support_hourai?.api_damage?.GetEachDamages(true)
			   ?? defaultValue;
		#endregion

		#region 포격전
		public static FleetDamages GetEnemyDamages(this kcsapi_data_hougeki hougeki)
			=> hougeki?.api_damage?.GetEnemyDamages(hougeki.api_df_list)
				?? defaultValue;

		public static FleetDamages GetEachFirstEnemyDamages(this kcsapi_data_hougeki hougeki)
			=> hougeki?.api_damage?.GetEachEnemyDamages(hougeki.api_df_list, hougeki.api_at_eflag)
			   ?? defaultValue;
		public static FleetDamages GetEachSecondEnemyDamages(this kcsapi_data_hougeki hougeki)
			=> hougeki?.api_damage?.GetEachEnemyDamages(hougeki.api_df_list, hougeki.api_at_eflag, true)
			   ?? defaultValue;
		#endregion

		#region 야전
		public static FleetDamages GetEnemyDamages(this kcsapi_data_midnight_hougeki hougeki)
			=> hougeki?.api_damage?.GetEnemyDamages(hougeki.api_df_list)
				?? defaultValue;
		#endregion

		#region 항공전
		public static FleetDamages GetEnemyDamages(this kcsapi_data_kouku kouku)
			=> kouku?.api_stage3?.api_edam?.GetDamages()
				?? defaultValue;

		public static FleetDamages GetSecondEnemyDamages(this kcsapi_data_kouku kouku)
			=> kouku?.api_stage3_combined?.api_edam?.GetDamages()
			   ?? defaultValue;
		#endregion

		#region 기지항공대
		public static FleetDamages GetEnemyDamages(this kcsapi_data_airbaseattack[] air_base)
			=> air_base?.Select(x => x?.api_stage3?.api_edam?.GetDamages() ?? defaultValue)
				?.Aggregate((a, b) => a.Add(b)) ?? defaultValue;

		public static FleetDamages GetEachFirstEnemyDamages(this kcsapi_data_airbaseattack[] attacks)
			=> attacks?.Select(x => x?.api_stage3?.api_edam?.GetDamages() ?? defaultValue)
			?.Aggregate((a, b) => a.Add(b)) ?? defaultValue;

		public static FleetDamages GetEachSecondEnemyDamages(this kcsapi_data_airbaseattack[] attacks)
			=> attacks?.Select(x => x?.api_stage3_combined?.api_edam?.GetDamages() ?? defaultValue)
			?.Aggregate((a, b) => a.Add(b)) ?? defaultValue;
		#endregion

		#region 기지항공대 분식
		public static FleetDamages GetEnemyDamages(this kcsapi_data_airbase_injection injection)
			=> injection?.api_stage3?.api_edam?.GetDamages()
			   ?? defaultValue;

		public static FleetDamages GetSecondEnemyDamages(this kcsapi_data_airbase_injection injection)
			=> injection?.api_stage3_combined?.api_edam?.GetDamages()
			   ?? defaultValue;
		#endregion

		#region 선제뇌격 / 뇌격전
		public static FleetDamages GetEnemyDamages(this kcsapi_data_raigeki raigeki)
			=> raigeki?.api_edam?.GetDamages()
				?? defaultValue;

		public static FleetDamages GetEachFirstEnemyDamages(this kcsapi_data_raigeki raigeki)
			=> raigeki?.api_edam?.GetEachDamages()
			   ?? defaultValue;

		public static FleetDamages GetEachSecondEnemyDamages(this kcsapi_data_raigeki raigeki)
			=> raigeki?.api_edam?.GetEachDamages(true)
			   ?? defaultValue;
		#endregion


		public static IEnumerable<T> GetFriendData<T>(this IEnumerable<T> source, int origin = 1)
			=> source.Skip(origin).Take(6);

		public static IEnumerable<T> GetEnemyData<T>(this IEnumerable<T> source, int origin = 1)
			=> source.Skip(origin + 6).Take(6);

		public static IEnumerable<T> GetEachFriendData<T>(this IEnumerable<T> source, int origin = 1)
			=> source.Skip(origin).Take(6);

		public static IEnumerable<T> GetEachEnemyData<T>(this IEnumerable<T> source, int origin = 1)
			=> source.Skip(origin + 12).Take(6);


		public static FleetDamages GetDamages(this decimal[] damages)
			=> damages
				.GetFriendData()
				.Select(Convert.ToInt32)
				.ToArray()
				.ToFleetDamages();

		public static FleetDamages GetEnemyDamages(this object[] damages, object[] df_list)
			=> damages
				.ToIntArray()
				.ToSortedDamages(df_list.ToIntArray())
				.GetEnemyData(0)
				.ToFleetDamages();

		public static FleetDamages GetEachDamages(this decimal[] damages, bool IsSecond = false)
			=> damages
				.GetFriendData(IsSecond ? 7 : 1)
				.Select(Convert.ToInt32)
				.ToArray()
				.ToFleetDamages();

		public static FleetDamages GetEachEnemyDamages(this object[] damages, object[] df_list, int[] at_eflag, bool IsSecond = false)
			=> damages
				.ToSortedDamages(df_list, at_eflag)
				.GetEachEnemyData(IsSecond ? 6 : 0)
				.ToFleetDamages();

		private static int[] ToIntArray(this object[] damages)
			=> damages
				.Where(x => x is Array)
				.Select(x => ((Array)x).Cast<object>())
				.SelectMany(x => x.Select(Convert.ToInt32))
				.ToArray();

		private static int[] ToSortedDamages(this int[] damages, int[] dfList)
		{
			var zip = damages.Zip(dfList, (da, df) => new { df, da });
			var ret = new int[12];

			foreach (var d in zip.Where(d => 0 < d.df))
				ret[d.df - 1] += d.da;

			return ret;
		}

		private static int[] ToSortedDamages(this object[] damages, object[] dfList, int[] at_eflag)
		{
			var zip = damages.Zip(dfList, (da, df) => new { df, da })
				.Zip(at_eflag, (dl, ef) => new { ef, dl.df, dl.da });

			var ret = new int[24];
			foreach (var d in zip.Where(d => d.ef == 1)) // Friend -> Enemy (ME)
			{
				if (d.df is Array)
				{
					var o = (d.df as object[]).Select(Convert.ToInt32).ToArray();
					for (var i = 0; i < o.Length; i++)
						ret[o[i] - 1] += (d.da as object[]).Select(Convert.ToInt32).ToArray()[i];
				}
				else
					ret[(int)d.df - 1] += (int)d.da;
			}
			foreach (var d in zip.Where(d => d.ef == 0)) // Enemy -> Friend (ME)
			{
				if (d.df is Array)
				{
					var o = (d.df as object[]).Select(Convert.ToInt32).ToArray();
					for (var i = 0; i < o.Length; i++)
						ret[o[i] + 12 - 1] += (d.da as object[]).Select(Convert.ToInt32).ToArray()[i];
				}
				else
					ret[(int)d.df + 12 - 1] += (int)d.da;
			}
			return ret;
		}
	}
}
