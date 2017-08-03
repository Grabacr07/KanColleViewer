﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleWrapper
{
	public class KanColleClient : Notifier
	{
		#region singleton

		public static KanColleClient Current { get; } = new KanColleClient();

		#endregion

		public IKanColleClientSettings Settings { get; set; }

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
		/// 번역
		/// </summary>
		public Translations Translations { get; private set; }

		/// <summary>
		/// 번역파일 및 기타 업데이트
		/// </summary>
		public Updater Updater { get; private set; }

		/// <summary>
		/// 자동번역기
		/// </summary>
		public WebTranslator WebTranslator { get; private set; }

		/// <summary>
		/// 기록
		/// </summary>
		public Logger Logger { get; private set; }

		/// <summary>
		/// 자원 기록
		/// </summary>
		public ResourceLogManager ResourceLogger { get; private set; }

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


		private KanColleClient()
		{
			this.Initialieze();

			var start = this.Proxy.api_req_map_start;
			var end = this.Proxy.api_port;

			this.Proxy.ApiSessionSource
				.SkipUntil(start.Do(_ => this.IsInSortie = true))
				.TakeUntil(end)
				.Finally(() => this.IsInSortie = false)
				.Repeat()
				.Subscribe();
		}


		public void Initialieze()
		{
			this.Translations = new Translations();
			this.WebTranslator = new WebTranslator();
			this.Updater = new Updater();

			var proxy = this.Proxy ?? (this.Proxy = new KanColleProxy());

			var start2Source = proxy.api_start2.TryParse<kcsapi_start2>();
			var requireInfoSource = proxy.api_get_member_require_info.TryParse<kcsapi_require_info>();

			// Homeport の初期化と require_info の適用に Master のインスタンスが必要なため、初回のみ足並み揃えて実行
			// 2 回目以降は受信したタイミングでそれぞれ更新すればよい

			var firstTime = start2Source
				.CombineLatest(requireInfoSource, (start2, requireInfo) => new { start2, requireInfo })
				.FirstAsync();

			firstTime.Subscribe(x =>
			{
				this.Master = new Master(x.start2.Data);
				this.Homeport = new Homeport(proxy);
				this.SetRequireInfo(x.requireInfo.Data);
				this.IsStarted = true;
			});

			start2Source
				.SkipUntil(firstTime)
				.Subscribe(x => this.Master = new Master(x.Data));

			requireInfoSource
				.SkipUntil(firstTime)
				.Subscribe(x => this.SetRequireInfo(x.Data));

			this.Logger = new Logger(proxy);
			this.ResourceLogger = new ResourceLogManager(this);
		}

		private void SetRequireInfo(kcsapi_require_info data)
		{
			if (this.Homeport == null) return;

			if (data.api_basic != null)
				this.Homeport.UpdateAdmiral(data.api_basic);

			if (data.api_slot_item != null)
				this.Homeport.Itemyard.Update(data.api_slot_item);

			if (data.api_kdock != null)
				this.Homeport.Dockyard.Update(data.api_kdock);

			if (data.api_useitem != null)
				this.Homeport.Itemyard.Update(data.api_useitem);
		}
	}
}
