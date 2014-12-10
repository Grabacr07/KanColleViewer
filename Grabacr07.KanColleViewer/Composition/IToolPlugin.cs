
namespace Grabacr07.KanColleViewer.Composition
{
	/// <summary>
	/// プラグインの主機能を [ツール] タブに表示させるためのメンバーを公開します。
	/// </summary>
	public interface IToolPlugin : IPlugin
	{
		string ToolName { get; }

		object GetToolView();
	}
}
