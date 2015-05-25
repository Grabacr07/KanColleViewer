namespace EventMapHpViewer.Models.Raw
{
    public class map_start_next
    {
        public int api_rashin_flg { get; set; }
        public int api_rashin_id { get; set; }
        public int api_maparea_id { get; set; }
        public int api_mapinfo_no { get; set; }
        public int api_no { get; set; }
        public int api_color_no { get; set; }
        public int api_event_id { get; set; }
        public int api_event_kind { get; set; }
        public int api_next { get; set; }
        public int api_bosscell_no { get; set; }
        public int api_bosscomp { get; set; }
        public Api_Eventmap api_eventmap { get; set; }
        public int api_comment_kind { get; set; }
        public int api_production_kind { get; set; }
        public Api_Enemy api_enemy { get; set; }
        public Api_Happening api_happening { get; set; }
    }

    public class Api_Enemy
    {
        public int api_enemy_id { get; set; }
        public int api_result { get; set; }
        public string api_result_str { get; set; }
    }

    public class Api_Happening
    {
        public int api_type { get; set; }
        public int api_count { get; set; }
        public int api_usemst { get; set; }
        public int api_mst_id { get; set; }
        public int api_icon_id { get; set; }
        public int api_dentan { get; set; }
    }
}
