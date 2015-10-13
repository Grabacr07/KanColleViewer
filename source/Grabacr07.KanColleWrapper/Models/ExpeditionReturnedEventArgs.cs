using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	public class ExpeditionReturnedEventArgs
	{
		public string FleetName { get; }

		internal ExpeditionReturnedEventArgs(string fleetName)
		{
			this.FleetName = fleetName;
		}
	}
}
