using System;
using System.ComponentModel.Composition;
using BattleInfoPlugin.Models;
using BattleInfoPlugin.Models.Notifiers;
using BattleInfoPlugin.Models.Repositories;
using BattleInfoPlugin.ViewModels;
using BattleInfoPlugin.Views;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace BattleInfoPlugin
{
    [Export(typeof(IToolPlugin))]
    [ExportMetadata("Title", "BattleInfo")]
    [ExportMetadata("Description", "戦闘情報を表示します。")]
    [ExportMetadata("Version", "1.0.0")]
    [ExportMetadata("Author", "@veigr")]
    public class BattleInfoPlugin : IToolPlugin
    {
        private readonly ToolViewModel vm = new ToolViewModel(new BattleData(), new BattleEndNotifier());
        internal static readonly KcsResourceWriter ResourceWriter = new KcsResourceWriter();
        internal static kcsapi_start2 RawStart2 { get; private set; }

        public BattleInfoPlugin()
        {
            KanColleClient.Current.Proxy.api_start2.TryParse<kcsapi_start2>().Subscribe(x =>
            {
                RawStart2 = x.Data;
                Models.Repositories.Master.Current.Update(x.Data);
            });
        }

        public object GetToolView()
        {
            return new ToolView { DataContext = this.vm };
        }

        public string ToolName
        {
            get { return "BattleInfo"; }
        }

        public object GetSettingsView()
        {
            return null;
        }
    }
}
