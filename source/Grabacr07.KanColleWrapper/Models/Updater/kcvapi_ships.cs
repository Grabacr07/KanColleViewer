namespace Grabacr07.KanColleWrapper.Models.Updater
{
	// ReSharper disable InconsistentNaming
	public class kcvapi_ships
	{
		public string version { get; set; }
		public kcvapi_ships_ship[] ships;
	}

	public class kcvapi_ships_ship
	{
		public string name_ja { get; set; }
		public string name { get; set; }
	}
}