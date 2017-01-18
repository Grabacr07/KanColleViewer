using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	public class ResourceModel
	{
		public DateTime Date;

		public int Fuel;
		public int Ammo;
		public int Steel;
		public int Bauxite;

		public int RepairBucket;
		public int InstantConstruction;
		public int DevelopmentMaterial;
		public int ImprovementMaterial;

		public override string ToString()
		{
			return string.Format(
				"Date={0}, Fuel={1}, Ammo={2}, Steel={3}, Bauxite={4}, RepairBucket={5}, InstantConstruction={6}, DevelopmentMaterial={7}, ImprovementMaterial={8}",
				(object)this.Date, (object)this.Fuel, (object)this.Ammo, (object)this.Steel, (object)this.Bauxite,
				(object)this.RepairBucket, (object)this.InstantConstruction, (object)this.DevelopmentMaterial, (object)this.ImprovementMaterial
			);
		}
	}
}
