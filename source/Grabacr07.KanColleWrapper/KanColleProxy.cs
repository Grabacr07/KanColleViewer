using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Nekoxy;
using StatefulModel;

namespace Grabacr07.KanColleWrapper
{
	public partial class KanColleProxy
	{
		private readonly IConnectableObservable<Session> connectableSessionSource;
		private readonly IConnectableObservable<Session> apiSource;
		private readonly MultipleDisposable compositeDisposable;

		public IObservable<Session> SessionSource => this.connectableSessionSource.AsObservable();

		public IObservable<Session> ApiSessionSource => this.apiSource.AsObservable();

		#region UpstreamProxySettingsプロパティ

		private IProxySettings _UpstreamProxySettings;

		public IProxySettings UpstreamProxySettings
		{
			get { return this._UpstreamProxySettings; }
			set
			{
				this._UpstreamProxySettings = value;
				this.ApplyUpstreamProxySettings();
			}
		}

		#endregion

		public int ListeningPort { get; private set; } = 37564;

		public KanColleProxy()
		{
			this.compositeDisposable = new MultipleDisposable();

			this.connectableSessionSource = Observable
				.FromEvent<Action<Session>, Session>(
					action => action,
					h => HttpProxy.AfterSessionComplete += h,
					h => HttpProxy.AfterSessionComplete -= h)
				.Publish();

			this.apiSource = this.connectableSessionSource
				.Where(s => s.Request.PathAndQuery.StartsWith("/kcsapi"))
				.Where(s => s.Response.MimeType.Equals("text/plain"))
				#region .Do(debug)
#if DEBUG
.Do(session =>
				{
					Debug.WriteLine("==================================================");
					Debug.WriteLine("Nekoxy session: ");
					Debug.WriteLine(session);
					Debug.WriteLine("");
				})
#endif
			#endregion
				.Publish();
		}


		public void Startup(int proxy = 37564)
		{
			this.ListeningPort = proxy;
			
			HttpProxy.Startup(proxy, false, false);
			this.ApplyUpstreamProxySettings();

			this.compositeDisposable.Add(this.connectableSessionSource.Connect());
			this.compositeDisposable.Add(this.apiSource.Connect());
		}

		public void Shutdown()
		{
			this.compositeDisposable.Dispose();
			HttpProxy.Shutdown();
		}

		/// <summary>
		/// 上流プロキシを設定
		/// </summary>
		private void ApplyUpstreamProxySettings()
		{
			switch (this.UpstreamProxySettings?.Type)
			{
				case ProxyType.DirectAccess:
					HttpProxy.UpstreamProxyConfig = new ProxyConfig(ProxyConfigType.DirectAccess);
					break;
				case ProxyType.SystemProxy:
					HttpProxy.UpstreamProxyConfig = new ProxyConfig(ProxyConfigType.SystemProxy);
					break;
				case ProxyType.SpecificProxy:
					HttpProxy.UpstreamProxyConfig = new ProxyConfig(ProxyConfigType.SpecificProxy, this.UpstreamProxySettings.HttpHost, this.UpstreamProxySettings.HttpPort);
					break;
				default:
					//UpstreamProxySettings == null は SystemProxy使用とみなす
					HttpProxy.UpstreamProxyConfig = new ProxyConfig(ProxyConfigType.SystemProxy);
					break;
			}
		}
	}
}
