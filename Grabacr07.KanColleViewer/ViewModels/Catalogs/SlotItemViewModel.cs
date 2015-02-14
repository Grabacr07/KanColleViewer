﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class SlotItemViewModel : ViewModel
	{
		private int count;
		public List<Counter> Ships { get; private set; }

		public class Counter
		{
			public Ship Ship { get; set; }
			public int Count { get; set; }

			public string ShipLevel
			{
				get { return "Lv." + this.Ship.Level; }
			}

			public string CountString
			{
				get { return this.Count == 1 ? "" : " x " + this.Count + " "; }
			}
			public string EquipStar { get; set; }
		}


		public SlotItemInfo SlotItem { get; set; }

		public int Count
		{
			get { return this.count; }
			set { this.count = this.Remainder = value; }
		}

		public int Remainder { get; set; }


		public SlotItemViewModel()
		{
			this.Ships = new List<Counter>();
		}


		public void AddShip(Ship ship, bool InEnhanceEquipment)
		{
			var target = this.Ships.FirstOrDefault(x => x.Ship.Id == ship.Id);
			if (target == null)
			{
				if (InEnhanceEquipment)
					this.Ships.Add(new Counter { Ship = ship, Count = 1, EquipStar = "★" });
				else
					this.Ships.Add(new Counter { Ship = ship, Count = 1 });
			}
			else
			{
				target.Count++;
			}

			this.Remainder--;
		}
	}
}
