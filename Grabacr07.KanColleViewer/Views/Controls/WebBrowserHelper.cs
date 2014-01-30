using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Grabacr07.KanColleViewer.Views.Controls
{
	public class WebBrowserHelper
	{
		#region ScriptErrorsSuppressed 添付プロパティ

		public static readonly DependencyProperty ScriptErrorsSuppressedProperty =
			DependencyProperty.RegisterAttached("ScriptErrorsSuppressed", typeof(bool), typeof(WebBrowserHelper), new PropertyMetadata(default(bool), ScriptErrorsSuppressedChangedCallback));

		public static void SetScriptErrorsSuppressed(WebBrowser browser, bool value)
		{
			browser.SetValue(ScriptErrorsSuppressedProperty, value);
		}

		public static bool GetScriptErrorsSuppressed(WebBrowser browser)
		{
			return (bool)browser.GetValue(ScriptErrorsSuppressedProperty);
		}

		private static void ScriptErrorsSuppressedChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var browser = d as WebBrowser;
			if (browser == null) return;
			if (!(e.NewValue is bool)) return;

			try
			{
				var axIWebBrowser2 = typeof(WebBrowser).GetProperty("AxIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
				if (axIWebBrowser2 == null) return;

				var comObj = axIWebBrowser2.GetValue(browser, null);
				if (comObj == null) return;

				comObj.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, comObj, new[] { e.NewValue, });
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		#endregion
	}
}
