using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Codeplex.Data;
using Fiddler;
using Grabacr07.KanColleWrapper.Internal;
using Livet;

namespace Grabacr07.KanColleWrapper
{
	public class SortieLogger : NotificationObject
	{
		private Sortie current;

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

		

		internal SortieLogger(KanColleProxy proxy)
		{
			var start = proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_req_map/start");
			var end = proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_auth_member/logincheck");

			var sortieLog = proxy.ApiSessionSource
				.SkipUntil(start.Do(_ => this.Sortie()))
				.TakeUntil(end)
				.Finally(this.Homing)
				.Repeat();

			sortieLog.Subscribe(x =>
			{
				if (this.current != null) current.Set(x);
			});
		}


		/// <summary>
		/// 艦隊出撃
		/// </summary>
		private void Sortie()
		{
			this.IsInSortie = true;

			this.current = new Sortie();
		}

		/// <summary>
		/// 艦隊帰投
		/// </summary>
		private void Homing()
		{
			this.IsInSortie = false;
		}
	}

	public class Sortie : NotificationObject, IDisposable
	{
		private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();

		#region Trajectories 変更通知プロパティ

		private ObservableSynchronizedCollection<Trajectory> _Trajectories;

		public ObservableSynchronizedCollection<Trajectory> Trajectories
		{
			get { return this._Trajectories; }
			set
			{
				if (this._Trajectories != value)
				{
					this._Trajectories = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		internal Sortie() { }

		internal Sortie(KanColleProxy proxy)
		{
			proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_req_map/start")
				.Select(x => x.GetResponseAsJson());

		}

		internal void Set(Session session)
		{
			
		}



		private void ParseCell(string json)
		{
			var djson = DynamicJson.Parse(json);

			//djson.IsDefined("enemy");
		}




		public void Dispose()
		{
			this.compositeDisposable.Dispose();	
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class Trajectory
	{
		
	}

	public class Battle : Trajectory
	{
		
	}
}
