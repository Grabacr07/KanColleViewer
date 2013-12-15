using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.Desktop.Metro.Controls
{
	public interface ITabItem
	{
		string Name { get; }
		int? Badge { get; }
		bool IsSelected { get; set; }
	}
}
