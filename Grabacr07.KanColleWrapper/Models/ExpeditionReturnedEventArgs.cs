
namespace Grabacr07.KanColleWrapper.Models
{
	public class ExpeditionReturnedEventArgs
	{
		public string FleetName { get; private set; }

		internal ExpeditionReturnedEventArgs(string fleetName)
		{
			this.FleetName = fleetName;
		}
	}
}
