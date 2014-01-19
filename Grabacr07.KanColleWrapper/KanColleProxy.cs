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

		public IObservable<Session> SessionSource
		{
			get { return this.connectableSessionSource.AsObservable(); }
		}

		public IObservable<Session> ApiSessionSource
		{
			get { return this.apiSource.AsObservable(); }
		}

		/// <summary>
		/// Fiddler からリクエストが送られる際に使用されるプロキシサーバーのホスト名を取得または設定します。
		/// </summary>
		public string UpstreamProxyHost { get; set; }

		/// <summary>
		/// Fiddler からリクエストが送られる際に使用されるプロキシサーバーのポート番号を取得または設定します。
		/// </summary>
		public ushort UpstreamProxyPort { get; set; }

		/// <summary>
		/// リクエスト時にプロキシサーバーを経由するかどうかを取得または設定します。
		/// </summary>
		public bool UseProxyOnConnect { get; set; }

		/// <summary>
		/// SSL リクエスト時のみプロキシサーバーを経由するかどうかを取得または設定します。
		/// </summary>
		public bool UseProxyOnSSLConnect { get; set; }


		public KanColleProxy()
		{
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
			FiddlerApplication.BeforeRequest += this.SetUpstreamProxyHandler;

			SetIESettings("localhost:" + proxy);

			this.compositeDisposable.Add(this.connectableSessionSource.Connect());
			this.compositeDisposable.Add(this.apiSource.Connect());
		}

		public void Shutdown()
		{
			this.compositeDisposable.Dispose();

			FiddlerApplication.BeforeRequest -= this.SetUpstreamProxyHandler;
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
		/// Fiddler からのリクエスト発行時にプロキシを挟む設定を行います。
		/// </summary>
		/// <param name="requestingSession">通信を行おうとしているセッション。</param>
		private void SetUpstreamProxyHandler(Session requestingSession)
		{
			var useGateway = !string.IsNullOrEmpty(this.UpstreamProxyHost) && this.UseProxyOnConnect;
			if (!useGateway || (IsSessionSSL(requestingSession) && !this.UseProxyOnSSLConnect)) return;

			var gateway = this.UpstreamProxyHost.Contains(":")
				// IPv6 アドレスをプロキシホストにした場合はホストアドレス部分を [] で囲う形式にする。
				? string.Format("[{0}]:{1}", this.UpstreamProxyHost, this.UpstreamProxyPort)
				: string.Format("{0}:{1}", this.UpstreamProxyHost, this.UpstreamProxyPort);

			requestingSession["X-OverrideGateway"] = gateway;
		}

		/// <summary>
		/// セッションが SSL 接続を使用しているかどうかを返します。
		/// </summary>
		/// <param name="session">セッション。</param>
		/// <returns>セッションが SSL 接続を使用する場合は true、そうでない場合は false。</returns>
		internal static bool IsSessionSSL(Session session)
		{
			// 「http://www.dmm.com:433/」の場合もあり、これは Session.isHTTPS では判定できない
			return session.isHTTPS || session.fullUrl.StartsWith("https:") || session.fullUrl.Contains(":443");
		}
	}
}
