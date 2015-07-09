using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper
{
	public class KanColleError
	{
		public Exception Exception { get; private set; }

		public KanColleError(Exception ex)
		{
			this.Exception = ex;
		}
	}
}
