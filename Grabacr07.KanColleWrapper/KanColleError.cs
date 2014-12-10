using System;

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
