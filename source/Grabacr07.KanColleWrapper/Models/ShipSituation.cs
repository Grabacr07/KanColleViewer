using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	[Flags]
	public enum ShipSituation
	{
		None = 0,
		//Sortie = 1,
		Repair = 1 << 1,
		Evacuation = 1 << 2,
		Tow = 1 << 3,
		//ModerateDamaged = 1 << 4,
		HeavilyDamaged = 1 << 5,
		DamageControlled = 1 << 6,
	}
}