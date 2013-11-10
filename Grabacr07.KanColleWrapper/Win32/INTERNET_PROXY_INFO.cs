using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Win32
{
	// ReSharper disable InconsistentNaming
	internal struct INTERNET_PROXY_INFO
	{
		public int dwAccessType;
		public IntPtr proxy;
		public IntPtr proxyBypass;
	}
	// ReSharper restore InconsistentNaming
}
