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
                if (value == null)
                {
                    //UpstreamProxySettings == null は SystemProxy使用とみなす
                    HttpProxy.IsEnableUpstreamProxy = false;
                    HttpProxy.UpstreamProxyHost = null;
                    return;
                }
                HttpProxy.IsEnableUpstreamProxy = value.Type == ProxyType.SpecificProxy;
                //Host指定がない場合、HTTPはDirectAccessとなる
                HttpProxy.UpstreamProxyHost = string.IsNullOrWhiteSpace(value.Host) ? null : value.Host;
                HttpProxy.UpstreamProxyPort = value.Port;
            }
        }

        #endregion

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
    }
}
