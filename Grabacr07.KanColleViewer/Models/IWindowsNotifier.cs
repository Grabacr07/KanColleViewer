using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Models
{
	/// <summary>
	/// Windows の通知機能を公開します。
	/// </summary>
	public interface IWindowsNotifier : IDisposable
	{
		void Initialize();
		void Show(string header, string body, Action activated, Action<Exception> failed = null);
	}
}
