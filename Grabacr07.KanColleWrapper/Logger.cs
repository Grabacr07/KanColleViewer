using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Codeplex.Data;
using Fiddler;
using Grabacr07.KanColleWrapper.Internal;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using Livet;

namespace Grabacr07.KanColleWrapper
{
    public class Logger : NotificationObject
    {
        public bool EnableLogging { get; set; }

        private bool waitingForShip = false;
        private int dockid;
        private int[] shipmats;

        internal Logger(KanColleProxy proxy)
        {
            shipmats = new int[5];
            proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_req_kousyou/createitem")
                .Select(x => { SvData<kcsapi_createitem> result; return SvData.TryParse(x, out result) ? result : null; })
                .Where(x => x != null && x.IsSuccess)
                .Subscribe(x => CreateItem(x.Data, x.RequestBody));

            proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_req_kousyou/createship")
                .Select(x => { SvData<kcsapi_createship> result; return SvData.TryParse(x, out result) ? result : null; })
                .Where(x => x != null && x.IsSuccess)
                .Subscribe(x => CreateShip(x.Data, x.RequestBody));

            proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_get_member/kdock")
                .TryParse<kcsapi_kdock[]>()
                .Subscribe(this.KDock);

            proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_req_sortie/battleresult")
                .TryParse<kcsapi_battleresult>()
                .Subscribe(this.BattleResult);
        }

        private void CreateItem(kcsapi_createitem item, NameValueCollection req)
        {
            Log("Item","{0},{1},{2},{3},{4},{5}", item.api_create_flag == 1 ? KanColleClient.Current.Master.SlotItems[item.api_slotitem_id].Name : "실패",
                KanColleClient.Current.Homeport.Fleets[1].Ships[0].Info.ShipType.Name,
                req["api_item1"], req["api_item2"], req["api_item3"], req["api_item4"]);
        }

        private void CreateShip(kcsapi_createship ship, NameValueCollection req)
        {
            waitingForShip = true;
            dockid = Int32.Parse(req["api_kdock_id"]);
            shipmats[0] = Int32.Parse(req["api_item1"]);
            shipmats[1] = Int32.Parse(req["api_item2"]);
            shipmats[2] = Int32.Parse(req["api_item3"]);
            shipmats[3] = Int32.Parse(req["api_item4"]);
            shipmats[4] = Int32.Parse(req["api_item5"]);
        }

        private void KDock(kcsapi_kdock[] docks)
        {
            foreach (var dock in docks)
            {
                if (waitingForShip && dock.api_id == dockid)
                {
                    Log("KDock","{0},{1},{2},{3},{4},{5}", KanColleClient.Current.Master.Ships[dock.api_created_ship_id].Name, shipmats[0], shipmats[1], shipmats[2], shipmats[3], shipmats[4]);
                    waitingForShip = false;
                }
            }
        }

        private void BattleResult(kcsapi_battleresult br)
        {
            Log("Battle","{0},{1},{2},{3}", br.api_get_ship != null ? br.api_get_ship.api_ship_name : "없음", br.api_quest_name, br.api_enemy_info.api_deck_name, br.api_win_rank);
        }
        
        private void Log(string Type,string format, params object[] args)
        {
            if (EnableLogging)
            {
                if (Type == "Item")
                {
                    var location = System.Reflection.Assembly.GetEntryAssembly().Location;
                    string Main_folder = Path.GetDirectoryName(location);
                    if (System.IO.File.Exists(Main_folder + "\\Create_Item_log.csv") == true)
                    {
                        using (StreamWriter w = File.AppendText("Create_Item_log.csv"))
                        {
                            w.WriteLine(format, args);
                        }
                    }
                    else
                    {
                        using (StreamWriter w = File.AppendText("Create_Item_log.csv"))
                        {
                            w.WriteLine("장비명,비서함,연,탄,강,보키", args);
                            w.WriteLine(format, args);
                        }
                    }
                }
                else if (Type == "KDock")
                {
                    var location = System.Reflection.Assembly.GetEntryAssembly().Location;
                    string Main_folder = Path.GetDirectoryName(location);
                    if (System.IO.File.Exists(Main_folder + "\\Create_Ship_log.csv") == true)
                    {
                        using (StreamWriter w = File.AppendText("Create_Ship_log.csv"))
                        {
                            w.WriteLine(format, args);
                        }
                    }
                    else
                    {
                        using (StreamWriter w = File.AppendText("Create_Ship_log.csv"))
                        {
                            w.WriteLine("결과,연,탄,강,보키,개발자재", args);
                            w.WriteLine(format, args);
                        }
                    }
                }
                else{
                    var location = System.Reflection.Assembly.GetEntryAssembly().Location;
                    string Main_folder = Path.GetDirectoryName(location);
                    if (System.IO.File.Exists(Main_folder + "\\Battle_log.csv") == true)
                    {
                        using (StreamWriter w = File.AppendText("Battle_log.csv"))
                        {
                            w.WriteLine(format, args);
                        }
                    }
                    else
                    {
                        using (StreamWriter w = File.AppendText("Battle_log.csv"))
                        {
                            w.WriteLine("결과,해역,적 함대,랭크", args);
                            w.WriteLine(format, args);
                        }
                    }
                }
                
            }
        }
    }
}