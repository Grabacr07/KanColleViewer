using System;
using System.Runtime.InteropServices;

namespace Grabacr07.KanColleWrapper.Win32
{
	internal class NativeMethods
	{
		[DllImport("wininet.dll", SetLastError = true)]
		public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);
	}
}
