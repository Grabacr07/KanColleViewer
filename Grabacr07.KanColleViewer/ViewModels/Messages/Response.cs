using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.ViewModels.Messages
{
	public class Processing
	{
		public bool IsSuccess { get; private set; }
		public Exception Exception { get; private set; }

		public Processing()
		{
			this.IsSuccess = true;
		}

		public Processing(Exception ex)
		{
			this.IsSuccess = false;
			this.Exception = ex;
		} 
	}

	public class Processing<T> : Processing
	{
		public T Result { get; private set; }

		public Processing(Exception ex) : base(ex) { }

		public Processing(T data)
		{
			this.Result = data;
		}
	}
}
