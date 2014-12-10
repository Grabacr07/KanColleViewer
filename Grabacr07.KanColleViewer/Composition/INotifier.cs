using System;

namespace Grabacr07.KanColleViewer.Composition
{
	/// <summary>
	/// 通知機能を公開します。
	/// </summary>
	public interface INotifier : IPlugin, IDisposable
	{
		void Initialize();

		void Show(NotifyType type, string header, string body, Action activated, Action<Exception> failed = null);
	}
}
