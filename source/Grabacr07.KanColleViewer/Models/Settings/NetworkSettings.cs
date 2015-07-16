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

			#region IProxySettings members

			ProxyType IProxySettings.Type => Type.Value;

			string IProxySettings.Host => Host.Value;

			ushort IProxySettings.Port => Port.Value;

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
