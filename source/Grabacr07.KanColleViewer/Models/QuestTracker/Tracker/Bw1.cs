using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;

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
        public bool IsTracking { get; set; }

        private System.EventArgs emptyEventArgs = new System.EventArgs();

        public void RegisterEvent(TrackManager manager)
        {
            List<string> BossNameList = new List<string>
            {
                "敵主力艦隊", //1-1
                "敵主力部隊",
                // 1-3: 敵主力艦隊
                "敵機動部隊",
                "敵通商破壊主力艦隊",
                // 2-1: 敵主力艦隊
                "敵通商破壊艦隊", //2-2
                "敵主力打撃群",
                "敵侵攻中核艦隊",
                // 2-5: 敵主力艦隊
                "敵北方侵攻艦隊", //3-1
                "敵キス島包囲艦隊",
                "深海棲艦泊地艦隊",
                "深海棲艦北方艦隊中枢",
                "北方増援部隊主力",
                "東方派遣艦隊", //4-1
                "東方主力艦隊",
                // 4-3: 東方主力艦隊
                "敵東方中枢艦隊",
                "リランカ島港湾守備隊",
                "敵前線司令艦隊", //5-1
                "敵機動部隊本隊",
                "敵サーモン方面主力艦隊",
                "敵補給部隊本体",
                "敵任務部隊本隊",
                "敵回航中空母", //6-1
                "敵攻略部隊本体",
                "留守泊地旗艦艦隊"
            };

            manager.BattleResultEvent += (sender, args) =>
            {
                if (!IsTracking) return;

                if (args.IsFirstCombat)
                {
                    if (progress_combat < 36)
                        progress_combat++;
                }

                // S win?
                if (args.Rank == "S")
                    if (progress_combat_s < 6)
                        progress_combat_s++;

                // is boss
                if (BossNameList.Contains(args.EnemyName))
                {
                    if (progress_boss < 24)
                        progress_boss++;

                    // boss & win?
                    if (args.Rank == "S" || args.Rank == "A" || args.Rank == "B")
                        if (progress_boss_win < 12)
                            progress_boss_win++;
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
                $" 보스전 돌입 {progress_boss}/24, 보스전 승리 {progress_boss_win}/12";
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
