using System;
using System.Collections.Generic;
using System.Linq;

namespace Grabacr07.KanColleViewer.Composition
{
	public class NotifyEventArgs : EventArgs
	{
		public string Header { get; private set; }

		public string Body { get; private set; }

		public Action Activated { get; set; }

		public Action<Exception> Failed { get; set; }

		public NotifyEventArgs(string header, string body)
		{
			this.Header = header;
			this.Body = body;
		}
	}
}
