namespace BattleInfoPlugin.Models.Raw
{
    /// <summary>
    /// 連合艦隊-航空戦
    /// </summary>
    public class combined_battle_airbattle
    {
        public int api_deck_id { get; set; }
        public int[] api_ship_ke { get; set; }
        public int[] api_ship_lv { get; set; }
		public decimal[] api_nowhps { get; set; }
		public decimal[] api_maxhps { get; set; }
		public decimal[] api_nowhps_combined { get; set; }
		public decimal[] api_maxhps_combined { get; set; }
        public int api_midnight_flag { get; set; }
        public int[][] api_eSlot { get; set; }
        public int[][] api_eKyouka { get; set; }
        public int[][] api_fParam { get; set; }
        public int[][] api_eParam { get; set; }
        public int[][] api_fParam_combined { get; set; }
        public int[] api_search { get; set; }
        public int[] api_formation { get; set; }
        public int[] api_stage_flag { get; set; }
        public Api_Kouku api_kouku { get; set; }
        public int api_support_flag { get; set; }
        public Api_Support_Info api_support_info { get; set; }
        public int[] api_stage_flag2 { get; set; }
        public Api_Kouku api_kouku2 { get; set; }
    }
}
