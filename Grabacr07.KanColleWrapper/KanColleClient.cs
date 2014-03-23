using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Internal;
using Livet;

namespace Grabacr07.KanColleWrapper
{
	public class KanColleClient : NotificationObject
	{
		#region singleton

		private static readonly KanColleClient current = new KanColleClient();

		public static KanColleClient Current
		{
			get { return current; }
		}

		#endregion

		/// <summary>
		/// 艦これの通信をフックするプロキシを取得します。
		/// </summary>
		public KanColleProxy Proxy { get; private set; }

		/// <summary>
		/// ユーザーに依存しないマスター情報を取得します。
		/// </summary>
		public Master Master { get; private set; }

		/// <summary>
		/// 母港の情報を取得します。
		/// </summary>
		public Homeport Homeport { get; private set; }

		/// <summary>
		/// 建造、開発、ドロップのログへアクセスできるようにします。
		/// </summary>
		public Logger Logger { get; private set; }

		/// <summary>
		/// 出撃ログへアクセスできるようにします。
		/// </summary>
		public SortieLogger SortieLogger { get; private set; }

		/// <summary>
		/// 発生したエラー情報のコレクションを取得します。
		/// </summary>
		public ObservableSynchronizedCollection<KanColleError> Errors { get; private set; }

		#region IsStarted 変更通知プロパティ

		private bool _IsStarted;

		/// <summary>
		/// 艦これが開始されているかどうかを示す値を取得します。
		/// </summary>
		public bool IsStarted
		{
			get { return this._IsStarted; }
			set
			{
				if (this._IsStarted != value)
				{
					this._IsStarted = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		private KanColleClient()
		{
			this.Errors = new ObservableSynchronizedCollection<KanColleError>();

			this.Proxy = new KanColleProxy();
			this.Master = new Master(this.Proxy);
			this.Homeport = new Homeport(this.Proxy);
			this.Logger = new Logger(this.Proxy);
			this.SortieLogger = new SortieLogger(this.Proxy);

			this.Proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_start")
				.TryParse()
				.Subscribe(x => this.IsStarted = x.IsSuccess);
		}
	}
}
