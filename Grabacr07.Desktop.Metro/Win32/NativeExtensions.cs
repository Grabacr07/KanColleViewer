using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.Desktop.Metro.Win32
{
	internal static class NativeExtensions
	{
		public static ushort ToLoWord(this IntPtr dword)
		{
			return (ushort)((uint)dword & 0xffff);
		}

		public static ushort ToHiWord(this IntPtr dword)
		{
			return (ushort)((uint)dword >> 16);
		}
	}
}
