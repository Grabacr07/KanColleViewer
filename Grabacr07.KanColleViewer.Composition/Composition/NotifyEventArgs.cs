using System;
using System.Collections.Generic;
using System.Linq;

namespace Grabacr07.KanColleViewer.Composition
{
	public class NotifyEventArgs : EventArgs
	{
		public string Title { get; private set; }

		public string Message { get; private set; }

		public Action Activated { get; set; }

		public Action<Exception> Failed { get; set; }

		public NotifyEventArgs(string title, string message)
		{
			this.Title = title;
			this.Message = message;
		}
	}
}
