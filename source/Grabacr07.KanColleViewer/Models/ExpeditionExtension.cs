using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.Models
{
    class ExpeditionExtension
    {
        public static Dictionary<int, string> ExpeditionList;

        static ExpeditionExtension()
        {
            var tempList = new Dictionary<int, string>();
            var Translations = KanColleClient.Current.Translations;
            int ListCount = Translations.GetExpeditionListCount();

            for (int i = 1; ; i++)
            {
                var TRName = Translations.GetExpeditionData("TR-Name", i);
                var FlagLv = Translations.GetExpeditionData("FlagLv", i);

                if (TRName != string.Empty && FlagLv != string.Empty)
                    tempList.Add(i, string.Format("[{0}] {1}", i, TRName));

                if (tempList.Count == ListCount) break;
            }
            ExpeditionList = tempList;
        }

        // 대성공 확률 계산
        public static float CheckGreateSuccessChance(int Mission, Ship[] fleet)
        {
            // 계산 기준: http://gall.dcinside.com/board/view/?id=kancolle&no=4514491

            int[] drums = new int[] {
                ExpeditionExtension.CheckDrums(fleet)[0],
                ExpeditionExtension.CheckRequireDrums(Mission)[0]
            };
            int kira = ExpeditionExtension.KiraShipsCount(fleet);

            float[] ChanceTable = new float[] { 0, 19, 38, 57, 76, 95, 98, 98.66f, 99.33f };

            switch (Mission)
            {
                // 드럼 A
                case 21: // 북방쥐 수송작전
                case 37: // 도쿄급행
                    if (drums[0] >= drums[1] + 1) return Math.Min(100.0f, (kira + 2) * 19.0f); // 드럼A, 100% 확정
                    return ChanceTable[kira]; // 드럼A, 드럼 충족 X
                case 38: // 도쿄급행(이)
                    if (drums[0] >= drums[1] + 2) return Math.Min(100.0f, (kira + 2) * 19.0f); // 드럼A, 100% 확정
                    return ChanceTable[kira]; // 드럼A, 드럼 충족 X

                // 드럼 B
                case 24: // 북방항로 해상호위
                    if (drums[0] >= 4) return ChanceTable[kira + 2]; // 드럼B, 확정 없음
                    return ChanceTable[kira]; // 드럼B, 드럼 충족 X
                case 40: // 수상기 전선 수송
                    if (drums[0] >= 4) return ChanceTable[kira + 2]; // 드럼B, 확정 없음
                    return ChanceTable[kira]; // 드럼B, 드럼 충족 X

                default: // 그 외 일반 원정들
                    if (kira < fleet.Length) return 0; // 반짝이 아닌 함선 포함
                    return ChanceTable[kira];
            }
        }

        public static bool CheckDrumPossible(int Mission, Ship[] fleet)
        {
            int[] nDrumData = ExpeditionExtension.CheckRequireDrums(Mission);
            int nTotalDrum = nDrumData[0];
            int nHasDrumShip = nDrumData[1];

            int[] rDrumData = ExpeditionExtension.CheckDrums(fleet);
            int rTotalDrum = rDrumData[0];
            int rHasDrumShip = rDrumData[1];

            if (rTotalDrum >= nTotalDrum && rHasDrumShip >= nHasDrumShip)
                return true;
            return false;
        }
        public static int[] CheckDrums(Ship[] fleet)
        {
            bool shipCheck = false;
            int rTotalDrum = 0, rHasDrumShip = 0;

            foreach (var ship in fleet)
            {
                shipCheck = false;
                foreach (var slot in ship.Slots)
                {
                    if (slot.Equipped && slot.Item.Info.CategoryId == 30)
                    {
                        rTotalDrum++;
                        if (!shipCheck)
                        {
                            rHasDrumShip++;
                            shipCheck = true;
                        }
                    } // if
                } // slot
            } // ship

            return new int[] { rTotalDrum, rHasDrumShip };
        }
        public static int[] CheckRequireDrums(int Mission)
        {
            var NeedDrumRaw = KanColleClient.Current.Translations.GetExpeditionData("DrumCount", Mission);
            var sNeedDrumRaw = NeedDrumRaw.Split(';');
            if (sNeedDrumRaw.Length == 1) return new int[] { 0, 0 };

            int nTotalDrum, nHasDrumShip;
            int.TryParse(sNeedDrumRaw[0], out nTotalDrum);
            int.TryParse(sNeedDrumRaw[1], out nHasDrumShip);

            return new int[] { nTotalDrum, nHasDrumShip };
        }
        public static int KiraShipsCount(Ship[] fleet)
        {
            return fleet.Where(x => x.ConditionType == ConditionType.Brilliant)
                        .Count();
        }
    }
}
