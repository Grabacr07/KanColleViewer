using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper
{
	public interface IProxySettings
	{
		ProxyType Type { get; }

		string Host { get; }

		ushort Port { get; }
	}
}
