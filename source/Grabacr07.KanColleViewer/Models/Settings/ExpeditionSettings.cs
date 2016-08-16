using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MetroTrilithon.Serialization;

namespace Grabacr07.KanColleViewer.Models.Settings
{
    public static class ExpeditionSettings
    {
        /// <summary>
        /// 원정 탭의 선택된 원정 번호를 저장합니다.
        /// </summary>
        public static SerializableProperty<int> ExpeditionId1 { get; }
            = new SerializableProperty<int>(GetKey(), Providers.Roaming, 1);

        /// <summary>
        /// 원정 탭의 선택된 원정 번호를 저장합니다.
        /// </summary>
        public static SerializableProperty<int> ExpeditionId2 { get; }
            = new SerializableProperty<int>(GetKey(), Providers.Roaming, 1);

        /// <summary>
        /// 원정 탭의 선택된 원정 번호를 저장합니다.
        /// </summary>
        public static SerializableProperty<int> ExpeditionId3 { get; }
            = new SerializableProperty<int>(GetKey(), Providers.Roaming, 1);

        private static string GetKey([CallerMemberName] string propertyName = "")
        {
            return nameof(ExpeditionSettings) + "." + propertyName;
        }
    }
}
