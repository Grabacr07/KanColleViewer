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
		private readonly CompositeDisposable compositeDisposable;

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
				if (value == null)
				{
					//UpstreamProxySettings == null は SystemProxy使用とみなす
					HttpProxy.IsEnableUpstreamProxy = false;
					HttpProxy.UpstreamProxyHost = null;
					return;
				}
				HttpProxy.IsEnableUpstreamProxy = value.Type == ProxyType.SpecificProxy;
				//Host指定がない場合、HTTPはDirectAccessとなる
				HttpProxy.UpstreamProxyHost = string.IsNullOrWhiteSpace(value.Host) ? null : value.Host;
				HttpProxy.UpstreamProxyPort = value.Port;
			}
		}

		#endregion


		public KanColleProxy()
		{
			this.compositeDisposable = new CompositeDisposable();

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
			//UpstreamProxySettings == null は SystemProxy使用とみなす
			var isSetIEProxySettings = this.UpstreamProxySettings == null || this.UpstreamProxySettings.Type != ProxyType.DirectAccess;
			HttpProxy.Startup(proxy, false, isSetIEProxySettings);

			//プロキシを使用しない場合、HTTPだけNekoxyを通し、後は直アクセス
			if (!isSetIEProxySettings)
				WinInetUtil.SetProxyInProcess("http=localhost:" + proxy, "local");

			this.compositeDisposable.Add(this.connectableSessionSource.Connect());
			this.compositeDisposable.Add(this.apiSource.Connect());
		}

		public void Shutdown()
		{
			this.compositeDisposable.Dispose();
			HttpProxy.Shutdown();
		}
	}
}
