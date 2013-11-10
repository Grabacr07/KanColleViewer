using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Win32
{
	internal class NativeMethods
	{
		[DllImport("wininet.dll", SetLastError = true)]
		public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);
	}
}
