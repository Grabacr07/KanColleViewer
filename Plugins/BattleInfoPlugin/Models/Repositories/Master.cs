using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;
using BattleInfoPlugin.Properties;
using System.IO;
using System.Runtime.Serialization.Json;

namespace BattleInfoPlugin.Models.Repositories
{
    [DataContract]
    public class Master
    {
        private static readonly DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Master));

        private static Master _Current;

        public static Master Current { get { return _Current = _Current ?? new Master(); } }

        /// <summary>
        /// すべての海域の定義を取得します。
        /// </summary>
        [DataMember]
        public ConcurrentDictionary<int, MapArea> MapAreas { get; private set; }

        /// <summary>
        /// すべてのマップの定義を取得します。
        /// </summary>
        [DataMember]
        public ConcurrentDictionary<int, MapInfo> MapInfos { get; private set; }

        /// <summary>
        /// すべてのセルの定義を取得します。
        /// </summary>
        [DataMember]
        public ConcurrentDictionary<int, MapCell> MapCells { get; private set; }

        public Master()
        {
			Settings.Default.FirstIsCritical = false;
			Settings.Default.SecondIsCritical = false;
            this.MapAreas = new ConcurrentDictionary<int, MapArea>();
            this.MapInfos = new ConcurrentDictionary<int, MapInfo>();
            this.MapCells = new ConcurrentDictionary<int, MapCell>();
            this.Reload();
        }

        public void Update(kcsapi_start2 start2)
        {
            var areas = start2.api_mst_maparea.Select(x => new MapArea(x)).ToDictionary(x => x.Id, x => x);
            var infos = start2.api_mst_mapinfo.Select(x => new MapInfo(x)).ToDictionary(x => x.Id, x => x);
            var cells = start2.api_mst_mapcell.Select(x => new MapCell(x)).ToDictionary(x => x.Id, x => x);

            foreach (var key in areas.Keys) this.MapAreas.AddOrUpdate(key, areas[key], (k, v) => areas[k]);
            foreach (var key in infos.Keys) this.MapInfos.AddOrUpdate(key, infos[key], (k, v) => infos[k]);
            foreach (var key in cells.Keys) this.MapCells.AddOrUpdate(key, cells[key], (k, v) => cells[k]);

            this.Save();
        }

        //private static void UpdateMasterTable<T>(IDictionary<int, T> target, Dictionary<int, T> source)
        //{
        //    foreach (var sourceKey in source.Keys)
        //    {
        //        if (target.ContainsKey(sourceKey))
        //            target[sourceKey] = source[sourceKey];
        //        else
        //            target.Add(sourceKey, source[sourceKey]);
        //    }
        //}

        private void Reload()
        {
            //deserialize
            var path = Environment.CurrentDirectory + "\\" + Settings.Default.MasterDataFilePath;
            if (!File.Exists(path)) return;

            using (var stream = Stream.Synchronized(new FileStream(path, FileMode.OpenOrCreate)))
            {
                var obj = serializer.ReadObject(stream) as Master;
                if (obj == null) return;
                this.MapAreas = obj.MapAreas;
                this.MapInfos = obj.MapInfos;
                this.MapCells = obj.MapCells;
            }
        }

        private void Save()
        {
            //serialize
            var path = Environment.CurrentDirectory + "\\" + Settings.Default.MasterDataFilePath;
            using (var stream = Stream.Synchronized(new FileStream(path, FileMode.OpenOrCreate)))
            {
                serializer.WriteObject(stream, this);
            }
        }
    }
}
