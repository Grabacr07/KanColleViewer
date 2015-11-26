using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Nekoxy;
using StatefulModel;

namespace Grabacr07.KanColleWrapper
{
    public partial class KanColleProxy
    {
        private readonly IConnectableObservable<Session> connectableSessionSource;
        private readonly IConnectableObservable<Session> apiSource;
        private readonly CompositeDisposable compositeDisposable;

        public IObservable<Session> SessionSource => this.connectableSessionSource.AsObservable();

        public IObservable<Session> ApiSessionSource => this.apiSource.AsObservable();

        #region UpstreamProxySettingsプロパティ

        private IProxySettings _UpstreamProxySettings;

		public IProxySettings UpstreamProxySettings
		{
			get { return this._UpstreamProxySettings; }
			set
			{
				this._UpstreamProxySettings = value;
				this.ApplyProxySettings();
			}
		}

        #endregion

		public int ListeningPort { get; private set; } = 37564;

        public KanColleProxy()
        {
            this.compositeDisposable = new CompositeDisposable();
            this.connectableSessionSource = Observable
                .FromEvent<Action<Session>, Session>(
                    action => action,
                    h => HttpProxy.AfterSessionComplete += h,
                    h => HttpProxy.AfterSessionComplete -= h)
                .Publish();

            this.apiSource = this.connectableSessionSource
                .Where(s => s.Request.PathAndQuery.StartsWith("/kcsapi"))
                .Where(s => s.Response.MimeType.Equals("text/plain"))
            #region .Do(debug)
#if DEBUG
.Do(session =>
                {
                    Debug.WriteLine("==================================================");
                    Debug.WriteLine("Nekoxy session: ");
                    Debug.WriteLine(session);
                    Debug.WriteLine("");
                })
#endif
            #endregion
            #region .Do(send-database)
.Do(s =>
            {
                // 艦これ統計データベースへ送信が有効で、かつアクセスキーが入力されていた場合は送信
                if (UpstreamProxySettings.SendDb && !string.IsNullOrEmpty(UpstreamProxySettings.DbAccessKey))
                {
                    string[] urls =
                    {
                        "api_port/port",
                        "api_get_member/ship2",
                        "api_get_member/ship3",
                        "api_get_member/slot_item",
                        "api_get_member/kdock",
                        "api_get_member/mapinfo",
                        "api_req_hensei/change",
                        "api_req_kousyou/createship",
                        "api_req_kousyou/getship",
                        "api_req_kousyou/createitem",
                        "api_req_map/start",
                        "api_req_map/next",
                        "api_req_map/select_eventmap_rank",
                        "api_req_sortie/battle",
                        "api_req_battle_midnight/battle",
                        "api_req_battle_midnight/sp_midnight",
                        "api_req_sortie/night_to_day",
                        "api_req_sortie/battleresult",
                        "api_req_combined_battle/battle",
                        "api_req_combined_battle/airbattle",
                        "api_req_combined_battle/midnight_battle",
                        "api_req_combined_battle/battleresult",
                        "api_req_sortie/airbattle",
                        "api_req_combined_battle/battle_water",
                        "api_req_combined_battle/sp_midnight",
                    };
                    foreach (var url in urls)
                    {
                        if (s.Request.PathAndQuery.IndexOf(url) > 0)
                        {
                            using (System.Net.WebClient wc = new System.Net.WebClient())
                            {
                                System.Collections.Specialized.NameValueCollection post = new System.Collections.Specialized.NameValueCollection();
                                post.Add("token", UpstreamProxySettings.DbAccessKey);
                                post.Add("agent", "LZXNXVGPejgSnEXLH2ur");  // このクライアントのエージェントキー
                                post.Add("url", s.Request.PathAndQuery);
                                string requestBody = System.Text.RegularExpressions.Regex.Replace(s.Request.BodyAsString, @"&api(_|%5F)token=[0-9a-f]+|api(_|%5F)token=[0-9a-f]+&?", "");	// api_tokenを送信しないように削除
                                post.Add("requestbody", requestBody);
                                post.Add("responsebody", s.Response.BodyAsString);

                                wc.UploadValuesAsync(new Uri("http://api.kancolle-db.net/2/"), post);
#if DEBUG
                                Debug.WriteLine("==================================================");
                                Debug.WriteLine("Send to KanColle statistics database");
                                Debug.WriteLine(s);
                                Debug.WriteLine("==================================================");
#endif
                            }
                            break;
                        }
                    }
                }
            })
            #endregion
            .Publish();
        }

