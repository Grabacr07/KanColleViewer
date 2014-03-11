using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Grabacr07.KanColleViewer.Models.Internal;

namespace Grabacr07.KanColleViewer.Models
{
	/// <summary>
	/// Windows の通知機能を公開します。
	/// </summary>
	public abstract class WindowsNotifier : IDisposable
	{
		public void Initialize()
		{
			try
			{
				abstractinit();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		public abstract void abstractinit();
		public abstract void Dispose();
		public abstract void Show(string header, string body, Action activated, Action<Exception> failed = null);

	}
}
