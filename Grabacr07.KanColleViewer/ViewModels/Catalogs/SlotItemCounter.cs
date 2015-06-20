using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class SlotItemCounter
	{
		// Key:   装備レベル (★)
		// Value: レベル別の装備数カウンター
		private readonly Dictionary<int, SlotItemCounterByLevel> itemsByLevel;

		public SlotItemInfo Target { get; private set; }

		public IReadOnlyCollection<SlotItemCounterByLevel> Levels
		{
			get { return this.itemsByLevel.OrderBy(x => x.Key).Select(x => x.Value).ToList(); }
		}

		public int Count
		{
			get { return this.itemsByLevel.Sum(x => x.Value.Count); }
		}


		public SlotItemCounter(SlotItemInfo target, IEnumerable<SlotItem> items)
		{
			this.Target = target;

			this.itemsByLevel = items
				.GroupBy(x => x.Level)
				.ToDictionary(x => x.Key, x => new SlotItemCounterByLevel { Level = x.Key, Count = x.Count(), });
		}

		public void AddShip(Ship ship, int itemLevel)
		{
			this.itemsByLevel[itemLevel].AddShip(ship);
		}
	}


	public class SlotItemCounterByLevel
	{
		// Key:   艦娘の ID
		// Value: 艦娘別の装備カウンター
		private readonly Dictionary<int, SlotItemCounterByShip> itemsByShip;
		private int count;

		public int Level { get; set; }

		public IReadOnlyCollection<SlotItemCounterByShip> Ships
		{
			get { return this.itemsByShip.Values.OrderByDescending(x => x.Ship.Level).ThenBy(x => x.Ship.SortNumber).ToList(); }
		}

		public int Count
		{
			get { return this.count; }
			set { this.count = this.Remainder = value; }
		}

		// 余り
		public int Remainder { get; private set; }


		public SlotItemCounterByLevel()
		{
			this.itemsByShip = new Dictionary<int, SlotItemCounterByShip>();
		}

		public void AddShip(Ship ship)
		{
			SlotItemCounterByShip target;
			if (this.itemsByShip.TryGetValue(ship.Id, out target))
			{
				target.Count++;
			}
			else
			{
				this.itemsByShip.Add(ship.Id, new SlotItemCounterByShip { Ship = ship, Count = 1 });
			}

			this.Remainder--;
		}
	}

	public class SlotItemCounterByShip
	{
		public Ship Ship { get; set; }

		public int Count { get; set; }

		public string ShipName
		{
			get { return this.Ship.Info.Name; }
		}

		public string ShipLevel
		{
			get { return "Lv." + this.Ship.Level; }
		}

		public string CountString
		{
			get { return this.Count == 1 ? "" : " x " + this.Count + " "; }
		}
	}
}
