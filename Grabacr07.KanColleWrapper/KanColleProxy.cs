using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Fiddler;
using Grabacr07.KanColleWrapper.Win32;
using Livet;

namespace Grabacr07.KanColleWrapper
{
	public class KanColleProxy
	{
		private readonly IConnectableObservable<Session> connectableSessionSource;
		private readonly IConnectableObservable<Session> apiSource;
		private readonly LivetCompositeDisposable compositeDisposable;
		private readonly SessionStateHandler _SetUpstreamProxy;

		public IObservable<Session> SessionSource
		{
			get { return this.connectableSessionSource.AsObservable(); }
		}

		public IObservable<Session> ApiSessionSource
		{
			get { return this.apiSource.AsObservable(); }
		}


		public KanColleProxy()
		{
			this._SetUpstreamProxy = new SessionStateHandler(SetUpstreamProxy);
			this.compositeDisposable = new LivetCompositeDisposable();

			this.connectableSessionSource = Observable
				.FromEvent<SessionStateHandler, Session>(
					action => new SessionStateHandler(action),
					h => FiddlerApplication.AfterSessionComplete += h,
					h => FiddlerApplication.AfterSessionComplete -= h)
				.Publish();

			this.apiSource = this.connectableSessionSource
				.Where(s => s.PathAndQuery.StartsWith("/kcsapi"))
				.Where(s => s.oResponse.MIMEType.Equals("text/plain"))
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
			FiddlerApplication.Startup(proxy, false, true);
			// プロキシを通すための処理を追加する
			FiddlerApplication.BeforeRequest += this._SetUpstreamProxy;
			SetIESettings("localhost:" + proxy);

			this.compositeDisposable.Add(this.connectableSessionSource.Connect());
			this.compositeDisposable.Add(this.apiSource.Connect());
		}

		public void Shutdown()
		{
			this.compositeDisposable.Dispose();

			// プロキシを通すための処理を削除する
			FiddlerApplication.BeforeRequest -= this._SetUpstreamProxy;
			FiddlerApplication.Shutdown();
		}


		private static void SetIESettings(string proxyUri)
		{
			// ReSharper disable InconsistentNaming
			const int INTERNET_OPTION_PROXY = 38;
			const int INTERNET_OPEN_TYPE_PROXY = 3;
			// ReSharper restore InconsistentNaming

			INTERNET_PROXY_INFO proxyInfo;
			proxyInfo.dwAccessType = INTERNET_OPEN_TYPE_PROXY;
			proxyInfo.proxy = Marshal.StringToHGlobalAnsi(proxyUri);
			proxyInfo.proxyBypass = Marshal.StringToHGlobalAnsi("local");

			var proxyInfoSize = Marshal.SizeOf(proxyInfo);
			var proxyInfoPtr = Marshal.AllocCoTaskMem(proxyInfoSize);
			Marshal.StructureToPtr(proxyInfo, proxyInfoPtr, true);

			NativeMethods.InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY, proxyInfoPtr, proxyInfoSize);
		}


		private string _UpstreamProxyHost;

		/// <summary>
		/// Fiddlerからリクエストが送られる際に使用されるプロキシサーバーのホスト名を取得または設定します。
		/// </summary>
		public string UpstreamProxyHost
		{
			get { return this._UpstreamProxyHost; }
			set { this._UpstreamProxyHost = value; }
		}


		private UInt16 _UpstreamProxyPort;

		/// <summary>
		/// Fiddlerからリクエストが送られる際に使用されるプロキシサーバーのポート番号を取得または設定します。
		/// </summary>
		public UInt16 UpstreamProxyPort
		{
			get { return this._UpstreamProxyPort; }
			set { this._UpstreamProxyPort = value; }
		}


		private void SetUpstreamProxy(Session RequestingSession)
		{
			if ((this.UpstreamProxyHost != null) && (this.UpstreamProxyHost.Length > 0))
			{
				// プロキシサーバーのホスト名が指定されている
				if (this.UpstreamProxyPort > 0)
				{
					// ポートも指定されている
					string RequestingURI = RequestingSession.fullUrl;
					string Gateway;
					// 「http://www.dmm.com:433/」の場合もある
					if ((RequestingURI.Substring(0, 6) == "https:") || (RequestingURI.IndexOf(":443") >= 0))
					{
						// SSL接続時は通さない
						Gateway = "DIRECT";
					}
					else
					{
						Gateway = String.Format("{0}:{1}", this.UpstreamProxyHost, this.UpstreamProxyPort);
					}

					RequestingSession["X-OverrideGateway"] = Gateway;
				}
			}
		}
	}
}
