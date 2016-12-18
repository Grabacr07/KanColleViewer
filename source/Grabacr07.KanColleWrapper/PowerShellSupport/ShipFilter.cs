using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleWrapper.PowerShellSupport
{
	public class ShipFilter : PowerShellHost
	{
		public EventHandler<Ship[]> FilterRequested;

		protected override Task<InvocationResult> HandleResult(PSDataCollection<PSObject> results)
		{
			return Task.Run(() =>
			{
				var ships = new List<Ship>();
				var notShips = new List<PSObject>();

				foreach (var result in results)
				{
					var ship = result.BaseObject as Ship;
					if (ship == null) notShips.Add(result);
					else ships.Add(ship);
				}

				this.FilterRequested?.Invoke(this, ships.ToArray());

				return this.OutString(notShips);
			});
		}
	}
}
