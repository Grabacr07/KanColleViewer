using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper
{
	public interface IProxySettings
	{
		ProxyType Type { get; }

		string HttpHost { get; }

		ushort HttpPort { get; }

		string HttpsHost { get; }

		ushort HttpsPort { get; }

		string FtpHost { get; }

		ushort FtpPort { get; }

		string SocksHost { get; }

		ushort SocksPort { get; }

		bool IsUseHttpProxyForAllProtocols { get; }
	}
}
