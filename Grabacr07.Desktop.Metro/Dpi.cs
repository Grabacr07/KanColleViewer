using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.Desktop.Metro
{
	/// <summary>
	/// モニターの DPI (dots per inch) を表します。
	/// </summary>
	public struct Dpi
	{
		public static readonly Dpi Default = new Dpi(96, 96);

		public uint X { get; set; }
		public uint Y { get; set; }

		public double ScaleX
		{
			get { return this.X / (double)Default.X; }
		}

		public double ScaleY
		{
			get { return this.Y / (double)Default.Y; }
		}

		public Dpi(uint x, uint y)
			: this()
		{
			this.X = x;
			this.Y = y;
		}

		public static bool operator ==(Dpi dpi1, Dpi dpi2)
		{
			return dpi1.X == dpi2.X && dpi1.Y == dpi2.Y;
		}

		public static bool operator !=(Dpi dpi1, Dpi dpi2)
		{
			return !(dpi1 == dpi2);
		}

		public bool Equals(Dpi other)
		{
			return this.X == other.X && this.Y == other.Y;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is Dpi && this.Equals((Dpi)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((int)this.X * 397) ^ (int)this.Y;
			}
		}
	}
}
