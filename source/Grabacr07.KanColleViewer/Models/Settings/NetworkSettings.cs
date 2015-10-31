using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using MetroTrilithon.Serialization;

namespace Grabacr07.KanColleViewer.Models.Settings
{
	public class NetworkSettings
	{
		public class Proxy : IProxySettings
		{
			public static SerializableProperty<ProxyType> Type { get; }
				= new SerializableProperty<ProxyType>(GetKey(), Providers.Local, ProxyType.SystemProxy);

			public static SerializableProperty<string> Host { get; }
				= new SerializableProperty<string>(GetKey(), Providers.Local, null);

			public static SerializableProperty<ushort> Port { get; }
				= new SerializableProperty<ushort>(GetKey(), Providers.Local, 80);

			public static SerializableProperty<string> HttpsHost { get; }
				= new SerializableProperty<string>(GetKey(), Providers.Local, null);

			public static SerializableProperty<ushort> HttpsPort { get; }
				= new SerializableProperty<ushort>(GetKey(), Providers.Local, 443);

			public static SerializableProperty<string> FtpHost { get; }
				= new SerializableProperty<string>(GetKey(), Providers.Local, null);

			public static SerializableProperty<ushort> FtpPort { get; }
				= new SerializableProperty<ushort>(GetKey(), Providers.Local, 21);

			public static SerializableProperty<string> SocksHost { get; }
				= new SerializableProperty<string>(GetKey(), Providers.Local, null);

			public static SerializableProperty<ushort> SocksPort { get; }
				= new SerializableProperty<ushort>(GetKey(), Providers.Local, 1080);

			public static SerializableProperty<bool> IsUseHttpProxyForAllProtocols { get; }
				= new SerializableProperty<bool>(GetKey(), Providers.Local, true);

			#region IProxySettings members

			ProxyType IProxySettings.Type => Type.Value;

			string IProxySettings.HttpHost => Host.Value;

			ushort IProxySettings.HttpPort => Port.Value;

			string IProxySettings.HttpsHost => HttpsHost.Value;

			ushort IProxySettings.HttpsPort => HttpsPort.Value;

			string IProxySettings.FtpHost => FtpHost.Value;

			ushort IProxySettings.FtpPort => FtpPort.Value;

			string IProxySettings.SocksHost => SocksHost.Value;

			ushort IProxySettings.SocksPort => SocksPort.Value;

			bool IProxySettings.IsUseHttpProxyForAllProtocols => IsUseHttpProxyForAllProtocols;

			#endregion
			
			private static string GetKey([CallerMemberName] string propertyName = "")
			{
				return nameof(NetworkSettings) + "." + nameof(Proxy) + "." + propertyName;
			}
		}

		public static class LocalProxy
		{
			public static SerializableProperty<bool> IsEnabled { get; }
				= new SerializableProperty<bool>(GetKey(), Providers.Local, false);

			public static SerializableProperty<ushort> Port { get; }
				= new SerializableProperty<ushort>(GetKey(), Providers.Local, 37564);


			private static string GetKey([CallerMemberName] string propertyName = "")
			{
				return nameof(NetworkSettings) + "." + nameof(LocalProxy) + "." + propertyName;
			}
		}
	}
}
