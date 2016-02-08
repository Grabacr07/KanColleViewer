namespace Grabacr07.KanColleWrapper.Models.Updater
{
	// ReSharper disable InconsistentNaming
	public class kcvapi_version
	{
		public string api_version { get; set; }
		public string api_url { get; set; }
		public string selected_culture { get; set; }
		public kcvapi_version_component[] components { get; set; }
	}

	public class kcvapi_version_component
	{
		public string type { get; set; }
		public string version { get; set; }
		public string url { get; set; }
		public string installer { get; set; }
	}
}