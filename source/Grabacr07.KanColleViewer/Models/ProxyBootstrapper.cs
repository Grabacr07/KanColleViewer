using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;

namespace Grabacr07.KanColleViewer.Models
{
	public enum ProxyBootstrapResult
	{
		None,

		Success,

		/// <summary>
		/// 10048
		/// </summary>
		WsaEAddrInUse,

		UnexpectedException,
	}

	public class ProxyBootstrapper
	{
		public int ListeningPort { get; private set; }

		public ProxyBootstrapResult Result { get; private set; }

		public Exception Exception { get; private set; }

		public ProxyBootstrapper()
		{
			this.Result = ProxyBootstrapResult.None;

			if (Settings.NetworkSettings.LocalProxy.Port < 1 || 65535 < Settings.NetworkSettings.LocalProxy.Port)
			{
				Settings.NetworkSettings.LocalProxy.Port.Value = Settings.NetworkSettings.LocalProxy.Port.Default;
			}

			KanColleClient.Current.Proxy.UpstreamProxySettings = new Settings.NetworkSettings.Proxy();
		}

		public void Try()
		{
			this.ListeningPort = Settings.NetworkSettings.LocalProxy.Port;

			try
			{
				if(Settings.NetworkSettings.LocalProxy.IsEnabled)
					KanColleClient.Current.Proxy.Startup(this.ListeningPort);
				else
					KanColleClient.Current.Proxy.Startup();

				this.Result = ProxyBootstrapResult.Success;
			}
			catch (SocketException ex)
			{
				// 参照: Windows ソケットのエラー コード、値、および意味 https://support.microsoft.com/en-us/kb/819124/ja
				if (ex.ErrorCode != 10048) throw;

				this.Result = ProxyBootstrapResult.WsaEAddrInUse;
				this.Exception = ex;
			}
			catch (Exception ex)
			{
				this.Result = ProxyBootstrapResult.UnexpectedException;
				this.Exception = ex;
				Application.TelemetryClient.TrackException(ex);
			}
		}

		public static void Shutdown()
		{
			KanColleClient.Current.Proxy.Shutdown();
		}
	}
}
