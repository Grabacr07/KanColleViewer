using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiddler;
using Grabacr07.KanColleWrapper;

namespace BattleInfoPlugin.Models.Repositories
{
    public class KcsResourceWriter
    {
        public KcsResourceWriter()
        {
            var resources = KanColleClient.Current.Proxy.SessionSource
                .Where(s => s.PathAndQuery.StartsWith("/kcs/resources/swf/map"));

            resources
                .Where(s => s.NeedDecode())
                .Where(s => s.utilDecodeResponse(true)) //chunkedはDecodeが必要っぽい
                .Subscribe(s => s.SaveResponseBody(s.GetSaveFilePath()));

            resources
                .Where(s => !s.NeedDecode())
                .Subscribe(s => s.SaveResponseBody(s.GetSaveFilePath()));
        }
    }

    static class KcsResourceWriterExtensions
    {
        public static string GetSaveFilePath(this Session session)
        {
            return Properties.Settings.Default.CacheDirPath
                   + session.PathAndQuery.Split('?').First();
        }

        public static bool NeedDecode(this Session session)
        {
            return session.oResponse.headers
                    .Where(h => h.Name == "Transfer-Encoding")
                    .Any(h => h.Value == "chunked");
        }
    }
}
