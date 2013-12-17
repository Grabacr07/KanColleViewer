using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Model
{
	public class ThemeService
	{
		#region singleton members

		private static readonly ThemeService current = new ThemeService();

		public static ThemeService Current
		{
			get { return current; }
		}

		#endregion

		private ThemeService() { }

		
	}
}
