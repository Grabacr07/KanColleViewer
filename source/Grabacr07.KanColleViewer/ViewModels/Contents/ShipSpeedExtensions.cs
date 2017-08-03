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
					return "초고속";
				case ShipSpeed.Faster:
					return "고속+";
				case ShipSpeed.Fast:
					return "고속";
				case ShipSpeed.Slow:
					return "저속";
				default:
					return "";
			}
		}
	}
}
