using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Models
{
    public interface IOrientationMode
	{
        OrientationType Current { get; }
        OrientationType[] SupportedModes { get; }

        OrientationType CurrentMode { get; set; }

        string CurrentModeString { get; }
	}
}
