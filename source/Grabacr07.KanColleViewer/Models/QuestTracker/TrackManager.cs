using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Linq;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Livet;

using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;

using Grabacr07.KanColleViewer.Models.QuestTracker.Extensions;
using Grabacr07.KanColleViewer.Models.QuestTracker.Model;
using Grabacr07.KanColleViewer.Models.QuestTracker.EventArgs;

namespace Grabacr07.KanColleViewer.Models.QuestTracker
{
    class TrackManager
    {
        private readonly Dictionary<int, DateTime> trackingTime = new Dictionary<int, DateTime>();
        private ObservableCollection<ITracker> trackingAvailable = new ObservableCollection<ITracker>();

        public List<ITracker> TrackingQuests
        {
            get
            {
                return trackingAvailable.Where(x => x.IsTracking).ToList();
            }
        }
        public List<ITracker> AllQuests
        {
            get
            {
                return trackingAvailable.ToList();
            }
        }

        internal static DateTime TokyoDateTime => TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Tokyo Standard Time");

        public event EventHandler<BattleResultEventArgs> BattleResultEvent;
        public event EventHandler<MissionResultEventArgs> MissionResultEvent;
        public event EventHandler<PracticeResultEventArgs> PractiveResultEvent;
        public event EventHandler RepairStartEvent;
        public event EventHandler ChargeEvent;
        public event EventHandler CreateItemEvent;
        public event EventHandler CreateShipEvent;
        public event EventHandler DestoryShipEvent;
        public event EventHandler DestoryItemEvent;
        public event EventHandler PowerUpEvent;
        public event EventHandler ReModelEvent;

        public readonly System.EventArgs EmptyEventArg = new System.EventArgs();
        public event EventHandler QuestsEventChanged;

        private void CatchHelper(Action action)
        {
            try { action(); }
            catch { }
        }

        public TrackManager()
        {
            var proxy = KanColleClient.Current.Proxy;
            var MapInfo = new TrackerMapInfo();
            var battleTracker = new BattleTracker();

            // 연습전 종료
            proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_practice/battle_result")
                .TryParse<kcsapi_practice_result>().Subscribe(x =>
                    CatchHelper(() => PractiveResultEvent?.Invoke(this, new PracticeResultEventArgs(x.Data)))
                );

            // 근대화 개수
            proxy.api_req_kaisou_powerup.TryParse<kcsapi_powerup>().Subscribe(x => CatchHelper(() => PowerUpEvent?.Invoke(this, this.EmptyEventArg)));

            // 개수공창 개수
            proxy.api_req_kousyou_remodel_slot.TryParse<kcsapi_remodel_slot>().Subscribe(x => CatchHelper(() => ReModelEvent?.Invoke(this, this.EmptyEventArg)));

            // 폐기
            proxy.api_req_kousyou_destroyitem2.TryParse<kcsapi_destroyitem2>().Subscribe(x => CatchHelper(() => DestoryItemEvent?.Invoke(this, this.EmptyEventArg)));

            // 해체
            proxy.api_req_kousyou_destroyship.TryParse<kcsapi_destroyship>().Subscribe(x => CatchHelper(() => DestoryShipEvent?.Invoke(this, this.EmptyEventArg)));

            // 건조
            proxy.api_req_kousyou_createship.TryParse<kcsapi_createship>().Subscribe(x => CatchHelper(() => CreateShipEvent?.Invoke(this, this.EmptyEventArg)));

            // 개발
            proxy.api_req_kousyou_createitem.TryParse<kcsapi_createitem>().Subscribe(x => CatchHelper(() => CreateItemEvent?.Invoke(this, this.EmptyEventArg)));

            // 보급
            proxy.api_req_hokyu_charge.TryParse<kcsapi_charge>().Subscribe(x => CatchHelper(() => ChargeEvent?.Invoke(this, this.EmptyEventArg)));

            // 입거
            proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_nyukyo/start")
                .Subscribe(x => CatchHelper(() => RepairStartEvent?.Invoke(this, this.EmptyEventArg)));

            // 원정
            proxy.api_req_mission_result.TryParse<kcsapi_mission_result>().Subscribe(x => CatchHelper(() => MissionResultEvent?.Invoke(this, new MissionResultEventArgs(x.Data))));

            // 출격 (시작)
            proxy.api_req_map_start.TryParse<kcsapi_map_start>().Subscribe(x => CatchHelper(() => MapInfo.Reset(x.Data.api_maparea_id)));

            // 통상 - 주간전
            proxy.api_req_sortie_battle.TryParse<kcsapi_sortie_battle>().Subscribe(x => CatchHelper(() => battleTracker.BattleProcess(x.Data)));

            // 통상 - 야전
            proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_battle_midnight/battle")
                .TryParse<kcsapi_battle_midnight_battle>().Subscribe(x => CatchHelper(() => battleTracker.BattleProcess(x.Data)));

            // 통상 - 개막야전
            proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_battle_midnight/sp_midnight")
                .TryParse<kcsapi_battle_midnight_sp_midnight>().Subscribe(x => CatchHelper(() => battleTracker.BattleProcess(x.Data)));

            // 전투 종료 (연합함대 포함)
            proxy.api_req_sortie_battleresult.TryParse<kcsapi_battleresult>()
                .Subscribe(x => CatchHelper(() => BattleResultEvent?.Invoke(this, new BattleResultEventArgs(MapInfo.AfterCombat(), battleTracker.enemyShips, x.Data))));
            proxy.api_req_combined_battle_battleresult.TryParse<kcsapi_combined_battle_battleresult>()
                .Subscribe(x => CatchHelper(() => BattleResultEvent?.Invoke(this, new BattleResultEventArgs(MapInfo.AfterCombat(), battleTracker.enemyShips, x.Data))));


            // Register all trackers
            trackingAvailable = new ObservableCollection<ITracker>(trackingAvailable.OrderBy(x => x.Id));
            trackingAvailable.CollectionChanged += (sender, e) =>
            {
                if (e.Action != NotifyCollectionChangedAction.Add)
                    return;

                foreach (ITracker tracker in e.NewItems)
                {
                    tracker.RegisterEvent(this);
                    tracker.ResetQuest();
                    tracker.ProcessChanged += ((x, y) =>
                    {
                        QuestsEventChanged?.Invoke(this, EmptyEventArg);

                        WriteToStorage();
                    });
                }
            };

            Assembly.GetExecutingAssembly().GetTypes()
                    .ToList().Where(x => x.Namespace == "Grabacr07.KanColleViewer.Models.QuestTracker.Tracker" && typeof(ITracker).IsAssignableFrom(x))
                    .ToList().ForEach(x => trackingAvailable.Add((ITracker)Activator.CreateInstance(x)));

            ReadFromStorage();
            WriteToStorage();

            proxy.api_get_member_questlist.Subscribe(x => new System.Threading.Thread(ProcessQuests).Start());
            QuestsEventChanged?.Invoke(this, EmptyEventArg);
        }

