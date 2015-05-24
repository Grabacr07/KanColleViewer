namespace BattleInfoPlugin.Models.Raw
{
    /// <summary>
    /// 演習-昼戦
    /// </summary>
    public class practice_battle
    {
        public int api_dock_id { get; set; }
        public int[] api_ship_ke { get; set; }
        public int[] api_ship_lv { get; set; }
		public decimal[] api_nowhps { get; set; }
		public decimal[] api_maxhps { get; set; }
        public int api_midnight_flag { get; set; }
        public int[][] api_eSlot { get; set; }
        public int[][] api_eKyouka { get; set; }
        public int[][] api_fParam { get; set; }
        public int[][] api_eParam { get; set; }
        public int[] api_search { get; set; }
        public int[] api_formation { get; set; }
        public int[] api_stage_flag { get; set; }
        public Api_Kouku api_kouku { get; set; }
        public int api_opening_flag { get; set; }
        public Raigeki api_opening_atack { get; set; }
        public int[] api_hourai_flag { get; set; }
        public Hougeki api_hougeki1 { get; set; }
        public Hougeki api_hougeki2 { get; set; }
        public Hougeki api_hougeki3 { get; set; }
        public Raigeki api_raigeki { get; set; }
    }
}
