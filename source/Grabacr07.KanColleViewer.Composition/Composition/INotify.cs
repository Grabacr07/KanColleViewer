using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	/// <summary>
	/// ユーザーへの通知のためのメンバーを公開します。
	/// </summary>
	public interface INotify
	{
		string Type { get; }

		string Header { get; }

		string Body { get; }

		Action Activated { get; }

		Action<Exception> Failed { get; }
	}
}
