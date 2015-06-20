using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper
{
	public interface IProxySettings
	{
		bool IsEnabled { get; set; }

		bool IsEnabledOnSSL { get; set; }

		string Host { get; set; }

		ushort Port { get; set; }

		bool IsAuthRequired { get; set; }

		string Username { get; set; }

		string Password { get; set; }
	}
}
