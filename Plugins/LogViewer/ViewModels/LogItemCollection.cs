using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Plugins.ViewModels
{
    public class LogItemCollection : IEnumerable
    {
        public List<string> Columns = new List<string>();
        public List<string[]> Rows = new List<string[]>();

        public IEnumerator GetEnumerator()
        {
            return this.Rows.GetEnumerator();
        }
    }
}