        public void Startup(int proxy = 37564)
        {
            //UpstreamProxySettings == null は SystemProxy使用とみなす
            var isSetIEProxySettings = this.UpstreamProxySettings == null || this.UpstreamProxySettings.Type != ProxyType.DirectAccess;
            HttpProxy.Startup(proxy, false, isSetIEProxySettings);

            //プロキシを使用しない場合、HTTPだけNekoxyを通し、後は直アクセス
            if (!isSetIEProxySettings)
                WinInetUtil.SetProxyInProcess("http=localhost:" + proxy, "local");

            this.compositeDisposable.Add(this.connectableSessionSource.Connect());
            this.compositeDisposable.Add(this.apiSource.Connect());
        }

        public void Shutdown()
		{
			this.compositeDisposable.Dispose();
			HttpProxy.Shutdown();
		}

		/// <summary>
		/// プロキシ設定を反映
		/// </summary>
		private void ApplyProxySettings()
		{
			this.ApplyUpstreamProxySettings();
			this.ApplyDownstreamProxySettings();
		}

		/// <summary>
		/// 上流プロキシを設定
		/// </summary>
		private void ApplyUpstreamProxySettings()
		{
			switch (this.UpstreamProxySettings?.Type)
			{
				case ProxyType.DirectAccess:
					HttpProxy.UpstreamProxyConfig = new ProxyConfig(ProxyConfigType.DirectAccess);
					break;
				case ProxyType.SystemProxy:
					HttpProxy.UpstreamProxyConfig = new ProxyConfig(ProxyConfigType.SystemProxy);
					break;
				case ProxyType.SpecificProxy:
					HttpProxy.UpstreamProxyConfig = new ProxyConfig(ProxyConfigType.SpecificProxy, this.UpstreamProxySettings.HttpHost, this.UpstreamProxySettings.HttpPort);
					break;
				default:
					//UpstreamProxySettings == null は SystemProxy使用とみなす
					HttpProxy.UpstreamProxyConfig = new ProxyConfig(ProxyConfigType.SystemProxy);
					break;
			}
		}

		/// <summary>
		/// HttpProxy.UpstreamProxyConfig を元に、下流からの通信がNekoxyを通るよう設定
		/// </summary>
		private void ApplyDownstreamProxySettings()
		{
			var config = HttpProxy.UpstreamProxyConfig;
			switch (config.Type)
			{
				case ProxyConfigType.SystemProxy:
					WinInetUtil.SetProxyInProcessForNekoxy(this.ListeningPort);
					break;
				case ProxyConfigType.SpecificProxy:
					//指定プロキシの場合、HTTPだけNekoxyを通し、後は指定プロキシに流す
					if (!string.IsNullOrWhiteSpace(config.SpecificProxyHost))
					{
						if (this.UpstreamProxySettings.IsUseHttpProxyForAllProtocols)
						{
							// 「全てのプロトコルでこのプロキシ サーバーを使用する」
							WinInetUtil.SetProxyInProcess(
								$"http=127.0.0.1:{this.ListeningPort};"
								+ $"https={this.UpstreamProxySettings.HttpHost}:{this.UpstreamProxySettings.HttpPort};"
								+ $"ftp={this.UpstreamProxySettings.HttpHost}:{this.UpstreamProxySettings.HttpPort};"
								// IE に合わせて SOCKS は対象外
								//+ $"socks={this.UpstreamProxySettings.HttpHost}:{this.UpstreamProxySettings.HttpPort};"
								, "local");
						}
						else
						{
							WinInetUtil.SetProxyInProcess(
								$"http=127.0.0.1:{this.ListeningPort};"
								+ ((!string.IsNullOrWhiteSpace(this.UpstreamProxySettings.HttpsHost))
									? $"https={this.UpstreamProxySettings.HttpsHost}:{this.UpstreamProxySettings.HttpsPort};" : string.Empty)
								+ ((!string.IsNullOrWhiteSpace(this.UpstreamProxySettings.FtpHost))
									? $"ftp={this.UpstreamProxySettings.FtpHost}:{this.UpstreamProxySettings.FtpPort};" : string.Empty)
								+ ((!string.IsNullOrWhiteSpace(this.UpstreamProxySettings.SocksHost))
									? $"socks={this.UpstreamProxySettings.SocksHost}:{this.UpstreamProxySettings.SocksPort};" : string.Empty)
								, "local");
						}
					}
					else
					{
						//UpstreamProxyHost が空の場合は直アクセスとみなす
						WinInetUtil.SetProxyInProcess($"http=127.0.0.1:{this.ListeningPort}", "local");
					}
					break;
				case ProxyConfigType.DirectAccess:
					//プロキシを使用しない場合、HTTPだけNekoxyを通し、後は直アクセス
					WinInetUtil.SetProxyInProcess($"http=127.0.0.1:{this.ListeningPort}", "local");
					break;
			}
		}
	}
}
