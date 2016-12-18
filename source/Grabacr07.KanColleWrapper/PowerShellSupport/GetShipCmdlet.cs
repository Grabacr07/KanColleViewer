using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace Grabacr07.KanColleWrapper.PowerShellSupport
{
	[Cmdlet(VerbsCommon.Get, "Ship")]
	public class GetShipCmdlet : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			foreach (var ship in KanColleClient.Current.Homeport.Organization.Ships.Values)
			{
				this.WriteObject(ship);
			}
		}
	}
}
