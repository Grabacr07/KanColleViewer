using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using EventMapHpViewer.Models.Raw;
using Grabacr07.KanColleWrapper;
using Livet;

namespace EventMapHpViewer.Models
{
    public class MapInfoProxy : NotificationObject
    {

        #region Maps変更通知プロパティ
        private Maps _Maps;

        public Maps Maps
        {
            get
            { return this._Maps; }
            set
            { 
                if (this._Maps == value)
                    return;
                this._Maps = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        public MapInfoProxy()
        {
            this.Maps = new Maps();

            var proxy = KanColleClient.Current.Proxy;

            proxy.api_start2
                .TryParse<kcsapi_start2>()
                .Subscribe(x =>
                {
                    Maps.MapAreas = new MasterTable<MapArea>(x.Data.api_mst_maparea.Select(m => new MapArea(m)));
                    Maps.MapInfos = new MasterTable<MapInfo>(x.Data.api_mst_mapinfo.Select(m => new MapInfo(m, Maps.MapAreas)));
                });

            proxy.ApiSessionSource.Where(s => s.PathAndQuery.StartsWith("/kcsapi/api_get_member/mapinfo"))
                .TryParse<member_mapinfo[]>()
                .Subscribe(m =>
                {
                    Debug.WriteLine("MapInfoProxy - member_mapinfo");
                    this.Maps.MapList = m.Data.Select(x => new MapData
                    {
                        IsCleared = x.api_cleared,
                        DefeatCount = x.api_defeat_count,
                        IsExBoss = x.api_exboss_flag,
                        Id = x.api_id,
                        Eventmap = x.api_eventmap != null
                            ? new Eventmap
                            {
                                MaxMapHp = x.api_eventmap.api_max_maphp,
                                NowMapHp = x.api_eventmap.api_now_maphp,
                                SelectedRank = x.api_eventmap.api_selected_rank,
                                State = x.api_eventmap.api_state,
                            }
                            : null,
                    }).ToArray();
                    this.RaisePropertyChanged(() => this.Maps);
                });

            //// 難易度選択→即出撃時にゲージを更新するなら…
            //proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_req_map/start")
            //    .TryParse<map_start_next>()
            //    .Subscribe(m =>
            //    {
            //        if (m.Data.api_eventmap == null) return;
            //        var eventMap = this.Maps.MapList.LastOrDefault(x => x.Eventmap != null);
            //        if (eventMap == null) return;
            //        if (eventMap.Eventmap.MaxMapHp != 9999) return;
            //        Debug.WriteLine("MapInfoProxy - map_start_next");
            //        eventMap.Eventmap.NowMapHp = m.Data.api_eventmap.NowMapHp;    //常にMAXなので普段は読んではいけない
            //        eventMap.Eventmap.MaxMapHp = m.Data.api_eventmap.MaxMapHp;
            //        this.RaisePropertyChanged(() => this.Maps);
            //    });
        }
    }
}
