using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Shell;

namespace Grabacr07.KanColleViewer.Composition
{
	/// <summary>
	/// タスク バーのプログレス インジケーターに状態を報告するためのメンバーを公開します。
	/// このインターフェイスは、KanColleViewer プラグインのコントラクト型です。
	/// </summary>
	[PluginFeature]
	public interface ITaskbarProgress
	{
		/// <summary>
		/// この機能をシステムが識別するための ID を取得します。
		/// </summary>
		string Id { get; }

		/// <summary>
		/// この機能をユーザーが選択するときに識別するための名前を取得します。
		/// </summary>
		string DisplayName { get; }

		/// <summary>
		/// プログレス インジケーターに報告する現在の状態を取得します。
		/// </summary>
		TaskbarItemProgressState State { get; }

		/// <summary>
		/// プログレス インジケーターに報告する現在の値を取得します。
		/// </summary>
		double Value { get; }

		/// <summary>
		/// <see cref="State"/> または <see cref="Value"/> が変更されたときに発生します。
		/// </summary>
		event EventHandler Updated;
	}
}
