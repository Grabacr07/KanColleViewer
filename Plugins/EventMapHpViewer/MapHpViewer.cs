using System.ComponentModel.Composition;
using EventMapHpViewer.Models;
using EventMapHpViewer.ViewModels;
using Grabacr07.KanColleViewer.Composition;

namespace EventMapHpViewer
{
    [Export(typeof(IToolPlugin))]
    [ExportMetadata("Title", "MapHPViewer")]
    [ExportMetadata("Description", "Map HPを表示します。")]
    [ExportMetadata("Version", "2.2.0")]
    [ExportMetadata("Author", "@veigr")]
    public class MapHpViewer : IToolPlugin
    {
        private readonly ToolViewModel _vm = new ToolViewModel(new MapInfoProxy());

        public object GetToolView()
        {
            return new ToolView {DataContext = this._vm};
        }

        public string ToolName
        {
            get { return "MapHP"; }
        }

        public object GetSettingsView()
        {
            return null;
        }
    }
}
