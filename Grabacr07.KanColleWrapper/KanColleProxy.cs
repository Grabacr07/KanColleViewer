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
		private readonly SessionStateHandler setUpstreamProxy;

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
			this.setUpstreamProxy = new SessionStateHandler(SetUpstreamProxyHandler);
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
			FiddlerApplication.BeforeRequest += this.setUpstreamProxy;
			SetIESettings("localhost:" + proxy);

			this.compositeDisposable.Add(this.connectableSessionSource.Connect());
			this.compositeDisposable.Add(this.apiSource.Connect());
		}

		public void Shutdown()
		{
			this.compositeDisposable.Dispose();

			// プロキシを通すための処理を削除する
			FiddlerApplication.BeforeRequest -= this.setUpstreamProxy;
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


		/// <summary>
		/// Fiddlerからリクエストが送られる際に使用されるプロキシサーバーのホスト名を取得または設定します。
		/// </summary>
		public string UpstreamProxyHost;

		/// <summary>
		/// Fiddlerからリクエストが送られる際に使用されるプロキシサーバーのポート番号を取得または設定します。
		/// </summary>
		public UInt16 UpstreamProxyPort;

		/// <summary>
		/// リクエスト時にプロキシサーバーを経由するかどうかを取得または設定します。
		/// </summary>
		public bool UseProxyOnConnect;

		/// <summary>
		/// SSLリクエスト時にプロキシサーバーを経由するかどうかを取得または設定します。
		/// </summary>
		public bool UseProxyOnSSLConnect;


		/// <summary>
		/// Fiddlerからのリクエスト発行時にプロキシを挟む設定を行います。
		/// </summary>
		/// <param name="requestingSession">通信を行おうとしているセッション</param>
		private void SetUpstreamProxyHandler(Session requestingSession)
		{
			bool isHostValid = (this.UpstreamProxyHost != null) && (this.UpstreamProxyHost.Length > 0);
			bool useGateway;
			string gateway;

			// 「http://www.dmm.com:433/」の場合もあり、これはSession.isHTTPSでは判定できない。
			if (IsSessionSSL(requestingSession))
			{
				useGateway = isHostValid && this.UseProxyOnSSLConnect;
			}
			else
			{
				useGateway = isHostValid && this.UseProxyOnConnect;
			}

			if (useGateway)
			{
				if (this.UpstreamProxyHost.Contains(":"))
				{
					// IPv6アドレスをプロキシホストにした場合はホストアドレス部分を[]で囲う形式にする。
					gateway = String.Format("[{0}]:{1}", this.UpstreamProxyHost, this.UpstreamProxyPort);
				}
				else
				{
					gateway = String.Format("{0}:{1}", this.UpstreamProxyHost, this.UpstreamProxyPort);
				}

				requestingSession["X-OverrideGateway"] = gateway;
			}
		}

		/// <summary>
		/// セッションがSSL接続を使用しているかどうかを返します。
		/// </summary>
		/// <param name="session">セッション</param>
		/// <returns>セッションがSSL接続を使用する場合はtrue、そうでない場合はfalseを返します。</returns>
		internal static bool IsSessionSSL(Session session)
		{
			string uri = session.fullUrl;
			return (uri.Substring(0, 6) == "https:") || (uri.Contains(":443"));
		}
	}
}
