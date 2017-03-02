using System;
using System.Linq;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public static class ShipSpeedExtensions
	{
		public static string ToDisplayString(this ShipSpeed? speed) => ToDisplayString(speed ?? ShipSpeed.Immovable);

		public static string ToDisplayString(this ShipSpeed speed)
		{
			switch (speed)
			{
				case ShipSpeed.Fastest:
					return "最速";
				case ShipSpeed.Faster:
					return "高速+";
				case ShipSpeed.Fast:
					return "高速";
				case ShipSpeed.Slow:
					return "低速";
				default:
					return "";
			}
		}
	}
}
