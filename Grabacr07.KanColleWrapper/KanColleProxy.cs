using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Nekoxy;
using Livet;

namespace Grabacr07.KanColleWrapper
{
	public partial class KanColleProxy
	{
		private readonly IConnectableObservable<Session> connectableSessionSource;
		private readonly IConnectableObservable<Session> apiSource;
		private readonly LivetCompositeDisposable compositeDisposable;

		public IObservable<Session> SessionSource
		{
			get { return this.connectableSessionSource.AsObservable(); }
		}

		public IObservable<Session> ApiSessionSource
		{
			get { return this.apiSource.AsObservable(); }
		}

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
					HttpProxy.UpstreamProxyHost = null;
					return;
				}
				HttpProxy.UpstreamProxyHost = value.IsEnabled ? value.Host : null;
				HttpProxy.UpstreamProxyPort = value.Port;
			}
		}

		#endregion


		public KanColleProxy()
		{
			this.compositeDisposable = new LivetCompositeDisposable();

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
					Debug.WriteLine("Fiddler session: ");
					Debug.WriteLine(session);
					Debug.WriteLine("");
				})
#endif
			#endregion
				.Publish();
		}


		public void Startup(int proxy = 37564)
		{
			HttpProxy.Startup(proxy);
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
