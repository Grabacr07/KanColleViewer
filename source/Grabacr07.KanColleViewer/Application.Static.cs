using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CefSharp;
using CefSharp.Wpf;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Models.Cef;
using Grabacr07.KanColleViewer.ViewModels;
using Grabacr07.KanColleWrapper;
using MetroTrilithon.Desktop;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Grabacr07.KanColleViewer
{
	partial class Application
	{
		static Application()
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, args) => ReportException(sender, args.ExceptionObject as Exception);

			TelemetryClient = new TelemetryClient();
			TelemetryClient.Context.Session.Id = Guid.NewGuid().ToString();
			TelemetryClient.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
			TelemetryClient.Context.Component.Version = ProductInfo.VersionString;
#if DEBUG
			TelemetryClient.Context.User.Id = Environment.UserName;
#endif
			SetInstrumentationKey();
		}

		public static TelemetryClient TelemetryClient { get; }

		public static Application Instance => Current as Application;

		static partial void SetInstrumentationKey();

		private static void CefInitialize()
		{
			var cefSettings = new CefSettings()
			{
				CachePath = CefBridge.CachePath,
			};
			cefSettings.CefCommandLineArgs.Add("proxy-server", Models.Settings.NetworkSettings.LocalProxySettingsString);
			//cefSettings.CefCommandLineArgs.Add("disable-webgl", "1");
			CefSharpSettings.SubprocessExitIfParentProcessClosed = true;

			Cef.EnableHighDPISupport();
			Cef.Initialize(cefSettings);
		}

		/// <summary>
		/// <see cref="ProxyBootstrapper"/> を使用し、<see cref="KanColleProxy"/> を起動することを試みます。
		/// 必要に応じて、ユーザーに操作を求めるダイアログを表示します。
		/// </summary>
		/// <returns><see cref="KanColleProxy"/> の起動に成功した場合は true、それ以外の場合は false。</returns>
		private static bool BootstrapProxy()
		{
			var bootstrapper = new ProxyBootstrapper();
			bootstrapper.Try();

			if (bootstrapper.Result == ProxyBootstrapResult.Success)
			{
				return true;
			}

			var vmodel = new ProxyBootstrapperViewModel(bootstrapper) { Title = ProductInfo.Title, };
			var window = new Views.Settings.ProxyBootstrapper { DataContext = vmodel, };
			window.ShowDialog();

			return vmodel.DialogResult;
		}

		private static void ReportException(object sender, Exception exception)
		{
			var now = DateTimeOffset.Now;
			var path = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				ProductInfo.Company,
				ProductInfo.Product,
				"ErrorReports",
				$"ErrorReport-{now:yyyyMMdd-HHmmss}-{now.Millisecond:000}.log");

			var message = $@"*** Error Report ***
{ProductInfo.Product} ver.{ProductInfo.VersionString}
{DateTimeOffset.Now}

{new SystemEnvironment()}

Sender:    {sender.GetType().FullName}
Exception: {exception.GetType().FullName}

{exception}
";
			try
			{
				// ReSharper disable once AssignNullToNotNullAttribute
				Directory.CreateDirectory(Path.GetDirectoryName(path));
				File.AppendAllText(path, message);

				TelemetryClient.TrackException(exception);
				TelemetryClient.TrackTrace(message, SeverityLevel.Critical);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			Current.Shutdown();
		}
	}
}
