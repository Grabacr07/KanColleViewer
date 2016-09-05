using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleViewer.Models.QuestTracker.Extensions;

namespace Grabacr07.KanColleViewer.Models.QuestTracker.Tracker
{
    /// <summary>
    /// 아호작전
    /// </summary>
    internal class Bw1 : ITracker
    {
        private int progress_boss;
        private int progress_boss_win;
        private int progress_combat;
        private int progress_combat_s;

        public event EventHandler ProcessChanged;

        int ITracker.Id => 214;
        public QuestType Type => QuestType.Weekly;
        public bool IsTracking { get; set; }

        private System.EventArgs emptyEventArgs = new System.EventArgs();

        public void RegisterEvent(TrackManager manager)
        {
            var BossNameList = new string[]
            {
                "敵主力艦隊",             // 1-1, 1-3, 2-1, 2-5
                "敵主力部隊",             // 1-2
                "敵機動部隊",             // 1-4
                "敵通商破壊主力艦隊",     // 1-5

                "敵通商破壊艦隊",         // 2-2
                "敵主力打撃群",           // 2-3
                "敵侵攻中核艦隊",         // 2-4

                "敵北方侵攻艦隊",         // 3-1
                "敵キス島包囲艦隊",       // 3-2
                "深海棲艦泊地艦隊",       // 3-3
                "深海棲艦北方艦隊中枢",   // 3-4
                "北方増援部隊主力",       // 3-5

                "東方派遣艦隊",           // 4-1
                "東方主力艦隊",           // 4-2, 4-3
                "敵東方中枢艦隊",         // 4-4
                "リランカ島港湾守備隊",   // 4-5

                "敵前線司令艦隊",         // 5-1
                "敵機動部隊本隊",         // 5-2
                "敵サーモン方面主力艦隊", // 5-3
                "敵補給部隊本体",         // 5-4
                "敵任務部隊本隊",         // 5-5

                "敵回航中空母",           // 6-1
                "敵攻略部隊本体",         // 6-2
                "留守泊地旗艦艦隊",       // 6-3
                "離島守備隊"              // 6-4
            };

            manager.BattleResultEvent += (sender, args) =>
            {
                if (!IsTracking) return;

                // 출격
                if (args.IsFirstCombat)
                    progress_combat = progress_combat.Add(1).Max(36);

                // S 승리
                if (args.Rank == "S")
                    progress_combat_s = progress_combat_s.Add(1).Max(6);

                // 보스전
                if (BossNameList.Contains(args.EnemyName))
                {
                    progress_boss = progress_boss.Add(1).Max(24);

                    // 보스전 승리
                    if ("SAB".Contains(args.Rank))
                        progress_boss_win = progress_boss_win.Add(1).Max(12);
                }

                ProcessChanged?.Invoke(this, emptyEventArgs);
            };
        }

        public void ResetQuest()
        {
            progress_combat = 0;
            progress_combat_s = 0;
            progress_boss = 0;
            progress_boss_win = 0;
            ProcessChanged?.Invoke(this, emptyEventArgs);
        }

        public double GetProgress()
        {
            return (double)(progress_combat + progress_combat_s + progress_boss + progress_boss_win) / (36 + 6 + 24 + 12) * 100;
        }

        public string GetProgressText()
        {
            if (progress_combat >= 36 && progress_combat_s >= 6 && progress_boss >= 24 && progress_boss_win >= 12)
                return "완료";

            return
                $"출격 {progress_combat}/36, S승리 {progress_combat_s}/6," +
                $" 보스전 {progress_boss}/24, 보스전 승리 {progress_boss_win}/12";
        }

        public string SerializeData()
        {
            return $"{progress_combat},{progress_combat_s},{progress_boss},{progress_boss_win}";
        }

        public void DeserializeData(string data)
        {
            try
            {
                var part = data.Split(',');
                progress_combat = int.Parse(part[0]);
                progress_combat_s = int.Parse(part[1]);
                progress_boss = int.Parse(part[2]);
                progress_boss_win = int.Parse(part[3]);
            }
            catch
            {
                progress_combat = progress_combat_s = progress_boss = progress_boss_win = 0;
            }
        }
    }
}
