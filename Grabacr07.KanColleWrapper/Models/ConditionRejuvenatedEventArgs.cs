using System;

namespace Grabacr07.KanColleWrapper.Models
{
	public class ConditionRejuvenatedEventArgs : EventArgs
	{
		public string FleetName { get; private set; }

		public int MinCondition { get; private set; }

		public ConditionRejuvenatedEventArgs(string fleetName, int minCondtion)
		{
			this.FleetName = fleetName;
			this.MinCondition = minCondtion;	
		}
	}
}