        private void ProcessQuests()
        {
            var quests = KanColleClient.Current.Homeport.Quests;
            if (quests.All == null || quests.All.Count == 0) return;

            foreach (var quest in quests.All)
            {
                var tracker = trackingAvailable.Where(t => t.Id == quest.Id);
                if (!tracker.Any()) continue; // 추적할 수 없는 임무

                try
                {
                    // 만료된 경우 (임무가 갱신되었다던가)
                    if (trackingTime.ContainsKey(quest.Id) && !IsTrackingAvailable(quest.Type, trackingTime[quest.Id]))
                    {
                        // 임무 초기화
                        if (trackingTime.ContainsKey(quest.Id)) trackingTime.Remove(quest.Id); // 추적중이었으면 추적 시작시간 제거
                        tracker.First().ResetQuest();
                    }

                    switch (quest.State)
                    {
                        case QuestState.None:
                            tracker.First().IsTracking = false;
                            break;

                        case QuestState.TakeOn:
                            tracker.First().IsTracking = true; // quest taking

                            // 임무 추적 시작시간 등록
                            if (!trackingTime.ContainsKey(quest.Id))
                                trackingTime.Add(quest.Id, TrackManager.TokyoDateTime);
                            break;

                        case QuestState.Accomplished:
                            tracker.First().IsTracking = false;
                            break;
                    }
                } catch { }
            }

            QuestsEventChanged?.Invoke(this, EmptyEventArg);
            WriteToStorage();
        }
        private bool IsTrackingAvailable(QuestType type, DateTime time)
        {
            // 임무는 오전 5시, UTC+4에 갱신됨
            // 일광절약 없는 아랍 에미레이트 연합 표준시 (ar-AE) => UTC+4

            if (time == DateTime.MinValue)
                return false;

            var no = TrackManager.TokyoDateTime.AddHours(-5);
            time = time.AddHours(-5);

            switch (type)
            {
                case QuestType.OneTime:
                case QuestType.Other:
                    return true;

                case QuestType.Daily:
                    return time.Date == no.Date;

                case QuestType.Weekly:
                    var cal = CultureInfo.CreateSpecificCulture("ar-AE").Calendar;
                    var w_time = cal.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                    var w_now = cal.GetWeekOfYear(no, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

                    return w_time == w_now && time.Year == no.Year;

                case QuestType.Monthly:
                    return time.Month == no.Month && time.Year == no.Year;

                default:
                    return false;
            }
        }

        private void WriteToStorage()
        {
            var baseDir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            var list = new List<StorageData>();

            foreach (var tracker in trackingAvailable)
            {
                var item = new StorageData();

                DateTime dateTime = TrackManager.TokyoDateTime;
                trackingTime.TryGetValue(tracker.Id, out dateTime);

                try
                {
                    if (tracker.GetProgress() == 0 && dateTime == DateTime.MinValue) continue;

                    item.Id = tracker.Id;
                    item.TrackTime = dateTime;
                    item.Type = tracker.Type;
                    item.Serialized = tracker.SerializeData();
                    list.Add(item);
                }
                catch { }
            }

            string path = Path.Combine(baseDir, "TrackingQuest.csv");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                foreach (var item in list)
                {
                    try { CSV.Write(fs, item.Id, item.TrackTime, item.Type, item.Serialized); }
                    catch { }
                }

                fs.Flush();
            }
        }
        private void ReadFromStorage()
        {
            var baseDir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string path = Path.Combine(baseDir, "TrackingQuest.csv");
            if (!File.Exists(path)) return;

            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                while (fs.Position < fs.Length)
                {
                    string[] data = CSV.Read(fs);

                    try
                    {
                        int Id;
                        DateTime trackTime;
                        string QuestTypeText;
                        QuestType QuestType;
                        string Serialized;

                        if (!int.TryParse(data[0], out Id)) continue;
                        DateTime.TryParse(data[1], out trackTime);
                        QuestTypeText = data[2];
                        Enum.TryParse<QuestType>(QuestTypeText, out QuestType);
                        Serialized = data[3];

                        if (!trackingAvailable.Any(x => x.Id == Id)) continue;
                        if (IsTrackingAvailable(QuestType, trackTime))
                        {
                            var tracker = trackingAvailable.Where(x => x.Id == Id).First();

                            trackingTime.Add(Id, trackTime);
                            tracker.IsTracking = true;
                            tracker.DeserializeData(Serialized);
                        }
                    }
                    catch { }
                }
            }
        }
    }
}
