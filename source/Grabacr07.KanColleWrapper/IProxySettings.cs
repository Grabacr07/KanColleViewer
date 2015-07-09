using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper
{
	public interface IProxySettings
	{
		ProxySettingType SettingType { get; set; }

		string Host { get; set; }

		ushort Port { get; set; }
	}
}
