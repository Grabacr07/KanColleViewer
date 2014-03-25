using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Codeplex.Data;
using Fiddler;
using Grabacr07.KanColleWrapper.Internal;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using Livet;

namespace Grabacr07.KanColleWrapper
{
	public class SortieLogger : NotificationObject
	{
		#region IsInSortie 変更通知プロパティ

		private bool _IsInSortie;

		/// <summary>
		/// 艦隊が出撃中かどうかを示す値を取得します。
		/// </summary>
		public bool IsInSortie
		{
			get { return this._IsInSortie; }
			private set
			{
				if (this._IsInSortie != value)
				{
					this._IsInSortie = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Current 変更通知プロパティ

		private Sortie _Current;

		public Sortie Current
		{
			get { return this._Current; }
			set
			{
				if (this._Current != value)
				{
					this._Current = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		internal SortieLogger(KanColleProxy proxy)
		{
			var start = proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_get_member/mapcell");
			var end = proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_auth_member/logincheck");

			var log = proxy.ApiSessionSource
				.SkipUntil(start.Do(_ => this.Sortie()))
				.TakeUntil(end)
				.Finally(this.Homing)
				.Repeat()
				.Where(_ => this.CheckLogging())
				.Publish();

			log.Where(x => x.PathAndQuery == "/kcsapi/api_req_map/start")
				.TryParse<kcsapi_map_next>()
				.Select(x => new MapCellEvent(x))
				.Subscribe(_ => { });

			log.Where(x => x.PathAndQuery == "/kcsapi/api_req_map/next")
				.TryParse<kcsapi_map_next>()
				.Select(x => new MapCellEvent(x))
				.Subscribe(_ => { });

			log.Where(x => x.PathAndQuery == "/kcsapi/api_req_sortie/battle")
				.TryParse<kcsapi_battle>()
				.Select(x => new Combat(x))
				.Subscribe(_ => { });

			log.Where(x => x.PathAndQuery == "/kcsapi/api_req_sortie/battleresult")
				.TryParse<kcsapi_battleresult>()
				.Select(x => new CombatResult(x))
				.Subscribe(_ => { });

			log.Connect();
		}


		/// <summary>
		/// 艦隊出撃
		/// </summary>
		private void Sortie()
		{
			this.IsInSortie = true;

			this.Current = new Sortie();
		}

		/// <summary>
		/// 艦隊帰投
		/// </summary>
		private void Homing()
		{
			this.IsInSortie = false;
		}

		private bool CheckLogging()
		{
			return this.IsInSortie && this.Current != null;
		}

	}

	public class Sortie : NotificationObject
	{
		internal Sortie() { }
	}


	public class SortieLog : NotificationObject
	{
	}

	public class BattleLog : SortieLog
	{

		#region Result 変更通知プロパティ

		private CombatResult _Result;

		public CombatResult Result
		{
			get { return this._Result; }
			set
			{
				if (this._Result != value)
				{
					this._Result = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

	}

	public class ItemGet : SortieLog
	{
		public int Count { get; internal set; }
		public int IconId { get; internal set; }
	}
}
