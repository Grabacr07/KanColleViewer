using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.ViewModels
{
    public class KeyNameTabItemViewModel : TabItemViewModel
    {
        public override string Name { get; protected set; }
        public string Key { get; }

        public KeyNameTabItemViewModel(string Key, string TabName) : base()
        {
            this.Key = Key;
            this.Name = TabName;
        }

        public void SetTabName(string TabName)
        {
            this.Name = TabName;
        }
    }
}
